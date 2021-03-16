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
            var token = HttpContext.Session.GetString("_UserToken");
         
        
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
                Image image = Image.FromStream(file.OpenReadStream(), true, true);
                var scaleImg = ImageResize.Scale(image, 10, 10);
                scaleImg.SaveAs($"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{"Waka"+file.FileName}");


            }

            await firebase.UploadProfilBilde($"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{"Waka" + file.FileName}", file, brukerId);



            return View("ProfilSide");
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
        public IActionResult UpsertBruker(Bruker oppBruker, string filename, IFormFile file, [FromServices] IHostingEnvironment oHostingEnvironment)
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
            return View(oppBruker);
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

            if (felt == "Arbeidserfaring") { 
            //ViewData["liste"] = firebase.SorterAlleInnlegg(kategori);
          
            Bruker.CV.ArbeidsErfaring.Add(par1);
            Bruker.CV.ArbeidsErfaring.Add(par2);
            Bruker.CV.ArbeidsErfaring.Add(par3);
            Bruker.CV.ArbeidsErfaring.Add(par4);
            Bruker.CV.ArbeidsErfaring.Add(par5);
            } else if(felt == "Utdanning")
            {
                Bruker.CV.Utdanning.Add(par1);
                Bruker.CV.Utdanning.Add(par2);
                Bruker.CV.Utdanning.Add(par3);
                Bruker.CV.Utdanning.Add(par4);
            }
            else if(felt == "Ferdigheter")
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
    }
    
}
