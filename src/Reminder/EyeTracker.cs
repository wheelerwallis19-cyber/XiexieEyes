using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Reminder
{
    /// <summary>
    /// 眼部保健——视线追踪练习焦点球模块
    /// 在锁屏遮罩背景上运行一个缓慢移动的"焦点球"，供用户做30秒视线追踪练习
    /// </summary>
    public class EyeTracker
    {
        private readonly Form _hostForm;
        private Timer _moveTimer;
        private Timer _durationTimer;
        private bool _active = false;
        private bool _finished = false;

        // 焦点球状态
        private PointF _ballCenter;
        private float _angle = 0f;
        private float _ballRadius;
        private float _pathRadiusX;   // 水平椭圆半轴
        private float _pathRadiusY;   // 垂直椭圆半轴
        private float _speed;          // 弧度/帧
        private int _elapsedSeconds = 0;

        // 画布引用
        private Bitmap _buffer;
        private object _lock = new object();

        // 颜色
        private static readonly Color BallColor = Color.FromArgb(220, 255, 200, 100);  // 半透明金色
        private static readonly Color TrailColor = Color.FromArgb(80, 255, 200, 100);  // 更透明拖尾
        private static readonly Color CenterDotColor = Color.FromArgb(200, 255, 255, 255); // 白色中心点

        public EyeTracker(Form hostForm)
        {
            _hostForm = hostForm;
        }

        /// <summary>
        /// 启动30秒视线追踪练习
        /// </summary>
        public void Start()
        {
            if (_active) return;
            _active = true;
            _finished = false;
            _elapsedSeconds = 0;
            _angle = 0f;

            // 根据主屏幕尺寸动态计算路径大小
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            float cx = bounds.Width / 2f;
            float cy = bounds.Height / 2f;
            _ballCenter = new PointF(cx, cy);

            // 椭圆路径：水平占屏幕宽度65%，垂直占高度55%（避开健康文案区域）
            _pathRadiusX = bounds.Width * 0.28f;
            _pathRadiusY = bounds.Height * 0.22f;
            _ballRadius = Math.Min(bounds.Width, bounds.Height) * 0.018f;
            if (_ballRadius < 6f) _ballRadius = 6f;
            if (_ballRadius > 28f) _ballRadius = 28f;

            // 速度：30秒绕约3圈，一圈2PI，共6PI弧度
            _speed = (float)(Math.PI * 6.0 / 300.0);  // 300帧（100ms间隔×30秒）
            if (_speed < 0.02f) _speed = 0.02f;

            // 预分配缓冲区
            int w = bounds.Width;
            int h = bounds.Height;
            _buffer = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // 移动定时器 ~100ms 间隔
            _moveTimer = new Timer();
            _moveTimer.Interval = 100;
            _moveTimer.Tick += OnMoveTick;
            _moveTimer.Start();

            // 总时长定时器 1秒间隔
            _durationTimer = new Timer();
            _durationTimer.Interval = 1000;
            _durationTimer.Tick += OnDurationTick;
            _durationTimer.Start();

            // 注册到宿主窗体的Paint事件
            _hostForm.Paint += OnHostPaint;
            _hostForm.Invalidate();
        }

        /// <summary>
        /// 立即停止练习
        /// </summary>
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

            _hostForm.Paint -= OnHostPaint;

            lock (_lock)
            {
                if (_buffer != null)
                {
                    _buffer.Dispose();
                    _buffer = null;
                }
            }

            _hostForm.Invalidate();
        }

        public bool IsActive()
        {
            return _active;
        }

        public bool IsFinished()
        {
            return _finished;
        }

        private void OnMoveTick(object sender, EventArgs e)
        {
            if (!_active)
            {
                Stop();
                return;
            }

            // 更新球位置（椭圆路径）
            _angle += _speed;
            if (_angle > (float)(Math.PI * 2.0))
                _angle -= (float)(Math.PI * 2.0);

            float x = _ballCenter.X + (float)Math.Cos((double)_angle) * _pathRadiusX;
            float y = _ballCenter.Y + (float)Math.Sin((double)_angle) * _pathRadiusY;

            RenderFrame(x, y);
            _hostForm.Invalidate();
        }

        private void OnDurationTick(object sender, EventArgs e)
        {
            _elapsedSeconds++;
            if (_elapsedSeconds >= 30)
            {
                Stop();
                _finished = true;
                _hostForm.Invalidate();
            }
        }

        /// <summary>
        /// 渲染一帧到缓冲区
        /// </summary>
        private void RenderFrame(float ballX, float ballY)
        {
            if (_buffer == null) return;

            lock (_lock)
            {
                using (Graphics g = Graphics.FromImage(_buffer))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);

                    Rectangle bounds = Screen.PrimaryScreen.Bounds;

                    // 绘制路径指引（虚线椭圆）
                    using (Pen dotPen = new Pen(Color.FromArgb(60, 220, 220, 220), 1.5f))
                    {
                        dotPen.DashStyle = DashStyle.Dash;
                        g.DrawEllipse(dotPen,
                            _ballCenter.X - _pathRadiusX,
                            _ballCenter.Y - _pathRadiusY,
                            _pathRadiusX * 2,
                            _pathRadiusY * 2);
                    }

                    // 拖尾效果：画2个小圈作为拖尾
                    float trailAngle1 = _angle - _speed * 4;
                    float trailAngle2 = _angle - _speed * 8;
                    float tx1 = _ballCenter.X + (float)Math.Cos((double)trailAngle1) * _pathRadiusX;
                    float ty1 = _ballCenter.Y + (float)Math.Sin((double)trailAngle1) * _pathRadiusY;
                    float tx2 = _ballCenter.X + (float)Math.Cos((double)trailAngle2) * _pathRadiusX;
                    float ty2 = _ballCenter.Y + (float)Math.Sin((double)trailAngle2) * _pathRadiusY;

                    using (Brush trailBrush1 = new SolidBrush(TrailColor))
                    using (Brush trailBrush2 = new SolidBrush(Color.FromArgb(30, 255, 200, 100)))
                    {
                        g.FillEllipse(trailBrush1, tx1 - _ballRadius * 0.7f, ty1 - _ballRadius * 0.7f,
                            _ballRadius * 1.4f, _ballRadius * 1.4f);
                        g.FillEllipse(trailBrush2, tx2 - _ballRadius * 0.5f, ty2 - _ballRadius * 0.5f,
                            _ballRadius * 1.0f, _ballRadius * 1.0f);
                    }

                    // 绘制焦点球主体（带渐变）
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        gp.AddEllipse(ballX - _ballRadius, ballY - _ballRadius,
                            _ballRadius * 2, _ballRadius * 2);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = Color.FromArgb(255, 255, 240, 150);  // 亮金黄中心
                            pgb.SurroundColors = new Color[] { Color.FromArgb(220, 255, 180, 60) }; // 金黄边缘
                            pgb.CenterPoint = new PointF(ballX - _ballRadius * 0.3f, ballY - _ballRadius * 0.3f);
                            g.FillEllipse(pgb, ballX - _ballRadius, ballY - _ballRadius,
                                _ballRadius * 2, _ballRadius * 2);
                        }
                    }

                    // 高光点
                    float hl = _ballRadius * 0.4f;
                    using (Brush hlBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                    {
                        g.FillEllipse(hlBrush,
                            ballX - _ballRadius * 0.4f, ballY - _ballRadius * 0.45f,
                            hl, hl * 0.7f);
                    }

                    // 中心微点
                    using (Brush dotBrush = new SolidBrush(CenterDotColor))
                    {
                        g.FillEllipse(dotBrush,
                            ballX - 1.5f, ballY - 1.5f, 3, 3);
                    }

                    // 提示文字（右下角 - 避开左上角健康文案）
                    string hint;
                    if (_elapsedSeconds < 30)
                    {
                        int remaining = 30 - _elapsedSeconds;
                        hint = "\u89c6\u7ebf\u8ffd\u8e2a\u7ec3\u4e60 (" + remaining.ToString() + "\u79d2)";
                    }
                    else
                    {
                        hint = "\u7ec3\u4e60\u5b8c\u6210 \u2713";
                    }

                    using (Font hintFont = new Font("\u5fae\u8f6f\u96c5\u9ed1", 12f, FontStyle.Regular))
                    using (Brush hintBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                    {
                        SizeF textSize = g.MeasureString(hint, hintFont);
                        g.DrawString(hint, hintFont, hintBrush,
                            bounds.Width - textSize.Width - 30,
                            bounds.Height - textSize.Height - 70);
                    }

                    // 进度条（底部）
                    float progressPct;
                    if (_elapsedSeconds >= 30)
                        progressPct = 1f;
                    else
                        progressPct = (float)_elapsedSeconds / 30f;

                    int barWidth = 200;
                    int barHeight = 6;
                    int barX = (bounds.Width - barWidth) / 2;
                    int barY = bounds.Height - 40;

                    using (Brush bgBrush = new SolidBrush(Color.FromArgb(60, 200, 200, 200)))
                    {
                        g.FillRectangle(bgBrush, barX, barY, barWidth, barHeight);
                    }
                    using (Brush fgBrush = new SolidBrush(Color.FromArgb(200, 255, 220, 80)))
                    {
                        g.FillRectangle(fgBrush, barX, barY, (int)(barWidth * progressPct), barHeight);
                    }
                }
            }
        }

        private void OnHostPaint(object sender, PaintEventArgs e)
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
