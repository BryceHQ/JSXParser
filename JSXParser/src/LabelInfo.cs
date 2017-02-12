using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class LabelInfo
    {
        public string Kind { get; set; }
        public string Name { get; set; }

        public static LabelInfo Loop = new LabelInfo() { Kind = "loop" };
        public static LabelInfo Switch = new LabelInfo() { Kind = "switch" };
    }
}
