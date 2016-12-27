using System;
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
            int lastmodifieddays = int.Parse(ConfigurationManager.AppSettings["lastmodifieddays"]); // Default 15 days
            if (a.LastModifiedDays.HasValue && a.LastModifiedDays.Value > 0)
                lastmodifieddays = a.LastModifiedDays.Value;

            foreach (var s in Directory.GetDirectories(a.SourceDirectory))
            {
                DirectoryInfo di = new DirectoryInfo(s);
                if (di.CreationTime <= DateTime.Now.AddDays(-lastmodifieddays))
                {
                    string filename = di.Name + ".7z";
                    Archiver.CreateZipFile(s, Path.Combine(a.DestinationDirectory, filename));
                    Archiver.TestZipFile(Path.Combine(a.DestinationDirectory, filename));
                    if (!string.IsNullOrEmpty(a.AutoDelete) && a.AutoDelete == "1") // ONLY 1! Special value
                        di.Delete(true);
                }
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("s required    source directory to search subdirectories for archiving");
            Console.WriteLine("a,    in case this parameter is set to 1 - REMOVE recursively directories that were successfully archived");
            Console.WriteLine("l,    in case this parameter is set only directories, modified more than l will be archived: -l 500    - archive folders, that were modified 500 days before now");

            Console.ReadKey();
        }
    }
}
