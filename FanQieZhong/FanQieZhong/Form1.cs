using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace FanQieZhong
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        //设置窗体句柄的窗体为活动窗体，因为到后面如果此程序并不在焦点中，可以是此程序回归活动窗体
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private int Timer;//用户设置的倒计时的时间
        private DateTime startTime;//用来保存开始番茄钟的时间
        private bool flag=false;//用来控制是否保存数据到本地
        int hours;//用来保存转换后的小时数
        int min;//保存分钟数
        int second;//保存秒数
        public Form1()
        {
            InitializeComponent();
            label3.Text = 0.ToString();
            checkBox1.Checked = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            flag = checkBox1.Checked;//开始时获取用户是否要保存文件到本地
            startTime = DateTime.Now.ToLocalTime();//记录此次番茄钟开始时间，用来后期数据保存
            Timer = Convert.ToInt32(textBox1.Text)*60;
            button2.Text = "暂停计时";
            timer1.Enabled = true;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = Timer;
            progressBar1.Step = 1;
        }
        /// <summary>
        /// 用计时器来控制实现 时间的减少
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(Timer>0)
            {
                Timer -= 1;
                hours=Timer / 3600;
                min = (Timer - 3600 * hours) / 60;
                second = Timer % 60;
                label5.Text = hours.ToString() + "h" + min.ToString() + "m" + second.ToString() + "s";
                progressBar1.Value = Timer;
            }
            else
            {
                timer1.Enabled = false;
                SetForegroundWindow(this.Handle);
                MessageBox.Show("时间到了，休息一会吧^_^");
                SaveDate();
            }
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveDate()
        {
            int i = Convert.ToInt32(label3.Text);
            i += 1;
            label3.Text = i.ToString();
            if(flag)
            {
                string directoryPath = @"C:\TomatoDate";//定义一个路径变量
                string filePath = "TomatoDate.txt";//定义一个文件路径变量
                if (!Directory.Exists(directoryPath))//如果路径不存在
                {
                    Directory.CreateDirectory(directoryPath);//创建一个路径的文件夹
                }
                StreamWriter sw = new StreamWriter(Path.Combine(directoryPath, filePath),true);//打开文件，并设定为追加数据
                sw.WriteLine("开始时间"+startTime.ToString()+"——结束时间"+DateTime.Now.ToLocalTime().ToString()+"专注时间"+textBox1.Text.ToString());
                sw.WriteLine("——————————————————————————————————————————————————————————————");
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// 控制暂停和恢复计时 按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {
            if(button2.Text=="暂停计时")
            {
                timer1.Stop();
                button2.Text = "恢复计时";
            }
            else
            {
                timer1.Start();
                button2.Text = "暂停计时";
            }
        }
        /// <summary>
        /// 放弃本次后将一切回归初始值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button2.Text = "暂停计时";
            label5.Text = "00h00m00s";
            Timer = 0;
            progressBar1.Value = 0;
        }
    }
}
