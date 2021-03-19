using FireSharp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        public Kommentar Kommentar { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert_Innlegg(string innleggID)
        {
            Innlegg = new Innlegg();

            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");

            if (innleggID != "")
            {
                //create
                Innlegg = firebase.HentSpesifiktInnlegg(innleggID); 
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

                if (string.IsNullOrEmpty(innlegg.Id))
                {
                    try
                    {

                        ProfilSideController profilSideController = new ProfilSideController();


                        //logg.Register(oppBruker.Email, oppBruker.Password).Result;
                        innlegg.EierId = HttpContext.Session.GetString("_UserID");
                        firebase.RegistrerInnlegg(innlegg);
                        Console.WriteLine("EYOOOOOOOOOOO BRUUUUH");
                        profilSideController.UploadFile(file, oHostingEnvironment, "- MTuNdX2ldnO73BCZwFp");
                        Console.WriteLine("EYOOOOOOOOOOO BRUUUUH");
                        ModelState.AddModelError(string.Empty, "Registrering suksessfult!");

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
                else
                {
                    innlegg.EierId = HttpContext.Session.GetString("_UserID"); 
                    firebase.OppdaterInnlegg(innlegg);
                }
            }

            return View(Innlegg);
        }

        public IActionResult Nav_Innlegg(string id)
        {
            //Skjekker om bruker er logget inn
            var model = new InnleggController();
            
            var token = HttpContext.Session.GetString("_UserToken");
            var isLoggedOn = false;
            if (token != null) isLoggedOn = true;
            ViewBag.Status = isLoggedOn;
            
            //Henter innlegget bruker klikket på
            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(id);
            var bruker = new Bruker();
            bruker = firebase.HentEnkeltBruker(innlegg.EierId);
                
            ViewData["bruker"] = bruker;
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
            return View(innlegg);

        }

        public IActionResult NyttKommentar(string tekst, string innleggId)
        {
            Kommentar kommentar = new Kommentar();
            DateTime today = DateTime.Today;
            DateTime l = today;
            kommentar.Tekst = tekst;
            kommentar.InnleggId = innleggId;
            kommentar.Dato = l.ToString("dd/MM/yyyy");

            try
            {
                firebase.RegistrerKommentar(kommentar);
            }
            catch(Exception ex)
            {

            }
            return Redirect("~/ProfilSide/ProfilSide");
        }

    }
}
