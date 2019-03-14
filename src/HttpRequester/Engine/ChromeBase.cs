using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp;
using CliWrap;

namespace HttpRequester.Engine
{
    public abstract class ChromeBase
    {
        string GetChromeExecutableFromRegistry()
        {
            bool hasRegistry = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (hasRegistry)
            {
                var keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                return (string)Registry.GetValue(keyName, "Path", null);
            }

            return null;
        }

        public string FindChromeExecutable()
        {
            // https://stackoverflow.com/questions/17736215/universal-path-to-chrome-exe
            // %LOCALAPPDATA%
            // %programfiles(x86)%

            var paths = new string[]
            {
                Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"Google\Chrome", "chrome.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("programfiles(x86)"), @"Google\Chrome\Application", "chrome.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("programfiles"), @"Google\Chrome\Application", "chrome.exe"),
                GetChromeExecutableFromRegistry()
            };

            var idx = paths.ToList().FindIndex(w => File.Exists(w));
            return idx > -1 ? paths[idx] : null;
        }
    }
}
