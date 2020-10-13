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


namespace Chaszcze_Wyniki
{
    [Activity(Label = "QR akcja")]
    public class QRakcja : AppCompatActivity, IPermissionListener
    {
        private ZXingScannerView scannerView;


        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            this.Finish();
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            scannerView.SetResultHandler(new ScanResultHandler(this));
            scannerView.StartCamera();
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
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

            public ScanResultHandler(QRakcja qrakcja)
            {
                this.qrakcja = qrakcja;
            }

            public void HandleResult(ZXing.Result rawResult)
            {
                String dodany = rawResult.ToString();
                //Toast.MakeText(qrakcja, dodany, ToastLength.Long).Show();

                //Dodaj kod do bazy i zapisz grę
                Zarzadzanie.danePatroli.Add(new Patrol(dodany));
                Zarzadzanie.SaveGame();
                Akcje.wyswietlPodsumowanie();
                qrakcja.Finish();
            }
        }
    }

}