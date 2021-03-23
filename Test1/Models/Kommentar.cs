using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class Kommentar
    {
        [Key]
        public string Id { get; set; }       
        public string LenkeId { get; set; }
        public string InnleggId { get; set; }
        public string EierId{ get; set; }
        public string Eierbilde { get; set; }
        public string Dato { get; set; }
        [Required]
        public string Tekst { get; set; }       
    }
}
