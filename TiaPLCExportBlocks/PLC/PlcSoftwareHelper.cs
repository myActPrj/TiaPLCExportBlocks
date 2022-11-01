using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using ClassLibrary;

namespace TiaExportBlocks.PLC
{
    
    public class ConfigIni
    {
        private static ConfigIni _configIni=null;
        private ConfigIni() { }
       
        public static ConfigIni GetObject()
        {
            if (_configIni == null)
            {
                _configIni = new ConfigIni();
                _configIni.ReadConfiginiFile();
            }
            return _configIni;
        }

        public string ProjectPath { get; set; }
        public string PlcName { get; set; }
        public string DataFolder { get; set; }  //OutputDataFolder
        public bool OpenProjectModeWithUserInterface { get; set; }

        //private const string IniFilePath= @"D:\NPZ_Bitoxide_Opennes\TiaUtils\TiaPLCExportBlocks\Config.ini";
        private const string IniFilePath= @"D:\NPZ_Bitoxide_Opennes\TiaUtils\TiaPLCExportBlocks\TiaPLCExportBlocks\Data\Config.ini";

        public void ReadConfiginiFile()
        {
            if (!File.Exists(IniFilePath))
            {
                //Console.WriteLine($"System.IO.FileNotFoundException: \"Не удалось найти указанный файл.\" ({IniFilePath})");
                //throw new System.IO.FileNotFoundException();
                //{ "Не удалось найти указанный файл.":null}
                throw new System.IO.FileNotFoundException() { Source = $"({IniFilePath})" };
            }

            using (IniFile confiniFile = new IniFile(IniFilePath))
            {
                ProjectPath  = confiniFile.Read(Section: "SourcePlcProject", Key: "ProjectPath");
                //ProjectPath.Replace(@"\\", @"\");
                PlcName      = confiniFile.Read(Section: "SourcePlcProject", Key: "PlcName");
                OpenProjectModeWithUserInterface = Boolean.Parse( confiniFile.Read(Section: "SourcePlcProject", Key: "OpenProjectModeWithUserInterface") ); 
                DataFolder   = confiniFile.Read(Section: "Main", Key: "DataFolder").Replace(@"\\",@"\");
            }
        }

        public void WriteConfiginiFile()
        {
            using (IniFile confiniFile = new IniFile(IniFilePath))
            {
                confiniFile.Write(Section: "SourcePlcProject", Key: "ProjectPath", Value: ProjectPath) ;
                confiniFile.Write(Section: "SourcePlcProject", Key: "PlcName", Value: PlcName);
                confiniFile.Write(Section: "SourcePlcProject", Key: "OpenProjectModeWithUserInterface",Value:OpenProjectModeWithUserInterface.ToString());
                confiniFile.Write(Section: "Main", Key: "DataFolder", Value: DataFolder);
            }
        }
    }




    public class PlcSoftwareHelper
    {
        private static PlcSoftwareHelper _instance = null;
        private PlcSoftwareHelper() { }
        public static PlcSoftwareHelper GetObject() =>
            _instance == null ? new PlcSoftwareHelper() : _instance;


        //мій супер любімічкий метод який я викохав 2-ва дня
        //метод повертає обєкт PlcSoftware з якого можна вже тягнути програмні блоки
        //фішка методу що він спочатку шукає вказаний проект з вказаним ПЛК спочатку у відкритих TIAPortal-лах а якщо не знаходить самостійно запускає новий TIAPortal
        //ложка дьогдя - це діалогове вікно підтвердження доступу
        public PlcSoftware GetPlcSoftware_InExistOrRunNewTiaPortal()
        {
            PlcSoftware plcSoftware = _GetPlcSoftwareInOpenningTiaPortal();

            if (plcSoftware == null)
            {
                plcSoftware = _GetPlcSoftwareInRunningAndOpenningTiaPortal();
            }
            return plcSoftware;
        }
        //-----------------------------------------------------------------------------------------------
        private PlcSoftware _GetPlcSoftwareInOpenningTiaPortal()
        {
            ConfigIni configIni = ConfigIni.GetObject();

            //bool DoneFindProjectPath = false;
            //bool DoneFindDevice = false;
            //bool DoneFindDeviceSoftwareContainer = false;

            Console.WriteLine("Enumerating TIA process..");
            foreach (TiaPortalProcess tiaPortalProcess in TiaPortal.GetProcesses())
            {
                Console.WriteLine("Process ID " + tiaPortalProcess.Id);
                Console.WriteLine("Project PATH " + tiaPortalProcess.ProjectPath); //F:\Bitoxide2_riforming2022_SCADA\Bitoxide2_riforming2022_SCADA.ap16
                if(tiaPortalProcess.ProjectPath == null)
                {
                    // Найденный откритый TIAPortal процесс не содержит открытого проекта 
                    Console.WriteLine("This TIAPortal process no opened project");
                    continue;
                }

                // проверяем ще до підключення до проекту - до появи діалогового вікна чи це цей проект що потрібно,
                // хоча і в GetPlcSoftware_SpecyfyInConfigIni() - ф-ї що викликаєтся далі теж є перевірка
                if (!String.Equals(tiaPortalProcess.ProjectPath.FullName, configIni.ProjectPath))
                {
                    Console.WriteLine("This project no source project, go to the next tiaportall process....");
                    continue;
                }

                /*TiaPortal*/ tiaPortal = tiaPortalProcess.Attach();

                //please press OK in TIAPortal window dialog Acess opennes...
                foreach (Project project in tiaPortal.Projects)
                {
                    return GetPlcSoftware_SpecyfyInConfigIni(project);
                }

            }
            return null;
        }
        private PlcSoftware _GetPlcSoftwareInRunningAndOpenningTiaPortal()
        {
            ConfigIni configIni = ConfigIni.GetObject();
            Project project = RunTiaPortal(configIni.DataFolder);

            if (project!=null)
            {
                return GetPlcSoftware_SpecyfyInConfigIni(project);
            }

            return null;
        }
        //-----------------------------------------------------------------------------------------------

        /// <summary>
        /// Повертає PlcSoftware-програмну частину ПЛК в переданому проекті, 
        /// тільки якщо проект і ПЛК в ньому відповідає по параметрам проекту джерела данних що вказуєтся в файлі конфігурації - Config.ini
        /// </summary>
        private PlcSoftware GetPlcSoftware_SpecyfyInConfigIni(Project project)
        {
            this.project = project;             //#Delete

            ConfigIni configIni = ConfigIni.GetObject();

            bool DoneFindProjectPath = false;
            bool DoneFindDevice = false;

            //2-га перевірка чи це цей проеект що вказано в configIni
            if (!String.Equals(project.Path.FullName,configIni.ProjectPath))
            {
                return null;
            }
            DoneFindProjectPath = true;
            //Devices.Find()
            Console.WriteLine($"Handling project '{project.Name}'"); //Bitoxide2_riforming2022_SCADA
            foreach (Device device in project.Devices)               //"SCADA"
            {
                Console.WriteLine("Handling device '" + device.Name + "' of type [" + device.TypeIdentifier + "]");  //[System:Device.PC], "DeviceProxy-Station_1"[System:IPIProxy.Device]
                                                                                                                        //if (device.Name== "C600 - new" && device.TypeIdentifier == "System:Device.S71500/HR")
                if (String.Equals(device.Name, configIni.PlcName))
                {
                    DoneFindDevice = true;

                    foreach (DeviceItem deviceItem in device.DeviceItems)
                    {
                        //deviceItme.Name = "SCADA"
                        Console.WriteLine("Handling device item '" + deviceItem.Name + "' of type " + deviceItem.TypeIdentifier);
                        //deviceItem.Classification == "CPU"
                        Console.WriteLine("Handling PLC device item");
                        SoftwareContainer softwareContainer = ((IEngineeringServiceProvider)deviceItem).GetService<SoftwareContainer>();
                        if (softwareContainer != null)
                        {
                            PlcSoftware software = softwareContainer.Software as PlcSoftware;
                            //device proxy не содержит software хотя и softwareContainer имеется
                            if (software != null)
                            {
                                return software;
                            }
                        }
                    }

                    break;
                }
            }


            //проект найден в откритом процессе, но устройсво не найдено
            if(DoneFindProjectPath==true && DoneFindDevice == false)
            {
                //PLC device name '{configIni.PlcName}' в указаном проекте не найден
                throw new Exception($"PLC device name '{configIni.PlcName}' in the specified project not found");
            }

            //устройство найдено, но PlcSoftware в обекте не обнаруженно
            if (DoneFindProjectPath==true && DoneFindDevice == true)
            {
                //PLC device name не содержит PlcSoftware - PLC device name не корректно указано или не имеет програмной части
                throw new Exception($"PLC device name '{configIni.PlcName}' not countain PlcSoftware - PLC device no correct specified");
            }

            return null;

        }
        //-----------------------------------------------------------------------------------------------

        //ProjectComposition projects;
        public Project project;                                  //#Delete
        public bool isAppRunTiaPortal_forAutoClosePrjEndApp;    //#Delete
        public TiaPortal tiaPortal; // for IDispose.Dispose() - with a try-finally block and call the IDispose.Dispose() method within the finally block.

        //7.7 Example program
        //Openness: Automating creation of projects, System Manual, 10 / 2018
        //page 68
        //private static void RunTiaPortal()
        public Project RunTiaPortal(string projectFullPath)
        {
            ConfigIni configIni = ConfigIni.GetObject();

            Console.WriteLine("Starting TIA Portal");

            Console.WriteLine("Wait run TiaPortal");
            Console.WriteLine("and wait confirm acess in dialog window");

            TiaPortalMode tiaPortalMode = configIni.OpenProjectModeWithUserInterface ? TiaPortalMode.WithUserInterface : TiaPortalMode.WithoutUserInterface;

            // ConfirmDialog()
            /*TiaPortal*/ tiaPortal = new TiaPortal(tiaPortalMode);
            //using (TiaPortal tiaPortal = new TiaPortal(tiaPortalMode))
            //{
            Console.WriteLine("and wait confirm acess in dialog window - OK");

                Console.WriteLine("TIA Portal has started");
                ProjectComposition projects = tiaPortal.Projects;

                if (String.IsNullOrEmpty(projectFullPath))
                {
                    Console.WriteLine("Project not set");
                    return null;
                }
                Console.WriteLine("Opening Project...");
                Console.WriteLine("Please wait...");
                isAppRunTiaPortal_forAutoClosePrjEndApp = true;

                //FileInfo projectPath = new FileInfo(@"C:\Demo\AnyCompanyProject.ap14"); //edit the path according to your project
                FileInfo fileinfoProjectPath = new FileInfo(configIni.ProjectPath); 
                /*Project*/ project = null;
                try
                {
                    project = projects.Open(fileinfoProjectPath);
                    return project;
                }
                catch
                {
                    throw new Exception(String.Format("Could not open project {0}", fileinfoProjectPath.FullName));
                }

                Console.WriteLine(String.Format("Project {0} is open", project.Path.FullName));
                IterateThroughDevices(project);
                //project.Close();
                Console.WriteLine("Demo complete hit enter to exit");
                Console.ReadLine();
            //}
        }

        private static void IterateThroughDevices(Project project)
        {
            if (project == null)
            {
                Console.WriteLine("Project cannot be null");
                return;
            }
            Console.WriteLine(String.Format("Iterate through {0} device(s)",
            project.Devices.Count));

            // Device device = project.Devices.Find("");
            foreach (Device device in project.Devices)
            {
                Console.WriteLine(String.Format("Device: \"{0}\".", device.Name));
            }
            Console.WriteLine();
        }





    }
}
