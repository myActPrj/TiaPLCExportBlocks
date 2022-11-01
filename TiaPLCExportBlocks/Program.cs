using Siemens.Engineering.SW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


// Назва проекту: TiaPlcExporter (Export PLC all data for auto generate tags for SCADA)
// Задача: генерацiя данних ПЛК для того щоб можна булоб здійснювати автогенерування тегів для SCADA в подальшому.
// Для отримання інформації по ПЛК використовуєтся відкритий механізм Opennes від Siemens.
// Opennes доступний для програмного забезпечення TIA Portal (v13_SP1,v14,v15,v16,v17,...)

// Ця програма тільки Генерує доступному представленні, набiр датаблоків що заявленні в ПЛК,
// всі наступні задачі по генерації тегів, виявленні нових ресурсів for SCADA, лежать на наступних утлітах, що можуть бути реалізовані на інших мовах (VBS,VBA_Excel)

using TiaExportBlocks.PLC;

namespace TiaExportBlocks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //PlcSoftwareHelper plcSoftwareHelper = PlcSoftwareHelper.GetObject();


            StartExport();

            //PlcSoftwareHelper.RunTiaPortal();

            //Siemens.Engineering.SW.PlcSoftware software = plcSoftwareHelper.GetPlcSoftware();

            //PlcBlockController plc = new PlcBlockController();
            //plc.ExportBlocks(software);

            Console.WriteLine("Program end, press any key for exit...");
            Console.ReadKey();
        }


        static void StartExport()
        {
            try
            {
                ReadConfigIniFile();
                OpenProjectExportingData();
                ExportDataProjectExportingData();
                //CloseProjectExportingData(); // в блоці finally - там йому місце
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message + "\r\n"+ e.Source);
                Console.WriteLine("Error, closing this program..");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                Console.WriteLine("--------------------------");
                if (false) CloseProjectExportingData();
            }
        }



        private static void ReadConfigIniFile()
        {
            var configini = ConfigIni.GetObject();
            if (!File.Exists(configini.ProjectPath))
            {
                throw new System.IO.FileNotFoundException() { Source = $"({configini.ProjectPath})" };
            }
            
            if (!Directory.Exists(configini.DataFolder))
            {
                throw new System.IO.DirectoryNotFoundException() { Source = $"({configini.DataFolder})" };
            }
        }

        //---------------------------------------------------------------------------------------------
        static PlcSoftwareHelper  plcSoftwareHelper;
        static PlcSoftware plcSoftware;
        private static void OpenProjectExportingData()
        {
            plcSoftwareHelper = PlcSoftwareHelper.GetObject();
            plcSoftware = plcSoftwareHelper.GetPlcSoftware_InExistOrRunNewTiaPortal();
        }

        private static void ExportDataProjectExportingData()
        {
            CezarTiaExportBlock.exportLocation = ConfigIni.GetObject().DataFolder;
            CezarTiaExportBlock.CheckDirectories(CezarTiaExportBlock.exportLocation);
            CezarTiaExportBlock.ExportAll(plcSoftware);
        }
        //---------------------------------------------------------------------------------------------

        private static void CloseProjectExportingData() //#Delete
        {
            //ніфіга не працюэ - за межами тут сам обєкт вже очистяний збірщиком сміття
            //хотыв щоб програма якщо вона запускла TIAPortal- то й закривала за собою.... але виникли проблеми

            //PlcSoftwareHelper plcSoftwareHelper2 = PlcSoftwareHelper.GetObject();
            if (plcSoftwareHelper.project != null)
            {

                //закриття проекту і закриття TiaPortal - якщо він був відкритий
                //але закрити проект - є ф-я а закрити TiaPortal - Немаоє!! тому не закриваю зовсім
                //if (plcSoftwareHelper.isAppRunTiaPortal_forAutoClosePrjEndApp)
                //{
                //    Console.WriteLine("Closing project, please press any key....");
                //    Console.ReadKey();

                //    plcSoftwareHelper.project.Close();
                //}

                // correct close connection instance TiaPortal
                if (plcSoftwareHelper.tiaPortal!=null)
                {
                    plcSoftwareHelper.tiaPortal.Dispose();
                }
            }

        }


    }
}
