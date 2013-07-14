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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImageEvolver.Core.Utilities;

namespace ImageEvolver.Rendering.OpenGL
{
    public sealed class SingleThreadTaskScheduler : TaskScheduler, IDisposable
    {
        /// <summary>The thread used by the scheduler.</summary>
        private readonly Thread _thread;

        private bool _disposed;

        /// <summary>Stores the queued tasks to be executed by the thread.</summary>
        private BlockingCollection<Task> _tasks;

        /// <summary>Initializes a new instance of the StaTaskScheduler class with the specified concurrency level.</summary>
        public SingleThreadTaskScheduler()
        {
            // Initialize the tasks collection
            _tasks = new BlockingCollection<Task>();

            // Create the threads to be used by this scheduler
            _thread = new Thread(() =>
            {
                // Continually get the next task and try to execute it.
                // This will continue until the scheduler is disposed and no more tasks remain.
                foreach (var t in _tasks.GetConsumingEnumerable())
                {
                    TryExecuteTask(t);
                }
            });

            _thread.IsBackground = true;
            _thread.Name = "SingleThreadTaskScheduler Thread";
            _thread.Start();
        }

        /// <summary>
        ///     Cleans up the scheduler by indicating that no more tasks will be queued.
        ///     This method blocks until all threads successfully shutdown.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_tasks != null)
            {
                // Indicate that no new tasks will be coming in
                _tasks.CompleteAdding();

                _thread.Join();

                // Cleanup
                DisposeHelper.Dispose(ref _tasks);
            }


            _disposed = true;
        }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        /// <summary>Provides a list of the scheduled tasks for the debugger to consume.</summary>
        /// <returns>An enumerable of all tasks currently scheduled.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // Serialize the contents of the blocking collection of tasks for the debugger
            return _tasks.ToArray();
        }

        /// <summary>Queues a Task to be executed by this scheduler.</summary>
        /// <param name="task">The task to be executed.</param>
        protected override void QueueTask(Task task)
        {
            // Push it into the blocking collection of tasks
            _tasks.Add(task);
        }

        /// <summary>Determines whether a Task may be inlined.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">Whether the task was previously queued.</param>
        /// <returns>true if the task was successfully inlined; otherwise, false.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // Try to inline if the current thread is one of our internal threads
            return _thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId && TryExecuteTask(task);
        }
    }
}