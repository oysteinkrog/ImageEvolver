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
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;

namespace ImageEvolver.Rendering.Bitmap
{
    public sealed class GenericFeaturesRendererBitmap : IDisposable, IImageCandidateRenderer<IImageCandidate, System.Drawing.Bitmap>
    {
        public System.Drawing.Bitmap _bitmap;
        private Graphics _g;

        public GenericFeaturesRendererBitmap(Size size)
        {
            _bitmap = new System.Drawing.Bitmap(size.Width, size.Height);
            _g = Graphics.FromImage(_bitmap);
        }

        ~GenericFeaturesRendererBitmap()
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
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }

                if (_g != null)
                {
                    _g.Dispose();
                    _g = null;
                }
            }
            // free native resources if there are any.
        }

        public void Render(IImageCandidate candidate, out System.Drawing.Bitmap bitmap)
        {
            _g.Clear(candidate.BackgroundColor);

            foreach (IFeature feature in candidate.Features)
            {
                Render(feature, _g);
            }
            bitmap = _bitmap;
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

        private void Render(IFeature feature, Graphics g)
        {
            var polygonFeature = feature as PolygonFeature;
            if (polygonFeature != null)
            {
                RenderPolygon(polygonFeature, g);
            }
            else
            {
                throw new NotSupportedException(string.Format("Rendering feature of type {0} is not supported", feature.GetType()));
            }
        }

        private static void RenderPolygon(PolygonFeature feature, Graphics g)
        {
            using (Brush brush = GetGDIBrush(feature.Color))
            {
                Point[] points = GetGDIPoints(feature.Points);
                g.FillPolygon(brush, points);
            }
        }
    }
}