using System;
using System.Net;
using System.Threading;
using FluentScheduler;
using RestSharp;

namespace neworldCheckIn // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        private static readonly AutoResetEvent AutoResetEvent = new(false);

        static void Main(string[] args)
        {
            Console.WriteLine("NewWorldCheckIn服务已启动");

            Console.WriteLine(
                $"cronExpr表达式为：{Environment.GetEnvironmentVariable("cronExpr", EnvironmentVariableTarget.Process)} ，" +
                $"username: {Environment.GetEnvironmentVariable("userName", EnvironmentVariableTarget.Process)}," +
                $"pwd:{Environment.GetEnvironmentVariable("pwd", EnvironmentVariableTarget.Process)}");

            JobManager.AddJob(CheckIn, t => { t.ToRunNow().AndEvery(1).Days(); });

            Console.CancelKeyPress += ((s, a) =>
            {
                Console.WriteLine("Exit");
                AutoResetEvent.Reset();
            });
            AutoResetEvent.WaitOne();
        }

        private static async void CheckIn()
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
                Console.WriteLine($"执行结束，下次执行");
            }
        }
    }
}