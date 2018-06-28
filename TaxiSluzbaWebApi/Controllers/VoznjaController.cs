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
            Models.Enum.StatusVoznje filter = Models.Enum.StatusVoznje.Formirana;
            var tempFilter = jToken.Value<string>("Filter");
            Int32.TryParse(jToken.Value<string>("Flag"), out int flag);
            Int32.TryParse(jToken.Value<string>("OdOcena"), out int odOcena);
            Int32.TryParse(jToken.Value<string>("DoOcena"), out int doOcena);
            Int32.TryParse(jToken.Value<string>("OdCena"), out int odCena);
            Int32.TryParse(jToken.Value<string>("DoCena"), out int doCena);
            var odDatumT = jToken.Value<string>("OdDatum");
            var doDatumT = jToken.Value<string>("DoDatum");
            var musterijaIme = jToken.Value<string>("MusterijaIme");
            var musterijaPrezime = jToken.Value<string>("MusterijaPrezime");
            var vozacIme = jToken.Value<string>("VozacIme");
            var vozacPrezime = jToken.Value<string>("VozacPrezime");


            var datum = jToken.Value<bool>("Datum");
            var ocena = jToken.Value<bool>("Ocena");
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

            #region FILTER
            var filtrirane = new List<Voznja>();
            if (!tempFilter.Equals(""))
            {
                filter = (Models.Enum.StatusVoznje)System.Enum.Parse(typeof(Models.Enum.StatusVoznje), jToken.Value<string>("Filter"));
                foreach (var item in BazaPodataka.Instanca.Voznje)
                {
                    if (item.StatusVoznje == filter)
                    {
                        filtrirane.Add(item);
                    }
                }
            }
            else
            {
                filtrirane = BazaPodataka.Instanca.Voznje;
            }
            #endregion
            #region OD-DO OCENA
            var ocenaTemp = new List<Voznja>();
            foreach (var item in filtrirane)
            {
                if (item.Komentar.Ocena >= odOcena && item.Komentar.Ocena <= doOcena)
                {
                    ocenaTemp.Add(item);
                }
            }
            filtrirane = ocenaTemp;
#endregion
            #region OD-DO CENA
            var cenaTemp = new List<Voznja>();
            foreach (var item in filtrirane)
            {
                if (doCena == 1000)
                {
                    if (item.Iznos >= odCena)
                    {
                        cenaTemp.Add(item);
                    }
                }
                else
                {
                    if (item.Iznos >= odCena && item.Iznos <= doCena)
                    {
                        cenaTemp.Add(item);
                    }
                }
                
            }
            filtrirane = cenaTemp;
            #endregion
            #region OD-DO DATUM
            var datumTemp = new List<Voznja>();
            foreach (var item in filtrirane)
            {
                if (odDatumT.Equals("") && doDatumT.Equals(""))
                {
                    datumTemp.Add(item);
                }
                else if(odDatumT.Equals(""))
                {
                    var doDatum = DateTime.Parse(doDatumT);
                    doDatum += new TimeSpan(23, 59, 59);
                    if (item.DatumVremePoruzbine <= doDatum)
                    {
                        datumTemp.Add(item);
                    }
                }
                else if (doDatumT.Equals(""))
                {
                    var odDatum = DateTime.Parse(odDatumT);
                    if (item.DatumVremePoruzbine >= odDatum)
                    {
                        datumTemp.Add(item);
                    }
                }
                else
                {
                    var odDatum = DateTime.Parse(odDatumT);
                    var doDatum = DateTime.Parse(doDatumT);
                    doDatum += new TimeSpan(23, 59, 59);
                    if (item.DatumVremePoruzbine >= odDatum && item.DatumVremePoruzbine <= doDatum)
                    {
                        datumTemp.Add(item);
                    }
                }
                
            }
            filtrirane = datumTemp;
            #endregion
            #region DATUM-OCENA SORT
            if (datum && ocena)
            {
                
                filtrirane = filtrirane.OrderBy(p => p.DatumVremePoruzbine).ThenBy(p => p.Komentar.Ocena).ToList();
                filtrirane.Reverse();
            }
            else if (datum)
            {
                filtrirane.Sort((x, y) => DateTime.Compare(x.DatumVremePoruzbine, y.DatumVremePoruzbine));
                filtrirane.Reverse();
            }
            else if (ocena)
            {
                filtrirane.Sort((x, y) => x.Komentar.Ocena.CompareTo(y.Komentar.Ocena));
                filtrirane.Reverse();
            }
            #endregion
            #region MusterijaPretraga
            var musterijaTemp = new List<Voznja>();
            foreach (var item in filtrirane)
            {
                if (item.Musterija != null)
                {
                    if (musterijaIme.Equals("") && musterijaPrezime.Equals(""))
                    {
                        musterijaTemp.Add(item);
                    }
                    else if (musterijaIme.Equals(""))
                    {
                        if (item.Musterija.Prezime != null && item.Musterija.Prezime.Equals(musterijaPrezime))
                        {
                            musterijaTemp.Add(item);
                        }
                    }
                    else if (musterijaPrezime.Equals(""))
                    {
                        if (item.Musterija.Ime != null && item.Musterija.Ime.Equals(musterijaIme))
                        {
                            musterijaTemp.Add(item);
                        }
                    }
                    else
                    {
                        if (item.Musterija.Ime != null && item.Musterija.Ime.Equals(musterijaIme) && item.Musterija.Prezime != null && item.Musterija.Prezime.Equals(musterijaPrezime))
                        {
                            musterijaTemp.Add(item);
                        }
                    }                  
                }
            }
            filtrirane = musterijaTemp;
            #endregion
            #region VozacPretraga
            var vozacTemp = new List<Voznja>();
            foreach (var item in filtrirane)
            {
                if (item.Vozac != null)
                {
                    if (vozacIme.Equals("") && vozacPrezime.Equals(""))
                    {
                        vozacTemp.Add(item);
                    }
                    else if (vozacIme.Equals(""))
                    {
                        if (item.Vozac.Prezime != null && item.Vozac.Prezime.Equals(vozacPrezime))
                        {
                            vozacTemp.Add(item);
                        }
                    }
                    else if (vozacPrezime.Equals(""))
                    {
                        if (item.Vozac.Ime != null && item.Vozac.Ime.Equals(vozacIme))
                        {
                            vozacTemp.Add(item);
                        }
                    }
                    else
                    {
                        if (item.Vozac.Ime != null && item.Vozac.Ime.Equals(vozacIme) && item.Vozac.Prezime != null && item.Vozac.Prezime.Equals(vozacPrezime))
                        {
                            vozacTemp.Add(item);
                        }
                    }
                }
            }
            filtrirane = vozacTemp;
            #endregion
            #region FilterUI
            informations += @"<div class=""filt""><select id=""tipFiltera"">";
            informations += @"<option value="""">-</option>";
            informations += @"<option value=""Kreirana"">Kreirana</option>";
            informations += @"<option value=""Formirana"">Formirana</option>";
            informations += @"<option value=""Obradjena"">Obradjena</option>";
            informations += @"<option value=""Prihvacena"">Prihvacena</option>";
            informations += @"<option value=""Otkazana"">Otkazana</option>";
            informations += @"<option value=""Neuspesna"">Neuspesna</option>";
            informations += @"<option value=""Uspesna"">Uspesna</option>";
            informations += @"</select><button id=""filtriraj"" value=""0"">Filter</button></br>";
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
            #endregion
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
                    informations += @"<div></br><input type=""text"" id=""musterijaIme"" placeholder=""Pretrazi po imenu musterije""/>";
                    informations += @"</br><input type=""text"" id=""musterijaPrezime"" placeholder=""Pretrazi po prezimenu musterije""/>";
                    informations += @"</br><input type=""text"" id=""vozacIme"" placeholder=""Pretrazi po imenu vozaca""/>";
                    informations += @"</br><input type=""text"" id=""vozacPrezime"" placeholder=""Pretrazi po prezimenu vozaca""/></div>";

                    foreach (var item in filtrirane)
                    {
                        informations += "<table>";
                        if (flag == 0)
                        {
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
                        else
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
            #region FilterUI
            informations += @"<div class=""filt""><select id=""tipFiltera"">";
            informations += @"<option value="""""">-</option>";
            informations += @"<option value=""Kreirana"">Kreirana</option>";
            informations += @"<option value=""Formirana"">Formirana</option>";
            informations += @"<option value=""Obradjena"">Obradjena</option>";
            informations += @"<option value=""Prihvacena"">Prihvacena</option>";
            informations += @"<option value=""Otkazana"">Otkazana</option>";
            informations += @"<option value=""Neuspesna"">Neuspesna</option>";
            informations += @"<option value=""Uspesna"">Uspesna</option>";
            informations += @"</select><button id=""filtriraj"" value=""0"">Filter</button>";
            informations += @"<input type=""checkbox"" id=""sortirajDatum"">Sortiraj po datumu</br>";
            informations += @"<input type=""checkbox"" id=""sortirajOcenu"">Sortiraj po oceni</br>";
            informations += @"</br><label>Pretraga po oceni(od-do):</label></br>";
            informations += @"<label for=""od"">0 &nbsp;&nbsp;&nbsp;&nbsp;1 &nbsp;&nbsp;&nbsp;&nbsp;2 &nbsp;&nbsp;&nbsp;&nbsp;3 &nbsp;&nbsp;&nbsp;&nbsp;4 &nbsp;&nbsp;&nbsp;&nbsp;5</label></br><input type=""range"" multiple id=""od"" min=""0"" max=""5"" value=""0"">";
            informations += @"</br><label for=""do"">0 &nbsp;&nbsp;&nbsp;&nbsp;1 &nbsp;&nbsp;&nbsp;&nbsp;2 &nbsp;&nbsp;&nbsp;&nbsp;3 &nbsp;&nbsp;&nbsp;&nbsp;4 &nbsp;&nbsp;&nbsp;&nbsp;5</label></br><input type=""range"" multiple id=""do"" min=""0"" max=""5"" value=""5"">";
            informations += @"</br><label style=""font-size: 15px !important;"" for=""odCena"">0 &nbsp;&nbsp;&nbsp;100 &nbsp;&nbsp;&nbsp;200 &nbsp;&nbsp;&nbsp;300 &nbsp;&nbsp;&nbsp;400 &nbsp;&nbsp;&nbsp;500 ;&nbsp;&nbsp;&nbsp;600 &nbsp;&nbsp;&nbsp;&nbsp;700 &nbsp;&nbsp;&nbsp;&nbsp;800 &nbsp;&nbsp;&nbsp;&nbsp;900 &nbsp;&nbsp;&nbsp;&nbsp;1000+</label></br><input type=""range"" style=""width: 400px"" multiple id=""odCena"" min=""0"" max=""1000"" step=""100""value=""0"">";
            informations += @"</br><label style=""font-size: 15px !important;"" for=""doCena"">0 &nbsp;&nbsp;&nbsp;100 &nbsp;&nbsp;&nbsp;200 &nbsp;&nbsp;&nbsp;300 &nbsp;&nbsp;&nbsp;400 &nbsp;&nbsp;&nbsp;500 ;&nbsp;&nbsp;&nbsp;600 &nbsp;&nbsp;&nbsp;&nbsp;700 &nbsp;&nbsp;&nbsp;&nbsp;800 &nbsp;&nbsp;&nbsp;&nbsp;900 &nbsp;&nbsp;&nbsp;&nbsp;1000+</label></br><input type=""range"" style=""width: 400px"" multiple id=""doCena"" min=""0"" max=""1000"" step=""100"" value=""1000"">";
            informations += @"</br><label for=""datuOd"">Datum od:</label></br><input type=""date"" id=""odDatum"">";
            informations += @"</br><label for=""datuDo"">Datum do:</label></br><input type=""date"" id=""doDatum"">";
            informations += @"<div id=""filterError""></div></div>";
            #endregion
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
                    informations += @"<div></br><input type=""text"" id=""musterijaIme"" placeholder=""Pretrazi po imenu musterije""/>";
                    informations += @"</br><input type=""text"" id=""musterijaPrezime"" placeholder=""Pretrazi po prezimenu musterije""/>";
                    informations += @"</br><input type=""text"" id=""vozacIme"" placeholder=""Pretrazi po imenu vozaca""/>";
                    informations += @"</br><input type=""text"" id=""vozacPrezime"" placeholder=""Pretrazi po prezimenu vozaca""/></div>";
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