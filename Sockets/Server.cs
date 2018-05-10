/************************************************************************ 
 * 项目名称 ：Sockets
 * 项目描述 ：
 * 文件名称 ：Server.cs
 * 版 本 号 ：v1.0.0.0
 * 说    明 ：
 * 作    者 ：WuZe
 * IDE 环境 ：Visual Studio 2013
 * 创建时间 ：2018/5/10 12:09:50
 * 更新时间 ：2018/5/10 12:09:50
************************************************************************
 * Copyright @ Njbosa 2018. All rights reserved.
************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Sockets
{
    public partial class Server : Form
    {
        private EndPoint endPoint;
        private List<SocketMsg> list;
        private Socket server;

        public Server()
        {
            InitializeComponent();
            this.Load += new EventHandler(Server_Load);
            this.FormClosing += new FormClosingEventHandler(Server_FormClosing);
        }
        
        // 关闭Socket
        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (server == null)
                return;

            if (!server.Connected)
                return;

            try
            {
                server.Shutdown(SocketShutdown.Both);
            }
            catch { }
            try
            {
                server.Close();
            }
            catch { }
        }

        private void Server_Load(object sender, EventArgs e)
        {
            InitSocket();
            Client form_CLient = new Client();
            form_CLient.Show();
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            string msg = txt_send.Text;
            for (int i = 0; i < list.Count; i++)
            {
                SendMsg(list[i].Client, msg);
            }
        }

        private void InitSocket()
        {
            list = new List<SocketMsg>();
            endPoint = new IPEndPoint(IPAddress.Any, 4399);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);    // 初始化soket对象
            try
            {
                server.Bind(endPoint);                                                                      // 绑定 0.0.0.0:4399
                server.Listen(10);                                                                          // 开始监听
                ShowMsg("服务器开始监听");
                server.BeginAccept(new AsyncCallback(AcceptCallBack), server);                              // 异步等待连接
            }
            catch (Exception e)
            {
                ShowMsg(e.Message);
            }
        }

        //异步等待连接
        private void AcceptCallBack(IAsyncResult result)
        {
            //server = (Socket)result.AsyncState;
            Socket client = server.EndAccept(result);                                                       // 获取到连接的对象
            ShowMsg(client.RemoteEndPoint.ToString() + "已连接");
            string msg = "欢迎你，" + client.RemoteEndPoint.ToString();
            SendMsg(client, msg);                                                                           // 发送欢迎信息
            SocketMsg clientMsg = new SocketMsg()
            {
                Client = client,
                Buffer = new byte[1024]
            };
            // 开始接受客户端发送的信息
            clientMsg.Client.BeginReceive(clientMsg.Buffer, 0, clientMsg.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientMsg);
            list.Add(clientMsg);                                                                            // 添加一个客户端对象
            server.BeginAccept(new AsyncCallback(AcceptCallBack), server);                                  // 等待下一个连接
        }

        // 异步接收信息
        private void ReceiveCallback(IAsyncResult result)
        {
            SocketMsg clientMsg = (SocketMsg)result.AsyncState;
            int len = clientMsg.Client.EndReceive(result);                                                  // 读取到缓存，获取到的字节数
            result.AsyncWaitHandle.Close();
            byte[] buffer = new byte[len];
            Array.Copy(clientMsg.Buffer, buffer, len);                                                      // 有效字节
            ShowMsg(string.Format("收到消息：{0}", Encoding.UTF8.GetString(buffer)));
            clientMsg.Buffer = new byte[clientMsg.Buffer.Length];                                           //清空数据，重新开始异步接收
            // 开始下一次接收
            clientMsg.Client.BeginReceive(clientMsg.Buffer, 0, clientMsg.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientMsg);
        }

        // 发送消息
        private void SendMsg(Socket client, string msg)
        {
            Byte[] b_send = System.Text.Encoding.UTF8.GetBytes(msg);                                        // 转成字节流
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
