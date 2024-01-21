using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var latestBlockNumber = await GetLatestBlockNumber();
            Console.WriteLine($"Latest block number: {latestBlockNumber}");

            // Diğer işlemlerinizi burada devam ettirebilirsiniz
        }

        static async Task<int> GetLatestBlockNumber()
        {
            try
            {
                string alchemyApiKey = Environment.GetEnvironmentVariable("ALCHEMY_API_KEY");
                string apiUrl = $"https://eth-mainnet.alchemyapi.io/v2/{alchemyApiKey}";

                string openseaNFTContractAddress = "0x00000000000000ADc04C56Bf30aC9d3c0aAF14dC";
                string orderFulfilledEventSignature = "0x9d9af8e38d66c62e2c12f0225249fd9d721c54b83f48d9352c97c6cacdcb6f31";

                using (HttpClient client = new HttpClient())
                {
                    string endpoint = $"/?module=logs&action=getLogs&fromBlock=latest&toBlock=latest&address={openseaNFTContractAddress}&topic0={orderFulfilledEventSignature}";
                    var response = await client.GetStringAsync(apiUrl + endpoint);
                    dynamic data = JsonConvert.DeserializeObject(response);

                    int latestBlockNumber = data.result[0].blockNumber;
                    Console.WriteLine($"Latest block number: {latestBlockNumber}");
                    return latestBlockNumber;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Alchemy API error: {ex.Message}");
                throw new Exception("Error during Alchemy API request");
            }
        }

        static void HandleOrderFulfilledEvent(dynamic data)
        {
            Console.WriteLine($"Received OrderFulfilled event data: {data}");
            // Burada OrderFulfilled event'ını işleyebilirsiniz
        }

        static async Task Main(string[] args)
        {
            var latestBlockNumber = await GetLatestBlockNumber();
            Console.WriteLine($"Latest block number: {latestBlockNumber}");

            // Diğer işlemlerinizi burada devam ettirebilirsiniz
        }

        static void Main(string[] args)
        {
            var latestBlockNumber = GetLatestBlockNumber().Result;
            Console.WriteLine($"Latest block number: {latestBlockNumber}");

            // Diğer işlemlerinizi burada devam ettirebilirsiniz
        }

        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseMvc();
            }
        }

        public class WeatherController : Controller
        {
            [HttpGet("/block")]
            public async Task<IActionResult> GetBlock()
            {
                try
                {
                    var latestBlockNumber = await GetLatestBlockNumber();
                    return Json(new { blockNumber = latestBlockNumber });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during /block endpoint: {ex.Message}");
                    return StatusCode(500, new { error = "Internal Server Error" });
                }
            }

            [HttpPost("/alchemy-webhook")]
            public IActionResult AlchemyWebhook([FromBody] dynamic data)
            {
                // Gelen veri içinde OrderFulfilled event'ını işleyecek bir kontrol ekleyebilirsiniz
                if (data != null && data.event != null && data.event.type == "OrderFulfilled")
                {
                    HandleOrderFulfilledEvent(data);
                }
                return Ok("Webhook received successfully");
            }
        }
    }
}
