using PlaywrightSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace WinInstall.Services
{
    class Downloader
    {
        public async Task<string> FromUrl(string url, string userAgent = null)
        {
            using (var client = new HttpClient())
            {
                if (userAgent != null)
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);

                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    var downloadedFile = Path.Combine(Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())).FullName, await GetFileName(url, userAgent));

                    await using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    await using (var streamToReadTo = File.Open(downloadedFile, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToReadTo);
                    }

                    return downloadedFile;
                }
            }
        }

        public async Task<string> GetHtml(string url)
        {
            await Playwright.InstallAsync();
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            await page.WaitForNavigationAsync();
            return await page.GetContentAsync();
        }

        private async Task<string> GetFileName(string url, string userAgent = null)
        {
            using (var client = new HttpClient())
            {
                if (userAgent != null)
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);

                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    string fileName = null;

                    if (response.Content.Headers.ContentDisposition != null)
                        fileName = response.Content.Headers.ContentDisposition.FileName;
                    else if (new Regex(@"^.+\.[A-Za-z]{1,6}$").IsMatch(Path.GetFileName(url)))
                        fileName = Path.GetFileName(url);
                    else
                        fileName = response.RequestMessage.RequestUri.Segments[^1];

                    return HttpUtility.UrlDecode(fileName).Replace("\"", "").Replace("'", "").Replace(" ", "");
                }
            }
        }
    }
}