using System;
using System.Collections.Generic;
using System.IO;

namespace AK_Lab4
{
    class Program
    {
        static void PrintHelp()
        {
            Console.WriteLine("\nЦя програма рахує кількість файлів у всіх підкаталогах окремо,\nпочинаючи з поточного шляху " +
                "(або починаючи із переданого параметром)\n\n" +
                "Параметри програми:\n\n" +
                "Lab4.exe [path] [fileTemplate] [flag1] [flag2] ... [flagN]\n\n" +
                "\tФлаги\n\n" +
                "/? або help\tвиклик підказки\n\n" +
                "/shf\t\tпоказувати окрім кількості також самі файли у каталогах\n\t\t   (вимкнено за замовчуванням)\n\n" +
                "/H\t\tобрати лише приховані файли\n\n" +
                "/A\t\tобрати лише архівні файли\n\n" +
                "/R\t\tобрати файли лише для читання\n\n\n" +
                "/H, /A, /R можуть комбінуватися\n\n\n" +
                "[fileTemplate] - шаблон, за яким має бути відбір файлів (наприклад *.exe)\n\n" +
                "[path] - стартовий дисковий шлях\n\n\n" +
                "Примітка:\n\nПараметри можуть бути передані у будь-якій кількості та послідовності\n");
        }

        static void CalcFilesInSubcatalogs(string startPath, string searchPattern = "*", bool needHidden = false,
                                           bool needArchive = false, bool needReadOnly = false, bool printFiles = false)
        {
            Console.WriteLine($"\n{startPath}\n");  //Друкуємо поточний шлях
            string[] filesStr = Directory.GetFiles(startPath, searchPattern);   //Отримуємо массив файлів
            List<FileInfo> files = new List<FileInfo>(filesStr.Length);         //Створюємо коллекцію

            if (needArchive || needHidden || needReadOnly)  //Якщо необхідно шукати файли з особливими параметрами
            {
                foreach (string fileStr in filesStr)        //Перебираємо всі файли
                {
                    FileInfo file = new FileInfo(fileStr);
                    if ((needHidden ? ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) : true) &&
                        (needArchive ? ((file.Attributes & FileAttributes.Archive) == FileAttributes.Archive) : true) &&
                        (needReadOnly ? ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) : true))
                    {
                        files.Add(file);            //Додаємо до фінального списку лиш ті, що підходять умові
                    }
                }
            }
            else {      //Інакше заносимо усі файли до фінального списку
                foreach (string fileStr in filesStr)
                    files.Add(new FileInfo(fileStr));
            }

            Console.WriteLine($"Amount Of Files = {files.Count}\n\n");  //Виводимо кількість файлів на екран

            if (printFiles) {       //Якщо флаг було встановлено, виводимо також самі файли
                Console.WriteLine("Files:\n");
                
                foreach (var file in files)
                    Console.WriteLine(file.Name);
                
                Console.WriteLine("\n\n");
            }

            string[] subcatalogs = Directory.GetDirectories(startPath);     //Отримуємо підкаталоги у поточного
            
            foreach (string catalog in subcatalogs)         //Рекурсивно обходимо всі підкаталоги
                CalcFilesInSubcatalogs(catalog, searchPattern,
                                        needHidden: needHidden,
                                        needArchive: needArchive,
                                        needReadOnly: needReadOnly,
                                        printFiles: printFiles);
        }


        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            string searchPattern = "*";
            string startCatalog = "";
            bool shf = false, needHidden = false, needArchive = false, needReadOnly = false;

            //Parsing arguments
            foreach (string a in args) {
                string arg = a.Trim();
                string argToLwr = arg.ToLower();
                if (arg == "/?" || argToLwr == "help") {
                    PrintHelp();
                    return;
                }

                if (argToLwr == "/shf") shf = true;
                else if (argToLwr == "/h") needHidden = true;
                else if (argToLwr == "/r") needReadOnly = true;
                else if (argToLwr == "/a") needArchive = true;
                else if (arg.Contains('*')) searchPattern = arg;
                else if (startCatalog == "") startCatalog = arg;
            }

            Console.WriteLine($"\nneedHidden = {needHidden}\nneedReadOnly = {needReadOnly}\nneedArchive = {needArchive}\n\nshowFiles = {shf}\n\nsearchPattern = {searchPattern}\n");

            if (startCatalog == "") startCatalog = Directory.GetCurrentDirectory();

            //Якщо переданий каталог існує, викликаємо функцію для рекурсивного обходу
            if (Directory.Exists(startCatalog))
            {
                CalcFilesInSubcatalogs(startCatalog, searchPattern,
                                        needHidden: needHidden, 
                                        needArchive: needArchive,
                                        needReadOnly: needReadOnly,
                                        printFiles: shf);

                Console.WriteLine("*** Кінець підкаталогів! ***");
            }
            else Console.WriteLine($"\nERROR! Каталог {startCatalog} не було знайдено!");

            Console.WriteLine("\n\n");
        }
    }
}
