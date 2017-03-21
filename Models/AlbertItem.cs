using System.Collections.Generic;

namespace albert_extensions.Models
{
    class AlbertItem
    {
        /// <summary>
        /// is the plugin wide unique id of the result
        /// </summary>
        public string id { get; set; }  
        
        /// <summary>
        /// is the name of the result
        /// </summary>
        public string name { get; set; }  
        
        /// <summary>
        /// is the description of the result
        /// </summary>
        public string description { get; set; }  
        
        /// <summary>
        /// is the icon of the result (name or path)
        /// </summary>
        public string icon { get; set; }  
        
        /// <summary>
        /// is a array of objects representing the actions for the item.
        /// </summary>
        public List<AlbertItemAction> actions { get; set; } = new List<AlbertItemAction>();
    }
}