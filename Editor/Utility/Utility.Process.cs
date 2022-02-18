using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Editor
{
    public static partial class Utility
    {
        public static class Process
        {
            /// <summary>
            /// 异步执行命令行
            /// </summary>
            /// <param name="workingDirectory">工作路径</param>
            /// <param name="fileName">文件名</param>
            /// <param name="arguments">参数</param>
            /// <returns></returns>
            public static Task<string> InvokeProcessAsync(string workingDirectory, string fileName, string arguments)
            {
                var psi = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                };

                System.Diagnostics.Process p;
                try
                {
                    p = System.Diagnostics.Process.Start(psi);
                }
                catch (Exception ex)
                {
                    return Task.FromException<string>(ex);
                }

                var tcs = new TaskCompletionSource<string>();
                p.EnableRaisingEvents = true;
                p.Exited += (object sender, System.EventArgs e) =>
                {
                    var data = p.StandardOutput.ReadToEnd();
                    p.Dispose();
                    p = null;

                    tcs.TrySetResult(data);
                };

                return tcs.Task;
            }
        }
    }
}