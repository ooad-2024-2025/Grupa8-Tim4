using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;


namespace Cvjecara_Latica.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [StringLength(300, ErrorMessage = "Image URL cannot exceed 300 characters.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        public double Price { get; set; }

        [DisplayName("Flower type")]

        [StringLength(50, ErrorMessage = "Flower type cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s\-]*$", ErrorMessage = "Flower type can contain only letters, spaces, and hyphens.")]
        public string? FlowerType { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, 1000, ErrorMessage = "Stock must be between 0 and 1000.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product type is required.")]
        [StringLength(50, ErrorMessage = "Product type cannot exceed 50 characters.")]
        public string productType { get; set; }
        [Required(ErrorMessage = "Color is required.")]
        [StringLength(30, ErrorMessage = "Color cannot exceed 30 characters.")]
        public string Color { get; set; }

        public bool IsBestSeller { get; set; }

        public Product()
        {
        }
    }
   
    }
