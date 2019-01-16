using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Web;

namespace facebook_music_spammer
{
    public static class HttpTriggered
    {
        private static HttpClient _httpClient = new HttpClient();
        private static string superSecret = System.Environment.GetEnvironmentVariable("test");

        [FunctionName("HttpTriggered")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var pr = "spotify:user:bambuseq:playlist:2GK6j1Wh1NfmIPcFVXC97t";

            var content = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var reqB = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            reqB.Content = content;
            reqB.Headers.Add("Authorization", "Basic thetoken!!!");

            var res = await _httpClient.SendAsync(reqB);

            var resBody = await res.Content.ReadAsAsync<dynamic>();
            var token = resBody["access_token"].ToString();
            
            log.LogInformation($"Token is {token}");

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["fields"] = "items(track(name,album(name))), next";

            string queryString = query.ToString();
            var reqTracks = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/playlists/2GK6j1Wh1NfmIPcFVXC97t/tracks?{queryString}");
            reqTracks.Headers.Add("Authorization", $"Bearer {token}");

            var ress = await _httpClient.SendAsync(reqTracks);
            var tracks = await ress.Content.ReadAsAsync<dynamic>();
            // var jsonTracks = JsonConvert.SerializeObject(tracks);
            //parse this json to get only name and artist, check if limit 100 always

            log.LogInformation($"Tracks are {Convert.ToString(tracks)}");

            //youtube api to discover
            //https://www.slickremix.com/docs/get-api-key-for-youtube/
            //key thetoken!!!
            //https://developers.google.com/youtube/v3/docs/search/list#http-request
            //https://developers.google.com/youtube/registering_an_application#Create_API_Keys
            //https://www.googleapis.com/youtube/v3/search?q=cat&part=snippet&key=thetoken!!!



            
            //https://www.apreche.net/tutorial-programatically-post-a-status-update-to-your-facebook-page/
            // how to use facebook api as fb user

            return (ActionResult)new OkObjectResult($"Hello");
        }
    }
}
