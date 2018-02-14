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
using System.Runtime.InteropServices;

namespace FanQieZhong
{
    public partial class Form1 : Form
    {
        #region 引用外部Dll文件的函数
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        //设置窗体句柄的窗体为活动窗体，因为如果此程序并不在焦点中，可以使此程序回归活动窗体
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        //释放按钮
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        //向某个窗体发送某种信息
        #endregion

        private int Timer;//用户设置的倒计时的时间
        private DateTime startTime;//用来保存开始番茄钟的时间
        private bool flag=false;//用来控制是否保存数据到本地
        private bool isrun = false;//用来控制番茄钟是否在运行状态
        private const float op = 0.3f;
        int hours;//用来保存转换后的小时数
        int min;//保存分钟数
        int second;//保存秒数
        public Form1()
        {
            InitializeComponent();
            label3.Text = 0.ToString();
            checkBox1.Checked = true;
            checkBox2.Checked = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            #region 初始化一些参数
            flag = checkBox1.Checked;//开始时获取用户是否要保存文件到本地
            startTime = DateTime.Now.ToLocalTime();//记录此次番茄钟开始时间，用来后期数据保存
            Timer = Convert.ToInt32(textBox1.Text)*60;
            button2.Text = "暂停计时";
            timer1.Enabled = true;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = Timer;
            progressBar1.Step = 1;
            isrun = true;
            #endregion

            #region 番茄钟运行时变为半透明无边框样式
            this.FormBorderStyle = FormBorderStyle.None;//窗体无边框
            this.Opacity = op;
            #endregion
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
            isrun = false;
            this.Opacity = 1;
            this.FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            if(isrun)
            {
                this.Opacity = 1;
            }     
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            if(isrun)
            {
                this.Opacity = op;
            }
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked)
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
        }
        /// <summary>
        /// 当用户在窗体界面按下鼠标后，先释放鼠标，然后在向此窗体发送一条鼠标在标题栏按下的消息，以此来骗过窗体
        /// 以此来实现无边框窗体的移动。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        { 
            int WM_SYSCOMMAND = 0x0112;
            int SC_MOVE = 0xF010;
            int HTCAPTION = 0x0002;
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
    }
}
