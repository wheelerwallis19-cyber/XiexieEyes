using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Reminder
{
    /// <summary>
    /// 眼部保健——视线追踪练习焦点球模块
    /// 独立 TopMost 透明遮罩窗体，独立于 RestFrm 的渲染链
    /// </summary>
    public class EyeTracker
    {
        private Form _overlayForm;
        private Timer _moveTimer;
        private Timer _durationTimer;
        private bool _active = false;
        private bool _finished = false;

        // 焦点球状态
        private float _angle = 0f;
        private float _ballRadius;
        private float _pathRadiusX;
        private float _pathRadiusY;
        private float _speed;
        private int _elapsedSeconds = 0;

        // 画布
        private Bitmap _buffer;
        private readonly object _lock = new object();

        // 窗口尺寸
        private int _windowWidth;
        private int _windowHeight;
        private Rectangle _screenBounds;

        public EyeTracker()
        {
        }

        public void Start()
        {
            if (_active) return;
            _active = true;
            _finished = false;
            _elapsedSeconds = 0;
            _angle = 0f;

            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            _screenBounds = bounds;
            _windowWidth = bounds.Width;
            _windowHeight = bounds.Height;

            // 椭圆路径：加大到更好的视觉比例
            _pathRadiusX = bounds.Width * 0.32f;   // 水平占64%跨度
            _pathRadiusY = bounds.Height * 0.28f;  // 垂直占56%跨度
            _ballRadius = Math.Min(bounds.Width, bounds.Height) * 0.025f;
            if (_ballRadius < 14f) _ballRadius = 14f;
            if (_ballRadius > 40f) _ballRadius = 40f;

            _speed = (float)(Math.PI * 6.0 / 300.0);
            if (_speed < 0.02f) _speed = 0.02f;

            // 预分配缓冲区
            _buffer = new Bitmap(_windowWidth, _windowHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // 创建独立 overlay 窗体
            _overlayForm = new Form();
            _overlayForm.FormBorderStyle = FormBorderStyle.None;
            _overlayForm.Bounds = bounds;
            _overlayForm.StartPosition = FormStartPosition.Manual;
            _overlayForm.TopMost = true;
            _overlayForm.TopLevel = true;
            _overlayForm.ShowInTaskbar = false;
            _overlayForm.BackColor = Color.Black;
            _overlayForm.TransparencyKey = Color.Black; // 黑色全透明
            _overlayForm.Opacity = 1.0;
            _overlayForm.AllowTransparency = true;
            // _overlayForm.DoubleBuffered is protected; default is OK
            _overlayForm.Paint += OnOverlayPaint;

            // 禁止点击穿透
            _overlayForm.Show();

            // 移动定时器
            _moveTimer = new Timer();
            _moveTimer.Interval = 100;
            _moveTimer.Tick += OnMoveTick;
            _moveTimer.Start();

            // 计时器
            _durationTimer = new Timer();
            _durationTimer.Interval = 1000;
            _durationTimer.Tick += OnDurationTick;
            _durationTimer.Start();

            // 渲染第一帧
            RenderFrame(_angle);
            _overlayForm.Invalidate();
        }

        public void Stop()
        {
            if (!_active) return;
            _active = false;
            _finished = true;

            if (_moveTimer != null)
            {
                _moveTimer.Stop();
                _moveTimer.Dispose();
                _moveTimer = null;
            }
            if (_durationTimer != null)
            {
                _durationTimer.Stop();
                _durationTimer.Dispose();
                _durationTimer = null;
            }

            if (_overlayForm != null && !_overlayForm.IsDisposed)
            {
                _overlayForm.Paint -= OnOverlayPaint;
                _overlayForm.Close();
                _overlayForm.Dispose();
                _overlayForm = null;
            }

            lock (_lock)
            {
                if (_buffer != null)
                {
                    _buffer.Dispose();
                    _buffer = null;
                }
            }
        }

        public bool IsActive() { return _active; }
        public bool IsFinished() { return _finished; }

        private void OnMoveTick(object sender, EventArgs e)
        {
            if (!_active)
            {
                Stop();
                return;
            }

            _angle += _speed;
            if (_angle > (float)(Math.PI * 2.0))
                _angle -= (float)(Math.PI * 2.0);

            RenderFrame(_angle);
            if (_overlayForm != null && !_overlayForm.IsDisposed)
                _overlayForm.Invalidate();
        }

        private void OnDurationTick(object sender, EventArgs e)
        {
            _elapsedSeconds++;
            if (_elapsedSeconds >= 30)
            {
                Stop();
                _finished = true;
            }
        }

        /// <summary>
        /// 根据角度渲染一帧到缓冲区
        /// </summary>
        private void RenderFrame(float currentAngle)
        {
            if (_buffer == null) return;

            float cx = _windowWidth / 2f;
            float cy = _windowHeight / 2f;

            float ballX = cx + (float)Math.Cos((double)currentAngle) * _pathRadiusX;
            float ballY = cy + (float)Math.Sin((double)currentAngle) * _pathRadiusY;

            lock (_lock)
            {
                using (Graphics g = Graphics.FromImage(_buffer))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);

                    // ----- 虚线路径指引 -----
                    using (Pen dotPen = new Pen(Color.FromArgb(60, 220, 220, 220), 1.5f))
                    {
                        dotPen.DashStyle = DashStyle.Dash;
                        g.DrawEllipse(dotPen,
                            cx - _pathRadiusX, cy - _pathRadiusY,
                            _pathRadiusX * 2, _pathRadiusY * 2);
                    }

                    // ----- 拖尾 (2级) -----
                    float trailAngle1 = currentAngle - _speed * 4;
                    float trailAngle2 = currentAngle - _speed * 8;
                    float tx1 = cx + (float)Math.Cos((double)trailAngle1) * _pathRadiusX;
                    float ty1 = cy + (float)Math.Sin((double)trailAngle1) * _pathRadiusY;
                    float tx2 = cx + (float)Math.Cos((double)trailAngle2) * _pathRadiusX;
                    float ty2 = cy + (float)Math.Sin((double)trailAngle2) * _pathRadiusY;

                    using (Brush b1 = new SolidBrush(Color.FromArgb(80, 255, 200, 100)))
                    using (Brush b2 = new SolidBrush(Color.FromArgb(30, 255, 200, 100)))
                    {
                        g.FillEllipse(b1, tx1 - _ballRadius * 0.7f, ty1 - _ballRadius * 0.7f,
                            _ballRadius * 1.4f, _ballRadius * 1.4f);
                        g.FillEllipse(b2, tx2 - _ballRadius * 0.5f, ty2 - _ballRadius * 0.5f,
                            _ballRadius * 1.0f, _ballRadius * 1.0f);
                    }

                    // ----- 球体主体 (渐变金黄) -----
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        gp.AddEllipse(ballX - _ballRadius, ballY - _ballRadius,
                            _ballRadius * 2, _ballRadius * 2);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = Color.FromArgb(255, 255, 240, 150);
                            pgb.SurroundColors = new Color[] { Color.FromArgb(220, 255, 180, 60) };
                            pgb.CenterPoint = new PointF(ballX - _ballRadius * 0.3f, ballY - _ballRadius * 0.3f);
                            g.FillEllipse(pgb, ballX - _ballRadius, ballY - _ballRadius,
                                _ballRadius * 2, _ballRadius * 2);
                        }
                    }

                    // ----- 高光 -----
                    float hl = _ballRadius * 0.4f;
                    using (Brush hb = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                    {
                        g.FillEllipse(hb,
                            ballX - _ballRadius * 0.4f, ballY - _ballRadius * 0.45f,
                            hl, hl * 0.7f);
                    }

                    // ----- 中心点 -----
                    using (Brush db = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                    {
                        g.FillEllipse(db, ballX - 1.5f, ballY - 1.5f, 3, 3);
                    }

                    // ----- 右下角提示文字（避开倒计时区域）-----
                    //倒计时位置：右上角或左下角——我们用右下偏下
                    string hint;
                    if (_elapsedSeconds < 30 && !_finished)
                    {
                        int rem = 30 - _elapsedSeconds;
                        hint = "\u89c6\u7ebf\u8ffd\u8e2a\u7ec3\u4e60 (" + rem.ToString() + "\u79d2)";
                    }
                    else
                    {
                        hint = "\u7ec3\u4e60\u5b8c\u6210 \u2713";
                    }

                    using (Font hf = new Font("\u5fae\u8f6f\u96c5\u9ed1", 11f, FontStyle.Regular))
                    using (Brush hb2 = new SolidBrush(Color.FromArgb(160, 255, 255, 255)))
                    {
                        SizeF ts = g.MeasureString(hint, hf);
                        g.DrawString(hint, hf, hb2,
                            _windowWidth - ts.Width - 20,
                            _windowHeight - ts.Height - 50);
                    }

                    // ----- 底部进度条 -----
                    float pct;
                    if (_elapsedSeconds >= 30 || _finished)
                        pct = 1f;
                    else
                        pct = (float)_elapsedSeconds / 30f;

                    int barW = 200;
                    int barH = 5;
                    int barX = (_windowWidth - barW) / 2;
                    int barY = _windowHeight - 30;

                    using (Brush bg = new SolidBrush(Color.FromArgb(60, 200, 200, 200)))
                        g.FillRectangle(bg, barX, barY, barW, barH);
                    using (Brush fg = new SolidBrush(Color.FromArgb(200, 255, 220, 80)))
                        g.FillRectangle(fg, barX, barY, (int)(barW * pct), barH);
                }
            }
        }

        private void OnOverlayPaint(object sender, PaintEventArgs e)
        {
            if (!_active) return;

            lock (_lock)
            {
                if (_buffer != null)
                {
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.DrawImage(_buffer, 0, 0);
                }
            }
        }
    }
}
