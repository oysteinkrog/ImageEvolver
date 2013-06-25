using System.Collections.Generic;

namespace ImageEvolver.Core.Mutation
{
    public interface IFeatures
    {
        IEnumerable<IFeature> Features { get; }
    }
}