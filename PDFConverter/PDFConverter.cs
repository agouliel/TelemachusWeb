using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PDFConverter
{
    public static class PDFConverter
    {
        public static async Task<FileInfo> Convert(string rootpath, string htmlContent)
        {

            var processPath = Path.Combine(rootpath, "Rotativa", "Windows", "wkhtmltopdf.exe");

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (!isWindows)
            {
                processPath = Path.Combine(rootpath, "Rotativa", "Linux", "wkhtmltopdf-amd64");
            }

            if (!File.Exists(processPath))
            {
                throw new Exception("wkhtmltopdf not found, searched for " + processPath);
            }
            var tempPath = Path.Combine(Path.GetTempPath(), "wkhtmltopdf");
            if (Directory.Exists(tempPath) == false)
            {
                Directory.CreateDirectory(tempPath);
            }
            var libraryPath = Path.Combine(rootpath, "Rotativa", "lib");
            var bootstrapPath = Path.Combine(tempPath, "bootstrap.min.css");
            if (File.Exists(bootstrapPath) == false)
            {
                File.Copy(Path.Combine(libraryPath, "bootstrap.min.css"), bootstrapPath);
            }
            var outputBaseName = Guid.NewGuid();
            var inputFile = Path.Combine(tempPath, $"{outputBaseName}.html");
            var exportFile = Path.Combine(tempPath, $"{outputBaseName}.pdf");
            if (File.Exists(inputFile))
            {
                File.Delete(inputFile);
            }
            await File.WriteAllTextAsync(inputFile, htmlContent);
            string error = "";
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = processPath;
            p.StartInfo.WorkingDirectory = tempPath;
            p.StartInfo.Arguments = $"-n -q -l --zoom 1 --header-right \"[title]\" --footer-center \"Page [page] of [topage]\" --header-font-size 9 --footer-font-size 9 --print-media-type --disable-smart-shrinking -O landscape --enable-local-file-access {outputBaseName}.html {outputBaseName}.pdf";
            p.ErrorDataReceived += (sender, args) =>
            {
                error += args.Data;
                Debug.WriteLine(args.Data);
            };
            p.Start();
            string output = await p.StandardOutput.ReadToEndAsync();
            p.WaitForExit(60000);
            FileInfo pdf = null;
            //if (!(p.ExitCode != 0)) 
            pdf = new FileInfo(exportFile);
            if (File.Exists(inputFile))
            {
                File.Delete(inputFile);
            }
            //if (p.ExitCode != 0)
            //{
            //    throw new Exception(error + Environment.NewLine + processPath + Environment.NewLine + tempPath + Environment.NewLine + outputBaseName + Environment.NewLine + inputFile + exportFile);
            //}
            return pdf;
        }
    }
}
