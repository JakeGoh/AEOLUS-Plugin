using MissionPlanner.Plugins.AEOLUS.Mode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Plugins.AEOLUS
{
    public partial class TrialRenderer : UserControl
    {
        public TrialRenderer()
        {
            InitializeComponent();
            Initalize();
        }
        
        public TrialRenderer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initalize();
        }
        
        private void Initalize()
        {
            this.focusDotGraphic.Paint += this.OnDotPaint;
        }

        public void ResetInputs()
        {
            this.mX = 0;
            this.mY = 0;
            this.zoom_level = 1f;
        }

        private float zoom_level = 1.0f;

        private float dx = 0;
        private float dy = 0;
        private float scaleFactor = 0;

        private float dotLoc = -1;

        private PointF mouseDownLoc;
        private float mdX = 0f;
        private float mdY = 0f;
        private float mX = 0f;
        private float mY = 0f;

        private float sizeX, sizeY;

        private float sqr_mouse_min_dist = 0f;
        private bool is_time_scrub = false;

        private PointF[] pointsArray = null;
        private const float OFFSET = 25f;
        private const float GRAB_DIST = 10f;

        private List<ScreenMode.Frame> frames { get { return MainScreen.instance.CurrentMode.Frames; } }
        
        private void SetDotToLine()
        {
            if (pointsArray == null) return;

            this.sqr_mouse_min_dist = float.MaxValue;
            PointF mouse_pos_local = this.PointToClient(MousePosition);

            mouse_pos_local.X -= this.dx;
            mouse_pos_local.Y -= this.dy;
            
            for (int i = 0; i < pointsArray.Length; i++)
            {
                PointF p1 = pointsArray[i];

                PointF mouse_pos = mouse_pos_local.Subtract(p1);
                float sqr_mouse_dist = mouse_pos.Dot(mouse_pos);

                // check closest point on line seg to mouse
                if (sqr_mouse_dist < this.sqr_mouse_min_dist)
                {
                    this.sqr_mouse_min_dist = sqr_mouse_dist;
                    this.dotLoc = i;
                }

                if (i == 0) continue;
                // check line segment between current and prev

                PointF p0 = pointsArray[i - 1];

                // no line segment if points are the same
                if (p0.Equals(p1)) continue;

                PointF p2_to_p1_vec = p0.Subtract(p1);
                float sqr_line_seg_len = p2_to_p1_vec.Dot(p2_to_p1_vec);
                float t = p2_to_p1_vec.Dot(mouse_pos);

                // t is the scalar of vector dot product against the line segment
                if (t < 0 || t > sqr_line_seg_len) continue;
                
                float cx = p1.X + t * p2_to_p1_vec.X / sqr_line_seg_len;
                float cy = p1.Y + t * p2_to_p1_vec.Y / sqr_line_seg_len;
                PointF intersect_pos = new PointF(cx, cy);

                PointF hp = intersect_pos.Subtract(mouse_pos_local);
                float sqrMouseDist = hp.Dot(hp);
                
                if (sqrMouseDist > this.sqr_mouse_min_dist) continue;

                this.sqr_mouse_min_dist = sqrMouseDist;

                float norm_diff = (t / sqr_line_seg_len);
                this.dotLoc = i - 1 + norm_diff;
            }

        }

        private float GetTimeAtFrame(float time)
        {
            int div_time = (int)Math.Floor(time);
            float mod_time = time - div_time;

            int t0 = GetTimeAtFrame(div_time);

            if (mod_time == 0) return t0;

            int t1 = GetTimeAtFrame(div_time+1);

            return MathUtils.Linear_Interpolate(t1, t0, mod_time);
        }

        private int GetTimeAtFrame(int i)
        {
            if (i < 0) return 0;
            return this.frames[Math.Min(i, this.frames.Count-1)].time_ms - this.frames[0].time_ms;
        }

        private PointF GetPointFromTime(int time)
        {
            int idx = 0;
            while (idx < this.frames.Count - 1 && GetTimeAtFrame(idx) < time)
            {
                idx++;
            }

            int t0 = GetTimeAtFrame(idx);
            PointF p0 = this.pointsArray[idx];

            if (t0 == time) return p0;
            if (idx == 0) return p0;

            int t1 = GetTimeAtFrame(idx-1);
            PointF p1 = this.pointsArray[idx-1];

            float norm_diff = (float)(time - t1) / (t0 - t1);
            
            return MathUtils.Linear_Interpolate(p0, p1, norm_diff);
        }

        // Internal API

        public int Time { get; private set; }
        public Action<float> OnDotClick;

        internal void OnTimeChange(long time)
        {
            this.Time = (int)time;
            this.Invalidate();
        }

        // Windows Form API

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.dotLoc = -1;
            this.focusDotGraphic.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (this.sqr_mouse_min_dist < GRAB_DIST * GRAB_DIST)
                    {
                        if (this.OnDotClick != null) this.OnDotClick(this.GetTimeAtFrame(this.dotLoc));
                        this.is_time_scrub = true;
                    } else
                    {
                        this.mouseDownLoc = e.Location;
                        this.is_time_scrub = false;
                    }
                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    this.mX += this.mdX;
                    this.mY += this.mdY;
                    this.mdX = 0f;
                    this.mdY = 0f;
                    
                    break;
            }
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.SetDotToLine();
            this.focusDotGraphic.Invalidate();

            switch (e.Button) {
                case MouseButtons.Left:
                    if (this.is_time_scrub)
                    {
                        if (this.OnDotClick != null) this.OnDotClick(this.GetTimeAtFrame(this.dotLoc));
                    }
                    else
                    {
                        this.mdX = (e.Location.X - this.mouseDownLoc.X) / scaleFactor;
                        this.mdY = (e.Location.Y - this.mouseDownLoc.Y) / scaleFactor;

                        if (this.mdX + this.mX > this.sizeX)
                            this.mdX = this.sizeX - this.mX;
                        if (this.mdX + this.mX < -this.sizeX)
                            this.mdX = -this.sizeX - this.mX;

                        if (this.mdY + this.mY > this.sizeY)
                            this.mdY = this.sizeY - this.mY;
                        if (this.mdY + this.mY < -this.sizeY)
                            this.mdY = -this.sizeY - this.mY;
                    }
                    this.Invalidate();
                    break;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            this.zoom_level *= 1f + 0.002f * e.Delta;
            this.zoom_level = Math.Max(this.zoom_level, 1f);

            this.Invalidate();
        }
        //the white dot on the 2-D graph
        private void OnDotPaint(object sender, PaintEventArgs e)
        {
            if (this.pointsArray == null)
            {
                this.label.Text = "";
                return;
            }

            e.Graphics.TranslateTransform(dx, dy);

            PointF dot_p = this.GetPointFromTime(this.Time);
            e.Graphics.FillEllipse
            (
                new SolidBrush(Color.Yellow),
                dot_p.X - 5,
                dot_p.Y - 5,
                10,
                10
            );
            
            this.label.Text = string.Format("{0:0.00}m, {1:0.00}m",
                dot_p.X / scaleFactor, dot_p.Y / scaleFactor);

            if (this.dotLoc != -1)
            {
                int dot_idx = (int)this.dotLoc;
                float dot_rem = this.dotLoc - dot_idx;

                PointF p0 = this.pointsArray[dot_idx];
                PointF point;

                if (dot_rem != 0)
                {
                    PointF p1 = this.pointsArray[dot_idx + 1];

                    point = MathUtils.Linear_Interpolate(p0, p1, dot_rem);
                }
                else
                {
                    point = p0;
                }

                e.Graphics.FillEllipse
                (
                    new SolidBrush(Color.White),
                    point.X - 5,
                    point.Y - 5,
                    10,
                    10
                );
            }
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            List<PointF> points = new List<PointF>();
            if (MainScreen.instance != null)
            {
                points = 
                    this.frames.ConvertAll<PointF>(
                        (ScreenMode.Frame f) => { return f.point; });
            }

            g.DrawRectangle
            (
                Pens.White,
                new System.Drawing.Rectangle(0, 0, this.Width - 1, this.Height - 1)
            );

            float x = 50f;
            while (x < this.Width)
            {
                g.DrawLine(Pens.DimGray,
                    x, 0,
                    x, this.Width);

                x += 100f;
            }

            float y = 50f;
            while (y < this.Height)
            {
                g.DrawLine(Pens.DimGray,
                    0, y,
                    this.Width, y);

                y += 100f;
            }
            
            if (points.Count < 2)
            {
                pointsArray = null;
                return;
            }

            pointsArray = points.ToArray();

            float minX = points.Min((p) => { return p.X; });
            float maxX = points.Max((p) => { return p.X; });

            float minY = points.Min((p) => { return p.Y; });
            float maxY = points.Max((p) => { return p.Y; });
            
            this.sizeX = maxX - minX;
            this.sizeY = maxY - minY;

            float ratio = Math.Min((this.Width - OFFSET * 2) / sizeX, (this.Height - OFFSET * 2) / sizeY);
            this.scaleFactor = ratio * this.zoom_level;
                
            for (int i = 0; i < pointsArray.Length; i++)
            {
                pointsArray[i] = new PointF
                (
                    pointsArray[i].X * scaleFactor,
                    pointsArray[i].Y * scaleFactor
                );
            }
            
            this.dx = (this.mX + this.mdX - minX) * scaleFactor;
            this.dy = (this.mY + this.mdY - minY) * scaleFactor;
            float s = scaleFactor;

            if (sizeX < sizeY)
            {
                this.dx += (this.Width - sizeX * scaleFactor) / 2;
                this.dy += OFFSET;
            }
            else
            {
                this.dx += OFFSET;
                this.dy += (this.Height - sizeY * scaleFactor) / 2;
            }
            
            g.TranslateTransform(dx, dy);

            g.DrawLines(Pens.Yellow, pointsArray);

            Font font = new Font(SystemFonts.DefaultFont.FontFamily, 11f);
            //scalfactor is pixels per meter

            g.DrawString(string.Format("{0:0.00}m", 100f/scaleFactor),
                font, Brushes.White, this.Width-dx-6*font.Size, this.Height-dy-1.5f*font.Height);
        }

    }
}
