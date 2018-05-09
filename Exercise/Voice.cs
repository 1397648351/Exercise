/************************************************
 * 项目名称 ：Exercise   
 * 项目描述 ：
 * 文件名称 ：Voice.cs
 * 版 本 号 ：v1.0.0.0  
 * 说    明 ：
 * 作    者 ：MyPC 
 * 创建时间 ：2018/5/9 21:15:49 
 * 更新时间 ：2018/5/9 21:15:49 
*************************************************/

using System;
using System.Windows.Forms;
using SpeechLib;
using System.Speech.Synthesis;
using System.Threading;
using System.Collections.Generic;

namespace Exercise
{
    public partial class Voice : Form
    {
        private SpeechSynthesizer synth;                                                   // Synthesis对象
        public Voice()
        {
            InitializeComponent();
            this.Load += new EventHandler(Voice_Load);                                     // 窗口加载事件
            this.Shown += new EventHandler(Voice_Shown);                                   // 窗口显示事件
            this.FormClosing += new FormClosingEventHandler(Voice_FormClosing);            // 窗口关闭事件
        }

        // 异步合成语音
        private void Speak(string text)
        {
            synth.SpeakAsync(text);
        }

        // 窗口加载事件
        private void Voice_Load(object sender, EventArgs e)
        {
            synth = new SpeechSynthesizer();
            synth.Rate = -1;                                                                // 语速-10到10
            synth.Volume = 100;                                                             // 音量0到100
        }

        // 窗口显示事件
        private void Voice_Shown(object sender, EventArgs e)
        {
            Speak("请输入文字");
        }

        // 窗口关闭事件
        private void Voice_FormClosing(object sender, FormClosingEventArgs e)
        {
            synth.SpeakAsyncCancelAll();                                                    // 停止合成
            synth.Dispose();                                                                // 释放资源
        }

        // 按钮点击事件
        private void btn_ok_Click(object sender, EventArgs e)
        {
            string text = txt_msg.Text;
            if (string.IsNullOrEmpty(text))
            {
                Speak("请输入文字");
                return;
            }
            Speak(txt_msg.Text);
        }
    }
}
