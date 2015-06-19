using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gun
{
    class Computer : Panel
    {
        string _number;

        public Computer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var number = _number;

            if (number != null)
            {
                var w = Width;
                var h = Height;
                var font = new Font("Arial", 56);

                var size = g.MeasureString(number, font);
                //g.DrawString(number, font, Brushes.Green, w/2 - size.Width / 2, h/2 - size.Height / 2);
            }

            DrawNumberLine(g, 0, 200);
        }

        void DrawNumberLine(Graphics g, int start, int end)
        {
            var w = Width;
            var h = Height;
            var font = new Font("System", 20, FontStyle.Bold);
            var x = 0f;
            var y = 00f;

            var i = start;
            var pad = 0;
            var offsetXL = 0f;
            var offsetYT = 0f;
            var offsetXR = 0f;
            var offsetYB = 0f;

            while (i <= end)
            {
                for (; i <= end; i++)
                {
                    var size = g.MeasureString(i.ToString(), font);

                    g.DrawString(i.ToString(), font, Brushes.Green, x, y);
                    
                    if (x + offsetXL + size.Width + size.Height >= w)
                    {
                        i++;
                        x = w - size.Height - offsetXR;
                        offsetYT += size.Height;
                        y = offsetYT;
                        break;
                    }

                    x += size.Width + pad;
                }
                
                for (; i <= end; i++)
                {
                    var size = g.MeasureString(i.ToString(), font, 100, new StringFormat(StringFormatFlags.DirectionVertical));

                    g.DrawString(i.ToString(), font, Brushes.Green, x, y, new StringFormat(StringFormatFlags.DirectionVertical));
                    
                    if (y + size.Height + offsetYT >= h)
                    {
                        i++;
                        y = h - offsetYB - size.Width;
                        offsetXR += size.Width;
                        x = w - offsetXR;
                        break;
                    }
                    y += size.Height + pad;
                }
                for (; i <= end; i++)
                {
                    var size = g.MeasureString(i.ToString(), font);
                    x -= size.Width + pad;
                    if (x <= offsetXL)
                    {
                        x = offsetXL;
                        offsetYB += size.Height;
                        y = h - offsetYB;
                        break;
                    }
                    g.DrawString(i.ToString(), font, Brushes.Green, x, y);
                }
                for (; i <= end; i++)
                {
                    var size = g.MeasureString(i.ToString(), font, 100, new StringFormat(StringFormatFlags.DirectionVertical));
                    y -= size.Height + pad;
                    if (y <= offsetYT)
                    {
                        y = offsetYT;
                        offsetXL += size.Width;
                        x = offsetXL;
                        break;
                    }
                    g.DrawString(i.ToString(), font, Brushes.Green, x, y, new StringFormat(StringFormatFlags.DirectionVertical));
                }
            }
        }

        internal void ShowNumber(string number)
        {
            _number = number;
            Invalidate();
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            Invalidate();
        }
    }
}
