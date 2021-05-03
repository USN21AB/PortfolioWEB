using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Alle mappene til brukerne legges her med dets innhold.
namespace Test1.Models
{
    public class Portfolio
    {

        public string BrukerID { get; set; }

        public string MappeNavn { get; set; }

        public List<Innlegg> MappeInnhold { get; set; }

        public Portfolio(string BrukerID, string MappeNavn)
        {

            this.BrukerID = BrukerID;
            this.MappeNavn = MappeNavn;

            MappeInnhold = new List<Innlegg>();

        }

    }
}
