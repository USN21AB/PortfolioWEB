using FireSharp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
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
            Debug.WriteLine("------------------------------------upsert innlegg GET: " + innleggID);

            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }

            if (innleggID != "")
            {
                //create
                Innlegg = firebase.HentSpesifiktInnlegg(innleggID);

                if (Innlegg == null)
                    Innlegg = new Innlegg(); 
                Debug.WriteLine("------------------------------------upsert innlegg GET INNI IF TOM: " + Innlegg.Tittel);
                return View(Innlegg);
            }

       
            return View(Innlegg);
        }

        /*[HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> UploadCoverPhoto(Microsoft.AspNetCore.Http.IFormFile file, Innlegg innlegg, [FromServices] IHostingEnvironment oHostingEnvironment, string submit)
        {
          
                        if (file != null)
                            await firebase.UploadCoverPhoto($"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{file.FileName}", file);
                       

            return View(Innlegg);
        }*/


        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Upsert_InnleggAsync(IFormFile inputfile, IFormFile coverfile,Innlegg innlegg, [FromServices] IHostingEnvironment oHostingEnvironment, string mappenavn, string innleggid)
        {

            innlegg.EierId = HttpContext.Session.GetString("_UserID");
            innlegg.Id = innleggid;
          
            DateTime today = DateTime.Today;
            DateTime l = today;
            innlegg.Dato = l.ToString("dd/MM/yyyy");
    

            innlegg.Tagger[0].Split(",");
            var splitTag = innlegg.Tagger[0].Split(",");

            
            
            innlegg.Tagger.Clear();
            for (var i = 0; i < splitTag.Length; i++)
            {
                
                    innlegg.Tagger.Add(splitTag[i].Trim());
            }

            var bruker = firebase.HentEnkeltBruker(innlegg.EierId);

            innlegg.Klokkeslett = DateTime.Now.ToString("h:mm:ss tt");

            if (ModelState.IsValid)
            {


                Debug.WriteLine("dwdwadada" + innlegg.Id);
                Debug.WriteLine("dwdwadada" + innleggid);


                if (string.IsNullOrEmpty(innlegg.Id))
                { //REgistrer nytt innlegg siden id er null.

                    Boolean mappefinnes = false;

                    Debug.WriteLine("lol" + innlegg.EierId);
                    for (var i = 0; i < bruker.Mapper.Count; i++)
                    {
                        Debug.WriteLine("for runde: " + i);
                        if (bruker.Mapper[i].MappeNavn == mappenavn)
                        {
                            Portfolio portfolio = bruker.Mapper[i];
                            portfolio.MappeInnhold.Add(innlegg);

                            //bruker.Mapper[i].MappeInnhold.Add(innlegg);
                            mappefinnes = true;
                            Debug.WriteLine("Mappen finnes");

                        }
                    }

                    if (mappefinnes == false)
                    {
                        Debug.WriteLine("Mappen finnes ikke");
                        Portfolio portfolio = new Portfolio(bruker.Id, mappenavn);
                        portfolio.MappeInnhold.Add(innlegg);
                        bruker.Mapper.Add(portfolio);


                    }



                    try
                    {

                        if (coverfile != null)
                            await firebase.UploadCoverPhoto($"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{coverfile.FileName}", coverfile, innlegg.EierId);

                        if (inputfile != null)
                        {
                            innlegg.antallLikes = 0;
                            await firebase.RegistrerInnleggMedFil($"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{inputfile.FileName}", inputfile, innlegg);
                          
                            firebase.OppdaterBruker(bruker);
                        }

                        ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                } // Oppdater innlegg siden id finnes!
                else
                {

                    
                    Debug.WriteLine("lol   oppdatert      " + innlegg.EierId);
                    for (var i = 0; i < bruker.Mapper.Count; i++)
                    {
                        Debug.WriteLine("oppfdater for runde mappe : " + i);


                        for (var y = 0; y < bruker.Mapper[i].MappeInnhold.Count; y++)
                        {
                            Debug.WriteLine("oppfdater for runde innlegg: " + y);
                            if (bruker.Mapper[i].MappeInnhold[y].Id == innleggid)
                            {
                                bruker.Mapper[i].MappeInnhold[y] = innlegg;
                                //bruker.Mapper[i].MappeInnhold.Add(innlegg);
                                Debug.WriteLine("oppdatert Mappen finnes");

                        }

                        }

                       
                    }

                    

                    //MÅ LEGGE TIL UPDATE INNLEGG HER
                    innlegg.EierId = HttpContext.Session.GetString("_UserID"); 
                    firebase.OppdaterInnlegg(innlegg);
                    firebase.OppdaterBruker(bruker);

                }
            }

            var str = JsonConvert.SerializeObject(bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
            ViewData["Innlogget_Bruker"] = bruker;

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
            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }
            return View(innlegg);

        }
        /*
        [HttpPost]
        public IActionResult Nav_Innlegg(string Nyid)
        {
            Innlegg innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(Nyid);
            return View(innlegg);
        }*/

        public IActionResult NyttKommentar(string tekst, string innleggId)
        {
            
            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            var innBruker = JsonConvert.DeserializeObject<Bruker>(str);
            var brukerBilde = innBruker.Profilbilde;
            var brukerId = innBruker.Id;
            var brukerNavn = innBruker.Navn;

            Kommentar kommentar = new Kommentar();
            DateTime today = DateTime.Today;
            DateTime l = today;

            kommentar.Tekst = tekst;
            kommentar.InnleggId = innleggId;
            kommentar.Dato = l.ToString("dd/MM/yyyy");
            
            kommentar.EierId = brukerId;
            kommentar.EierNavn = brukerNavn;
            kommentar.EierBilde = brukerBilde;
            

            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            innlegg.Kommentar.Add(kommentar);
            Debug.WriteLine("Oppdaterer innlegg: " + innleggId + "Oppdaterer innlegg: " );
            try
            {
                if (innlegg.Id != null)
                {
                    Debug.WriteLine("Oppdaterer innlegg: " + innlegg.Kommentar[0].InnleggId);
                    //firebase.RegistrerKommentar(kommentar);
                    firebase.OppdaterInnlegg(innlegg);

                    Notifications not = new Notifications("Kommentar", false, innBruker.Id, innBruker.Navn, innlegg.EierId, innlegg.Id, l.ToString("dd/MM/yyyy"));
                    firebase.SendNotification(not);
                  
                }
                
            }
            catch(Exception ex)
            {

            }
            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);
        }

        public IActionResult NyttReply(string tekst, string innleggId, int kommentId)
        {
            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            var innBruker = JsonConvert.DeserializeObject<Bruker>(str);
            var brukerBilde = innBruker.Profilbilde;
            var brukerId = innBruker.Id;
            var brukerNavn = innBruker.Navn;

            Kommentar kommentar = new Kommentar();
            DateTime today = DateTime.Today;
            DateTime l = today;

            kommentar.Tekst = tekst;
            kommentar.InnleggId = innleggId;
            kommentar.Dato = l.ToString("dd/MM/yyyy");

            kommentar.EierId = brukerId;
            kommentar.EierNavn = brukerNavn;
            kommentar.EierBilde = brukerBilde;

            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            var baseKomment = innlegg.Kommentar[kommentId];
            baseKomment.Kommentarer.Add(kommentar);
            innlegg.Kommentar[kommentId] = baseKomment;
            Debug.WriteLine("Oppdaterer innlegg1: " + innleggId + "Oppdaterer innlegg: ");
            try
            {
                if (innlegg.Id != null)
                {
                    Debug.WriteLine("Oppdaterer innlegg2: " + innlegg.Kommentar[0].InnleggId);
                    Debug.WriteLine("Oppdaterer innlegg3: " + innlegg.Kommentar[0].Kommentarer[0].Tekst);
                    //firebase.RegistrerKommentar(kommentar);
                    firebase.OppdaterInnlegg(innlegg);
                }

            }
            catch (Exception ex)
            {

            }
            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);
        }
                                                                           
        public IActionResult DeleteKommentar(string innleggId, int kommentarId)
        {
            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            innlegg.Kommentar.RemoveAt(kommentarId);

            try
            {
                if (innlegg.Id != null)
                {
                    Debug.WriteLine("Oppdaterer innlegg2: " + innlegg.Id);
                    //firebase.RegistrerKommentar(kommentar);
                    firebase.OppdaterInnlegg(innlegg);
                }

            }
            catch (Exception ex)
            {

            }

            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);
            
        }

        public IActionResult DeleteReply(string innleggId, int kommentarId, int replyId)
        {
            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            innlegg.Kommentar[kommentarId].Kommentarer.RemoveAt(replyId);

            try
            {
                if (innlegg.Id != null)
                {
                    Debug.WriteLine("Oppdaterer innlegg2: " + innlegg.Id);
                    //firebase.RegistrerKommentar(kommentar);
                    firebase.OppdaterInnlegg(innlegg);
                }

            }
            catch (Exception ex)
            {

            }

            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);

        }

        [HttpPost]
        public JsonResult LikeInnlegg(string innleggId)
        {
            
            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            var innBruker = JsonConvert.DeserializeObject<Bruker>(str);
         

            var innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            var bruker = firebase.HentEnkeltBruker(innlegg.EierId);

           
            if (innlegg.Likes != null)
            {
                innlegg.Likes.Antall += 1;
              
                innlegg.Likes.Brukere.Add(innBruker.Id);
            }
            else
            {
                Debug.WriteLine("Noe: " + innBruker.Id);
                Like like = new Like();
                like.Antall = 1;
                like.Brukere.Add(innBruker.Id);

                innlegg.Likes = like;
            }
           

            try
            {
                if (innlegg.Id != null)
                {
                    string antLike = (bruker.likeRatio + 1).ToString(); 
                    
                    firebase.OppdaterInnlegg(innlegg);
                    firebase.UpdateSingleUserValue(innlegg.EierId, "likeRatio", antLike);
                    DateTime today = DateTime.Today;
                    DateTime l = today;
                   
                    Notifications not = new Notifications("Like", false, innBruker.Id, innBruker.Navn, innlegg.EierId, innlegg.Id, l.ToString("dd/MM/yyyy"));
                    firebase.SendNotification(not);
                }
            }
            catch (Exception ex)
            {

            }

            var resultat = "Jobberfaring oppdatert: Like";
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        public JsonResult DislikeInnlegg(string innleggId)
        {
            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

            var innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            var bruker = firebase.HentEnkeltBruker(innlegg.EierId);
            var i = 0;
            while (i < innlegg.Likes.Brukere.Count)
            {
                if (innBruker.Id.Equals(innlegg.Likes.Brukere[i]))
                {
                    innlegg.Likes.Brukere.RemoveAt(i);
                    innlegg.Likes.Antall -= 1;
                }
                i++;
            }

            try
            {
                if (innlegg.Id != null)
                {
                    string antLike = (bruker.likeRatio - 1).ToString();
                    firebase.UpdateSingleUserValue(innlegg.EierId, "likeRatio", antLike);
                    firebase.OppdaterInnlegg(innlegg);
                }
            }
            catch (Exception ex)
            {

            }
            var resultat = "Jobberfaring oppdatert: Like";
            var data = new { status = "ok", result = resultat };

            return Json(data);


        }
    }      
}                                                         
