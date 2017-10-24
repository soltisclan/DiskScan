using System;
using System.Diagnostics;
using System.IO;


namespace DiskScan
{
    class Program
    {
        private static long size;
        private static int excount;
        private static int filecount;
        private static int dircount;

        static void Main(string[] args)
        {
            var basedir = (args.Length > 0) ? args[0] : "c:\\windows";

            var timer = new Stopwatch();
            timer.Start();


            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady && drive.Name.ToLower() == basedir.Substring(0, 3).ToLower())
                {
                    Console.WriteLine("Total free space on '{0}': {1}", basedir.Substring(0, 1).ToUpper(), FormatBytes(drive.AvailableFreeSpace));
                }
            }

            Console.Write("Scanning '{0}' ...", basedir);

            ScanDirectory(basedir);

            timer.Stop();

            Console.SetCursorPosition(0, Console.CursorTop);

            Console.WriteLine("Disk space used by '{0}': {1}", basedir, FormatBytes(size));
            Console.WriteLine("Total number of folders: {0}", dircount.ToString("N0"));
            Console.WriteLine("Total number of files: {0}", filecount.ToString("N0"));
            Console.WriteLine("Inaccessible folders: {0}", excount.ToString("N0"));
            var ts = timer.Elapsed;
            Console.WriteLine("Time taken by scan: {0}", string.Format("{0}:{1}", Math.Floor(ts.TotalMinutes), ts.ToString("ss\\.ff")));
            Console.ReadLine();
        }

        private static void ScanDirectory(string basedir)
        {
            try
            {
                dircount++;

                switch (dircount % 4)
                {
                    case 0: Console.Write("/"); break;
                    case 1: Console.Write("-"); break;
                    case 2: Console.Write("\\"); break;
                    case 3: Console.Write("|"); break;
                }

                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

                var dirinfo = new DirectoryInfo(basedir);
                var subdirs = dirinfo.GetDirectories();
                var files = dirinfo.GetFiles();
                long subdirsize = 0;
                foreach (var file in files)
                {
                    filecount++;
                    subdirsize += new FileInfo(file.FullName).Length;
                }

                //Console.WriteLine(basedir + " " + subdirsize);

                size += subdirsize;

                foreach (var subdir in subdirs)
                {
                    ScanDirectory(subdir.FullName);
                }

            }
            catch(Exception)
            {
                //Console.WriteLine(ex.Message);
                excount++;
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
    }
}
