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
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using ImageEvolver.Core.Fitness;

namespace ImageEvolver.Fitness.Bitmap
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///     Performance optimizations from http://danbystrom.se/2008/12/14/improving-performance/
    /// </remarks>
    public sealed class FitnessEvaluatorBitmap : IDisposable, IFitnessEvaluator<System.Drawing.Bitmap>
    {
        private readonly FitnessEquation _equationType;
        private readonly Pixel[] _sourceImagePixels;
        private readonly Func<Pixel[], System.Drawing.Bitmap, double> _fitnessEquation;

        public FitnessEvaluatorBitmap(System.Drawing.Bitmap sourceBitmap, FitnessEquation equationType)
        {
            _equationType = equationType;
            _sourceImagePixels = GenerateSourceColorMatrix(sourceBitmap);

            switch (equationType)
            {
                case FitnessEquation.SimpleSE:
                    _fitnessEquation = CalculateBitmapSimpleSE.EvaluateFitness;
                    break;
                case FitnessEquation.MSE:
                    _fitnessEquation = CalculateBitmapMSE.EvaluateFitness;
                    break;
                case FitnessEquation.AE:
                    _fitnessEquation = CalculateBitmapAE.EvaluateFitness;
                    break;
                case FitnessEquation.MAE:
                    _fitnessEquation = CalculateBitmapMAE.EvaluateFitness;
                    break;
                case FitnessEquation.RMSD:
                    _fitnessEquation = CalculateBitmapRMSD.EvaluateFitness;
                    break;
                case FitnessEquation.NRMSD:
                    _fitnessEquation = CalculateBitmapNRMSD.EvaluateFitness;
                    break;
                case FitnessEquation.PSNR:
                    _fitnessEquation = CalculateBitmapPSNR.EvaluateFitness;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("equationType");
            }
        }

        public async Task<double> EvaluateFitnessAsync(System.Drawing.Bitmap candidate)
        {
            return _fitnessEquation(_sourceImagePixels, candidate);
        }

        ~FitnessEvaluatorBitmap()
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

        private static Pixel[] GenerateSourceColorMatrix(System.Drawing.Bitmap sourceImage)
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
    }
}