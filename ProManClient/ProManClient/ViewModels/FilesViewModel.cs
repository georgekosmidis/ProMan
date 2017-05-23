using System.Linq;

namespace ProManClient.ViewModels {
    public class FilesViewModel {

        public string DeveloperName { get; set; }
        public int DeveloperID { get; set; }
        public bool Allowed { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }

        public string FileName {
            get {
                return Url.Split( '/' )[Url.Split( '/' ).Count() - 1];
            }
            private set { }
        }
        public string Url { get; set; }
        public string Type { get; set; }
        public float Bytes { get; set; }
    }
}