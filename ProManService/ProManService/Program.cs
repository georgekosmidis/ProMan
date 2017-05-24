using ProManService.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ProManService
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
#if DEBUG
            new EngineFactory().Get().Start();
#else
            ServiceBase[] servicesToRun = new ServiceBase[]
            {
                new Service1()
            };

            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}


