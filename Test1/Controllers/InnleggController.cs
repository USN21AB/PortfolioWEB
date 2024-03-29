﻿using FireSharp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Test1.Models;
/// <summary>
///     Kontrolleren for innleggsiden. Den har ansvaret for alle innleggsobjektene og dens registreringer. 
/// </summary>
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

        //Registreringskjema for innlegg
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

        //Registreringskjema blir posta og behandla
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Upsert_InnleggAsync(IFormFile inputfile, IFormFile coverfile,Innlegg innlegg, [FromServices] IHostingEnvironment oHostingEnvironment, string mappenavn)
        {
            string InnleggID = "";
            innlegg.EierId = HttpContext.Session.GetString("_UserID");
          
            DateTime today = DateTime.Today;
            DateTime l = today;
            innlegg.Dato = l.ToString("dd/MM/yyyy");


            if (innlegg.Tagger[0] != null)
            {
                var splitTag = innlegg.Tagger[0].Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                

                innlegg.Tagger.Clear();
                for (var i = 0; i < splitTag.Length; i++)
                {

                    innlegg.Tagger.Add(splitTag[i].Trim());
                }
            }
        
        var bruker = firebase.HentEnkeltBruker(innlegg.EierId);

            innlegg.Klokkeslett = DateTime.Now.ToString("h:mm:ss tt");
            Debug.WriteLine("Inni upsert INNlegg POST");
            if (ModelState.IsValid)
            {


                Debug.WriteLine("dwdwadada" + innlegg.Id);

                if (string.IsNullOrEmpty(innlegg.Id))
                { //REgistrer nytt innlegg siden id er null.

                    Boolean mappefinnes = false;

                    Debug.WriteLine("Inni create Innlegg!" + innlegg.EierId);
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
                          InnleggID =  await firebase.RegistrerInnleggMedFil($"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{inputfile.FileName}", inputfile, innlegg);
                          
                            firebase.OppdaterBruker(bruker);
                        }

                        ModelState.AddModelError(string.Empty, "Registrering suksessfult!");
                    }
                    catch (Exception ex)
                    {
                        var str0 = JsonConvert.SerializeObject(bruker);
                        HttpContext.Session.SetString("Innlogget_Bruker", str0);
                        Debug.WriteLine("Inni catch");
                        ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
                        ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
                        ViewData["Innlogget_Bruker"] = bruker;
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(Innlegg);
                    }
                } // Oppdater innlegg siden id finnes!
                else
                {

                    InnleggID = innlegg.Id; 
                    Debug.WriteLine("JEG OPPDATERER    " + innlegg.EierId);
                    for (var i = 0; i < bruker.Mapper.Count; i++)
                    {
                        Debug.WriteLine("oppfdater for runde mappe : " + i);


                        for (var y = 0; y < bruker.Mapper[i].MappeInnhold.Count; y++)
                        {
                            Debug.WriteLine("oppfdater for runde innlegg: " + y);
                            if (bruker.Mapper[i].MappeInnhold[y].Id == InnleggID)
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

            }else
            {
                Debug.WriteLine("Inni invalid Model " );


                var str2 = JsonConvert.SerializeObject(bruker);
                HttpContext.Session.SetString("Innlogget_Bruker", str2);
                ViewData["bruker"] = bruker; 
                ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
                ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
                ViewData["Innlogget_Bruker"] = bruker;

                //Logg ex i debug.
                var message = string.Join(" | ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));

                
                Exception exception = new Exception(message.ToString());
                Debug.WriteLine("Ex: " + exception.Message);

                return View(Innlegg);
            }

            var str = JsonConvert.SerializeObject(bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            ViewData["bruker"] = bruker;
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
            ViewData["Innlogget_Bruker"] = bruker;
            Debug.WriteLine("UPSERT BUNN");
            return RedirectToAction("Nav_Innlegg", new { id = InnleggID });
        }

        //Selve innlegget blir vist frem
        public IActionResult Nav_Innlegg(string id)
        {
            //Skjekker om bruker er logget inn
            var model = new InnleggController();
            
            var token = HttpContext.Session.GetString("_UserToken");
            var isLoggedOn = false;
            if (token != null) isLoggedOn = true;
            ViewBag.Status = isLoggedOn;

           
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
            if (HttpContext.Session.GetString("Innlogget_Bruker") != null)
            {
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }
            //Henter innlegget bruker klikket på
            var innlegg = new Innlegg();
            try
            {
                innlegg = firebase.HentSpesifiktInnlegg(id);
              
             }
            catch (Exception)
            {
                Debug.WriteLine("Jeg er inni catch Innlegg!"); 
                return View(innlegg);
            }

            if (innlegg == null)
                return View(new Innlegg()); 

            var bruker = firebase.HentEnkeltBruker(innlegg.EierId);

            ViewData["bruker"] = bruker;
            return View(innlegg);

        }
       
        //Registrere nyKommentar i innlegget
        public IActionResult NyttKommentar(string tekst, string innleggId)
        {
            
            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            Bruker innBruker = JsonConvert.DeserializeObject<Bruker>(str);
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

            if(!innBruker.kommentertPå.Contains(innleggId))
                innBruker.kommentertPå.Add(innleggId);

            var str0 = JsonConvert.SerializeObject(innBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str0);

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
                    firebase.OppdaterBruker(innBruker);
                    Notifications not = new Notifications("Kommentar", false, innBruker.Id, innBruker.Navn, innlegg.EierId, innlegg.Id, l.ToString("dd/MM/yyyy"));
                    firebase.SendNotification(not);
                  
                }
                
            }
            catch(Exception ex)
            {

            }
            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);
        }

        //registrere nytt svar til kommentar i innlegg
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

            if (!innBruker.kommentertPå.Contains(innleggId))
                innBruker.kommentertPå.Add(innleggId);

            var str0 = JsonConvert.SerializeObject(innBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str0);

            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            var baseKomment = innlegg.Kommentar[kommentId];
            baseKomment.Kommentarer.Add(kommentar);
            innlegg.Kommentar[kommentId] = baseKomment;
            Debug.WriteLine("Dette er reply: " + innleggId + "Oppdaterer innlegg: ");
            try
            {
                if (innlegg.Id != null)
                {

                    firebase.OppdaterInnlegg(innlegg);
                    firebase.OppdaterBruker(innBruker);
                }

            }
            catch (Exception ex)
            {

            }
            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);
        }
                        
        //Slett en kommentar
        public IActionResult DeleteKommentar(string innleggId, int kommentarId)
        {
            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            innlegg.Kommentar.RemoveAt(kommentarId);

            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            Bruker innBruker = JsonConvert.DeserializeObject<Bruker>(str);

            int antall = 0;
            for (int i = 0; i<innlegg.Kommentar.Count; i++)
            {
              if(innlegg.Kommentar[i].EierId == innBruker.Id)
                {
                    antall++; 
                }     
            }

           
            if(antall == 0)
            innBruker.kommentertPå.RemoveAt(innBruker.kommentertPå.IndexOf(innleggId));

            var str0 = JsonConvert.SerializeObject(innBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str0);

            try
            {
                if (innlegg.Id != null)
                {
                    firebase.OppdaterInnlegg(innlegg);
                    firebase.OppdaterBruker(innBruker);
                }

            }
            catch (Exception ex)
            {

            }

            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);
            
        }

        //Slett ett kommentar svar
        public IActionResult DeleteReply(string innleggId, int kommentarId, int replyId)
        {
            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(innleggId);
            innlegg.Kommentar[kommentarId].Kommentarer.RemoveAt(replyId);


            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            Bruker innBruker = JsonConvert.DeserializeObject<Bruker>(str);

            int antall = 0;
            for (int i = 0; i < innlegg.Kommentar.Count; i++)
            {
                if (innlegg.Kommentar[i].EierId == innBruker.Id)
                {
                    for (int j = 0; j < innlegg.Kommentar[i].Kommentarer.Count; j++)
                    {

                        antall++;
                    }

                    antall++;
                }
            }

         
            if (antall == 0)
                innBruker.kommentertPå.RemoveAt(innBruker.kommentertPå.IndexOf(innleggId));

            var str0 = JsonConvert.SerializeObject(innBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str0);

            try
            {
                if (innlegg.Id != null)
                {
                    firebase.OppdaterInnlegg(innlegg);
                    firebase.OppdaterBruker(innBruker);
                }

            }
            catch (Exception ex)
            {

            }

            return Redirect("~/Innlegg/Nav_Innlegg/" + innlegg.Id);

        }

        //Bruker liker ett innlegg
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

        //Bruker fjerner lik
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
