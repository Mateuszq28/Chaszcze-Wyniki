using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;

namespace Chaszcze_Wyniki
{
    //[Activity(Label = "Chaszcze Wyniki", Theme = "@style/AppTheme", MainLauncher = true)]
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class Poczatek : AppCompatActivity
    {

        //Metoda wywoływana podczas tworzenia obiektu
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.poczatek_);


            //Sprawdź pozwolenia
            string[] PERMISSIONS =
            {
                "android.permission.READ_EXTERNAL_STORAGE",
                "android.permission.WRITE_EXTERNAL_STORAGE",
                "android.permission.CAMERA"
            };

            var permission = ContextCompat.CheckSelfPermission(this, "android.permission.WRITE_EXTERNAL_STORAGE");
            var permissionread = ContextCompat.CheckSelfPermission(this, "android.permission.READ_EXTERNAL_STORAGE");
            var permissioncamera = ContextCompat.CheckSelfPermission(this, "android.permission.CAMERA");

            if (permission != Permission.Granted || permissionread != Permission.Granted)
                ActivityCompat.RequestPermissions(this, PERMISSIONS, 1);

            if (permissioncamera != Permission.Granted)
                ActivityCompat.RequestPermissions(this, PERMISSIONS, 2);


            //Przypisz elementy interfejsu do zmiennych
            Button NowaGra = FindViewById<Button>(Resource.Id.button1);
            Button Wczytaj = FindViewById<Button>(Resource.Id.button2);


            //Przypisz przyciskom funkcje
            NowaGra.Click += (sender, e) =>
            {
                if (permission != Permission.Granted || permissionread != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, PERMISSIONS, 1);
                    var intent = new Intent(this, typeof(Poczatek));
                    Toast.MakeText(this, "Czy na pewno zaakceptowałeś zgody?", ToastLength.Long).Show();
                    StartActivity(intent);
                    this.Finish();
                }
                else
                {
                    if (permissioncamera != Permission.Granted)
                    {
                        ActivityCompat.RequestPermissions(this, PERMISSIONS, 2);
                        var intent = new Intent(this, typeof(Poczatek));
                        Toast.MakeText(this, "Czy na pewno zaakceptowałeś zgody?", ToastLength.Long).Show();
                        StartActivity(intent);
                        this.Finish();
                    }
                    else
                    {
                        //Okno dialogowe potwierdzające zbieranie nowych wyników
                        Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert = dialog.Create();
                        alert.SetTitle("Uwaga!");
                        alert.SetMessage("Czy na pewno chcesz rozpocząć zbieranie wyników od nowa?");
                        alert.SetButton("TAK", (c, ev) =>
                        {
                            Zarzadzanie.reset();
                            Zarzadzanie.czyWynikiTrwaja = true;
                            Zarzadzanie.SaveGame();
                            var intent = new Intent(this, typeof(Akcje));
                            StartActivity(intent);
                            this.Finish();
                        });
                        alert.SetButton2("ANULUJ", (c, ev) => { });
                        alert.Show();
                    }
                }
            };


            Wczytaj.Click += (sender, e) =>
            {
                if (permission != Permission.Granted || permissionread != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, PERMISSIONS, 1);
                    var intent = new Intent(this, typeof(Poczatek));
                    Toast.MakeText(this, "Czy na pewno zaakceptowałeś zgody?", ToastLength.Long).Show();
                    StartActivity(intent);
                    this.Finish();
                }
                else
                {
                    if (permissioncamera != Permission.Granted)
                    {
                        ActivityCompat.RequestPermissions(this, PERMISSIONS, 2);
                        var intent = new Intent(this, typeof(Poczatek));
                        Toast.MakeText(this, "Czy na pewno zaakceptowałeś zgody?", ToastLength.Long).Show();
                        StartActivity(intent);
                        this.Finish();
                    }
                    else
                    {
                        var intent = new Intent(this, typeof(Akcje));
                        StartActivity(intent);
                        this.Finish();
                    }
                }
            };


            if (permission != Permission.Granted || permissionread != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, PERMISSIONS, 1);
            }
            else
            {
                if (permissioncamera != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, PERMISSIONS, 2);
                }
                else
                {
                    Zarzadzanie.ReadGame();
                    if (Zarzadzanie.czyWynikiTrwaja)
                    {
                        var intent = new Intent(this, typeof(Akcje));
                        StartActivity(intent);
                        this.Finish();
                    }
                }
            }
            
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}