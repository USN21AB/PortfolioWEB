using FireSharp.Interfaces;
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
        public IActionResult Upsert_Innlegg(Innlegg innlegg)
        {
            DateTime today = DateTime.Today;
            DateTime l = today;
            innlegg.Dato = l.ToString("dd/MM/yyyy");
            //Hent innlogget person innlegg.EierId = firebase.hentBruker();  

            innlegg.Klokkeslett = DateTime.Now.ToString("h:mm:ss tt");

            if (ModelState.IsValid)
            {
                try
                {
                    firebase.RegistrerInnlegg(innlegg);
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

            return View(innlegg);

        }

        public IActionResult Register()
        {
            return View();
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
