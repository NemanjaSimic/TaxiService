using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Automobil
    {
        public Vozac Vozac { get; set; }
        public String Godiste { get; set; }
        public String BrojRegistarskeOznake { get; set; }
        public String BrojTaksiVozila { get; set; }
        public Enum.TipAutomobila TipAutomobila { get; set; }
        public Automobil()
        {
            
        }

    }
}