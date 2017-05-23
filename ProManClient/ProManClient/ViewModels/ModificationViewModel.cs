using ProManClient.Helpers;

namespace ProManClient.ViewModels {
    public class ModificationViewModel {
        public string Type { get; set; }

        private float _totalBytes { get; set; }
        public float TotalBytes {
            get { return _totalBytes / 1024; }
            set { _totalBytes = value; }
        }
        public string TypeTitle { get {
            return Helper.GetResStr( "lbl_"+ this.Type, "Site" );
            } 
        }
    }
}