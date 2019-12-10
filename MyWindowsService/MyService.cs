using System;
using System.ServiceProcess;
using System.IO;

namespace MyWindowsService
{
    public partial class MyService : ServiceBase
    {
        System.Timers.Timer _Timer;  //计时器
        private static object _LockSMS_Send = new object();

        public MyService()
        {
            InitializeComponent();
        }

        //"@" 让转移字符"\"保持原意，不要转义
        string filePath = @"C:\MyServiceLog.txt";

        protected override void OnStart(string[] args)
        {
            //服务开启执行代码
            using (FileStream stream = new FileStream(filePath,FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                //$"是在C#6.0出现的一个新特性
                //writer.WriteLine($"{DateTime.Now},服务启动！");
                writer.WriteLine(string.Format("{0},服务启动！", DateTime.Now));
                
            }

            // TODO: 在此处添加代码以启动服务。
            int minute = 1; // Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GapDate"].ToString());
            this._Timer = new System.Timers.Timer();
            this._Timer.Interval = minute * 60 * 1000;  //设置计时器事件间隔执行时间
            this._Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            this._Timer.Enabled = true;
        }

        protected override void OnStop()
        {
            //服务结束执行代码
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                //writer.WriteLine("{DateTime.Now},服务停止！");
                writer.WriteLine(string.Format("{0},服务停止！", DateTime.Now));
            }
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

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Log.Read(DateTime.Now.ToString("-----------------------------------时 间：yyyy-MM-dd HH:mm:ss------------------------------------"));
            //DateTime t1 = DateTime.Now;
            //Log.Read(" ");
            //Log.Read(string.Format("执行时间：{0}分{1}秒", (DateTime.Now - t1).Minutes, (DateTime.Now - t1).Seconds));
            //Log.Read(" ");

            lock(_LockSMS_Send)
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Append))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    //writer.WriteLine("{DateTime.Now},执行时间！");
                    writer.WriteLine(string.Format("{0},Timer执行时间！", DateTime.Now));
                }
            }
        }
    }
}
