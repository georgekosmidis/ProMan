using ProManService.Factories;
using System.ServiceProcess;


namespace ProManService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) { }

        public void OnStart()
        {
            new EngineFactory().Get().Start();
        }

        protected override void OnStop() { }
    }
}