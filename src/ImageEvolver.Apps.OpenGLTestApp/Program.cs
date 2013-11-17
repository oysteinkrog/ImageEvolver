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
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageEvolver.Apps.ConsoleTestApp;
using ImageEvolver.Resources.Images;
using Nito.AsyncEx;

namespace ImageEvolver.Apps.OpenGLTestApp
{
    internal static class Program
    {
        private static object _runEvoluationTask;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AsyncContext.Run(() => RunSimulation());
        }

        private static async void RunSimulation()
        {
            Bitmap image = Images.MonaLisa_EvoLisa200x200;
            SimpleEvolutionSystemOpenCL evolutionSystem = await SimpleEvolutionSystemOpenCL.Create(image);

            // need to disable context for ogl context sharing to work
            await evolutionSystem.OpenGlContext.Disable();
            using (var window = new TestWindow(image, evolutionSystem, evolutionSystem.OpenGlContext))
            {
                await evolutionSystem.OpenGlContext.Enable();

                _runEvoluationTask = Task.Factory.StartNew(async () =>
                {
                    while (!window.IsExiting)
                    {
                        bool updateRender = await evolutionSystem.Engine.StepAsync();
                        if (updateRender)
                        {
                            window.NotifyUpdateRender();
                        }
                    }
                },
                                                           default(CancellationToken),
                                                           TaskCreationOptions.LongRunning,
                                                           TaskScheduler.Default);

                window.Run();
            }
        }
    }
}