using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TaxiSluzbaWebApi.Models;

namespace TaxiSluzbaWebApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/Dispecer")]
    public class DispecerController : ApiController
    {
        [System.Web.Http.Route("Registracija")]
        public HttpResponseMessage Registracija([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var sifra = jToken.Value<string>("Sifra");

            Vozac noviVozac = new Vozac();
            noviVozac.Ime = jToken.Value<string>("Ime");
            noviVozac.Prezime = jToken.Value<string>("Prezime");
            noviVozac.JMBG = jToken.Value<string>("JMBG");
            noviVozac.KorisnickoIme = jToken.Value<string>("KorisnickoIme");
            noviVozac.Sifra = jToken.Value<string>("Sifra");
            noviVozac.Email = jToken.Value<string>("Email");
            noviVozac.KontaktTelefon = jToken.Value<string>("KontaktTelefon");
            Int32.TryParse(jToken.Value<string>("Pol"), out int p);
            if (p == 0)
            {
                noviVozac.Pol = Models.Enum.Pol.Muski;
            }
            else
            {
                noviVozac.Pol = Models.Enum.Pol.Zenski;
            }          

            Automobil auto = new Automobil();
            auto.Vozac = noviVozac.KorisnickoIme;
            auto.Godiste = jToken.Value<string>("GodisteAutomobila");
            auto.BrojTaksiVozila = jToken.Value<string>("Id");
            auto.BrojRegistarskeOznake = jToken.Value<string>("Registracija");
            Int32.TryParse(jToken.Value<string>("Tip"), out int t);
            if (t == 1)
            {
                auto.TipAutomobila = Models.Enum.TipAutomobila.Putnicki;
            }
            else
            {
                auto.TipAutomobila = Models.Enum.TipAutomobila.Kombi;
            }

            Adresa adresa = new Adresa();
            adresa.Broj = jToken.Value<string>("BrojKuce");
            adresa.Ulica = jToken.Value<string>("Ulica");
            adresa.PozivniBroj = jToken.Value<string>("PozivniBroj");
            adresa.NasenjenoMesto = jToken.Value<string>("Mesto");

            if (noviVozac.Ime == null || noviVozac.Prezime == null || noviVozac.JMBG == null ||
                noviVozac.KontaktTelefon == null || noviVozac.KorisnickoIme == null || noviVozac.Sifra == null || noviVozac.Email == null ||
                 auto.Godiste == null || auto.BrojRegistarskeOznake == null || auto.BrojTaksiVozila == null || adresa.Broj == null || adresa.Ulica == null ||
                 adresa.PozivniBroj == null || adresa.NasenjenoMesto == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (noviVozac.Ime == "" || noviVozac.Prezime == "" || noviVozac.JMBG == "" ||
                noviVozac.KontaktTelefon == "" || noviVozac.KorisnickoIme == "" || noviVozac.Sifra == "" || noviVozac.Email == "" ||
                 auto.Godiste == "" || auto.BrojRegistarskeOznake == "" || auto.BrojTaksiVozila == "" || adresa.Broj == "" || adresa.Ulica == "" ||
                 adresa.PozivniBroj == "" || adresa.NasenjenoMesto == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var input = noviVozac.KontaktTelefon;
            Regex pattern = new Regex(@"\d{8,10}");
            Match match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = noviVozac.JMBG;
            pattern = new Regex(@"\d{13}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = auto.Godiste;
            pattern = new Regex(@"\d{4}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = auto.BrojRegistarskeOznake;
            pattern = new Regex(@"\d{6,8}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = auto.BrojTaksiVozila;
            pattern = new Regex(@"\d{4}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = adresa.PozivniBroj;
            pattern = new Regex(@"\d{4,8}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = adresa.Broj;
            pattern = new Regex(@"\d{1,4}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            foreach (var item in BazaPodataka.Instanca.Musterije)
            {
                if (noviVozac.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }
            foreach (var item in BazaPodataka.Instanca.Dispeceri)
            {
                if (noviVozac.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (noviVozac.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }

            BazaPodataka.Instanca.Vozaci.Add(noviVozac);
            BazaPodataka.Instanca.UpisiUBazuVozace();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}