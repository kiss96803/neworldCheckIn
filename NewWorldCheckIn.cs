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

        var userName = Environment.GetEnvironmentVariable("userName", EnvironmentVariableTarget.Process);
        var pwd = Environment.GetEnvironmentVariable("pwd", EnvironmentVariableTarget.Process);

        loginRequest.AddBody(@"email=" + userName + "&passwd=" + pwd, ContentType.FormUrlEncoded);
        var loginResponse = await client.PostAsync(loginRequest);
        if (loginResponse is {Content: not null, IsSuccessful: true})
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

            if (checkInResponse is {Content: not null, IsSuccessful: true})
                Console.WriteLine(System.Text.RegularExpressions.Regex.Unescape(checkInResponse.Content));
            Console.WriteLine($"执行结束，下次执行{context.NextFireTimeUtc!.Value.ToLocalTime()}");
        }

        await Task.Delay(10000);
    }
}