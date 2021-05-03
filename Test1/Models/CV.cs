using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// CV til bruker. 
/// </summary>
namespace Portefolio_webApp.Models
{
    public class CV
    {
        public string BrukerID { get; set; }
        public string SelvBeskrivelse { get; set; }
        public List<string> Utdanning { get; set; }
        public List<string> ArbeidsErfaring { get; set; }
        public List<string> Ferdigheter { get; set; }
        public List<string> Språk { get; set; }
        public List<string> Hobbyer { get; set; }
        public List<string> Referanser { get; set; }
        public List<string> Sertifikater { get; set; }
        public CV() 
        { 
            Utdanning = new List<string>(); 

            ArbeidsErfaring = new List<string>();

            Ferdigheter = new List<string>();

            Språk = new List<string>();

            Hobbyer = new List<string>();

            Referanser = new List<string>();

            Sertifikater = new List<string>();
        }

    }
}
