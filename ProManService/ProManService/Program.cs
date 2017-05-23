using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ProManService {
    static class Program {

        [STAThread]
        static void Main( string[] args ) {

            Libs.WriteLine( "Starting..." );
            Libs.WriteLine();
            Engine.Start();
            //Console.ReadKey();

        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// [STAThread]
        //        static void Main() {

        //#if DEBUG
        //            var  service = new Service1();
        //            service.OnStart();            
        //            return;
        //#endif
        //            ServiceBase[] servicesToRun = new ServiceBase[] 
        //            { 
        //                new Service1() 
        //            };

        //            ServiceBase.Run( servicesToRun );
        //        }
    }
}
