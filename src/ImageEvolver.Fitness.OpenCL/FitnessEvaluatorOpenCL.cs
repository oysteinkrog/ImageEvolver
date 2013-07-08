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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using Cloo;
using Clpp.Core;
using Clpp.Core.Scan;
using Clpp.Core.Utilities;
using ImageEvolver.Core.Fitness;
using Koeky3D.Textures;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ImageEvolver.Fitness.OpenCL
{
    public sealed class FitnessEvaluatorOpenCL : IFitnessEvaluator<Texture2D>, IDisposable
    {
        private readonly TaskFactory _taskFactory;
        private ClppContext _clppContext;

        private ClppScanGPU<uint> _clppScanSum;

        private ComputeCommandQueue _computeCommandQueue;

        private ComputeContext _computeContext;
        private CalculateImageDifferenceError _errorCalc;
        private Size _size;
        private Bitmap _sourceBitmap;
        private ComputeImage2D _sourceBitmapComputeImage;
        private Texture2D _sourceBitmapTexture;


        public FitnessEvaluatorOpenCL(TaskFactory taskFactory, Bitmap sourceBitmap, GraphicsContext graphicsContext)
        {
            _taskFactory = taskFactory;

            _taskFactory.StartNew(() =>
            {
                _sourceBitmap = sourceBitmap;
                _size = _sourceBitmap.Size;

                // TODO: get the opencl device from the opengl context..
                // perhaps use Cloo.Bindings.CLx.GetGLContextInfoKHR()

                ComputePlatform computePlatform = ComputePlatform.Platforms[0];
                ComputeDevice device = computePlatform.Devices[0];

                bool hasGlSharing = device.Extensions.Contains("cl_khr_gl_sharing");
                if (!hasGlSharing)
                {
                    throw new Exception("gl sharing not supported by this device");
                }

                _computeContext = new ComputeContext(new List<ComputeDevice>
                                                     {
                                                         device
                                                     },
                                                     OpenGLInterop.GetInteropProperties(graphicsContext, computePlatform),
                                                     null,
                                                     IntPtr.Zero);

                _computeCommandQueue = new ComputeCommandQueue(_computeContext, device, ComputeCommandQueueFlags.Profiling);

                _clppContext = new ClppContext(device, _computeContext, _computeCommandQueue);

                _clppScanSum = new ClppScanGPU<uint>(_clppContext, _size.Width*_size.Height);

                _sourceBitmapTexture = new Texture2D(_sourceBitmap, false);
                _sourceBitmapComputeImage = ComputeImage2D.CreateFromGLTexture2D(_computeContext,
                                                                                 ComputeMemoryFlags.ReadOnly,
                                                                                 (int) _sourceBitmapTexture.TextureTarget,
                                                                                 0,
                                                                                 _sourceBitmapTexture.TextureId);

                _errorCalc = new CalculateImageDifferenceError(_computeContext, device, _computeCommandQueue, _sourceBitmapComputeImage);
            })
                        .Wait();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                DisposeHelper.Dispose(ref _errorCalc);
                DisposeHelper.Dispose(ref _sourceBitmapComputeImage);

                _sourceBitmapTexture.DestroyTexture();
                _sourceBitmapTexture = null;

                DisposeHelper.Dispose(ref _clppScanSum);
                DisposeHelper.Dispose(ref _clppContext);
                DisposeHelper.Dispose(ref _computeCommandQueue);
                DisposeHelper.Dispose(ref _computeContext);
                DisposeHelper.Dispose(ref _clppContext);
            }
            // get rid of unmanaged resources
        }

        public double EvaluateFitness(Texture2D inputTexture)
        {
            return _taskFactory.StartNew(() => EvaluateFitnessInternal(inputTexture))
                               .Result;
        }

        private double EvaluateFitnessInternal(Texture2D inputTexture)
        {
            // flush opengl operations
            GL.Flush();
            GL.Finish();

            uint fitnessErrorSum = 0;

            using (
                ComputeImage2D computeImage = ComputeImage2D.CreateFromGLTexture2D(_computeContext,
                                                                                   ComputeMemoryFlags.ReadOnly,
                                                                                   (int) inputTexture.TextureTarget,
                                                                                   0,
                                                                                   inputTexture.TextureId))
            {
                var computeMemories = new Collection<ComputeMemory>(new List<ComputeMemory>
                                                                    {
                                                                        _sourceBitmapComputeImage,
                                                                        computeImage
                                                                    });

                _computeCommandQueue.AcquireGLObjects(computeMemories, null);

                _errorCalc.ComputeDifference(computeImage);

                var val = new uint[_size.Width * _size.Height];
                _computeCommandQueue.ReadFromBuffer(_errorCalc.ErrorSquaredOutputBuffer, ref val, true, null);

                _clppScanSum.PushCLDatas(_errorCalc.ErrorSquaredOutputBuffer);
                _clppScanSum.Scan();

                _computeCommandQueue.ReadFromBuffer(_errorCalc.ErrorSquaredOutputBuffer, ref val, true, null);
                unsafe
                {
                    // read last element... which is total sum
                    _clppContext.CommandQueue.Read(_errorCalc.ErrorSquaredOutputBuffer,
                                                   true,
                                                   _errorCalc.ErrorSquaredOutputBuffer.Count - 1,
                                                   1,
                                                   new IntPtr(&fitnessErrorSum),
                                                   null);
                }

                _computeCommandQueue.ReleaseGLObjects(computeMemories, null);
            }

            // flush opencl operations
            _computeCommandQueue.Finish();

            return fitnessErrorSum;
        }
    }
}