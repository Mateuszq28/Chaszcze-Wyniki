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

//Dodano na potrzeby generowania zdjęcia z kodem QR
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using ZXing;
using ZXing.Common;

namespace Chaszcze
{
    [Activity(Label = "Akcje")]
    public class Akcje : Activity
    {
        //Przyciski odpowiadające polom na kody z lampionów 1-12
        static public Button pk1, pk2, pk3, pk4, pk5, pk6, pk7, pk8, pk9, pk10, pk11, pk12;
        //Przycisk kończący grę, a później pozawalający wrócić do menu [id = button13]
        static private Button zakoncz;


        //Funkcja wywołująca zapisywanie w kluczowych momentach (np przed zabiciem obiektu klasy Akcje)
        protected override void OnSaveInstanceState(Bundle outState)
        {
            Zarzadzanie.SaveGeme();

            //Ponieżej zapisywanie do zmiennych w programie, ale nie działa to po zamknięciu apki (zabicie procesu w Androidzie)
            //
            //outState.PutString("nazwaPatrolu", Zarzadzanie.nazwaPatrolu);
            //outState.PutBoolean("czyGraTrwa", Zarzadzanie.czyGraTrwa);
            //Log.Debug(GetType().FullName, "Zarzadzanie/Akcje - Saving instance state");

            // always call the base implementation!
            base.OnSaveInstanceState(outState);
        }

        /*Ponieżej oczytywanie z zmiennych w programie, ale nie działa to po zamknięciu apki
        //Fragment zostawiono w celach edukacyjncyh, powinien znajdować się w funkcji OnCreate
        //
        if (savedInstanceState != null)
        {
            Zarzadzanie.nazwaPatrolu = savedInstanceState.GetString("nazwaPatrolu");
            Zarzadzanie.czyGraTrwa = savedInstanceState.GetBoolean("czyGraTrwa");
            Log.Debug(GetType().FullName, "Zarzadzanie/Akcje - Recovered instance state");
        }*/


        //Działanie przycisków wywołujących kod QR
        private void wywolajQR(string nr)
        {
            if (Zarzadzanie.czyGraTrwa)
            {
                var intent = new Intent(this, typeof(QRakcja));
                intent.PutExtra("nrPunktu", nr);
                StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "Gra się już zakończyła!", ToastLength.Long).Show();
            }
        }


        //Generuje obrazek z kodem QR
        private void generujQR(string message, ImageView image)
        {
            string CodeType = "QR Code";
            int size = 660;
            int small_size = 264;

            string[] PERMISSIONS =
            {
                "android.permission.READ_EXTERNAL_STORAGE",
                "android.permission.WRITE_EXTERNAL_STORAGE"
            };

            var permission = ContextCompat.CheckSelfPermission(this, "android.permission.WRITE_EXTERNAL_STORAGE");
            var permissionread = ContextCompat.CheckSelfPermission(this, "android.permission.READ_EXTERNAL_STORAGE");

            if (permission != Permission.Granted || permissionread != Permission.Granted)
                ActivityCompat.RequestPermissions(this, PERMISSIONS, 1);

            try
            {
                if (permission == Permission.Granted && permissionread == Permission.Granted)
                {
                    BitMatrix bitmapMatrix = null;

                    switch (CodeType)
                    {
                        case "QR Code":
                            bitmapMatrix = new MultiFormatWriter().encode(message, BarcodeFormat.QR_CODE, size, size);
                            break;
                        case "PDF 417":
                            bitmapMatrix = new MultiFormatWriter().encode(message, BarcodeFormat.PDF_417, size, small_size);
                            break;
                        case "CODE 128":
                            bitmapMatrix = new MultiFormatWriter().encode(message, BarcodeFormat.CODE_128, size, small_size);
                            break;
                        case "CODE 39":
                            bitmapMatrix = new MultiFormatWriter().encode(message, BarcodeFormat.CODE_39, size, small_size);
                            break;
                        case "AZTEC":
                            bitmapMatrix = new MultiFormatWriter().encode(message, BarcodeFormat.AZTEC, size, small_size);
                            break;
                    }

                    var width = bitmapMatrix.Width;
                    var height = bitmapMatrix.Height;
                    int[] pixelsImage = new int[width * height];

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (bitmapMatrix[j, i])
                                pixelsImage[i * width + j] = (int)Convert.ToInt64(0xff000000);
                            else
                                pixelsImage[i * width + j] = (int)Convert.ToInt64(0xffffffff);

                        }
                    }

                    Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                    bitmap.SetPixels(pixelsImage, 0, width, 0, 0, width, height);

                    var sdpath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    var path = System.IO.Path.Combine(sdpath, "logeshbarcode.jpg");
                    var stream = new FileStream(path, FileMode.Create);
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    stream.Close();

                    image.SetImageBitmap(bitmap);
                    image.Visibility = Android.Views.ViewStates.Visible;
                }
                else
                {
                    Console.WriteLine("No Permission");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex} ");
            }
        }


        //Wyświetla podsumowanie na koniec gry
        private void wyswietlPodsumowanie(TextView pole)
        {
            //Wyswietl info zwrotne
            string infoZwrotne = "Nazwa patrolu: " + Zarzadzanie.nazwaPatrolu;
            infoZwrotne += "\nPunkty karne: " + Zarzadzanie.karne;
            infoZwrotne += "\nCzas rozpoczęcia: " + Zarzadzanie.czasRozpoczecia.ToString("dd.MM.yyyy HH:mm");
            infoZwrotne += "\nMinuta startowa: " + Zarzadzanie.minutaStartowa.ToString("HH:mm");
            infoZwrotne += "\nCzas zakończenia: " + Zarzadzanie.czasZakonczenia.ToString("dd.MM.yyyy HH:mm");
            infoZwrotne += "\nMinuta zakończenia: " + Zarzadzanie.minutaZakonczenia.ToString("HH:mm");
            infoZwrotne += "\nCałkowity czas przejścia: " + (DateTime.MinValue + Zarzadzanie.calkowityCzas).ToString("HH:mm");
            infoZwrotne += "\n\nLegenda:";
            infoZwrotne += "\nzielony - prawidłowy Punkt Kontrolny (0)";
            infoZwrotne += "\npomarańczowy - Punkt Stowarzyszony (25)";
            infoZwrotne += "\nczarny - brak Punktu Kontrolnego (90)";
            infoZwrotne += "\nczerwony - Punkt Mylny lub o innym numerze (90+60)\n";
            pole.Text = infoZwrotne;
        }


        //Metoda wywołuje się w momencie tworzenia obiektu
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.akcje_);


            //Wczytaj dane
            Zarzadzanie.ReadGame();

            //Przypisz elementy interfejsu do zmiennych roboczych
            pk1 = FindViewById<Button>(Resource.Id.button1);
            pk2 = FindViewById<Button>(Resource.Id.button2);
            pk3 = FindViewById<Button>(Resource.Id.button3);
            pk4 = FindViewById<Button>(Resource.Id.button4);
            pk5 = FindViewById<Button>(Resource.Id.button5);
            pk6 = FindViewById<Button>(Resource.Id.button6);
            pk7 = FindViewById<Button>(Resource.Id.button7);
            pk8 = FindViewById<Button>(Resource.Id.button8);
            pk9 = FindViewById<Button>(Resource.Id.button9);
            pk10 = FindViewById<Button>(Resource.Id.button10);
            pk11 = FindViewById<Button>(Resource.Id.button11);
            pk12 = FindViewById<Button>(Resource.Id.button12);
            zakoncz = FindViewById<Button>(Resource.Id.button13);
            TextView textGodz = FindViewById<TextView>(Resource.Id.textView1);
            TextView podsumowanie = FindViewById<TextView>(Resource.Id.podsumowanie);
            ImageView obrazek = FindViewById<ImageView>(Resource.Id.imageView1);

            //Ustaw nagłówek karty patrolu
            textGodz.Text = Zarzadzanie.nazwaPatrolu + "\nGodzina startu: " + Zarzadzanie.czasRozpoczecia.ToString("HH:mm") + " (" + Zarzadzanie.minutaStartowa.ToString("HH:mm") + ")";
            //Ustaw Legendę na dole ekranu
            string zasady = "Legenda:\nzielony - zebrany Punkt Kontrolny\nżółty - poprawiony Punkt Kontrolny (10 punktów karnych za każdną poprawkę)";
            zasady += "\n\nZasady:";
            zasady += "\nprawidłowy Punkt Kontrolny - 0 punktów karnych";
            zasady += "\nPunkt Stowarzyszony - 25 punktów karnych";
            zasady += "\nbrak Punktu Kontrolnego - 90 punktów karnych";
            zasady += "\nPunkt Mylny lub o innym numerze - 90+60 punktów karnych\n";
            podsumowanie.Text = zasady;
            //Ustaw obrazek
            obrazek.SetImageResource(Resource.Drawable.keh_logo);
            obrazek.Visibility = Android.Views.ViewStates.Gone;

            //Dodanie funkcji do przycisków do skanerów kodów QR
            pk1.Click += (sender, e) =>
            {
                string nr = "1";
                wywolajQR(nr);
            };
            pk2.Click += (sender, e) =>
            {
                string nr = "2";
                wywolajQR(nr);
            };
            pk3.Click += (sender, e) =>
            {
                string nr = "3";
                wywolajQR(nr);
            };
            pk4.Click += (sender, e) =>
            {
                string nr = "4";
                wywolajQR(nr);
            };
            pk5.Click += (sender, e) =>
            {
                string nr = "5";
                wywolajQR(nr);
            };
            pk6.Click += (sender, e) =>
            {
                string nr = "6";
                wywolajQR(nr);
            };
            pk7.Click += (sender, e) =>
            {
                string nr = "7";
                wywolajQR(nr);
            };
            pk8.Click += (sender, e) =>
            {
                string nr = "8";
                wywolajQR(nr);
            };
            pk9.Click += (sender, e) =>
            {
                string nr = "9";
                wywolajQR(nr);
            };
            pk10.Click += (sender, e) =>
            {
                string nr = "10";
                wywolajQR(nr);
            };
            pk11.Click += (sender, e) =>
            {
                string nr = "11";
                wywolajQR(nr);
            };
            pk12.Click += (sender, e) =>
            {
                string nr = "12";
                wywolajQR(nr);
            };


            //Dodanie funkcji do przycisku Zakoncz gre/Powrot do menu
            zakoncz.Click += (sender, e) =>
            {
                if (Zarzadzanie.czyGraTrwa)
                {
                    //Okno dialogowe potwierdzające zakończenie gry
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Uwaga!");
                    alert.SetMessage("Czy na pewno chcesz zakończyć grę?");
                    alert.SetButton("TAK", (c, ev) =>
                    {
                        string zapisGry;
                        //Policz i ustaw falgi
                        zapisGry = Zarzadzanie.zakonczenie();
                        //Wyswietl wyniki
                        wyswietlPodsumowanie(podsumowanie);
                        //Zmien funkcjonalnosc przycisku
                        zakoncz.Text = "Powrót do menu";
                        //Generuj i ustaw obrazek z kodem QR
                        generujQR(zapisGry, obrazek);
                    });
                    alert.SetButton2("ANULUJ", (c, ev) => { });
                    alert.Show();
                }
                else
                {
                    var intent = new Intent(this, typeof(Poczatek));
                    StartActivity(intent);
                    this.Finish();
                }
            };


            //Pokoloruj kartę odpowiedzi
            Zarzadzanie.ustawKolory();
            if (Zarzadzanie.czyGraTrwa == false)
            {
                //Wyswietl wyniki
                wyswietlPodsumowanie(podsumowanie);
                //Zmien funkcjonalnosc przycisku
                zakoncz.Text = "Powrót do menu";
                //Generuj i ustaw obrazek z kodem QR
                generujQR(Zarzadzanie.ReadGame(), obrazek);
            }
        }


        //Zmienia kolor przycisku o numerze nr (1-12) w tablicy odpwowiedzi
        //option to nazwa koloru w języku angielskim
        static public void zmienKolor(string nr, string option)
        {
            Button pk;
            switch (nr)
            {
                case "1":
                    pk = pk1;
                    break;
                case "2":
                    pk = pk2;
                    break;
                case "3":
                    pk = pk3;
                    break;
                case "4":
                    pk = pk4;
                    break;
                case "5":
                    pk = pk5;
                    break;
                case "6":
                    pk = pk6;
                    break;
                case "7":
                    pk = pk7;
                    break;
                case "8":
                    pk = pk8;
                    break;
                case "9":
                    pk = pk9;
                    break;
                case "10":
                    pk = pk10;
                    break;
                case "11":
                    pk = pk11;
                    break;
                case "12":
                    pk = pk12;
                    break;
                default:
                    pk = pk1;
                    break;
            }

            switch (option)
            {
                case "red":
                    pk.SetBackgroundColor(Color.Red);
                    break;
                case "green":
                    pk.SetBackgroundColor(Color.Green);
                    break;
                case "yellow":
                    pk.SetBackgroundColor(Color.Yellow);
                    break;
                case "orange":
                    pk.SetBackgroundColor(Color.Orange);
                    break;
                case "black":
                    pk.SetBackgroundColor(Color.Black);
                    pk.SetTextColor(Color.White);
                    break;
                case "white":
                    pk.SetBackgroundColor(Color.White);
                    break;
                case "light_gray":
                    pk.SetBackgroundColor(Color.LightGray);
                    break;
                default:
                    pk.SetBackgroundColor(Color.LightGray);
                    break;
            }
        }
    }
}