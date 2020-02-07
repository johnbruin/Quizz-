using System;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Quizz_
{
    public class SID : IDisposable
    {
        private Thread t;
        private Process proc;

        public SID()
        {
            proc = new Process();
            t = new Thread(new ThreadStart(Execute));
            stop();
        }

        public void play()
        {
            try
            {
                t.Start();
            }
            catch
            {
            }
        }

        public void stop()
        {
            try
            {
                Process[] myProcesses;
                // Returns array containing all instances of Notepad.
                myProcesses = Process.GetProcessesByName("sidplay2");
                foreach (Process myProcess in myProcesses)
                {
                    myProcess.Kill();
                }

                foreach (string strFile in Directory.GetFiles(@"..\..\music\", "*.dat"))
                {
                    try
                    {
                        File.Delete(strFile);
                    }
                    catch { }
                }
            }
            catch { }
        }

        public string Filename
        {
            get;
            set;
        }

        public int Seconds
        {
            get;
            set;
        }

        private void Execute()
        {
            try
            {
                //Process proc = new Process();
                using (proc)
                {
                    proc.StartInfo.FileName = @"..\..\music\sidplay2.exe";
                    proc.StartInfo.WorkingDirectory = @"..\..\music\";
                    if (Seconds > 0)
                    {
                        proc.StartInfo.Arguments = "-t" + Seconds.ToString() + " " + this.Filename;
                    }
                    else
                    {
                        proc.StartInfo.Arguments = this.Filename;
                    }
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = false;
                    proc.StartInfo.RedirectStandardError = false;
                    proc.StartInfo.CreateNoWindow = true;
                    try
                    {
                        proc.Start();
                    }
                    catch
                    {
                        proc.Start();
                    }
                }
            }
            catch { }
        }

        #region IDisposable Members

        public void Dispose()
        {
            stop();
        }

        #endregion
    }
}
