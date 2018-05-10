using System;
using System.Messaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace Message
{
    public delegate bool ControlCtrlDelegate(int CtrlType);
    class Program
    {
        [DllImport("kernel32.dll")]                                                                             // 声明和实现的连接
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);         // 外部方法
        static ControlCtrlDelegate newDelegate = new ControlCtrlDelegate(HandlerRoutine);                       // 创建关闭事件委托
        public static bool HandlerRoutine(int CtrlType)
        {
            Close();
            return false;
        }

        const string path = @".\private$\myQueue";
        //string path = @"FormatName:Direct=TCP:192.168.1.153\private$\queuedemo";
        static void Main(string[] args)
        {
            bool bRet = SetConsoleCtrlHandler(newDelegate, true);
            MessageQueue msmq;
            if (!MessageQueue.Exists(path))
                msmq = MessageQueue.Create(path);
            else
            {
                msmq = new MessageQueue(path);
            }
            Thread th = new Thread(new ThreadStart(new Action(() =>
            {
                int i = 0;
                while (true)
                {
                    i++;
                    Send(string.Format("第{0}次", i));
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            })));
            th.IsBackground = true;
            th.Start();
            Recive();
            Console.ReadLine();
        }

        static void Send(string text)
        {
            if (!MessageQueue.Exists(path))
                return;
            MessageQueue msmq = new MessageQueue(path);
            System.Messaging.Message msg = new System.Messaging.Message();
            msg.Body = text;
            msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            msmq.Send(msg);
        }

        static void Recive()
        {
            if (!MessageQueue.Exists(path))
                return;
            MessageQueue msmq = new MessageQueue(path);
            msmq.Formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(string) });
            Thread th = new Thread(() =>
            {
                while (true)
                {
                    System.Messaging.Message msg = msmq.Receive();
                    if (msg != null)
                    {
                        Console.WriteLine(msg.Body.ToString());
                    }

                }
            });
            th.IsBackground = true;
            th.Start();
        }

        static void Close()
        {
            if (MessageQueue.Exists(path))
            {
                MessageQueue msmq = new MessageQueue(path);
                msmq.Purge(); 
                MessageQueue.Delete(path);
            }
        }
    }
}
