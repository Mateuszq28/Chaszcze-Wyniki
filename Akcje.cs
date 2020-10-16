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

//Generowanie excela
using Syncfusion.XlsIO;

namespace Chaszcze_Wyniki
{
    [Activity(Label = "Akcje")]
    public class Akcje : Activity
    {
        //Przycisk dodający patrol i powracający do menu
        static private Button dodajPatrol, powrot, excel_gen;

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


        //Generuj Excel
        void generuj_excel(object sender, EventArgs e)
        {
            //Create an instance of ExcelEngine.
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Set the default application version as Excel 2013.
                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;

                //Create a workbook with a worksheet
                IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);

                //Access first worksheet from the workbook instance.
                IWorksheet worksheet = workbook.Worksheets[0];

                //Adding text
                Zarzadzanie.wyslijExcel(ref worksheet);

                //Save the workbook to stream in xlsx format. 
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);

                workbook.Close();

                //Save the stream as a file in the device and invoke it for viewing
                SaveAndroid androidSave = new SaveAndroid();
                //androidSave.SaveAndView("CreateExcel.xlsx", "application/msexcel", stream, this);
                androidSave.JustSave("CreateExcel.xlsx", "application/msexcel", stream, this);
            }
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
            excel_gen = FindViewById<Button>(Resource.Id.btn_excel);

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


            //Dodanie funkcji do przycisku generuj excel
            excel_gen.Click += generuj_excel;


            //Wyswietl wyniki
            wyswietlPodsumowanie();
            
        }
    }
}