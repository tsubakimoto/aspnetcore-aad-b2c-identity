using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using WebApplication1.Models;
using WebApplication1.Models.AccountViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly string tenant;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly AuthenticationContext authContext;
        private readonly ClientCredential credential;
        private readonly string userPassword;

        public HomeController(IConfiguration configuration)
        {
            tenant = configuration["Tenant"];
            clientId = configuration["ClientId"];
            clientSecret = configuration["ClientSecret"];
            authContext = new AuthenticationContext($"https://login.microsoftonline.com/{tenant}");
            credential = new ClientCredential(clientId, clientSecret);
            userPassword = configuration["UserPassword"];
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

        private async Task<string> SendGraphGetRequestAsync(string api, string query = null)
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

        private async Task<string> SendGraphPostRequestAsync(string api, string json)
        {
            // NOTE: This client uses ADAL v2, not ADAL v4
            var accessToken = await GetTokenAsync();
            HttpClient http = new HttpClient();
            var url = $"https://graph.windows.net/{tenant}{api}?api-version=1.6";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IActionResult> GetUsers()
        {
            var result = await SendGraphGetRequestAsync("/users");
            return Content(result);
        }

        public async Task<IActionResult> CreateUser()
        {
            var name = $"yuta{DateTime.Now.Ticks}";
            var email = $"{name}@example.com";
            var user = new AadB2cUser
            {
                accountEnabled = true,
                signInNames = new Signinname[]
                {
                    new Signinname { type = "emailAddress", value = email }
                },
                creationType = "LocalAccount",
                displayName = name,
                mailNickname = name,
                passwordProfile = new Passwordprofile
                {
                    password = userPassword,
                    forceChangePasswordNextLogin = false
                },
                passwordPolicies = "DisablePasswordExpiration",
                city = "Fukuoka",
                givenName = "Yuta",
                postalCode = "000-0000",
                state = "Fukuoka",
                surname = "Matsumura",
            };
            var result = await SendGraphPostRequestAsync("/users", JsonConvert.SerializeObject(user));
            return Content(result);
        }
    }
}
