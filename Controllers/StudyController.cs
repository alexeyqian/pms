using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PMS.Controllers
{
    public class StudyController : Controller
    {
        private readonly MyConfigration _config;
        public StudyController(IOptions<MyConfigration> config)
        {
            _config = config.Value;
        }


        public async Task<IActionResult> Index()
        {
            string errorMessage = string.Empty;
            string responseMessage = string.Empty;
            try
            {
                ViewData["ResponseMessage"] = await GetProjects();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.ToString();
            }

            return View();
        }

        public async Task<string> GetProjects()
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

                string url = string.Format("https://dev.azure.com/{0}/_apis/projects", _config.VSOrg);
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
