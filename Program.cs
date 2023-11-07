using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace neworldCheckIn // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始启动服务……");
            Console.WriteLine(
                $"cronExpr表达式为：{Environment.GetEnvironmentVariable("cronExpr", EnvironmentVariableTarget.Process)} ，" +
                $"username: {Environment.GetEnvironmentVariable("userName", EnvironmentVariableTarget.Process)}," +
                $"pwd:{Environment.GetEnvironmentVariable("pwd", EnvironmentVariableTarget.Process)}");
            try
            {
                Task.Run(async () => { await SchedulerTask(); });

                while (true)
                {
                    Console.Read();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("按任意键退出……");
                Console.ReadKey();
            }
        }

        private static async Task SchedulerTask()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            //创建调度实例
            IScheduler scheduler = await sf.GetScheduler();
            //创建任务实例
            IJobDetail job = JobBuilder.Create<NewWorldCheckIn>().WithIdentity(new JobKey("NewWorldCheckIn")).Build();
            //创建触发器实例

            //读取Cron表达式
            var cronExpr = Environment.GetEnvironmentVariable("cronExpr", EnvironmentVariableTarget.Process);

            DateTime current = DateTime.UtcNow;
            ITrigger trigger = TriggerBuilder.Create().StartAt(current).WithCronSchedule(cronExpr).Build();
            Console.WriteLine(
                $"cronExpr表达式为：{cronExpr} ，下一次运行时间为(本地时间)：{trigger.GetFireTimeAfter(current)!.Value.ToLocalTime()}");

            await scheduler.ScheduleJob(job, trigger); //绑定触发器和任务

            await scheduler.Start(); //启动监控
            Console.WriteLine("NewWorldCheckIn服务已启动");
        }
    }
}