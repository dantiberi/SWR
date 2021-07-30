﻿using Microsoft.AspNetCore.Http;
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
        //Scrapers
        AmzScraper amz = new AmzScraper();

        [HttpPost()]
        [Route("AddAmazonProduct/")]
        public string storeAmazonProduct([FromBody] ProductModel amzProduct)
        {
            amz.start(amzProduct.url);
            
            Program.db.addProduct(DB.conn, amz.url, amz.name, amz.price, amz.productImg);
            //Program.db.printProductTable(DB.conn);
            string res = JsonConvert.SerializeObject(amz);

            amz.stop();

            GC.Collect();//Helps a ton with a memory leak coming from Iron Web Scraper.

            return res;
        }

        /*
         * Returns the last inserted product.
         * Useful for add and immediate retrieve functionality.
         * */
        [HttpGet]
        public string LastProduct()
        {
            return Program.db.getLastInsertedProductId().ToString();
        }
    }
}