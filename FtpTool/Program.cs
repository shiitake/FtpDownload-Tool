using System;

namespace FtpTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "",
                username = "",
                password = "",
                remotePath = "",
                localPath = "";

            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-server":
                    case "-s":
                        server = args[i + 1];
                        break;
                    case "-username":
                    case "-u":
                        username = args[i + 1];
                        break;
                    case "-p":
                    case "-password":
                        password = args[i + 1];
                        break;
                    case "-r":
                    case "-remote":
                        remotePath = args[i + 1];
                        break;
                    case "-l":
                    case "-local":
                        localPath = args[i + 1];
                        break;
                    case "-?":
                    case "-help":
                        PrintHelp();
                        return;
                }
            }

            var download = new Download(server, username, password, remotePath, localPath);

            var downloadCount = download.Execute();

            if (downloadCount > 0)
            {
                Console.WriteLine("Successfully downloaded {0} files.", downloadCount);
            }
            else
            {
                Console.WriteLine("No files were downloaded.");
            }
        }

        public static void PrintHelp()
        {
            Console.WriteLine("FtpTool.exe will download jpgs from a particular ftp location.");
            Console.WriteLine();
            Console.WriteLine("Usage:" + "\t" + "FtpTool.exe [-s server] [-u username] [-p password] [-l local] [-r remote]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("\t-server, -s" + "\t\t" + "Ftp Server address");
            Console.WriteLine("\t-username, -u" + "\t\t" + "Ftp Username");
            Console.WriteLine("\t-password, -p" + "\t\t" + "Ftp Password");
            Console.WriteLine("\t-local, -l" + "\t\t" + "Local download path");
            Console.WriteLine("\t-remote, -r" + "\t\t" + "Remote folder path");
        }
    }
}
