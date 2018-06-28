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
    [RoutePrefix("api/Vozac")]
    public class VozacController : ApiController
    {
        
        [HttpPut]
        [Route("PromenaLokacije")]
        public HttpResponseMessage PromenaLokacije([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var Mesto = jToken.Value<string>("Mesto");
            var Ulica = jToken.Value<string>("Ulica");
            var Broj = jToken.Value<string>("BrojKuce");
            var PozivniBroj = jToken.Value<string>("PozivniBroj");
            var xKordinata = jToken.Value<double>("XKordinata");
            var yKordinata = jToken.Value<double>("YKordinata");
            var response = new HttpResponseMessage();

            if (Mesto == null || Ulica == null || Broj == null || PozivniBroj == null || korisnickoIme == null)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (Mesto == "" || Ulica == "" || Broj == "" || PozivniBroj == "" || korisnickoIme == "")
            {
                response.Content = new StringContent("Los zahtev!");
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

            var vozac = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);
            if (vozac == null)
            {
                response.Content = new StringContent("Zabranjen pristup!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (vozac.Blokiran)
            {
                response.Content = new StringContent("Korisnik blokiran!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            BazaPodataka.Instanca.Vozaci.Remove(vozac);
            vozac.Lokacija = new Lokacija
            {
                Adresa = new Adresa()
            };
            vozac.Lokacija.Adresa.Ulica = Ulica;
            vozac.Lokacija.Adresa.Broj = Broj;
            vozac.Lokacija.Adresa.NasenjenoMesto = Mesto;
            vozac.Lokacija.X = xKordinata;
            vozac.Lokacija.Y = yKordinata;
            vozac.Lokacija.Adresa.PozivniBroj = PozivniBroj;
            BazaPodataka.Instanca.Vozaci.Add(vozac);
            BazaPodataka.Instanca.UpisiUBazuVozace();

            response.Content = new StringContent("Uspesno sacuvana promena");
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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Vozac)
            {
                response.Content = new StringContent("Zabranjen pristup!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var vozac = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);

            var voznja = BazaPodataka.Instanca.Voznje.Find(v => v.ID == id);

            if (vozac == null || voznja == null)
            {
                response.Content = new StringContent("Greska na serveru!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            BazaPodataka.Instanca.Vozaci.Remove(vozac);
            vozac.Zauzet = false;
            vozac.Komentarisao = true;
            BazaPodataka.Instanca.Vozaci.Add(vozac);
            BazaPodataka.Instanca.UpisiUBazuVozace();
            var kom = new Komentar()
            {
                Opis = komentar,
                Ocena = ocena,
                Voznja = voznja.ID,
                DatumObjave = DateTime.Now
            };

            kom.Korisnik = vozac.KorisnickoIme;

            voznja.Komentar = new Komentar();
            voznja.Komentar = kom;
            BazaPodataka.Instanca.UpisiUBazuVoznje();
            response.Content = new StringContent("Uspesno ostavljen komentar na voznju!");
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        [HttpGet]
        [Route("Status")]
        public HttpResponseMessage Status()
        {
            var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");
            var response = new HttpResponseMessage();
            var informations = "";

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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Vozac)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var vozac = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);
            if (vozac == null)
            {
                response.Content = new StringContent("<h1>Greska na serveru!</h1>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            if (vozac.Blokiran)
            {
                response.Content = new StringContent("Korisnik blokiran!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (vozac.Komentarisao)
            {
                foreach (var item in BazaPodataka.Instanca.Voznje)
                {
                    if ((item.Vozac.KorisnickoIme != null && item.Vozac.KorisnickoIme.Equals(korisnickoIme)) && (item.StatusVoznje == Models.Enum.StatusVoznje.Obradjena || item.StatusVoznje == Models.Enum.StatusVoznje.Prihvacena || item.StatusVoznje == Models.Enum.StatusVoznje.Formirana))
                    {
                        informations += String.Format(@"<table><tr><td>Ulica:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Ulica);
                        informations += String.Format(@"<tr><td>Broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Broj);
                        informations += String.Format(@"<tr><td>Mesto:</td><td>{0}</td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                        informations += String.Format(@"<tr><td>Pozivni broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.PozivniBroj);
                        informations += @"<tr><td>Status:</td><td><select id=""statusV""><option value=""6"">Uspesna</option><option value=""5"">Neuspesna</option></select></td></tr>";
                        informations += String.Format(@"<tr><td>Ulica[odrediste]:</td><td><input type=""text"" class=""odrediste"" id=""ulicaO"" /></td></tr>");
                        informations += String.Format(@"<tr><td>Broj[odrediste]:</td><td><input type=""text"" class=""odrediste"" id=""brojO"" /></td></tr>");
                        informations += String.Format(@"<tr><td>Mesto[odrediste]:</td><td><input type=""text"" class=""odrediste"" id=""mestoO"" /></td></tr>");
                        informations += String.Format(@"<tr><td>Pozivni broj[odrediste]:</td><td><input type=""text"" class=""odrediste"" id=""pozivniBrojO"" /></td></tr>");
                        informations += String.Format(@"<tr><td>Iznos:</td><td><input type=""text"" class=""odrediste"" id=""iznos"" /></td></tr>");
                        informations += String.Format(@"<tr><td></td><td><button id=""promeniStatus"" value=""{0}"">Sacuvaj</button></td></tr>", item.ID);
                        informations += "</table>";
                        informations += @"<div id=""regVal""></div>";
                        response.Content = new StringContent(informations);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.OK;
                        return response;
                    }
                }
                response.Content = new StringContent("<h1>Niste ni na jednoj voznji!</h1>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            else
            {
                response.Content = new StringContent("<h1>Morate prvo komentarisati predhodnu voznju!</h1>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }     
        }
        [HttpPut]
        [Route("ZavrsiVoznju")]
        public HttpResponseMessage ZavrsiVoznju([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var Mesto = jToken.Value<string>("Mesto");
            var Ulica = jToken.Value<string>("Ulica");
            var Broj = jToken.Value<string>("BrojKuce");
            var PozivniBroj = jToken.Value<string>("PozivniBroj");
            var Status = jToken.Value<string>("Status");
            Int32.TryParse(jToken.Value<string>("ID"),out int id);
            Int32.TryParse(jToken.Value<string>("Iznos"),out int iznos);
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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Vozac)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var vozac = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);
            if (vozac == null)
            {
                response.Content = new StringContent("Zabranjen pristup!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }


            var voznja = BazaPodataka.Instanca.Voznje.Find(v => v.ID == id);

            if (voznja == null)
            {
                response.Content = new StringContent("Zabranjen pristup!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (Status.Equals("6"))
            {
                if (Mesto == null || Ulica == null || Broj == null || PozivniBroj == null || korisnickoIme == null)
                {
                    response.Content = new StringContent("Los zahtev!");
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }

                if (Mesto == "" || Ulica == "" || Broj == "" || PozivniBroj == "" || korisnickoIme == "")
                {
                    response.Content = new StringContent("Los zahtev!");
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
                BazaPodataka.Instanca.Vozaci.Remove(vozac);
                vozac.Zauzet = false;
                vozac.Komentarisao = true;
                BazaPodataka.Instanca.Vozaci.Add(vozac);
                BazaPodataka.Instanca.UpisiUBazuVozace();
                BazaPodataka.Instanca.Voznje.Remove(voznja);
                voznja.Odrediste = new Lokacija
                {
                    Adresa = new Adresa()
                };
                voznja.Odrediste.Adresa.Ulica = Ulica;
                voznja.Odrediste.Adresa.Broj = Broj;
                voznja.Odrediste.Adresa.NasenjenoMesto = Mesto;
                voznja.Odrediste.Adresa.PozivniBroj = PozivniBroj;
                voznja.Iznos = iznos;
                voznja.Komentar = new Komentar()
                {
                    ID = 0
                };
                voznja.StatusVoznje = Models.Enum.StatusVoznje.Uspesna;
                BazaPodataka.Instanca.Voznje.Add(voznja);
                BazaPodataka.Instanca.UpisiUBazuVoznje();             
                response.Content = new StringContent("Voznja zavrsena!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            else
            {
                BazaPodataka.Instanca.Vozaci.Remove(vozac);
                vozac.Komentarisao = false;
                BazaPodataka.Instanca.Vozaci.Add(vozac);

                BazaPodataka.Instanca.Voznje.Remove(voznja);
                voznja.Odrediste = new Lokacija
                {
                    Adresa = new Adresa()
                };
                voznja.Odrediste.Adresa.Ulica = Ulica;
                voznja.Odrediste.Adresa.Broj = Broj;
                voznja.Odrediste.Adresa.NasenjenoMesto = Mesto;
                voznja.Odrediste.Adresa.PozivniBroj = PozivniBroj;
                voznja.Iznos = iznos;
                voznja.Komentar = new Komentar()
                {
                    ID = 0
                };
                voznja.StatusVoznje = Models.Enum.StatusVoznje.Neuspesna;
                BazaPodataka.Instanca.Voznje.Add(voznja);
                BazaPodataka.Instanca.UpisiUBazuVoznje();
                BazaPodataka.Instanca.Vozaci.Remove(vozac);
                vozac.Zauzet = true;
                vozac.Komentarisao = false;
                BazaPodataka.Instanca.Vozaci.Add(vozac);
                BazaPodataka.Instanca.UpisiUBazuVozace();
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
                page += String.Format(@"<button id=""komentarisi"" value=""{0}"">Komentarisi</button></div>",voznja.ID);
                response.Content = new StringContent(page);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Redirect;
                return response;
            }
        }
        [HttpGet]
        [Route("Obradi")]
        public HttpResponseMessage Obradi()
        {
            var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");
            var response = new HttpResponseMessage();
            var informations = "";

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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Vozac)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var vozac = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);

            if (vozac.Blokiran)
            {
                response.Content = new StringContent("Korisnik blokiran!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (vozac.Komentarisao)
            {
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
                    informations += @"<div><button id=""sortVozac"">Sortiraj po udaljenosti</button></div>";
                    foreach (var item in listaVoznji)
                    {
                        if ((item.TipAutomobila != Models.Enum.TipAutomobila.BezNaznake && item.TipAutomobila == vozac.Automobil.TipAutomobila) || item.TipAutomobila == Models.Enum.TipAutomobila.BezNaznake)
                        {
                            informations += String.Format(@"<table><tr><td>Ulica:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Ulica);
                            informations += String.Format(@"<tr><td>Broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Broj);
                            informations += String.Format(@"<tr><td>Mesto:</td><td>{0}</td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                            informations += String.Format(@"<tr><td>Pozivni broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.PozivniBroj);
                            informations += String.Format(@"<tr><td></td><td><button class=""obradiV"" value=""{0}"">Obradi voznju</button></td></tr>", item.ID);
                            informations += "</table>";
                        }
                    }
                }
                response.Content = new StringContent(informations);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                return response;
            }
            else
            {
                informations += "<h1>Zavrsite sa komentarisanjem predhodne voznje!</h1>";
                response.Content = new StringContent(informations);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                return response;
            }
        }
        [HttpPut]
        [Route("Obradi")]
        public HttpResponseMessage Obradi([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var Id = jToken.Value<string>("ID");

            var response = new HttpResponseMessage();
            if (Id == null  || korisnickoIme == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            if (Id == "" || korisnickoIme == "")
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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Vozac)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            Int32.TryParse(Id, out int id);
            var vozacTemp = BazaPodataka.Instanca.Vozaci.Find(v => v.KorisnickoIme.Equals(korisnickoIme));
            if (vozacTemp == null)
            {
                response.Content = new StringContent("Greska na serveru!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (vozacTemp.Zauzet)
            {
                response.Content = new StringContent("Dodeljena vam je voznja u medjuvremenu!");
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
            voznjaTemp.StatusVoznje = Models.Enum.StatusVoznje.Prihvacena;
            voznjaTemp.Vozac = new Vozac();
            voznjaTemp.Vozac = vozacTemp;
            BazaPodataka.Instanca.Voznje.Add(voznjaTemp);
            BazaPodataka.Instanca.UpisiUBazuVozace();
            response.Content = new StringContent("Voznja je uspesno prihvacena!");
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        [Route("Sort")]
        public HttpResponseMessage Sort()
        {
            var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");
            var response = new HttpResponseMessage();
            var informations = "";

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
            else if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)).Uloga != Models.Enum.Uloga.Vozac)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var vozac = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);

            if (vozac.Blokiran)
            {
                response.Content = new StringContent("Korisnik blokiran!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (vozac.Komentarisao)
            {
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
                        item.Distanca = Math.Pow((item.Lokacija.X - vozac.Lokacija.X), 2) + Math.Pow((item.Lokacija.Y - vozac.Lokacija.Y), 2);
                    }
                    listaVoznji = listaVoznji.OrderBy(v => v.Distanca).ToList();
                    foreach (var item in listaVoznji)
                    {
                        if ((item.TipAutomobila != Models.Enum.TipAutomobila.BezNaznake && item.TipAutomobila == vozac.Automobil.TipAutomobila) || item.TipAutomobila == Models.Enum.TipAutomobila.BezNaznake)
                        {
                            informations += String.Format(@"<table><tr><td>Ulica:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Ulica);
                            informations += String.Format(@"<tr><td>Broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Broj);
                            informations += String.Format(@"<tr><td>Mesto:</td><td>{0}</td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                            informations += String.Format(@"<tr><td>Pozivni broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.PozivniBroj);
                            informations += String.Format(@"<tr><td></td><td><button class=""obradiV"" value=""{0}"">Obradi voznju</button></td></tr>", item.ID);
                            informations += "</table>";
                        }
                    }
                }
                response.Content = new StringContent(informations);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                return response;
            }
            else
            {
                informations += "<h1>Zavrsite sa komentarisanjem predhodne voznje!</h1>";
                response.Content = new StringContent(informations);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                return response;
            }
        }
    }    
}