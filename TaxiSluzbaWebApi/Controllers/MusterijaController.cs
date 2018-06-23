using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var ulica = jToken.Value<string>("Ulica");
            var broj = jToken.Value<string>("Broj");
            var mesto = jToken.Value<string>("Mesto");
            var pozivniBroj = jToken.Value<string>("PozivniBroj");
            Int32.TryParse(jToken.Value<string>("TipVozila"), out int tip);
            var tipVozila = Models.Enum.TipAutomobila.BezNaznake;

            if (tip == 1)
            {
                tipVozila = Models.Enum.TipAutomobila.Putnicki;
            }
            else
            {
                tipVozila = Models.Enum.TipAutomobila.Kombi;
            }
            if (ulica == null || broj == null || mesto == null || pozivniBroj == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (ulica == "" || broj == "" || mesto == "" || pozivniBroj == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Voznja novaVoznja = new Voznja();
            novaVoznja.Lokacija = new Lokacija();
            novaVoznja.Lokacija.Adresa.Ulica = ulica;
            novaVoznja.Lokacija.Adresa.Broj = broj;
            novaVoznja.Lokacija.Adresa.NasenjenoMesto = mesto;
            novaVoznja.Lokacija.Adresa.PozivniBroj = pozivniBroj;
            novaVoznja.TipAutomobila = tipVozila;
            novaVoznja.StatusVoznje = Models.Enum.StatusVoznje.Kreirana;


            if (HttpContext.Current.Application["noveVoznje"] == null)
            {
                HttpContext.Current.Application["noveVoznje"] = new List<Voznja>();
            }
            var listaVoznji = (List<Voznja>)HttpContext.Current.Application["noveVoznje"];
            novaVoznja.ID = listaVoznji.Count - 1;

            listaVoznji.Add(novaVoznja);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        

    }
}