using System;
using System.ServiceProcess;
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using System.IO;

namespace MyWindowsService
{
    public partial class MyService : ServiceBase
    {
        //记录到event log中，在[事件查看器]可查询，地址是 C:\WINDOWS\system32\config\MyNewLog.evt (双击查看即可，文件名为MyNewLog)
        private static System.Diagnostics.EventLog eventLog1;
        private int eventId = 1;

        System.Timers.Timer _Timer;  //计时器
        private static object _LockSMS_Send = new object();

        public MyService()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MyServiceLog"))
            {
                System.Diagnostics.EventLog.CreateEventSource("MyServiceLog", "MyNewLog");
            }
            eventLog1.Source = "MyServiceLog";
            eventLog1.Log = "MyNewLog";
        }

        //"@" 让转移字符"\"保持原意，不要转义
        static string filePath = @"C:\MyServiceLog.txt"; // == "C:\\MyServiceLog.txt";

        protected override void OnStart(string[] args)
        {
            //服务开启执行代码
            eventLog1.WriteEntry("In OnStart.");
            log("In OnStart.");

            // TODO: 在此处添加代码以启动服务。
            int second = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GapDate"].ToString());
            if (second == 0) { second = 5; }

            this._Timer = new System.Timers.Timer();
            this._Timer.Interval = second * 1000;  //minute * 60 * 1000; 设置计时器事件间隔执行时间
            this._Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            this._Timer.Enabled = true;
        }

        protected override void OnStop()
        {
            //服务结束执行代码
            eventLog1.WriteEntry("In OnStop.");
            log("服务停止！");
        }

        protected override void OnPause()
        {
            //服务暂停执行代码
            base.OnPause();
        }

        protected override void OnContinue()
        {
            //服务恢复执行代码
            base.OnContinue();
        }

        protected override void OnShutdown()
        {
            //系统即将关闭执行代码
            base.OnShutdown();
        }

        /// <summary>
        /// 记录到指定路径：C:\MyServiceLog.txt
        /// </summary>
        /// <param name="message"></param>
        private static void log(string message)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using(StreamWriter writer=new StreamWriter(stream))
            {
                //writer.WriteLine($"{DateTime.Now}:{message}");
                writer.WriteLine(string.Format("{0}:{1}", DateTime.Now, message));
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
            lock(_LockSMS_Send)
            {
                DateTime t1 = DateTime.Now;
                Thread.Sleep(3000);
                log(string.Format("Timer执行时间：{0}分{1}秒", (DateTime.Now - t1).Minutes, (DateTime.Now - t1).Seconds));
            }

            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            int second = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GapDate"].ToString());
            if (second != 0 && second * 1000 != this._Timer.Interval) 
            {
                log(second.ToString());
                this._Timer.Interval = second * 1000;  //设置计时器事件间隔执行时间
            }
            
        }
    }
}
