using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Test1.Models;

namespace Test1.Models
{
    public class Kommentar
    {
        [Key]
        public string Id { get; set; }       
        public string LenkeId { get; set; }
        public string InnleggId { get; set; }
        public string EierId{ get; set; }
        public string EierBilde { get; set; }
        public string EierNavn { get; set; }
        public string Dato { get; set; }
        [Required]
        public string Tekst { get; set; }

        public List<Kommentar> Kommentarer { get; set; }

        public Kommentar()
        {
            Kommentarer = new List<Kommentar>();
        }

    }
}
