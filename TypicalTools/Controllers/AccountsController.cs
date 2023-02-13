using TypicalTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TypicalTools.Controllers
{
    public class AccountsController : Controller
    {

        private readonly DBContext context;



        public AccountsController(DBContext dBContext)
        {
            context = dBContext;
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


        // public IActionResult AccessDenied([FromQuery] string redirectUrl)
        public IActionResult AccessDenied()
        {

            return View();
        }
        public IActionResult Login([FromQuery] string redirectUrl)
        {
            Account account = new Account()
            //add returnurl parameter to Account immediatly after we create account by account constructor. the syntaxt is similar to int a[2]={2,8};
            {
                //ReturnUrl = String.IsNullOrWhiteSpace(redirectUrl) ? "/Accounts/Login" : redirectUrl
                ReturnUrl = redirectUrl
            };
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Account account)
        {
            if (ModelState.IsValid)
            {
                Account logged = context.CheckLogin(account);
                if (logged != null)
                {
                    HttpContext.Session.SetString("User", logged.Username);
                    HttpContext.Session.SetString("Role", logged.Role);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, logged.Username),
                        new Claim(ClaimTypes.Role, logged.Role)

                    };
                    // this is what will be passed to the cookie
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                        RedirectUri = account.ReturnUrl
                    };
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Product");
                   
                }
            }

            ViewBag.ErrorMessage = "Invalid Username or Password";
            return View(account);
        }

        public IActionResult Register()
        {
            PopulateRoleOptions();

            return View();
        }

        private void PopulateRoleOptions()
        {
            var roles = Enum.GetValues(typeof(Roles)).Cast<Roles>().Select(c => new SelectListItem
            {
                Text = c.ToString(),
                Value = ((int)c).ToString()
            });
            ViewBag.roles = roles;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Account account)
        {
            if (account.Password.Equals(account.Salt) == false)
            {
                ViewBag.ErrorMessage = "Make sure password and confirmation match.";
                PopulateRoleOptions();
                return View(account);
            }

            account.Role = Enum.GetValues(typeof(Roles)).Cast<Roles>().ElementAt(int.Parse(account.Role)).ToString();
            bool status = context.CreateAccount(account);

            if (status)
            {
                return RedirectToAction("Index", "Home");
            }

            PopulateRoleOptions();
            ViewBag.ErrorMessage = "Username Already Exists.";
            return View(account);
        }
    




       public enum Roles
       {
           Admin,
           Customer
       }

          


     }
}
