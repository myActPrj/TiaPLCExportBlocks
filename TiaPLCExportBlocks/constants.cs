using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaExportBlocks.PLC;

namespace TiaExportBlocks
{
    public class constants
    {
        static ConfigIni configini = ConfigIni.GetObject();
        //if (!File.Exists(configini.ProjectPath))

        //static public string TIAPortalProjectPath = "";
        //static public string folderJsonData = @"D:\NPZ_Bitoxide_Opennes\TiaExportBlocks\t\jsonData";   //@"F:\NPZ_Bitoxide_Opennes\t\jsonData"; //@"F:\TIAPortal_Opennes\workSpace\dataFiles"; //D:\NPZ_Bitoxide_Opennes\TiaExportBlocks\t
        //static public string folderManExport = @"D:\NPZ_Bitoxide_Opennes\TiaExportBlocks\t\ManExport";


        //static public string PlcDB_All_csvFileName { get => folderJsonData + "\\plcDB_All.csv"; }
        static public string PlcDB_All_csvFileName { get => configini.DataFolder + "\\plcDB_All.csv"; }
        static public string PlcDB_All_jsonFileName { get => configini.DataFolder + "\\plcDB_All.json"; }

        //static public string tiaPrjAllDevices_jsonFileName { get => @"D:\NPZ_Bitoxide_Opennes\TiaExportBlocks\t\tiaPrjAllDevices.json"; }
    }
}
