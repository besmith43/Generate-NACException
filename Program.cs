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
            Console.WriteLine(path);

            GenerateInfo info = new GenerateInfo();
            info.StartGenerateInfo();
        }

        // see http://codebuckets.com/2017/10/19/getting-the-root-directory-path-for-net-core-applications/ for original text
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
