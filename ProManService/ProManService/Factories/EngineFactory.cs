using ProManService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManService.Factories
{
    public class EngineFactory
    {
        public EngineFactory()
        {

        }

        public Engine Get()
        {
            return new Engine(
                       new Engines.SVN(
                           new EfData()
                       ),
                       new Engines.TFS(
                           new EfData()
                       )
            );
        }
    }
}
