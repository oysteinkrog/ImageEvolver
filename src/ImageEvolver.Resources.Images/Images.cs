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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace ImageEvolver.Resources.Images
{
    public static class Images
    {

        public static Bitmap MonaLisa_Big
        {
            get { return GetImageByName("MonaLisa_Big.jpg"); }
        }

        public static Bitmap MonaLisa_EvoLisa200x200
        {
            get { return GetImageByName("MonaLisa_EvoLisa200x200.bmp"); }
        }

        public static Bitmap MonaLisa_EvoLisa200x200_TestApproximation
        {
            get { return GetImageByName("MonaLisa_EvoLisa200x200-approx-28400-278805.bmp"); }
        }

        private static Bitmap GetImageByName(string imageName)
        {
            using (Stream s = typeof (Images).Assembly.GetManifestResourceStream("ImageEvolver.Resources.Images." + imageName))
            {
                return new Bitmap(s);
            }
        }

        public static Bitmap Resize(Bitmap sourceBMP, double scale)
        {
            var scaledWidth = (int) (sourceBMP.Width*scale);
            var scaledHeight = (int) (sourceBMP.Height*scale);
            var newBitmap = new Bitmap(scaledWidth, scaledHeight);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
//                g.InterpolationMode = InterpolationMode.High;
//                g.CompositingQuality = CompositingQuality.HighQuality;
//                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(sourceBMP, 0, 0, scaledWidth, scaledHeight);
            }
            return newBitmap;
        }
    }
}