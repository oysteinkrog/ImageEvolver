#region Copyright

//     ImageEvolver
//     Copyright (C) 2013-2013 Øystein Krog
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using ImageEvolver.Core;
using ImageEvolver.Features;

namespace ImageEvolver.Algorithms.EvoLisa.Renderer
{
    internal sealed class EvoLisaRendererBitmap : IDisposable, IImageCandidateRenderer<EvoLisaImageCandidate, Bitmap>
    {
        private Graphics _g;

        public EvoLisaRendererBitmap(Size size)
        {
            Value = new Bitmap(size.Width, size.Height);
            _g = Graphics.FromImage(Value);
        }

        ~EvoLisaRendererBitmap()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (Value != null)
                {
                    Value.Dispose();
                    Value = null;
                }

                if (_g != null)
                {
                    _g.Dispose();
                    _g = null;
                }
            }
            // free native resources if there are any.
        }

        public Bitmap Value { get; private set; }

        public void Render(EvoLisaImageCandidate drawing)
        {
            _g.Clear(Color.Black);

            foreach (PolygonFeature polygon in drawing.Polygons)
            {
                Render(polygon, _g);
            }
        }

        private static Brush GetGDIBrush(ColorFeature b)
        {
            return new SolidBrush(Color.FromArgb(b.Alpha, b.Red, b.Green, b.Blue));
        }

        private static Point[] GetGDIPoints(IList<PointFeature> points)
        {
            var gdiPoints = new Point[points.Count];
            int i = 0;
            foreach (PointFeature pt in points)
            {
                gdiPoints[i++] = new Point(pt.X, pt.Y);
            }
            return gdiPoints;
        }

        private void Render(PolygonFeature polygonFeature, Graphics g)
        {
            using (Brush brush = GetGDIBrush(polygonFeature.Color))
            {
                Point[] points = GetGDIPoints(polygonFeature.Points);
                g.FillPolygon(brush, points);
            }
        }
    }
}