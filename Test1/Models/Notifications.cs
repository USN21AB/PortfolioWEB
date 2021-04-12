using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test1.Models
{
    public class Notifications
    {
        public string type;
        public Boolean erLest;
        public string FraHvemID;
        public string FraHvemNavn;
        public string TilHvemID;
        public string innleggID;
        public string Tidspunkt;
        public Notifications(string type, Boolean erLest, string FraHvemID, string FraHvemNavn, string TilHvemID, string innleggID, string Tidspunkt)
        {
            this.type = type;
            this.erLest = erLest;
            this.FraHvemID = FraHvemID;
            this.FraHvemNavn = FraHvemNavn;
            this.TilHvemID = TilHvemID;
            this.innleggID = innleggID;
            this.Tidspunkt = Tidspunkt;
        }
    }

}
