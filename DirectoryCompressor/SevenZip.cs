using System.Configuration;
using System.Diagnostics;

namespace DirectoryCompressor
{
    static class Archiver
    {   public static void CreateZipFile(string fileDirPath, string prefferedPath)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = ConfigurationManager.AppSettings["7zipPath"] + "7z.exe";
            p.Arguments = "a -t7z \"" + prefferedPath + "\" \"" + fileDirPath + "\"";
            p.WindowStyle = ProcessWindowStyle.Normal;
            Process x = Process.Start(p);
            x.WaitForExit();
            return;
        }

        public static void TestZipFile(string prefferedPath)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = ConfigurationManager.AppSettings["7zipPath"] + "7z.exe";
            p.Arguments = "t \"" + prefferedPath + "\"";
            p.WindowStyle = ProcessWindowStyle.Normal;
            Process x = Process.Start(p);
            x.WaitForExit();
            return;
        }
    }
}
