using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Komentar
    {
        public int ID { get; set; }
        public string Opis { get; set; }
        public DateTime DatumObjave { get; set; }
        public String Korisnik { get; set; }
        public Voznja Voznja { get; set; }
        public int Ocena { get; set; }

        public Komentar()
        {
           
        }
    }
}