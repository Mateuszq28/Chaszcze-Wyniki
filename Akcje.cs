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

using Android.Graphics;
using System.IO;
using System.Threading.Tasks;

namespace Chaszcze_Wyniki
{
    [Activity(Label = "Akcje")]
    public class Akcje : Activity
    {
        //Przycisk dodający patrol i powracający do menu
        static private Button dodajPatrol, powrot;

        public static TextView ranking, podsumowanie;


        //Funkcja wywołująca zapisywanie w kluczowych momentach (np przed zabiciem obiektu klasy Akcje)
        protected override void OnSaveInstanceState(Bundle outState)
        {
            Zarzadzanie.SaveGame();
            base.OnSaveInstanceState(outState);
        }


        //Wyświetla podsumowanie na koniec
        public static void wyswietlPodsumowanie()
        {
            ranking.Text = Zarzadzanie.ranking();
            podsumowanie.Text = Zarzadzanie.drukujPatrole();
        }


        //Metoda wywołuje się w momencie tworzenia obiektu
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.akcje_);

            //Wczytaj dane
            Zarzadzanie.ReadGame();

            //Przypisz elementy interfejsu do zmiennych roboczych
            dodajPatrol = FindViewById<Button>(Resource.Id.btndodaj);
            powrot = FindViewById<Button>(Resource.Id.btnwroc);
           
            ranking = FindViewById<TextView>(Resource.Id.ranking);
            podsumowanie = FindViewById<TextView>(Resource.Id.listap);

          
            //Dodanie funkcji do przycisku Zakoncz gre/Powrot do menu
            powrot.Click += (sender, e) =>
            {
                Zarzadzanie.czyWynikiTrwaja = false;
                Zarzadzanie.SaveGame();
                var intent = new Intent(this, typeof(Poczatek));
                StartActivity(intent);
                this.Finish();
            };


            //Dodanie funkcji do przycisku Zakoncz gre/Powrot do menu
            dodajPatrol.Click += (sender, e) =>
            {
                Zarzadzanie.czyWynikiTrwaja = true;
                var intent = new Intent(this, typeof(QRakcja));
                StartActivity(intent);
            };


            //Wyswietl wyniki
            wyswietlPodsumowanie();
            
        }
    }
}