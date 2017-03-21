using System.Collections.Generic;

namespace albert_extensions.Models
{
    class AlbertMetadata
    {
        public string iid { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public string author { get; set; }
        public List<string> dependencies { get; set; } = new List<string>();
        public string trigger { get; set; }
    }
}