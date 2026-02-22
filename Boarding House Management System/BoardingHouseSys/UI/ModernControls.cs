using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BoardingHouseSys.UI
{
    public class PieChartControl : Control
    {
        public Dictionary<string, double> Data { get; set; } = new Dictionary<string, double>();
        public List<Color> Colors { get; set; } = new List<Color> 
        { 
            Color.FromArgb(0, 123, 255), // Blue
            Color.FromArgb(40, 167, 69), // Green
            Color.FromArgb(255, 193, 7), // Yellow
            Color.FromArgb(220, 53, 69), // Red
            Color.FromArgb(23, 162, 184) // Teal
        };

        public PieChartControl()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.Size = new Size(360, 260);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (Data == null || Data.Count == 0)
            {
                string msg = "No Data";
                SizeF sz = e.Graphics.MeasureString(msg, this.Font);
                e.Graphics.DrawString(msg, this.Font, Brushes.Gray, (Width - sz.Width) / 2, (Height - sz.Height) / 2);
                return;
            }

            double total = Data.Values.Sum();
            if (total == 0) return;

            float startAngle = 0;
            int colorIndex = 0;

            int margin = 20;
            int legendWidth = 160;
            int diameter = Math.Min(Width - legendWidth - (margin * 2), Height - (margin * 2));
            if (diameter < 0) diameter = 0;

            Rectangle rect = new Rectangle(margin, margin, diameter, diameter);
            
            foreach (var kvp in Data)
            {
                float sweepAngle = (float)(kvp.Value / total * 360);
                using (SolidBrush brush = new SolidBrush(Colors[colorIndex % Colors.Count]))
                {
                    e.Graphics.FillPie(brush, rect, startAngle, sweepAngle);
                }
                startAngle += sweepAngle;
                colorIndex++;
            }

            // Draw Legend
            float legendX = rect.Right + 20;
            float legendY = margin;
            colorIndex = 0;

            foreach (var kvp in Data)
            {
                Color c = Colors[colorIndex % Colors.Count];
                using (SolidBrush brush = new SolidBrush(c))
                {
                    e.Graphics.FillRectangle(brush, legendX, legendY, 15, 15);
                }
                
                string label = $"{kvp.Key} ({kvp.Value/total:P0})";
                e.Graphics.DrawString(label, this.Font, Brushes.Black, legendX + 20, legendY);
                
                legendY += 25;
                colorIndex++;
            }
        }
    }

    public class LineChartControl : Control
    {
        public List<double> Values { get; set; } = new List<double>();
        public List<string> Labels { get; set; } = new List<string>();
        public string Title { get; set; } = "Chart";

        public LineChartControl()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.Size = new Size(500, 300);
            this.BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Title
            using (Font titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                e.Graphics.DrawString(Title, titleFont, Brushes.Black, 10, 5);
            }

            if (Values == null || Values.Count == 0) return;

            // Chart Area
            int margin = 40;
            int chartWidth = Width - (margin * 2);
            int chartHeight = Height - (margin * 2);
            int startX = margin;
            int startY = Height - margin;

            // Axes
            using (Pen axisPen = new Pen(Color.Gray, 1))
            {
                e.Graphics.DrawLine(axisPen, startX, startY, startX + chartWidth, startY); // X Axis
                e.Graphics.DrawLine(axisPen, startX, startY, startX, startY - chartHeight); // Y Axis
            }

            double maxVal = Values.Max();
            if (maxVal == 0) maxVal = 100;

            // Plot Points
            PointF[] points = new PointF[Values.Count];
            float xStep = (float)chartWidth / (Values.Count > 1 ? Values.Count - 1 : 1);

            for (int i = 0; i < Values.Count; i++)
            {
                float x = startX + (i * xStep);
                float y = startY - (float)((Values[i] / maxVal) * chartHeight);
                points[i] = new PointF(x, y);

                // Draw X Label
                if (Labels != null && i < Labels.Count)
                {
                    e.Graphics.DrawString(Labels[i], this.Font, Brushes.Gray, x - 10, startY + 5);
                }

                // Draw Point
                e.Graphics.FillEllipse(Brushes.Blue, x - 3, y - 3, 6, 6);
            }

            // Draw Line
            if (points.Length > 1)
            {
                using (Pen linePen = new Pen(Color.FromArgb(0, 123, 255), 2))
                {
                    e.Graphics.DrawLines(linePen, points);
                }
            }
        }
    }
}
