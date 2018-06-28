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
    [System.Web.Http.RoutePrefix("api/Musterija")]
    public class MusterijaController : ApiController
    {
        [HttpPost]
        [Route("KreirajVoznju")]
        public HttpResponseMessage KreirajVoznju([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var ulica = jToken.Value<string>("Ulica");
            var broj = jToken.Value<string>("Broj");
            var mesto = jToken.Value<string>("Mesto");
            var pozivniBroj = jToken.Value<string>("PozivniBroj");
            var xKordinata = jToken.Value<double>("XKordinata");
            var yKordinata = jToken.Value<double>("YKordinata");
            Int32.TryParse(jToken.Value<string>("TipVozila"), out int tip);
            var tipVozila = Models.Enum.TipAutomobila.BezNaznake;

            if (tip == 1)
            {
                tipVozila = Models.Enum.TipAutomobila.Putnicki;
            }
            else if (tip == 2)
            {
                tipVozila = Models.Enum.TipAutomobila.Kombi;
            }


            if (BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme) == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (ulica == null || broj == null || mesto == null || pozivniBroj == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (ulica == "" || broj == "" || mesto == "" || pozivniBroj == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var response = new HttpResponseMessage();
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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Musterija)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme).Blokiran)
            {
                response.Content = new StringContent("Korisnik blokiran!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            var input = pozivniBroj;
            var pattern = new Regex(@"\d{4,8}");
            var match = pattern.Match(input);
            if (!match.Success)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            input = broj;
            pattern = new Regex(@"\d{1,4}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            Voznja novaVoznja = new Voznja
            {
                Lokacija = new Lokacija(),
                Dispecer = new Dispecer(),
                Musterija = new Musterija(),
                Odrediste = new Lokacija()
            };
            novaVoznja.Lokacija.Adresa.Ulica = ulica;
            novaVoznja.Lokacija.Adresa.Broj = broj;
            novaVoznja.Lokacija.Adresa.NasenjenoMesto = mesto;
            novaVoznja.Lokacija.Adresa.PozivniBroj = pozivniBroj;
            novaVoznja.Lokacija.Y = yKordinata;
            novaVoznja.Lokacija.X = xKordinata;
            novaVoznja.TipAutomobila = tipVozila;
            novaVoznja.StatusVoznje = Models.Enum.StatusVoznje.Kreirana;
            novaVoznja.Musterija = BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme);
            novaVoznja.DatumVremePoruzbine = DateTime.Now;         
            novaVoznja.ID = BazaPodataka.Instanca.Voznje.Count + 1;
            BazaPodataka.Instanca.Voznje.Add(novaVoznja);
            BazaPodataka.Instanca.UpisiUBazuVoznje();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpDelete]
        [Route("OtkaziVoznju")]
        public HttpResponseMessage OtkaziVoznju([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            if(!Int32.TryParse(jToken.Value<string>("Id"),out int id))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
                       
            if (korisnickoIme == null )
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (korisnickoIme == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var response = new HttpResponseMessage();
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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Musterija)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            if (BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme).Blokiran)
            {
                response.Content = new StringContent("Korisnik blokiran!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var voznja = BazaPodataka.Instanca.Voznje.Find(v => v.ID == id);
            if (voznja == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (voznja.StatusVoznje != Models.Enum.StatusVoznje.Kreirana)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
            voznja.StatusVoznje = Models.Enum.StatusVoznje.Otkazana;
            BazaPodataka.Instanca.UpisiUBazuVoznje();
            var page = "";
            page += @"<div class=""commentSection"">";
            page += @"<h2>Komentar je obavezan</h2></br>";
            page += @"<textarea placeholder=""Komentar voznje...""></textarea></br>";
            page += @"<label>Ocena:</label><select id=""ocena"">";
            page += @"<option value=""1"">1</option>";
            page += @"<option value=""2"">2</option>";
            page += @"<option value=""3"">3</option>";
            page += @"<option value=""4"">4</option>";
            page += @"<option value=""5"">5</option></select></br>";
            page += String.Format(@"<button id=""komentarisiVoznju"" value=""{0}"">Komentarisi</button></div>", voznja.ID);
            response.Content = new StringContent(page);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        [HttpPut]
        [Route("IzmeniVoznju")]
        public HttpResponseMessage IzmeniVoznju([FromBody]JToken jToken)
        {
            var KorisnickoIme = jToken.Value<string>("KorisnickoIme");
            var Ulica = jToken.Value<string>("Ulica");
            var Broj = jToken.Value<string>("Broj");
            var Mesto = jToken.Value<string>("Mesto");
            var PozivniBroj = jToken.Value<string>("PozivniBroj");

            var response = new HttpResponseMessage();
            if (Mesto == null || Ulica == null || Broj == null || PozivniBroj == null || KorisnickoIme == null)
            {
                response.Content = new StringContent("<label>Greska na serveru</label>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (Mesto == "" || Ulica == "" || Broj == "" || PozivniBroj == "" || KorisnickoIme == "")
            {
                response.Content = new StringContent("<label>Los zahtev!</label>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var input = PozivniBroj;
            var pattern = new Regex(@"\d{4,8}");
            var match = pattern.Match(input);
            if (!match.Success)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            input = Broj;
            pattern = new Regex(@"\d{1,4}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (ulogovani == null)
            {
                response.Content = new StringContent("<label>Greska na serveru</label>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(KorisnickoIme)) == null)
            {
                response.Content = new StringContent("<label>Niste ulogovani!</label>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(KorisnickoIme)).Uloga != Models.Enum.Uloga.Musterija)
            {
                response.Content = new StringContent("<label>Niste autorizovani za ovu opciju!</label>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (BazaPodataka.Instanca.NadjiMusteriju(KorisnickoIme).Blokiran)
            {
                response.Content = new StringContent("Korisnik blokiran!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var voznja = new Voznja();
            foreach (var item in BazaPodataka.Instanca.Voznje)
            {
                if(item.Musterija != null)
                {
                    if(item.Musterija.KorisnickoIme != null)
                    {
                        if (item.Musterija.KorisnickoIme.Equals(KorisnickoIme) && item.StatusVoznje == Models.Enum.StatusVoznje.Kreirana)
                            voznja = item;
                    }
                }
            }

            if (voznja == null)
            {
                response.Content = new StringContent("<label>Voznja je prihvacena u medjuvremenu!</label>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            if (voznja.StatusVoznje != Models.Enum.StatusVoznje.Kreirana)
            {
                response.Content = new StringContent("<label>Voznja je prihvacena u medjuvremenu!</label>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            BazaPodataka.Instanca.Voznje.Remove(voznja);
            voznja.Lokacija.Adresa.Ulica = Ulica;
            voznja.Lokacija.Adresa.Broj = Broj;
            voznja.Lokacija.Adresa.NasenjenoMesto = Mesto;
            voznja.Lokacija.Adresa.PozivniBroj = PozivniBroj;
            BazaPodataka.Instanca.Voznje.Add(voznja);
            response.Content = new StringContent("");
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        [HttpPost]
        [Route("Komentarisi")]
        public HttpResponseMessage Komentarisi([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var komentar = jToken.Value<string>("Komentar");
            Int32.TryParse(jToken.Value<string>("Ocena"), out int ocena);
            Int32.TryParse(jToken.Value<string>("ID"), out int id);
            var response = new HttpResponseMessage();
            komentar = komentar.Replace(';', '.');
            komentar = komentar.Replace('\n', ' ');
            if (korisnickoIme == null || komentar == null)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (ulogovani == null)
            {
                response.Content = new StringContent("Zabranjen pristup!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)) == null)
            {
                response.Content = new StringContent("Zabranjen pristup!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Musterija)
            {
                response.Content = new StringContent("Zabranjen pristup!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            

            var voznja = BazaPodataka.Instanca.Voznje.Find(v => v.ID == id);

            if (voznja == null)
            {
                response.Content = new StringContent("Greska na serveru!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }


            var kom = new Komentar()
            {
                Opis = komentar,
                Ocena = ocena,
                Voznja = voznja.ID,
                DatumObjave = DateTime.Now
            };

            kom.Korisnik = korisnickoIme;

            voznja.Komentar = new Komentar();
            voznja.Komentar = kom;
            BazaPodataka.Instanca.UpisiUBazuVoznje();
            response.Content = new StringContent("Uspesno ostavljen komentar na voznju!");
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}