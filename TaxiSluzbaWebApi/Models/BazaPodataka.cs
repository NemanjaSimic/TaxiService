using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TaxiSluzbaWebApi.Models
{
    public class BazaPodataka
    {
        public List<Dispecer> Dispeceri { get; set; }
        public List<Musterija> Musterije { get; set; }
        public List<Vozac> Vozaci { get; set; }
        public List<Voznja> Voznje { get; set; }
        
        private static BazaPodataka instanca;
        private BazaPodataka()
        {
            Dispeceri = new List<Dispecer>();
            Musterije = new List<Musterija>();
            Vozaci = new List<Vozac>();
            Voznje = new List<Voznja>();
        }
        public static BazaPodataka Instanca
        {
            get
            {
                if (instanca == null)
                {
                    instanca = new BazaPodataka();
                }
                return instanca;
            }
        }

        public void UcitajDispecere()
        {
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Dispeceri.txt"))
            {
                Dispecer dispecer = null;
                string informacije = string.Empty;
                while ((informacije = tr.ReadLine()) != null)
                {
                    string[] parametri = informacije.Split(';');
                    dispecer = new Dispecer()
                    {
                        KorisnickoIme = parametri[0],
                        Sifra = parametri[1],
                        Ime = parametri[2],
                        Prezime = parametri[3],
                        Pol = (parametri[4].Equals("Muski")) ? Enum.Pol.Muski : Enum.Pol.Zenski,
                        JMBG = parametri[5],
                        KontaktTelefon = parametri[6],
                        Email = parametri[7]
                    };
                    Dispeceri.Add(dispecer);
                }
            }
        }

        public void UcitajMusterije()
        {
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Musterije.txt"))
            {
                Musterija musterija = null;
                string informacije = string.Empty;
                while ((informacije = tr.ReadLine()) != null)
                {
                    string[] parametri = informacije.Split(';');
                    musterija = new Musterija()
                    {
                        KorisnickoIme = parametri[0],
                        Sifra = parametri[1],
                        Ime = parametri[2],
                        Prezime = parametri[3],
                        Pol = (parametri[4].Equals("Muski")) ? Enum.Pol.Muski : Enum.Pol.Zenski,
                        JMBG = parametri[5],
                        KontaktTelefon = parametri[6],
                        Email = parametri[7]
                    };
                    Musterije.Add(musterija);
                }
            }
        }

        public void UcitajVozace()
        {
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Vozaci.txt"))
            {
                Vozac vozac = null;
                string informacije = string.Empty;
                while ((informacije = tr.ReadLine()) != null)
                {
                    string[] parametri = informacije.Split(';');
                    vozac = new Vozac()
                    {
                        KorisnickoIme = parametri[0],
                        Sifra = parametri[1],
                        Ime = parametri[2],
                        Prezime = parametri[3],
                        Pol = (parametri[4].Equals("Muski")) ? Enum.Pol.Muski : Enum.Pol.Zenski,
                        JMBG = parametri[5],
                        KontaktTelefon = parametri[6],
                        Email = parametri[7],
                        
                    };
                    //vozac.Lokacija.Adresa.Ulica = parametri[8];
                    //vozac.Lokacija.Adresa.Broj = parametri[9];
                    //vozac.Lokacija.Adresa.NasenjenoMesto = parametri[10];
                    //vozac.Lokacija.Adresa.PozivniBroj = parametri[11];

                    Vozaci.Add(vozac);
                }
            }
           
        }

        public void UcitajVoznje()
        {
            int id = 1;
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Voznje.txt"))
            {
                Voznja voznja = null;              
                string informacije = string.Empty;
                while ((informacije = tr.ReadLine()) != null)
                {
                    voznja = new Voznja
                    {
                        Lokacija = new Lokacija(),
                        Vozac = new Vozac(),
                        Komentar = new Komentar(),
                        Musterija = new Musterija(),
                        Odrediste = new Lokacija(),                       
                    };
                    string[] parametri = informacije.Split(';');                   
                    voznja.ID = id++;
                    voznja.DatumVremePoruzbine = DateTime.Parse(parametri[0]);
                    voznja.Lokacija.Adresa.Ulica = parametri[1];
                    voznja.Lokacija.Adresa.Broj = parametri[2];
                    voznja.Lokacija.Adresa.NasenjenoMesto = parametri[3];
                    voznja.Lokacija.Adresa.PozivniBroj = parametri[4];
                    Int32.TryParse(parametri[5], out int tip);
                    if (tip == 0)
                    {
                        voznja.TipAutomobila = Enum.TipAutomobila.BezNaznake;
                    }
                    else if(tip == 1)
                    {
                        voznja.TipAutomobila = Enum.TipAutomobila.Putnicki;
                    }
                    else
                    {
                        voznja.TipAutomobila = Enum.TipAutomobila.Kombi;
                    }
                    if (!parametri[6].Equals(""))
                    {
                        voznja.Musterija = BazaPodataka.Instanca.NadjiMusteriju(parametri[6]);
                    }
                    voznja.Odrediste.Adresa = new Adresa
                    {
                        Ulica = parametri[7],
                        Broj = parametri[8],
                        NasenjenoMesto = parametri[9],
                        PozivniBroj = parametri[10]
                    };
                    if (!parametri[11].Equals(""))
                    {
                        voznja.Dispecer = BazaPodataka.Instanca.NadjiDispecera(parametri[11]);
                    }
                    if (!parametri[12].Equals(""))
                    {
                        voznja.Vozac = BazaPodataka.Instanca.NadjiVozaca(parametri[12]);
                    }
                    voznja.StatusVoznje = Enum.StatusVoznje.Uspesna;

                    Voznje.Add(voznja);
                }
            }

        }

        public void UcitajPodatkeIzBaze()
        {
            UcitajDispecere();
            UcitajMusterije();
            UcitajVozace();
            UcitajVoznje();
        }

        public void UpisiUBazuMusterije()
        {
            try
            {
                using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Musterije.txt"))
                {
                    foreach (var item in Musterije)
                    {
                        tw.Write(item.KorisnickoIme);
                        tw.Write(";");
                        tw.Write(item.Sifra);
                        tw.Write(";");
                        tw.Write(item.Ime);
                        tw.Write(";");
                        tw.Write(item.Prezime);
                        tw.Write(";");
                        tw.Write(item.Pol);
                        tw.Write(";");
                        tw.Write(item.JMBG);
                        tw.Write(";");
                        tw.Write(item.KontaktTelefon);
                        tw.Write(";");
                        tw.Write(item.Email);
                        if(Musterije.IndexOf(item) != Musterije.Count() - 1)
                        {
                            tw.Write("\n");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void UpisiUBazuDispecere()
        {
            try
            {
                using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Dispeceri.txt"))
                {
                    foreach (var item in Dispeceri)
                    {
                        tw.Write(item.KorisnickoIme);
                        tw.Write(";");
                        tw.Write(item.Sifra);
                        tw.Write(";");
                        tw.Write(item.Ime);
                        tw.Write(";");
                        tw.Write(item.Prezime);
                        tw.Write(";");
                        tw.Write(item.Pol);
                        tw.Write(";");
                        tw.Write(item.JMBG);
                        tw.Write(";");
                        tw.Write(item.KontaktTelefon);
                        tw.Write(";");
                        tw.Write(item.Email);
                        if (Dispeceri.IndexOf(item) != Dispeceri.Count() - 1)
                        {
                            tw.Write("\n");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void UpisiUBazuVozace()
        {
            try
            {
                using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Vozaci.txt"))
                {
                    foreach (var item in Vozaci)
                    {
                        tw.Write(item.KorisnickoIme);
                        tw.Write(";");
                        tw.Write(item.Sifra);
                        tw.Write(";");
                        tw.Write(item.Ime);
                        tw.Write(";");
                        tw.Write(item.Prezime);
                        tw.Write(";");
                        tw.Write(item.Pol);
                        tw.Write(";");
                        tw.Write(item.JMBG);
                        tw.Write(";");
                        tw.Write(item.KontaktTelefon);
                        tw.Write(";");
                        tw.Write(item.Email);
                        //tw.Write(";");
                        //tw.Write(item.Lokacija.Adresa.Ulica);
                        //tw.Write(";");
                        //tw.Write(item.Lokacija.Adresa.Broj);
                        //tw.Write(";");
                        //tw.Write(item.Lokacija.Adresa.NasenjenoMesto);
                        //tw.Write(";");
                        //tw.Write(item.Lokacija.Adresa.PozivniBroj);
                        if (Vozaci.IndexOf(item) != Vozaci.Count() - 1)
                        {
                            tw.Write("\n");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void UpisiUBazuVoznje()
        {
            try
            {
                using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiSluzbaWebApi\TaxiService\TaxiSluzbaWebApi\App_Data\Voznje.txt"))
                {
                    foreach (var item in Voznje)
                    {
                        tw.Write(item.DatumVremePoruzbine.ToString());
                        tw.Write(";");
                        tw.Write(item.Lokacija.Adresa.Ulica);
                        tw.Write(";");
                        tw.Write(item.Lokacija.Adresa.Broj);
                        tw.Write(";");
                        tw.Write(item.Lokacija.Adresa.NasenjenoMesto);
                        tw.Write(";");
                        tw.Write(item.Lokacija.Adresa.PozivniBroj);
                        tw.Write(";");                       
                        tw.Write(item.TipAutomobila.ToString());
                        tw.Write(";");
                        if (item.Musterija == null)
                        {
                            tw.Write("");
                        }
                        else
                        {
                            tw.Write(item.Musterija.KorisnickoIme);
                        }
                        tw.Write(";");
                        if (item.Odrediste == null)
                        {
                            tw.Write("");
                            tw.Write(";");
                            tw.Write("");
                            tw.Write(";");
                            tw.Write("");
                            tw.Write(";");
                            tw.Write("");
                            tw.Write(";");
                        }
                        else
                        {
                            tw.Write(item.Odrediste.Adresa.Ulica);
                            tw.Write(";");
                            tw.Write(item.Odrediste.Adresa.Broj);
                            tw.Write(";");
                            tw.Write(item.Odrediste.Adresa.NasenjenoMesto);
                            tw.Write(";");
                            tw.Write(item.Odrediste.Adresa.PozivniBroj);
                            tw.Write(";");
                        }
                        
                        if (item.Dispecer == null)
                        {
                            tw.Write("");
                        }
                        else
                        {
                            tw.Write(item.Dispecer.KorisnickoIme);
                        }
                        tw.Write(";");
                        if (item.Vozac == null)
                        {
                            tw.Write("");
                            tw.Write(";");
                        }
                        else
                        {
                            tw.Write(item.Vozac.KorisnickoIme);
                            tw.Write(";");
                        }
                        if (item.Iznos <= 0)
                        {
                            tw.Write("");
                            tw.Write(";");
                        }
                        else
                        {
                            tw.Write(item.Iznos.ToString());
                            tw.Write(";");
                        }
                        if (item.Komentar == null)
                        {
                            tw.Write("");
                            tw.Write(";");
                        }
                        else
                        {
                            tw.Write(item.Komentar.ID.ToString());
                            tw.Write(";");
                        }                        
                        
                        tw.Write(item.StatusVoznje.ToString());
                        if (Voznje.IndexOf(item) != Voznje.Count() - 1)
                        {
                            tw.Write("\n");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public Vozac NadjiVozaca(string korisnickoIme)
        {
            foreach (var item in Vozaci)
            {
                if (item.KorisnickoIme.Equals(korisnickoIme))
                {
                    return item;
                }
            }
            return null;
        }

        public Musterija NadjiMusteriju(string korisnickoIme)
        {
            foreach (var item in Musterije)
            {
                if (item.KorisnickoIme.Equals(korisnickoIme))
                {
                    return item;
                }
            }
            return null;
        }

        public Dispecer NadjiDispecera(string korisnickoIme)
        {
            foreach (var item in Dispeceri)
            {
                if (item.KorisnickoIme.Equals(korisnickoIme))
                {
                    return item;
                }
            }
            return null;
        }
    }
}