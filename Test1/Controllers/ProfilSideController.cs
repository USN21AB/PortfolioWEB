using Microsoft.AspNetCore.Mvc;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Test1.Models;

namespace Portefolio_webApp.Controllers
{
    public class ProfilSideController : Controller
    {
        private readonly FirebaseDB firebase;
        [BindProperty]
        public CV CirVit { get; set; }

        [BindProperty]
        public Bruker Bruker { get; set; }

        public ProfilSideController()
        {
            firebase = new FirebaseDB();
        }
        public IActionResult ProfilSide()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CV()
        {
            CirVit = new CV();
            Bruker = new Bruker();

            CirVit.Utdanning.Add("2010 - 2020");
            CirVit.Utdanning.Add("Videregående Skole");
            CirVit.Utdanning.Add("Håvåsen Skole");

            CirVit.Utdanning.Add("2010 - 2020");
            CirVit.Utdanning.Add("Videregående Skole");
            CirVit.Utdanning.Add("Håvåsen Skole");

            CirVit.ArbeidsErfaring.Add("2019 - 2021");
            CirVit.ArbeidsErfaring.Add("Developer");
            CirVit.ArbeidsErfaring.Add("Tesla Inc.");
            CirVit.ArbeidsErfaring.Add("Working on developing better batteries for the Tesla X edtions");

            CirVit.ArbeidsErfaring.Add("2015 - 2019");
            CirVit.ArbeidsErfaring.Add("Headhunter");
            CirVit.ArbeidsErfaring.Add("Mekonomen");
            CirVit.ArbeidsErfaring.Add("I denne stillingen lette jeg etter kandidater for Mekonomen til div. stillinger som: Mekaniker, Dekkskifter, Dørmenn, Sekretærer o.l.");

            CirVit.Ferdigheter.Add("HTML5");
            CirVit.Ferdigheter.Add("90%");

            CirVit.Ferdigheter.Add("PHP");
            CirVit.Ferdigheter.Add("20%");

            CirVit.Ferdigheter.Add("C#");
            CirVit.Ferdigheter.Add("44%");

            CirVit.Språk.Add("English");
            CirVit.Språk.Add("98%");

            CirVit.Språk.Add("Russian");
            CirVit.Språk.Add("28%");

            Bruker.Navn = "Mary Jane";
            Bruker.Stilling = "Gardener";
            Bruker.Profilbilde = "https://images.unsplash.com/photo-1542103749-8ef59b94f47e?ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&ixlib=rb-1.2.1&auto=format&fit=crop&w=1350&q=80";
            Bruker.Epost = "Mary@hotmail.com";
            Bruker.Id = "-MTL4PwvI0ChIfEaZwEu";
          
            Debug.WriteLine("PROFIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIILLLLLLLLLLLLL");
            //firebase.RegistrerBruker(Bruker); 

            ViewData["Cv_Innhold"] = CirVit;
            ViewData["Bruker_Innhold"] = Bruker;
            return View(CirVit);

        }

        [HttpPost]
        public IActionResult CV(CV cv)
        {
            Bruker.CV = cv;
            Debug.WriteLine("PROFIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIILLLLLLLLLLLLL222222222222");

            if (ModelState.IsValid)
            {
                try
                {
                    firebase.OppdaterBruker(Bruker);
                    //ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                }
                catch (Exception ex)
                {
                  //  ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(cv);
        }

        public IActionResult Portefølje()
        {
            return View();
        }
       
        
    }
    
}
