using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaExportBlocks
{
    public static class FileUtlits
    {
        public static void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Export location " + path + " does not exist, create new directory");
                Directory.CreateDirectory(path);
            }
        }

        //іноді при генерації назв файлів що формуються з назв блоків зявлятюся заборонені символи такі як "/" наприклад: "D:\\NPZ_Bitoxide_Opennes\\TiaExportBlocks\\t\\AutoExport\\tag_tables\\xml\\DI/O C600.xml"
        //рішення: замінити заборонений символ на дозволений
        public static string ReplaceCorrectFileName(string fullFileName)
        {
            string correctFileName = fullFileName.Replace("/", "'");
            return correctFileName;
        }

        public static void ResaveFileAsPreviousName(string path)
        {
            if (File.Exists(path))
            {
                string PathPrev = path + "_prev";
                if (File.Exists(PathPrev)) File.Delete(PathPrev);
                Directory.Move(path, PathPrev);
            }
        }


    }
}
