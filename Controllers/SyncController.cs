using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
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
        static string _apiVersion = "6.1-preview.3";
        static string _projectId = "f2b55896-e832-438d-9220-cbc08c545713";
        
        private readonly MyConfigration _config;
        private readonly string[] _ids;

        public SyncController(IOptions<MyConfigration> config)
        {
            _config = config.Value;
            _ids = System.IO.File.ReadAllLines("bugids.txt");
        }

        public async Task<IActionResult> Index()
        {
            string errorMessage = string.Empty;
            string responseMessage = string.Empty;
            try
            {
                ViewData["ResponseMessage"] = await GetProjects();
                ViewData["WorkItem"] = await GetWorkItem("1937102");
                ViewData["PullRequest"] = await GetPullRequest("1206724");
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

        public async Task<string> GetWorkItem(string id)
        {            
            return await GetDataFromVS(string.Format("_apis/wit/workitems/{0}?api-version={1}&$expand=all", id, _apiVersion));
        }

        public async Task<string> GetPullRequest(string id)
        {
            //https://dev.azure.com/O365Exchange/f2b55896-e832-438d-9220-cbc08c545713/_apis/git/pullrequests/1206724?api-version=6.1-preview.3
            return await GetDataFromVS(string.Format("{0}/_apis/git/pullrequests/{1}", _projectId, id));
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
