using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Test1.Models;

namespace Portefolio_webApp.Models
{
    public class RecentUploadList
    {
        public string Id { get; set; }

        public string Dato { get; set; }

        public string Klokkeslett { get; set; }
    }
}
