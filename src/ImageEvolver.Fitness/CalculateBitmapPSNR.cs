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

namespace ImageEvolver.Fitness
{
    public static class CalculateBitmapPSNR
    {
        public static double EvaluateFitness(Pixel[] sourceImagePixelCache, Bitmap bitmap)
        {
            // mean square error
            var mse = CalculateBitmapMSE.EvaluateFitness(sourceImagePixelCache, bitmap);
            
            // Calculate PSNR (Peak Signal to noise ratio).
            var psnr = 10*Math.Log10((255.0*255.0)/Math.Sqrt(mse));
            
            return psnr;
        }
    }
}