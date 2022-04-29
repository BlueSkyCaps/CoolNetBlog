using CommonObject.Constructs;
using System.Diagnostics;
using System.Text;

namespace CommonObject.Methods
{
    public class BashExecute
    {
        /// <summary>
        /// 执行bash command,接收结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ValueResult Bash(string cmdInput, bool unix=true)
        {
            ValueResult result = new ValueResult();
            var escapedArgs = unix ? $"-c \"{cmdInput.Replace("\"", "\\\"")}\"" : "/C " + cmdInput;
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = unix ? "/bin/bash" : "cmd.exe",
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            var errPut = new StringBuilder();
            process.ErrorDataReceived += (sender, args) => errPut.AppendLine(args.Data);
            var output = "";
            try
            {
                process.Start();
                process.BeginErrorReadLine();
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                result.Code = Enums.ValueCodes.Error;
                result.HideMessage = "执行bash命令发生异常: " + e.Message;
                return result;
            }

            if (process.ExitCode != 0)
            {
                result.Code = Enums.ValueCodes.UnKnow;
                result.HideMessage = "执行bash命令得到未知结果，code:" + process.ExitCode + ";结果: " + errPut;
                return result;
            }
            result.Code = Enums.ValueCodes.Success;
            result.TipMessage = output;
            result.HideMessage = output;
            return result;
        }
    }
}
