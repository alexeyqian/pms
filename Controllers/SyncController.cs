﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.Data;
using PMS.Migrations;
using PMS.Models;
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
        private readonly IConfiguration _configuration;
        private readonly PMSDBContext _context;       
        private readonly string[] _ids;

        public SyncController(IConfiguration configuration, PMSDBContext context)
        {
            _configuration = configuration;
            _context = context;            
        }

        public async Task<IActionResult> Index()
        {  
            return View();
        }

        public async Task<string> Sync(bool clearAll = false)
        {
            // read data from data.json
            string fileName = "data.json";
            string jsonString = System.IO.File.ReadAllText(fileName);
            var bugsPartial = JsonConvert.DeserializeObject<List<Bug>>(jsonString,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }).ToList();

            // clean existing data
            if(clearAll)
                _context.Database.ExecuteSqlRaw("TRUNCATE TABLE [Bug]");

            // sync data from excel/json file
            foreach (var b in bugsPartial)
            {
                var bug = await _context.Bug.Where(r => r.NO == b.NO).FirstOrDefaultAsync();                

                if (bug == null) // create new record
                {
                    bug = new Bug();
                    bug.NO = b.NO;
                    UpdateFields(b, bug);
                    _context.Add(bug);
                }
                else // data already exist
                {
                    if (!string.IsNullOrEmpty(bug.StatusInVS) && bug.StatusInVS.ToLower() == "closed") continue; // ignore closed bugs
                    if (bug.SyncedOn.HasValue && DateTime.Now.Subtract(bug.SyncedOn.Value).Hours <= 2)
                        continue; // ignore recent synced data
                    UpdateFields(b, bug);
                    _context.Update(bug);
                }
            }            
            await _context.SaveChangesAsync();

            // sync data from DevOps
            var syncHelper = new SyncHelper(_configuration, _context);
            foreach (var bug in bugsPartial)
            {
                //if (!string.IsNullOrEmpty(bug.StatusInVS) && bug.StatusInVS.ToLower() == "closed") continue; // ignore closed bugs
                if (bug.SyncedOn.HasValue && DateTime.Now.Subtract(bug.SyncedOn.Value).Hours <= 2)
                    continue; // ignore recent synced data
                await syncHelper.SyncBug(bug.NO);
            }                

            return "OK";
        }

        private void UpdateFields(Bug from, Bug to)
        {
            to.Status = from.Status;
            to.Developer = from.Developer;
            to.StartedDate = from.StartedDate;
            to.FixedDate = from.FixedDate;
            to.ActualHours = from.ActualHours;
            to.Note = from.Note;
        }
    }
}
