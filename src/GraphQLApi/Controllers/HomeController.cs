using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GraphQLApi.Controllers
{
    public class TokenRequestModel
    {
        public string Text { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string clientId, string secret)
        {


            var client = _httpClientFactory.CreateClient();

            var url = $"{_config["JwtAuthSettings:Instance"]}/{_config["JwtAuthSettings:TenantId"]}/oauth2/v2.0/token";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("scope", $"api://{_config["JwtAuthSettings:ClientId"]}/.default"),
                new KeyValuePair<string, string>("client_secret", secret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await client.PostAsync(url, formContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var parsedResult = JsonConvert.DeserializeObject<TokenResponse>(result);
                return View(new TokenRequestModel() { Text = "{\n \"Authorization\": \"Bearer " + parsedResult.access_token + "\" \n}" });
            }

            return View(new TokenRequestModel() { Text = "INVALID CREDENTIALS" });
        }

        public IActionResult Documentation()
        {
            return View();
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
    }
}
