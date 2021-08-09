using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SWR_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// Calls the static DB instance to add a product to the database.
        /// </summary>
        /// <param name="amzProduct">Json object using the ProductModel</param>
        /// <returns>Ok status with added product in body.</returns>
        [HttpPost()]
        [Route("AddAmazonProduct/")]
        public IActionResult storeAmazonProduct([FromBody] ProductModel amzProduct)
        {
            AmzScraper amz = new AmzScraper();

            amz.start(amzProduct.url);

            Program.db.AddProduct(DB.conn, amz.url, amz.name, amz.price, amz.productImg, amz.isOnSale);
            //Program.db.printProductTable(DB.conn);
            string res = JsonConvert.SerializeObject(amz);

            amz.stop();
            amz = null;

            GC.Collect();//Helps a ton with a memory leak coming from Iron Web Scraper.

            //return res;

            return Ok(res);
        }

        /// <summary>
        /// Calls the static DB instance to return a JSON object representing the last product inserted.
        /// Useful for add and immediate retrieve functions.
        /// </summary>
        /// <returns>A string representing a product as a Json object.</returns>
        [HttpGet]
        [Route("LastProduct/")]
        public string LastProduct()
        {
            return Program.db.GetLastInsertedProductId(DB.conn).ToString();
        }

        /// <summary>
        /// Calls the static DB instance to return a JSON object string for the product with the given p_id.
        /// </summary>
        /// <param name="id">p_id of product.</param>
        /// <returns>A string representing a product as a Json object.</returns>
        [HttpGet]
        [Route("GetProduct/")]
        public string getProduct(int id)
        {
            //System.Diagnostics.Debug.WriteLine("!!!!! getProduct CALLED");
            //System.Diagnostics.Debug.WriteLine(Program.db.getJsonOfProduct(DB.conn, id));
            return Program.db.GetJsonOfProduct(DB.conn, id);
        }

        /// <summary>
        /// Calls the static DB instance to return a JSON object array string of all products in the database.
        /// </summary>
        /// <returns>String representing a Json object array.</returns>
        [HttpGet]
        [Route("GetAllProducts/")]
        public string getAllProducts()
        {
            return Program.db.GetAllProductsInJson(DB.conn);
        }

        /// <summary>
        /// Removes a product from the database with the given p_id.
        /// </summary>
        /// <param name="id">p_id of product</param>
        /// <returns>Ok status.</returns>
        [HttpDelete]
        [Route("RemoveProduct/")]
        public IActionResult removeProduct(int id)
        {
            //System.Diagnostics.Debug.WriteLine("DELETE CALLED ON PRODUCT " + id);
            Program.db.RemoveProduct(DB.conn, id);
            //return "Product " + id + " has successfully been deleted";
            return Ok("{Product " + id + " has successfully been deleted}");
        }
    }
}
