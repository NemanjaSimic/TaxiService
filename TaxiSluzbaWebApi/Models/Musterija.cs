using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Musterija : User
    {
        public bool Blokiran { get; set; } = false;
        public Musterija()
        {
            Uloga = Enum.Uloga.Musterija;
            Voznje = new List<Voznja>();
        }
    }
}