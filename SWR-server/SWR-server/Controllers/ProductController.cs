using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWR_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpPost()]
        [Route("AddAmazonProduct/")]
        public string storeAmazonProduct([FromBody] ProductModel amzProduct)
        {
            AmzScraper amz = new AmzScraper();

            amz.start(amzProduct.url);

            Program.db.addProduct(DB.conn, amz.url, amz.name, amz.price, amz.productImg, amz.isOnSale);
            //Program.db.printProductTable(DB.conn);
            string res = JsonConvert.SerializeObject(amz);

            amz.stop();
            amz = null;

            GC.Collect();//Helps a ton with a memory leak coming from Iron Web Scraper.

            return res;
        }

        /*
         * Returns the last inserted product.
         * Useful for add and immediate retrieve functionality.
         * */
        [HttpGet]
        [Route("LastProduct/")]
        public string LastProduct()
        {
            return Program.db.getLastInsertedProductId(DB.conn).ToString();
        }

        /*
         * Returns the product JSON of given p_id.
         * */
        [HttpGet]
        [Route("GetProduct/")]
        public string getProduct(int id)
        {
            //System.Diagnostics.Debug.WriteLine("!!!!! getProduct CALLED");
            //System.Diagnostics.Debug.WriteLine(Program.db.getJsonOfProduct(DB.conn, id));
            return Program.db.getJsonOfProduct(DB.conn, id);
        }

        /// <summary>
        /// Calls the DB instance to return a JSON object array string of all products in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllProducts/")]
        public string getAllProducts()
        {
            return Program.db.getAllProductsInJson(DB.conn);
        }

        /// <summary>
        /// Calls database to remove the product with the given p_id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("RemoveProduct/")]
        public string removeProduct(int id)
        {
            //System.Diagnostics.Debug.WriteLine("DELETE CALLED ON PRODUCT " + id);
            Program.db.removeProduct(DB.conn, id);
            return "Product " + id + " has successfully been deleted";
        }
    }
}
