using System.Collections.Generic;

namespace MMX_Web_Tools.Models
{
    public class SaleTemplate
    {
        public string Name { get; set; }
        public List<string> Terms { get; set; } = new List<string>();
    }
}
