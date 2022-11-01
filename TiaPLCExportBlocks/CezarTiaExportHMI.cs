using Siemens.Engineering;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.Hmi.Tag;
using Siemens.Engineering.Hmi.TextGraphicList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaExportBlocks
{
    //Exports all tag tables from an HMI device
    public class CezarTiaExportHMI
    {

        //public static string exportLocation = @"D:\NPZ_Bitoxide_Opennes\TiaExportBlocks\t\AutoExport";

        //private static void ExportAllTagTablesFromHMITarget(HmiTarget hmitarget)
        //{
        //    TagSystemFolder sysFolder = hmitarget.TagFolder;
        //    //First export the tables in underlying user folder    
        //    foreach (TagUserFolder userFolder in sysFolder.Folders)
        //    {
        //        ExportUserFolderDeep(userFolder);
        //    }
        //    //then, export all tables in the system folder    
        //    ExportTablesInSystemFolder(sysFolder);
        //}
        //private static void ExportUserFolderDeep(TagUserFolder rootUserFolder)
        //{
        //    foreach (TagUserFolder userFolder in rootUserFolder.Folders)
        //    {
        //        ExportUserFolderDeep(userFolder);
        //    }
        //    ExportTablesInUserFolder(rootUserFolder);
        //}
        //private static void ExportTablesInUserFolder(TagUserFolder folderToExport)
        //{
        //    TagTableComposition tables = folderToExport.TagTables;
        //    foreach (TagTable table in tables)
        //    {
        //        string extension = ".xml";
        //        var fileInfo = new FileInfo(exportLocation + @"\hmi_tag_tables\xml\" + table.Name + extension);
        //        try
        //        {
        //            if (File.Exists(fileInfo.FullName)) File.Delete(fileInfo.FullName);
        //            Console.WriteLine(table.Name + " to " + fileInfo.FullName);
        //            table.Export(fileInfo, ExportOptions.WithDefaults);
        //        }
        //        catch (Exception exc)
        //        {
        //            Console.WriteLine(exc.ToString());
        //        }
        //    }
        //}
        //private static void ExportTablesInSystemFolder(TagSystemFolder folderToExport)
        //{
        //    TagTableComposition tables = folderToExport.TagTables;
        //    foreach (TagTable table in tables)
        //    {
        //        string extension = ".xml";
        //        var fileInfo = new FileInfo(exportLocation + @"\hmi_tag_tables\xml\" + table.Name + extension);
        //        try
        //        {
        //            if (File.Exists(fileInfo.FullName)) File.Delete(fileInfo.FullName);
        //            Console.WriteLine(table.Name + " to " + fileInfo.FullName);
        //            table.Export(fileInfo, ExportOptions.WithDefaults);
        //        }
        //        catch (Exception exc)
        //        {
        //            Console.WriteLine(exc.ToString());
        //        }
        //    }
        //}
        ////Export TextLists 
        //private static void ExportTextLists(HmiTarget hmitarget)
        //{
        //    TextListComposition text = hmitarget.TextLists;
        //    foreach (TextList textList in text)
        //    {
        //        string extension = ".xml";
        //        var fileInfo = new FileInfo(exportLocation + @"\hmi_text_lists\xml\" + textList.Name + extension);
        //        try
        //        {
        //            if (File.Exists(fileInfo.FullName)) File.Delete(fileInfo.FullName);
        //            Console.WriteLine(textList.Name + " to " + fileInfo.FullName);
        //            textList.Export(fileInfo, ExportOptions.WithDefaults);
        //        }
        //        catch (Exception exc)
        //        {
        //            Console.WriteLine(exc.ToString());
        //        }
        //    }
        //}
    }
}
