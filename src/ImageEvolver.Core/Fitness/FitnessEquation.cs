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

namespace ImageEvolver.Core.Fitness
{
    public enum FitnessEquation
    {
        /// <summary>
        /// "Simple" Squared-error, as used in EvoLisa
        /// </summary>
        SimpleSE,

        /// <summary>
        /// Mean Squared Error
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Mean_squared_error
        /// </remarks>
        MSE,

        /// <summary>
        /// Absolute Error
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Mean_absolute_error
        /// </remarks>
        AE,

        /// <summary>
        /// Mean Absolute Error
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Mean_absolute_error
        /// </remarks>
        MAE,

        /// <summary>
        /// Root Mean Square Deviation
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Root_mean_square_deviation
        /// </remarks>
        RMSD,

        /// <summary>
        /// Normalized Root Mean Square-Deviation
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Root_mean_square_deviation
        /// </remarks>
        NRMSD,

        /// <summary>
        /// Peak Signal-To-Noise Ratio
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Peak_signal-to-noise_ratio
        /// </remarks>
        PSNR

    }
}