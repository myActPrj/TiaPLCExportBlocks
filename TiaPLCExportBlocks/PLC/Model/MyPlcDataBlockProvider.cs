using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaExportBlocks.Model
{
    public class MyPlcDataBlockProvider
    {
        public List<MyPlcDataBlockModel> MyPlcDataBlocks;

        public MyPlcDataBlockProvider()
        {
             MyPlcDataBlocks = new List<MyPlcDataBlockModel>();
        }

        public void AddBlock(string name, string blockType, int number, string instanceOfName, string path)
        {

            //MyPlcDataBlocks.Add(
            //    new MyPlcDataBlockModel
            //    {
            //        Name = name,
            //        BlockType = blockType, //DB FB FC
            //        Number = number,
            //        InstanceOfName = instanceOfName,
            //        Path = path
            //    }

            MyPlcDataBlockModel block = new MyPlcDataBlockModel
            {
                Name = name,
                BlockType = blockType, //DB FB FC
                Number = number,
                InstanceOfName = instanceOfName,
                Path = path
            };

            MyPlcDataBlocks.Add(block);

        }

        public static bool SaveCSV(string fileName, List<MyPlcDataBlockModel> myPlcDataBlocks)
        {
            const string CsvSpliter = ";";

            string[] modelFields = { "Name", "BlockType", "Number", "Adress", "InstanceOfName", "Path" };

            try
            {
                System.IO.StreamWriter f = new System.IO.StreamWriter(fileName);
                //System.IO.StreamReader f = new System.IO.StreamReader(fileName, System.Text.Encoding.UTF8);

                foreach (var fieldName in modelFields)
                {
                    f.Write(fieldName + CsvSpliter);
                }
                f.WriteLine();

                foreach (var plcBlock in myPlcDataBlocks)
                {
                    f.Write(plcBlock.Name + CsvSpliter);
                    f.Write(plcBlock.BlockType + CsvSpliter);
                    f.Write(plcBlock.Number + CsvSpliter);
                    f.Write(plcBlock.Adress + CsvSpliter);
                    f.Write(plcBlock.InstanceOfName + CsvSpliter);
                    f.Write(plcBlock.Path + CsvSpliter);
                    f.WriteLine();
                }

                f.Close();
                return true;
            }
            catch
            {
                Console.WriteLine($"Error save CSV-file: {fileName}");
                return false;
            }

        }

        public static bool SaveJSON(string fileName, List<MyPlcDataBlockModel> myPlcDataBlocks)
        {
            string jsonData = JsonConvert.SerializeObject(myPlcDataBlocks);
            try
            {
                System.IO.StreamWriter f = new System.IO.StreamWriter(fileName);
                f.Write(jsonData);
                f.Close();
                return true;
            }
            catch
            {
                Console.WriteLine($"Error save Json-file: {fileName}");
                return false;
            }
        }

        public static bool OpenJSON(string fileName, out List<MyPlcDataBlockModel> myPlcDataBlocks)
        {
            try
            {
                System.IO.StreamReader f = new System.IO.StreamReader(fileName);
                string jsonData = f.ReadToEnd();
                f.Close();

                myPlcDataBlocks = JsonConvert.DeserializeObject<List<MyPlcDataBlockModel>>(jsonData);
                return true;
            }
            catch (Exception e)
            {
                myPlcDataBlocks = null;
                Console.WriteLine($"Error {e} open Json-file: {fileName}");
                return false;
            }
        }

    }
}
