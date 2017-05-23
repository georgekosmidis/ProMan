
namespace ProManClient.ViewModels {
    public class ListViewModel {
        public long ID { get; set; }
        public string Name { get; set; }
        public float TotalBytes { get; set; }
        public double TotalDevTime { get; set; }
        public bool Allowed { get; set; }
        public int OrderNumber { get; set; }
    }
}