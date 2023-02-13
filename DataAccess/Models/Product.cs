using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Product
    {
        [Required]
        [Display(Name ="Product Code")]
        [MaxLength(5)]
        public string ProductCode { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        [MaxLength(50)]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Product Price")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Two decimal places. ")]
        public decimal ProductPrice  { get; set; }
        [MaxLength(2000)]
        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Update Date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}
