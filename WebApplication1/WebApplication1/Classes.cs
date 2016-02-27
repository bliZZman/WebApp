using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebApplication1
{
    public class Image
    {
        private string name { get; set; } //імя картинки
        private DateTime timeOfUpload { get; set; } //час завантаження на сервер

        private string pathToTxtFile = HttpContext.Current.Server.MapPath("~/data.txt"); // шлях до файлу, в якому містяться всі дані 
        public string PathToTxtFile { get { return pathToTxtFile; } }

        private string pathToImage = HttpContext.Current.Server.MapPath("~/images/"); // шлях до папки з картинками
        public string PathToImage { get { return pathToImage; } }
    }

    public class Load : Image
    { 
        public void ShowUploadDates()
        {
            string[] files = Directory.GetFiles(PathToImage, "*.jpg");
            HttpContext.Current.Session["files"] = files;
            Dictionary<string, DateTime> resultArray = new Dictionary<string, DateTime>();
            StreamReader reader = new StreamReader(PathToTxtFile);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] tokens = line.Split('#');
                string fileName = tokens[0];
                DateTime time = DateTime.Parse(tokens[1]);
                resultArray[fileName] = time;
            }
            reader.Close();
            reader.Dispose();
        }

        string[] CreateForm(int filesCount, int QuantityOfPictures)
        {

            string[] result = new string[(filesCount - 1) / QuantityOfPictures + 1];
            var i = 0;

            do
            {
                filesCount = filesCount - QuantityOfPictures;
                result[i] = "/Default.cshtml?page=" + i;
                i++;
            }
            while (filesCount > 0);

            return result;
        }
        void DoDelete()
        {
            string[] files = Directory.GetFiles(PathToImage, "*.jpg");

            string imageFilePath = PathToImage;
            if (files.Length == 0)
            {
                throw new InvalidOperationException("The array is empty");
            }
            else File.Delete(imageFilePath + HttpContext.Current.Request.Form["DDList"]);
        }
        void DoUpload()
        {

            DateTime now = DateTime.Now;
            string Text = "Ok";
            Text = HttpContext.Current.Request.Form["FirstName"];

            var path = Path.Combine(PathToImage, HttpContext.Current.Request.Files[0].FileName);

            string FileName = Path.GetFileNameWithoutExtension(path);
            string FileExt = Path.GetExtension(path);

            string temp = FileName;
            int i = 1;

            while (File.Exists(path))
            {
                temp = FileName + i;
                path = Path.Combine(PathToImage, temp + FileExt);
                i++;
            }
            FileName = temp + FileExt;

            HttpContext.Current.Request.Files[0].SaveAs(path);


            Text += FileName;


            using (StreamWriter myStream = new StreamWriter(PathToTxtFile, true))
            {
                myStream.WriteLine("{0}#{1}", FileName, now);
            }

        }
    }


}

