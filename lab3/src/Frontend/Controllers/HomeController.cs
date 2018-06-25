using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        HttpClient _client = new HttpClient();
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string data) 
        { 
            _client.BaseAddress = new Uri("http://localhost:5000");
            var content = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("value", data)
            });
            var request = await _client.PostAsync("api/values", content);
                var requestContent = await request.Content.ReadAsStringAsync();
            return Ok(requestContent);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
