using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Adresa
    {
        public String Ulica { get; set; }
        public String Broj { get; set; }
        public String NasenjenoMesto { get; set; }
        public String PozivniBroj { get; set; }

        public Adresa()
        {

        }

    }
}