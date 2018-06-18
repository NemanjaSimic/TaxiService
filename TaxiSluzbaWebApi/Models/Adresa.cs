using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Adresa
    {
        public string Ulica { get; set; }
        public UInt32 Broj { get; set; }
        public string NasenjenoMesto { get; set; }
        public UInt32 PozivniBroj { get; set; }

    }
}