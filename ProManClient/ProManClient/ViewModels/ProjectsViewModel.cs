using System;

namespace ProManClient.ViewModels {
    public class ProjectsViewModel : BaseViewModel {
        public long ID { get; set; }
        public long RepositoryID { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public DateTime? CommitDate { get; set; }

        private float _totalBytes { get; set; }
        public float TotalBytes {
            get { return _totalBytes / 1024; }
            set { _totalBytes = value; }
        }

        public float BPL { get; set; }
        public int CocomoMode { get; set; }

        public double TotalEffort {
            get {
                return this.CalculateEffort( this.CocomoMode, this._totalBytes, this.BPL );
            }
            set { }
        }
        public double TotalDevTime {
            get {
                return this.CalculateDevTime( this.CocomoMode, this._totalBytes, this.BPL );
            }
            set { }
        }
        public DateTime LastUpdate { get; set; }
        public bool Allowed { get; set; }
    }
}