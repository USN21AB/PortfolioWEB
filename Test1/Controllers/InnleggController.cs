﻿using FireSharp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
                Debug.WriteLine("------------------------------------upsert innlegg GET INNI IF TOM: " + innleggID);
                return View(Innlegg);
            }

       
            return View(Innlegg);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Upsert_InnleggAsync(Microsoft.AspNetCore.Http.IFormFile file, Innlegg innlegg, [FromServices] IHostingEnvironment oHostingEnvironment, string mappenavn)
        {



            innlegg.EierId = HttpContext.Session.GetString("_UserID");

            Debug.WriteLine("------------------------------------upsert innlegg POST" + mappenavn);
            Console.WriteLine("EYOOOOOOOOOOO BRUUUUH");
            DateTime today = DateTime.Today;
            DateTime l = today;
            innlegg.Dato = l.ToString("dd/MM/yyyy");
            //Hent innlogget person innlegg.EierId = firebase.hentBruker();  


            innlegg.Tagger[1].Split(",");
            var splitTag = innlegg.Tagger[1].Split(",");
            
            innlegg.Tagger.Clear();
            for (var i = 0; i < splitTag.Length; i++)
            {
                innlegg.Tagger.Add(splitTag[i].Trim());
            }

            var bruker = firebase.HentEnkeltBruker(innlegg.EierId);

            innlegg.Klokkeslett = DateTime.Now.ToString("h:mm:ss tt");

            if (ModelState.IsValid)
            {
                
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

                if (mappefinnes ==  false)
                {
                    Debug.WriteLine("Mappen finnes ikke");
                    Portfolio portfolio = new Portfolio(bruker.Id, mappenavn);
                    portfolio.MappeInnhold.Add(innlegg);
                    bruker.Mapper.Add(portfolio);

                }

                if (string.IsNullOrEmpty(innlegg.Id))
                {


                    try
                    {

                        ProfilSideController profilSideController = new ProfilSideController();


                        //logg.Register(oppBruker.Email, oppBruker.Password).Result;
                        //innlegg.EierId = HttpContext.Session.GetString("_UserID");
                        Console.WriteLine("EYOOOOOOOOOOO BRUUUUH");
                        //profilSideController.UploadInnleggFile(file, oHostingEnvironment, innlegg);
                        if (file != null)
                        {
                            await firebase.UploadInnleggFile($"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{file.FileName}", file, innlegg);
                            firebase.OppdaterBrukerAsync(bruker);
                        }
                        else
                        {

                            firebase.RegistrerInnlegg(innlegg);
                            firebase.OppdaterBrukerAsync(bruker);
                        }
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
                    firebase.OppdaterBrukerAsync(bruker);
                }
            }

            var str = JsonConvert.SerializeObject(bruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

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
    }      
}                                                         
