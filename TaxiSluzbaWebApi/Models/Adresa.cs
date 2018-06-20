using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Adresa
    {
        public string Ulica { get; set; }
        public String Broj { get; set; }
        public string NasenjenoMesto { get; set; }
        public String PozivniBroj { get; set; }

    }
}