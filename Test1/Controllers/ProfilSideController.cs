using Microsoft.AspNetCore.Mvc;
using Portefolio_webApp.Models;
using System.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Test1.Models;

namespace Portefolio_webApp.Controllers
{
    public class ProfilSideController : Controller
    {
        private readonly FirebaseDB firebase;

        public ProfilSideController()
        {
            firebase = new FirebaseDB();
        }

        [BindProperty]
        public CV CirVit { get; set; }

        [BindProperty]
        public Bruker Bruker { get; set; }

        /*
        private IHostingEnvironment Environment;

        public ProfilSideController(IHostingEnvironment _environment)
        {
            Environment = _environment;
        }
        */

            public IActionResult ProfilSide()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CV()
        {
          
            //Dummy bruker med id. 
            Bruker = firebase.HentEnkeltBruker("-MTuNdX2ldnO73BCZwFp"); 

            ViewData["Bruker"] = Bruker;
       
            return View(Bruker);
        }

        [HttpPost]
        public IActionResult CV(Bruker cvbruker)
        {
            /*
            Debug.WriteLine("PROFIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIILLLLLLLLLLLLL222222222222" + cv.ArbeidsErfaring.ElementAt(1) +
                cv.ArbeidsErfaring.ElementAt(2) + cv.ArbeidsErfaring.ElementAt(3) + cv.ArbeidsErfaring.ElementAt(4));
            */
            if (ModelState.IsValid)
            {
                try
                {
                    Debug.WriteLine("Inni isValid");
                    firebase.OppdaterBruker(cvbruker);
                    //ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                }
                catch (Exception ex)
                {
                    //  ModelState.AddModelError(string.Empty, ex.Message);
                }
            }else
            {
                Debug.WriteLine("------------------------------------------------------- inni else til CV " + cvbruker.CV.ArbeidsErfaring.Count);
            }
            Bruker hentBruker = firebase.HentEnkeltBruker(cvbruker.Id);
            ViewData["Bruker"] = hentBruker; 
            return View(hentBruker);
        }

        [HttpGet]
        public IActionResult UpsertBruker()
        {
            Bruker nybruker = new Bruker();
            nybruker = firebase.HentEnkeltBruker("-MTuAm8t_eBlv5KMiuWX"); 

            if(nybruker == null)
                return NotFound();

            return View(nybruker); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertBruker(Bruker oppBruker)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(oppBruker.Id)) {
                        firebase.RegistrerBruker(oppBruker);
                        ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                    } else
                    {
                        firebase.OppdaterBruker(oppBruker);
                        ModelState.AddModelError(string.Empty, "Oppdatert suksessfult!");
                    }
                }
                
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(oppBruker);
        }


        [HttpPost]
        public ActionResult UploadFile(IFormFile file)
        {

            firebase.UploadFile(file);

            return View("ProfilSide");
        }


        public IActionResult Portefølje()
        {
            //Dummy bruker. Hent via session eller onclick senere...
            Bruker = firebase.HentEnkeltBruker("-MTuNdX2ldnO73BCZwFp");
            //  Portfolio po = firebase.HentAlleMapper("");

            // Debug.WriteLine("url til bilde: " + (((Innlegg)po.MappeInnhold.ElementAt(0)).IkonURL)); 


            ViewData["Bruker_Innhold"] = Bruker;
           // ViewData["Port"] = firebase.HentAlleMapper("");
            return View(Bruker);

        }
    }
    
}
