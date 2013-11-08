using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImageEvolver.Core;
using ImageEvolver.Core.Extensions;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Core.Random;
using ImageEvolver.Core.Utilities;
using ImageEvolver.Features;

namespace ImageEvolver.UnitTests.Rendering
{
    internal class TestCandidateRandom : IImageCandidate
    {
        private readonly IEnumerable<PolygonFeature> _features;

        public TestCandidateRandom(Size size, Range<int> numFeaturesRange, Range<int> pointsPerFeatureRange)
        {
            Size = size;


            var r = new BasicPseudoRandomProvider();

            int numFeatures = r.NextInt(numFeaturesRange);
            _features = Enumerable.Range(0, numFeatures)
                                  .Select(x => new PolygonFeature(Enumerable.Range(0, r.NextInt(pointsPerFeatureRange))
                                                                            .Select(z => new PointFeature(r.NextPoint(Size))),
                                                                  new ColorFeature(r.NextInt(0, 255),
                                                                                   r.NextInt(0, 255),
                                                                                   r.NextInt(0, 255),
                                                                                   r.NextInt(0, 255))));
        }

        public IEnumerable<IFeature> Features
        {
            get { return _features; }
        }

        public Color BackgroundColor
        {
            get { return Color.White; }
        }

        public Size Size { get; private set; }
    }
}