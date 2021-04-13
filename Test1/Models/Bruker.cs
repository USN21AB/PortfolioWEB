using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Test1.Models;

namespace Portefolio_webApp.Models
{
    public class Bruker
    {

        [Key]
        public string Id { get; set; }
        [MaxLength(30)]
        [Required]
        public string Navn { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Bio { get; set; }

        public string Profilbilde { get; set; }

        [Range(10, 100)]
        public int Alder { get; set; }

        public string Stilling { get; set; }

        public string MessageToken { get; set; }

        public string Bosted { get; set; }

        [Phone]
        public string Tlf { get; set; }

        public double likeRatio { get; set; }

        public List<string> CVAdgang { get; set; }
        public int NumberOfNotifications { get; set; }

        public List<Notifications> notifications { get; set; }

        public List<string> kommentertPå { get; set; }

        public CV CV { get; set; }
        public List<Portfolio> Mapper { get; set; }

        public Bruker()
        {
            CV = new CV(); 
            Mapper = new List<Portfolio>();
            notifications = new List<Notifications>(); 
        }

    }

}
   
