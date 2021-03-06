﻿using Newtonsoft.Json.Linq;
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

            Automobil auto = new Automobil
            {
                Vozac = new Vozac()
            };
            auto.Vozac = noviVozac;
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
            noviVozac.Automobil = new Automobil();
            noviVozac.Automobil = auto;

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
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (auto.BrojTaksiVozila.Equals(item.Automobil.BrojTaksiVozila))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,"ID taksi vozila je zauzet");
                }
            }

            BazaPodataka.Instanca.Vozaci.Add(noviVozac);
            BazaPodataka.Instanca.UpisiUBazuVozace();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpGet]
        [Route("Voznja")]
        public HttpResponseMessage Voznja()
        {
            var jToken = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var xKordinata = jToken.Value<double>("XKordinata");
            var yKordinata = jToken.Value<double>("YKordinata");
            Int32.TryParse(jToken.Value<string>("TipAutomobila"), out int tip);
            var TipVozila = Models.Enum.TipAutomobila.BezNaznake;

            if (tip == 1)
            {
                TipVozila = Models.Enum.TipAutomobila.Putnicki;
            }
            else if (tip == 2)
            {
                TipVozila = Models.Enum.TipAutomobila.Kombi;
            }
            else
            {
                TipVozila = Models.Enum.TipAutomobila.BezNaznake;
            }

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
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (ulogovani.Find(u => u.KorisnickoIme.Equals(item.KorisnickoIme)) != null)
                {
                    if (!item.Zauzet && (item.Automobil.TipAutomobila == TipVozila || TipVozila == Models.Enum.TipAutomobila.BezNaznake))
                    {
                        listaSlobodnihVozaca.Add(item);
                    }
                }
            }

            foreach (var v in listaSlobodnihVozaca)
            {
                v.Distanca = Math.Pow((v.Lokacija.X - xKordinata), 2) + Math.Pow((v.Lokacija.Y - yKordinata), 2);
            }
            listaSlobodnihVozaca = listaSlobodnihVozaca.OrderBy(v => v.Distanca).ToList();
            var result = new List<Vozac>();
            for (int i = 0; i < 5; i++)
            {
                if (i + 1 > listaSlobodnihVozaca.Count)
                {
                    break;
                }
                result.Add(listaSlobodnihVozaca.ElementAt(i));
            }
            return Request.CreateResponse(HttpStatusCode.OK,result);
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
            var xKordinata = jToken.Value<double>("XKordinata");
            var yKordinata = jToken.Value<double>("YKordinata");
            var vozac = jToken.Value<string>("Vozac");
            var response = new HttpResponseMessage();
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

            if (Mesto == null || Ulica == null || Broj == null || PozivniBroj == null || vozac == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (Mesto == "" || Ulica == "" || Broj == "" || PozivniBroj == "" || vozac == "")
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
            var vozacTemp = BazaPodataka.Instanca.Vozaci.Find(v => v.KorisnickoIme.Equals(vozac));
            if (vozacTemp == null)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            else if (vozacTemp.Automobil.TipAutomobila != TipVozila && TipVozila != Models.Enum.TipAutomobila.BezNaznake)
            {
                response.Content = new StringContent("Los zahtev!Vozac ne vozi zadati tip automobila!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            else if (vozacTemp.Zauzet)
            {
                response.Content = new StringContent("Vozac je u medjuvremenu prihvatio drugu voznju!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }
            else if (ulogovani.Find(v => v.KorisnickoIme.Equals(vozac)) == null)
            {
                response.Content = new StringContent("Vozac se u medjuvremenu odjavio!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }


            Voznja novaVoznja = new Voznja
            {
                ID = BazaPodataka.Instanca.Voznje.Count(),
                DatumVremePoruzbine = DateTime.Now,
                Lokacija = new Lokacija
                {
                    X = xKordinata,
                    Y = yKordinata,
                    Adresa = new Adresa
                    {
                        Broj = Broj,
                        NasenjenoMesto = Mesto,
                        PozivniBroj = PozivniBroj,
                        Ulica = Ulica
                    }
                }
            };
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
            BazaPodataka.Instanca.UpisiUBazuVoznje();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpGet]
        [Route("Obradi")]
        public HttpResponseMessage Obradi()
        {
            var response = new HttpResponseMessage();
            var informations = "";

            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (ulogovani == null)
            {
                response.Content = new StringContent("Greska na serveru!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var listaSlobodnihVozaca = new List<Vozac>();
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (ulogovani.Find(u => u.KorisnickoIme.Equals(item.KorisnickoIme)) != null)
                {
                    if (!item.Zauzet)
                    {
                        listaSlobodnihVozaca.Add(item);
                    }
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
                        if (v.Automobil.TipAutomobila == item.TipAutomobila || item.TipAutomobila == Models.Enum.TipAutomobila.BezNaznake)
                        {
                            informations += String.Format(@"<option val=""{0}"">{1}</option>", v.KorisnickoIme, v.KorisnickoIme);
                        }                    
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
      
            var voznjaTemp = BazaPodataka.Instanca.Voznje.Find(v => v.ID == id);

            if (voznjaTemp == null)
            {
                response.Content = new StringContent("Greska na serveru!Ne postoji voznja!");
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

            var vozacTemp = BazaPodataka.Instanca.Vozaci.Find(v => v.KorisnickoIme.Equals(vozac));
            if (vozacTemp == null)
            {
                response.Content = new StringContent("Los zahtev!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            else if (vozacTemp.Automobil.TipAutomobila != voznjaTemp.TipAutomobila && voznjaTemp.TipAutomobila != Models.Enum.TipAutomobila.BezNaznake)
            {
                response.Content = new StringContent("Los zahtev!Vozac ne vozi zadati tip automobila!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }
            else if (vozacTemp.Zauzet)
            {
                response.Content = new StringContent("Vozac je u medjuvremenu prihvatio drugu voznju!");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }
            else if (ulogovani.Find(v => v.KorisnickoIme.Equals(vozac)) == null)
            {
                response.Content = new StringContent("Vozac se u medjuvremenu odjavio!");
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
        [HttpGet]
        [Route("Voznje/{korisnickoIme}")]
        public HttpResponseMessage Voznje(string korisnickoIme)
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
            informations += @"<div class=""filt""><select id=""tipFiltera"">";
            informations += @"<option value="""""">-</option>";
            informations += @"<option value=""Kreirana"">Kreirana</option>";
            informations += @"<option value=""Formirana"">Formirana</option>";
            informations += @"<option value=""Obradjena"">Obradjena</option>";
            informations += @"<option value=""Prihvacena"">Prihvacena</option>";
            informations += @"<option value=""Otkazana"">Otkazana</option>";
            informations += @"<option value=""Neuspesna"">Neuspesna</option>";
            informations += @"<option value=""Uspesna"">Uspesna</option>";
            informations += @"</select><button id=""filtriraj"" value=""1"">Filter</button>";
            informations += @"<input type=""checkbox"" id=""sortirajDatum""/>Sortiraj po datumu</br>";
            informations += @"<input type=""checkbox"" id=""sortirajOcenu""/>Sortiraj po oceni</br>";
            informations += @"</br><label>Pretraga po oceni(od-do):</label></br>";
            informations += @"<label for=""od"">0 &nbsp;&nbsp;&nbsp;&nbsp;1 &nbsp;&nbsp;&nbsp;&nbsp;2 &nbsp;&nbsp;&nbsp;&nbsp;3 &nbsp;&nbsp;&nbsp;&nbsp;4 &nbsp;&nbsp;&nbsp;&nbsp;5</label></br><input type=""range"" multiple id=""od"" min=""0"" max=""5"" value=""0"">";
            informations += @"</br><label for=""do"">0 &nbsp;&nbsp;&nbsp;&nbsp;1 &nbsp;&nbsp;&nbsp;&nbsp;2 &nbsp;&nbsp;&nbsp;&nbsp;3 &nbsp;&nbsp;&nbsp;&nbsp;4 &nbsp;&nbsp;&nbsp;&nbsp;5</label></br><input type=""range"" multiple id=""do"" min=""0"" max=""5"" value=""5"">";
            informations += @"</br><label style=""font-size: 15px !important;"" for=""odCena"">0 &nbsp;&nbsp;&nbsp;100 &nbsp;&nbsp;&nbsp;200 &nbsp;&nbsp;&nbsp;300 &nbsp;&nbsp;&nbsp;400 &nbsp;&nbsp;&nbsp;500 ;&nbsp;&nbsp;&nbsp;600 &nbsp;&nbsp;&nbsp;&nbsp;700 &nbsp;&nbsp;&nbsp;&nbsp;800 &nbsp;&nbsp;&nbsp;&nbsp;900 &nbsp;&nbsp;&nbsp;&nbsp;1000+</label></br><input type=""range"" style=""width: 400px"" multiple id=""odCena"" min=""0"" max=""1000"" step=""100""value=""0"">";
            informations += @"</br><label style=""font-size: 15px !important;"" for=""doCena"">0 &nbsp;&nbsp;&nbsp;100 &nbsp;&nbsp;&nbsp;200 &nbsp;&nbsp;&nbsp;300 &nbsp;&nbsp;&nbsp;400 &nbsp;&nbsp;&nbsp;500 ;&nbsp;&nbsp;&nbsp;600 &nbsp;&nbsp;&nbsp;&nbsp;700 &nbsp;&nbsp;&nbsp;&nbsp;800 &nbsp;&nbsp;&nbsp;&nbsp;900 &nbsp;&nbsp;&nbsp;&nbsp;1000+</label></br><input type=""range"" style=""width: 400px"" multiple id=""doCena"" min=""0"" max=""1000"" step=""100"" value=""1000"">";
            informations += @"</br><label for=""datuOd"">Datum od:</label></br><input type=""date"" id=""odDatum"">";
            informations += @"</br><label for=""datuDo"">Datum do:</label></br><input type=""date"" id=""doDatum"">";
            informations += @"<div id=""filterError""></div></div>";
            informations += @"<div></br><input type=""text"" id=""musterijaIme"" placeholder=""Pretrazi po imenu musterije""/>";
            informations += @"</br><input type=""text"" id=""musterijaPrezime"" placeholder=""Pretrazi po prezimenu musterije""/>";
            informations += @"</br><input type=""text"" id=""vozacIme"" placeholder=""Pretrazi po imenu vozaca""/>";
            informations += @"</br><input type=""text"" id=""vozacPrezime"" placeholder=""Pretrazi po prezimenu vozaca""/></div>";
            foreach (var item in BazaPodataka.Instanca.Voznje)
            {
                informations += "<table>";

                if (item.Musterija != null || item.Musterija.KorisnickoIme != null)
                {
                    informations += String.Format(@"<tr><td>Musterija:</td><td>{0}</td></tr>", item.Musterija.KorisnickoIme);
                }
                if (item.Dispecer != null)
                {
                    if (item.Dispecer.KorisnickoIme != null)
                    {
                        informations += String.Format(@"<tr><td>Dispecer:</td><td>{0}</td></tr>", item.Dispecer.KorisnickoIme);
                    }
                }
                if (item.Vozac != null)
                {
                    if (item.Vozac.KorisnickoIme != null)
                    {
                        informations += String.Format(@"<tr><td>Vozac:</td><td>{0}</td></tr>", item.Vozac.KorisnickoIme);
                    }
                }
                informations += String.Format(@"<tr><td>Datum:</td><td>{0}</td></tr>", item.DatumVremePoruzbine.ToString());
                informations += String.Format(@"<tr><td>Ulica:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Ulica);
                informations += String.Format(@"<tr><td>Broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.Broj);
                informations += String.Format(@"<tr><td>Mesto:</td><td>{0}</td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                informations += String.Format(@"<tr><td>Pozivni broj:</td><td>{0}</td></tr>", item.Lokacija.Adresa.PozivniBroj);
                informations += String.Format(@"<tr><td>Status:</td><td>{0}</td></tr>", item.StatusVoznje.ToString());
                if (item.StatusVoznje == Models.Enum.StatusVoznje.Uspesna)
                {
                    informations += String.Format(@"<tr><td>Ulica[ODREDISTE]:</td><td>{0}</td></tr>", item.Odrediste.Adresa.Ulica);
                    informations += String.Format(@"<tr><td>Broj[ODREDISTE]:</td><td>{0}</td></tr>", item.Odrediste.Adresa.Broj);
                    informations += String.Format(@"<tr><td>Mesto[ODREDISTE]:</td><td>{0}</td></tr>", item.Odrediste.Adresa.NasenjenoMesto);
                    informations += String.Format(@"<tr><td>Pozivni broj[ODREDISTE]:</td><td>{0}</td></tr>", item.Odrediste.Adresa.PozivniBroj);
                    informations += String.Format(@"<tr><td>Iznos:</td><td>{0}</td></tr>", item.Iznos);
                }
                if (item.Komentar.Korisnik != null)
                {
                    informations += String.Format(@"<tr><td>Komentar:</td><td>{0}</td></tr>", item.Komentar.Opis);
                    informations += String.Format(@"<tr><td>Korisnik:</td><td>{0}</td></tr>", item.Komentar.Korisnik);
                    informations += String.Format(@"<tr><td>Ocena:</td><td>{0}</td></tr>", item.Komentar.Ocena.ToString());
                    informations += String.Format(@"<tr><td>Datum komentara:</td><td>{0}</td></tr>", item.Komentar.DatumObjave.ToString());
                }
                informations += "</table>";
            }
            if (informations.Equals(""))
            {
                informations += "Ne postoji ni jedna voznja u bazi!";
            }
            response.Content = new StringContent(informations);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            //HttpContext.Current.Session["ulogovan"] = new Vozac();
            //HttpContext.Current.Session["ulogovan"] = v;
            //HttpContext.Current.Application["ulogovan"] = new Vozac();                      
            return response;
        }
        [HttpGet]
        [Route("Blok")]
        public HttpResponseMessage Blok()
        {
            var lista = new List<User>();
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                lista.Add((User)item);
            }
            foreach (var item in BazaPodataka.Instanca.Musterije)
            {
                lista.Add((User)item);
            }
            return Request.CreateResponse(HttpStatusCode.OK, lista);
        }
        [HttpPut]
        [Route("Blok/{korisnickoIme}")]
        public HttpResponseMessage Blok(string korisnickoIme)
        {
            var korisnikM = BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme);
            var korisnikV = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);
            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (korisnikM == null && korisnikV == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Los zahvtev");
            }
            else if(korisnikV != null)
            {
                BazaPodataka.Instanca.Vozaci.Remove(korisnikV);
                if (korisnikV.Blokiran)
                {
                    korisnikV.Blokiran = false;
                    BazaPodataka.Instanca.Vozaci.Add(korisnikV);
                    BazaPodataka.Instanca.UpisiUBazuVozace();
                    return Request.CreateResponse(HttpStatusCode.OK, "Uspesno odblokiran korisnik!");
                }
                else
                {
                    ulogovani.Remove(ulogovani.Find(u => u.KorisnickoIme.Equals(korisnikV)));
                    HttpContext.Current.Application["ulogovani"] = ulogovani;
                    korisnikV.Blokiran = true;
                    BazaPodataka.Instanca.Vozaci.Add(korisnikV);
                    BazaPodataka.Instanca.UpisiUBazuVozace();
                    return Request.CreateResponse(HttpStatusCode.OK, "Uspesno blokiran korisnik!");
                }              
               
            }
            else
            {
                BazaPodataka.Instanca.Musterije.Remove(korisnikM);
                if (korisnikM.Blokiran)
                {
                    korisnikM.Blokiran = false;
                    BazaPodataka.Instanca.Musterije.Add(korisnikM);
                    BazaPodataka.Instanca.UpisiUBazuMusterije();
                    return Request.CreateResponse(HttpStatusCode.OK, "Uspesno odblokiran korisnik!");
                }
                else
                {
                    ulogovani.Remove(ulogovani.Find(u => u.KorisnickoIme.Equals(korisnikM)));
                    HttpContext.Current.Application["ulogovani"] = ulogovani;
                    korisnikM.Blokiran = true;
                    BazaPodataka.Instanca.Musterije.Add(korisnikM);
                    BazaPodataka.Instanca.UpisiUBazuMusterije();
                    return Request.CreateResponse(HttpStatusCode.OK, "Uspesno blokiran korisnik!");
                }
               
            }
        }
    }
}