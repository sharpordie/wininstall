using System;
using System.IO;
using System.Threading.Tasks;

namespace WinInstall.Programs
{
    class UngoogledChromium : Program
    {
        public override string Name => "Ungoogled Chromium";

        public override string Info => "Free and open-source web browser from Google.";

        public override string Type => "Internet";

        public override string Root => Path.Combine(Environment.GetEnvironmentVariable("PROGRAMFILES(X86)"), "Chromium");

        public override Task<string> Download()
        {
            throw new NotImplementedException();
        }

        public override Task Install()
        {
            throw new NotImplementedException();
        }

        protected override async Task<string> GetAvailableVersion()
        {
            return await Task.Delay(new Random().Next(500, 2000)).ContinueWith(t => "4.0.0.0");
        }

        protected override async Task<string> GetInstalledVersion()
        {
            return await Task.Delay(new Random().Next(500, 2000)).ContinueWith(t => "3.0.0.0");
        }
    }
}
