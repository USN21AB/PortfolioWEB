﻿using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using LazZiya.ImageResize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// Rest-APIet mot firebase. Her kommuniserer vi med storage og realtime database.
/// Vi har brukt firebase dokumentasjon for å løse noe av koden
/// https://firebase.google.com/docs/database
/// https://firebase.google.com/docs/storage
/// </summary>
namespace Test1.Models
{

    public class FirebaseDB
    {
        public readonly IFirebaseConfig db;
        public IFirebaseClient klient;

        private string croppedProfilImageUrl;
        private string CoverImageUrl;
       


        [BindProperty]
        public List<Innlegg> AlleInnlegg { get; set; }

        public FirebaseDB()
        {
            db = new FirebaseConfig
            {
                AuthSecret = "HIEibEg1o7S9UXwEvMQ0KPLVd8JwZdxZppMLHMUe",
                BasePath = "https://bachelor-it-97124-default-rtdb.europe-west1.firebasedatabase.app/"
            };

            klient = new FireSharp.FirebaseClient(db);
        }

        /// <summary>
        /// Henter ett brukerInnlegg basert på InnleggID
        /// </summary>
        /// <param name="Innlegg_id"></param>
        /// <returns>Det funnede innlegget</returns>
        public Innlegg HentSpesifiktInnlegg(string Innlegg_id)
        {
           
            FirebaseResponse respons = klient.Get("Innlegg/" + Innlegg_id);
            Innlegg returnInnlegg = JsonConvert.DeserializeObject<Innlegg>(respons.Body);
            return returnInnlegg;
        }

        /// <summary>
        /// Henter alle innleggene i databasen
        /// </summary>
        /// <returns>Liste med innlegg</returns>
        public List<Innlegg> HentAlleInnlegg()
        {
            FirebaseResponse respons = klient.Get("Innlegg");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(respons.Body);
            var list = new List<Innlegg>();
            if(data != null)
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Innlegg>(((JProperty)item).Value.ToString()));
            };

            
            AlleInnlegg = list;
            return AlleInnlegg;
        }

        //Laster opp cover foto til innlegg
        public async Task UploadCoverPhoto(string filename, IFormFile file, string inleggID)
        {

                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var stream1 = File.Open(@filename, FileMode.Open);

                // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
                var task = new FirebaseStorage("bachelor-it-97124.appspot.com")
                .Child("Cover")
                .Child(file.FileName)
                .PutAsync(stream1);

                // Track progress of the upload
                task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

                // await the task to wait until upload completes and get the download url
                var downloadUrl = await task;

                Console.WriteLine("Link " + downloadUrl);

                CoverImageUrl = downloadUrl;

                stream1.Close();

                //Slett Lokal Fil etter opplastet

                Console.WriteLine("File Location: " + filename);

                System.IO.File.Delete(filename);

        }

        //Registrer ett innlegg sammen med filen som er valgt
        public async Task<string> RegistrerInnleggMedFil(string filename, IFormFile file, Innlegg innlegg)
        {
                using (var stream = new FileStream(filename, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

           

            var stream1 = File.Open(@filename, FileMode.Open);
            var data = innlegg;

            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage("bachelor-it-97124.appspot.com")
            .Child(data.Kategori)
            .Child(file.FileName + innlegg.Id)
            .PutAsync(stream1);



            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;

            
            Console.WriteLine("Link " + downloadUrl);

            data.IkonURL = downloadUrl;

            Console.WriteLine("COVER URL: " + CoverImageUrl);

            if(CoverImageUrl!="")
            data.CoverURL = CoverImageUrl;

            PushResponse respons = klient.Push("Innlegg/", data);
            data.Id = respons.Result.name;
            SetResponse setResponse = klient.Set("Innlegg/" + data.Id, data);

            stream1.Close();
          

            //Slett Lokal Fil etter opplastet

            Console.WriteLine("File Location: " + filename);

            System.IO.File.Delete(filename);

            return data.Id; 
        }

        //Last opp profibilde til firebase
        public async Task UploadProfilBilde(string filename, string brukerId, string previousprofilepic)
        {


            var stream1 = File.Open(filename, FileMode.Open);

            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage("bachelor-it-97124.appspot.com")
            .Child("ProfilBilde")
            .Child(brukerId)
            .PutAsync(stream1);
            


            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;

            Console.WriteLine("URL TIL BILDET: " + downloadUrl);


            croppedProfilImageUrl = downloadUrl;
            stream1.Close();


            Console.WriteLine("Previous File Name: " + previousprofilepic + "\n" + "ProfileName Now: " + stream1.Name);

            //Slett Lokal Fil etter opplastet

            System.IO.File.Delete(filename);

            
        }

        //Sorter alle innleggene basert på valgt kategori
        public List<Innlegg> SorterAlleInnlegg(string Type, List<Innlegg> liste)
        {
            if (Type.Equals("alt"))
                return liste;

            var SortertListe = new List<Innlegg>();
            if (liste != null)
                foreach (var item in liste)
                {
                    if (item.Kategori.Equals(Type))
                    {
                        SortertListe.Add(item);
                    }
                }
            return SortertListe;
        }

        //Oppdater en enkelt verdi i datbasen
        public void UpdateSingleUserValue(string brukerid, string rad,string value)
        {
            klient.Set("Bruker/" + brukerid + "/"+rad, value);
        } 

        //Hent en enkelt bruker basert på brukerID
        public Bruker HentEnkeltBruker(string bruker_id)
        {

            try { 
            FirebaseResponse respons = klient.Get("Bruker/"+bruker_id);
            Bruker returnBruker = JsonConvert.DeserializeObject<Bruker>(respons.Body);

                return returnBruker;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null; 
            }

        }

        //Hent alle mappene til en bruker
        public Portfolio HentAlleMapper(string bruker_id)
        {

            try
            {
                FirebaseResponse respons = klient.Get("Portefolio/" + "-MTuw28LJjRnN0ncHHx9");
                Portfolio returnBruker = JsonConvert.DeserializeObject<Portfolio>(respons.Body);

                return returnBruker;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

        }

        //Registrer en ny mappe i firebase
        public void RegistrerMappe(Portfolio port)
        {
            PushResponse respons = klient.Push("Portefolio/", port);
            port.BrukerID = respons.Result.name;
            SetResponse setResponse = klient.Set("Portefolio/" + port.BrukerID, port);
        }

        //Registrer en ny bruker
        public async Task RegistrerBruker(Bruker bruker)
        {
            bruker.Profilbilde = "https://firebasestorage.googleapis.com/v0/b/bachelor-it-97124.appspot.com/o/DefaultProfileImage%2Fdefault_account.jpg?alt=media&token=e0172d4b-dd3f-410e-9405-bb316c3f4e36";            //PushResponse respons = klient.Push("Bruker /"+bruker.Id, bruker);
            // bruker.Id = respons.Result.name;
            SetResponse setResponse = klient.Set("Bruker/" + bruker.Id, bruker);
        }

        //Sender en notifacation til en bruker
        public void SendNotification(Notifications notification)
        {
            if(notification.FraHvemID != notification.TilHvemID) {
              Bruker bruker = HentEnkeltBruker(notification.TilHvemID);
              bruker.NumberOfNotifications += 1;

              bruker.notifications.Add(notification);
              SetResponse setResponse = klient.Set("Bruker/" + notification.TilHvemID, bruker);
            }
        }

        //Oppdaterer en bruker
        public void OppdaterBruker(Bruker bruker)
        {
            SetResponse respons = klient.Set("Bruker/" + bruker.Id, bruker);
        }

        //teller antall notifications i databasen og leverer tilbake svaret. 
        //Brukes for å finne ut om det er nye notifications som skal sendes til bruker
        public int TellAntallRader(string brukerID)
        {
            FirebaseResponse respons = klient.Get("Bruker/" + brukerID + "/NumberOfNotifications");
            
            if (respons.Body == null)
                return -1;
            int length = JsonConvert.DeserializeObject<int>(respons.Body); 
        
            return length;
        }

        //Oppdater brukerens bilde
        public void OppdaterBrukerBilde(Bruker bruker) 
        {

            bruker.Profilbilde = croppedProfilImageUrl;

            if (bruker.Profilbilde == "")
            {
                bruker.Profilbilde = "https://firebasestorage.googleapis.com/v0/b/bachelor-it-97124.appspot.com/o/images%2Fdefault_account.jpg?alt=media&token=290b6907-f17e-4095-90a6-dca2c52563b9";
            }




            SetResponse respons = klient.Set("Bruker/"+bruker.Id,bruker);
        }

        //Endre passord til bruker
        public void OppdaterAuth(string gammelEmail, string Email, string Password, string GammelPassord)
        {
            klient.ChangePassword(Email,GammelPassord, Password);
        }

        // oppdaterer ett innlegg 
        public void OppdaterInnlegg(Innlegg innlegg)
        {
            SetResponse respons = klient.Set("Innlegg/" + innlegg.Id, innlegg);
        }

        //registrer en kommentar
        public void RegistrerKommentar(Kommentar kommentar)
        {
            var link = "Innlegg/" + kommentar.InnleggId + "/Kommentarer/";
            PushResponse respons = klient.Push(link, kommentar);
            kommentar.Id = respons.Result.name;
            SetResponse setResponse = klient.Set(link + kommentar.Id, kommentar);
        }

        //sletter en kommentar
        public void SlettInnlegg(string innleggID)
        {
            FirebaseResponse respons = klient.Delete("Innlegg/" + innleggID);
        }

        //Sletter ett mappeinnlegg
        public void SlettMappeInnlegg(string brukerID, int mappeIndex, int innleggId)
        {
            FirebaseResponse respons = klient.Delete("Bruker/" + brukerID + "/Mapper/" + mappeIndex + "/MappeInnhold/" + innleggId);
        }

        //En algoritme som finner ut hvem som er det mest populære brukerne på nettsiden
        //de 8 mest populære brukerne skrevet ut i stigende rekkefølge.
        public List<Bruker> updateAlgorithm()
        {
            FirebaseResponse respons = klient.Get("Bruker");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(respons.Body);
            var list = new List<Bruker>();
            if (data != null)
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Bruker>(((JProperty)item).Value.ToString()));
                };

            List<Bruker> SortedList = list.OrderBy(o => o.likeRatio).ToList();

            if(SortedList.Count >= 8)
            SortedList.RemoveRange(0, (SortedList.Count - 8));
           
            return SortedList;
        }
    }
}
