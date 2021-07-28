using IronWebScraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWR_server
{
    public class AmzScraper : WebScraper
    {
        private String url;
        private String productImg; //Url to product image.

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
        }

        //Getters
        public String getProductImgUrl()
        {
            return this.productImg;
        }
    }
}
