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
    [RoutePrefix("api/Voznja")]
    public class VoznjaController : ApiController
    {
       [HttpGet]
       [Route("Filter")]
       public HttpResponseMessage Filter()
        {
            var jToken = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var filter = (Models.Enum.StatusVoznje)System.Enum.Parse(typeof(Models.Enum.StatusVoznje), jToken.Value<string>("Filter"));
            var informations = "";
            var response = new HttpResponseMessage();
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

            var filtrirane = new List<Voznja>();

            foreach (var item in BazaPodataka.Instanca.Voznje)
            {
                if (item.StatusVoznje == filter)
                {
                    filtrirane.Add(item);
                }
            }

            var m = BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme);
            if (m == null)
            {
                var d = BazaPodataka.Instanca.NadjiDispecera(korisnickoIme);
                if (d == null)
                {
                    var v = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);
                    if (v == null)
                    {
                        response.Content = new StringContent("");
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.Conflict;
                        return response;
                    }
                    else
                    {
                        informations += @"<div class=""filt""><select id=""tipFiltera"">";
                        informations += @"<option value=""Kreirana"">Kreirana</option>";
                        informations += @"<option value=""Formirana"">Formirana</option>";
                        informations += @"<option value=""Obradjena"">Obradjena</option>";
                        informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                        informations += @"<option value=""Otkazana"">Otkazana</option>";
                        informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                        informations += @"<option value=""Uspesna"">Uspesna</option>";
                        informations += @"</select><button id=""filtriraj"">Filter</button>";
                        informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                        foreach (var item in filtrirane)
                        {
                            informations += "<table>";
                            if ((item.Vozac != null && item.Vozac.KorisnickoIme != null))
                            {
                                if (item.Vozac.KorisnickoIme.Equals(korisnickoIme))
                                {
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

                            }
                        }
                        response.Content = new StringContent(informations);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.OK;
                        //HttpContext.Current.Session["ulogovan"] = new Vozac();
                        //HttpContext.Current.Session["ulogovan"] = v;
                        //HttpContext.Current.Application["ulogovan"] = new Vozac();                      
                        return response;
                    }

                }
                else
                {
                    informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                    informations += @"<div class=""filt""><select id=""tipFiltera"">";
                    informations += @"<option value=""Kreirana"">Kreirana</option>";
                    informations += @"<option value=""Formirana"">Formirana</option>";
                    informations += @"<option value=""Obradjena"">Obradjena</option>";
                    informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                    informations += @"<option value=""Otkazana"">Otkazana</option>";
                    informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                    informations += @"<option value=""Uspesna"">Uspesna</option>";
                    informations += @"</select><button id=""filtriraj"">Filter</button>";
                    informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                    foreach (var item in filtrirane)
                    {
                        informations += "<table>";
                        if ((item.Dispecer != null && item.Dispecer.KorisnickoIme != null))
                        {
                            if (item.Dispecer.KorisnickoIme.Equals(korisnickoIme))
                            {
                                if (item.Musterija != null)
                                {
                                    if (item.Musterija.KorisnickoIme != null)
                                    {
                                        informations += String.Format(@"<tr><td>Musterija:</td><td>{0}</td></tr>", item.Musterija.KorisnickoIme);
                                    }
                                }
                                informations += String.Format(@"<tr><td>Vozac:</td><td>{0}</td></tr>", item.Vozac.KorisnickoIme);
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
                        }
                    }
                    response.Content = new StringContent(informations);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.OK;
                    //HttpContext.Current.Session["ulogovan"] = new Vozac();
                    //HttpContext.Current.Session["ulogovan"] = v;
                    //HttpContext.Current.Application["ulogovan"] = new Vozac();
                    return response;
                }

            }
            else
            {
                informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                informations += @"<div class=""filt""><select id=""tipFiltera"">";
                informations += @"<option value=""Kreirana"">Kreirana</option>";
                informations += @"<option value=""Formirana"">Formirana</option>";
                informations += @"<option value=""Obradjena"">Obradjena</option>";
                informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                informations += @"<option value=""Otkazana"">Otkazana</option>";
                informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                informations += @"<option value=""Uspesna"">Uspesna</option>";
                informations += @"</select><button id=""filtriraj"">Filter</button>";
                informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                foreach (var item in filtrirane)
                {
                    informations += "<table>";
                    if ((item.Musterija != null && item.Musterija.KorisnickoIme != null))
                    {
                        if (item.Musterija.KorisnickoIme.Equals(korisnickoIme))
                        {

                            if (item.StatusVoznje == Models.Enum.StatusVoznje.Kreirana)
                            {
                                informations += String.Format(@"<tr><td>Ulica:</td><td><input type=""text"" value=""{0}"" id=""ulicaM""/></td></tr>", item.Lokacija.Adresa.Ulica);
                                informations += String.Format(@"<tr><td>Broj:</td><td><input type=""text"" value=""{0}"" id=""brojKuceM""/></td></tr>", item.Lokacija.Adresa.Broj);
                                informations += String.Format(@"<tr><td>Mesto:</td><td><input type=""text"" value=""{0}"" id=""mestoM""/></td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                                informations += String.Format(@"<tr><td>Pozivni broj:</td><td><input type=""text"" value=""{0}"" id=""pozivniBrojM""/></td></tr>", item.Lokacija.Adresa.PozivniBroj);
                                informations += String.Format(@"<tr><td>Status:</td><td>{0}</td></tr>", item.StatusVoznje.ToString());
                                informations += String.Format(@"<tr><td><button id=""izmeniVoznju"" value=""{0}"">Izmeni</button></td><td><button id=""otkaziVoznju"" value=""{1}"">Otkazi</button></td></tr>", item.ID, item.ID);
                                informations += "</table>";
                                informations += String.Format(@"<div id=""regVal""></div>");
                            }
                            else
                            {
                                if (item.Dispecer != null)
                                {
                                    if (item.Dispecer.KorisnickoIme != null)
                                    {
                                        informations += String.Format(@"<tr><td>Dispecer:</td><td>{0}</td></tr>", item.Dispecer.KorisnickoIme);
                                    }
                                }
                                informations += String.Format(@"<tr><td>Vozac:</td><td>{0}</td></tr>", item.Vozac.KorisnickoIme);
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
                                else
                                {
                                    informations += String.Format(@"<tr><td>Komentar:</td><td><textarea placeholder=""Komentar voznje...""></textarea></td></tr>");
                                    informations += String.Format(@"<tr><td>Ocena:</td><td>");
                                    informations += @"<select id=""ocena"">";
                                    informations += @"<option value=""1"">1</option>";
                                    informations += @"<option value=""2"">2</option>";
                                    informations += @"<option value=""3"">3</option>";
                                    informations += @"<option value=""4"">4</option>";
                                    informations += @"<option value=""5"">5</option></select></td></tr>";
                                    informations += String.Format(@"<tr><td></td><td><button id=""komentarisiVoznju"" value=""{0}"">Ostavi komentar</button></td></tr>", item.ID);
                                }
                                informations += "</table>";
                            }
                        }
                    }
                }
                response.Content = new StringContent(informations);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                //HttpContext.Current.Session["ulogovan"] = new Vozac();
                //HttpContext.Current.Session["ulogovan"] = v;
                //HttpContext.Current.Application["ulogovan"] = new Vozac();

                return response;


            }
        }


        [HttpGet]
        [Route("SortDatum")]
        public HttpResponseMessage SortDatum()
        {
            var jToken = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var informations = "";
            var response = new HttpResponseMessage();
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

            var filtrirane = BazaPodataka.Instanca.Voznje;

            filtrirane.Sort((x, y) => DateTime.Compare(x.DatumVremePoruzbine, y.DatumVremePoruzbine));



            var m = BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme);
            if (m == null)
            {
                var d = BazaPodataka.Instanca.NadjiDispecera(korisnickoIme);
                if (d == null)
                {
                    var v = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);
                    if (v == null)
                    {
                        response.Content = new StringContent("");
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.Conflict;
                        return response;
                    }
                    else
                    {
                        informations += @"<div class=""filt""><select id=""tipFiltera"">";
                        informations += @"<option value=""Kreirana"">Kreirana</option>";
                        informations += @"<option value=""Formirana"">Formirana</option>";
                        informations += @"<option value=""Obradjena"">Obradjena</option>";
                        informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                        informations += @"<option value=""Otkazana"">Otkazana</option>";
                        informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                        informations += @"<option value=""Uspesna"">Uspesna</option>";
                        informations += @"</select><button id=""filtriraj"">Filter</button>";
                        informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                        foreach (var item in filtrirane)
                        {
                            informations += "<table>";
                            if ((item.Vozac != null && item.Vozac.KorisnickoIme != null))
                            {
                                if (item.Vozac.KorisnickoIme.Equals(korisnickoIme))
                                {
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

                            }
                        }
                        response.Content = new StringContent(informations);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.OK;
                        //HttpContext.Current.Session["ulogovan"] = new Vozac();
                        //HttpContext.Current.Session["ulogovan"] = v;
                        //HttpContext.Current.Application["ulogovan"] = new Vozac();                      
                        return response;
                    }

                }
                else
                {
                    informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                    informations += @"<div class=""filt""><select id=""tipFiltera"">";
                    informations += @"<option value=""Kreirana"">Kreirana</option>";
                    informations += @"<option value=""Formirana"">Formirana</option>";
                    informations += @"<option value=""Obradjena"">Obradjena</option>";
                    informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                    informations += @"<option value=""Otkazana"">Otkazana</option>";
                    informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                    informations += @"<option value=""Uspesna"">Uspesna</option>";
                    informations += @"</select><button id=""filtriraj"">Filter</button>";
                    informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                    foreach (var item in filtrirane)
                    {
                        informations += "<table>";
                        if ((item.Dispecer != null && item.Dispecer.KorisnickoIme != null))
                        {
                            if (item.Dispecer.KorisnickoIme.Equals(korisnickoIme))
                            {
                                if (item.Musterija != null)
                                {
                                    if (item.Musterija.KorisnickoIme != null)
                                    {
                                        informations += String.Format(@"<tr><td>Musterija:</td><td>{0}</td></tr>", item.Musterija.KorisnickoIme);
                                    }
                                }
                                informations += String.Format(@"<tr><td>Vozac:</td><td>{0}</td></tr>", item.Vozac.KorisnickoIme);
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
                        }
                    }
                    response.Content = new StringContent(informations);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.OK;
                    //HttpContext.Current.Session["ulogovan"] = new Vozac();
                    //HttpContext.Current.Session["ulogovan"] = v;
                    //HttpContext.Current.Application["ulogovan"] = new Vozac();
                    return response;
                }

            }
            else
            {
                informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                informations += @"<div class=""filt""><select id=""tipFiltera"">";
                informations += @"<option value=""Kreirana"">Kreirana</option>";
                informations += @"<option value=""Formirana"">Formirana</option>";
                informations += @"<option value=""Obradjena"">Obradjena</option>";
                informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                informations += @"<option value=""Otkazana"">Otkazana</option>";
                informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                informations += @"<option value=""Uspesna"">Uspesna</option>";
                informations += @"</select><button id=""filtriraj"">Filter</button>";
                informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                foreach (var item in filtrirane)
                {
                    informations += "<table>";
                    if ((item.Musterija != null && item.Musterija.KorisnickoIme != null))
                    {
                        if (item.Musterija.KorisnickoIme.Equals(korisnickoIme))
                        {

                            if (item.StatusVoznje == Models.Enum.StatusVoznje.Kreirana)
                            {
                                informations += String.Format(@"<tr><td>Ulica:</td><td><input type=""text"" value=""{0}"" id=""ulicaM""/></td></tr>", item.Lokacija.Adresa.Ulica);
                                informations += String.Format(@"<tr><td>Broj:</td><td><input type=""text"" value=""{0}"" id=""brojKuceM""/></td></tr>", item.Lokacija.Adresa.Broj);
                                informations += String.Format(@"<tr><td>Mesto:</td><td><input type=""text"" value=""{0}"" id=""mestoM""/></td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                                informations += String.Format(@"<tr><td>Pozivni broj:</td><td><input type=""text"" value=""{0}"" id=""pozivniBrojM""/></td></tr>", item.Lokacija.Adresa.PozivniBroj);
                                informations += String.Format(@"<tr><td>Status:</td><td>{0}</td></tr>", item.StatusVoznje.ToString());
                                informations += String.Format(@"<tr><td><button id=""izmeniVoznju"" value=""{0}"">Izmeni</button></td><td><button id=""otkaziVoznju"" value=""{1}"">Otkazi</button></td></tr>", item.ID, item.ID);
                                informations += "</table>";
                                informations += String.Format(@"<div id=""regVal""></div>");
                            }
                            else
                            {
                                if (item.Dispecer != null)
                                {
                                    if (item.Dispecer.KorisnickoIme != null)
                                    {
                                        informations += String.Format(@"<tr><td>Dispecer:</td><td>{0}</td></tr>", item.Dispecer.KorisnickoIme);
                                    }
                                }
                                informations += String.Format(@"<tr><td>Vozac:</td><td>{0}</td></tr>", item.Vozac.KorisnickoIme);
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
                                else
                                {
                                    informations += String.Format(@"<tr><td>Komentar:</td><td><textarea placeholder=""Komentar voznje...""></textarea></td></tr>");
                                    informations += String.Format(@"<tr><td>Ocena:</td><td>");
                                    informations += @"<select id=""ocena"">";
                                    informations += @"<option value=""1"">1</option>";
                                    informations += @"<option value=""2"">2</option>";
                                    informations += @"<option value=""3"">3</option>";
                                    informations += @"<option value=""4"">4</option>";
                                    informations += @"<option value=""5"">5</option></select></td></tr>";
                                    informations += String.Format(@"<tr><td></td><td><button id=""komentarisiVoznju"" value=""{0}"">Ostavi komentar</button></td></tr>", item.ID);
                                }
                                informations += "</table>";
                            }
                        }
                    }
                }
                response.Content = new StringContent(informations);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                //HttpContext.Current.Session["ulogovan"] = new Vozac();
                //HttpContext.Current.Session["ulogovan"] = v;
                //HttpContext.Current.Application["ulogovan"] = new Vozac();

                return response;


            }
        }

        [HttpGet]
        [Route("Voznje")]
        public HttpResponseMessage Voznje()
        {
            var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());

            var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");
            var response = new HttpResponseMessage();

            var informations = "";

            if (korisnickoIme == null)
            {
                response.Content = new StringContent("");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];
            if (ulogovani == null)
            {
                response.Content = new StringContent("");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (ulogovani.Find(u => u.KorisnickoIme.Equals(korisnickoIme)) == null)
            {
                response.Content = new StringContent("");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            //var ulogovan = (User)HttpContext.Current.Session["ulogovan"];
            //if (ulogovan != null)
            //{
            //    response.Content = new StringContent("");
            //    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            //    response.StatusCode = HttpStatusCode.Forbidden;
            //    return response;
            //}



            var m = BazaPodataka.Instanca.NadjiMusteriju(korisnickoIme);
            if (m == null)
            {
                var d = BazaPodataka.Instanca.NadjiDispecera(korisnickoIme);
                if (d == null)
                {
                    var v = BazaPodataka.Instanca.NadjiVozaca(korisnickoIme);
                    if (v == null)
                    {
                        response.Content = new StringContent("");
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.Conflict;
                        return response;
                    }
                    else
                    {
                        informations += @"<div class=""filt""><select id=""tipFiltera"">";
                        informations += @"<option value=""Kreirana"">Kreirana</option>";
                        informations += @"<option value=""Formirana"">Formirana</option>";
                        informations += @"<option value=""Obradjena"">Obradjena</option>";
                        informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                        informations += @"<option value=""Otkazana"">Otkazana</option>";
                        informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                        informations += @"<option value=""Uspesna"">Uspesna</option>";
                        informations += @"</select><button id=""filtriraj"">Filter</button>";
                        informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                        foreach (var item in BazaPodataka.Instanca.Voznje)
                        {
                            informations += "<table>";
                            if ((item.Vozac != null && item.Vozac.KorisnickoIme != null))
                            {
                                if (item.Vozac.KorisnickoIme.Equals(korisnickoIme))
                                {
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

                            }
                        }
                        response.Content = new StringContent(informations);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.OK;
                        //HttpContext.Current.Session["ulogovan"] = new Vozac();
                        //HttpContext.Current.Session["ulogovan"] = v;
                        //HttpContext.Current.Application["ulogovan"] = new Vozac();                      
                        return response;
                    }

                }
                else
                {
                    informations += @"<div class=""filt""><select id=""tipFiltera"">";
                    informations += @"<option value=""Kreirana"">Kreirana</option>";
                    informations += @"<option value=""Formirana"">Formirana</option>";
                    informations += @"<option value=""Obradjena"">Obradjena</option>";
                    informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                    informations += @"<option value=""Otkazana"">Otkazana</option>";
                    informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                    informations += @"<option value=""Uspesna"">Uspesna</option>";
                    informations += @"</select><button id=""filtriraj"">Filter</button>";
                    informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                    foreach (var item in BazaPodataka.Instanca.Voznje)
                    {
                        informations += "<table>";
                        if ((item.Dispecer != null && item.Dispecer.KorisnickoIme != null))
                        {
                            if (item.Dispecer.KorisnickoIme.Equals(korisnickoIme))
                            {
                                if (item.Musterija != null)
                                {
                                    if (item.Musterija.KorisnickoIme != null)
                                    {
                                        informations += String.Format(@"<tr><td>Musterija:</td><td>{0}</td></tr>", item.Musterija.KorisnickoIme);
                                    }
                                }
                                informations += String.Format(@"<tr><td>Vozac:</td><td>{0}</td></tr>", item.Vozac.KorisnickoIme);
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
                        }
                    }
                    response.Content = new StringContent(informations);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.OK;
                    //HttpContext.Current.Session["ulogovan"] = new Vozac();
                    //HttpContext.Current.Session["ulogovan"] = v;
                    //HttpContext.Current.Application["ulogovan"] = new Vozac();
                    return response;
                }

            }
            else
            {
                informations += @"<div class=""filt""><select id=""tipFiltera"">";
                informations += @"<option value=""Kreirana"">Kreirana</option>";
                informations += @"<option value=""Formirana"">Formirana</option>";
                informations += @"<option value=""Obradjena"">Obradjena</option>";
                informations += @"<option value=""Prihvacena"">Prihvacena</option>";
                informations += @"<option value=""Otkazana"">Otkazana</option>";
                informations += @"<option value=""Neuspesna"">Neuspesna</option>";
                informations += @"<option value=""Uspesna"">Uspesna</option>";
                informations += @"</select><button id=""filtriraj"">Filter</button>";
                informations += @"<button id=""sortirajDatum"">Sortiraj po datumu</button></div>";
                foreach (var item in BazaPodataka.Instanca.Voznje)
                {
                    informations += "<table>";
                    if ((item.Musterija != null && item.Musterija.KorisnickoIme != null))
                    {
                        if (item.Musterija.KorisnickoIme.Equals(korisnickoIme))
                        {

                            if (item.StatusVoznje == Models.Enum.StatusVoznje.Kreirana)
                            {
                                informations += String.Format(@"<tr><td>Ulica:</td><td><input type=""text"" value=""{0}"" id=""ulicaM""/></td></tr>", item.Lokacija.Adresa.Ulica);
                                informations += String.Format(@"<tr><td>Broj:</td><td><input type=""text"" value=""{0}"" id=""brojKuceM""/></td></tr>", item.Lokacija.Adresa.Broj);
                                informations += String.Format(@"<tr><td>Mesto:</td><td><input type=""text"" value=""{0}"" id=""mestoM""/></td></tr>", item.Lokacija.Adresa.NasenjenoMesto);
                                informations += String.Format(@"<tr><td>Pozivni broj:</td><td><input type=""text"" value=""{0}"" id=""pozivniBrojM""/></td></tr>", item.Lokacija.Adresa.PozivniBroj);
                                informations += String.Format(@"<tr><td>Status:</td><td>{0}</td></tr>", item.StatusVoznje.ToString());
                                informations += String.Format(@"<tr><td><button id=""izmeniVoznju"" value=""{0}"">Izmeni</button></td><td><button id=""otkaziVoznju"" value=""{1}"">Otkazi</button></td></tr>", item.ID, item.ID);
                                informations += "</table>";
                                informations += String.Format(@"<div id=""regVal""></div>");
                            }
                            else
                            {
                                if (item.Dispecer != null)
                                {
                                    if (item.Dispecer.KorisnickoIme != null)
                                    {
                                        informations += String.Format(@"<tr><td>Dispecer:</td><td>{0}</td></tr>", item.Dispecer.KorisnickoIme);
                                    }
                                }
                                informations += String.Format(@"<tr><td>Vozac:</td><td>{0}</td></tr>", item.Vozac.KorisnickoIme);
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
                                else
                                {
                                    informations += String.Format(@"<tr><td>Komentar:</td><td><textarea placeholder=""Komentar voznje...""></textarea></td></tr>");
                                    informations += String.Format(@"<tr><td>Ocena:</td><td>");
                                    informations += @"<select id=""ocena"">";
                                    informations += @"<option value=""1"">1</option>";
                                    informations += @"<option value=""2"">2</option>";
                                    informations += @"<option value=""3"">3</option>";
                                    informations += @"<option value=""4"">4</option>";
                                    informations += @"<option value=""5"">5</option></select></td></tr>";
                                    informations += String.Format(@"<tr><td></td><td><button id=""komentarisiVoznju"" value=""{0}"">Ostavi komentar</button></td></tr>", item.ID);
                                }
                                informations += "</table>";
                            }
                        }
                    }
                }
                response.Content = new StringContent(informations);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                //HttpContext.Current.Session["ulogovan"] = new Vozac();
                //HttpContext.Current.Session["ulogovan"] = v;
                //HttpContext.Current.Application["ulogovan"] = new Vozac();

                return response;
            }


        }
    }
}