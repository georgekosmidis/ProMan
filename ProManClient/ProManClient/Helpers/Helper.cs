using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace ProManClient.Helpers {
    public class Helper {

        #region Resources

        public static string GetResStr( string key, string resourcefilename = "Site", string defaultvalue = null ) {
            string res = null;

            string baseName = "ProManClient.Resources.";

            baseName += resourcefilename == "Site" || resourcefilename == null ? "Site" : "Controllers." + resourcefilename;
            ResourceManager resManager = new ResourceManager( baseName, Assembly.GetExecutingAssembly() );
            res = resManager.GetString( key );

            if ( res == null ) {
                res = defaultvalue == null ? key : defaultvalue;
                var t = Resources.Strings.Site.ResourceManager.GetString( key.Trim() );
                return t == null ? res : t;
            }
            if ( res == null )
                res = key;

            return res;
        }


        #endregion


        public static T Setting<T>( string name ) {
            string value = ConfigurationManager.AppSettings[name];

            if ( value == null ) {
                throw new Exception( String.Format( "Could not find setting '{0}',", name ) );
            }

            return (T)Convert.ChangeType( value, typeof( T ), CultureInfo.InvariantCulture );
        }
    }
}