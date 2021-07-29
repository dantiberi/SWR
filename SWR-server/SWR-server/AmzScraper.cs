using IronWebScraper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public double price;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public Boolean parseComplete = false;

        public AmzScraper(String url)
        {
            this.url = url;

            this.Start();
        }

        public override void Init()
        {
            //this.LoggingLevel = WebScraper.LogLevel.All; //Can be disabled
            this.Request(url, Parse);
        }

        public override void Parse(Response response)
        {
            //Get Product Image Url       
            HtmlNode productImgNode = response.GetElementById("landingImage");      
            this.productImg = productImgNode.GetAttribute("src");

            this.price = Double.Parse(response.GetElementById("priceblock_ourprice").InnerText.Remove(0,1));//Removes '$' from innter text.

            this.name = response.GetElementById("productTitle").InnerText.Replace("\n","");
      
            this.parseComplete = true;
        }

        //Getters
        public String getProductImgUrl()
        {
            return this.productImg;
        }
    }
}
