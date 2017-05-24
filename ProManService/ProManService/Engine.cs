using ProManService.Data;
using ProManService.Engines;

namespace ProManService
{
    public class Engine
    {
        SVN _svn;
        TFS _tfs;

        public Engine(SVN svn, TFS tfs)
        {
            _svn = svn;
            _tfs = tfs;
        }

        public void Start()
        {

            //_svn.Update();
            _tfs.Update();

        }
        
    }
}