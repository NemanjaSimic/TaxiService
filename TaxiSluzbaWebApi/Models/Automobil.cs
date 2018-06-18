using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Automobil
    {
        public Vozac Vozac { get; set; }
        public UInt32 Godiste { get; set; }
        public UInt32 BrojRegistarskeOznake { get; set; }
        public UInt32 BrojTaksiVozila { get; set; }
        public Enum.TipAutomobila TipAutomobila { get; set; }

    }
}