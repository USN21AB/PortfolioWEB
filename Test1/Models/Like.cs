using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Portefolio_webApp.Models;

namespace Test1.Models
{
    public class Like
    {
        public string Id { get; set; }
        public int Antall { get; set; }
        public  List<string> Brukere { get; set; }

        public Like()
        {
            Brukere = new List<string>();
        }
    }
}
