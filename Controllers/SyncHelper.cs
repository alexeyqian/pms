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
        public string RepositoryId{get;set;}
        public int PullRequestId { get; set; }
        public PullRequestMeta(string url) {
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

        private async Task UpdateBugFields(PMS.Models.Bug bug, VSWorkItem workitem, bool isCreate)
        {
            if (isCreate)
            {
                bug.NO = workitem.id;
                bug.CreatedDate = workitem.fields.SystemCreatedDate;
            }
            bug.StatusInVS = workitem.fields.SystemState;
            //bug.Reason = workitem.fields.SystemReason;
            bug.Title = workitem.fields.SystemTitle;
            bug.Tags = workitem.fields.SystemTags;

            bug.CommentCount = workitem.fields.SystemCommentCount;
            var pullRequests = workitem.relations.Where(r => r.attributes.name == "Pull Request").ToList();
            if (pullRequests == null || pullRequests.Count() == 0)
                bug.PullRequestCount = 0;
            else
            {
                bug.PullRequestCount = pullRequests.Count();

                var prList = pullRequests.OrderBy(r => r.attributes.resourceCreatedDate).ToList();
                var pullRequestMeta = new PullRequestMeta(pullRequests[0].url);
                int firstPullRequestId = pullRequestMeta.PullRequestId;

                if (isCreate) // only need to fill at creating
                    bug.FirstPullRequestDate = pullRequests[0].attributes.resourceCreatedDate;

                string json = await GetPullRequest(pullRequestMeta);
                var pullrequestFullData = JsonConvert.DeserializeObject<VSPullRequest>(json, _jsonSettings);
                bug.FirstPullRequestStatus = pullrequestFullData.status;

                json = await GetCommitsOfPullRequest(pullRequestMeta);
                var commitsOfPullRequest = JsonConvert.DeserializeObject<VSCommitsOfPullRequest>(json, _jsonSettings);
                bug.FirstPullRequestCommitCount = commitsOfPullRequest.count;

                json = await GetThreadsOfPullRequest(pullRequestMeta);
                var threadsOfPullRequest = JsonConvert.DeserializeObject<VSThreadsOfPullRequest>(json, _jsonSettings);
                int pullRequestCommentCount = 0;
                foreach (var t in threadsOfPullRequest.value)
                {
                    // TODO:  filter out 'system' comments in future
                    pullRequestCommentCount += t.comments.Count();
                }
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

        private async Task<string> GetPullRequest(PullRequestMeta meta)
        {
            //GET https://dev.azure.com/{organization}/{project}/_apis/git/pullrequests/{pullrequest}?api-version={apiversion}
            //EX: https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/pullrequests/1206724?api-version=6.1-preview.3
            return await GetDataFromVS(string.Format("{0}/_apis/git/pullrequests/{1}", meta.ProjectId, meta.PullRequestId));
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

        private async Task<string> GetDataFromVS(string path)
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

                string url = string.Format("https://dev.azure.com/{0}/{1}", _config["MyConfiguration:VSOrg"], path);
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    result = await response.Content.ReadAsStringAsync();
                }
            }

            return result;
        }
    }
}
