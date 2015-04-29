using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AjaxApp.Api.Controllers
{
	[Authorize]
    public class ProductController : ApiController
    {
        // GET api/products
		[HttpGet]
		[Route("api/products/list")]
		public IHttpActionResult Get()
        {
            return Ok<ProductListViewModel>(new ProductListViewModel());
        }

        // GET api/product/5
		[HttpGet]
		[Route("api/products/get/{id}")]
        public IHttpActionResult Get(int id)
        {
			 var model = new ProductListViewModel();

			 if (id > model.Products.Count)
	        {
		        return BadRequest("Invalid Product Id");
	        }


			 return Ok(model.Products[id]);
        }

		
			
		//Push, Put, Deltet haven't been implemented


		public class ProductListViewModel
		{
			private List<Product> _products = new List<Product>();

			public List<Product> Products
			{
				get { return _products; }
			}

			public ProductListViewModel()
			{
				SeedMethod();
			}

			private void SeedMethod()
			{
				Products.Add(new Product()
				{
					Name = "Xbox One",
					Price = 498,
					Made = "Microsoft",
					NoOfStock = 10,
					ImageUrl = "/images/xbox_one.jpg"
				});

				Products.Add(new Product()
				{
					Name = "PlayStation 4 ",
					Price = 598,
					Made = "Sony",
					NoOfStock = 20,
					ImageUrl = "/images/ps4.jpg"
				});

				Products.Add(new Product()
				{
					Name = "3DS XL",
					Price = 258,
					Made = "Nitendo",
					NoOfStock = 8,
					ImageUrl = "/images/3dsxl.jpg"
				});

				Products.Add(new Product()
				{
					Name = "IPhone 6",
					Price = 869,
					Made = "Apple",
					NoOfStock = 12,
					ImageUrl = "/images/iphone6.jpg"
				});

				Products.Add(new Product()
				{
					Name = "Galaxy S6",
					Price = 799,
					Made = "Samsung",
					NoOfStock = 14,
					ImageUrl = "/images/galaxy_s6.jpg"
				});
			}
		}

	    public class Product
	    {
			public string Name { get; set; }
			public decimal Price { get; set; }
			public string Made { get; set; }
			public int NoOfStock { get; set; }
			//this is not a good way...but just a demo!!!
			public string ImageUrl { get; set; }
	    }
    }
}
