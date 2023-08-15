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

            try
            {
                Task.Run(async () =>
                {
                    await SchedulerTask();
                });

                while (true)
                {
                    Console.ReadKey();
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
            string cronExpr = "0 1 20,0 * * ?";
            ITrigger trigger = TriggerBuilder.Create().StartAt(DateTime.Now).WithCronSchedule(cronExpr).Build();
            await scheduler.ScheduleJob(job, trigger);     //绑定触发器和任务
            await scheduler.Start();   //启动监控
            Console.WriteLine("NewWorldCheckIn服务已启动");
        }

    }
}