using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Comment
    {
        
        public int CommentId { get; set; }
        [Required]
        [MaxLength(500)]
        public string CommentText { get; set; }
        [Required]
        [MaxLength(5)]
        public string ProductCode { get; set; }

        [MaxLength(200)]
        public string SessionId { get; set; }
       
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public Comment()
        { 
        
        }
        public Comment(int commentId, string commentText, string productCode, int userId, DateTime createdDate)
        {
            CommentId = commentId;
            CommentText = commentText;
            ProductCode = productCode;
            UserId = userId;
            CreatedDate = CreatedDate;
        }

        public Comment(int commentId, string commentText, string productCode, int userId, DateTime createdDate, string sessionId) : this(commentId, commentText, productCode, userId, createdDate)
        {
            SessionId = sessionId;
        }
    }
}
