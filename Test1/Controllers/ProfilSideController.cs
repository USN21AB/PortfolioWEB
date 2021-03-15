﻿using Microsoft.AspNetCore.Mvc;
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
using Test1.Controllers;

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

       


        public IActionResult ProfilSide()
        {
            //Midlertidig kommenter ut
           // var token = HttpContext.Session.GetString("_UserToken");
            //if (token != null)
            //{
                return View();
           // }
          //  else
          //  {
          //      return Redirect("~/Login/SignIn");
          //  }
        }

        [HttpGet]
        public IActionResult CV()
        {

            //Dummy bruker med id. 
            Bruker = firebase.HentEnkeltBruker("-MTuNdX2ldnO73BCZwFp");
            Innlegg innlegg = new Innlegg();
            innlegg.Tittel = "TESTITTEL"; 
            ViewData["Bruker"] = Bruker;
            ViewData["test"] = innlegg; 
            return View(Bruker);
        }


        [HttpPost]
        [RequestSizeLimit(4294967295)]
        public async Task<ActionResult> UploadFile(IFormFile file, [FromServices] IHostingEnvironment oHostingEnvironment, string brukerId)
        {

            Console.WriteLine("ACTIVATED;;;;;;;");
            string filename = $"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(filename))
            {

                file.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();


            }

            await firebase.UploadProfilBilde(filename, file,brukerId);

            return View("ProfilSide");
        }

        [HttpGet]
        public IActionResult UpsertBruker()
        {
            Bruker nybruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID")); 
             Debug.WriteLine("-----------------------------------SESSIONNN!!!" + HttpContext.Session.GetString("_UserID"));
            if (nybruker == null)
                nybruker = new Bruker(); 

            return View(nybruker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertBruker(Bruker oppBruker, string filename, IFormFile file, [FromServices] IHostingEnvironment oHostingEnvironment)
        {
            Debug.WriteLine("INNI POST UPSERTBRUKER I KONTROLLER"); 
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(oppBruker.Id)) {
                      //  firebase.RegistrerBruker(oppBruker);
                        ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                    } else
                    {
                        firebase.OppdaterBruker(oppBruker);
                        Console.WriteLine("" + file.FileName);
                        UploadFile(file,oHostingEnvironment,oppBruker.Id);
                        ModelState.AddModelError(string.Empty, "Oppdatert suksessfult!");
                    }
                }
                
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            Debug.WriteLine("INNI POST UPSERTBRUKER I KONTROLLER ENDING");

            var logginn = new LoginController();
            logginn.Register(oppBruker);
            //return RedirectToAction("Login",
            //        "Register",
            //    new { bruker = oppBruker });

            return RedirectToAction("ProfilSide"); 
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

        [HttpPost]
        public JsonResult LeggTilCV(string felt, string par1, string par2, string par3, string par4, string par5)
        {
            Debug.WriteLine("---------------------------------yo " + par3 + " og " + par4);
            Bruker = firebase.HentEnkeltBruker("-MTuNdX2ldnO73BCZwFp");

            if (felt == "Arbeidserfaring")
            {
                //ViewData["liste"] = firebase.SorterAlleInnlegg(kategori);

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
            else
            {
                Bruker.CV.Språk.Add(par1);
                Bruker.CV.Språk.Add(par2);
            }

            firebase.OppdaterBruker(Bruker);
            var resultat = "Jobberfaring oppdatert: " + par1 + " " + par2;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        [HttpPost]
        public JsonResult DeleteCV(string felt, string index)
        {
            Debug.WriteLine("---------------------------------yo " + felt + " og " + index);
            Bruker = firebase.HentEnkeltBruker("-MTuNdX2ldnO73BCZwFp");
            if (felt == "ArbeidsErfaring")
            {
                Bruker.CV.ArbeidsErfaring.RemoveRange(Int32.Parse(index), 5);
            }
            else if (felt == "Utdanning")
            {
                Bruker.CV.Utdanning.RemoveRange(Int32.Parse(index), 4);
            }

            firebase.OppdaterBruker(Bruker);
            var resultat = "Jobberfaring oppdatert: " + felt + " " + index;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        [HttpPost]
        public JsonResult UpdateCV(string bruker, string felt, string index, string årFra, string årTil, string tittel, string bedrift, string bio)
        {
            Debug.WriteLine("------------------------------------------ MOMOMOMOMOM");
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

            firebase.OppdaterBruker(Bruker);
            var resultat = "Jobberfaring oppdatert: " + tittel + " " + index;
            var data = new { status = "ok", result = resultat };
           
            return Json(data);
        }
    }
    
}
