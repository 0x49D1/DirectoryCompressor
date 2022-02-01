using System.Configuration;
using System.Diagnostics;

namespace DirectoryCompressor
{
    static class Archiver
    {
        public static void CreateZipFile(string fileDirPath, string preferredPath)
        {
            ProcessStartInfo p = new ProcessStartInfo
            {
                FileName = ConfigurationManager.AppSettings["7zipPath"] + "7z.exe",
                Arguments = "a -t7z \"" + preferredPath + "\" \"" + fileDirPath + "\"",
                WindowStyle = ProcessWindowStyle.Normal
            };
            Process x = Process.Start(p);
            x.WaitForExit();
            return;
        }

        public static void TestZipFile(string preferredPath)
        {
            ProcessStartInfo p = new ProcessStartInfo
            {
                FileName = ConfigurationManager.AppSettings["7zipPath"] + "7z.exe",
                Arguments = "t \"" + preferredPath + "\"",
                WindowStyle = ProcessWindowStyle.Normal
            };
            Process x = Process.Start(p);
            x.WaitForExit();
            return;
        }
    }
}