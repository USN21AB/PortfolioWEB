using FireSharp.Response;
using Microsoft.AspNetCore.Http;
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
using Firebase.Database;
using Firebase.Database.Query;

using Test1.Models;

namespace Portefolio_webApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirebaseDB firebase;
        //private ISession session;

        [BindProperty]
        public List<Innlegg> AlleInnlegg { get; set; }
     

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebase = new FirebaseDB();
        //   this.session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult LoggInnSide()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BrowseSide(string kategori)
        {
            if (string.IsNullOrEmpty(kategori))
            {
          
                AlleInnlegg = firebase.HentAlleInnlegg();
                TempData["valgtKnapp"] = "alt";
               // session.SetString("AlleInnlegg", "Hello my name is");

                ViewData["liste"] = AlleInnlegg;
            }
            else {

                var listen = firebase.HentAlleInnlegg(); 
                
                ViewData["liste"] = firebase.SorterAlleInnlegg(kategori, listen);
                TempData["valgtKnapp"] = kategori;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult ProfilSide()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SorterListe()
        {
            //asp-route-Type="Kunst" 
            //ViewData["liste"] = firebase.SorterAlleInnlegg("Kunst");
                   return RedirectToPage("BrowseSide"); 
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
