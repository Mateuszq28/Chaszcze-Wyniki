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
                zawartoscPliku += "\n" + p.minutaStartowa.ToString();
                zawartoscPliku += "\n" + p.czasRozpoczecia.ToString();
                zawartoscPliku += "\n" + p.minutaZakonczenia.ToString();
                zawartoscPliku += "\n" + p.czasZakonczenia.ToString();
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
                            danePatroli[i].minutaStartowa = DateTime.Parse(line);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].czasRozpoczecia = DateTime.Parse(line);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].minutaZakonczenia = DateTime.Parse(line);
                        }
                        if ((line = reader.ReadLine()) != null)
                        {
                            zawartoscPliku += "\n" + line;
                            danePatroli[i].czasZakonczenia = DateTime.Parse(line);
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
            sortujPatrole();
            string text = "RANKING\n";
            int i = 1;
            foreach (Patrol p in danePatroli)
            {
                text += i + ". " + p.nazwaPatrolu + " - " + p.karne + "p. karne - czas: " + p.calkowityCzas + "\n";
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
    }
}