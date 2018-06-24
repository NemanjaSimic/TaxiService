using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using TaxiSluzbaWebApi.Models;

namespace TaxiSluzbaWebApi.Controllers
{
    [RoutePrefix("api/Dispecer")]
    public class DispecerController : ApiController
    {   
        [HttpPost]
        [Route("Registracija")]
        public HttpResponseMessage Registracija([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var sifra = jToken.Value<string>("Sifra");

            Vozac noviVozac = new Vozac
            {
                Ime = jToken.Value<string>("Ime"),
                Prezime = jToken.Value<string>("Prezime"),
                JMBG = jToken.Value<string>("JMBG"),
                KorisnickoIme = jToken.Value<string>("KorisnickoIme"),
                Sifra = jToken.Value<string>("Sifra"),
                Email = jToken.Value<string>("Email"),
                KontaktTelefon = jToken.Value<string>("KontaktTelefon")
            };
            Int32.TryParse(jToken.Value<string>("Pol"), out int p);
            if (p == 0)
            {
                noviVozac.Pol = Models.Enum.Pol.Muski;
            }
            else
            {
                noviVozac.Pol = Models.Enum.Pol.Zenski;
            }          

            //Automobil auto = new Automobil();
            //auto.Vozac = new Vozac();
            //auto.Vozac = noviVozac;
            //auto.Godiste = jToken.Value<string>("GodisteAutomobila");
            //auto.BrojTaksiVozila = jToken.Value<string>("Id");
            //auto.BrojRegistarskeOznake = jToken.Value<string>("Registracija");
            //Int32.TryParse(jToken.Value<string>("Tip"), out int t);
            //if (t == 1)
            //{
            //    auto.TipAutomobila = Models.Enum.TipAutomobila.Putnicki;
            //}
            //else
            //{
            //    auto.TipAutomobila = Models.Enum.TipAutomobila.Kombi;
            //}

            //noviVozac.Automobil = auto;

            //Adresa adresa = new Adresa();
            //adresa.Broj = jToken.Value<string>("BrojKuce");
            //adresa.Ulica = jToken.Value<string>("Ulica");
            //adresa.PozivniBroj = jToken.Value<string>("PozivniBroj");
            //adresa.NasenjenoMesto = jToken.Value<string>("Mesto");

            //Lokacija lokacija = new Lokacija();
            //lokacija.Adresa = adresa;

            //noviVozac.Lokacija = lokacija;

            if (noviVozac.Ime == null || noviVozac.Prezime == null || noviVozac.JMBG == null ||
                noviVozac.KontaktTelefon == null || noviVozac.KorisnickoIme == null || noviVozac.Sifra == null || noviVozac.Email == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (noviVozac.Ime == "" || noviVozac.Prezime == "" || noviVozac.JMBG == "" ||
                noviVozac.KontaktTelefon == "" || noviVozac.KorisnickoIme == "" || noviVozac.Sifra == "" || noviVozac.Email == "")
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

            //input = auto.Godiste;
            //pattern = new Regex(@"\d{4}");
            //match = pattern.Match(input);
            //if (!match.Success)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            //input = auto.BrojRegistarskeOznake;
            //pattern = new Regex(@"\d{6,8}");
            //match = pattern.Match(input);
            //if (!match.Success)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            //input = auto.BrojTaksiVozila;
            //pattern = new Regex(@"\d{4}");
            //match = pattern.Match(input);
            //if (!match.Success)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            if (BazaPodataka.Instanca.NadjiDispecera(noviVozac.KorisnickoIme) != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
            if (BazaPodataka.Instanca.NadjiMusteriju(noviVozac.KorisnickoIme) != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
            if (BazaPodataka.Instanca.NadjiVozaca(noviVozac.KorisnickoIme) != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }


            BazaPodataka.Instanca.Vozaci.Add(noviVozac);
            BazaPodataka.Instanca.UpisiUBazuVozace();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Route("KreirajVoznju")]
        public HttpResponseMessage KreirajVoznju([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var Mesto = jToken.Value<string>("Mesto");
            var Ulica = jToken.Value<string>("Ulica");
            var Broj = jToken.Value<string>("Broj");
            var PozivniBroj = jToken.Value<string>("PozivniBroj");
            var KorisnickoImeV = jToken.Value<string>("KorisnickoImeV");
            var TipVozila = Models.Enum.TipAutomobila.BezNaznake;
            Int32.TryParse(jToken.Value<string>("TipVozila"), out int t);
            if (t == 1)
            {
                TipVozila = Models.Enum.TipAutomobila.Putnicki;
            }
            else if (t == 2)
            {
                TipVozila = Models.Enum.TipAutomobila.Kombi;
            }

            if (BazaPodataka.Instanca.NadjiDispecera(korisnickoIme) == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (Mesto == null || Ulica == null || Broj == null || PozivniBroj == null || KorisnickoImeV == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (Mesto == "" || Ulica == "" || Broj == "" || PozivniBroj == "" || KorisnickoImeV == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            Voznja novaVoznja = new Voznja
            {
                ID = BazaPodataka.Instanca.Voznje.Count(),
                DatumVremePoruzbine = DateTime.Now,
                Lokacija = new Lokacija()
            };
            novaVoznja.Lokacija.Adresa = new Adresa
            {
                Broj = Broj,
                NasenjenoMesto = Mesto,
                PozivniBroj = PozivniBroj,
                Ulica = Ulica
            };
            novaVoznja.StatusVoznje = Models.Enum.StatusVoznje.Formirana;
            novaVoznja.TipAutomobila = TipVozila;
            novaVoznja.Dispecer = new Dispecer();
            novaVoznja.Dispecer = BazaPodataka.Instanca.NadjiDispecera(korisnickoIme);

            if (HttpContext.Current.Application["noveVoznje"] == null)
            {
                HttpContext.Current.Application["noveVoznje"] = new List<Voznja>();
            }
            var listaVoznji = (List<Voznja>)HttpContext.Current.Application["noveVoznje"];

            listaVoznji.Add(novaVoznja);

            return Request.CreateResponse(HttpStatusCode.OK,novaVoznja);
        }
        [HttpGet]
        [Route("Obradi")]
        public HttpResponseMessage Obradi()
        {
            var response = new HttpResponseMessage();
            var informations = "";
            var listaVoznji = (List<Voznja>)HttpContext.Current.Application["noveVoznje"];
            if (listaVoznji == null)
            {
                informations += "<h1>NEMA ZAHTEVA ZA VOZNJU!</h1>";
            }
            else
            {
                foreach (var item in listaVoznji)
                {
                    informations += String.Format(@"<table><tr><td>Ulica:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Ulica);
                    informations += String.Format(@"<tr><td>Broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Broj);
                    informations += String.Format(@"<tr><td>Mesto:</td><td>{0}</td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                    informations += String.Format(@"<tr><td>Pozivni broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.PozivniBroj);
                    informations += String.Format(@"<tr><td></td><td><button class=""obradi"" value=""{0}"">Obradi voznju</button></td></tr>", item.ID);
                    informations += "</table>";
                }
            }
            response.Content = new StringContent(informations);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            return response;
        }
        [HttpGet]
        [Route("Obradi/{id:int}")]
        public HttpResponseMessage Obradi(int id)
        {
            var response = new HttpResponseMessage();
            var informations = "";
            var listaSlobodnihVozaca = new List<Vozac>();
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (!item.Zauzet)
                {
                    listaSlobodnihVozaca.Add(item);
                }
            }

            var listaVoznji = (List<Voznja>)HttpContext.Current.Application["noveVoznje"];
            if (listaVoznji == null)
            {
                informations += "<h1>ZAHTEV JE PRIHVACEN ILI OTKAZAN!</h1>";
            }
            else
            {
                foreach (var item in listaVoznji)
                {
                    if (item.ID == id)
                    {
                        informations += String.Format(@"<table><tr><td>Ulica:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Ulica);
                        informations += String.Format(@"<tr><td>Broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Broj);
                        informations += String.Format(@"<tr><td>Mesto:</td><td>{0}</td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                        informations += String.Format(@"<tr><td>Pozivni broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.PozivniBroj);
                        informations += String.Format(@"<tr><td>Biraje vozaca:</td><td><select>");
                        foreach (var v in listaSlobodnihVozaca)
                        {
                            informations += String.Format(@"<option>{0}</option>",v.KorisnickoIme);
                        }
                        informations += String.Format(@"</select></td></tr>");
                        informations += String.Format(@"<tr><td></td><td><button class=""obradi"" value=""{0}"">Obradi voznju</button></td></tr>", item.ID);
                        informations += "</table>";
                        response.Content = new StringContent(informations);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        return response;
                    }                    
                }
            }
            informations += "<h1>ZAHTEV JE PRIHVACEN ILI OTKAZAN!</h1>";
            response.Content = new StringContent(informations);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}



