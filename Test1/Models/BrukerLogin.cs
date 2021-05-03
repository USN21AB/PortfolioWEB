using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// Logg inn bruker model for hjelp til å sette opp autentication
/// </summary>
namespace Test1.Models
{
    public class BrukerLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
