using StampaRegistri.LogicaApplicativa;
using StampaRegistri.Oggetti;
using StampaRegistri.Utils;
using System;
using System.IO;

namespace StampaRegistri
{
	internal class StampaDaConsole
	{
		private static Configuration _conf;

		private static Docspa docspa;

		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(Trace.GetDescrizioneMessaggio(90));
				StampaDaConsole._conf = new Configuration();
				bool flag = true;
				string empty = string.Empty;
				Console.WriteLine(Trace.GetDescrizioneMessaggio(91));
				if (FSO.EsisteFile(StampaDaConsole._conf.Log_File))
				{
					string pathCompletoFile = FSO.GetPathCompletoFile(StampaDaConsole._conf.HistoryLog_PathFolder, FSO.GetTimeFileName(StampaDaConsole._conf.HistoryLog_FilePrefix, "Log"));
					if (pathCompletoFile != null)
					{
						File.Move(StampaDaConsole._conf.Log_File, pathCompletoFile);
					}
					else
					{
						File.Move(StampaDaConsole._conf.Log_File, FSO.GetPathCompletoFile(StampaDaConsole._conf.Log_PathFolder, FSO.GetTimeFileName(StampaDaConsole._conf.HistoryLog_FilePrefix, "Log")));
						flag = false;
					}
				}
				Costanti.DispositiviDiLog log_Device = StampaDaConsole._conf.Log_Device;
				Trace.Traccia(207, "Main()", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
				if (log_Device != StampaDaConsole._conf.Log_Device || StampaDaConsole._conf.Log_Device == Costanti.DispositiviDiLog.Nessuno)
				{
					if (StampaDaConsole._conf.Log_Device == Costanti.DispositiviDiLog.Nessuno)
					{
						Console.WriteLine("Attenzione: Non è stato selezionato alcun dispositivo di Log. \n Tutti i messaggi verranno mostrati a video.");
					}
					string text;
					do
					{
						Console.WriteLine("Vuoi Continuare il processo ugualmente?(premere S o N seguito da INVIO)");
						text = Console.ReadLine();
					}
					while (text.ToUpper() != "N" && text.ToUpper() != "S");
					if (text.ToUpper().Equals("N"))
					{
						Trace.Traccia(209, "Dispositivo di log non valido, è stata richiesta l'uscita dal programma.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
						return;
					}
					Trace.Traccia(209, "Dispositivo di log non valido, è stata richiesta la continuazione del processo.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
				}
				Trace.Traccia(Costanti.Informazioni.Work_InizioProcesso, StampaDaConsole._conf);
				Trace.Traccia(Costanti.Informazioni.Work_InizioProcesso, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
				if (!flag)
				{
					Trace.Traccia(Costanti.Informazioni.Work_StoricLog_Fallito, StampaDaConsole._conf);
					Trace.Traccia(Costanti.Informazioni.Work_StoricLog_Fallito, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
				}
				Trace.Traccia(209, "Verifica esistenza dei parametri di configurazione.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
				if (StampaDaConsole._conf.Errore)
				{
					Trace.Traccia(StampaDaConsole._conf.DescrizioneErrore, Costanti.TipoMessaggio.ERRORE, StampaDaConsole._conf);
					flag = false;
				}
				Trace.Traccia(209, "Verifica sintattica dei parametri di configurazione.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
				if (StampaDaConsole._conf.HistoryLog_PathFolder == null)
				{
					Trace.Traccia(Costanti.Errori.Config_PathHistoryLog_NonValido, StampaDaConsole._conf);
					flag = false;
				}
				if (StampaDaConsole._conf.Docspa_IDAmm.Equals(string.Empty))
				{
					Trace.Traccia(Costanti.Errori.Config_IDAmm_NonValido, StampaDaConsole._conf);
					flag = false;
				}
				if (StampaDaConsole._conf.Docspa_UserName.Equals(string.Empty))
				{
					Trace.Traccia(Costanti.Errori.Config_UserName_NonValido, StampaDaConsole._conf);
					flag = false;
				}
				if (StampaDaConsole._conf.Docspa_PWD.Equals(string.Empty))
				{
					Trace.Traccia(Costanti.Errori.Config_Password_NonValido, StampaDaConsole._conf);
				}
				if (StampaDaConsole._conf.Docspa_Ruolo_IDCorr.Equals(string.Empty))
				{
					Trace.Traccia(Costanti.Errori.Config_IdRuolo_NonValido, StampaDaConsole._conf);
				}
				if (StampaDaConsole._conf.Docspa_Versione == 0)
				{
					Trace.Traccia(Costanti.Errori.Config_VersioneNonValida, StampaDaConsole._conf);
					flag = false;
				}
				if (StampaDaConsole._conf.Docspa_IDRegistro.Equals(string.Empty))
				{
					Trace.Traccia(Costanti.Errori.Config_Registro_Vuoto, StampaDaConsole._conf);
				}
				try
				{
					Convert.ToBoolean(StampaDaConsole._conf.Work_ForzaChiusuraReg);
				}
				catch
				{
					Trace.Traccia(Costanti.Errori.Config_FlagForzaChiusuraRegNonValido, StampaDaConsole._conf);
					flag = false;
				}
				try
				{
					Convert.ToBoolean(StampaDaConsole._conf.Work_ApriRegDopoProcesso);
				}
				catch
				{
					Trace.Traccia(Costanti.Errori.Config_FlagAperturaRegDopoProcNonValido, StampaDaConsole._conf);
					flag = false;
				}
				if (StampaDaConsole._conf.Docspa_TimeoutRichiestaWSInMinuti == 0)
				{
					Trace.Traccia(Costanti.Errori.Config_TimeoutRichiestaWS_NonValido, StampaDaConsole._conf);
					flag = false;
				}
				if (!flag)
				{
					Trace.Traccia(Costanti.Informazioni.Config_ParametriNonValidi, StampaDaConsole._conf);
					Trace.Traccia(Costanti.Informazioni.Config_ParametriNonValidi, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
				}
				else
				{
					Trace.Traccia(209, "verifica esistenza dei WS di Docspa.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
					try
					{
						int docspa_Versione = StampaDaConsole._conf.Docspa_Versione;
						if (docspa_Versione != 25)
						{
							if (docspa_Versione != 305)
							{
								Trace.Traccia(Costanti.Errori.Config_VersioneNonValida, StampaDaConsole._conf);
							}
							else
							{
								StampaDaConsole.docspa = new DocsPa305(StampaDaConsole._conf);
								Trace.Traccia(103, "3.05", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
							}
						}
						else
						{
							StampaDaConsole.docspa = new DocsPa25(StampaDaConsole._conf);
							Trace.Traccia(103, "2.5", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
						}
					}
					catch (Exception ex)
					{
						Trace.Traccia(ex, StampaDaConsole._conf);
						StampaDaConsole.docspa = null;
					}
					if (StampaDaConsole.docspa == null)
					{
						Trace.Traccia(Costanti.Informazioni.Docspa_ConnessioneNonRiuscita, StampaDaConsole._conf);
						Trace.Traccia(Costanti.Informazioni.Docspa_ConnessioneNonRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
					}
					else
					{
						Trace.Traccia(Costanti.Informazioni.Docspa_ConnessioneRiuscita, StampaDaConsole._conf);
						Trace.Traccia(Costanti.Informazioni.Docspa_ConnessioneRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
						StampaDaConsole.docspa.Login(StampaDaConsole._conf.Docspa_UserName, StampaDaConsole._conf.Docspa_PWD, StampaDaConsole._conf.Docspa_IDAmm, StampaDaConsole._conf.Docspa_LoginForzata, StampaDaConsole._conf.Docspa_Ruolo_IDCorr);
						if (!StampaDaConsole.docspa.AutenticatoSuDocsPa)
						{
							if (StampaDaConsole.docspa.Errore)
							{
								Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
							}
							Trace.Traccia(Costanti.Informazioni.Docspa_LoginNonRiuscita, StampaDaConsole._conf);
							Trace.Traccia(Costanti.Informazioni.Docspa_LoginNonRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
						}
						else
						{
							Trace.Traccia(Costanti.Informazioni.Docspa_LoginRiuscita, StampaDaConsole._conf);
							Trace.Traccia(Costanti.Informazioni.Docspa_LoginRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
							StampaDaConsole.docspa.GetRegistro(StampaDaConsole._conf.Docspa_IDRegistro);
							if (StampaDaConsole.docspa.Errore)
							{
								Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
								Trace.Traccia(Costanti.Informazioni.Docspa_CaricamentoRegistroNonRiuscito, StampaDaConsole._conf);
								Trace.Traccia(Costanti.Informazioni.Docspa_CaricamentoRegistroNonRiuscito, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
							}
							else
							{
								Trace.Traccia(Costanti.Informazioni.Docspa_CaricamentoRegistroRiuscito, StampaDaConsole._conf);
								Trace.Traccia(Costanti.Informazioni.Docspa_CaricamentoRegistroRiuscito, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
								StampaDaConsole.docspa.GetStatoRegistro(out empty);
								if (StampaDaConsole.docspa.Errore)
								{
									Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
									Trace.Traccia(Trace.GetDescrizioneMessaggio(113), Costanti.TipoMessaggio.ERRORE, StampaDaConsole._conf);
									Trace.Traccia(Costanti.Informazioni.Docspa_CaricamentoStatoRegistroNonRiuscito, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
								}
								else
								{
									Trace.Traccia(Trace.GetDescrizioneMessaggio(114), Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
									Trace.Traccia(209, "stato registro:" + empty, Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
									string a;
									if ((a = empty) != null)
									{
										if (!(a == "A"))
										{
											if (a == "C")
											{
												Trace.Traccia(209, "Registro già chiuso.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
												flag = true;
												goto IL_66F;
											}
										}
										else
										{
											Trace.Traccia(209, "Registro Aperto.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
											if (Convert.ToBoolean(StampaDaConsole._conf.Work_ForzaChiusuraReg))
											{
												Trace.Traccia(209, "richiesta chiusura.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
												flag = StampaDaConsole.docspa.CambiaStatoRegistro("C");
												goto IL_66F;
											}
											Trace.Traccia(Costanti.Informazioni.Docspa_ChiusuraRegistroNonRichiesta, StampaDaConsole._conf);
											flag = false;
											goto IL_66F;
										}
									}
									Trace.Traccia(39, empty, Costanti.TipoMessaggio.ERRORE, StampaDaConsole._conf);
									flag = false;
									IL_66F:
									if (StampaDaConsole.docspa.Errore)
									{
										Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
										Trace.Traccia(Costanti.Informazioni.Docspa_ChiusuraRegistroNonRiuscita, StampaDaConsole._conf);
										Trace.Traccia(Costanti.Informazioni.Docspa_ChiusuraRegistroNonRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
									}
									else if (!flag)
									{
										Trace.Traccia(Costanti.Informazioni.Docspa_ChiusuraRegistroNonRiuscita, StampaDaConsole._conf);
										Trace.Traccia(Costanti.Informazioni.Docspa_ChiusuraRegistroNonRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
									}
									else
									{
										Trace.Traccia(Costanti.Informazioni.Docspa_ChiusuraRegistroRiuscita, StampaDaConsole._conf);
										Trace.Traccia(Costanti.Informazioni.Docspa_ChiusuraRegistroRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
										string empty2 = string.Empty;
										string empty3 = string.Empty;
										string empty4 = string.Empty;
										Trace.Traccia(Costanti.Informazioni.Work_StampaRegInCorso, StampaDaConsole._conf);
										Trace.Traccia(Costanti.Informazioni.Work_StampaRegInCorso, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
										flag = StampaDaConsole.docspa.StampaRegistro(out empty2, out empty3, out empty4);
										if (StampaDaConsole.docspa.Errore)
										{
											Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
											Trace.Traccia(Costanti.Informazioni.Docspa_StampaRegistroNonRiuscita, StampaDaConsole._conf);
											Trace.Traccia(Costanti.Informazioni.Docspa_StampaRegistroNonRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
										}
										else
										{
											Trace.Traccia(Costanti.Informazioni.Docspa_StampaRegistroRiuscita, StampaDaConsole._conf);
											Trace.Traccia(Costanti.Informazioni.Docspa_StampaRegistroRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
											Trace.Traccia(121, string.Format("\n Ruolo: {0} \n Registro: {1} \n IDDocumento: {2}", empty4, empty2, empty3), Costanti.TipoMessaggio.INFORMAZIONE, StampaDaConsole._conf);
											Trace.Traccia(121, string.Format("\n Ruolo: {0} \n Registro: {1} \n IDDocumento: {2}", empty4, empty2, empty3), Costanti.TipoMessaggio.INFORMAZIONE, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
										}
										empty = string.Empty;
										StampaDaConsole.docspa.GetStatoRegistro(out empty);
										if (StampaDaConsole.docspa.Errore)
										{
											Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
											Trace.Traccia(Trace.GetDescrizioneMessaggio(113), Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
										}
										else
										{
											Trace.Traccia(Trace.GetDescrizioneMessaggio(114), Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
											Trace.Traccia(209, "stato registro:" + empty, Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
											string a2;
											if ((a2 = empty) != null)
											{
												if (!(a2 == "C"))
												{
													if (a2 == "A")
													{
														flag = true;
														goto IL_8D2;
													}
												}
												else
												{
													if (Convert.ToBoolean(StampaDaConsole._conf.Work_ApriRegDopoProcesso))
													{
														Trace.Traccia(209, "richiesta apertura registro.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
														flag = StampaDaConsole.docspa.CambiaStatoRegistro("A");
														goto IL_8D2;
													}
													Trace.Traccia(209, "non richiesta apertura registro.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
													Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroNonRichiesta, StampaDaConsole._conf);
													Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroNonRichiesta, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
													flag = false;
													goto IL_8D2;
												}
											}
											Trace.Traccia(39, empty, Costanti.TipoMessaggio.ERRORE, StampaDaConsole._conf);
											flag = false;
											IL_8D2:
											if (StampaDaConsole.docspa.Errore)
											{
												Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
												Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroNonRiuscita, StampaDaConsole._conf);
												Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroNonRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
											}
											else if (!flag)
											{
												Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroNonRiuscita, StampaDaConsole._conf);
												Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroNonRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
											}
											else
											{
												Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroRiuscita, StampaDaConsole._conf);
												Trace.Traccia(Costanti.Informazioni.Docspa_AperturaRegistroRiuscita, StampaDaConsole._conf, Costanti.DispositiviDiLog.Console);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				if (StampaDaConsole.docspa != null && StampaDaConsole.docspa.Errore)
				{
					Trace.Traccia(StampaDaConsole.docspa.CodiceErrore, StampaDaConsole._conf);
				}
				else
				{
					Trace.Traccia(ex2, StampaDaConsole._conf);
				}
			}
			finally
			{
				try
				{
					Trace.Traccia(209, "Richiesto il LogOut a DocsPa.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
					if (StampaDaConsole.docspa != null && StampaDaConsole.docspa.AutenticatoSuDocsPa)
					{
						StampaDaConsole.docspa.Logout();
					}
				}
				catch (Exception ex3)
				{
					Trace.Traccia(ex3, StampaDaConsole._conf);
				}
				finally
				{
					Trace.Traccia(209, "richiesta chiususa connessione ai WS.", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
					if (StampaDaConsole.docspa != null)
					{
						StampaDaConsole.docspa.Close();
					}
					Trace.Traccia(Costanti.Informazioni.Work_FineProcesso, StampaDaConsole._conf);
					Console.WriteLine(Trace.GetDescrizioneMessaggio(106));
					if (StampaDaConsole._conf.Work_ConfermaChiusuraDopoProcesso)
					{
						Console.Read();
					}
					Trace.Traccia(208, "Main()", Costanti.TipoMessaggio.DEBUG, StampaDaConsole._conf);
				}
			}
		}
	}
}
