using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Enum
    {
        public enum Pol
        {
            Muski,
            Zenski
        }

        public enum Uloga
        {
            Musterija,
            Vozac,
            Dispecer
        }

        public enum TipAutomobila
        {
            BezNaznake,
            Putnicki,
            Kombi
        }

        public enum StatusVoznje
        {
            Kreirana,
            Formirana,
            Obradjena,
            Prihvacena,
            Otkazana,
            Neuspesna,
            Uspesna
        }
    }
}