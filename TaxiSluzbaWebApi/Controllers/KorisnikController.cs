using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TaxiSluzbaWebApi.Models;

namespace TaxiSluzbaWebApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/Korisnik")]
    public class KorisnikController : ApiController
    {

        ///api/Korisnik/LogIn
        [System.Web.Http.Route("LogIn")]
        public User LogIn([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("korisnickoIme");
            var sifra = jToken.Value<string>("sifra");

            foreach (var item in BazaPodataka.Instanca.Dispeceri)
            {
                if (item.KorisnickoIme.Equals(korisnickoIme))
                {
                    if (item.Sifra.Equals(sifra))
                    {
                        return item;
                    }
                }
            }
            foreach (var item in BazaPodataka.Instanca.Musterije)
            {
                if (item.KorisnickoIme.Equals(korisnickoIme))
                {
                    if (item.Sifra.Equals(sifra))
                    {
                        return item;
                    }
                }
            }
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (item.Sifra.Equals(sifra))
                {
                    return item;
                }
            }
            return null;
        }
    }
}