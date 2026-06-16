namespace Reminder
{
    partial class RestFrm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerRst = new System.Windows.Forms.Timer(this.components);
            this.lbl_seconds = new System.Windows.Forms.Label();
            this.lbl_minutes = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            // 健康提醒标签
            this.lblHealthTitle = new System.Windows.Forms.Label();
            this.lblHealthLine1 = new System.Windows.Forms.Label();
            this.lblHealthLine2 = new System.Windows.Forms.Label();
            this.lblHealthLine3 = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // timerRst
            this.timerRst.Interval = 1000;
            this.timerRst.Tick += new System.EventHandler(this.TimerRst_Tick);

            // lbl_seconds
            this.lbl_seconds.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lbl_seconds.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_seconds.ForeColor = System.Drawing.Color.White;
            this.lbl_seconds.Location = new System.Drawing.Point(536, 253);
            this.lbl_seconds.Name = "lbl_seconds";
            this.lbl_seconds.Size = new System.Drawing.Size(100, 55);
            this.lbl_seconds.TabIndex = 0;

            // lbl_minutes
            this.lbl_minutes.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lbl_minutes.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_minutes.ForeColor = System.Drawing.Color.White;
            this.lbl_minutes.Location = new System.Drawing.Point(472, 253);
            this.lbl_minutes.Name = "lbl_minutes";
            this.lbl_minutes.Size = new System.Drawing.Size(54, 55);
            this.lbl_minutes.TabIndex = 1;
            this.lbl_minutes.Text = "  ";

            // label1 (冒号)
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(519, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 79);
            this.label1.TabIndex = 2;
            this.label1.Text = ":";

            // lblText
            this.lblText.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblText.AutoSize = true;
            this.lblText.Font = new System.Drawing.Font("楷体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblText.ForeColor = System.Drawing.Color.White;
            this.lblText.Location = new System.Drawing.Point(82, 189);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(509, 29);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "键盘和鼠标被锁定，站起来活动下！";
            this.lblText.Click += new System.EventHandler(this.lblText_Click);

            // label2 (运动图标1)
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.Image = global::Reminder.Properties.Resources.sport_64_01;
            this.label2.Location = new System.Drawing.Point(332, 470);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 104);
            this.label2.TabIndex = 4;

            // label3
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.Font = new System.Drawing.Font("楷体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(180, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 29);
            this.label3.TabIndex = 5;
            this.label3.Text = "解锁倒计时：";

            // label5 (坐姿图标)
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.Image = global::Reminder.Properties.Resources.sit_64;
            this.label5.Location = new System.Drawing.Point(207, 470);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 104);
            this.label5.TabIndex = 7;

            // label4 (运动图标3)
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.Image = global::Reminder.Properties.Resources.sport_64_03;
            this.label4.Location = new System.Drawing.Point(591, 470);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 104);
            this.label4.TabIndex = 8;

            // label6
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(318, 510);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 54);
            this.label6.TabIndex = 9;
            this.label6.Text = ">";

            // label7
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(438, 510);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 54);
            this.label7.TabIndex = 10;
            this.label7.Text = ">";

            // label8 (运动图标2)
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label8.Image = global::Reminder.Properties.Resources.sport_64_02;
            this.label8.Location = new System.Drawing.Point(460, 470);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 104);
            this.label8.TabIndex = 11;

            // label9
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label9.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(582, 510);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 54);
            this.label9.TabIndex = 12;
            this.label9.Text = ">";

            // ========== 健康提醒标签（全新注入 v2） ==========

            // lblHealthTitle - "人民日报提醒："
            this.lblHealthTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblHealthTitle.AutoSize = true;
            this.lblHealthTitle.BackColor = System.Drawing.Color.Green;
            this.lblHealthTitle.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHealthTitle.ForeColor = System.Drawing.Color.White;
            this.lblHealthTitle.Location = new System.Drawing.Point(250, 310);
            this.lblHealthTitle.Name = "lblHealthTitle";
            this.lblHealthTitle.Size = new System.Drawing.Size(400, 26);
            this.lblHealthTitle.TabIndex = 13;
            this.lblHealthTitle.Text = "人民日报提醒：";
            this.lblHealthTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // lblHealthLine0 - "如果用一串数字来量化"久坐"的后果，大概是这样的："
            this.lblHealthLine0 = new System.Windows.Forms.Label();
            this.lblHealthLine0.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblHealthLine0.AutoSize = true;
            this.lblHealthLine0.BackColor = System.Drawing.Color.Green;
            this.lblHealthLine0.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHealthLine0.ForeColor = System.Drawing.Color.White;
            this.lblHealthLine0.Location = new System.Drawing.Point(150, 345);
            this.lblHealthLine0.Name = "lblHealthLine0";
            this.lblHealthLine0.Size = new System.Drawing.Size(600, 26);
            this.lblHealthLine0.TabIndex = 17;
            this.lblHealthLine0.Text = "如果用一串数字来量化\u201C久坐\u201D的后果，大概是这样的：";
            this.lblHealthLine0.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // lblHealthLine1 - 第一条
            this.lblHealthLine1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblHealthLine1.AutoSize = true;
            this.lblHealthLine1.BackColor = System.Drawing.Color.Green;
            this.lblHealthLine1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHealthLine1.ForeColor = System.Drawing.Color.White;
            this.lblHealthLine1.Location = new System.Drawing.Point(180, 381);
            this.lblHealthLine1.Name = "lblHealthLine1";
            this.lblHealthLine1.Size = new System.Drawing.Size(600, 21);
            this.lblHealthLine1.TabIndex = 14;
            this.lblHealthLine1.Text = "\u2022 久坐1小时的危害\u2248抽2根烟\u2248减寿22分钟（源：澳大利亚昆士兰大学）";
            this.lblHealthLine1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // lblHealthLine2 - 第二条
            this.lblHealthLine2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblHealthLine2.AutoSize = true;
            this.lblHealthLine2.BackColor = System.Drawing.Color.Green;
            this.lblHealthLine2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHealthLine2.ForeColor = System.Drawing.Color.White;
            this.lblHealthLine2.Location = new System.Drawing.Point(180, 408);
            this.lblHealthLine2.Name = "lblHealthLine2";
            this.lblHealthLine2.Size = new System.Drawing.Size(600, 21);
            this.lblHealthLine2.TabIndex = 15;
            this.lblHealthLine2.Text = "\u2022 每天久坐超6小时，总体早亡风险增加19%（源：美国癌症协会21年跟踪研究）";
            this.lblHealthLine2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // lblHealthLine3 - 第三条
            this.lblHealthLine3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblHealthLine3.AutoSize = true;
            this.lblHealthLine3.BackColor = System.Drawing.Color.Green;
            this.lblHealthLine3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHealthLine3.ForeColor = System.Drawing.Color.White;
            this.lblHealthLine3.Location = new System.Drawing.Point(180, 435);
            this.lblHealthLine3.Name = "lblHealthLine3";
            this.lblHealthLine3.Size = new System.Drawing.Size(600, 21);
            this.lblHealthLine3.TabIndex = 16;
            this.lblHealthLine3.Text = "\u2022 久坐时，下肢肌肉基本关闭，新陈代谢降50%，脂肪分解酶降90%";
            this.lblHealthLine3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // RestFrm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(905, 620);
            this.Controls.Add(this.lblHealthLine3);
            this.Controls.Add(this.lblHealthLine2);
            this.Controls.Add(this.lblHealthLine1);
            this.Controls.Add(this.lblHealthLine0);
            this.Controls.Add(this.lblHealthTitle);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_minutes);
            this.Controls.Add(this.lbl_seconds);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RestFrm";
            this.Opacity = 0.75D;
            this.ShowInTaskbar = false;
            this.Text = "歇歇眼睛";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RestFrm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RestFrm_FormClosed);
            this.Load += new System.EventHandler(this.RestFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Timer timerRst;
        private System.Windows.Forms.Label lbl_seconds;
        private System.Windows.Forms.Label lbl_minutes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        // 健康提醒 label
        private System.Windows.Forms.Label lblHealthTitle;
        private System.Windows.Forms.Label lblHealthLine0;
        private System.Windows.Forms.Label lblHealthLine1;
        private System.Windows.Forms.Label lblHealthLine2;
        private System.Windows.Forms.Label lblHealthLine3;
    }
}
