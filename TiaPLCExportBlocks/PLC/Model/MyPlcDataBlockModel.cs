using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaExportBlocks
{
    public class MyPlcDataBlockModel
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string InstanceOfName { get; set; }
        public string Path { get; set; }
        public string BlockType { get; set; } //DB FB FC OB
        public string Adress //DB1
        {
            get => BlockType+Number.ToString();
        }
    }
}
