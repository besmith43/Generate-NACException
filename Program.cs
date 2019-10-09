using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;

/*
to do list

add network printer functionality
    for macs: bash command = ping "ip-address"; arp -a | grep "\<149.149.140.5\>" | awk '{print $4}'
add menu if no ethernet nics are found for user selection
    do I want to have a cmd line flag to do the menu directly?
 */

namespace Generate_NACException
{
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Verbose")]
        public bool Verbose { get; }

        [Option(Description = "Version", ShortName = "V")]
        public bool Version { get; }

        [Option(Description = "Printer")]
        public bool Printer { get; }

        public static string VersionNumber = "1.1";

        private void OnExecute()
        {
            if (Version)
            {
                GetVersion();
            }

            if (Printer)
            {
                GenPrinter();
            }

            string hostname = Environment.MachineName.ToUpper();

            GenerateInfo info = new GenerateInfo(hostname);
            string csvContent = info.StartGenerateInfo();

            if (csvContent.Equals("no ethernet mac addresses found") || csvContent.Equals("non-standard OS"))
            {
                Console.WriteLine(csvContent);
                Process.GetCurrentProcess().Kill();
            }

            string path = "";
            string FileName = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                #if DEBUG
                    path = GetApplicationRootDebug();
                #else
                    path = GetApplicationRootRelease();
                #endif

                FileName = $"{ path }\\{ GenerateFileName(hostname) }";
            }
            else
            {
                #if DEBUG
                    path = GetApplicationRootDebug();
                #else
                    path = GetApplicationRootRelease();
                #endif

                FileName = $"{ path }/{ GenerateFileName(hostname) }";
            }

            if (!File.Exists(FileName))
            {
                try
                {
                	using(StreamWriter sw = File.CreateText(FileName))
                	{
                    		sw.WriteLine(csvContent);
                	}
		        }
                catch
                {
                    Console.WriteLine($"Couldn't write to path: { FileName }");
                }
            }
            else
            {
                Console.WriteLine("CSV already exists");
                Console.WriteLine("Would you like to replace it? (y/n)");
                string Answer = Console.ReadLine();

                if (Answer == "y" || Answer == "Y" || Answer.ToLower() == "yes")
                {
                    File.Delete(FileName);
                    using (StreamWriter sw = File.CreateText(FileName))
                    {
                        sw.WriteLine(csvContent);
                    }
                }
            }

            if (Verbose)
            {
                Console.WriteLine($"Path: { FileName }");
                Console.WriteLine($"CSV Content: { csvContent }");
            }
        }

        // see http://codebuckets.com/2017/10/19/getting-the-root-directory-path-for-net-core-applications/ for original text
        public static string GetApplicationRootDebug()
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static string GetApplicationRootRelease()
        {
            //return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            //return Path.GetDirectoryName(Environment.CurrentDirectory);

            string tempPath = Environment.CurrentDirectory;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !tempPath.Contains("Desktop"))
            {
                return $"{ tempPath }/Desktop";
            }
            else
            {
                return tempPath;
            }
        }

        public static string GenerateFileName(string host)
        {
            string FormatedDate = $"{ DateTime.Today.ToString("d").Replace("/","") }";

            return $"{ FormatedDate }-{ host }.csv";
        }

        public static void GetVersion()
        {
            Console.WriteLine($"Generate-NACException Version: { VersionNumber }");
            Process.GetCurrentProcess().Kill();
        }

        public static void GenPrinter()
        {
            Console.WriteLine("This feature is not yet implemented");
            Process.GetCurrentProcess().Kill();
        }
    }
}
