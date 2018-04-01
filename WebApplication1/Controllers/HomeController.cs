using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly string tenant;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly AuthenticationContext authContext;
        private readonly ClientCredential credential;

        public HomeController(IConfiguration configuration)
        {
            tenant = configuration["Tenant"];
            clientId = configuration["ClientId"];
            clientSecret = configuration["ClientSecret"];
            authContext = new AuthenticationContext($"https://login.microsoftonline.com/{tenant}");
            credential = new ClientCredential(clientId, clientSecret);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetTokenAsync()
        {
            var result = await authContext.AcquireTokenAsync("https://graph.windows.net", credential);
            return result.AccessToken;
        }

        private async Task<string> SendGraphGetRequest(string api, string query = null)
        {
            var http = new HttpClient();
            var url = $"https://graph.windows.net/{tenant}{api}?api-version=1.6";
            if (!string.IsNullOrWhiteSpace(query))
            {
                url += "&" + query;
            }

            var accessToken = await GetTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await http.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IActionResult> GetUsers()
        {
            var result = await SendGraphGetRequest("/users");
            return Content(result);
        }
    }
}
