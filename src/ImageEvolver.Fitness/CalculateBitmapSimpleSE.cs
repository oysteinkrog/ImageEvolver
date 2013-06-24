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
using System.Drawing.Imaging;

namespace ImageEvolver.Fitness
{
    public static class CalculateBitmapSimpleSE
    {
        public static double EvaluateFitness(Pixel[] sourceImagePixelCache, Bitmap bitmap)
        {
            double error = 0;

            BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                unchecked
                {
                    unsafe
                    {
                        fixed (Pixel* pSourcePixels = sourceImagePixelCache)
                        {
                            var p1 = (Pixel*)bd.Scan0.ToPointer();
                            Pixel* p2 = pSourcePixels;
                            for (int i = sourceImagePixelCache.Length; i > 0; i--, p1++, p2++)
                            {
                                int r = p1->R - p2->R;
                                int g = p1->G - p2->G;
                                int b = p1->B - p2->B;

                                // r^2 + g^2 + b^2
                                error += r * r + g * g + b * b;
                            }
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bd);
            }

            return error;
        }
    }
}