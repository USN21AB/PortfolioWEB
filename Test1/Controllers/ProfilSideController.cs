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
        public ActionResult UploadFile(IFormFile file)
        {

            firebase.UploadFile(file);

            return View("ProfilSide");
        }

        [HttpPost]
        public IActionResult CV(CV cv) { 
        Bruker.CV = cv;
            Debug.WriteLine("PROFIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIILLLLLLLLLLLLL222222222222"+cv.ArbeidsErfaring.ElementAt(1) + 
                cv.ArbeidsErfaring.ElementAt(2) + cv.ArbeidsErfaring.ElementAt(3) + cv.ArbeidsErfaring.ElementAt(4));

            if (ModelState.IsValid)
            {
                try
                {
                    Debug.WriteLine("Inni isValid");
                   // firebase.OppdaterBruker(Bruker);
                    //ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                }
                catch (Exception ex)
                {
                  //  ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            Debug.WriteLine("Fortsetter--------------------------------");

            Bruker.Navn = "Mary Jane";
            Bruker.Stilling = "Gardener";
            Bruker.Profilbilde = "https://images.unsplash.com/photo-1542103749-8ef59b94f47e?ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&ixlib=rb-1.2.1&auto=format&fit=crop&w=1350&q=80";
            Bruker.Epost = "Mary@hotmail.com";
            Bruker.Id = "-MTL4PwvI0ChIfEaZwEu";

            firebase.OppdaterBruker(Bruker);
            ViewData["Cv_Innhold"] = CirVit;
            ViewData["Bruker_Innhold"] = Bruker;
            return View(cv);
        }


        public IActionResult Portefølje()
        {
            Bruker = new Bruker();



            BildeInnlegg bilde = new BildeInnlegg();
            bilde.Beskrivelse = "Lenore";
            bilde.Tittel = "Castlevania";
            bilde.StorageURL = "https://i.redd.it/tsj2clkpt7q41.jpg";

            BildeInnlegg bilde1 = new BildeInnlegg();
            bilde1.Beskrivelse = "Alucard";
            bilde1.Tittel = "Castlevania";
            bilde1.StorageURL = "https://i.pinimg.com/originals/55/c2/8d/55c28dc320497ec4e623d51814140fa3.jpg";

            BildeInnlegg bilde2 = new BildeInnlegg();
            bilde2.Beskrivelse = "Sypha";
            bilde2.Tittel = "Castlevania";
            bilde2.StorageURL = "https://i.ytimg.com/vi/p3DzNzuw1Aw/maxresdefault.jpg";

            BildeInnlegg bilde5 = new BildeInnlegg();
            bilde5.Beskrivelse = "Trevor";
            bilde5.Tittel = "Castlevania";
            bilde5.StorageURL = "https://www.therpf.com/forums/attachments/latest-3-png.1063723/";

            BildeInnlegg bilde6 = new BildeInnlegg();
            bilde6.Beskrivelse = "Dracula";
            bilde6.Tittel = "Castlevania";
            bilde6.StorageURL = "https://www.denofgeek.com/wp-content/uploads/2018/10/castlevania-dracula.jpg?fit=1200%2C800";

            Portfolio mappe = new Portfolio();
            mappe.MappeNavn = "Castlevania";
            mappe.MappeInnhold.Add(bilde);
            mappe.MappeInnhold.Add(bilde1);
            mappe.MappeInnhold.Add(bilde2);
            mappe.MappeInnhold.Add(bilde5);
            mappe.MappeInnhold.Add(bilde6);
            Bruker.Mapper.Add(mappe);

            BildeInnlegg bilde3 = new BildeInnlegg();
            bilde3.Beskrivelse = "Dimirti";
            bilde3.Tittel = "Sly";
            bilde3.StorageURL = "https://i1.sndcdn.com/artworks-000247789062-987i6j-t500x500.jpg";

            BildeInnlegg bilde4 = new BildeInnlegg();
            bilde4.Beskrivelse = "Sly Cooper";
            bilde4.Tittel = "Sly";
            bilde4.StorageURL = "https://images.ctfassets.net/x3227kynr7c6/2uw3G0jH84bkSeVzSymmW/535e6b9d31f3e632f1083780a5dec05a/3zwod-255K5G28QJK-Full-Image_GalleryBackground-en-US-1525537586876._SX1080_.jpg?w=800";


            Portfolio mappe1 = new Portfolio();
            mappe1.MappeNavn = "Sly";
            mappe1.MappeInnhold.Add(bilde3);
            mappe1.MappeInnhold.Add(bilde4);
            Bruker.Mapper.Add(mappe1);



            Bruker.Navn = "Johannes Tingnes Bø";
            Bruker.Stilling = "Gardener";
            Bruker.Profilbilde = "https://images.unsplash.com/photo-1542103749-8ef59b94f47e?ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&ixlib=rb-1.2.1&auto=format&fit=crop&w=1350&q=80";
            Bruker.Epost = "Mary@hotmail.com";
            Bruker.Id = "-MTL4PwvI0ChIfEaZwEu";
            ViewData["Bruker_Innhold"] = Bruker;
            return View(Bruker);
        }
    }
    
}
