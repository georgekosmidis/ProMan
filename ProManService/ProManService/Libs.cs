//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProManService {
//    public static class Libs {

//        #region console
//        public static void WriteLine( string l = "****************************************************" ) {
//            if ( l != "****************************************************" )
//                l = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + l;
//            Console.WriteLine( l );
//        }

//        public static void ShowMessage( string msg ) {
//            var c = Console.ForegroundColor;
//            Console.ForegroundColor = ConsoleColor.Blue;
//            WriteLine();
//            WriteLine( msg );
//            WriteLine();
//            Console.ForegroundColor = c;

//           // MessageBox.Show( msg, "Ενημέρωση!", MessageBoxButtons.OK, MessageBoxIcon.Information );
//        }

//        public static void ShowError( string error ) {
//            var c = Console.ForegroundColor;
//            Console.ForegroundColor = ConsoleColor.Red;
//            WriteLine();
//            WriteLine( error );
//            WriteLine();
//            Console.ForegroundColor = c;

//           // MessageBox.Show( error, "Σφάλμα!", MessageBoxButtons.OK, MessageBoxIcon.Error );
//        }
//        #endregion

//        public static string Encrypt( string inString ) {
//            byte[] rgbIV = new byte[8]
//              {
//                (byte) 51,
//                (byte) 37,
//                (byte) 87,
//                (byte) 121,
//                (byte) 156,
//                (byte) 172,
//                (byte) 206,
//                (byte) 240
//              };
//            try {
//                byte[] bytes1 = Encoding.UTF8.GetBytes( "pr0man:)" );
//                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
//                byte[] bytes2 = Encoding.UTF8.GetBytes( inString );
//                MemoryStream memoryStream = new MemoryStream();
//                CryptoStream cryptoStream = new CryptoStream( (Stream)memoryStream, cryptoServiceProvider.CreateEncryptor( bytes1, rgbIV ), CryptoStreamMode.Write );
//                cryptoStream.Write( bytes2, 0, bytes2.Length );
//                cryptoStream.FlushFinalBlock();
//                return Convert.ToBase64String( memoryStream.ToArray() );
//            }
//            catch ( Exception ex ) {
//                //Debug.WriteLine( ex.Message );
//                return "";
//            }
//        }

//        public static string Decrypt( string inString ) {
//            byte[] rgbIV = new byte[8]
//              {
//                (byte) 51,
//                (byte) 37,
//                (byte) 87,
//                (byte) 121,
//                (byte) 156,
//                (byte) 172,
//                (byte) 206,
//                (byte) 240
//              };
//            try {
//                byte[] bytes = Encoding.UTF8.GetBytes( "pr0man:)" );
//                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
//                byte[] buffer = Convert.FromBase64String( inString );
//                MemoryStream memoryStream = new MemoryStream();
//                cryptoServiceProvider.Key = bytes;
//                cryptoServiceProvider.IV = rgbIV;
//                CryptoStream cryptoStream = new CryptoStream( (Stream)memoryStream, cryptoServiceProvider.CreateDecryptor( bytes, rgbIV ), CryptoStreamMode.Write );
//                cryptoStream.Write( buffer, 0, buffer.Length );
//                cryptoStream.FlushFinalBlock();
//                return Encoding.UTF8.GetString( memoryStream.ToArray() );
//            }
//            catch {
//                return "";
//            }
//        }

//    }
//}
