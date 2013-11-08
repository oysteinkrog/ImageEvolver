using System.Collections.Generic;
using System.Drawing;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;

namespace ImageEvolver.UnitTests.Rendering
{
    internal class TestCandidate : IImageCandidate
    {
        public TestCandidate(Size size)
        {
            Size = size;
        }

        public IEnumerable<IFeature> Features
        {
            get
            {
                yield return new PolygonFeature(new List<PointFeature>
                                                {
                                                    new PointFeature(100, 20),
                                                    new PointFeature(250, 20),
                                                    new PointFeature(180, 200),
                                                    new PointFeature(80, 200)
                                                },
                                                new ColorFeature(0, 0, 255, 128));

                yield return new PolygonFeature(new List<PointFeature>
                                                {
                                                    new PointFeature(50, 10),
                                                    new PointFeature(300, 150),
                                                    new PointFeature(200, 300),
                                                    new PointFeature(20, 400)
                                                },
                                                new ColorFeature(255, 0, 0, 128));

                yield return new PolygonFeature(new List<PointFeature>
                                                {
                                                    new PointFeature(150, 50),
                                                    new PointFeature(400, 150),
                                                    new PointFeature(130, 400)
                                                },
                                                new ColorFeature(0, 255, 0, 128));
            }
        }

        public Color BackgroundColor
        {
            get { return Color.White; }
        }

        public Size Size { get; set; }
    }
}