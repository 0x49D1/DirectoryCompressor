﻿using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Fclp;

namespace DirectoryCompressor
{
    class Program
    {
        public class ApplicationArguments
        {
            private string _destinationDirectory;
            public string SourceDirectory { get; set; }
            public string AutoDelete { get; set; }
            public bool Help { get; set; }
            public int? LastModifiedDays { get; set; }

            public string DestinationDirectory
            {
                get
                {
                    if (string.IsNullOrEmpty(_destinationDirectory))
                        _destinationDirectory = SourceDirectory;
                    return _destinationDirectory;
                }
                set { _destinationDirectory = value; }
            }
        }

        static void Main(string[] args)
        {
            var p = new FluentCommandLineParser<ApplicationArguments>();
            // Setup parameters
            p.Setup(arg => arg.SourceDirectory).As('s', "source").Required();
            p.Setup(arg => arg.DestinationDirectory).As('d', "destination");
            p.Setup(arg => arg.AutoDelete).As('a', "delete");
            p.Setup(arg => arg.LastModifiedDays).As('l', "lastmodified");
            p.Setup(arg => arg.Help).As('h', "help");

            var res = p.Parse(args);

            var a = p.Object;

            if (a.Help)
            {
                ShowHelp();
                return;
            }

            if (res.HasErrors)
            {
                throw new AmbiguousMatchException("Errors found in parameters!");
            }

            int lastModifiedDays = int.Parse(ConfigurationManager.AppSettings["lastmodifieddays"]); // Default 15 days
            if (a.LastModifiedDays.HasValue && a.LastModifiedDays.Value > 0)
                lastModifiedDays = a.LastModifiedDays.Value;

            var dirs = Directory.GetDirectories(a.SourceDirectory);
            Console.WriteLine($"Found {dirs.Length} directories for possible processing");
            foreach (var s in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(s);
                if (di.CreationTime <= DateTime.Now.AddDays(-lastModifiedDays))
                {
                    if (DirSize(di) > 0)
                    {
                        string filename = di.Name + ".7z";
                        Archiver.CreateZipFile(s, Path.Combine(a.DestinationDirectory, filename));
                        Archiver.TestZipFile(Path.Combine(a.DestinationDirectory, filename));
                    }

                    if (!string.IsNullOrEmpty(a.AutoDelete) && a.AutoDelete == "1") // ONLY 1! Special value
                        di.Delete(true);
                    Console.WriteLine($"Processed {di.Name}");
                }
            }

            Console.WriteLine("Directories processed");
            var files = Directory.GetFiles(a.SourceDirectory);
            Console.WriteLine($"Found {files.Length} files for possible processing");
            foreach (var s in files)
            {
                FileInfo fi = new FileInfo(s);
                // zip ONLY LOG files for now
                if (fi.Extension == ".log" && fi.CreationTime <= DateTime.Now.AddDays(-lastModifiedDays))
                {
                    string filename = fi.Name + ".7z";
                    Archiver.CreateZipFile(s, Path.Combine(a.DestinationDirectory, filename));
                    Archiver.TestZipFile(Path.Combine(a.DestinationDirectory, filename));
                    if (!string.IsNullOrEmpty(a.AutoDelete) && a.AutoDelete == "1") // ONLY 1! Special value
                        fi.Delete();
                    Console.WriteLine($"Processed {fi.Name}");
                }
            }

            Console.WriteLine("Finished processing");
        }

        private static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }

            return size;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("s required    source directory; to search subdirectories for archiving");
            Console.WriteLine("a,    autodelete; in case this parameter is set to 1 - REMOVE recursively directories that were successfully archived");
            Console.WriteLine("l,    lastmodified; in case this parameter is set only directories, modified more than l will be archived: -l 500    - archive folders, that were modified 500 days before now");
            Console.WriteLine("d,    destination directory; default is same as source directory");

            Console.ReadKey();
        }
    }
}