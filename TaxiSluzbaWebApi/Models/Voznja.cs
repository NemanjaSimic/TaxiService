using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Voznja
    {
        public DateTime DatumVremePoruzbine { get; set; }
        public Lokacija Lokacija { get; set; }
        public Enum.TipAutomobila TipAutomobila { get; set; }

        public Musterija Musterija { get; set; }//ako je musterija inicirala voznju
        public Lokacija Odrediste { get; set; } //vozac je zaduzen za update odredista
        public Dispecer Dispecer { get; set; }//ako je formirao ili obradio voznju,ako je vozac prihvaio onda je polje prazno
        public Vozac Vozac { get; set; }// koji je prihvatio ili kojem je dodeljena voznja
        public double Iznos { get; set; }
        public Komentar Komentar { get; set; }
        public Voznja()
        {
            TipAutomobila = Enum.TipAutomobila.BezNaznake;
        }
    }
}