using System;
using System.ServiceProcess;
using System.IO;

namespace MyWindowsService
{
    public partial class MyService : ServiceBase
    {
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
    }
}
