
using TypicalTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using DataAccess;
using DataAccess.Models;

using Microsoft.AspNetCore.Authorization;

namespace TypicalTools.Controllers
{
    public class CommentController : Controller
    {
        private readonly DBContext _context;

        public CommentController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CommentList(string id)
        {
            string productCode = id;
            List<Comment> comments = _context.GetCommentsForProduct(productCode);

            if(comments == null)
            {
                return RedirectToAction("Index", "Product");
            }

            return View(comments);

        }


        // Show a form to add a new comment
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet]
        public IActionResult AddComment(string productCode)
        {
            Comment comment = new Comment();
            comment.ProductCode = productCode;
            return View(comment);
        }

        // Receive and handle the newly created comment data
        [Authorize(Roles = "Customer,Admin")]
        [HttpPost]
        public IActionResult AddComment(Comment comment)
        {
            
            if (ModelState.IsValid)
            {
                _context.AddComment(comment, HttpContext.Session.Id);
            }

                // A session id is only set once a value has been added!
                // adding a value here to ensure the session is created
                HttpContext.Session.SetString("CommentText", comment.CommentText);

                return RedirectToAction("CommentList", "Comment", new { id = comment.ProductCode });
         
        }

        // Receive and handle a request to Delete a comment
        [Authorize(Roles = "Customer,Admin")]
        public IActionResult RemoveComment(int commentId)
        {
            var comment = _context.GetSingleComment(commentId);

            // Check if the admin is logged in
            //string authStatus = HttpContext.Session.GetString("Authenticated");
            //bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("True");

            // Peform the deletion conditionally
           // if (comment.SessionId == HttpContext.Session.Id || isAdmin)
           // {
                _context.DeleteComment(commentId);
           // }

            return RedirectToAction("CommentList", "Comment", new {id = comment.ProductCode});
        }

        // Show a existing comment details in a form to allow for editing
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet]
        public IActionResult EditComment(int commentId)
        {
            Comment comment = _context.GetSingleComment(commentId);
            return View(comment);
        }

        // Receive and handle the edited comment data
        [Authorize(Roles = "Customer,Admin")]
        [HttpPost]
        public IActionResult EditComment(Comment comment)
        {
            if(comment == null)
            {
                return RedirectToAction("CommentList", "Product");
            }

            // Check if the admin is logged in
            string authStatus = HttpContext.Session.GetString("Authenticated");
            bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("True");

            if (comment.SessionId == HttpContext.Session.Id || isAdmin)
            {
                if (ModelState.IsValid)
                { 
                    _context.EditComment(comment); 
                
                }
            }

            

            return RedirectToAction("CommentList", "Comment", new { id = comment.ProductCode });

        }
    }
}
