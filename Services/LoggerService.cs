using MELI.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MELI.Services
{
    public class LoggerService : ILoggerService
    {
        //public static string logPath { get; set; } = Application.StartupPath + @"\Log\";
        private string logPath { get; set; }
        public string logFile { get; set; } /*= logPath + "log.txt";*/
        public string line = "\r\n------------------------------------------------------\r\n";
        string contentRoot = "";


        public LoggerService(IConfiguration configuration)
        {
            contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey).ToString();
            createFile(contentRoot + @"\log");

            logPath = contentRoot + @"\Log\"; ;
            logFile = logPath + "log.txt";
            checkDir();
        }

        public void checkDir()
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        public string createFile(string name)
        {
            string aux = logPath + $"{name}.txt";
            if (!File.Exists(aux))
            {
                FileStream f = File.Create(aux);
                f.Close();
            }
            return aux;
        }

        public void recordLogError(string modulo, string err, string user)
        {
            checkDir();

            string file = createFile(DateTime.Now.ToString("yyyy_MM_dd"));

            using (StreamWriter logWriter = new StreamWriter(file, true))
            {
                logWriter.WriteLine("*Usuario: " + user + $"\r\n*Horario error: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"\r\n*Modulo: {modulo} \r\n*Error: {err} {line}");
            }
        }

        public async Task recordLogMsj(string modulo, string msj, string user)
        {
            checkDir();

            string file = createFile(DateTime.Now.ToString("yyyy_MM_dd"));

            using (StreamWriter logWriter = new StreamWriter(file, true))
            {
                await logWriter.WriteLineAsync("*Usuario: " + user + $"\r\n*Horario msj: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"\r\n*Modulo: {modulo} \r\n*Msj: {msj} {line}");
            }
        }

        public List<string> getLogFiles()
        {
            List<string> resp = new List<string>();
            
            try
            {
                List<string> listado = Directory.GetFiles(logPath).OrderByDescending(T => T).ToList();

                foreach (string path in listado)
                {
                    List<string> box = path.Split("\\").ToList();
                    int largo = box.Count();
                    resp.Add(box[largo - 1]);
                }

                return resp;
            }
            catch (Exception ex)
            {
                using (StreamWriter logWriter = new StreamWriter(contentRoot + @"\log.txt", true))
                {
                    logWriter.WriteLine("err: " + ex.Message + "\r\n" ) ;
                }

                resp.Add("err:"+ex.Message);
                
                return resp;
            }
        }

        public async Task<List<ErrLog>> getLogData(string dia)
        {
            try
            {
                using (StreamReader logReader = new StreamReader(logPath + dia))
                {
                    string box = await logReader.ReadToEndAsync();
                    List<string> aux = box.Split(line).ToList();
                    List<ErrLog> resp = new List<ErrLog>();

                    foreach (string log in aux)
                    {
                        if (log.Length > 5)
                        {
                            resp.Add(new ErrLog(log.Replace("\r\n", "")));
                        }
                    }

                    return resp;
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logWriter = new StreamWriter(contentRoot + @"\log.txt", true))
                {
                    logWriter.WriteLine("err: " + ex.Message + "\r\n");
                }

                List<ErrLog> resp = new List<ErrLog>();
                resp.Add(new ErrLog(ex));
                return resp;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetLastMethodName()
        {
            var st = new StackTrace(new StackFrame(1));
            return st.GetFrame(0).GetMethod().Name;
        }
    }

    public class ErrLog
    {
        public string Usuario { get; set; }
        public string Horario { get; set; }
        public string Modulo { get; set; }
        public string Error { get; set; }

        public ErrLog(string log)
        {
            List<string> aux = log.Split('*').ToList();
            this.Usuario = aux[1].Split(':',2)[1];
            this.Horario = aux[2].Split(':',2)[1];
            this.Modulo = aux[3].Split(':',2)[1];
            this.Error = aux[4].Split(':', 2)[1];
        }

        public ErrLog(Exception ex)
        {
            this.Usuario = "System";
            this.Horario = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.Modulo = "LoggerService";
            this.Error = ex.Message;
        }

    }
}