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
        [HttpGet]
        [Route("Lokacija")]
        public HttpResponseMessage Lokacija()
        {
            var response = new HttpResponseMessage();
            var page = "";
            page += @"<input type=""text"" id=""ulica"" placeholder=""Ulica"" autocomplete=""off"" /><br />";
            page += @"<input type=""text"" id=""brojKuce"" placeholder=""Broj kuce"" autocomplete=""off"" /><br />";
            page += @"<input type=""text"" id=""mesto"" placeholder=""Naseljeno mesto"" autocomplete=""off"" /><br />";
            page += @"<input type=""text"" id=""pozivniBroj"" placeholder=""Postasnki broj"" autocomplete=""off"" /><br />";
            page += @"<button id=""promeniLokaciju"">Promeni lokaciju</button>";
            page += @"<div id=""regVal""></div>";
            response.Content = new StringContent(page);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        [HttpPut]
        [Route("PromenaLokacije")]
        public HttpResponseMessage PromenaLokacije([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var Mesto = jToken.Value<string>("Mesto");
            var Ulica = jToken.Value<string>("Ulica");
            var Broj = jToken.Value<string>("BrojKuce");
            var PozivniBroj = jToken.Value<string>("PozivniBroj");
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
            else
            {
                BazaPodataka.Instanca.Vozaci.Remove(vozac);
                vozac.Lokacija = new Lokacija
                {
                    Adresa = new Adresa()
                };
                vozac.Lokacija.Adresa.Ulica = Ulica;
                vozac.Lokacija.Adresa.Broj = Broj;
                vozac.Lokacija.Adresa.NasenjenoMesto = Mesto;
                vozac.Lokacija.Adresa.PozivniBroj = PozivniBroj;
                BazaPodataka.Instanca.Vozaci.Add(vozac);
                BazaPodataka.Instanca.UpisiUBazuVozace();

                response.Content = new StringContent("Uspesno sacuvana promena");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }                
        }


    }
}