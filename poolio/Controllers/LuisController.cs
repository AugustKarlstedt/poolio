using Microsoft.Cognitive.LUIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace poolio.Controllers
{
    public class LuisController
    {
        private readonly string AppId = Environment.GetEnvironmentVariable("LUIS_APPID");
        private readonly string AzureSubscriptionKey = Environment.GetEnvironmentVariable("AZURE_SUBKEY");

        private LuisClient _client;

        public LuisController()
        {
            _client = new LuisClient(AppId, AzureSubscriptionKey, false);
        }

        public async Task<Intent[]> GetIntents(string input)
        {
            LuisResult res = await _client.Predict(input);

            return res.Intents;
        }

    }
}