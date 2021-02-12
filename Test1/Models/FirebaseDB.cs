using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    }
}
