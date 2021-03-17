using FireSharp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Portefolio_webApp.Models;
using System;
using System.Diagnostics;
using Test1.Models;

namespace Portefolio_webApp.Controllers
{
    public class InnleggController : Controller
    {
        public readonly IFirebaseConfig db;
        public IFirebaseClient klient;

        private readonly FirebaseDB firebase;

        public InnleggController()
        {
            firebase = new FirebaseDB();
        }

        [BindProperty]
        public Innlegg Innlegg { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert_Innlegg()
        {
            Innlegg = new Innlegg();

            if (Innlegg.Id == null)
            {
                //create
                return View(Innlegg);
            }

            return View(Innlegg);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Upsert_InnleggAsync(Microsoft.AspNetCore.Http.IFormFile file, Innlegg innlegg, [FromServices] IHostingEnvironment oHostingEnvironment)
        {
            Console.WriteLine("EYOOOOOOOOOOO BRUUUUH");
            DateTime today = DateTime.Today;
            DateTime l = today;
            innlegg.Dato = l.ToString("dd/MM/yyyy");
            //Hent innlogget person innlegg.EierId = firebase.hentBruker();  

            innlegg.Klokkeslett = DateTime.Now.ToString("h:mm:ss tt");

            if (ModelState.IsValid)
            {
                try
                {

                    ProfilSideController profilSideController = new ProfilSideController();

                    
                    //logg.Register(oppBruker.Email, oppBruker.Password).Result;

                    firebase.RegistrerInnlegg(innlegg);
                    Console.WriteLine("EYOOOOOOOOOOO BRUUUUH");
                    profilSideController.UploadFile(file,oHostingEnvironment, "- MTuNdX2ldnO73BCZwFp");
                    Console.WriteLine("EYOOOOOOOOOOO BRUUUUH");
                    ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                    
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(Innlegg);
        }

        public IActionResult Nav_Innlegg(string id)
        {
            Debug.WriteLine("id er:  " + id);
            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(id);
            return View(innlegg);
            
        }

    }

}
