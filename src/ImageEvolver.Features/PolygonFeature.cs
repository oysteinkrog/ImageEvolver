#region Copyright

//     ImageEvolver
//     Copyright (C) 2013-2013 �ystein Krog
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
using System.Linq;
using ImageEvolver.Core.Mutation;

namespace ImageEvolver.Features
{
    public sealed class PolygonFeature : IFeatureWithSubFeatures, IEquatable<PolygonFeature>
    {
        private readonly List<PointFeature> _points;

        public PolygonFeature(IEnumerable<PointFeature> points, ColorFeature color)
        {
            _points = points.ToList();
            Color = color;
        }

        public ColorFeature Color { get; private set; }

        public IReadOnlyList<PointFeature> Points
        {
            get { return _points; }
        }

        public bool Equals(PolygonFeature other)
        {
            return Equals(Color, other.Color) && Points.SequenceEqual(other.Points);
        }

        public IFeature Clone()
        {
            IEnumerable<PointFeature> clonedPoints = Points.Select(point => point.Clone())
                                                           .Cast<PointFeature>();
            IFeature clonedColor = Color.Clone();
            return new PolygonFeature(clonedPoints, (ColorFeature) clonedColor);
        }

        public IEnumerable<IFeature> Features
        {
            get
            {
                yield return Color;
                foreach (PointFeature pointFeature in Points)
                {
                    yield return pointFeature;
                }
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
            return obj is PolygonFeature && Equals((PolygonFeature) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Color != null ? Color.GetHashCode() : 0)*397) ^ (Points != null ? CalculatePointsListHashCode() : 0);
            }
        }

        public void InsertPoint(int index, PointFeature newPoint)
        {
            _points.Insert(index, newPoint);
        }

        public void RemovePointAt(int index)
        {
            _points.RemoveAt(index);
        }

        private int CalculatePointsListHashCode()
        {
            int hash = 19;
            foreach (PointFeature point in _points)
            {
                hash = hash*31 + point.GetHashCode();
            }
            return hash;
        }
    }
}