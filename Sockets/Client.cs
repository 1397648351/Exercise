/************************************************************************ 
 * 项目名称 ：Sockets
 * 项目描述 ：
 * 文件名称 ：Client.cs
 * 版 本 号 ：v1.0.0.0
 * 说    明 ：
 * 作    者 ：MyPC
 * IDE 环境 ：Visual Studio 2013
 * 创建时间 ：2018/5/10 12:12:50
 * 更新时间 ：2018/5/10 12:12:50
************************************************************************
 * Copyright @ Njbosa 2018. All rights reserved.
************************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Sockets
{
    public partial class Client : Form
    {
        Socket client;
        public Client()
        {
            InitializeComponent();
            this.Load += new EventHandler(Client_Load);
        }

        private void Client_Load(object sender, EventArgs e)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);                                   // 初始化soket对象
            client.Connect(IPAddress.Parse("127.0.0.1"), 4399);                                                                     // 连接服务端
            SocketMsg clientMsg = new SocketMsg()
            {
                Client = client,
                Buffer = new byte[1024]
            };
            // 开始接收信息
            client.BeginReceive(clientMsg.Buffer, 0, clientMsg.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientMsg);
        }

        // 发送文本
        private void btn_send_Click(object sender, EventArgs e)
        {
            string msg = txt_send.Text;
            SendMsg(msg);
        }

        // 关闭Socket
        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client == null)
                return;

            if (!client.Connected)
                return;

            try
            {
                client.Shutdown(SocketShutdown.Both);
            }
            catch { }
            try
            {
                client.Close();
            }
            catch { }
        }

        // 异步接收信息
        private void ReceiveCallback(IAsyncResult result)
        {
            SocketMsg clientMsg = (SocketMsg)result.AsyncState;
            int len = clientMsg.Client.EndReceive(result);                                                                          // 读取到缓存，获取到的字节数
            result.AsyncWaitHandle.Close();
            byte[] buffer = new byte[len];
            Array.Copy(clientMsg.Buffer, buffer, len);                                                                              // 有效字节
            ShowMsg(string.Format("收到消息：{0}", Encoding.UTF8.GetString(buffer)));
            clientMsg.Buffer = new byte[clientMsg.Buffer.Length];                                                                   //清空数据，重新开始异步接收
            // 开始接收信息
            clientMsg.Client.BeginReceive(clientMsg.Buffer, 0, clientMsg.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientMsg);
        }

        // 发送消息
        private void SendMsg(string msg)
        {
            Byte[] b_send = System.Text.Encoding.UTF8.GetBytes(msg);
            client.Send(b_send, b_send.Length, SocketFlags.None);
        }

        // 显示信息
        private delegate void ShowMsgCallback(string text);
        private void ShowMsg(string text)
        {
            // InvokeRequired需要比较调用线程ID和创建线程ID
            // 如果它们不相同则返回true
            if (txt_rec.InvokeRequired)
            {
                ShowMsgCallback d = new ShowMsgCallback(ShowMsg);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                txt_rec.AppendText(text + "\r\n");
            }
        }
    }
}
