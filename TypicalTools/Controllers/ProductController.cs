
using TypicalTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace TypicalTools.Controllers
{
    public class ProductController : Controller
    {
        private readonly DBContext _context;

        public ProductController(DBContext dBContext)
        {
            _context = dBContext;
        }

        // Show all products
        public IActionResult Index()
        {
            var products = _context.GetProducts();
            foreach (var p in products)
            {
                p.ProductPrice = Math.Round(p.ProductPrice, 2);
            }
            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddProduct()
        {
            /* string authStatus = HttpContext.Session.GetString("Authenticated");
             bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("True");
             if(isAdmin)
             { 
                 Product product = new Product();
                 return View(product);
             }
             else 
             {
                 return RedirectToAction("AdminLogin", "Admin");
             }*/
            Product product = new Product();
            return View(product);
           
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            //check product code already exists
            if (_context.CheckProductCodeExist(product.ProductCode))// product code is not exist
            {
                //also check if price is 0 
                if (isPriceZero(product.ProductPrice))
                {
                    ViewBag.isPriceZero = "Price should be more than $0";// is 0
                    
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        _context.AddProduct(product);
                        return RedirectToAction("Index", "Product");
                    }
                }
                return View();

            }
            else
            {   
                ViewBag.isProductCodeExist = "Product Code is exist.";//exists

               
                return View();
            }

            


           

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult UpdateProductPrice(string id)
        {    
                Product product = new Product();
                string productCode = id;
                product = _context.GetAProduct(productCode);
                ViewBag.ProductName = product.ProductName;
                return View(product);          
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateProductPrice(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.UpdatePrice(product);

            }
                return RedirectToAction("Index", "Product");
            
        }

        private void SetupProductCodeCattegoryDDL()
        {
            var categories = _context.GetProductCodeCategories();
            var ddlCategories = categories.Select(c => new SelectListItem
            {
                Text = c.ProductName,
                Value = c.ProductCode.ToString()
            }).ToList();
            //Stores SelectListItem list into viewbag.
            ViewBag.Categories = ddlCategories;
        }

       /* private bool IsProductCodeExist(string productCode)
        {
            string existProductCode;
            existProductCode= _context.CheckProductCodeExist(productCode);
            if (String.IsNullOrEmpty(existProductCode))
            { return true; }
            return false;
        }*/

        private bool isPriceZero(decimal price)
        {
            if (price == 0)
            { 
                return true;
            }
            return false;
            
        }
    }
}
