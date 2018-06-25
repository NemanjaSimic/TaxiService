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

        [HttpGet]
        [Route("Voznja/{korisnickoIme}")]
        public HttpResponseMessage Voznja(string korisnickoIme)
        {
            //var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            //var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");
            var informations = "";
            var response = new HttpResponseMessage();
            if (korisnickoIme == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            if (korisnickoIme == "")
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }


            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (ulogovani == null)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)) == null)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Dispecer)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            var listaSlobodnihVozaca = new List<Vozac>();
            foreach (var item in ulogovani)
            {
                if (item.Uloga == Models.Enum.Uloga.Vozac)
                {
                    listaSlobodnihVozaca.Add(((Vozac)item));
                }
            }

            informations = @"<table><tr><td>Ulica:</td><td><input type=""text"" id=""ulica"" placeholder=""npr.Bulevar Oslobodjenja"" autocomplete=""off"" /></td></tr>";
            informations += @"<tr><td>Broj kuce/zgrade:</td><td><input type=""text"" id = ""brojKuce"" placeholder=""npr.147"" autocomplete=""off"" /></td></tr>";
            informations += @"<tr><td>Naseljeno mesto:</td><td><input type=""text"" id=""mesto"" placeholder=""npr.Novi Sad"" autocomplete=""off"" /></td></tr>";
            informations += @"<tr><td>Postanski broj:</td><td><input type=""text"" id=""pozivniBroj"" placeholder=""npr.21000"" autocomplete=""off"" /></td></tr>";
            informations += @"<tr><td>Zeljeni tip vozila:</td><td><select id=""tip""><option value=""0"">-</option><option value=""1"">Putnicki</option><option value=""2"">Kombi</option></select></td></tr>";
            informations += @"<tr><td>Korisnicko ime vozaca:</td><td><select id=""vozac"">";
            foreach (var v in listaSlobodnihVozaca)
            {
                informations += String.Format(@"<option val=""{0}"">{1}</option>", v.KorisnickoIme, v.KorisnickoIme);
            }
            informations += String.Format(@"</select></td></tr>");
            informations += @"<tr><td></td><td><button id=""kreirajVoznjuD"">Kreiraj voznju</ button></td></tr></table>";
            informations += @"<div id=""regVal""></div>";

            response.Content = new StringContent(informations);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            return response;
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
            var vozac = jToken.Value<string>("Vozac");
            var response = new HttpResponseMessage();
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

            if (Mesto == null || Ulica == null || Broj == null || PozivniBroj == null || KorisnickoImeV == null || vozac == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (Mesto == "" || Ulica == "" || Broj == "" || PozivniBroj == "" || KorisnickoImeV == "" || vozac == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (ulogovani == null)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)) == null)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Dispecer)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(vozac)) == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(vozac)).Uloga != Models.Enum.Uloga.Vozac)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
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
            var vozacTemp = BazaPodataka.Instanca.Vozaci.Find(v => v.KorisnickoIme.Equals(vozac));
            BazaPodataka.Instanca.Vozaci.Remove(vozacTemp);
            vozacTemp.Zauzet = true;
            BazaPodataka.Instanca.Vozaci.Add(vozacTemp);
            novaVoznja.StatusVoznje = Models.Enum.StatusVoznje.Formirana;
            novaVoznja.TipAutomobila = TipVozila;
            novaVoznja.Dispecer = new Dispecer();
            novaVoznja.Dispecer = BazaPodataka.Instanca.NadjiDispecera(korisnickoIme);
            novaVoznja.ID = BazaPodataka.Instanca.Voznje.Count + 1;
            novaVoznja.Vozac = new Vozac();
            novaVoznja.Vozac = vozacTemp;
            BazaPodataka.Instanca.Voznje.Add(novaVoznja);

            return Request.CreateResponse(HttpStatusCode.OK,novaVoznja);
        }
        [HttpGet]
        [Route("Obradi")]
        public HttpResponseMessage Obradi()
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
            var listaVoznji = new List<Voznja>();
            foreach (var item in BazaPodataka.Instanca.Voznje)
            {
                if (item.StatusVoznje == Models.Enum.StatusVoznje.Kreirana)
                {
                    listaVoznji.Add(item);
                }
            }
            if (listaVoznji.Count == 0)
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
                    informations += String.Format(@"<tr><td>Izaberite vozaca:</td><td><select id=""idVozaca"">");
                    foreach (var v in listaSlobodnihVozaca)
                    {
                        informations += String.Format(@"<option val=""{0}"">{1}</option>", v.KorisnickoIme, v.KorisnickoIme);
                    }
                    informations += String.Format(@"</select></td></tr>");
                    informations += String.Format(@"<tr><td></td><td><button class=""obradi"" value=""{0}"">Obradi voznju</button></td></tr>", item.ID);
                    informations += "</table>";
                }
            }
            response.Content = new StringContent(informations);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            return response;
        }
        [HttpPut]
        [Route("Obradi")]
        public HttpResponseMessage Obradi([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var Id = jToken.Value<string>("ID");
            var vozac = jToken.Value<string>("Vozac");
            var response = new HttpResponseMessage();
            if (Id == null || vozac == null || korisnickoIme == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            if (Id == "" || vozac == "" || korisnickoIme == "")
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }


            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (ulogovani == null)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)) == null)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Dispecer)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            Int32.TryParse(Id, out int id);
            var vozacTemp = BazaPodataka.Instanca.Vozaci.Find(v => v.KorisnickoIme.Equals(vozac));
            if (vozacTemp == null)
            {
                response.Content = new StringContent("Greska na serveru!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (vozacTemp.Zauzet)
            {
                response.Content = new StringContent("Vozac je u medjuvremenu prihvatio drugu voznju!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }

            var listaVoznji = new List<Voznja>();
            foreach (var item in BazaPodataka.Instanca.Voznje)
            {
                if (item.StatusVoznje == Models.Enum.StatusVoznje.Kreirana)
                {
                    listaVoznji.Add(item);
                }
            }
            if (listaVoznji.Count == 0)
            {
                response.Content = new StringContent("Voznja je u medjuvremenu obradjena!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }

            var voznjaTemp = listaVoznji.Find(v => v.ID == id);

            if (voznjaTemp == null)
            {
                response.Content = new StringContent("Voznja je u medjuvremenu obradjena!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }

            if (voznjaTemp.StatusVoznje != Models.Enum.StatusVoznje.Kreirana)
            {
                response.Content = new StringContent("Voznja je u medjuvremenu prhvacena ili otkazana!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }

            BazaPodataka.Instanca.Vozaci.Remove(vozacTemp);
            vozacTemp.Zauzet = true;
            BazaPodataka.Instanca.Vozaci.Add(vozacTemp);

            BazaPodataka.Instanca.Voznje.Remove(voznjaTemp);
            voznjaTemp.StatusVoznje = Models.Enum.StatusVoznje.Obradjena;
            voznjaTemp.Vozac = new Vozac();
            voznjaTemp.Vozac = vozacTemp;
            voznjaTemp.Dispecer = new Dispecer();
            voznjaTemp.Dispecer = BazaPodataka.Instanca.NadjiDispecera(korisnickoIme);
            BazaPodataka.Instanca.Voznje.Add(voznjaTemp);

            response.Content = new StringContent("Voznja je uspesno obradjena!");
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}



