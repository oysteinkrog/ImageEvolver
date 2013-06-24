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

using System.Collections.Generic;
using ImageEvolver.Core;

namespace ImageEvolver.Algorithms.EvoLisa
{
    internal class EvoLisaAlgorithmDetails : IAlgorithmDetails
    {
        private IReadOnlyCollection<ApproximationPrimitives> _primitives;

        public IReadOnlyCollection<ApproximationPrimitives> Primitives
        {
            get
            {
                return _primitives ?? (_primitives = new List<ApproximationPrimitives>
                                                     {
                                                         ApproximationPrimitives.Polygon
                                                     });
            }
        }

        public AlgorithmPlatform Platform
        {
            get { return AlgorithmPlatform.CPU; }
        }
    }
}