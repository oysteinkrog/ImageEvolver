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

#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageEvolver.Core.Fitness;

#endregion

namespace ImageEvolver.Fitness
{
    /// <summary>
    ///     Squared-error fitness generator, as used in EvoLisa
    /// </summary>
    /// <remarks>
    ///     Performance optimizations from http://danbystrom.se/2008/12/14/improving-performance/
    /// </remarks>
    public sealed class FitnessEvaluatorSEBitmap : IDisposable, IFitnessEvaluator<Bitmap>
    {
        private readonly Pixel[] _sourceImagePixels;

        public FitnessEvaluatorSEBitmap(Bitmap sourceBitmap)
        {
            _sourceImagePixels = GenerateSourceColorMatrix(sourceBitmap);
        }

        ~FitnessEvaluatorSEBitmap()
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
                // dispose managed resources
            }
            // free native resources if there are any.
        }

        public double EvaluateFitness(Bitmap bitmap)
        {
            double error = 0;

            BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                unchecked
                {
                    unsafe
                    {
                        fixed (Pixel* pSourcePixels = _sourceImagePixels)
                        {
                            var p1 = (Pixel*) bd.Scan0.ToPointer();
                            Pixel* p2 = pSourcePixels;
                            for (int i = _sourceImagePixels.Length; i > 0; i--, p1++, p2++)
                            {
                                int r = p1->R - p2->R;
                                int g = p1->G - p2->G;
                                int b = p1->B - p2->B;

                                // r^2 + g^2 + b^2
                                error += r*r + g*g + b*b;
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

        private static Pixel[] GenerateSourceColorMatrix(Bitmap sourceImage)
        {
            if (sourceImage == null)
            {
                throw new NotSupportedException("A source image of Bitmap format must be provided");
            }

            BitmapData bd = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                var sourcePixels = new Pixel[sourceImage.Width*sourceImage.Height];
                unsafe
                {
                    fixed (Pixel* psourcePixels = sourcePixels)
                    {
                        var pSrc = (Pixel*) bd.Scan0.ToPointer();
                        Pixel* pDst = psourcePixels;
                        for (int i = sourcePixels.Length; i > 0; i--)
                        {
                            *(pDst++) = *(pSrc++);
                        }
                    }
                }
                return sourcePixels;
            }
            finally
            {
                sourceImage.UnlockBits(bd);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Pixel
        {
            internal readonly byte B;
            internal readonly byte G;
            internal readonly byte R;
            internal readonly byte A;
        }
    }
}