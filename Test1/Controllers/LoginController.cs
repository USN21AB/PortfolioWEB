﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//I added Firebase.Auth
using Firebase.Auth;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Test1.Models;
using Portefolio_webApp.Models;
using System.Diagnostics;
using System;

namespace Test1.Controllers
{
    public class LoginController : Controller
    {
        FirebaseAuthProvider auth;
        public LoginController()
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyAF3lsyJBHDwpdd2u9D0qW-m3c2TJftQvE"));
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Bruker bruker)
        {
            //create the user
            await auth.CreateUserWithEmailAndPasswordAsync(bruker.Email, bruker.Password);
            //log in the new user

            var fbAuthLink = await auth
                            .SignInWithEmailAndPasswordAsync(bruker.Email, bruker.Password);

            string token = fbAuthLink.FirebaseToken;
            //saving the token in a session variable
            if (token != null)
            {
                HttpContext.Session.SetString("_UserToken", token);

                return Redirect("~/ProfilSide/ProfilSide");
            }
            else
            {
                return View();
            }
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(Bruker bruker)
        {

            try
            {
                //log in the user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(bruker.Email, bruker.Password);
                string token = fbAuthLink.FirebaseToken;
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

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

            //Debug.WriteLine("--------------||||||||||||||||||||||||" + token);

            //saving the token in a session variable

        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            return Redirect("SignIn");
        }
    }
}