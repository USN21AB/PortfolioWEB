using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public List<string> Serfikater { get; set; }


    }
}
