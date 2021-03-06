﻿using System;
using System.Collections.Generic;
using System.IO;

//do formatowania dat
using System.Globalization;

namespace Chaszcze_Wyniki
{
    class Patrol
    {
        //Zmienne podawane na początku gry
        public string nazwaPatrolu;
        public DateTime minutaStartowa;
        public DateTime czasRozpoczecia;

        //Ważne podczas gry
        public List<string> kodyLampionow;
        public bool czyGraTrwa = false;

        //Liczone dopiero na koniec gry
        public DateTime minutaZakonczenia;
        public DateTime czasZakonczenia;
        public TimeSpan calkowityCzas;
        public int karne;

        public Patrol()
        {
            kodyLampionow = new List<string>();

            //Zmienne podawane na początku gry
            nazwaPatrolu = "";
            minutaStartowa = DateTime.MinValue;
            czasRozpoczecia = DateTime.MinValue;

            //Ważne podczas gry
            kodyLampionow = new List<string>();
            czyGraTrwa = false;

            //Liczone dopiero na koniec gry
            minutaZakonczenia = DateTime.MinValue;
            czasZakonczenia = DateTime.MinValue;
            calkowityCzas = TimeSpan.MinValue;
            karne = 0;
        }

        public Patrol(String dodany)
        {
            //Do rozpoznawania formatu daty
            CultureInfo provider = CultureInfo.InvariantCulture;
            string formatData = "dd.MM.yyyy HH:mm:ss";
            string formatGodzina = "HH:mm";

            kodyLampionow = new List<string>();
            StringReader reader = new StringReader(dodany);
            string line;
            string zawartoscPliku = "";
            if ((line = reader.ReadLine()) != null)
            {
                nazwaPatrolu = line;
                zawartoscPliku = line;
            }
            if ((line = reader.ReadLine()) != null)
            {
                if (line == "1") czyGraTrwa = true;
                else czyGraTrwa = false;

                zawartoscPliku += "\n" + line;
            }
            if ((line = reader.ReadLine()) != null)
            {
                zawartoscPliku += "\n" + line;
                minutaStartowa = DateTime.ParseExact(line, formatData, provider);
            }
            if ((line = reader.ReadLine()) != null)
            {
                zawartoscPliku += "\n" + line;
                czasRozpoczecia = DateTime.ParseExact(line, formatData, provider);
            }
            if ((line = reader.ReadLine()) != null)
            {
                zawartoscPliku += "\n" + line;
                minutaZakonczenia = DateTime.ParseExact(line, formatData, provider);
            }
            if ((line = reader.ReadLine()) != null)
            {
                zawartoscPliku += "\n" + line;
                czasZakonczenia = DateTime.ParseExact(line, formatData, provider);
            }
            if ((line = reader.ReadLine()) != null)
            {
                zawartoscPliku += "\n" + line;
                karne = Int32.Parse(line);
            }
            if ((line = reader.ReadLine()) != null)
            {
                zawartoscPliku += "\n" + line;
                calkowityCzas = TimeSpan.Parse(line);
            }

            kodyLampionow.Clear();
            while ((line = reader.ReadLine()) != null)
            {
                zawartoscPliku += "\n" + line;
                kodyLampionow.Add(line);
            }
        }


        public string drukuj()
        {
            //Do rozpoznawania formatu daty
            CultureInfo provider = CultureInfo.InvariantCulture;
            string formatData = "dd.MM.yyyy HH:mm:ss";
            string formatGodzina = "HH:mm:ss";

            string text = "";

            text += "Nazwa: " + nazwaPatrolu;
            text += "\nMinuta startowa: " + minutaStartowa.ToString(formatData, provider);
            text += "\nCzas rozpoczęcia: " + czasRozpoczecia.ToString(formatData, provider);
            text += "\nMinuta zakończenia: " + minutaZakonczenia.ToString(formatData, provider);
            text += "\nCzas zakończenia: " + czasZakonczenia.ToString(formatData, provider);
            text += "\nCałkowity czas przejścia: " + (DateTime.MinValue + calkowityCzas).ToString(formatGodzina, provider);
            if (czyGraTrwa) text += "\nGRA DALEJ TRWA";
            else text += "\nGRA ZAKOŃCZONA";
            text += "\nPunkty karne: " + karne;
            text += "\nSpisane lampiony:";

            foreach (string l in kodyLampionow)
            {
                text += "\n" + l;
            }

            return text;
        }
    }
}