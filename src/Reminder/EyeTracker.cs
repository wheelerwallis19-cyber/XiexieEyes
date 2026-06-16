using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Reminder
{
    /// <summary>
    /// 眼部保健——视线追踪练习焦点球模块
    /// 使用 Win32 原生 API (WS_EX_LAYERED + WS_EX_TOPMOST + HWND_TOPMOST) 
    /// 确保绝对最上层 + 无闪烁
    /// </summary>
    public class EyeTracker
    {
        // --- Win32 imports for rock-solid topmost + layered window ---
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOACTIVATE = 0x0010;

        private const int SW_SHOW = 5;

        // --- Fields ---
        private Form _overlayForm;
        private Timer _moveTimer;
        private Timer _durationTimer;
        private bool _active = false;
        private bool _finished = false;

        // 运动状态
        private float _angle = 0f;
        private float _ballRadius;
        private float _pathRadiusX;
        private float _pathRadiusY;
        private float _speed;
        private int _elapsedSeconds = 0;

        // 帧缓冲
        private Bitmap _buffer;
        private readonly object _lock = new object();
        private int _winW, _winH;

        // 全部使用白色调
        private static readonly Color WhiteHint = Color.FromArgb(180, 255, 255, 255);
        private static readonly Color DotColor = Color.FromArgb(40, 255, 255, 255);
        private static readonly Color Trail1 = Color.FromArgb(80, 255, 255, 255);
        private static readonly Color Trail2 = Color.FromArgb(25, 255, 255, 255);
        private static readonly Color BallCenter = Color.FromArgb(255, 255, 255, 200);
        private static readonly Color BallEdge = Color.FromArgb(220, 255, 255, 220);
        private static readonly Color Highlight = Color.FromArgb(200, 255, 255, 255);
        private static readonly Color ProgBg = Color.FromArgb(50, 255, 255, 255);
        private static readonly Color ProgFg = Color.FromArgb(200, 255, 255, 255);

        public EyeTracker() { }

        /// <summary>
        /// 创建并显示独立 TopMost 覆盖层
        /// </summary>
        public void Start()
        {
            if (_active) return;
            _active = true;
            _finished = false;
            _elapsedSeconds = 0;
            _angle = 0f;

            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            _winW = bounds.Width;
            _winH = bounds.Height;

            // 大路径大圆
            _pathRadiusX = bounds.Width * 0.32f;
            _pathRadiusY = bounds.Height * 0.28f;
            _ballRadius = Math.Min(bounds.Width, bounds.Height) * 0.025f;
            if (_ballRadius < 14f) _ballRadius = 14f;
            if (_ballRadius > 40f) _ballRadius = 40f;

            _speed = (float)(Math.PI * 6.0 / 300.0);
            if (_speed < 0.02f) _speed = 0.02f;

            // 帧缓冲
            _buffer = new Bitmap(_winW, _winH,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // 创建 overlay 窗体——使用 WS_EX_LAYERED + WS_EX_TOPMOST 确保绝对最上层
            _overlayForm = new Form();
            _overlayForm.FormBorderStyle = FormBorderStyle.None;
            _overlayForm.Bounds = bounds;
            _overlayForm.StartPosition = FormStartPosition.Manual;
            _overlayForm.ShowInTaskbar = false;
            _overlayForm.BackColor = Color.Black;
            _overlayForm.TransparencyKey = Color.Black;

            // 应用 Win32 扩展样式
            IntPtr hwnd = _overlayForm.Handle;  // 触发句柄创建

            // 获取当前样式，添加 WS_EX_LAYERED (使TransparencyKey生效) + WS_EX_TOPMOST
            // + WS_EX_TRANSPARENT (鼠标穿透) + WS_EX_TOOLWINDOW (无任务栏) + WS_EX_NOACTIVATE
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |= WS_EX_LAYERED;
            exStyle |= WS_EX_TRANSPARENT;
            exStyle |= WS_EX_TOOLWINDOW;
            exStyle |= WS_EX_NOACTIVATE;
            SetWindowLong(hwnd, GWL_EXSTYLE, (IntPtr)exStyle);

            _overlayForm.Paint += OnOverlayPaint;
            _overlayForm.Show();

            // 强制置顶
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW | SWP_NOACTIVATE);

            // 再次Show以确保可见
            ShowWindow(hwnd, SW_SHOW);

            // 定时器
            _moveTimer = new Timer();
            _moveTimer.Interval = 100;
            _moveTimer.Tick += OnMoveTick;
            _moveTimer.Start();

            _durationTimer = new Timer();
            _durationTimer.Interval = 1000;
            _durationTimer.Tick += OnDurationTick;
            _durationTimer.Start();

            // 渲染首帧
            RenderFrame(_angle);
            _overlayForm.Invalidate();
        }

        public void Stop()
        {
            if (!_active) return;
            _active = false;
            _finished = true;

            if (_moveTimer != null) { _moveTimer.Stop(); _moveTimer.Dispose(); _moveTimer = null; }
            if (_durationTimer != null) { _durationTimer.Stop(); _durationTimer.Dispose(); _durationTimer = null; }

            if (_overlayForm != null && !_overlayForm.IsDisposed)
            {
                IntPtr hwnd = _overlayForm.Handle;
                SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                _overlayForm.Paint -= OnOverlayPaint;
                _overlayForm.Close();
                _overlayForm.Dispose();
                _overlayForm = null;
            }

            lock (_lock) { if (_buffer != null) { _buffer.Dispose(); _buffer = null; } }
        }

        public bool IsActive() { return _active; }
        public bool IsFinished() { return _finished; }

        private void OnMoveTick(object sender, EventArgs e)
        {
            if (!_active) { Stop(); return; }

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
            if (_elapsedSeconds >= 30) { Stop(); _finished = true; }
        }

        /// <summary>
        /// 渲染一帧——全部白色主题
        /// </summary>
        private void RenderFrame(float currentAngle)
        {
            if (_buffer == null) return;

            float cx = _winW / 2f;
            float cy = _winH / 2f;
            float ballX = cx + (float)Math.Cos((double)currentAngle) * _pathRadiusX;
            float ballY = cy + (float)Math.Sin((double)currentAngle) * _pathRadiusY;

            lock (_lock)
            {
                using (Graphics g = Graphics.FromImage(_buffer))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);

                    // ----- 虚线轨道 (淡白点状) -----
                    using (Pen dotPen = new Pen(DotColor, 1.5f))
                    {
                        dotPen.DashStyle = DashStyle.Dash;
                        g.DrawEllipse(dotPen,
                            cx - _pathRadiusX, cy - _pathRadiusY,
                            _pathRadiusX * 2, _pathRadiusY * 2);
                    }

                    // ----- 拖尾 -----
                    float ta1 = currentAngle - _speed * 4;
                    float ta2 = currentAngle - _speed * 8;
                    float tx1 = cx + (float)Math.Cos((double)ta1) * _pathRadiusX;
                    float ty1 = cy + (float)Math.Sin((double)ta1) * _pathRadiusY;
                    float tx2 = cx + (float)Math.Cos((double)ta2) * _pathRadiusX;
                    float ty2 = cy + (float)Math.Sin((double)ta2) * _pathRadiusY;

                    using (Brush b1 = new SolidBrush(Trail1))
                    using (Brush b2 = new SolidBrush(Trail2))
                    {
                        g.FillEllipse(b1, tx1 - _ballRadius * 0.7f, ty1 - _ballRadius * 0.7f,
                            _ballRadius * 1.4f, _ballRadius * 1.4f);
                        g.FillEllipse(b2, tx2 - _ballRadius * 0.5f, ty2 - _ballRadius * 0.5f,
                            _ballRadius * 1.0f, _ballRadius * 1.0f);
                    }

                    // ----- 球体 (白色渐变) -----
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        gp.AddEllipse(ballX - _ballRadius, ballY - _ballRadius,
                            _ballRadius * 2, _ballRadius * 2);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = BallCenter;
                            pgb.SurroundColors = new Color[] { BallEdge };
                            pgb.CenterPoint = new PointF(ballX - _ballRadius * 0.3f,
                                ballY - _ballRadius * 0.3f);
                            g.FillEllipse(pgb, ballX - _ballRadius, ballY - _ballRadius,
                                _ballRadius * 2, _ballRadius * 2);
                        }
                    }

                    // ----- 高光 -----
                    float hl = _ballRadius * 0.4f;
                    using (Brush hb = new SolidBrush(Highlight))
                    {
                        g.FillEllipse(hb,
                            ballX - _ballRadius * 0.4f, ballY - _ballRadius * 0.45f,
                            hl, hl * 0.7f);
                    }

                    // ----- 中心点 -----
                    using (Brush db = new SolidBrush(Highlight))
                        g.FillEllipse(db, ballX - 1.5f, ballY - 1.5f, 3, 3);

                    // ----- 右下角提示 (白色，避开倒计时) -----
                    string hint;
                    if (_elapsedSeconds < 30 && !_finished)
                        hint = "\u89c6\u7ebf\u8ffd\u8e2a\u7ec3\u4e60 (" + (30 - _elapsedSeconds).ToString() + "\u79d2)";
                    else
                        hint = "\u7ec3\u4e60\u5b8c\u6210 \u2713";

                    using (Font hf = new Font("\u5fae\u8f6f\u96c5\u9ed1", 11f, FontStyle.Regular))
                    using (Brush hb2 = new SolidBrush(WhiteHint))
                    {
                        SizeF ts = g.MeasureString(hint, hf);
                        g.DrawString(hint, hf, hb2,
                            _winW - ts.Width - 20,
                            _winH - ts.Height - 50);
                    }

                    // ----- 进度条 (白色) -----
                    float pct = (_elapsedSeconds >= 30 || _finished) ? 1f : (float)_elapsedSeconds / 30f;
                    int barW = 200;
                    int barH = 5;
                    int barX = (_winW - barW) / 2;
                    int barY = _winH - 30;

                    using (Brush bg = new SolidBrush(ProgBg))
                        g.FillRectangle(bg, barX, barY, barW, barH);
                    using (Brush fg = new SolidBrush(ProgFg))
                        g.FillRectangle(fg, barX, barY, (int)(barW * pct), barH);
                }
            }
        }

        private void OnOverlayPaint(object sender, PaintEventArgs e)
        {
            if (!_active) return;
            lock (_lock) { if (_buffer != null) e.Graphics.DrawImage(_buffer, 0, 0); }
        }
    }
}
