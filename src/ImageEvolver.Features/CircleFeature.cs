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
using ImageEvolver.Core.Mutation;

namespace ImageEvolver.Features
{
    public sealed class CircleFeature : IFeatureWithSubFeatures, IEquatable<CircleFeature>
    {
        public CircleFeature(PointFeature point, ColorFeature color)
        {
            Point = point;
            Color = color;
        }

        public ColorFeature Color { get; private set; }
        public PointFeature Point { get; private set; }

        public bool Equals(CircleFeature other)
        {
            return Equals(Color, other.Color) && Equals(Point, other.Point);
        }

        public IFeature Clone()
        {
            IFeature clonedPoint = Point.Clone();
            IFeature clonedColor = Color.Clone();
            return new CircleFeature((PointFeature) clonedPoint, (ColorFeature) clonedColor);
        }

        public IEnumerable<IFeature> Features
        {
            get
            {
                yield return Color;
                yield return Point;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is CircleFeature && Equals((CircleFeature) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Color != null ? Color.GetHashCode() : 0)*397) ^ (Point != null ? Point.GetHashCode() : 0);
            }
        }
    }
}