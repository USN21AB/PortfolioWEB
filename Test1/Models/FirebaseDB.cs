using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Test1.Models
{

    public class FirebaseDB
    {
        public readonly IFirebaseConfig db;
        public IFirebaseClient klient;


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

        public void RegistrerInnlegg(Innlegg innlegg)
        {
            var data = innlegg;
            PushResponse respons = klient.Push("Innlegg/", data);
            data.Id = respons.Result.name;
            SetResponse setResponse = klient.Set("Innlegg/" + data.Id, data);
        }

        public List<Innlegg> HentAlleInnlegg()
        {
            FirebaseResponse respons = klient.Get("Innlegg");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(respons.Body);
            var list = new List<Innlegg>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Innlegg>(((JProperty)item).Value.ToString()));
            };

            AlleInnlegg = list;
            return AlleInnlegg;
        }

        public async Task UploadFile(string filename, IFormFile file)
        {


            using (var stream = new FileStream(filename, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var stream1 = File.Open(@filename, FileMode.Open);


            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage("bachelor-it-97124.appspot.com")
            .Child("images")
            .Child(file.FileName)
            .PutAsync(stream1);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;
          

            Console.WriteLine("Link " + downloadUrl); }


        public async Task UploadProfilBilde(string filename, IFormFile file, string brukerId)
        {


            using (var stream = new FileStream(filename, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var stream1 = File.Open(@filename, FileMode.Open);


            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage("bachelor-it-97124.appspot.com")
            .Child("images")
            .Child(file.FileName)
            .PutAsync(stream1);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;

            //Putter bilde urlen i ProfilBilde
            UpdateSingleUserValue(brukerId, "Profilbilde", downloadUrl);


            Console.WriteLine("Link " + downloadUrl);
        }

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


        public Innlegg HentSpesifiktInnlegg(string Innlegg_id)
        {

            FirebaseResponse respons = klient.Get("Innlegg/" + Innlegg_id);
            Innlegg returnInnlegg = JsonConvert.DeserializeObject<Innlegg>(respons.Body);


            return returnInnlegg;
        }

        public void UpdateSingleUserValue(string brukerid, string rad,string value)
        {klient.Set("Bruker/" + brukerid + "/"+rad, value);} 

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

        public void RegistrerMappe(Portfolio port)
        {
            PushResponse respons = klient.Push("Portefolio/", port);
            port.BrukerID = respons.Result.name;
            SetResponse setResponse = klient.Set("Portefolio/" + port.BrukerID, port);
        }

        public void RegistrerBruker(Bruker bruker)
        {
       
            PushResponse respons = klient.Push("Bruker/", bruker);
            bruker.Id = respons.Result.name;
            SetResponse setResponse = klient.Set("Bruker/" + bruker.Id, bruker);
               
        }

        public void OppdaterBruker(Bruker bruker)
        {
            Debug.WriteLine("Oppdaterer bruker");
            SetResponse respons = klient.Set("Bruker/"+bruker.Id,bruker);
           // dynamic data = JsonConvert.DeserializeObject<Bruker>(respons.Body);
            //Bruker mellomBruker = JsonConvert.DeserializeObject<Bruker>(((JProperty)data).Value.ToString()); 

        }
    }
}
