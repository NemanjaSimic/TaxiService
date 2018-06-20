using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TaxiSluzbaWebApi.Models;

namespace TaxiSluzbaWebApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/Korisnik")]
    public class KorisnikController : ApiController
    {
        [System.Web.Http.Route("LogIn")]
        public HttpResponseMessage LogIn([FromBody]JToken jToken)
        {
            var korisnickoIme = jToken.Value<string>("korisnickoIme");
            var sifra = jToken.Value<string>("sifra");

            foreach (var item in BazaPodataka.Instanca.Dispeceri)
            {
                if (item.KorisnickoIme.Equals(korisnickoIme))
                {
                    if (item.Sifra.Equals(sifra))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, item);
                    }
                }
            }
            foreach (var item in BazaPodataka.Instanca.Musterije)
            {
                if (item.KorisnickoIme.Equals(korisnickoIme))
                {
                    if (item.Sifra.Equals(sifra))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, item);
                    }
                }
            }
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (item.Sifra.Equals(sifra))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,item);
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [System.Web.Http.Route("Registracija")]
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


            foreach (var item in BazaPodataka.Instanca.Musterije)
            {
                if (musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }
            foreach (var item in BazaPodataka.Instanca.Dispeceri)
            {
                if (musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }
            foreach (var item in BazaPodataka.Instanca.Vozaci)
            {
                if (musterija.KorisnickoIme.Equals(item.KorisnickoIme))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }

            BazaPodataka.Instanca.Musterije.Add(musterija);
            BazaPodataka.Instanca.UpisiUBazuMusterije();
          
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [System.Web.Http.Route("Edit")]
        public HttpResponseMessage Edit([FromBody]Musterija m)
        {
            Musterija musterija = m;
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
                        try
                        {

                            BazaPodataka.Instanca.Vozaci.Remove(tempV);
                        }
                        catch (Exception)
                        {

                            
                        }
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
                    try
                    {
                        BazaPodataka.Instanca.Dispeceri.Remove(tempD);               
                    }
                    catch (Exception)
                    {

                        
                    }
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
                try
                {
                    BazaPodataka.Instanca.Musterije.Remove(tempM);
                }
                catch (Exception)
                {

                    
                }
                    musterija.Sifra = tempM.Sifra;
                    BazaPodataka.Instanca.Musterije.Add(musterija);
                    BazaPodataka.Instanca.UpisiUBazuMusterije();

                return Request.CreateResponse(HttpStatusCode.OK,musterija);
            }
        }

        [System.Web.Http.Route("ChangePass")]
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
                        try
                        {
                        BazaPodataka.Instanca.Vozaci.Remove(tempV);

                        }
                        catch (Exception)
                        {

                            
                        }
                        tempV.Sifra = m.Sifra;
                        BazaPodataka.Instanca.Vozaci.Add(tempV);
                        BazaPodataka.Instanca.UpisiUBazuVozace();
                        return Request.CreateResponse(HttpStatusCode.OK,m);
                    }
                }
                else
                {
                    try
                    {
                    BazaPodataka.Instanca.Dispeceri.Remove(tempD);

                    }
                    catch (Exception)
                    {

                        
                    }
                    tempD.Sifra = m.Sifra;
                    BazaPodataka.Instanca.Dispeceri.Add(tempD);
                    BazaPodataka.Instanca.UpisiUBazuDispecere();
                    return Request.CreateResponse(HttpStatusCode.OK,m);
                }
            }
            else
            {
                try
                {
                BazaPodataka.Instanca.Musterije.Remove(tempM);

                }
                catch (Exception)
                {

                    
                }
                tempM.Sifra = m.Sifra;
                BazaPodataka.Instanca.Musterije.Add(tempM);
                BazaPodataka.Instanca.UpisiUBazuMusterije();
                return Request.CreateResponse(HttpStatusCode.OK,m);
            }
        }
    }
}