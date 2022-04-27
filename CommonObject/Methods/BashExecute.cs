using System.Diagnostics;

namespace CommonObject.Methods
{
    public class BashExecute
    {
        /// <summary>
        /// 执行bash command,接收结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Bash(string cmdInput, bool unix=true)
        {

            var escapedArgs = unix ? $"-c \"{cmdInput.Replace("\"", "\\\"")}\"" : cmdInput;
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = unix ? "/bin/bash" : "cmd.exe",
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string res = process.StandardOutput.ReadToEnd();
            string resErr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return string.IsNullOrWhiteSpace(resErr) ? res : resErr;
        }
    }
}
