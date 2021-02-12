using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portefolio_webApp.Models
{
    public class Bruker
    {

        [Key]
        public string Id { get; set; }
        [Required]
        public string Navn { get; set; }

        [Required]
        public string Epost { get; set; }

        public string Profilbilde { get; set; }

        public int Alder { get; set; }

        public string Stilling { get; set; }

        public string Bosted { get; set; }

        public string Tlf { get; set; }

        public CV CV { get; set; }
    }

}
   
