﻿using FireSharp.Interfaces;
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
            Debug.WriteLine("id er:  " + id);
            var innlegg = new Innlegg();
            innlegg = firebase.HentSpesifiktInnlegg(id);
            return View(innlegg);
            
        }

    }

}
