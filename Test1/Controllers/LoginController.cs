using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//I added Firebase.Auth
using Firebase.Auth;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Test1.Models;
using Portefolio_webApp.Models;
using System.Diagnostics;
using System;
using Newtonsoft.Json;

/// <summary>
/// Denne kontrolleren har ansvar for all logg inn og logg ut aktivitet. Den forholder seg til authentication i firebase. 
/// Vi har brukt dokumentasjonen til firebase
/// Firebase authentication docs: https://firebase.google.com/docs/auth
/// </summary>

namespace Test1.Controllers
{
    public class LoginController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly FirebaseDB firebase;

        public LoginController()
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyAF3lsyJBHDwpdd2u9D0qW-m3c2TJftQvE"));
            firebase = new FirebaseDB();

 
        }

        //registrer en ny bruker
        [NonAction]
        public async Task<string> Register(string Email, string Password)
        {
            Debug.WriteLine("--------------------------- INNI REGISTRER2 LOGGINN " + Email + " " + Password);
            var id = ""; 
            await auth.CreateUserWithEmailAndPasswordAsync(Email, Password);
            
  
            //log in the new user

            var fbAuthLink = await auth
                            .SignInWithEmailAndPasswordAsync(Email, Password);
            

            string token = fbAuthLink.FirebaseToken;
            //saving the token in a session variable
            if (token != null)
            {
                id = fbAuthLink.User.LocalId;
                Debug.WriteLine("--------------------------- INNI REGISTRER2 ID: " + id + " token: " + token );
            

      
                return id + "|" + token;
            }
            return "";  //Noe gikk feil.
        }

        //Logg inn side
        public IActionResult SignIn()
        {
            return View();
        }

        //Logg inn innlogginskjema har blitt postet
        [HttpPost]
        public async Task<IActionResult> SignIn(string Password, string Email)
        {

            try
            {
                //log in the user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(Email, Password);
              
                string token = fbAuthLink.FirebaseToken;
               
                if (token != null)
                {
                    
                    Bruker bruker2 = firebase.HentEnkeltBruker(fbAuthLink.User.LocalId);

                    if (bruker2 == null)
                    {
                        ViewBag.Exception = "Account does not exist";
                        return View();
                    }

                    //Saver token i en session variabel
                    HttpContext.Session.SetString("_UserToken", token);
                    HttpContext.Session.SetString("_UserID", fbAuthLink.User.LocalId);

                    var str = JsonConvert.SerializeObject(bruker2);
                    HttpContext.Session.SetString("Innlogget_Bruker", str);


                    return Redirect("~/Home/BrowseSide");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = "Wrong username or password ";
                return View();
            }


        }

        //Logg brukeren ut.
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            HttpContext.Session.Remove("_UserID");
            HttpContext.Session.Remove("Innlogget_Bruker");

            return Redirect("SignIn");
        }
    }
}