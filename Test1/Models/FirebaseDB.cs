using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
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

        List<Innlegg> AlleInnlegg { get; set; }

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

            Debug.WriteLine("Hellooooooooo-----------------------------------------------------------");

            AlleInnlegg = list; 
            return AlleInnlegg; 
        }

        public async void UploadFile(IFormFile file)
        {

            var filePath = Path.GetTempFileName();

              
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
                    var stream1 = File.Open(@filePath, FileMode.Open);


            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage("bachelor-it-97124.appspot.com")
            .Child("images")
            .Child(file.FileName)
            .PutAsync(stream1);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;

            Console.WriteLine("" + downloadUrl);}


    




public List<Innlegg> SorterAlleInnlegg(string Type)
        {
            var SortertListe = new List<Innlegg>(); 
            
            foreach(var item in AlleInnlegg)
            {
                if (item.Kategori.Equals(Type))
                {
                    SortertListe.Add(item); 
                }
            }

            return SortertListe; 
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
            SetResponse setResponse = klient.Set("Bruker/" + bruker.Id, bruker);
        }
    }
}
