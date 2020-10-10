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
using Android.Support.V7.App;
using ZXing;
using EDMTDev.ZXingXamarinAndroid;
using Com.Karumi.Dexter;
using Android;
using Com.Karumi.Dexter.Listener.Single;
using Com.Karumi.Dexter.Listener;


namespace Chaszcze
{
    [Activity(Label = "QR akcja")]
    public class QRakcja : AppCompatActivity, IPermissionListener
    {
        private ZXingScannerView scannerView;
        private string nrPunktu;


        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            this.Finish();
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            scannerView.SetResultHandler(new ScanResultHandler(this, nrPunktu));
            scannerView.StartCamera();
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            nrPunktu = Intent.GetStringExtra("nrPunktu");
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.qrakcja_);

            // Create your application here
            scannerView = FindViewById<ZXingScannerView>(Resource.Id.zxcan);

            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.Camera)
                .WithListener(this)
                .Check();
        }

        private class ScanResultHandler : IResultHandler
        {
            private QRakcja qrakcja;
            private string nrPunktu;

            public ScanResultHandler(QRakcja qrakcja, string nrPunktu)
            {
                this.qrakcja = qrakcja;
                this.nrPunktu = nrPunktu;
            }

            public void HandleResult(ZXing.Result rawResult)
            {
                //Wywal komunikacik co zeskanowało
                Toast.MakeText(qrakcja, "PK " + qrakcja.nrPunktu + " " + rawResult.ToString(), ToastLength.Long).Show();

                String dodany = qrakcja.nrPunktu + "-" + rawResult.ToString();
                //Toast.MakeText(qrakcja, dodany, ToastLength.Long).Show();

                //Zmień kolor przycisku
                if (Zarzadzanie.kodyLampionow.Find(x => x.StartsWith(nrPunktu + "-")) == null)
                    Akcje.zmienKolor(nrPunktu, "green");
                else Akcje.zmienKolor(nrPunktu, "yellow");

                //Dodaj kod do bazy i zapisz grę
                Zarzadzanie.kodyLampionow.Add(dodany);
                Zarzadzanie.SaveGeme();
                qrakcja.Finish();
            }
        }
    }

}