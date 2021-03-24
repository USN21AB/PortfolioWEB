using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public double FilStørrelse { get; set; }

     

    }
}
