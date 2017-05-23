using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using ProManService.tfs;
using SharpSvn;
using System.Collections.Generic;
using System.Data.Entity;

namespace ProManService {
    public partial class Service1 : ServiceBase {
        public Service1() {
            InitializeComponent();
        }

        protected override void OnStart( string[] args ) { }

        public void OnStart() {
            Engine.Start();
        }


       

        protected override void OnStop() { }
    }
}