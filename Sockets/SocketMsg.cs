/************************************************************************ 
 * 项目名称 ：Sockets
 * 项目描述 ：
 * 文件名称 ：SocketMsg.cs
 * 版 本 号 ：v1.0.0.0
 * 说    明 ：
 * 作    者 ：MyPC
 * IDE 环境 ：Visual Studio 2013
 * 创建时间 ：2018/5/10 13:10:02
 * 更新时间 ：2018/5/10 13:10:02
************************************************************************
 * Copyright @ Njbosa 2018. All rights reserved.
************************************************************************/

using System.Net.Sockets;

namespace Sockets
{
    public class SocketMsg
    {
        public Socket Client { get; set; }
        public byte[] Buffer { get; set; }
    }
}
