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
    public class ChromeClient : ChromeBase
    {
        readonly bool headless;
        readonly string chromeExecutable;

        // https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        public ChromeClient(bool headless)
        {
            this.headless = headless;
            this.chromeExecutable = FindChromeExecutable();

            if (string.IsNullOrEmpty(this.chromeExecutable))
                throw new Exception("Chrome cannot be found on system");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetContentAsync(string url)
        {
            var args = $"{(this.headless ? "--headless" : string.Empty)} --disable-gpu --dump-dom {url}";
            var result = await Cli.Wrap(this.chromeExecutable)
                        .SetWorkingDirectory(Path.GetDirectoryName(this.chromeExecutable))
                        .SetArguments(args ?? "")
                        .SetStandardOutputEncoding(System.Text.Encoding.UTF8)
                        .ExecuteAsync();

            return result.StandardOutput;
        }
    }
}
