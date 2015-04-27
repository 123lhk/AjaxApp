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
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "You are authorised!";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

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
					NoOfStock = 10
				});

				Products.Add(new Product()
				{
					Name = "PlayStation 4 ",
					Price = 598,
					Made = "Sony",
					NoOfStock = 20
				});

				Products.Add(new Product()
				{
					Name = "3DS XL",
					Price = 258,
					Made = "Nitendo",
					NoOfStock = 8
				});

				Products.Add(new Product()
				{
					Name = "IPhone 6",
					Price = 869,
					Made = "Apple",
					NoOfStock = 12
				});

				Products.Add(new Product()
				{
					Name = "Galaxy S6",
					Price = 799,
					Made = "Samsung",
					NoOfStock = 14
				});
			}
		}

	    public class Product
	    {
			public string Name { get; set; }
			public decimal Price { get; set; }
			public string Made { get; set; }
			public int NoOfStock { get; set; }
	    }
    }
}
