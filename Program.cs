using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Generate_NACException
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = GetApplicationRoot();

            string hostname = Environment.MachineName.ToUpper();

            GenerateInfo info = new GenerateInfo(hostname);
            string csvContent = info.StartGenerateInfo();

            string FileName = GenerateFileName(hostname);

            if(!File.Exists($"{ path }\\{ FileName }"))
            {
                using(StreamWriter sw = File.CreateText($"{ path }\\{ FileName }"))
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
                    File.Delete($"{ path }\\{ FileName }");
                    using (StreamWriter sw = File.CreateText($"{ path }\\{ FileName }"))
                    {
                        sw.WriteLine(csvContent);
                    }
                }
            }
        }

        // see http://codebuckets.com/2017/10/19/getting-the-root-directory-path-for-net-core-applications/ for original text
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static string GenerateFileName(string host)
        {
            string FormatedDate = $"{ DateTime.Today.ToString("d").Replace("/","") }";

            return $"{ FormatedDate }-{ host }.csv";
        }
    }
}
