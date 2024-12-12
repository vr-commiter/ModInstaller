namespace ModInstaller;

using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Path = System.IO.Path;

public class Installer
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SECURITY_ATTRIBUTES
    {
        internal uint nLength;
        internal IntPtr lpSecurityDescriptor;
        internal int bInheritHandle;
    }

    [DllImport("kernel32", EntryPoint = "CreateDirectoryW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
    private static extern bool CreateDirectory(string path, ref SECURITY_ATTRIBUTES lpSecurityAttributes);

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new(sourceDirName);
        dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        if (!dir.Exists)
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.
        if (!string.IsNullOrWhiteSpace(destDirName) && !Directory.Exists(destDirName))
        {
            SECURITY_ATTRIBUTES foo = new SECURITY_ATTRIBUTES();
            bool result = CreateDirectory(destDirName, ref foo);
        }
        // Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        foreach (FileInfo file in dir.GetFiles())
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            string temppath = Path.Combine(destDirName, file.Name);
            Debug.WriteLine("Copying to " + temppath);
            try
            {
                file.CopyTo(temppath, true);
            }
            catch
            {
                // ignored
            }
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
            foreach (DirectoryInfo subdir in dirs)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, true);
            }
    }

    public void ZipInstallMod(string temp_directory, string destination)
    {
        try
        {

            string mod_path = Path.Combine(temp_directory, "mod");
            if (Directory.Exists(mod_path))
                DirectoryCopy(mod_path, destination, true);
        }
        catch (Exception ex)
        {
            return;
        }
    }

    public void ZipInstallApp(string temp_directory, string checkFilePath1)
    {
        string app_path = Path.Combine(temp_directory, "app");
        string destination = Path.Combine(checkFilePath1, "app");

        if (!Directory.Exists(checkFilePath1))
            Directory.CreateDirectory(checkFilePath1);

        if (!Directory.Exists(destination))
            Directory.CreateDirectory(destination);

        if (Directory.Exists(app_path))
            DirectoryCopy(app_path, destination, true);

        string truegearInfoPath = Path.Combine(temp_directory, "truegear.info");
        string truegearInfoPath2 = Path.Combine(checkFilePath1, "truegear.info");

        FileInfo file = new FileInfo(truegearInfoPath);
        if (file.Exists)
            file.CopyTo(truegearInfoPath2, true);
    }

    public void Unzip(string zipFile, string destination)
    {
        if (!Directory.Exists(destination))
            Directory.CreateDirectory(destination);
        using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFile)))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                try
                {
                    string directoryName = Path.Combine(destination, theEntry.Name);
                    FileInfo fi = new FileInfo(directoryName);
                    if (!Directory.Exists(fi.DirectoryName))
                    {
                        Directory.CreateDirectory(fi.DirectoryName);
                    }

                    string fileName = Path.Combine(destination, theEntry.Name);
                    if (fileName != String.Empty && theEntry.IsFile)
                    {
                        using (FileStream streamWriter = File.Create(fileName))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            streamWriter.Close();
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error while unzipping file: " + theEntry.Name);
                }
            }
        }

        using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFile)))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                try
                {


                }
                catch (Exception)
                {
                    Console.WriteLine("Error while unzipping file: " + theEntry.Name);
                }
            }
        }
    }

    internal void ModDel(string GamePath, string dataPath)
    {
        string truegearInfoPath2 = Path.Combine(dataPath, "truegear.json");

        if (File.Exists(truegearInfoPath2))
        {
            string text = File.ReadAllText(truegearInfoPath2);
            AppModData gameInfo = JsonConvert.DeserializeObject<AppModData>(text);
            if (gameInfo == null)
                return;

            if (gameInfo.ModFiles != null)
            {
                foreach (var line in gameInfo.ModFiles)
                {
                    if (line.Length > 0)
                    {
                        string mod_path = Path.Combine(GamePath, line);
                        if (File.Exists(mod_path))
                            File.Delete(mod_path);
                    }
                }
            }
        }

        try
        {
            if (File.Exists(truegearInfoPath2))
                File.Delete(truegearInfoPath2);

            if (Directory.Exists(dataPath))
                Directory.Delete(dataPath, true);
        }
        catch (Exception e)
        {
            //logger.WriteMessage(e.Message + "ModDel");
        }
        // Process line
    }
}