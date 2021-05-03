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
            Debug.WriteLine("Jeg er inni upload file");
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

        public JsonResult CheckFirebaseCount()
        {

            if(HttpContext.Session.GetString("_UserID") != null)
            {
                int lengde = firebase.TellAntallRader(HttpContext.Session.GetString("_UserID"));

             
                
                var data = new { status = "ok", resultat = lengde };
                return Json(data);
            }

            var data2 = new { status = "ok", resultat = -1 };
            return Json(data2);
        }

        public JsonResult RefreshUser()
        {
            Bruker refresBruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));

            var str2 = JsonConvert.SerializeObject(refresBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str2);
           
            var notific = JsonConvert.SerializeObject(refresBruker.notifications[refresBruker.NumberOfNotifications - 1]);
            var data = new { status = refresBruker.NumberOfNotifications, notific = notific };

            return Json(data);
        }


        public IActionResult LoggInnSide()
        {
            return View();
        }


        public IActionResult PartialViewBrowse(string kategori, string søketekst,List<Innlegg> listen)
        {
            listen = firebase.HentAlleInnlegg();
            var katListe = new List<Innlegg>();
            if (string.IsNullOrEmpty(kategori))
            { //Kategori er ikke valgt. Kjør ut alle innleggene

                
                TempData["valgtKnapp"] = "alt";
                ViewData["Kategori"] = "alt";

                ViewData["liste"] = listen;
                //Katliste er ikke sortert på kategori
                katListe = listen;
            }
            else
            {
                //Sorterer på valgt kategori
                katListe = firebase.SorterAlleInnlegg(kategori, listen);
            }
                if (søketekst != null)
                { //Det er en søketekst tilgjengelig
                    var filterListe = new List<Innlegg>(); 
                    
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

              
          //  }
         
         Debug.WriteLine("Inni PartialViewBrowse " + kategori + " " + søketekst); 
            return PartialView("_browse",new Innlegg());
        }

        [HttpGet]
        public IActionResult BrowseSide(string kategori, string søketekst)
        {

            
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
            

            if (HttpContext.Session.GetString("Innlogget_Bruker") != null){
                var str = HttpContext.Session.GetString("Innlogget_Bruker");
                var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

                ViewData["Innlogget_Bruker"] = innBruker;
                
            }

            ViewData["BrukerListe"] = firebase.updateAlgorithm();
            return View();
        }

        public JsonResult SendNotification(string type, Boolean erLest, string FraHvemID, string FraHvemNavn, string TilHvemID, string innleggID, string Tidspunkt)
        {

            Notifications not = new Notifications(type, erLest, FraHvemID, FraHvemNavn, TilHvemID, innleggID, Tidspunkt); 

            firebase.SendNotification(not);

            var resultat = "Jobberfaring oppdatert: " + type + " " + FraHvemNavn;
            var data = new { status = "ok", result = resultat };

            return Json(data);
        }

        public JsonResult NotificationRead()
        {
            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

            
            for(int i = 0; i< innBruker.notifications.Count; i++)
            {
                innBruker.notifications[i].erLest = true;
            }

            var str2 = JsonConvert.SerializeObject(innBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str2);

            firebase.OppdaterBruker(innBruker);

            var resultat = "Notifications er lest";
            var data = new { status = "ok", result = resultat };
            return Json(data);
        }

        public JsonResult AcceptReadCV(string fraHvemID, int notIndex, Boolean isAccepted)
        {
            var str = HttpContext.Session.GetString("Innlogget_Bruker");
            var innBruker = JsonConvert.DeserializeObject<Bruker>(str);

            if (isAccepted)
            {
                if (innBruker.CVAdgang == null)
                {
                    innBruker.CVAdgang = new List<string>();
                }

                innBruker.CVAdgang.Add(fraHvemID);

            }

            innBruker.NumberOfNotifications -= 1; 
            innBruker.notifications.RemoveAt(notIndex);
            firebase.OppdaterBruker(innBruker);

            var str2 = JsonConvert.SerializeObject(innBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str2);

            var resultat = "Accepted read CV";
            var data = new { status = "ok", result = resultat };
            return Json(data);
        }

        public JsonResult MottatNotification()
        {
           
            Bruker refreshBruker = firebase.HentEnkeltBruker(HttpContext.Session.GetString("_UserID"));

            var str = JsonConvert.SerializeObject(refreshBruker);
            HttpContext.Session.SetString("Innlogget_Bruker", str);

            var resultat = "Jobberfaring oppdatert: ";
            var data = new { status = "ok", result = resultat };

            return Json(data);
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
