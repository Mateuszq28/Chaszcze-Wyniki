using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.IO;

//do formatowania dat
using System.Globalization;

//Generowanie excela
using Syncfusion.XlsIO;

namespace Chaszcze_Wyniki
{
    class Zarzadzanie
    {
        static public List<Patrol> danePatroli = new List<Patrol>();
        static public bool czyWynikiTrwaja = false;

        //Stałe wartości
        //Nazwa pliku do zapisywania savów z gry
        static string nazwaPliku = "wyniki_chaszcze.txt";
        static string sekwencjaSynchro = "???xxxSTART_SYNCHROxxx???";

        //Do rozpoznawania formatu daty
        static CultureInfo provider = CultureInfo.InvariantCulture;
        static string formatData = "dd.MM.yyyy HH:mm:ss";
        static string formatGodzina = "HH:mm";


        //Resetuje zbieranie wyników
        static public void reset()
        {
            Zarzadzanie.czyWynikiTrwaja = false;
            foreach (Patrol p in danePatroli)
            {
                p.kodyLampionow.Clear();
            }
            danePatroli.Clear();
        }


        //Zapisywanie gry do pliku
        //WERSJA TEKSTOWA
        static public string SaveGame()
        {
            string zawartoscPliku;
            if (czyWynikiTrwaja) zawartoscPliku = "1";
            else zawartoscPliku = "0";
            zawartoscPliku += "\n" + sekwencjaSynchro;
            var backingFile = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), nazwaPliku);

            foreach (Patrol p in danePatroli)
            {
                zawartoscPliku += "\n" + p.nazwaPatrolu;
                if (p.czyGraTrwa)
                {
                    zawartoscPliku += "\n1";
                }
                else
                {
                    zawartoscPliku += "\n0";
                }
                zawartoscPliku += "\n" + p.minutaStartowa.ToString(formatData, provider);
                zawartoscPliku += "\n" + p.czasRozpoczecia.ToString(formatData, provider);
                zawartoscPliku += "\n" + p.minutaZakonczenia.ToString(formatData, provider);
                zawartoscPliku += "\n" + p.czasZakonczenia.ToString(formatData, provider);
                zawartoscPliku += "\n" + p.karne;
                zawartoscPliku += "\n" + p.calkowityCzas.ToString();

                foreach (string kod in p.kodyLampionow)
                {
                    zawartoscPliku += "\n" + kod;
                }

                zawartoscPliku += "\n" + sekwencjaSynchro;
            }

            using (var writer = File.CreateText(backingFile))
            {
                writer.Write(zawartoscPliku);
            }

            return zawartoscPliku;
        }


        //Funkcja wczytuje grę z pliku tekstowego o nazwie 'nazwaPliku'
        //WERSJA TEKSTOWA
        public static string ReadGame()
        {
            var backingFile = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), nazwaPliku);
            string zawartoscPliku = "";

            if (backingFile == null || !File.Exists(backingFile))
            {
                return null;
            }

            foreach (Patrol p in danePatroli)
                p.kodyLampionow.Clear();
            danePatroli.Clear();

            using (var reader = new StreamReader(backingFile, true))
            {
                string line;
                int i = 0;

                //wcztanie zmiennej czyWynikiTrwaja
                line = reader.ReadLine();

                if(line != null)
                {
                    if (line == "1")
                    {
                        czyWynikiTrwaja = true;
                        zawartoscPliku += "1";
                    }
                    else
                    {
                        czyWynikiTrwaja = false;
                        zawartoscPliku += "0";
                    }
                }
                //Wczytanie sekwencji synchronizacji
                line = reader.ReadLine();

                //Czy sekwencja synchronizacji istnieje?
                while (line != null)
                {
                    zawartoscPliku += line;

                    if ((line = reader.ReadLine()) != null)
                    {
                        danePatroli.Add(new Patrol());
                        danePatroli[i].nazwaPatrolu = line;
                        zawartoscPliku += line;

                        if ((line = reader.ReadLine()) != null)
                        {
                            if (line == "1") danePatroli[i].czyGraTrwa = true;
                            else danePatroli[i].czyGraTrwa = false;

                            zawartoscPliku += "\n" + line;
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].minutaStartowa = DateTime.ParseExact(line, formatData, provider);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].czasRozpoczecia = DateTime.ParseExact(line, formatData, provider);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].minutaZakonczenia = DateTime.ParseExact(line, formatData, provider);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].czasZakonczenia = DateTime.ParseExact(line, formatData, provider);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].karne = Int32.Parse(line);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].calkowityCzas = TimeSpan.Parse(line);
                        }

                        danePatroli[i].kodyLampionow.Clear();
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line != sekwencjaSynchro)
                            {
                                zawartoscPliku += "\n" + line;
                                danePatroli[i].kodyLampionow.Add(line);
                            }
                            else break;
                        }
                        i++;
                    }
                }
            }
            return zawartoscPliku;
        }

        static private void zamienPatrole(int i, int j)
        {
            Patrol temp = danePatroli[i];
            danePatroli[i] = danePatroli[j];
            danePatroli[j] = temp;
        }

        static public void sortujPatrole()
        {
            int liczbaPatroli = danePatroli.Count;
            int najlepszy;
            int minKarne;
            TimeSpan minCzas;

            for (int i = 0; i < liczbaPatroli; i++)
            {
                najlepszy = i;
                minKarne = danePatroli[i].karne;
                minCzas = danePatroli[i].calkowityCzas;

                for (int j = i+1; j < liczbaPatroli; j++)
                {
                    if (danePatroli[najlepszy].karne > danePatroli[j].karne)
                    {
                        najlepszy = j;
                    }
                    else if (danePatroli[najlepszy].karne == danePatroli[j].karne)
                    {
                        if (danePatroli[najlepszy].calkowityCzas > danePatroli[j].calkowityCzas)
                        {
                            najlepszy = j;
                        }
                    }
                }

                if (najlepszy != i)
                {
                    zamienPatrole(najlepszy, i);
                }
            }
        }

        static public string ranking()
        {
            //Do rozpoznawania formatu daty
            CultureInfo provider = CultureInfo.InvariantCulture;
            string formatData = "dd.MM.yyyy HH:mm:ss";
            string formatGodzina = "HH:mm:ss";

            sortujPatrole();
            string text = "RANKING\n";
            int i = 1;
            foreach (Patrol p in danePatroli)
            {
                text += i + ". " + p.nazwaPatrolu + " - " + p.karne + "p. karne - czas: " + (DateTime.MinValue + p.calkowityCzas).ToString(formatGodzina, provider) + "\n";
                i++;
            }
            return text;
        }

        static public string drukujPatrole()
        {
            string text = "SZCZEGÓŁY\n\n";
            int i = 1;
            foreach (Patrol p in danePatroli)
            {
                text += i + ". miejsce\n" + p.drukuj() + "\n\n";
                i++;
            }
            return text;
        }


        public static void wyslijExcel(ref IWorksheet worksheet)
        {
            int licznik_rzad = 1;
            int licznik_lamp;

            worksheet.Range[licznik_rzad, 1].Text = "Miejsce";
            worksheet.Range[licznik_rzad, 2].Text = "Nazwa";
            worksheet.Range[licznik_rzad, 3].Text = "Minuta startowa";
            worksheet.Range[licznik_rzad, 4].Text = "Czas rozpoczęcia";
            worksheet.Range[licznik_rzad, 5].Text = "Minuta zakończenia";
            worksheet.Range[licznik_rzad, 6].Text = "Czas zakończenia";
            worksheet.Range[licznik_rzad, 7].Text = "Całkowity czas przejścia";
            worksheet.Range[licznik_rzad, 8].Text = "Stan gry";
            worksheet.Range[licznik_rzad, 9].Text = "Punkty karne";
            worksheet.Range[licznik_rzad, 11].Text = "Spisane lampiony:";

            licznik_rzad++;

            foreach (Patrol p in danePatroli)
            {
                worksheet.Range[licznik_rzad, 1].Text = (licznik_rzad - 1).ToString();
                worksheet.Range[licznik_rzad, 2].Text = p.nazwaPatrolu;
                worksheet.Range[licznik_rzad, 3].Text = p.minutaStartowa.ToString(formatData, provider);
                worksheet.Range[licznik_rzad, 4].Text = p.czasRozpoczecia.ToString(formatData, provider);
                worksheet.Range[licznik_rzad, 5].Text = p.minutaZakonczenia.ToString(formatData, provider);
                worksheet.Range[licznik_rzad, 6].Text = p.czasZakonczenia.ToString(formatData, provider);
                worksheet.Range[licznik_rzad, 7].Text = (DateTime.MinValue + p.calkowityCzas).ToString(formatData, provider);
                worksheet.Range[licznik_rzad, 8].Text = p.czyGraTrwa.ToString();
                worksheet.Range[licznik_rzad, 9].Text = p.karne.ToString();

                licznik_lamp = 11;
                foreach (string kod in p.kodyLampionow)
                {
                    worksheet.Range[licznik_rzad , licznik_lamp].Text = kod;
                    licznik_lamp++;
                }

                licznik_rzad++;
            }

        }

    }
}