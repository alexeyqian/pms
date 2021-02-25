using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.Data;
using PMS.VSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace PMS.Controllers
{
    public class PullRequestMeta
    {
        public string ProjectId { get; set; }
        public string RepositoryId { get; set; }
        public int PullRequestId { get; set; }
        public PullRequestMeta(string url)
        {
            // url example: 
            //vstfs:///Git/PullRequestId/f2b55896-e832-438d-9220-cbc08c545713%2Fcb274bb6-3339-491a-804d-e2a4f615ad4b%2F1206724",
            var decoded = HttpUtility.UrlDecode(url);
            var cleaned = decoded.Replace("vstfs:///Git/PullRequestId/", string.Empty);
            var arr = cleaned.Split("/");
            ProjectId = arr[0];
            RepositoryId = arr[1];
            PullRequestId = int.Parse(arr[2]);
        }
    }

    public class SyncHelper
    {
        // IMPORTANT: ALL API URL IS case sensitive!
        static string _apiVersion = "6.1-preview.3";
        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private readonly PMSDBContext _context;
        private readonly IConfiguration _config;

        public SyncHelper(IConfiguration configuration, PMSDBContext context)
        {
            _config = configuration;
            _context = context;
        }

        public async Task SyncBug(int bugId)
        {
            string json = string.Empty;

            try
            {
                json = await GetWorkItem(bugId);
                var workitem = JsonConvert.DeserializeObject<VSWorkItem>(json, _jsonSettings);
                if (workitem.fields.SystemWorkItemType != "Bug") throw new Exception("Workitem type should be bug");

                var bug = await _context.Bug.Where(r => r.NO == bugId).FirstOrDefaultAsync();
                if (bug == null) // create new record
                {
                    bug = new Models.Bug();
                    await UpdateBugFields(bug, workitem, true);
                    _context.Add(bug);

                }
                else // update existing record
                {
                    await UpdateBugFields(bug, workitem, false);
                    _context.Update(bug);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // TODO: find Resolved Date
        private async Task UpdateBugFields(PMS.Models.Bug bug, VSWorkItem workitem, bool isCreate)
        {
            bug.NO = workitem.id;
            bug.CreatedDate = workitem.fields.SystemCreatedDate;

            bug.StatusInVS = workitem.fields.SystemState;
            //bug.Reason = workitem.fields.SystemReason;
            bug.Title = workitem.fields.SystemTitle;
            bug.Tags = workitem.fields.SystemTags;

            bug.CommentCount = workitem.fields.SystemCommentCount;

            // get workitem updates
            var workitemUpdatesUrl = workitem._links.workItemUpdates.href;
            string json = await GetWorkItemUpdates(workitemUpdatesUrl);
            var workitemUpdates = JsonConvert.DeserializeObject<VSWorkItemUpdates>(json, _jsonSettings);
            // if it's been resolved, then update the resoved date
            if (!string.IsNullOrEmpty(bug.StatusInVS) && (bug.StatusInVS.ToLower() == "closed" || bug.StatusInVS.ToLower() == "resolved"))
            {
                var resolvedItem = workitemUpdates.value.Where(r =>
                    r.fields != null
                    && r.fields.SystemState != null
                    && r.fields.SystemState.newValue != null
                    && r.fields.SystemState.newValue.ToLower() == "resolved")
                    .FirstOrDefault();
                if (resolvedItem != null
                    && resolvedItem.fields.MicrosoftVSTSCommonResolvedDate != null
                    && resolvedItem.fields.MicrosoftVSTSCommonResolvedDate.newValue != null)
                {
                    bug.ResovedDate = DateTime.Parse(resolvedItem.fields.MicrosoftVSTSCommonResolvedDate.newValue);
                    if (resolvedItem.fields.MicrosoftVSTSCommonResolvedReason != null
                        && !string.IsNullOrEmpty(resolvedItem.fields.MicrosoftVSTSCommonResolvedReason.newValue))
                    {
                        bug.ResolvedReason = resolvedItem.fields.MicrosoftVSTSCommonResolvedReason.newValue;
                    }
                }
            }

            var pullRequests = workitem.relations.Where(r => r.attributes.name == "Pull Request").ToList();
            if (pullRequests == null || pullRequests.Count() == 0)
                bug.PullRequestCount = 0;
            else
            {
                bug.PullRequestCount = pullRequests.Count();

                var prList = pullRequests.OrderBy(r => r.attributes.resourceCreatedDate).ToList();
                var pullRequestMeta = new PullRequestMeta(pullRequests[0].url);
                int firstPullRequestId = pullRequestMeta.PullRequestId;

                bug.FirstPullRequestDate = pullRequests[0].attributes.resourceCreatedDate;

                // Using first PR data as fixed data if PR exists.
                if (bug.FirstPullRequestDate.HasValue)
                    bug.FixedDate = bug.FirstPullRequestDate;

                json = await GetPullRequest(pullRequestMeta);
                var pullrequestFullData = JsonConvert.DeserializeObject<VSPullRequest>(json, _jsonSettings);

                //if(pullrequestFullData.status.ToLower() == "abandoned") // try to find non-abandend one
                //{
                //    string json = await GetPullRequest(pullRequestMeta);
                //    var pullrequestFullData = JsonConvert.DeserializeObject<VSPullRequest>(json, _jsonSettings);
                //}

                bug.FirstPullRequestStatus = pullrequestFullData.status;

                json = await GetIterationsOfPullRequest(pullRequestMeta);
                var iterationsOfPullRequest = JsonConvert.DeserializeObject<VSIterationsOfPullRequest>(json, _jsonSettings);
                bug.FirstPullRequestIterationCount = iterationsOfPullRequest.count;

                json = await GetCommitsOfPullRequest(pullRequestMeta);
                var commitsOfPullRequest = JsonConvert.DeserializeObject<VSCommitsOfPullRequest>(json, _jsonSettings);
                bug.FirstPullRequestCommitCount = commitsOfPullRequest.count;

                json = await GetThreadsOfPullRequest(pullRequestMeta);
                var threadsOfPullRequest = JsonConvert.DeserializeObject<VSThreadsOfPullRequest>(json, _jsonSettings);
                int pullRequestCommentCount = 0;

                var orderedThreadsValues = threadsOfPullRequest.value.OrderBy(t => t.publishedDate);
                DateTime? firstPRHumanCommentDate = null;
                foreach (var t in orderedThreadsValues)
                {
                    pullRequestCommentCount += t.comments.Where(c => c.commentType != "system").Count();

                    // get first non human comment date
                    if (firstPRHumanCommentDate == null)
                    {
                        var tempComment = t.comments.Where(c1 => c1.commentType != "system").OrderBy(c => c.publishedDate).FirstOrDefault();
                        if (tempComment != null)
                        {
                            firstPRHumanCommentDate = tempComment.publishedDate;
                        }
                    }
                }

                bug.FirstPullRequestCommentDate = firstPRHumanCommentDate;
                bug.FirstPullRequestCommentCount = pullRequestCommentCount;
            }
        }

        private async Task<string> GetProjects()
        {
            return await GetDataFromVS("_apis/projects");
        }

        private async Task<string> GetWorkItem(int id)
        {
            //GET https://dev.azure.com/{organization}/_apis/wit/workitems/{workitem}?api-version={apiversion}&$expand=all
            //EX: https://dev.azure.com/O365Exchange/_apis/wit/workitems/1937102?api-version=6.1-preview.3&$expand=all
            return await GetDataFromVS(string.Format("_apis/wit/workitems/{0}?api-version={1}&$expand=all", id, _apiVersion));
        }


        private async Task<string> GetWorkItemUpdates(string url)
        {
            return await GetDataFromVSUrl(url);
        }

        private async Task<string> GetPullRequest(PullRequestMeta meta)
        {
            //GET https://dev.azure.com/{organization}/{project}/_apis/git/pullrequests/{pullrequest}?api-version={apiversion}
            //EX: https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/pullrequests/1206724
            return await GetDataFromVS(string.Format("{0}/_apis/git/pullrequests/{1}", meta.ProjectId, meta.PullRequestId));
        }

        private async Task<string> GetIterationsOfPullRequest(PullRequestMeta meta)
        {
            //GET https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version={apiversion}
            //EX: https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/repositories/cb274bb6-3339-491a-804d-e2a4f615ad4b/pullRequests/1206724/iterations
            return await GetDataFromVS(string.Format("{0}/_apis/git/repositories/{1}/pullRequests/{2}/iterations",
                meta.ProjectId, meta.RepositoryId, meta.PullRequestId));
        }

        private async Task<string> GetCommitsOfPullRequest(PullRequestMeta meta)
        {
            //GET https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version={apiversion}
            //EX: https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/repositories/cb274bb6-3339-491a-804d-e2a4f615ad4b/pullRequests/1206724/commits
            return await GetDataFromVS(string.Format("{0}/_apis/git/repositories/{1}/pullRequests/{2}/commits",
                meta.ProjectId, meta.RepositoryId, meta.PullRequestId));
        }

        private async Task<string> GetThreadsOfPullRequest(PullRequestMeta meta)
        {
            //GET https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/threads?api-version={apiversion}
            //EX: https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/repositories/cb274bb6-3339-491a-804d-e2a4f615ad4b/pullRequests/1206724/threads
            return await GetDataFromVS(string.Format("{0}/_apis/git/repositories/{1}/pullRequests/{2}/threads",
                meta.ProjectId, meta.RepositoryId, meta.PullRequestId));
        }

        private async Task<string> GetDataFromVSUrl(string url)
        {
            string result = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "", _config["MyConfiguration:PAT"]))));

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    result = await response.Content.ReadAsStringAsync();
                }
            }

            return result;
        }

        private async Task<string> GetDataFromVS(string path)
        {
            string url = string.Format("https://dev.azure.com/{0}/{1}", _config["MyConfiguration:VSOrg"], path);

            return await GetDataFromVSUrl(url);
        }
    }
}
