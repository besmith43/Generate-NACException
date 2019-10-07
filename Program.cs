using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;

namespace Generate_NACException
{
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Verbose")]
        public bool Verbose { get; }

        private void OnExecute()
        {
            string hostname = Environment.MachineName.ToUpper();

            GenerateInfo info = new GenerateInfo(hostname);
            string csvContent = info.StartGenerateInfo();

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

            if(!File.Exists(FileName))
            {
                using(StreamWriter sw = File.CreateText(FileName))
                {
                    sw.WriteLine(csvContent);
                }
            }
            else
            {
                Console.WriteLine("CSV already exists");
                Console.WriteLine("Would you like to replace it? (y/n)");
                string Answer = Console.ReadLine();

                if(Answer == "y" || Answer == "Y" || Answer.ToLower() == "yes")
                {
                    File.Delete(FileName);
                    using (StreamWriter sw = File.CreateText(FileName))
                    {
                        sw.WriteLine(csvContent);
                    }
                }
            }

            if(Verbose)
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
    }
}
