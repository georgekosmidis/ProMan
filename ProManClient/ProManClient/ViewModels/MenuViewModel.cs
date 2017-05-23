using System.Collections.Generic;

namespace ProManClient.ViewModels {
    public class MenuViewModel {
        public string Title { get; set; }
        public int ID { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Active { get; set; }
        public bool Allowed { get; set; }
        public string Key { get; set; }
        public bool HasChildren { get; set; }
        public string Icon { get; set; }

        public virtual IEnumerable<MenuViewModel> Children { get; set; }
    }
}