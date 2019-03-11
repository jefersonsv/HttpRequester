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

namespace HttpRequester
{
    public class ChromeHeadlessClient
    {
        string chromeExecutable;

        // https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        public ChromeHeadlessClient()
        {
            TryFindChromeExecutable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetContentAsync(string url)
        {
            var args = $"--headless --disable-gpu --dump-dom {url}";
            var result = await Cli.Wrap(this.chromeExecutable)
                        .SetWorkingDirectory(Path.GetFullPath(this.chromeExecutable))
                        .SetArguments(args ?? "")
                        .SetStandardOutputEncoding(System.Text.Encoding.UTF8)
                        .ExecuteAsync();

            return result.StandardOutput;
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
        }



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

        public void TryFindChromeExecutable()
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
            this.chromeExecutable = idx > -1 ? paths[idx] : null;
        }

        public void SetChromeExecutable(string chromeFilename)
        {
            this.chromeExecutable = chromeFilename;
        }
    }
}
