using System;
using System.Diagnostics;
using System.IO;

namespace Agentic.Tools
{
    public class PwshTool : ITool
    {
        public string Tool { get; set; } = "pwsh";
        public string Description { get; set; } = "Executes a PowerShell script, file management, systems management, access external resources and anything else";
        public bool RequireConfirmation { get; } = true;
        public ToolParameter<string> Script { get; set; }

        public string Invoke(ToolExecutionContext context)
        {
            var script = Script.Value;

            // Save script to a temporary file
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ps1");
            try
            {
                File.WriteAllText(tempFilePath, script);

                // Execute the script
                string result = ExecutePwshScript(tempFilePath);

                // Clean up: delete the temporary file
                File.Delete(tempFilePath);

                return result;
            }
            catch (Exception ex)
            {
                // Ensure temporary file is deleted even if an exception occurs
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
                return $"Exception: {ex.Message}";
            }
        }

        private static string ExecutePwshScript(string scriptFilePath)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "pwsh";
                    process.StartInfo.Arguments = $"-NoProfile -NoLogo -File \"{scriptFilePath}\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(errors))
                    {
                        return $"Error: {errors}";
                    }
                    return output.Trim();
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
