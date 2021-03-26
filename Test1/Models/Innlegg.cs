using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Test1.Models;

namespace Portefolio_webApp.Models
{
    public class Innlegg
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Tittel { get; set; }
        public string Dato { get; set; }
        public string Klokkeslett { get; set; }
        public  string EierId { get; set; }

        public string Beskrivelse { get; set; }

        [Required]
        public string Kategori { get; set; }

        public string SubKategori { get; set; }

        public List<string> Tagger { get; set; }

        public string IkonURL { get; set; }

        public double FilStørrelse { get; set; }

        public List<Kommentar> Kommentar { get; set; }

        public Innlegg()
        {
            Kommentar = new List<Kommentar>();
        }


    }
}
