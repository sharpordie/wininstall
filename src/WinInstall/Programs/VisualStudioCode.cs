using System;
using System.Threading.Tasks;

namespace WinInstall.Programs
{
    class VisualStudioCode : Program
    {
        public override string Name => "Visual Studio Code";

        public override string Info => "Free source-code editor made by Microsoft.";

        public override string Type => "Development";

        public override string Root => "C:\\";

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
            return await Task.Delay(new Random().Next(500, 2000)).ContinueWith(t => "4.0.0.0");
        }
    }
}
