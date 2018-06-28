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
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Dispeceri.txt"))
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
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Musterije.txt"))
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
                        Email = parametri[7],
                        Blokiran = (parametri[8].Equals("False")) ? false : true
                    };
                    Musterije.Add(musterija);
                }
            }
        }

        public void UcitajVozace()
        {
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Vozaci.txt"))
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
                        Blokiran = (parametri[12].Equals("False")) ? false : true,
                        Automobil = new Automobil()
                    };
                   
                    vozac.Automobil.BrojRegistarskeOznake = parametri[8];
                    vozac.Automobil.Godiste = parametri[9];
                    vozac.Automobil.BrojTaksiVozila = parametri[10];
                    vozac.Automobil.TipAutomobila = (Enum.TipAutomobila)System.Enum.Parse(typeof(Enum.TipAutomobila), parametri[11]);
                    Vozaci.Add(vozac);
                }
            }
           
        }

        public void UcitajVoznje()
        {
            int id = 1;
            using (TextReader tr = new StreamReader(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Voznje.txt"))
            {
                Voznja voznja = null;
                var musterija = new Musterija();
                var dispecer = new Dispecer();
                var vozac = new Vozac();
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
                        Dispecer = new Dispecer()
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
                        musterija = BazaPodataka.Instanca.NadjiMusteriju(parametri[6]);
                        voznja.Musterija = musterija;
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
                        dispecer = BazaPodataka.Instanca.NadjiDispecera(parametri[11]);
                        voznja.Dispecer = dispecer;
                    }
                    if (!parametri[12].Equals(""))
                    {
                        vozac = BazaPodataka.Instanca.NadjiVozaca(parametri[12]);
                        voznja.Vozac = vozac;
                    }
                    Int32.TryParse(parametri[13], out int iznos);
                    voznja.Iznos = iznos;
                    Int32.TryParse(parametri[14], out int kom);
                    voznja.StatusVoznje =(Enum.StatusVoznje)System.Enum.Parse(typeof(Enum.StatusVoznje), parametri[14]);
                    if (!parametri[15].Equals(""))
                    {
                        voznja.Komentar.DatumObjave = DateTime.Parse(parametri[15]);
                    }
                    if (!parametri[16].Equals(""))
                    {
                        voznja.Komentar.Korisnik = parametri[16];
                    }
                    if (!parametri[17].Equals(""))
                    {
                        voznja.Komentar.Opis = parametri[17];
                    }
                    if (!parametri[18].Equals(""))
                    {
                        Int32.TryParse(parametri[18], out int ocena);
                        voznja.Komentar.Ocena = ocena;
                    }
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
            using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Musterije.txt"))
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
                    tw.Write(";");
                    tw.Write(item.Blokiran.ToString());
                    if (Musterije.IndexOf(item) != Musterije.Count() - 1)
                    {
                        tw.Write("\n");
                    }
                }
            }
        }

        public void UpisiUBazuDispecere()
        {
                using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Dispeceri.txt"))
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

        public void UpisiUBazuVozace()
        {
            using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Vozaci.txt"))
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
                    tw.Write(";");
                    tw.Write(item.Automobil.BrojRegistarskeOznake);
                    tw.Write(";");
                    tw.Write(item.Automobil.Godiste);
                    tw.Write(";");
                    tw.Write(item.Automobil.BrojTaksiVozila);
                    tw.Write(";");
                    tw.Write(item.Automobil.TipAutomobila.ToString());
                    tw.Write(";");
                    tw.Write(item.Blokiran.ToString());
                    if (Vozaci.IndexOf(item) != Vozaci.Count() - 1)
                    {
                        tw.Write("\n");
                    }
                }
            }
        }

        public void UpisiUBazuVoznje()
        {
            using (TextWriter tw = new StreamWriter(@"C:\Users\Nemanja\Desktop\FAKS\3.GODINA\WEB\TaxiSluzbaWebApp\TaxiService\TaxiSluzbaWebApi\App_Data\Voznje.txt"))
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
                        tw.Write("0");
                        tw.Write(";");
                    }
                    else
                    {
                        tw.Write(item.Iznos.ToString());
                        tw.Write(";");
                    }                

                    tw.Write(item.StatusVoznje.ToString());
                    tw.Write(";");

                    if (item.Komentar.Korisnik != null)
                    {

                        tw.Write(item.Komentar.DatumObjave.ToString());
                        tw.Write(";");
                        tw.Write(item.Komentar.Korisnik);
                        tw.Write(";");
                        tw.Write(item.Komentar.Opis);
                        tw.Write(";");
                        tw.Write(item.Komentar.Ocena);
                        tw.Write(";");
                    }
                    else
                    {
                        tw.Write(";");
                        tw.Write(";");
                        tw.Write(";");
                        tw.Write(";");
                    }

                    if (Voznje.IndexOf(item) != Voznje.Count() - 1)
                    {
                        tw.Write("\n");
                    }
                }
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