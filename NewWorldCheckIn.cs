using System;
using System.Net;
using System.Threading.Tasks;
using Quartz;
using RestSharp;

namespace neworldCheckIn;

public class NewWorldCheckIn : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Start NewWorldCheckIn……");

        var options = new RestClientOptions("https://neworld.space");
        var client = new RestClient(options);

        var loginRequest = new RestRequest("/auth/login");

        loginRequest.AddBody(@"email=463936274@qq.com&passwd=kisskiss", ContentType.FormUrlEncoded);
        var loginResponse = await client.PostAsync(loginRequest);
        if (loginResponse.Content != null && loginResponse.IsSuccessful)
        {
            Console.WriteLine(System.Text.RegularExpressions.Regex.Unescape(loginResponse.Content));

            var checkInRequest = new RestRequest("/user/checkin");

            if (loginResponse.Cookies != null)
            {
                foreach (Cookie loginResponseCookie in loginResponse.Cookies)
                {
                    checkInRequest.AddCookie(loginResponseCookie.Name, loginResponseCookie.Value,
                        loginResponseCookie.Path, loginResponseCookie.Domain);
                }
            }

            var checkInResponse = await client.PostAsync(checkInRequest);

            if (checkInResponse.Content != null && checkInResponse.IsSuccessful)
                Console.WriteLine(System.Text.RegularExpressions.Regex.Unescape(checkInResponse.Content));
        }

        await Task.Delay(10000);
    }
}