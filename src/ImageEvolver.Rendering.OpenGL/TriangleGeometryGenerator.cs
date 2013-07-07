using System;
using System.Collections.Generic;
using System.Linq;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;
using ImageEvolver.Rendering.OpenGL.Triangulation;
using OpenTK;
using OpenTK.Graphics;

namespace ImageEvolver.Rendering.OpenGL
{
    internal static class TriangleGeometryGenerator
    {
        internal struct TriangleGeometry
        {
            public List<Color4> ColorList { get; set; }
            public List<ushort> IndexList { get; set; }
            public List<Vector2> VertexList { get; set; }
        }

        public static TriangleGeometry GenerateGeometry(IFeature feature)
        {
            var polygonFeature = feature as PolygonFeature;
            if (polygonFeature != null)
            {
                return GeneratePolygonGeometry(polygonFeature);
            }
            throw new NotSupportedException(String.Format("Rendering feature of type {0} is not supported", feature.GetType()));
        }

        private static TriangleGeometry GeneratePolygonGeometry(PolygonFeature feature)
        {
            var vertexList = feature.Points.Select(a => new Vector2(a.X, a.Y))
                                    .ToList();

            List<ushort> indexList;
            var edges = new Delaunay2D.DelaunayTriangulator();
            if (edges.Initialize(vertexList, 0.00001f))
            {
                edges.Process();
            }

            edges.ToIndexBuffer(out indexList);

            var color = new Color4(feature.Color.Red / 255f, feature.Color.Green / 255f, feature.Color.Blue / 255f, feature.Color.Alpha / 255f);
            var colorList = new List<Color4>();
            for (var i = 0; i < vertexList.Count; i++)
            {
                colorList.Add(color);
            }

            return new TriangleGeometry
                   {
                       VertexList = vertexList,
                       IndexList = indexList,
                       ColorList = colorList,
                   };
        }
    }
}