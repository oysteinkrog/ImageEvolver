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
using System.IO;
using System.Reflection;
using System.Text;
using Cloo;
using Cloo.Bindings;
using Clpp.Core.Utilities;

namespace ImageEvolver.Fitness.OpenCL
{
    public sealed class CalculateImageDifferenceError : IDisposable
    {
        private readonly ComputeCommandQueue _commandQueue;
        private readonly ComputeImage2D _sourceImage;
        private ComputeKernel _errorKernel;
        private ComputeProgram _errorProgram;
        private ComputeBuffer<uint> _errorSquaredOutputBuffer;

        public CalculateImageDifferenceError(ComputeContext computeContext,
                                             ComputeDevice computeDevice,
                                             ComputeCommandQueue commandQueue,
                                             ComputeImage2D sourceImage)
        {
            _commandQueue = commandQueue;
            _sourceImage = sourceImage;

            string errorSquaredKernelSource = ReadEmbeddedStream("ImageEvolver.Fitness.OpenCL.CalculateImageDifferenceError.cl");

            _errorProgram = new ComputeProgram(computeContext, errorSquaredKernelSource);

            const string buildOptions = "";

            try
            {
                _errorProgram.Build(new List<ComputeDevice>
                                    {
                                        computeDevice
                                    },
                                    buildOptions,
                                    Notify,
                                    IntPtr.Zero);
            }
            catch
            {
                var sb = new StringBuilder();
                var status = _errorProgram.GetBuildStatus(computeDevice);
                sb.Append("Device: ");
                sb.AppendLine(computeDevice.Name);
                sb.Append("Build status: ");
                sb.AppendLine(_errorProgram.GetBuildStatus(computeDevice)
                                           .ToString());
                sb.Append("Build log: ");
                sb.AppendLine(_errorProgram.GetBuildLog(computeDevice));
                string buildLog = sb.ToString();
            }

            _errorKernel = _errorProgram.CreateKernel("kernel__error_squared");

            _errorSquaredOutputBuffer = new ComputeBuffer<uint>(computeContext, ComputeMemoryFlags.ReadWrite, _sourceImage.Width*_sourceImage.Height);

            _errorKernel.SetMemoryArgument(1, _sourceImage);
            _errorKernel.SetMemoryArgument(2, _errorSquaredOutputBuffer);
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
                DisposeHelper.Dispose(ref _errorSquaredOutputBuffer);
                DisposeHelper.Dispose(ref _errorKernel);
                DisposeHelper.Dispose(ref _errorProgram);
            }
            // get rid of unmanaged resources
        }

        public ComputeBuffer<uint> ErrorSquaredOutputBuffer
        {
            get { return _errorSquaredOutputBuffer; }
        }

        public void ComputeDifference(ComputeImage2D image)
        {
            _errorKernel.SetMemoryArgument(0, image);

            // compute the error squared..
            _commandQueue.Execute(_errorKernel, null, new long[] {_sourceImage.Width, _sourceImage.Height}, null, null);
        }

        private static string ReadEmbeddedStream(string resourceFilePath, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceFilePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void Notify(CLProgramHandle programhandle, IntPtr notifydataptr) {}
    }
}