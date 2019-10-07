using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace Generate_NACException
{
    public class GenerateInfo
    {
        private static string headerInfo = "adap.mac,siblings,host.host,adap.loc,host.devType,host.expireDate,host.inact";
        private static string genericInfo = "Never,1825 Days";
        private static string OS;
        private static List<string> MACInfo;

        public GenerateInfo()
        {
            MACInfo = new List<string>();
        }

        public void StartGenerateInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GenerateMACInfoWin();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                GenerateMACInfoLinux();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                GenerateMACInfoOSX();
            }
            else
            {
                return; //"non-standard OS";
            }

            // get hostname
            // see https://docs.microsoft.com/en-us/dotnet/api/system.environment.machinename?view=netcore-3.0

            string hostname = Environment.MachineName;
            hostname = hostname.ToUpper();
            string roomLocation = "";

            try
            {
                roomLocation = hostname.Remove(hostname.LastIndexOf('-'), 4);
            }
            catch
            {
                roomLocation = Environment.MachineName;
                roomLocation = roomLocation.ToUpper();
            }

            Console.WriteLine(roomLocation);
            if(MACInfo.Count > 0)
            {
                Console.WriteLine(MACInfo[0]);
            }
        }

        // the goal for all three of these functions is to produce the csv information for each
        // need to gather the number of valid nics, get their mac addresses, and get the computers hostname
        // in this case valid nics means physical ethernet port

        private void GenerateMACInfoWin()
        {
            OS = "Windows";

            NetworkInterface[] adpaters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adpaters)
            {
                if (adapter.Name.Contains("Ethernet"))
                {
                    MACInfo.Add(FormatMACAddress(adapter.GetPhysicalAddress().ToString()));
                }
            }
        }

        private void GenerateMACInfoLinux()
        {
            OS = "Linux";

            NetworkInterface[] adpaters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adpaters)
            {
                if (adapter.Name.Contains("en"))
                {
                    string mac_address = adapter.GetPhysicalAddress().ToString();
                    mac_address = mac_address.Insert(2, ":");
                    mac_address = mac_address.Insert(5, ":");
                    mac_address = mac_address.Insert(8, ":");
                    mac_address = mac_address.Insert(11, ":");
                    mac_address = mac_address.Insert(14, ":");
                    MACInfo.Add(mac_address);
                }
            }
        }

        //will only come back with a single ethernet port regardless of the actual number due to bash command
        private void GenerateMACInfoOSX()
        {
            OS = "MacOSX";

            string ethernet = Bash("networksetup -listallhardwareports | awk '/Hardware Port: Ethernet/{getline; print $2}'");

            NetworkInterface[] adpaters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adpaters)
            {
                if (adapter.Name.Equals(ethernet))
                {
                    string mac_address = adapter.GetPhysicalAddress().ToString();
                    mac_address = mac_address.Insert(2, ":");
                    mac_address = mac_address.Insert(5, ":");
                    mac_address = mac_address.Insert(8, ":");
                    mac_address = mac_address.Insert(11, ":");
                    mac_address = mac_address.Insert(14, ":");
                    MACInfo.Add(mac_address);
                }
            }
        }

        // see https://loune.net/2017/06/running-shell-bash-commands-in-net-core/ for original text
        private string Bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

        private string FormatMACAddress(string mac_address)
        {
            mac_address = mac_address.Insert(2, ":");
            mac_address = mac_address.Insert(5, ":");
            mac_address = mac_address.Insert(8, ":");
            mac_address = mac_address.Insert(11, ":");
            mac_address = mac_address.Insert(14, ":");

            return mac_address;
        }
    }
}
