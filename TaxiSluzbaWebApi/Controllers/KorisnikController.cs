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
    [RoutePrefix("api/Korisnik")]
    public class KorisnikController : ApiController
    {
        [HttpGet]
        [Route("LogIn")]
        public HttpResponseMessage LogIn()
        {
            var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());
            
            var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");
            var sifra = jsonObj.Value<string>("Sifra");
            var refresh = jsonObj.Value<bool>("Refresh");
            var response = new HttpResponseMessage();

            var page = "";

            if (korisnickoIme == null || sifra == null)
            {
                response.Content = new StringContent("");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (HttpContext.Current.Application["ulogovani"] == null)
            {
                HttpContext.Current.Application["ulogovani"] = new List<User>();
            }
            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];

            //var ulogovan = (User)HttpContext.Current.Session["ulogovan"];
            //if (ulogovan != null)
            //{
            //    response.Content = new StringContent("");
            //    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            //    response.StatusCode = HttpStatusCode.Forbidden;
            //    return response;
            //}
            if (!refresh)
            {
                foreach (var item in ulogovani)
                {
                    if (item.KorisnickoIme.Equals(korisnickoIme))
                    {
                        response.Content = new StringContent("");
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.Forbidden;
                        return response;
                    }
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
                    else if(v.Sifra.Equals(sifra))
                    {
                        page += @"<button id=""view"">Pregledaj profil</button>";
                        page += @"<button id=""edit"">Izmeni profil</button>";
                        page += @"<button id=""change"">Promeni sifru</button>";
                        page += @"<button id=""changeLoc"">Promeni lokaciju</button>";
                        page += @"<button id=""changeStatus"">Promeni status voznje</button>";
                        page += @"<button id=""waiting"">Prikazi voznje na cekanju</button>";
                        page += @"<button id=""logout"">Izloguj se</button>";
                        response.Content = new StringContent(page);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.OK;
                        //HttpContext.Current.Session["ulogovan"] = new Vozac();
                        //HttpContext.Current.Session["ulogovan"] = v;
                        //HttpContext.Current.Application["ulogovan"] = new Vozac();
                        if (!refresh)
                        {
                            ulogovani.Add(v);
                            HttpContext.Current.Application["ulogovani"] = ulogovani;
                        }
                        return response;
                    }
                    else
                    {
                        response.Content = new StringContent("");
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.Unauthorized;
                        return response;
                    }
                }
                else if (d.Sifra.Equals(sifra))
                {
                    page += @"<button id=""view"">Pregledaj profil</button>";
                    page += @"<button id=""edit"">Izmeni profil</button>";
                    page += @"<button id=""change"">Promeni sifru</button>";
                    page += @"<button id=""kreirajVozaca"">Kreiraj vozaca</button>";
                    page += @"<button id=""formirajVoznju"">Formiraj voznju</button>";
                    page += @"<button id=""obradiVoznju"" > Obradi voznju</button>";
                    page += @"<button id=""pregledajVoznje"" > Ucitaj sve voznje</button>";
                    page += @"<button id=""logout"">Izloguj se</button>";
                    response.Content = new StringContent(page);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.OK;
                    if (!refresh)
                    {
                        ulogovani.Add(d);
                        HttpContext.Current.Application["ulogovani"] = ulogovani;
                    }
                    
                    //HttpContext.Current.Session["ulogovan"] = new Dispecer();
                    //HttpContext.Current.Session["ulogovan"] = d;
                    return response;
                }
                else
                {
                    response.Content = new StringContent("");
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    return response;
                }
            }
            else if(m.Sifra.Equals(sifra))
            {
                page += @"<button id=""view"">Pregledaj profil</button>";
                page += @"<button id=""edit"">Izmeni profil</button>";
                page += @"<button id=""change"">Promeni sifru</button>";
                page += @"<button id=""request"">Zahtev za voznju</button>";
                page += @"<button id=""sort"">Sortiraj voznje</button>";
                page += @"<button id=""filter"">Filtriraj voznje</button>";
                page += @"<button id=""logout"">Izloguj se</button>";
                response.Content = new StringContent(page);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                if (!refresh)
                {
                    ulogovani.Add(m);
                    HttpContext.Current.Application["ulogovani"] = ulogovani;
                }               
                //HttpContext.Current.Session["ulogovan"] = new Musterija();
                //HttpContext.Current.Session["ulogovan"] = m;
                return response;
            }
            else
            {
                response.Content = new StringContent("");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
          
        }
        [HttpDelete]
        [Route("LogOut")]
        public HttpResponseMessage LogOut([FromBody]JToken jToken)
        {
            //var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());

            //var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");
            var korisnickoIme = jToken.Value<string>("KorisnickoIme");
            var ulogovani = (List<User>)HttpContext.Current.Application["ulogovani"];


            if (ulogovani == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                ulogovani.Remove(ulogovani.Find(x => x.KorisnickoIme.Equals(korisnickoIme)));
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            HttpContext.Current.Application["ulogovani"] = ulogovani;
            var ulogovani2 = (List<User>)HttpContext.Current.Application["ulogovani"];
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Route("Registracija")]
        public HttpResponseMessage Registracija([FromBody]Musterija musterija)
        {
            if (musterija.Ime == null || musterija.Prezime == null || musterija.KontaktTelefon == null 
                || musterija.JMBG == null || musterija.Sifra == null || musterija.Email == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if(musterija.Ime == "" || musterija.Prezime == "" || musterija.KontaktTelefon == ""
                || musterija.JMBG == "" || musterija.Sifra == "" || musterija.Email == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            
            var input = musterija.KontaktTelefon;
            Regex pattern = new Regex(@"\d{8,10}");
            Match match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = musterija.JMBG;
            pattern = new Regex(@"\d{13}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }


            if(BazaPodataka.Instanca.NadjiDispecera(musterija.KorisnickoIme) != null || 
                BazaPodataka.Instanca.NadjiMusteriju(musterija.KorisnickoIme) != null ||
                BazaPodataka.Instanca.NadjiVozaca(musterija.KorisnickoIme) != null )
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            BazaPodataka.Instanca.Musterije.Add(musterija);
            BazaPodataka.Instanca.UpisiUBazuMusterije();
          
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPut]
        [Route("Edit")]
        public HttpResponseMessage Edit([FromBody]Musterija musterija)
        {
            if (musterija.Ime == null || musterija.Prezime == null || musterija.KontaktTelefon == null
                || musterija.JMBG == null || musterija.Sifra == null || musterija.Email == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (musterija.Ime == "" || musterija.Prezime == "" || musterija.KontaktTelefon == ""
                || musterija.JMBG == "" || musterija.Sifra == "" || musterija.Email == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            bool istoKorIme = musterija.KorisnickoIme.Equals(musterija.Sifra);        

            var input = musterija.KontaktTelefon;
            Regex pattern = new Regex(@"\d{8,10}");
            Match match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            input = musterija.JMBG;
            pattern = new Regex(@"\d{13}");
            match = pattern.Match(input);
            if (!match.Success)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Musterija tempM = null;
            Vozac tempV = null;
            Dispecer tempD = null;

            foreach (var item in BazaPodataka.Instanca.Musterije)
            {
                if (istoKorIme && musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    tempM = item;                 
                }
                else if (!istoKorIme && musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                else if (!istoKorIme && musterija.Sifra.Equals(item.KorisnickoIme))
                {
                    tempM = item;
                }
            }

            if(tempM == null)
            {
                foreach (var item in BazaPodataka.Instanca.Dispeceri)
                {
                    if (istoKorIme && musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                    {
                        tempD = item;
                    }
                    else if(!istoKorIme && musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }else if(!istoKorIme && musterija.Sifra.Equals(item.KorisnickoIme))
                    {
                        tempD = item;
                    }
                }
                if(tempD == null)
                {
                    foreach (var item in BazaPodataka.Instanca.Vozaci)
                    {
                        if (istoKorIme && musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                        {
                            tempV = item;
                        }
                        else if (!istoKorIme && musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                        {
                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                        else if (!istoKorIme && musterija.Sifra.Equals(item.KorisnickoIme))
                        {
                            tempV = item;
                        }
                    }
                    if (tempV == null) {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        BazaPodataka.Instanca.Vozaci.Remove(tempV);
                        tempV.KorisnickoIme = musterija.KorisnickoIme;
                        tempV.Ime = musterija.Ime;
                        tempV.Prezime = musterija.Prezime;
                        tempV.JMBG = musterija.JMBG;
                        tempV.Email = musterija.Email;
                        tempV.KontaktTelefon = musterija.KontaktTelefon;
                        BazaPodataka.Instanca.Vozaci.Add(tempV);
                        BazaPodataka.Instanca.UpisiUBazuVozace();
                        return Request.CreateResponse(HttpStatusCode.OK,tempV);
                    }
                }
                else
                {                  
                    BazaPodataka.Instanca.Dispeceri.Remove(tempD);               
                    tempD.KorisnickoIme = musterija.KorisnickoIme;
                    tempD.Ime = musterija.Ime;
                    tempD.Prezime = musterija.Prezime;
                    tempD.JMBG = musterija.JMBG;
                    tempD.Email = musterija.Email;
                    tempD.KontaktTelefon = musterija.KontaktTelefon;
                    BazaPodataka.Instanca.Dispeceri.Add(tempD);
                    BazaPodataka.Instanca.UpisiUBazuDispecere();
                    return Request.CreateResponse(HttpStatusCode.OK,tempD);
                }
            }
            else
            {
                    BazaPodataka.Instanca.Musterije.Remove(tempM);
                    musterija.Sifra = tempM.Sifra;
                    BazaPodataka.Instanca.Musterije.Add(musterija);
                    BazaPodataka.Instanca.UpisiUBazuMusterije();

                return Request.CreateResponse(HttpStatusCode.OK,musterija);
            }
        }
        [HttpPut]
        [Route("ChangePass")]
        public HttpResponseMessage ChangePass([FromBody]Musterija m)
        {
            Musterija tempM = null;
            Vozac tempV = null;
            Dispecer tempD = null;
            foreach (var item in BazaPodataka.Instanca.Musterije)
            {
                if (m.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    tempM = item;
                }
            }
            if(tempM == null)
            {
                foreach (var item in BazaPodataka.Instanca.Dispeceri)
                {
                    if (m.KorisnickoIme.Equals(item.KorisnickoIme))
                    {
                        tempD = item;
                    }
                }
                if(tempD == null)
                {
                    foreach (var item in BazaPodataka.Instanca.Vozaci)
                    {
                        if (m.KorisnickoIme.Equals(item.KorisnickoIme))
                        {
                            tempV = item;
                        }
                    }
                    if(tempV == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        BazaPodataka.Instanca.Vozaci.Remove(tempV);
                        tempV.Sifra = m.Sifra;
                        BazaPodataka.Instanca.Vozaci.Add(tempV);
                        BazaPodataka.Instanca.UpisiUBazuVozace();
                        return Request.CreateResponse(HttpStatusCode.OK,m);
                    }
                }
                else
                {
                    BazaPodataka.Instanca.Dispeceri.Remove(tempD);
                    tempD.Sifra = m.Sifra;
                    BazaPodataka.Instanca.Dispeceri.Add(tempD);
                    BazaPodataka.Instanca.UpisiUBazuDispecere();
                    return Request.CreateResponse(HttpStatusCode.OK,m);
                }
            }
            else
            {
                BazaPodataka.Instanca.Musterije.Remove(tempM);
                tempM.Sifra = m.Sifra;
                BazaPodataka.Instanca.Musterije.Add(tempM);
                BazaPodataka.Instanca.UpisiUBazuMusterije();
                return Request.CreateResponse(HttpStatusCode.OK,m);
            }
        }
        [HttpGet]
        [Route("Profil")]
        public HttpResponseMessage Profil()
        {
            var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());

            var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");

            var response = new HttpResponseMessage();

            var page = "";

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
                        response.StatusCode = HttpStatusCode.BadRequest;
                        return response;
                    }
                    else
                    {
                        page += $"<table><tr><td>Ime:</td><td>{v.Ime}</td></tr>";
                        page += $"<tr><td>Prezime:</td><td>{v.Prezime}</td></tr>";
                        page += $"<tr><td>JMBG:</td><td>{v.JMBG}</td></tr>";
                        page += $"<tr><td>Korisnicko ime:</td><td>{v.KorisnickoIme}</td></tr>";
                        page += $"<tr><td>Email:</td><td>{v.Email}</td></tr>";
                        page += $"<tr><td>BrojTelefona:</td><td>{v.KontaktTelefon}</td></tr></table>";                    
                        response.Content = new StringContent(page);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.OK;
                        return response;
                    }                   
                }                
                else
                {

                    page += $"<table><tr><td>Ime:</td><td>{d.Ime}</td></tr>";
                    page += $"<tr><td>Prezime:</td><td>{d.Prezime}</td></tr>";
                    page += $"<tr><td>JMBG:</td><td>{d.JMBG}</td></tr>";
                    page += $"<tr><td>Korisnicko ime:</td><td>{d.KorisnickoIme}</td></tr>";
                    page += $"<tr><td>Email:</td><td>{d.Email}</td></tr>";
                    page += $"<tr><td>BrojTelefona:</td><td>{d.KontaktTelefon}</td></tr></table>";
                    response.Content = new StringContent(page);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }           
            else
            {
                page += $"<table><tr><td>Ime:</td><td>{m.Ime}</td></tr>";
                page += $"<tr><td>Prezime:</td><td>{m.Prezime}</td></tr>";
                page += $"<tr><td>JMBG:</td><td>{m.JMBG}</td></tr>";
                page += $"<tr><td>Korisnicko ime:</td><td>{m.KorisnickoIme}</td></tr>";
                page += $"<tr><td>Email:</td><td>{m.Email}</td></tr>";
                page += $"<tr><td>BrojTelefona:</td><td>{m.KontaktTelefon}</td></tr></table>";
                response.Content = new StringContent(page);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
        }
        [HttpGet]
        [Route("ProfilEdit")]
        public HttpResponseMessage ProfilEdit()
        {
            var jsonObj = JToken.Parse(Request.RequestUri.ToString().Split('?').Last());

            var korisnickoIme = jsonObj.Value<string>("KorisnickoIme");

            var response = new HttpResponseMessage();

            var page = "";

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
                        response.StatusCode = HttpStatusCode.BadRequest;
                        return response;
                    }
                    else
                    {
                        page += @"<table><tr><td>Ime:</td><td><input type=""text"" value=""" + v.Ime + @""" id=""imeEdit""/></td></tr>";
                        page += @"<tr><td>Prezime:</td><td><input type=""text"" value=""" + v.Prezime + @""" id=""prezimeEdit""/></td></tr>";
                        page += @"<tr><td>JMBG:</td><td><input type=""text"" value=""" + v.JMBG + @""" id=""jmbgEdit""/></td></tr>";
                        page += @"<tr><td>Korisnicko ime:</td><td><input type=""text"" value=""" + v.KorisnickoIme + @""" id=""korisnickoImeEdit""/></td></tr>";
                        page += @"<tr><td>Email:</td><td><input type=""text"" value=""" + v.Email + @""" id=""emailEdit""/></td></tr>";
                        page += @"<tr><td>BrojTelefona:</td><td><input type=""text"" value=""" + v.KontaktTelefon + @""" id=""brojTelefonaEdit""/></td></tr>";
                        page += @"<tr><td></td><td><button id=""buttonEdit"">Sacuvaj izmene</button></td></tr></table>";
                        page += @"<div id=""regValEdit""></div>";
                        response.Content = new StringContent(page);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        response.StatusCode = HttpStatusCode.OK;
                        return response;
                    }
                }
                else
                {

                    page += @"<table><tr><td>Ime:</td><td><input type=""text"" value=""" + d.Ime + @""" id=""imeEdit""/></td></tr>";
                    page += @"<tr><td>Prezime:</td><td><input type=""text"" value=""" + d.Prezime + @""" id=""prezimeEdit""/></td></tr>";
                    page += @"<tr><td>JMBG:</td><td><input type=""text"" value=""" + d.JMBG + @""" id=""jmbgEdit""/></td></tr>";
                    page += @"<tr><td>Korisnicko ime:</td><td><input type=""text"" value=""" + d.KorisnickoIme + @""" id=""korisnickoImeEdit""/></td></tr>";
                    page += @"<tr><td>Email:</td><td><input type=""text"" value=""" + d.Email + @""" id=""emailEdit""/></td></tr>";
                    page += @"<tr><td>BrojTelefona:</td><td><input type=""text"" value=""" + d.KontaktTelefon + @""" id=""brojTelefonaEdit""/></td></tr>";
                    page += @"<tr><td></td><td><button id=""buttonEdit"">Sacuvaj izmene</button></td></tr></table>";
                    page += @"<div id=""regValEdit""></div>";
                    response.Content = new StringContent(page);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }
            else
            {
                page += @"<table><tr><td>Ime:</td><td><input type=""text"" value=""" + m.Ime + @""" id=""imeEdit""/></td></tr>";
                page += @"<tr><td>Prezime:</td><td><input type=""text"" value=""" + m.Prezime + @""" id=""prezimeEdit""/></td></tr>";
                page += @"<tr><td>JMBG:</td><td><input type=""text"" value=""" + m.JMBG + @""" id=""jmbgEdit""/></td></tr>";
                page += @"<tr><td>Korisnicko ime:</td><td><input type=""text"" value=""" + m.KorisnickoIme + @""" id=""korisnickoImeEdit""/></td></tr>";
                page += @"<tr><td>Email:</td><td><input type=""text"" value=""" + m.Email + @""" id=""emailEdit""/></td></tr>";
                page += @"<tr><td>BrojTelefona:</td><td><input type=""text"" value=""" + m.KontaktTelefon + @""" id=""brojTelefonaEdit""/></td></tr>";
                page += @"<tr><td></td><td><button id=""buttonEdit"">Sacuvaj izmene</button></td></tr></table>";
                page += @"<div id=""regValEdit""></div>";
                response.Content = new StringContent(page);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
        }
        [HttpGet]
        [Route("Sifra")]
        public HttpResponseMessage Sifra()
        {          
            var response = new HttpResponseMessage();
            var page = "";
            page += @"<table><tr><td>Unesite sifru:</td><td><input type=""password"" id=""sifraAut"" /></td></tr>";
            page += @"<tr><td>Unesite novu sifru:</td><td><input type=""password"" id=""sifraNova"" /></td></tr>";
            page += @"<tr><td>Unesite ponovite novu sifru:</td><td><input type=""password"" id=""sifraNovaP"" /></td></tr>";
            page += @"<tr><td></td><td><button id=""promeniSifru"">Promeni sifru</ button></td></tr></table>";
            page += @"<div id=""regValEditS""></div>";
            response.Content = new StringContent(page);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

    }
}
