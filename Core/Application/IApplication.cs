using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application
{
    public interface IApplication
    {
        /// <summary>
        /// Stars the application with the given arguments.
        /// </summary>
        /// <param name="args">Arguments to start application with.</param>
        void Start(string[] args);

        /// <summary>
        /// Stops the application.
        /// </summary>
        void Stop();
    }
}
