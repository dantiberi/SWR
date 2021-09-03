using IronWebScraper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * Must call start(url) and stop.
 */

namespace SWR_server
{
    [JsonObject(MemberSerialization.OptIn)]//Allows us to specify specific properties for Json conversion
    public class AmzScraper : WebScraper
    {
        [JsonProperty]//Specified which properties to use for converting this class to Json
        public String url;
        [JsonProperty]
        public String productImg; //Url to product image.
        [JsonProperty]
        public double price = -1.0;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public Boolean parseComplete = false;
        [JsonProperty]
        public int isOnSale = 0;

        public AmzScraper()
        {  
        }

        public override void Init()
        {
            //this.LoggingLevel = WebScraper.LogLevel.All; //Can be disabled      
            this.Request(this.url, Parse);

            //Handle bad Url
            if(this.price == -1.0)//Did not change after calling Request.
            {
                handleBadUrl();
            }
        }

        public override void Parse(Response response)
        {
            HtmlNode productImgNode = response.GetElementById("landingImage");
            this.productImg = productImgNode.GetAttribute("src");
            this.isOnSale = 0;//Will be overwritten if is on sale.

            HtmlNode price;
            String priceString = "";
            try
            {
                price = response.GetElementById("priceblock_ourprice");
                priceString = price.InnerText;
            }
            catch (System.NullReferenceException e)
            {
                priceString = response.GetElementById("priceblock_dealprice").InnerText;
                this.isOnSale = 1;
            }

            priceString = priceString.Remove(0, 1);//Removes '$' from string.
            this.price = Double.Parse(priceString);

            this.name = response.GetElementById("productTitle").InnerText.Replace("\n", "");

            this.parseComplete = true;
                 
        }

        /// <summary>
        /// Defines what this object will be if the url is bad. 
        /// This code will change later.
        /// </summary>
        public void handleBadUrl()
        {
            this.name = "Product Not Found!";
            this.productImg = "https://uboachan.net/warc/src/1340433133397.jpeg";
            this.price = -1.00;
            this.parseComplete = false;
            Print("404 Error");
        }

        //Getters
        public String getProductImgUrl()
        {
            return this.productImg;
        }

        public void stop()
        {
            this.Stop();
        }

        public void start(string url)
        {
            this.url = url;
            this.Start();
        }

        private static void Print(string s)
        {
            System.Diagnostics.Debug.WriteLine("DB OUTPUT: " + s);
        }
    }
}
