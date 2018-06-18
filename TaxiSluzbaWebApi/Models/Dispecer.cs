using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class Dispecer : User
    {
        public Dispecer()
        {
            Uloga = Enum.Uloga.Dispecer;
            Voznje = new List<Voznja>();
        }
    }
}