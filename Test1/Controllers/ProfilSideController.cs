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




        [HttpGet]
        public IActionResult ProfilSide(string brukerID)
        {

            //var brukerID = HttpContext.Session.GetString("_UserID");
            //Midlertidig kommenter ut
            // var token = HttpContext.Session.GetString("_UserToken");
            //if (token != null)
            //{

            Bruker = firebase.HentEnkeltBruker(brukerID);

            ViewData["Bruker"] = Bruker;
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");

            /*
            var str = HttpContext.Session.GetString("brukertest");
            var bruker2 = JsonConvert.DeserializeObject<Bruker>(str);

            Debug.WriteLine("-------------------------------Dette er serialized: " + bruker2.Navn); 
            ViewData["brukertesten"] = bruker2; 
            */
            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }

            return View();
            // }
            //  else
            //  {
            //      return Redirect("~/Login/SignIn");
            //  }

        }

        public IActionResult NyMappe(string brukerID)
        {

            Bruker bruker = firebase.HentEnkeltBruker(brukerID);

            bruker.Mapper.Add(new Portfolio(brukerID, "Default"));

            firebase.OppdaterBruker(bruker);

            return RedirectToAction("ProfilSide");
            
        }


        [HttpGet]
        public IActionResult CV(string brukerID)
        {
          
            //Dummy bruker med id. 
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
              
            }
            Bruker hentBruker = firebase.HentEnkeltBruker(cvbruker.Id);
            ViewData["Bruker"] = hentBruker; 
            return View(hentBruker);
        }


      

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertBrukerAsync(Bruker oppBruker,string password, string passwordRetyp, string filename, IFormFile file, [FromServices] IHostingEnvironment oHostingEnvironment)
        {

            Debug.WriteLine("--------------------------UPSERT BRUKER TEST CV: " + password + " " + passwordRetyp);
         

            if (string.IsNullOrEmpty(oppBruker.Id))
                    { //CREATE 

                if (string.Equals(password, passwordRetyp)) {
                    Debug.WriteLine("------inni is equal: " + string.Equals(password, passwordRetyp));
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

                            string hello;

                            if (oppBruker.Profilbilde == "")
                                hello = "EMPTY";
                            else
                            {
                                var spliturl = oppBruker.Profilbilde.Split("/");
                                hello = spliturl[spliturl.Length - 1];
                            }

                            Console.WriteLine("TEST: " + hello);



                            await firebase.RegistrerBruker(oppBruker);
                            await firebase.UploadProfilBilde(HttpContext.Session.GetString("CroppedPath"), oppBruker.Id, hello);
                            firebase.OppdaterBrukerBilde(oppBruker);
                            HttpContext.Session.Remove("CroppedPath");

                            Console.WriteLine("Previous Profilepic link: " + FBFileToRemoveName);
                        }

                    }
                    }else
                {
                    Debug.WriteLine("------inni is NOT equal: " + string.Equals(password, passwordRetyp));
                    ModelState.AddModelError(string.Empty, "Password must match in both password inputs");
                    return View(oppBruker); 
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

                Console.WriteLine("FEEE: " + HttpContext.Session.GetString("CroppedPath"));

                if (HttpContext.Session.GetString("CroppedPath") == "" || HttpContext.Session.GetString("CroppedPath") == null)
                {
                    firebase.OppdaterBruker(oppBruker);
                }
                else
                {




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


        public IActionResult Portefølje(string brukerid, int antall)
        {
            //Dummy bruker. Hent via session eller onclick senere...
            Bruker = firebase.HentEnkeltBruker(brukerid);
            //  Portfolio po = firebase.HentAlleMapper("");

            // Debug.WriteLine("url til bilde: " + (((Innlegg)po.MappeInnhold.ElementAt(0)).IkonURL)); 


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
            // ViewData["Port"] = firebase.HentAlleMapper("");
            Console.WriteLine(antall+"hold kjeft sondre");

            return View(Bruker);

        }

        public JsonResult RequestCV(string type, Boolean erLest, string FraHvemID, string FraHvemNavn, string TilHvemID, string innleggID, string Tidspunkt)
        {
            
            Notifications not = new Notifications(type, erLest, FraHvemID, FraHvemNavn, TilHvemID, innleggID, Tidspunkt);
            firebase.SendNotification(not);
            var resultat = "'";
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

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

        [HttpPost]
        public JsonResult DeleteCV(string felt, string index)
        {
            Debug.WriteLine("---------------------------------yo " + felt + " og " + index);
            Bruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));
            if (felt == "ArbeidsErfaring")
            {
                Bruker.CV.ArbeidsErfaring.RemoveRange(Int32.Parse(index), 5);
            }
            else if (felt == "Utdanning")
            {
                Bruker.CV.Utdanning.RemoveRange(Int32.Parse(index), 4);
            }

            else if (felt == "Ferdigheter")
            {
                Bruker.CV.Ferdigheter.RemoveRange(Int32.Parse(index), 2);
            }
            else if (felt == "Språk")
            {
                Bruker.CV.Språk.RemoveRange(Int32.Parse(index), 2);
            }

            firebase.OppdaterBruker(Bruker);

            var str = JsonConvert.SerializeObject(Bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            var resultat = "Jobberfaring oppdatert: " + felt + " " + index;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }



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
                int startLengde = Bruker.CV.Ferdigheter.Count;
                int tilSammen = array.Length - startLengde;

                for (int i = 0; i < Bruker.CV.Ferdigheter.Count; i++)
                {
                    Bruker.CV.Ferdigheter[i] = array[i];
                }
                for (int j = startLengde; j < array.Length; j++)
                {
                    Debug.WriteLine("------------------------------------------ MOMOMOMOMOM-add " + array[j]);
                    Bruker.CV.Ferdigheter.Add(array[j]);
                }
            }
            else if (felt == "Språk")
            {
                Debug.WriteLine("------------------------------------------ MOMOMOMOMOM INNI SPRÅK");
                int startLengde = Bruker.CV.Språk.Count;
                int tilSammen = array.Length - startLengde;

                for (int i = 0; i < Bruker.CV.Språk.Count; i++)
                {
                    Bruker.CV.Språk[i] = array[i];
                }
                for (int j = startLengde; j < array.Length; j++)
                {
                    Debug.WriteLine("------------------------------------------ MOMOMOMOMOM-add SPRÅK " + array[j]);
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

        


        [HttpPost]
        public JsonResult DeleteFolder(int mindex)
        {

            Debug.WriteLine("---------------------------------yo "  + mindex + " lol ");
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

        [HttpPost]
        public JsonResult DeleteInnlegg(string id, int mindex, int index)
        {

            Debug.WriteLine("---------------------------------yo " + mindex + " lol " + id);
            Bruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));

            

            firebase.SlettInnlegg(id);

            //firebase.SlettMappeInnlegg(Bruker.Id, mindex, index);

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
