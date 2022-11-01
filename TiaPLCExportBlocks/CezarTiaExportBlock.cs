using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Engineering;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.Hmi.Tag;
using Siemens.Engineering.Hmi.TextGraphicList;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.ExternalSources;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;
using TiaExportBlocks.Model;

namespace TiaExportBlocks
{
    public static class CezarTiaExportBlock
    {

        static Dictionary<string, string> programLanguageToExtension = new Dictionary<string, string> { { "SCL", ".scl" }, { "DB", ".db" } };
        static Dictionary<string, string> programLanguageToFolderPrefixExtension = new Dictionary<string, string> { { "SCL", @"\scl\" }, { "DB", @"\db\" } };
        
        /// <summary>
        /// "корнева"-папка в якій розташовуються папки та файли ектспорту.
        /// </summary>
        public static string exportLocation = @"D:\NPZ_Bitoxide_Opennes\TiaExportBlocks\t\AutoExport";

        //центральна ф-я буде викликати всі інші локальні ф-ї,
        //з неї можна побачити ієрархічну організацію викликаємих ф-й
        public static void ExportAll(PlcSoftware plcSoftware)
        {
            CheckDirectories(exportLocation);

            ExportAllTagTables(plcSoftware);
            ExportTypes(plcSoftware); //Увага! ф-я не до кінця реалізована - вона не шукає в середині груп! ,як наприклад ExportAllTagTables(..)
            ExportBlocks(plcSoftware); // робить сsv-файл реээстра i експортуэ деяк блоки в файл ()

            ExportBlocks_file(plcSoftware, HandleExportBlockStructurtToFile); // HandleBlock(block, software);
        }

        /// <summary>
        /// Створення всіх необхідних папок в папці експорту у разі їх відсутності (ф-я відразу створює весь набір).
        /// </summary>
        /// <param name="exportLocation">"корнева"-папка в якій розташовуються папки та файли ектспорту.</param>
        public static void CheckDirectories(string exportLocation)
        {
            //exportLocation = args[0];
            Console.WriteLine("Export location is " + exportLocation);
            FileUtlits.CheckDirectory(exportLocation);
            FileUtlits.CheckDirectory(exportLocation + @"\scl");
            FileUtlits.CheckDirectory(exportLocation + @"\db");
            FileUtlits.CheckDirectory(exportLocation + @"\udt");
            FileUtlits.CheckDirectory(exportLocation + @"\tag_tables\xml");
            //FileUtlits.CheckDirectory(exportLocation + @"\hmi_tag_tables\xml");
            //FileUtlits.CheckDirectory(exportLocation + @"\hmi_text_lists\xml");
        }


        //----------------------------------------------------------------------------------
        //Export all "Program blocks" in project tree
        //----------------------------------------------------------------------------------

        //static void HandleBlock(PlcBlock block, PlcSoftware software)
        //{

        //    if (block.Name == "AI_SET_TIR")
        //        Console.WriteLine();

        //    PlcExternalSourceSystemGroup externalSourceGroup = software.ExternalSourceGroup;
        //    //Console.WriteLine(block.Name + " " + block.GetType()+ " " +block.ProgrammingLanguage);
        //    if (programLanguageToExtension.ContainsKey(block.ProgrammingLanguage.ToString()))
        //    {
        //        string extension;
        //        programLanguageToExtension.TryGetValue(block.ProgrammingLanguage.ToString(), out extension);
        //        string folder_prefix;
        //        programLanguageToFolderPrefixExtension.TryGetValue(block.ProgrammingLanguage.ToString(), out folder_prefix);
        //        var fileInfo = new FileInfo(exportLocation + folder_prefix + BlockNameToCorrectFileName(block.Name) + extension);
        //        var blocks = new List<PlcBlock>() { block };
        //        try
        //        {
        //            if (File.Exists(fileInfo.FullName)) File.Delete(fileInfo.FullName);
        //            Console.WriteLine(block.Name + " to " + fileInfo.FullName);
        //            externalSourceGroup.GenerateSource(blocks, fileInfo, GenerateOptions.None);
        //        }
        //        catch (Exception exc)
        //        {
        //            Console.WriteLine(exc.ToString());
        //        }
        //    }

        //}


        //private static void BlockIteration(PlcBlockGroup group, PlcSoftware software, string groupPath = "\\")
        //{
        //    foreach (PlcBlock block in group.Blocks)
        //    {
        //        Console.WriteLine(block.Name);

        //        //if (block.ProgrammingLanguage == ProgrammingLanguage.DB)
        //        if (block.Name == "AI_FB")
        //        {
        //            //if(block.GetType().ToString()=="Siemens.Engineering.SW.Blocks.GlobalDB")
        //            {
        //                HandleBlock(block, software);
        //            }
        //        }
        //    }

        //    foreach (PlcBlockGroup blockGroup in group.Groups)
        //    {
        //        groupPath = $"{groupPath}{blockGroup.Name}\\";
        //        Console.WriteLine($"Group: {groupPath}");
        //        BlockIteration(blockGroup, software, groupPath);
        //    }
        //}
        //----------------------------------------------------------------------------------


        #region Export "PLC tags"
        //----------------------------------------------------------------------------------
        //Export all "PLC tags" in project tree
        //----------------------------------------------------------------------------------
        public static void ExportAllTagTables(PlcSoftware plcSoftware)
        {
            PlcTagTableSystemGroup plcTagTableSystemGroup = plcSoftware.TagTableGroup;
            // Export all tables in the system group
            ExportTagTables(plcTagTableSystemGroup.TagTables);
            // Export the tables in underlying user groups
            foreach (PlcTagTableUserGroup userGroup in plcTagTableSystemGroup.Groups)
            {
                ExportUserGroupDeep(userGroup);
            }
        }
        private static void ExportTagTables(PlcTagTableComposition tagTables)
        {
            FileUtlits.CheckDirectory(exportLocation + @"\tag_tables\xml");
            foreach (PlcTagTable table in tagTables)
            {
                string filePath = exportLocation + @"\tag_tables\xml\" + table.Name + ".xml";
                filePath = FileUtlits.ReplaceCorrectFileName(filePath);

                var fileInfo = new FileInfo(filePath);
                Console.WriteLine(table.Name + " to " + fileInfo.FullName);
                if (File.Exists(fileInfo.FullName)) File.Delete(fileInfo.FullName);
                table.Export(fileInfo, ExportOptions.WithDefaults);
            }
        }
        private static void ExportUserGroupDeep(PlcTagTableUserGroup group)
        {
            ExportTagTables(group.TagTables);
            foreach (PlcTagTableUserGroup userGroup in group.Groups)
            {
                ExportUserGroupDeep(userGroup);
            }
        }
        //----------------------------------------------------------------------------------
        #endregion

        #region Export "PLC data types"
        //----------------------------------------------------------------------------------
        //Export all "PLC data types" in project tree
        //----------------------------------------------------------------------------------

        //Увага! ф-я не до кінця реалізована - вона не шукає в середині Груп!
        /// <summary>
        /// Eкспорт всіх заявленних типів данних в ПЛК в набір ".udt"-файлів. 
        /// УВАГА! ф-я на данний момент (20.10.2022) не до кінця реалізована - вона не шукає в середині груп!
        /// </summary>
        private static void ExportTypes(PlcSoftware software)
        {
            #warning УВАГА! ф-я на "ExportTypes" данний момент (20.10.2022) не до кінця реалізована - вона не шукає в середині груп!
            foreach (PlcType plcType in software.TypeGroup.Types)
            {
                Console.WriteLine("Handling type " + plcType.Name);
                HandleType(plcType, software);
            }
        }
        static void HandleType(PlcType plcType, PlcSoftware software)
        {
            PlcExternalSourceSystemGroup externalSourceGroup = software.ExternalSourceGroup;
            string extension = ".udt";
            var fileInfo = new FileInfo(exportLocation + @"\udt\" + plcType.Name + extension);
            var blocks = new List<PlcType>() { plcType };
            try
            {
                if (File.Exists(fileInfo.FullName)) File.Delete(fileInfo.FullName);
                externalSourceGroup.GenerateSource(blocks, fileInfo, GenerateOptions.None);

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }

        }
        //----------------------------------------------------------------------------------
        #endregion



        #region Export "Program blocks"
        //взято і переробляно з проекту PlcBlockController
        //
        static private MyPlcDataBlockProvider _myPlcDataBlocks;

        //----------------------------------------------------------------------------------
        //Export all "PLC data types" in project tree
        //----------------------------------------------------------------------------------
        //in BlockGroup
        static private void recursiveBlock(PlcBlockGroup blocksGroup, in string groupPath = "")
        {
            foreach (PlcBlock block in blocksGroup.Blocks)
            {
                block_AddToList(block, _myPlcDataBlocks, groupPath);
            }

            foreach (PlcBlockGroup blockGroup in blocksGroup.Groups)
            {
                Console.WriteLine($" PlcSoftwareBlock group:  {blockGroup.Name}");
                recursiveBlock(blockGroup, groupPath + blockGroup.Name + "\\");
            }
        }

        static public List<MyPlcDataBlockModel> ExportBlocks(PlcSoftware software)
        {
            if (_myPlcDataBlocks == null)
            { 
                _myPlcDataBlocks = new MyPlcDataBlockProvider();
            }

            string name = software.Name;
            Console.WriteLine(name);

            //foreach (PlcBlock block in software.BlockGroup.Blocks)
            //{

            //}

            //return;
            foreach (PlcBlock block in software.BlockGroup.Blocks)
            {
                block_AddToList(block, _myPlcDataBlocks, "");
            }

            //foreach (PlcBlockGroup group in software.BlockGroup.Groups)
            //{

            //    Console.WriteLine($" PlcSoftwareBlock group:  {group.Name}");

            //    //group.Blocks
            //}

            foreach (PlcBlockGroup blockGroup in software.BlockGroup.Groups)
            {
                //Console.WriteLine("Handling block group " + blockGroup.Name);
                Console.WriteLine($" PlcSoftwareBlock group:  {blockGroup.Name}");
                //foreach (PlcBlock block in blockGroup.Blocks)
                //{
                //    HandleBlock(block, software);
                //}

                //recursiveBlock(blockGroup, groupPath + blockGroup.Name + "\\"););
                recursiveBlock(blockGroup, blockGroup.Name + "\\");
            }

            string csvFileName = constants.PlcDB_All_csvFileName;
            FileUtlits.ResaveFileAsPreviousName(csvFileName);
            MyPlcDataBlockProvider.SaveCSV(csvFileName, _myPlcDataBlocks.MyPlcDataBlocks);

            string jsonFileName = constants.PlcDB_All_jsonFileName;
            FileUtlits.ResaveFileAsPreviousName(jsonFileName);
            MyPlcDataBlockProvider.SaveJSON(jsonFileName, _myPlcDataBlocks.MyPlcDataBlocks);

            return _myPlcDataBlocks.MyPlcDataBlocks;
        }

        static public List<MyPlcDataBlockModel> ReadPrevExportedBlock()
        {
            string jsonFileName = constants.PlcDB_All_jsonFileName;
            MyPlcDataBlockProvider.OpenJSON(jsonFileName, out _myPlcDataBlocks.MyPlcDataBlocks);

            return _myPlcDataBlocks.MyPlcDataBlocks;
        }

        //----------------------------------------------------------------------------------
        #endregion



        static private void block_AddToList(PlcBlock block, MyPlcDataBlockProvider _myPlcDataBlocks, string groupPath)
        {
#warning не красивий код ф-и  "CezarTiaExportBlock.block_AddToList()"
            //if (block.ProgrammingLanguage == ProgrammingLanguage.DB) {  // SCL DB  LAD
            //if (block.ProgrammingLanguage == ProgrammingLanguage.DB)
            // {
            //     Console.WriteLine($" PlcSoftwareBlock block: DB{block.Number} {block.ProgrammingLanguage} {block.Name} "); //InstanceOfName: block.InstanceOfName '
            //}
            //else
            // {                                           //if (block.ProgrammingLanguage == ProgrammingLanguage.DB)  Siemens.Engineering.SW.Blocks.GlobalDB Siemens.Engineering.SW.Blocks.InstanceDB
            //var a = block.GetAttribute("SecondaryType"); //Siemens.Engineering.SW.Blocks.OB Siemens.Engineering.SW.Blocks.FB


            //string findBlockName = "AI_SET_TIR_1"; //"TIR691" "AI_SET_TIR_1"
            //if (block.Name == findBlockName)
            //{
            //    Console.WriteLine($"{findBlockName} - found");

            //    Console.WriteLine();
            //    HandleBlock(block, software);
            //}
            //else
            //{
            //    continue;
            //}

            string blockTypeName = "uncnown";
            string blockType = block.GetType().ToString();
            switch (blockType.ToString())
            //if (blockType == "Siemens.Engineering.SW.TechnologicalObjects")
            {
                case "Siemens.Engineering.SW.Blocks.OB":
                    blockTypeName = "OB";
                    break;
                case "Siemens.Engineering.SW.Blocks.FB":
                    blockTypeName = "FB";
                    break;
                case "Siemens.Engineering.SW.Blocks.FC":
                    blockTypeName = "FC";
                    break;
                case "Siemens.Engineering.SW.Blocks.GlobalDB":
                    blockTypeName = "DB";
                    break;
                case "Siemens.Engineering.SW.Blocks.InstanceDB":
                    blockTypeName = "DB";
                    break;
            }
            Console.Write($" PlcSoftwareBlock block: {blockTypeName}{block.Number} {block.ProgrammingLanguage} {block.Name}, blockType={blockType} ");


            string blockInstanceOfName = "";
            switch (blockType.ToString())
            //if (blockType == "Siemens.Engineering.SW.TechnologicalObjects")
            {
                case "Siemens.Engineering.SW.Blocks.OB":
                    //blockTypeName = "OB";
                    var secondaryTypeOB = block.GetAttribute("SecondaryType");
                    Console.WriteLine($" aditioanal atribute: SecondaryType={secondaryTypeOB}");
                    break;
                //case "Siemens.Engineering.SW.Blocks.FB":
                //    //blockTypeName = "FB";
                //    Console.WriteLine();
                //    break;
                //case "Siemens.Engineering.SW.Blocks.FC":
                //    //blockTypeName = "FB";
                //    Console.WriteLine();
                //    break;

                //case "Siemens.Engineering.SW.Blocks.GlobalDB":
                //    //blockTypeName = "DB";
                //    Console.WriteLine();
                //    break;
                case "Siemens.Engineering.SW.Blocks.InstanceDB":
                    blockInstanceOfName = block.GetAttribute("InstanceOfName").ToString();  //((Siemens.Engineering.SW.Blocks.InstanceDB)block).InstanceOfName = "SYS_INFO"
                    Console.WriteLine($" aditioanal atribute: InstanceOfName={blockInstanceOfName}");    //AI_FB
                    break;
                default:
                    Console.WriteLine();
                    break;
            }
            //_blockList.AddBlock(block.Name, blockTypeName, block.Number, instanceOfName.ToString(), groupPath);
            
            //_myPlcDataBlocks.AddBlock(block.Name, blockTypeName, block.Number, blockInstanceOfName, "");
            _myPlcDataBlocks.AddBlock(block.Name, blockTypeName, block.Number, (blockInstanceOfName != null ? blockInstanceOfName.ToString() : ""), groupPath);

            //Console.WriteLine();
            //block.Se
            //  }



            //}
            //HandleBlock(block, software);
        }

    }
}
