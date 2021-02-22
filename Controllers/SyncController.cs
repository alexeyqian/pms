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
using System.Web;

namespace PMS.Controllers
{
    public class SyncController : Controller
    {        
        private readonly PMSDBContext _context;       
        private readonly string[] _ids;

        public SyncController(PMSDBContext context)
        {
            _context = context;
            //_ids = System.IO.File.ReadAllLines("bugids.txt");
        }

        public async Task<IActionResult> Index()
        {            
            //int bugId = 1937102; // TODO: remove hardcode.
            //var syncHelper = new SyncHelper(_context);
            //await syncHelper.SyncBug(bugId);

            return View();
        }

    }
}
