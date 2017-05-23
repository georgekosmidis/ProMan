using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProManService.Engines.TfsLibs {

    public class CredentialsProvider : ICredentialsProvider {

        public ICredentials GetCredentials( Uri uri, ICredentials iCredentials ) {
            return new NetworkCredential( "UserName", "Password", "Domain" );
        }

        public void NotifyCredentialsAuthenticated( Uri uri ) {
            throw new ApplicationException( "Unable to authenticate" );
        }
    }
}
