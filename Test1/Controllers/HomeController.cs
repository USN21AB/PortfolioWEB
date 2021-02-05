using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Test1.Models;

namespace Portefolio_webApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirebaseDB firebase;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebase = new FirebaseDB();
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BrowseSide()
        {
            ViewData["liste"] = firebase.HentAlleInnlegg();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

     
        public IActionResult SorterListe(string type)
        {
            ViewData["liste"] = firebase.SorterAlleInnlegg(type);

            return View(); 
        }
      
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
