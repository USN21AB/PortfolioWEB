using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portefolio_webApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

using Test1.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Portefolio_webApp.Controllers
{
    public class HomeController : Controller
    {

     

        private readonly FirebaseDB firebase;

        //private ISession session;

        [BindProperty]
        public List<Innlegg> AlleInnlegg { get; set; }


        public HomeController()
        {
          
            firebase = new FirebaseDB();
            string fullpath1 = "";
        }

        private bool IsValidExtension(IFormFile filename)
        {
            bool isValid = false;
            Char delimiter = '.';
            string fileExtension;
            string[] imgTypes = new string[] { "png", "jpg", "gif", "jpeg" };
            string[] documentsTypes = new string[] { "doc", "docx", "xls", "xlsx", "pdf", "ppt", "pptx" };
            string[] collTypes = new string[] { "zip", "rar", "tar" };
            string[] VideoTypes = new string[] { "mp4", "flv", "mkv", "3gp" };
            fileExtension = filename.FileName.Split(delimiter).Last();
            // fileExtension = substrings[substrings.Length - 1].ToString();
            int fileType = 0;
            if (imgTypes.Contains(fileExtension.ToLower()))
            {
                fileType = 1;
            }
            else if (documentsTypes.Contains(fileExtension.ToLower()))
            {
                fileType = 2;
            }
            else if (collTypes.Contains(fileExtension.ToLower()))
            {
                fileType = 3;
            }
            else if (VideoTypes.Contains(fileExtension.ToLower()))
            {
                fileType = 4;
            }
            switch (fileType)
            {
                case 1:
                    if (imgTypes.Contains(fileExtension.ToLower()))
                    {
                        isValid = true;
                    }
                    break;
                case 2:
                    if (documentsTypes.Contains(fileExtension.ToLower()))
                    {
                        isValid = false;
                    }
                    break;
                case 3:
                    if (collTypes.Contains(fileExtension.ToLower()))
                    {
                        isValid = false;
                    }
                    break;
                case 4:
                    if (VideoTypes.Contains(fileExtension.ToLower()))
                    {
                        isValid = true;
                    }
                    break;
                default:
                    isValid = false;
                    break;
            }
            return isValid;
        }

        private string GetNewFileName(string filenamestart, IFormFile filename)
        {
            Char delimiter = '.';
            string fileExtension;
            string strFileName = string.Empty;
            strFileName = DateTime.Now.ToString().
                Replace(" ", string.Empty).
                Replace("/", "-").Replace(":", "-");
            fileExtension = filename.FileName.Split(delimiter).Last();
            Random ran = new Random();
            strFileName = $"{ filenamestart}_{ran.Next(0, 100)}_{strFileName}.{fileExtension}";
            return strFileName;
        }
        public async Task<ActionResult> UploadFilesWihtLocation([FromServices] IHostingEnvironment oHostingEnvironment)
        {

            Console.WriteLine("EHHHH?????");
            Console.WriteLine("ID: " + HttpContext.Session.GetString("_UserID"));
            string hoststr = oHostingEnvironment.WebRootPath;

            string[] strFileNames;
            ///string url = "";
            try
            {
               
                string brukerId = HttpContext.Session.GetString("_UserID");

                //END Temporary

                long size = 0;
                var files = Request.Form.Files;
                strFileNames = new string[files.Count];
                string fileLocation = Request.Form["UploadLocation"].ToString();
                //string[] path = fileLocation.Split("/");
                string fileInitals = Request.Form["FileInitials"].ToString();

                Console.WriteLine("EHHHH?????" + "Something" + fileInitals + " Eh?");
                int i = 0;
                string[] path = fileLocation.Split("\\");
                if (!Directory.Exists(hoststr + "\\" + path[1]))
                {
                    Directory.CreateDirectory(hoststr + "\\" + path[1]);
                    Directory.CreateDirectory(hoststr + "\\" + path[1] + "\\" + path[2]);
                }
                else if (!Directory.Exists(hoststr + fileLocation))
                {
                    Directory.CreateDirectory(hoststr + "\\" + path[1] + "\\" + path[2]);
                }
                foreach (var file in files)
                {
                    if (IsValidExtension(file))
                    {
                        var filename = GetNewFileName(fileInitals + i, file);
                        strFileNames[i] = fileLocation + filename;
                        string fullpath = hoststr + fileLocation + $@"\{filename}";
                        size += file.Length;
                        using (FileStream fs = System.IO.File.Create(fullpath))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                            fs.Close();


                            HttpContext.Session.SetString("CroppedPath", fullpath);



                        }
                    }
                    else
                    {
                        strFileNames = new string[1];
                        strFileNames[0] = "Invalid File";
                    }
                    i = i + 1;
                }
            }
            catch (Exception ex)
            {
                strFileNames = new string[1];
                strFileNames[0] = ex.Message;
            }
            return Json(strFileNames);

            return View("ProfilSide");


        }

        public void Test()
        {
            Debug.WriteLine("----------- JEG ER INNI CONTROLLER: MESSAGE TEST");
            Debug.WriteLine("----------- JEG ER INNI CONTROLLER: MESSAGE TEST");
        }

        public async Task<IActionResult> SendMelding()
        {

            if (FirebaseApp.DefaultInstance == null) { 
                var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
            });

                Debug.WriteLine(defaultApp.Name); // "[DEFAULT]"
            }

        
            var registrationToken = "fRD3Yaa0Q3DiAYY3OH35qI:APA91bGD4zOO_-P8IR5rPNbJKvIt9Ig17qCYzDW-Qotahsx_Qr5R6-4BHCvYqFVkftozTNL6xDEmECEplzTH7Gx-TE0ucJ4-JSU2wbqSkQU2xPkEBrh5d53_iLDeA-mJ5Usiy91y3JDJ";

            // See documentation on defining a message payload.
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    ["FirstName"] = "John",
                    ["LastName"] = "Doe"
                },
                Notification = new Notification
                {
                    Title = "FirebaseTest",
                    Body = "Dette er en beskjed til deg!"
                },

                Token = registrationToken
               
            };

            // Send a message to the device corresponding to the provided
            // registration token.
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            // Response is a message ID string.
            Debug.WriteLine("Successfully sent message: " + result);
            var data = new { status = "suksess" };
            return Json(data); 
        }

        public IActionResult LoggInnSide()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BrowseSide(string kategori, string søketekst)
        {
           
            Debug.WriteLine("SØK: " + søketekst); 
            if (string.IsNullOrEmpty(kategori))
            {
          
                AlleInnlegg = firebase.HentAlleInnlegg();
                TempData["valgtKnapp"] = "alt";
                ViewData["Kategori"] = "alt";

               ViewData["liste"] = AlleInnlegg;
            }
            else {

                var listen = firebase.HentAlleInnlegg();
                var katListe = firebase.SorterAlleInnlegg(kategori, listen);

                if (søketekst != null)
                {
                    var filterListe = new List<Innlegg>(); 
                    Debug.WriteLine("Let's search this site!!!" + søketekst);
                    
                    for(int i= 0; i<katListe.Count; i++)
                    {
                        var tagger = katListe[i].Tagger;
                        if ((katListe[i].Tittel.ToLower()).Contains(søketekst.ToLower()))
                        { //Funnet match i tittel
                            filterListe.Add(katListe[i]);
                        }
                        else { //Tittel ikke funnet, skjekk i tagger? 
                             if(tagger != null)
                                for(int j = 0; j< tagger.Count; j++)
                                 {

                                      if ((tagger[j].ToLower()).Contains(søketekst.ToLower()))
                                          filterListe.Add(katListe[i]); 
                                  }
                        }
                    }

                    katListe = filterListe; 
                }


                ViewData["liste"] = katListe; 
                TempData["valgtKnapp"] = kategori;
                ViewData["Kategori"] = kategori;
                ViewData["Søk"] = søketekst; 
            }

            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");

           
            if(HttpContext.Session.GetString("Innlogget_Bruker") != null){
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
            }

            return View();
        }

        [HttpGet]
        public IActionResult SorterListe()
        {
            //asp-route-Type="Kunst" 
            //ViewData["liste"] = firebase.SorterAlleInnlegg("Kunst");
                   return RedirectToPage("BrowseSide"); 
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
