using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SWR_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetcherController : ControllerBase
    {
        //string result = JsonConvert.SerializeObject(new BasicResponseObject());
        // GET: api/<FetcherController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
            
        //    //Response.Headers.Add("Access-Control-Allow-Origin", "*");
        //    if (!Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
        //    {
        //        Response.Headers.Add("Access-Control-Allow-Origin", "*");
        //    }
        //    return new string[] { "value1", "value2" };

        //}

        [HttpGet]
        public string Get()
        {
            string result = JsonConvert.SerializeObject(new BasicResponseObject());
            if (!Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return result;

        }

        // GET api/<FetcherController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            if (!Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return id.ToString();
        }

        // POST api/<FetcherController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FetcherController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FetcherController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
