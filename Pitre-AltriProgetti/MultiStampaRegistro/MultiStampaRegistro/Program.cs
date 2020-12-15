using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using log4net.Config;

namespace MultiStampaRegistro
{

    class RunResults
    {
        public int ExitCode;
        public Exception RunException;
        public StringBuilder Output;
        public StringBuilder Error;
    }

    class Program
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

        public static void writeLog(String message)
        {
            log.Info(message);
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            string myDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string configFile = myDir + "\\Config\\multistampa.ini";
            string modelFile = myDir + "\\Config\\model.config";
            string printBinary = myDir + "\\Stamparegistri\\StampaRegistri.exe";
            string printWorkdir = myDir + "\\Stamparegistri";
            string printConfigFile = myDir + "\\Stamparegistri\\StampaRegistri.exe.config";
            string printUserPrefix = String.Empty;
            string printPassword = String.Empty;
            ArrayList admBlackList = new ArrayList();
            DateTime d = DateTime.Now;
            string filename = myDir + "\\" + d.ToString("yyyyMMdd") + "_multistampa.log";

            try
            {
                writeLog("Multistampa registro v1.0.7\nReading config...");

                ConfigManager config = new ConfigManager(configFile);

                if (config.ExistKey("PRINTUSERPREFIX"))
                    printUserPrefix = config.GetValue("PRINTUSERPREFIX");

                if (config.ExistKey("PRINTPWD"))
                    printPassword = config.GetValue("PRINTPWD");

                if (config.ExistKey("BLACKLIST"))
                {
                    string val = config.GetValue("BLACKLIST");

                    admBlackList.AddRange(val.Split(';'));
                    
                    writeLog(admBlackList.Count + " blacklisted administrations found.");
                }

                DocsPaWR.Amministrazione[] ammWSlist = null;
                string resultmsg = string.Empty;

                DocsPaWR.DocsPaWebService WS = new DocsPaWR.DocsPaWebService();
                WS.Timeout = System.Threading.Timeout.Infinite;
                ammWSlist = WS.amministrazioneGetAmministrazioni(out resultmsg);

                writeLog(ammWSlist.Length + " administrations found.");

                int okCount = 0;
                int errCount = 0;
                int blkCount = 0;
                int unknCount = 0;

                foreach (DocsPaWR.Amministrazione amm in ammWSlist)
                {
                    writeLog("-----------------------------------");

                    string idAmm = "" + amm.systemId;
                    string codAmm = "" + amm.codice;
                    string descAmm = "" + amm.descrizione;
                    string printUser = codAmm + printUserPrefix;
                    string logPath = codAmm;
                    string logPathHistory = codAmm + "\\History";
                    string ipaddress = String.Empty;

                    DocsPaWR.Utente utente = null;
                    DocsPaWR.UserLogin userLogin = new DocsPaWR.UserLogin();
                    userLogin.UserName = printUser;
                    userLogin.Password = printPassword;
                    userLogin.IdAmministrazione = amm.systemId;

                    DocsPaWR.LoginResult lr = WS.Login(userLogin, true, Guid.NewGuid().ToString(), out utente, out ipaddress);

                    if (utente == null) writeLog("Utente non loggato in DocsPA: " + printUser + " per idAmm: " + amm.systemId);
                    else
                    {
                        WS.Logoff(utente.userId, utente.idAmministrazione, utente.sessionID, utente.dst);

                        // verificare che ruoli e registri siano valorizzati
                        if (utente.ruoli == null || utente.ruoli[0] == null) writeLog("Ruoli non configurati per utente: " + printUser + " per idAmm: " + amm.systemId);
                        else
                        {
                            if (utente.ruoli[0].registri == null || utente.ruoli[0].registri[0] == null) writeLog("Registri non configurati per utente: " + printUser + " per idAmm: " + amm.systemId);
                            else
                            {
                                foreach (DocsPaWR.Registro registro in utente.ruoli[0].registri)
                                {
                                    string idRegistro = registro.systemId;

                                    // Skip if blacklisted
                                    if (admBlackList.Contains(codAmm))
                                    {
                                        writeLog("Skipped print for " + codAmm + " - " + descAmm + ". Blacklisted.");
                                        blkCount++;
                                        continue;
                                    }

                                    // Delete current print config
                                    if (System.IO.File.Exists(printConfigFile))
                                        File.Delete(printConfigFile);

                                    String logString = "Writing config for " + codAmm + " - " + descAmm + " (" + idAmm + ") " + "user: " + printUser;

                                    writeLog(logString);

                                    // Build new config file
                                    StreamWriter configWriter = new System.IO.StreamWriter(printConfigFile, true);
                                    foreach (string line in File.ReadAllLines(modelFile))
                                    {
                                        string newline = line.Replace("_IDAMM_", idAmm);
                                        newline = newline.Replace("_USERNAME_", printUser);
                                        newline = newline.Replace("_PASSWORD_", printPassword);
                                        newline = newline.Replace("_LOGPATH_", logPath);
                                        newline = newline.Replace("_LOGPATHHISTORY_", logPathHistory);
                                        newline = newline.Replace("_IDREGISTRO_", idRegistro);
                                        configWriter.WriteLine(newline);
                                    }

                                    configWriter.Flush();
                                    configWriter.Close();

                                    writeLog("Running print service...");

                                    RunResults exeResult = null;
                                    try
                                    {
                                        exeResult = RunExecutable(printBinary, "", printWorkdir);
                                    }
                                    catch (Exception e)
                                    {
                                        writeLog("Error running print service:" + e.Message);
                                    }

                                    string output = exeResult.Output.ToString();
                                    writeLog(output);

                                    if (output.Contains("Docspa fallita"))  // login failed
                                    {
                                        writeLog("Print ERROR - Login failed");
                                        errCount++;
                                    }
                                    else if (output.Contains("Stampa del registro fallita"))  // nothing to print or file chars error (invalid chars in reg desc)
                                    {
                                        if (output.Contains("Non ci sono protocolli da stampare"))
                                        {
                                            writeLog("Print OK");
                                            okCount++;
                                        }
                                        else
                                        {
                                            writeLog("Print ERROR - Check print log for details");
                                            errCount++;
                                        }
                                    }
                                    else if (output.Contains("Docspa eseguita")) // login ok
                                    {
                                        writeLog("Print OK");
                                        okCount++;
                                    }
                                    else
                                    {
                                        writeLog("Print returned an unknown status. See print log for details.");
                                        unknCount++;
                                    }

                                    // Console.ReadKey();
                                }
                            }
                        }
                    }
                }

                writeLog("-----------------------------------");
                writeLog("Print process finished. OK: " + okCount + ", ERRORS: " + errCount + ", BLACKLISTED: " + blkCount + ", UNKNOWN: " + unknCount + " over a total of " + ammWSlist.Length);
            }
            catch (Exception e)
            {
                writeLog("Ex: " + e.Message + "\nStack: " + e.StackTrace);
            }
        }


        // run an executable and capture the output
        public static RunResults RunExecutable(string executablePath, string arguments, string workingDirectory)
        {

            RunResults runResults = new RunResults
            {

                Output = new StringBuilder(),
                Error = new StringBuilder(),
                RunException = null
            };

            try
            {
                if (File.Exists(executablePath))
                {
                    using (Process proc = new Process())
                    {
                        proc.StartInfo.FileName = executablePath;
                        proc.StartInfo.Arguments = arguments;
                        proc.StartInfo.WorkingDirectory = workingDirectory;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.RedirectStandardError = true;
                        proc.OutputDataReceived +=
                            (o, e) => runResults.Output.Append(e.Data).Append(Environment.NewLine);
                        proc.ErrorDataReceived +=
                            (o, e) => runResults.Error.Append(e.Data).Append(Environment.NewLine);
                        proc.Start();
                        proc.BeginOutputReadLine();
                        proc.BeginErrorReadLine();
                        proc.WaitForExit();
                        runResults.ExitCode = proc.ExitCode;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid executable path.", "executablePath");
                }

            }
            catch (Exception e)
            {
                runResults.RunException = e;
            }

            return runResults;



        }
    }
}
