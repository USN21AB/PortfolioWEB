using System.Collections.Generic;
using Test1.Models;

namespace Portefolio_webApp.Models
{
    public class Innlegg
    {
        public string Id { get; set; }
        public string Tittel { get; set; }
        public string Dato { get; set; }
        public string Klokkeslett { get; set; }
        public  string EierId { get; set; }

        public string Beskrivelse { get; set; }

        public string Kategori { get; set; }

        public string SubKategori { get; set; }

        public List<string> Tagger { get; set; }


        public string IkonURL { get; set; }

        public string CoverURL { get; set; }

        public double FilStørrelse { get; set; }

        public List<Kommentar> Kommentar { get; set; }

        public Innlegg()
        {
            Kommentar = new List<Kommentar>();
        }


    }
}
