using FroggyFlop.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FroggyFlop.Models.ViewModels.Shop
{
    public class ProductVM
    {
        public ProductVM() { }

        public ProductVM(ProductDTO model)
        {
            Id = model.Id;
            Name = model.Name;
            Slug = model.Slug;
            Description = model.Description;
            Price = model.Price;
            CategoryName = model.CategoryName;
            CategoryId = model.CategoryId;
            ImageName = model.ImageName;
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("Image")]
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<String> GalleryImages { get; set; }
    }
}