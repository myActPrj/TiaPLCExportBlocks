using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
//using TiaExportBlocks.Model;

namespace TiaExportBlocks
{
    public class PlcBlockController
    {
        //private MyPlcDataBlockProvider _myPlcDataBlock;
        //public PlcBlockController()
        //{
        //    _myPlcDataBlock = new MyPlcDataBlockProvider();
        //}



        //public List<MyPlcDataBlockModel> ExportBlocks(PlcSoftware software)
        public void ExportBlocks(PlcSoftware software)
        {
            string name = software.Name;
            Console.WriteLine(name);

            foreach (PlcBlock block in software.BlockGroup.Blocks)
            {
                //if (block.Name == "AI_SET_TIR_1")
                Console.Write($"block.Name={block.Name}\t block.Number={block.Number} \t block.ProgrammingLanguage={block.ProgrammingLanguage}");

                //if (block.ProgrammingLanguage == ProgrammingLanguage.DB) {  // SCL DB  LAD

                string blockType = block.GetType().ToString();

                if (block.ProgrammingLanguage == ProgrammingLanguage.DB)
                {
                    Console.WriteLine($" PlcSoftwareBlock block: DB{block.Number} {block.ProgrammingLanguage} {block.Name} "); //InstanceOfName: block.InstanceOfName '
                }
                else
                {                                                //if (block.ProgrammingLanguage == ProgrammingLanguage.DB)  Siemens.Engineering.SW.Blocks.GlobalDB Siemens.Engineering.SW.Blocks.InstanceDB
                    var a = block.GetAttribute("SecondaryType"); //Siemens.Engineering.SW.Blocks.OB Siemens.Engineering.SW.Blocks.FB
                }


            }
        }

    }

}
