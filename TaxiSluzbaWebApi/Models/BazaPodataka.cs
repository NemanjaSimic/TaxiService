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
        private static BazaPodataka instanca;
        private BazaPodataka()
        {
            Dispeceri = new List<Dispecer>();
            Musterije = new List<Musterija>();
            Vozaci = new List<Vozac>();
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

        private void UcitajDispecere()
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

        private void UcitajMusterije()
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

        private void UcitajVozace()
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
                        Email = parametri[7]
                    };
                    Vozaci.Add(vozac);
                }
            }
        }

        public void UcitajPodatkeIzBaze()
        {
            UcitajDispecere();
            UcitajMusterije();
            UcitajVozace();
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
                        tw.Write("\n");
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
                        tw.Write("\n");
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
                        tw.Write("\n");
                    }
                }
            }
            catch
            {

            }
        }
    }
}