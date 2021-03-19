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

            string hoststr = oHostingEnvironment.WebRootPath;

            string[] strFileNames;
            ///string url = "";
            try
            {
                //BrukerID

                Bruker nybruker = new Bruker();
                nybruker = firebase.HentEnkeltBruker("-MTuAm8t_eBlv5KMiuWX");

                string brukerId = nybruker.Id;

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

                            await firebase.UploadProfilBilde(fullpath, file, brukerId);
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
        }

        public IActionResult LoggInnSide()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BrowseSide(string kategori)
        {
            if (string.IsNullOrEmpty(kategori))
            {
          
                AlleInnlegg = firebase.HentAlleInnlegg();
                TempData["valgtKnapp"] = "alt";

                ViewData["liste"] = AlleInnlegg;
            }
            else {

                var listen = firebase.HentAlleInnlegg();
                
               
                ViewData["liste"] = firebase.SorterAlleInnlegg(kategori, listen);
                TempData["valgtKnapp"] = kategori;
            }
            ViewData["Token"] = HttpContext.Session.GetString("_UserToken");
            ViewData["Innlogget_ID"] = HttpContext.Session.GetString("_UserID");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult ProfilSide()
        {
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
