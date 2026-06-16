using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Reminder
{
    public partial class RestFrm : Form
    {
        private EyeTracker _eyeTracker;

        private int rst_m;
        private int wrk_m;
        private int rst_m2;
        private bool input_flag;
        int rst_s = 0;

        // 多屏子实例
        private List<RestFrmChild> childForms;

        public RestFrm()
        {
            InitializeComponent();
        }

        public RestFrm(int rst_minutes, int wrk_minutes, bool input_flag)
        {
            InitializeComponent();
            this.rst_m = rst_minutes;
            this.wrk_m = wrk_minutes;
            this.rst_m2 = rst_minutes;
            this.input_flag = input_flag;
            this.childForms = new List<RestFrmChild>();
        }

        /// <summary>
        /// 为每块非主屏幕创建一个全屏遮罩子实例
        /// </summary>
        private void CreateMultiScreenCovers()
        {
            Rectangle myBounds = this.Bounds;
            foreach (Screen s in Screen.AllScreens)
            {
                // 跳过主实例已经覆盖的那块屏幕
                if (s.Bounds.X == myBounds.X && s.Bounds.Y == myBounds.Y &&
                    s.Bounds.Width == myBounds.Width && s.Bounds.Height == myBounds.Height)
                {
                    continue;
                }

                RestFrmChild child = new RestFrmChild(s);
                child.TopMost = true;
                child.Show();
                childForms.Add(child);
            }
        }

        /// <summary>
        /// 关闭所有子遮罩实例
        /// </summary>
        private void CloseChildForms()
        {
            foreach (RestFrmChild child in childForms)
            {
                if (child != null && !child.IsDisposed)
                {
                    try { child.Close(); } catch { }
                }
            }
            childForms.Clear();
        }

        private void RestFrm_Load(object sender, EventArgs e)
        {
            if (input_flag)
            {
                lblText.Text = "您已久坐" + wrk_m.ToString() + "分钟了，键盘和鼠标被锁定，站起来活动下！";
            }
            else
            {
                lblText.Text = "您已久坐" + wrk_m.ToString() + "分钟了，站起来活动下！Alt+F4 退出本界面。";
            }

            timerRst.Enabled = true;
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
            this.Opacity = 0.75;

            if (input_flag)
            {
                KeyboardBlocker.off(); // 锁定键盘
            }

            // 创建多屏子遮罩

            // 增量：启动视线追踪练习模块（30秒焦點球）
            _eyeTracker = new EyeTracker(this);
            _eyeTracker.Start();
            CreateMultiScreenCovers();

            // 倒计时显示
            if (rst_s >= 10)
                lbl_seconds.Text = rst_s.ToString();
            else
                lbl_seconds.Text = "0" + rst_s.ToString();

            if (rst_m >= 10)
                lbl_minutes.Text = rst_m.ToString();
            else
                lbl_minutes.Text = "0" + rst_m.ToString();
        }

        private void TimerRst_Tick(object sender, EventArgs e)
        {
            timing();
        }

        private void timing()
        {
            if (rst_s > 0)
            {
                rst_s = rst_s - 1;
                if (rst_s >= 10)
                    lbl_seconds.Text = rst_s.ToString();
                else
                    lbl_seconds.Text = "0" + rst_s.ToString();
            }
            else
            {
                timerRst.Enabled = false;
                rst_m--;
                if (rst_m >= 10)
                    lbl_minutes.Text = rst_m.ToString();
                else
                    lbl_minutes.Text = "0" + rst_m.ToString();

                if (rst_m > -1)
                {
                    timerRst.Enabled = true;
                    rst_s = 59;
                    timing();
                }
                else
                {
                    if (input_flag)
                    {
                        KeyboardBlocker.on(); // 解锁键盘
                    }

                    // 关闭所有子屏遮罩
                    CloseChildForms();
            if (_eyeTracker != null) _eyeTracker.Stop();

                    if (rst_s == 0)
                    {
                        WorkFrm workFrm = new WorkFrm(wrk_m, rst_m2, input_flag);
                        workFrm.Show();
                    }
                    this.Close();
                }
            }
        }

        private void RestFrm_FormClosed(object sender, FormClosedEventArgs e) { }

        private void RestFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseChildForms();
            if (_eyeTracker != null) _eyeTracker.Stop();
        }

        private void lblText_Click(object sender, EventArgs e) { }
    }

    /// <summary>
    /// 子屏幕全屏遮罩窗体
    /// </summary>
    public class RestFrmChild : Form
    {
        public RestFrmChild(Screen screen)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.Bounds = screen.Bounds;
            this.BackColor = Color.Green;
            this.Opacity = 0.75;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;

            // 子屏幕只显示"请休息"文字（占用最小）
            Label lbl = new Label();
            lbl.Text = "请起身活动一下！";
            lbl.ForeColor = Color.White;
            lbl.BackColor = Color.Green;
            lbl.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Bold);
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl.Dock = DockStyle.Fill;
            this.Controls.Add(lbl);
        }
    }
}
