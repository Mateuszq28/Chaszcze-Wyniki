using System;
using System.IO;
using Android.Content;
using Java.IO;
using System.Threading.Tasks;
//using GettingstartedAndroid;
using Android.Support.V7.App;

namespace Chaszcze_Wyniki
{
    class SaveAndroid
    {
        //Method to save document as a file in Android and view the saved document
        public void SaveAndView(string fileName, String contentType, MemoryStream stream, Akcje activity)
        {
            string root = null;
            //Get the root path in android device.
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.ToString();
            }
            else
                root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //Create directory and file 
            Java.IO.File myDir = new Java.IO.File(root + "/Syncfusion");
            myDir.Mkdir();

            Java.IO.File file = new Java.IO.File(myDir, fileName);

            //Remove if the file exists
            if (file.Exists()) file.Delete();

            //Write the stream into the file
            FileOutputStream outs = new FileOutputStream(file);
            outs.Write(stream.ToArray());

            outs.Flush();
            outs.Close();

            //Invoke the created file for viewing
            if (file.Exists())
            {
                Android.Net.Uri path = Android.Net.Uri.FromFile(file);
                string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
                Intent intent = new Intent(Intent.ActionView);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.SetDataAndType(path, mimeType);
                //intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
                activity.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            }
        }


        //Tylko zapis
        public void JustSave(string fileName, String contentType, MemoryStream stream, Akcje activity)
        {
            var sdpath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var path = System.IO.Path.Combine(sdpath, "wyniki_chaszczy.xls");
            var file = path;

            //Write the stream into the file
            FileOutputStream outs = new FileOutputStream(file);
            outs.Write(stream.ToArray());

            outs.Flush();
            outs.Close();
        }


    }
}


