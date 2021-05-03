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
using System.IO;
using LazZiya.ImageResize;
using System.Drawing;

using Test1.Controllers;
using Newtonsoft.Json;

/// <summary>
/// Hovedsiden for å behandle brukerobjekter, registrering og oppdatering av dem. Samt deres variabler slik som CV
/// </summary>
namespace Portefolio_webApp.Controllers
{
    public class ProfilSideController : Controller
    {
        private readonly FirebaseDB firebase;
        LoginController logg;

    

        [BindProperty]
        public CV CirVit { get; set; }

        [BindProperty]
        public Bruker Bruker { get; set; }
        public ProfilSideController()
        {
            firebase = new FirebaseDB();
            logg = new LoginController();
        }



        //Rendre profilsiden med riktig bruker
        [HttpGet]
        public IActionResult ProfilSide(string brukerID)
        {

            Bruker = firebase.HentEnkeltBruker(brukerID);

            ViewData["Bruker"] = Bruker;
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");

         
            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }

            return View();
        }

        //Lag ny mappe
        public IActionResult NyMappe(string brukerID)
        {

            Bruker bruker = firebase.HentEnkeltBruker(brukerID);

            bruker.Mapper.Add(new Portfolio(brukerID, "Default"));

            firebase.OppdaterBruker(bruker);

            return RedirectToAction("ProfilSide");
            
        }

        //Rendre CV til en bruker
        [HttpGet]
        public IActionResult CV(string brukerID)
        {

            Bruker = firebase.HentEnkeltBruker(brukerID);

            ViewData["Bruker"] = Bruker;

            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");

            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }
            return View(Bruker);
        }

        //Poster CV Skjema
        [HttpPost]
        public IActionResult CV(Bruker cvbruker)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    Debug.WriteLine("Inni isValid");
                    firebase.OppdaterBruker(cvbruker);
                   
                }
                catch (Exception ex)
                {
                   
                }
            }else
            {
              
            }
            Bruker hentBruker = firebase.HentEnkeltBruker(cvbruker.Id);
            ViewData["Bruker"] = hentBruker; 
            return View(hentBruker);
        }


      
        //Rendrer ett registreringskjema for bruker
        [HttpGet]
        public IActionResult UpsertBruker()
        {
          
            Bruker nybruker = new Bruker();
            var token = HttpContext.Session.GetString("_UserToken");
            
            if (token != null)
            nybruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID")); 
            

            if(nybruker == null)
                return NotFound();

            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }



            return View(nybruker); 
        }

        //Poster registreringskjema til bruker og behandler den enten som en update eller registrer basert på om id er null eller ikke
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertBrukerAsync(Bruker oppBruker,string password, string passwordRetyp, string filename, IFormFile file, [FromServices] IHostingEnvironment oHostingEnvironment)
        {


            if (string.IsNullOrEmpty(oppBruker.Id))
                    { //CREATE 
                
                if (password != null && passwordRetyp != null && string.Equals(password, passwordRetyp) && password.Length > 7) {
                    
                    string logginnID = logg.Register(oppBruker.Email, password).Result;
                        string[] splittArr = logginnID.Split("|");

                    string FBFileToRemoveName = "";

                    oppBruker.Id = splittArr[0];
                        
                        oppBruker.Id = splittArr[0];
                        oppBruker.NumberOfNotifications = 0;
                        oppBruker.likeRatio = 0;

                    HttpContext.Session.SetString("_UserID", splittArr[0]);
                        HttpContext.Session.SetString("_UserToken", splittArr[1]);

                        if (oppBruker.Id == "")
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Password or Email!");
                        }
                        else
                        {
                            oppBruker.CV.BrukerID = oppBruker.Id;
                            

                            oppBruker.Mapper = new List<Portfolio>();
                            
                        if (HttpContext.Session.GetString("CroppedPath") == "" || HttpContext.Session.GetString("CroppedPath") == null)
                        {
                            firebase.RegistrerBruker(oppBruker);
                        }
                        else
                        {



                            await firebase.RegistrerBruker(oppBruker);
                            await firebase.UploadProfilBilde(HttpContext.Session.GetString("CroppedPath"), oppBruker.Id, oppBruker.Profilbilde);
                            
                            firebase.OppdaterBrukerBilde(oppBruker);
                            HttpContext.Session.Remove("CroppedPath");

                            Console.WriteLine("Previous Profilepic link: " + FBFileToRemoveName);
                        }

                    }
                    }else
                {
                   
                    if (password != passwordRetyp)
                    {
                        ModelState.AddModelError(string.Empty, "Password must match in both password inputs");
                        return View(oppBruker);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Password must be at least 8 characters");
                        return View(oppBruker);
                    }
                }
            }
            else
            { //OPPDATERER


                var str2 = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str2);
              
                oppBruker.CV = innBruker.CV;
                oppBruker.Mapper = innBruker.Mapper;
                oppBruker.CVAdgang = innBruker.CVAdgang;
                oppBruker.notifications = innBruker.notifications;
                oppBruker.Profilbilde = innBruker.Profilbilde;
                oppBruker.kommentertPå = innBruker.kommentertPå; 

                Console.WriteLine("FEEE: " + HttpContext.Session.GetString("CroppedPath"));

     

                if (HttpContext.Session.GetString("CroppedPath") == "" || HttpContext.Session.GetString("CroppedPath") == null)
                {
                    firebase.OppdaterBruker(oppBruker);
                    UpdateKommentarer(oppBruker);
                
                }
                else
                {
                    UpdateKommentarer(oppBruker);
                 
                    await firebase.UploadProfilBilde(HttpContext.Session.GetString("CroppedPath"), oppBruker.Id, oppBruker.Profilbilde);
                    firebase.OppdaterBrukerBilde(oppBruker);
                    HttpContext.Session.Remove("CroppedPath");
                }


              
                ModelState.AddModelError(string.Empty, "Oppdatert suksessfult!");
                    }



                    if (file != null)
                    {
                        Console.WriteLine("" + file.FileName);
                        
                    }

            var str = JsonConvert.SerializeObject(oppBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);
       


            ViewData["Innlogget_Bruker"] = oppBruker;
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");

            return RedirectToAction("BrowseSide", "Home");
        }

        //Multiupdate på alle kommentarene til brukeren siden bruker kan endre navn og profilbilde
        public void UpdateKommentarer(Bruker oppBruker)
        {
           
            //Update kommentarer i innlegg
            for (int i = 0; i < oppBruker.kommentertPå.Count; i++)
            {

                Innlegg innlegg = firebase.HentSpesifiktInnlegg(oppBruker.kommentertPå[i]);
                
                if(innlegg != null) { 
                for (int j = 0; j < innlegg.Kommentar.Count; j++)
                {
                   
                    if (innlegg.Kommentar[j].EierId == oppBruker.Id)
                    {
                        innlegg.Kommentar[j].EierBilde = oppBruker.Profilbilde;
                        innlegg.Kommentar[j].EierNavn = oppBruker.Navn;
                      
                    }

                    for (int h = 0; h < innlegg.Kommentar[j].Kommentarer.Count; h++)
                    {
                        if (innlegg.Kommentar[j].Kommentarer[h].EierId == oppBruker.Id)
                        {
                            innlegg.Kommentar[j].Kommentarer[h].EierBilde = oppBruker.Profilbilde;
                            innlegg.Kommentar[j].Kommentarer[h].EierNavn = oppBruker.Navn;
                        }
                    }
                }
                firebase.OppdaterInnlegg(innlegg);
                }
            }
        }

        //Rendrer brukerens portefolieside med mappeinnhold
        public IActionResult Portefølje(string brukerid, int antall)
        {
          
            Bruker = firebase.HentEnkeltBruker(brukerid);
     
            ViewData["Bruker_Innhold"] = Bruker;
            ViewData["antall"] = antall;
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");

            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }


            return View(Bruker);
        }

        //Etterspør en brukers CV
        public JsonResult RequestCV(string type, Boolean erLest, string FraHvemID, string FraHvemNavn, string TilHvemID, string innleggID, string Tidspunkt)
        {
            Debug.WriteLine("typen: " + type); 
            Notifications not = new Notifications(type, erLest, FraHvemID, FraHvemNavn, TilHvemID, innleggID, Tidspunkt);
            firebase.SendNotification(not);
            var resultat = "'";
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        //Legg til felt i CV
       [HttpPost]
        public JsonResult LeggTilCV(string felt, string par1, string par2, string par3, string par4, string par5)
        {
            
            Bruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));

            if (felt == "Arbeidserfaring")
            {
                Bruker.CV.ArbeidsErfaring.Add(par1);
                Bruker.CV.ArbeidsErfaring.Add(par2);
                Bruker.CV.ArbeidsErfaring.Add(par3);
                Bruker.CV.ArbeidsErfaring.Add(par4);
                Bruker.CV.ArbeidsErfaring.Add(par5);
            }
            else if (felt == "Utdanning")
            {
          
                Bruker.CV.Utdanning.Add(par1);
                Bruker.CV.Utdanning.Add(par2);
                Bruker.CV.Utdanning.Add(par3);
                Bruker.CV.Utdanning.Add(par4);
            }
            else if (felt == "Ferdigheter")
            {
                Bruker.CV.Ferdigheter.Add(par1);
                Bruker.CV.Ferdigheter.Add(par2);
            }
            else if (felt == "Språk")
            {
                Bruker.CV.Språk.Add(par1);
                Bruker.CV.Språk.Add(par2);
            }

            Bruker.CV.BrukerID ="123456"; 
            firebase.OppdaterBruker(Bruker);

            var str = JsonConvert.SerializeObject(Bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);


            var resultat = "Jobberfaring oppdatert: " + par1 + " " + par2;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        //Slett felt i CV
        [HttpPost]
        public JsonResult DeleteCV(string felt, string index, string indexNavn)
        {
            Bruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));
            if (felt == "ArbeidsErfaring")
            {
                int indexN = Bruker.CV.ArbeidsErfaring.IndexOf(indexNavn);
                Bruker.CV.ArbeidsErfaring.RemoveRange((indexN-2), 5);
            }
            else if (felt == "Utdanning")
            {
                int indexN = Bruker.CV.Utdanning.IndexOf(indexNavn);
                Bruker.CV.Utdanning.RemoveRange((indexN - 2), 4);
            }

            else if (felt == "Ferdigheter")
            {
                int indexN = Bruker.CV.Ferdigheter.IndexOf(indexNavn);
             
                Bruker.CV.Ferdigheter.RemoveRange(indexN, 2);
            }
            else if (felt == "Språk")
            {
                int indexN = Bruker.CV.Språk.IndexOf(indexNavn);
                Bruker.CV.Språk.RemoveRange(indexN, 2);
            }

            firebase.OppdaterBruker(Bruker);

            var str = JsonConvert.SerializeObject(Bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            var resultat = "Jobberfaring oppdatert: " + felt + " " + index;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }


        //Oppdater en verdi i CV
        [HttpPost]
        public JsonResult UpdateCV(string bruker, string felt, string index, string årFra, string årTil, string tittel, string bedrift, string bio, string[] array)
        {

            int indexI = Int32.Parse(index);

            Bruker = firebase.HentEnkeltBruker(bruker);
           
      
            if (felt == "ArbeidsErfaring")
            {
                Bruker.CV.ArbeidsErfaring[indexI] = årFra;
                Bruker.CV.ArbeidsErfaring[indexI + 1] = årTil;
                Bruker.CV.ArbeidsErfaring[indexI + 2] = tittel;
                Bruker.CV.ArbeidsErfaring[indexI + 3] = bedrift;
                Bruker.CV.ArbeidsErfaring[indexI + 4] = bio;

            }
            else if (felt == "Utdanning")
            {
                Bruker.CV.Utdanning[indexI] = årFra;
                Bruker.CV.Utdanning[indexI + 1] = årTil;
                Bruker.CV.Utdanning[indexI + 2] = tittel;
                Bruker.CV.Utdanning[indexI + 3] = bedrift;
            }
            else if (felt == "Ferdigheter")
            {
                Bruker.CV.Ferdigheter.Clear(); 

                for(int i= 0; i<array.Length; i++)
                {
                    Bruker.CV.Ferdigheter.Add(array[i]);
                }
            
            }
            else if (felt == "Språk")
            {
              
                int startLengde = Bruker.CV.Språk.Count;
                int tilSammen = array.Length - startLengde;

                for (int i = 0; i < Bruker.CV.Språk.Count; i++)
                {
                    Bruker.CV.Språk[i] = array[i];
                }
                for (int j = startLengde; j < array.Length; j++)
                {
                   
                    Bruker.CV.Språk.Add(array[j]);
                }
            }
            
            firebase.OppdaterBruker(Bruker);

            var str = JsonConvert.SerializeObject(Bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            var resultat = "Jobberfaring oppdatert: " + tittel + " " + index;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        

        //Slett en mappe
        [HttpPost]
        public JsonResult DeleteFolder(int mindex)
        {

            Bruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));


            var mappeinnlegg = Bruker.Mapper[mindex].MappeInnhold;
            var i = 0;
            List<string> innleggIDer = new List<string>();
            while(i < mappeinnlegg.Count)
            {
                innleggIDer.Add(mappeinnlegg[i].Id);
                i++;
            }

            var x = 0;
            while(x < innleggIDer.Count)
            {
                firebase.SlettInnlegg(innleggIDer[x]);
                x++;
            }

            
            
            Bruker.Mapper.RemoveAt(mindex);
            

            firebase.OppdaterBruker(Bruker);

            var str = JsonConvert.SerializeObject(Bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            var resultat = "Jobberfaring oppdatert: "  + " " + mindex;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        //Slett ett innlegg
        [HttpPost]
        public JsonResult DeleteInnlegg(string id, int mindex, int index)
        {

            Bruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));

            firebase.SlettInnlegg(id);

            if (Bruker.Mapper[mindex].MappeInnhold.Count == 1)
                Bruker.Mapper[mindex].MappeInnhold = null;
            else
            Bruker.Mapper[mindex].MappeInnhold.RemoveAt(index);

            firebase.OppdaterBruker(Bruker);

            var str = JsonConvert.SerializeObject(Bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            var resultat = "Jobberfaring oppdatert: " + id + " " + mindex;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

    }

}
