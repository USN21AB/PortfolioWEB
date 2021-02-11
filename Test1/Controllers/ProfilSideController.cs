using Microsoft.AspNetCore.Mvc;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portefolio_webApp.Controllers
{
    public class ProfilSideController : Controller
    {
        public CV BrukerCV;
        public Bruker CVBruker;

        public IActionResult ProfilSide()
        {
            return View();
        }
        public IActionResult CV()
        {
            BrukerCV = new CV();
            CVBruker = new Bruker();

            BrukerCV.Utdanning.Add("2010 - 2020");
            BrukerCV.Utdanning.Add("Videregående Skole");
            BrukerCV.Utdanning.Add("Håvåsen Skole");

            BrukerCV.Utdanning.Add("2010 - 2020");
            BrukerCV.Utdanning.Add("Videregående Skole");
            BrukerCV.Utdanning.Add("Håvåsen Skole");

            BrukerCV.ArbeidsErfaring.Add("2019 - 2021");
            BrukerCV.ArbeidsErfaring.Add("Developer");
            BrukerCV.ArbeidsErfaring.Add("Tesla Inc.");
            BrukerCV.ArbeidsErfaring.Add("Working on developing better batteries for the Tesla X edtions");

            BrukerCV.ArbeidsErfaring.Add("2015 - 2019");
            BrukerCV.ArbeidsErfaring.Add("Headhunter");
            BrukerCV.ArbeidsErfaring.Add("Mekonomen");
            BrukerCV.ArbeidsErfaring.Add("I denne stillingen lette jeg etter kandidater for Mekonomen til div. stillinger som: Mekaniker, Dekkskifter, Dørmenn, Sekretærer o.l.");

            BrukerCV.Ferdigheter.Add("HTML5");
            BrukerCV.Ferdigheter.Add("90%");

            BrukerCV.Ferdigheter.Add("PHP");
            BrukerCV.Ferdigheter.Add("20%");

            BrukerCV.Ferdigheter.Add("C#");
            BrukerCV.Ferdigheter.Add("44%");

            BrukerCV.Språk.Add("English");
            BrukerCV.Språk.Add("98%");

            BrukerCV.Språk.Add("Russian");
            BrukerCV.Språk.Add("28%");

            CVBruker.Navn = "Mary Jane";
            CVBruker.Stilling = "Gardener";
            CVBruker.Profilbilde = "https://images.unsplash.com/photo-1542103749-8ef59b94f47e?ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&ixlib=rb-1.2.1&auto=format&fit=crop&w=1350&q=80";

            ViewData["Cv_Innhold"] = BrukerCV;
            ViewData["Bruker_Innhold"] = CVBruker;
            return View();
        }
        public IActionResult Portefølje()
        {
            return View();
        }
    }
    
}
