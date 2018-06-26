using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Vozac : User
    {
        public Lokacija Lokacija { get; set; }
        public Automobil Automobil { get; set; }
        public bool Zauzet { get; set; } = false;
        public bool Komentarisao { get; set; } = true;
        public Vozac()
        {
            Uloga = Enum.Uloga.Vozac;
            Voznje = new List<Voznja>();
            Lokacija = new Lokacija();
        }
    }
}