using System.Collections.Generic;

namespace albert_extensions.Models
{
    class AlbertItemAction
    {
        /// <summary>
        /// is the actions name
        /// </summary>
        public string name { get; set; }
        
        /// <summary>
        /// is the program to be execute
        /// </summary>
        public string command { get; set; }
        
        /// <summary>
        /// is an array of parameters for command
        /// </summary>
        public List<string> arguments { get; set; } = new List<string>();
    }
}