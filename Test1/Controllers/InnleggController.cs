using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portefolio_webApp.Controllers
{
    public class InnleggController : Controller
    {

        private readonly IFirebaseConfig firebase = new FirebaseConfig
        {
            AuthSecret = "HIEibEg1o7S9UXwEvMQ0KPLVd8JwZdxZppMLHMUe",
            BasePath = "https://bachelor-it-97124-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        IFirebaseClient klient; 

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
        public IActionResult Upsert_Innlegg(Innlegg innlegg)
        {
            DateTime today = DateTime.Today;
            DateTime l = today;
            innlegg.Dato = l.ToString("dd/MM/yyyy");

            innlegg.Klokkeslett = DateTime.Now.ToString("h:mm:ss tt"); 

            if (ModelState.IsValid)
            {
                try
                {
                    AddInnleggToFirebase(innlegg);
                    ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
           
            return View(Innlegg);
        }

        private void AddInnleggToFirebase(Innlegg innlegg)
        {
            klient = new FireSharp.FirebaseClient(firebase);
            var data = innlegg;
            PushResponse respons = klient.Push("Innlegg/", data);
            data.Id = respons.Result.name;
            SetResponse setResponse = klient.Set("Innlegg/" + data.Id, data);
        }
    }
}
