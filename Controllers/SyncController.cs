using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.Data;
using PMS.Migrations;
using PMS.VSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PMS.Controllers
{
    public class SyncController : Controller
    {
        // IMPORTANT: ALL API URL IS case sensitive!
        static string _apiVersion = "6.1-preview.3";
        static string _projectId = "f2b55896-e832-438d-9220-cbc08c545713";
        static string _repositoryId = "cb274bb6-3339-491a-804d-e2a4f615ad4b";
        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private readonly PMSDBContext _context;
        private readonly MyConfigration _config;
        private readonly string[] _ids;

        public SyncController(IOptions<MyConfigration> config, PMSDBContext context)
        {
            _config = config.Value;
            _context = context;

            _ids = System.IO.File.ReadAllLines("bugids.txt");
        }

        public async Task<IActionResult> Index()
        {
            string errorMessage = string.Empty;
            string responseMessage = string.Empty;
            string json = string.Empty;
            try
            {
                int bugId = 1937102;
                
                json = await GetWorkItem(bugId);
                var workitem = JsonConvert.DeserializeObject<VSWorkItem>(json, _jsonSettings);
                if (workitem.fields.SystemWorkItemType != "Bug") throw new Exception("Workitem type should be bug");

                var bug = await _context.Bug.FindAsync(bugId);
                if(bug == null) // create new record
                {
                    bug = new Models.Bug();
                    bug.Id = bug.NO = workitem.id;
                    bug.Title = workitem.fields.SystemTitle;
                    bug.Tags = workitem.fields.SystemTags;
                    bug.CreatedDate = workitem.fields.SystemCreatedDate;
                    bug.CommentCount = workitem.fields.SystemCommentCount;
                    var pullRequests = workitem.relations.Where(r => r.attributes.name == "Pull Request").ToList();
                    if (pullRequests == null || pullRequests.Count() == 0)
                        bug.PullRequestCount = 0;
                    else
                    {
                        bug.PullRequestCount = pullRequests.Count();
                        var prList = pullRequests.OrderBy(r => r.attributes.resourceCreatedDate).ToList();
                        int firstPullRequestId =  pullRequests[0].attributes.id;
                        bug.FirstPullRequestDate = pullRequests[0].attributes.resourceCreatedDate;

                        json = await GetPullRequest(firstPullRequestId);
                        var pullrequestFullData = JsonConvert.DeserializeObject<VSPullRequest>(json, _jsonSettings);
                        bug.FirstPullRequestStatus = pullrequestFullData.status;

                        json = await GetCommitsOfPullRequest(firstPullRequestId);
                        var commitsOfPullRequest = JsonConvert.DeserializeObject<VSCommitsOfPullRequest>(json, _jsonSettings);                        
                        bug.FirstPullRequestCommitCount = commitsOfPullRequest.count;

                        json = await GetThreadsOfPullRequest(firstPullRequestId);
                        var threadsOfPullRequest = JsonConvert.DeserializeObject<VSThreadsOfPullRequest>(json, _jsonSettings);

                        int pullRequestCommentCount = 0;
                        foreach(var t in threadsOfPullRequest.value)
                        {
                            pullRequestCommentCount += t.comments.Count();
                        }
                        bug.FirstPullRequestCommentCount = pullRequestCommentCount;
                    }

                    _context.Add(bug);
                    
                }
                else // update existing record
                {
                    // bug.xx = workitem.xx;

                    _context.Update(bug);
                }
                await _context.SaveChangesAsync();

                ViewData["WorkItem"] = workitem;

                json = await GetPullRequest(1206724);
                var pullrequest = JsonConvert.DeserializeObject<VSPullRequest>(json, _jsonSettings);
                ViewData["PullRequest"] = pullrequest;
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.ToString();
            }

            return View();
        }

        public async Task<string> GetProjects()
        {
            return await GetDataFromVS("_apis/projects");
        }

        public async Task<string> GetWorkItem(int id)
        {            
            return await GetDataFromVS(string.Format("_apis/wit/workitems/{0}?api-version={1}&$expand=all", id, _apiVersion));
        }

        public async Task<string> GetPullRequest(int id)
        {
            //https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/pullrequests/1206724?api-version=6.1-preview.3
            return await GetDataFromVS(string.Format("{0}/_apis/git/pullrequests/{1}", _projectId, id));
        }

        public async Task<string> GetCommitsOfPullRequest(int pullRequestId)
        {
            //GET https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version=6.0
            //https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/repositories/cb274bb6-3339-491a-804d-e2a4f615ad4b/pullRequests/1206724/commits
            return await GetDataFromVS(string.Format("{0}/_apis/git/repositories/{1}/pullRequests/{2}/commits", _projectId, _repositoryId, pullRequestId));
        }

        public async Task<string> GetThreadsOfPullRequest(int pullRequestId)
        {
            //GET https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/threads?api-version=6.0
            //https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/repositories/cb274bb6-3339-491a-804d-e2a4f615ad4b/pullRequests/1206724/threads
            return await GetDataFromVS(string.Format("{0}/_apis/git/repositories/{1}/pullRequests/{2}/threads", _projectId, _repositoryId, pullRequestId));
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
                            string.Format("{0}:{1}", "", _config.PAT))));

                string url = string.Format("https://dev.azure.com/{0}/{1}", _config.VSOrg, path);
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
