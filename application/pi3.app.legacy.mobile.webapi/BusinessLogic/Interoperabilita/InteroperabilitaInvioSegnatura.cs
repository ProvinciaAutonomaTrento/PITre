// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.Interoperabilita.Semplificata;
using BusinessLogic.Utenti;
using DocsPaVO.Interoperabilita;
using DocsPaVO.utente;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico;
using Serilog;
using System.Collections;
using System.Xml;

namespace BusinessLogic.Interoperabilita;

public class InteroperabilitaInvioSegnatura
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(InteroperabilitaInvioSegnatura));
    private static System.Threading.Mutex semInterOper = new System.Threading.Mutex();

    public static byte[] CreaSegnaturaProtocollo(DocsPaVO.utente.Corrispondente mittSegnatura, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Registro regMittente, string docnumber, bool confermaRic, out DocsPaVO.documento.ResultSigilloElettronico resultSigillo,
       ISigilloElettronicoService sigilloElettronicoService, string pathFiles = "", Dictionary<string, string> CoppiaNomeFileENomeOriginale = null, string mailAddress = "")
    {
        byte[] contentSigned = null;
        resultSigillo = DocsPaVO.documento.ResultSigilloElettronico.OK;

        try
        {
            DocsPaVO.amministrazione.InfoAmministrazione infoAmministrazione = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
            if (string.IsNullOrEmpty(reg.codiceIpa))
                reg = BusinessLogic.Utenti.RegistriManager.getRegistro(reg.systemId);
            DocsPaVO.Interoperabilita.Segnatura.SegnaturaInformaticaType segnatura = CreaSegnaturaProtocolloXml(mittSegnatura, infoUtente, reg, regMittente, docnumber, confermaRic, pathFiles, CoppiaNomeFileENomeOriginale, mailAddress);
            string segnaturaXml = BusinessLogic.XmlParsing.StringWriterWithEncoding.ToXmlString(segnatura);
            byte[] content = System.Text.Encoding.UTF8.GetBytes(segnaturaXml);

            //Se la chiave è attiva, applico la firma al file di segnatura
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_APPLICA_SIGILLO_SEGNATURA")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_APPLICA_SIGILLO_SEGNATURA").Equals("1"))
            {
                try
                {
                    string statusCode = string.Empty;
                    //contentSigned = BusinessLogic.Documenti.DigitalSignature.RemoteSignature.Xmlsignature(reg.codiceIpa, infoAmministrazione.codiceIpa, content, out statusCode);

                    contentSigned = ((sigilloElettronicoService.SignXml(new Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico.ValueObjects.SignXMLType()
                    {
                        FileDaFirmare = content,
                        CodiceAOOIPA = reg.codiceIpa,
                        CodiceEnteIPA = infoAmministrazione.codiceIpa
                    })).Result).FileFirmato;

                    if (contentSigned == null || contentSigned.Length == 0)
                    {
                        switch (statusCode)
                        {
                            case "506":
                                resultSigillo = DocsPaVO.documento.ResultSigilloElettronico.SERVICE_UNAVAILABLE;
                                break;
                            case "504":
                                resultSigillo = DocsPaVO.documento.ResultSigilloElettronico.DATI_DI_FIRMA_ERRATI;
                                break;
                            default:
                                resultSigillo = DocsPaVO.documento.ResultSigilloElettronico.SYSTEM_ERROR;
                                break;
                        }
                        logger.Error("La firma sul file di segnatura non è stata inserita");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Errore durante la firma del file segnatura.xml. Eccezione: " + ex.ToString());
                    resultSigillo = DocsPaVO.documento.ResultSigilloElettronico.SYSTEM_ERROR;
                    return content;
                }
            }
            else
                return content;

            return contentSigned;
        }
        catch (Exception e)
        {
            logger.Error("Errore nella creazione del file di segnatura. Eccezione: " + e.ToString());
            return null;
        }
    }

    public static DocsPaVO.Interoperabilita.Segnatura.SegnaturaInformaticaType CreaSegnaturaProtocolloXml(DocsPaVO.utente.Corrispondente mittSegnatura, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Registro regMittente, string docnumber, bool confermaRic, string pathFiles = "", Dictionary<string, string> CoppiaNomeFileENomeOriginale = null, string mailAddress = "")
    {
        DocsPaVO.Interoperabilita.Segnatura.SegnaturaInformaticaType segnatura = new DocsPaVO.Interoperabilita.Segnatura.SegnaturaInformaticaType();

        DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);

        if (schedaDoc == null || schedaDoc.protocollo == null)
        {
            if (schedaDoc != null && schedaDoc.protocollo == null)
                throw new Exception("CreaSegnaturaProtocolloXml: errore nell'estrazione del protocollo");
            throw new Exception("CreaSegnaturaProtocolloXml: errore nell'estrazione del documento");
        }

        int MAX_LENGTH = 7;
        string zeroes = "";
        string numProto = schedaDoc.protocollo.numero;
        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
        {
            zeroes = zeroes + "0";
        }
        numProto = zeroes + numProto;

        string nomefile = string.Empty;
        DocsPaVO.fascicolazione.Fascicolo classificazione = null;
        try
        {
            ArrayList fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, schedaDoc.docNumber);
            if (fascicoli != null && fascicoli.Count > 0)
            {
                classificazione = (from DocsPaVO.fascicolazione.Fascicolo f in fascicoli where f.tipo == "G" select f).FirstOrDefault();
                if (classificazione == null)
                {
                    classificazione = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloByIdNoSecurity((fascicoli[0] as DocsPaVO.fascicolazione.Fascicolo).idClassificazione);
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("CreaSegnaturaProtocolloXml -> Fascicoli: " + e.Message);
        }
        #region Intestazione
        try
        {
            segnatura.Intestazione = new DocsPaVO.Interoperabilita.Segnatura.IntestazioneType();

            //Identificatore
            segnatura.Intestazione.Identificatore = new DocsPaVO.Interoperabilita.Segnatura.IdentificatoreType()
            {
                CodiceAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.CodiceIPA()
                {
                    Value = reg.codAmministrazione
                },
                CodiceAOO = new DocsPaVO.Interoperabilita.Segnatura.CodiceIPA()
                {
                    Value = reg.codRegistro
                },
                CodiceRegistro = reg.codRegistro,
                NumeroRegistrazione = numProto,
                DataRegistrazione = ConvertToDate(schedaDoc.protocollo.dataProtocollazione),
                OraRegistrazione = ConvertToDate(schedaDoc.oraCreazione)
            };
        }
        catch (Exception e)
        {
            throw new Exception("CreaSegnaturaProtocolloXml -> Intestazione: " + e.Message);
        }
        //PrimaRegistrazione (OPZIONALE)

        //Oggetto
        segnatura.Intestazione.Oggetto = schedaDoc.oggetto.descrizione;

        //Classifica (DA RIVEDERE)
        if (classificazione != null)
        {
            segnatura.Intestazione.Classifica = new DocsPaVO.Interoperabilita.Segnatura.ClassificaType()
            {
                Denominazione = classificazione.descrizione,
                Item = classificazione.codice
            };
        }
        else
        {
            segnatura.Intestazione.Classifica = new DocsPaVO.Interoperabilita.Segnatura.ClassificaType()
            {
                Denominazione = string.Empty,
                Item = string.Empty
            };
        }
        //Fascicolo (OPZIONALE)

        //Riservato (OPZIONALE)

        #endregion

        #region Riferimenti (OPZIONALE)
        #endregion

        #region Descrizione
        segnatura.Descrizione = new DocsPaVO.Interoperabilita.Segnatura.DescrizioneType();

        //Mittente
        try
        {
            if (string.IsNullOrEmpty(mailAddress))
                mailAddress = regMittente.email;
            segnatura.Descrizione.Mittente = AddMittente(mittSegnatura, mailAddress);
        }
        catch (Exception e)
        {
            throw new Exception("CreaSegnaturaProtocolloXml -> Mittente: " + e.Message);
        }

        //Destinatario
        var destinatariProt = new List<DocsPaVO.Interoperabilita.Segnatura.DestinatarioType>();
        DocsPaVO.documento.ProtocolloUscita protocolloUscita = (schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita);
        try
        {
            if (protocolloUscita.destinatari != null)
            {
                foreach (DocsPaVO.utente.Corrispondente dest in protocolloUscita.destinatari)
                {
                    if (dest.canalePref != null && dest.canalePref.descrizione.Equals("INTEROPERABILITA"))
                        destinatariProt.Add(AddDestinatario(dest, confermaRic, false));
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("CreaSegnaturaProtocolloXml -> Destinatari: " + e.Message);
        }
        //Destinatari in conoscenza
        try
        {
            if (protocolloUscita.destinatariConoscenza != null)
            {
                foreach (DocsPaVO.utente.Corrispondente dest in protocolloUscita.destinatariConoscenza)
                {
                    if (dest.canalePref != null && dest.canalePref.descrizione.Equals("INTEROPERABILITA"))
                        destinatariProt.Add(AddDestinatario(dest, confermaRic, true));
                }
            }
            segnatura.Descrizione.Destinatario = destinatariProt.ToArray();
        }
        catch (Exception e)
        {
            throw new Exception("CreaSegnaturaProtocolloXml -> Destinatario in conoscenza: " + e.Message);
        }

        //Documento Primario
        try
        {
            if (CoppiaNomeFileENomeOriginale == null)
            {
                nomefile = GetNomeFile(schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, infoUtente);
            }
            else
            {
                string estensioneFile = getEstensione(getDocumentoPrincipale(schedaDoc).fileName);
                nomefile = "Documento_principale." + estensioneFile;
                if (CoppiaNomeFileENomeOriginale.ContainsKey(String.Format(pathFiles + @"\" + nomefile).ToLowerInvariant()))
                    nomefile = CoppiaNomeFileENomeOriginale[String.Format(pathFiles + @"\" + nomefile).ToLowerInvariant()];

                if (estensioneFile == "")
                    nomefile = "empty.TXT";
            }

            segnatura.Descrizione.DocumentoPrimario = new DocsPaVO.Interoperabilita.Segnatura.DocumentoType()
            {
                nomeFile = nomefile,
                mimeType = Interoperabilita.MimeMapper.GetMimeType(Path.GetExtension(nomefile)),
                Descrizione = schedaDoc.oggetto.descrizione,
                Impronta = new DocsPaVO.Interoperabilita.Segnatura.ImprontaType()
                {
                    algoritmo = "SHA-256",
                    Value = ComputeHashAsSHA256((schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest).impronta)
                },

            };
        }
        catch (Exception e)
        {
            throw new Exception("CreaSegnaturaProtocolloXml -> Documento primario: " + e.Message);
        }
        //Allegato
        try
        {
            if (CoppiaNomeFileENomeOriginale != null)
            {
                foreach (string tsrVal in CoppiaNomeFileENomeOriginale.Values)
                {
                    if (Path.GetExtension(tsrVal).ToLowerInvariant() == ".tsr")
                    {
                        DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato { fileName = tsrVal, descrizione = "Marca Temporale TSR", versionId = "1" };
                        schedaDoc.allegati.Add(all);
                    }
                }
            }

            if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
            {
                int countIS = 0;
                int countPEC = 0;
                int segnaturaXml = 1;
                for (int i = 0; i < schedaDoc.allegati.Count; i++)
                {
                    if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) == "1")
                        countIS++;
                    if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) == "1")
                        countPEC++;
                }

                if (schedaDoc.allegati.Count - countIS - countPEC - segnaturaXml > 0)
                {
                    var allegati = new List<DocsPaVO.Interoperabilita.Segnatura.DocumentoType>();
                    DocsPaVO.Interoperabilita.Segnatura.DocumentoType allegato;
                    for (int i = 0; i < schedaDoc.allegati.Count; i++)
                    {

                        if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) != "1" &&
                            BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) != "1" &&
                            ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).TypeAttachment != 6)
                        {
                            string nomefileAll = string.Empty;
                            if (CoppiaNomeFileENomeOriginale == null)
                            {

                                nomefileAll = GetNomeFileAllegato(schedaDoc.allegati[i] as DocsPaVO.documento.FileRequest, "Allegato_" + (i + 1).ToString(), infoUtente);
                            }
                            else
                            {
                                string fileExt = getEstensione(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);


                                nomefileAll = "Allegato_" + (i + 1).ToString() + "." + fileExt;
                                if (fileExt == "")
                                    nomefileAll = "Allegato_" + (i + 1).ToString() + ".TXT";


                                if (CoppiaNomeFileENomeOriginale.ContainsKey(String.Format(pathFiles + @"\" + nomefileAll).ToLowerInvariant()))
                                    nomefileAll = CoppiaNomeFileENomeOriginale[String.Format(pathFiles + @"\" + nomefileAll).ToLowerInvariant()];
                                else if (fileExt.ToLowerInvariant() == "tsr")  //nel caso fosse un TSR (allegato ombra) prendo il nome popolato sopra.
                                    nomefileAll = Path.GetFileName(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);

                            }

                            allegato = new DocsPaVO.Interoperabilita.Segnatura.DocumentoType()
                            {
                                nomeFile = nomefileAll,
                                mimeType = Interoperabilita.MimeMapper.GetMimeType(Path.GetExtension(nomefileAll)),
                                Impronta = new DocsPaVO.Interoperabilita.Segnatura.ImprontaType()
                                {
                                    algoritmo = "SHA-256",
                                    Value = ComputeHashAsSHA256(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).impronta)
                                }
                            };
                            if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione != null)
                                allegato.Descrizione = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione;

                            allegati.Add(allegato);
                        }

                    }
                    segnatura.Descrizione.Allegato = allegati.ToArray();
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("CreaSegnaturaProtocolloXml -> Allegato: " + e.Message);
        }

        #endregion

        #region Signature
        #endregion


        return segnatura;
    }

    private static DateTime ConvertToDate(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                return DateTime.Parse(value.Replace(".", ":"), new System.Globalization.CultureInfo("it-IT", true));
            }
            catch (Exception ex1)
            {
                try
                {
                    return DateTime.Parse(value.Replace(".", ":"), new System.Globalization.CultureInfo("en-US", true));
                }
                catch (Exception ex2)
                {
                    try
                    {
                        return DateTime.Parse(value);
                    }
                    catch (Exception ex3)
                    {
                        return new DateTime();
                    }
                }
            }
        }
        else
        {
            return new DateTime();
        }
    }

    private static DocsPaVO.Interoperabilita.Segnatura.SoggettoType AddMittente(DocsPaVO.utente.Corrispondente corr, string mailAddress)
    {
        DocsPaVO.Interoperabilita.Segnatura.SoggettoType mittente = new DocsPaVO.Interoperabilita.Segnatura.SoggettoType();
        DocsPaVO.Interoperabilita.Segnatura.AmministrazioneType amministrazione = new DocsPaVO.Interoperabilita.Segnatura.AmministrazioneType();
        DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
        DocsPaVO.amministrazione.InfoAmministrazione infoAmministrazione = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(corr.idAmministrazione);

        if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
        {
            uo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
        }

        while (uo != null)
        {
            if (uo.parent != null)
            {
                uo = uo.parent;
                //perchè la uo ha solo l'id, così estraggo gli altri dati.
                uo = (DocsPaVO.utente.UnitaOrganizzativa)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(uo.systemId);
            }
            else
                break;
        }

        if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
        {
            DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)corr;
            DocsPaVO.Interoperabilita.Segnatura.PersonaFisicaType persona = new DocsPaVO.Interoperabilita.Segnatura.PersonaFisicaType()
            {
                Nome = utente.nome,
                Cognome = utente.cognome,
            };

            DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType indirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType()
            {
                tipo = DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoTypeTipo.smtp,
                Value = mailAddress
            };
            persona.Contatti = new DocsPaVO.Interoperabilita.Segnatura.ContattiType()
            {
                IndirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType[1] { indirizzoTelematico },
                Telefono = new string[1] { utente.telefono }
            };

            mittente.Item = persona;
        }
        else
        {
            amministrazione.DenominazioneAmministrazione = infoAmministrazione.Descrizione;
            amministrazione.CodiceIPAAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.CodiceIPA()
            {
                Value = infoAmministrazione.codiceIpa
            };

            if (!string.IsNullOrEmpty(infoAmministrazione.indirizzoDigitaleRiferimento))
            {
                DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType indirizzoTelematicoRiferimento = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType()
                {
                    tipo = DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoTypeTipo.smtp,
                    Value = mailAddress //infoAmministrazione.indirizzoDigitaleRiferimento //rimuovo l'indirizzo di riferimento altrimenti spediscono tutte le ricevute lì
                };
                amministrazione.ContattiAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.ContattiType()
                {
                    IndirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType[1] { indirizzoTelematicoRiferimento },
                    Telefono = new string[] { uo.telefono1 }
                };
            }

            DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType indirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType()
            {
                tipo = DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoTypeTipo.smtp,
                Value = mailAddress
            };

            amministrazione.ContattiUO = new DocsPaVO.Interoperabilita.Segnatura.ContattiType()
            {
                IndirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType[1] { indirizzoTelematico },
                Telefono = new string[] { uo.telefono1 }
            };

            mittente.Item = amministrazione;
        }
        return mittente;
    }

    private static DocsPaVO.Interoperabilita.Segnatura.DestinatarioType AddDestinatario(DocsPaVO.utente.Corrispondente corr, bool confermaRicezione, bool perConoscenza)
    {

        DocsPaVO.Interoperabilita.Segnatura.DestinatarioType destinatario = new DocsPaVO.Interoperabilita.Segnatura.DestinatarioType();
        DocsPaVO.utente.UnitaOrganizzativa uo = null;
        DocsPaVO.utente.Ruolo ruolo = null;
        DocsPaVO.utente.Utente utente = null;
        if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
        {
            utente = (DocsPaVO.utente.Utente)corr;
            DocsPaVO.Interoperabilita.Segnatura.PersonaFisicaType persona = new DocsPaVO.Interoperabilita.Segnatura.PersonaFisicaType()
            {
                Nome = utente.nome,
                Cognome = utente.cognome,
            };

            DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType indirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType()
            {
                tipo = DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoTypeTipo.smtp,
                Value = utente.email
            };
            persona.Contatti = new DocsPaVO.Interoperabilita.Segnatura.ContattiType()
            {
                IndirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType[1] { indirizzoTelematico },
                Telefono = new string[1] { utente.telefono }
            };

            destinatario.Item = persona;
        }

        if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
        {
            ruolo = (DocsPaVO.utente.Ruolo)corr;
            DocsPaVO.Interoperabilita.Segnatura.AmministrazioneType amministrazione = new DocsPaVO.Interoperabilita.Segnatura.AmministrazioneType();
            amministrazione.DenominazioneAmministrazione = ruolo.descrizione;
            amministrazione.CodiceIPAAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.CodiceIPA()
            {
                Value = ruolo.codiceAmm
            };

            DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType indirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType()
            {
                tipo = DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoTypeTipo.smtp,
                Value = ruolo.email
            };

            amministrazione.ContattiAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.ContattiType()
            {
                IndirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType[1] { indirizzoTelematico },
                Telefono = new string[] { ruolo.telefono1 }
            };

            destinatario.Item = amministrazione;
        }

        if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
        {
            uo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
            DocsPaVO.Interoperabilita.Segnatura.AmministrazioneType amministrazione = new DocsPaVO.Interoperabilita.Segnatura.AmministrazioneType();
            amministrazione.DenominazioneAmministrazione = uo.descrizione;
            amministrazione.CodiceIPAAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.CodiceIPA()
            {
                Value = uo.codiceAmm
            };

            DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType indirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType()
            {
                tipo = DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoTypeTipo.smtp,
                Value = uo.email
            };

            amministrazione.ContattiAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.ContattiType()
            {
                IndirizzoTelematico = new DocsPaVO.Interoperabilita.Segnatura.IndirizzoTelematicoType[1] { indirizzoTelematico },
                Telefono = new string[] { uo.telefono1 }
            };

            destinatario.Item = amministrazione;

        }

        destinatario.confermaRicezione = confermaRicezione;
        destinatario.perConoscenza = perConoscenza;
        return destinatario;
    }

    private static string GetNomeFile(DocsPaVO.documento.FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente)
    {
        string nomeFile = string.Empty;
        try
        {
            if (fileReq.fileName != null && fileReq.fileName != "")
            {
                char[] dot = { '.' };
                string[] parts;
                string suffix = "";
                if (!fileReq.fileName.ToUpper().EndsWith("P7M"))
                {
                    parts = fileReq.fileName.Split(dot);
                    suffix = parts[parts.Length - 1];
                    nomeFile = "Documento_principale." + suffix;
                }
                else
                {
                    string appodocPrincipaleName = fileReq.fileName.Substring(fileReq.fileName.LastIndexOf("\\") + 1);
                    parts = appodocPrincipaleName.Split(dot);
                    int cont = 0;
                    for (int i = 2; i < parts.Length; i++)
                    {
                        if (parts[i].ToUpper().Equals("P7M"))
                        {
                            cont = cont + 1;
                            suffix = suffix + ".P7M";
                        }
                    }
                    suffix = parts[parts.Length - cont - 1] + suffix;
                    appodocPrincipaleName = appodocPrincipaleName.Substring(appodocPrincipaleName.ToUpper().LastIndexOf(suffix.ToUpper()));
                    nomeFile = "Documento_principale." + appodocPrincipaleName;
                }
                DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getInfoFile(fileReq, infoUtente);
                if (String.IsNullOrEmpty(fd.nomeOriginale))
                    nomeFile = fd.name;
                else
                    nomeFile = fd.nomeOriginale;
            }
            else
            {
                nomeFile = "empty.txt";
            }
        }
        catch (Exception e)
        {
            logger.Error("Estrazione del file non eseguita.Eccezione: " + e.ToString());
        }

        return nomeFile;
    }

    private static string getEstensione(string fileName)
    {
        char[] dot = { '.' };

        #region OLD
        //			string[] parts=fileName.Split(dot);
        //			string suffix=parts[parts.Length-1];
        //
        //			if(suffix.ToUpper().Equals("P7M"))
        //			{
        //			   suffix=fileName.Substring(fileName.IndexOf(".")+1);
        //			}
        #endregion

        //inizio modifica: il documento principale in alcuni casi non veniva allegato,
        //Ciò accedeva quando durante l'estrazione del documento principale, per allegarlo alla mail,
        //nel percorso del file è presente un ulteriore '.', oltre a quello relativo all'estensione.
        //Ad esempio con un path del genere andava in errore ENTEC\2005\EC_RU\UO1.1\Partenza\467.XLS

        string[] parts;
        string suffix = "";
        if (!fileName.ToUpper().EndsWith("P7M"))
        {
            parts = fileName.Split(dot);
            suffix = parts[parts.Length - 1];
        }
        else
        {
            string appodocPrincipaleName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            parts = appodocPrincipaleName.Split(dot);
            int cont = 0;
            for (int i = 2; i < parts.Length; i++)
            {
                if (parts[i].ToUpper().Equals("P7M"))
                {
                    cont = cont + 1;
                    suffix = suffix + ".P7M";
                }
            }
            suffix = parts[parts.Length - cont - 1] + suffix;
        }
        return suffix;
    }

    private static DocsPaVO.documento.Documento getDocumentoPrincipale(DocsPaVO.documento.SchedaDocumento schedaDoc)
    {
        #region Codice Commentato
        /* Elimino il controllo sul documento da inviare
			System.Collections.ArrayList docs=schedaDoc.documenti;
			DocsPaVO.documento.Documento docRes=null;
			for(int i=0;i<docs.Count;i++)
			{
				string invio=((DocsPaVO.documento.Documento) docs[i]).daInviare;
				if(invio!=null && invio.Equals("1"))
				{
					docRes=(DocsPaVO.documento.Documento) docs[i];
				}
			}
			if(docRes==null){
			  docRes=(DocsPaVO.documento.Documento) docs[0];
			}
			return docRes;
			*/
        #endregion

        return (DocsPaVO.documento.Documento)schedaDoc.documenti[0];
    }

    private static byte[] ComputeHashAsSHA256(string hash)
    {
        if (!string.IsNullOrEmpty(hash))
        {
            int NumberChars = hash.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hash.Substring(i, 2), 16);
            return bytes;
        }
        else
            return null;
    }

    private static string GetNomeFileAllegato(DocsPaVO.documento.FileRequest fileReq, string allegatoName, DocsPaVO.utente.InfoUtente infoUtente)
    {
        string nomeFile = string.Empty;
        try
        {
            string fileExt = getEstensione(fileReq.fileName);
            if (fileExt != "")
            {
                DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getInfoFile(fileReq, infoUtente);
                if (String.IsNullOrEmpty(fd.nomeOriginale))
                    nomeFile = fd.name;
                else
                    nomeFile = fd.nomeOriginale;
            }
            else
            {
                nomeFile = allegatoName + ".TXT";
            }


        }
        catch (Exception e)
        {
            logger.Error("Estrazione del file non eseguita.Eccezione: " + e.ToString());
        }

        return nomeFile;
    }

    public static System.Collections.Hashtable dividiDestinatari(System.Collections.ArrayList dest, string codRegistroMitt, string emailReg, SendDocumentResponse retValue, String idAmm)
    {
        System.Collections.Hashtable result = new System.Collections.Hashtable();

        for (int i = 0; i < dest.Count; i++)
        {
            try
            {
                if (dest[i].GetType() != typeof(DocsPaVO.utente.Corrispondente))
                {
                    bool isMailPrefCorr;
                    bool isMailPref;
                    string emailCorr = null;
                    string varCodiceAOO = string.Empty;
                    string varCodiceAMM = string.Empty;
                    //si estrae la UO del destinatario
                    DocsPaVO.utente.UnitaOrganizzativa uo = null;
                    DocsPaVO.utente.RaggruppamentoFunzionale rf = null;
                    string tipoIE = string.Empty;

                    // Booleano utilizzato per indicare se bisogna spedire per interoperabilità semplificata
                    bool isSimpInterop = false;
                    String varUrl = String.Empty;



                    if (dest[i].GetType() == typeof(DocsPaVO.utente.Utente))
                    {
                        System.Collections.ArrayList ruoliUtente = new System.Collections.ArrayList();

                        tipoIE = ((DocsPaVO.utente.Utente)dest[i]).tipoIE; // RICAVO IL TIPO I/E

                        if (tipoIE == "I")
                        {
                            logger.Debug("Destinatario " + i + ": utente INTERNO");

                            ruoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)dest[i]).idPeople);
                            if (ruoliUtente != null)
                            {
                                if (ruoliUtente.Count > 0)
                                {
                                    uo = ((DocsPaVO.utente.Ruolo)ruoliUtente[0]).uo;
                                    //INC000001373477 CCT problema con spedizione a destinatari interni(per utenti multiregistro)
                                    if ((!string.IsNullOrEmpty(uo.codiceAOO) && uo.codiceAOO != codRegistroMitt) ||
                                        (!string.IsNullOrEmpty(uo.email) && uo.email != emailReg))
                                    {
                                        ArrayList registriRuolo;
                                        DocsPaVO.utente.Registro reg;
                                        DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                                        foreach (DocsPaVO.utente.Ruolo ruolo in ruoliUtente)
                                        {
                                            registriRuolo = u.GetRegistriRuolo(ruolo.systemId);
                                            reg = (from r in registriRuolo.Cast<DocsPaVO.utente.Registro>() where r.codRegistro.Equals(codRegistroMitt) select r).FirstOrDefault();
                                            if (reg != null)
                                            {
                                                uo = (DocsPaVO.utente.UnitaOrganizzativa)u.GetCorrispondenteBySystemID(u.GetUoIdFromRoleId(ruolo.systemId));
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                uo = null;
                            }
                        }
                        else
                        {
                            logger.Debug("Destinatario " + i + ": utente ESTERNO");
                            uo = null;
                        }

                        if (uo != null) // caso di utenti interni
                        {
                            emailCorr = uo.email;
                            varCodiceAOO = uo.codiceAOO;
                            varCodiceAMM = uo.codiceAmm;
                            //luluciani 26/10/2005
                            ((DocsPaVO.utente.Utente)dest[i]).codiceAmm = varCodiceAMM;
                            ((DocsPaVO.utente.Utente)dest[i]).codiceAOO = varCodiceAOO;

                        }
                        else // caso di utenti esterni
                        {
                            emailCorr = ((DocsPaVO.utente.Utente)dest[i]).email;
                            varCodiceAMM = ((DocsPaVO.utente.Utente)dest[i]).codiceAmm;
                            varCodiceAOO = ((DocsPaVO.utente.Utente)dest[i]).codiceAOO;

                        }
                    }

                    if (dest[i].GetType() == typeof(DocsPaVO.utente.Ruolo))
                    {
                        tipoIE = ((DocsPaVO.utente.Ruolo)dest[i]).tipoIE; // RICAVO IL TIPO I/E

                        if (((DocsPaVO.utente.Ruolo)dest[i]).tipoIE != null && ((DocsPaVO.utente.Ruolo)dest[i]).tipoIE == "I")
                        {
                            logger.Debug("Destinatario " + i + ": ruolo INTERNO");
                            uo = ((DocsPaVO.utente.Ruolo)dest[i]).uo;
                            emailCorr = uo.email;
                            varCodiceAOO = uo.codiceAOO;
                            varCodiceAMM = uo.codiceAmm;
                        }
                        else
                        {
                            logger.Debug("Destinatario " + i + ": ruolo ESTERNO");
                            emailCorr = ((DocsPaVO.utente.Ruolo)dest[i]).email;
                            varCodiceAOO = ((DocsPaVO.utente.Ruolo)dest[i]).codiceAOO;
                            varCodiceAMM = ((DocsPaVO.utente.Ruolo)dest[i]).codiceAmm;
                        }
                    }

                    if (dest[i].GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                    {
                        tipoIE = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).tipoIE; // RICAVO IL TIPO I/E

                        logger.Debug("Destinatario " + i + ": unita' organizzativa");

                        uo = (DocsPaVO.utente.UnitaOrganizzativa)dest[i];
                        emailCorr = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).email;
                        varCodiceAOO = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).codiceAOO;
                        varCodiceAMM = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).codiceAmm;

                        // Verifica se utilizzare l'interoperabilità semplificata
                        DocsPaVO.utente.UnitaOrganizzativa c = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]);
                        isSimpInterop = c.canalePref != null && (c.canalePref.tipoCanale == InteroperabilitaSemplificataManager.InteroperabilityCode || c.canalePref.typeId == InteroperabilitaSemplificataManager.InteroperabilityCode) && c.Url != null && c.Url.Count > 0 && !String.IsNullOrEmpty(c.Url[0].Url) && Uri.IsWellFormedUriString(c.Url[0].Url, UriKind.Absolute);
                        varUrl = c.Url != null && c.Url.Count > 0 ? c.Url[0].Url : String.Empty;
                    }

                    if (dest[i].GetType() == typeof(DocsPaVO.utente.RaggruppamentoFunzionale))
                    {
                        tipoIE = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).tipoIE;

                        logger.Debug("Destinatario " + i + ": unita' organizzativa");

                        rf = (DocsPaVO.utente.RaggruppamentoFunzionale)dest[i];

                        emailCorr = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).email;
                        varCodiceAOO = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).codiceAOO;
                        varCodiceAMM = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).codiceAmm;

                        // Verifica se utilizzare l'interoperabilità semplificata per l'RF
                        DocsPaVO.utente.RaggruppamentoFunzionale c = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]);
                        isSimpInterop = c.canalePref != null &&
                            (c.canalePref.tipoCanale == InteroperabilitaSemplificataManager.InteroperabilityCode || c.canalePref.typeId == InteroperabilitaSemplificataManager.InteroperabilityCode)
                            && c.Url != null && c.Url.Count > 0 && !String.IsNullOrEmpty(c.Url[0].Url) && Uri.IsWellFormedUriString(c.Url[0].Url, UriKind.Absolute);
                        varUrl = c.Url != null && c.Url.Count > 0 ? c.Url[0].Url : String.Empty;
                    }

                    // Se il canale preferito è interoperabilità semplificata, questa si può utilizzare solo se attiva
                    if (isSimpInterop)
                        isSimpInterop &= InteroperabilitaSemplificataManager.IsEnabledSimplifiedInteroperability(idAmm);

                    //Federica
                    if (tipoIE.Equals("E"))
                    {
                        isMailPref = false;
                        if (dest[i].GetType() != typeof(DocsPaVO.utente.Utente)) //corrispondente di tipo uo/ruolo/rf
                            if (uo != null)
                                isMailPref = isMailPreferred(uo);
                            else
                                isMailPref = isMailPreferred(rf);
                        if (!isMailPref && dest[i].GetType() != typeof(DocsPaVO.utente.Utente))
                        {
                            //se canale preferenziale non interop, verifico il mezzo di spedizione
                            if (uo != null)
                                isMailPref = (uo.canalePref.descrizione.ToUpper().Equals("MAIL") || uo.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ? true : false;
                            else if (rf != null)
                                isMailPref = (rf.canalePref.descrizione.ToUpper().Equals("MAIL") || rf.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ? true : false;
                        }
                        isMailPrefCorr = isMailPreferred((DocsPaVO.utente.Corrispondente)dest[i]);
                        if (!isMailPrefCorr)
                        {
                            //se canale preferenziale non interop, verifico il mezzo di spedizione
                            isMailPrefCorr = (((DocsPaVO.utente.Corrispondente)dest[i]).canalePref.descrizione.ToUpper().Equals("MAIL") ||
                                                ((DocsPaVO.utente.Corrispondente)dest[i]).canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA") ||
                                                ((DocsPaVO.utente.Corrispondente)dest[i]).canalePref.descrizione.ToUpper().Equals("PORTALE")
                                                ) ? true : false;
                        }
                    }
                    else
                    {
                        // per i corrisp interni nel caso di interoperabilità si suppone che il canale
                        // preferenziale sia la mail
                        isMailPref = true;
                        isMailPrefCorr = false;
                    }

                    string email = null;

                    //CONTROLLO NUOVO
                    if (uo != null && uo.interoperante)
                    {
                        logger.Debug("UO interoperante");

                        //OLD:IN QUESTO CASO SI PRENDE L'INDIRIZZO MAIL DELL'UO SE QUESTA HA COME CANALE PREF. LA MAIL, ALTRIMENTI NULLA

                        //no si prende sempre l'indirizzo mail della AOO data dal varCodiceAOO
                        if (isMailPref)
                        {
                            //old:email=uo.email;

                            email = GetMailAOOInteropbyDest(uo);

                            logger.Debug("Email destinatario " + i + ": " + email);
                        }
                        else
                        {
                            logger.Debug("La mail non e' canale preferenziale della UO. Destinatario, quindi per noi non interoperante " + i + " scartato");
                            email = null;
                        }
                    }
                    else
                    {
                        logger.Debug("UO nulla o non interoperante");
                        //IMPORTANTE: UTENTI E RUOLI INTERNI POSSONO INTEROPERARE SOLO SE APPARTENGONO AD UNA UO INTEROPERANTE
                        //IN QUESTO CASO SI PRENDE L'INDIRIZZO MAIL DELL'UTENTE SE QUESTO HA COME CANALE PREFERENZIALE LA MAIL
                        //RUOLI; UTENTI interni all'amministrazione non devono essere inseriti nella DPA_T_CANALE_CORR
                        if (isMailPrefCorr)
                        {
                            email = emailCorr;
                            logger.Debug("Mail destinatario " + i + ": " + email);
                        }
                        else
                        {
                            logger.Debug("La mail non e' canale preferenziale del destinatario. Destinatario " + i + " scartato");
                            email = null;
                        }
                    }


                    //aggiunta condizione per spedire solamente a quei destinatari che appartengono ad AOO differenti da quella del mittente
                    //MAC_INPS 3749
                    if ((!string.IsNullOrEmpty(varCodiceAOO) && varCodiceAOO != codRegistroMitt && tipoIE == "I") || (
                        !string.IsNullOrEmpty(email) &&
                        email != emailReg) ||
                        isSimpInterop)
                    {
                        //if ((email != null) && (!email.Equals("")) && IsValidEmail(email) == true)
                        //{
                        if (email == null)
                            email = string.Empty;

                        // Se si è nel caso di spedizione per interoperabilità semplificata, la chiave è l'url
                        if (isSimpInterop)
                            email = varUrl;

                        if (result.ContainsKey(email))
                        {
                            ((System.Collections.ArrayList)result[email]).Add(dest[i]);
                        }
                        else
                        {

                            System.Collections.ArrayList al = new System.Collections.ArrayList();
                            al.Add(dest[i]);
                            result.Add(email, al);
                        }
                        //}
                        //else //da Scartare
                        //{
                        //    //modifica gennaro
                        //    bool interop_no_mail = false;
                        //    DocsPaVO.utente.Corrispondente c = (DocsPaVO.utente.Corrispondente)dest[i];
                        //    string Email = "";
                        //    if (!string.IsNullOrEmpty(c.email))
                        //        Email = c.email;

                        //    if (Interoperabilità.InteroperabilitaUtils.InteropIntNoMail && c.tipoIE == "I")
                        //    {
                        //        c.codiceAOO = varCodiceAOO;
                        //        interop_no_mail = true;
                        //    }


                        //    SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(Email, true, "KO");//ex ok
                        //    r.Destinatari = new ArrayList();
                        //    r.Destinatari.Add(c);

                        //    if (varCodiceAOO == codRegistroMitt)
                        //        r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";

                        //    if (!interop_no_mail || varCodiceAOO == codRegistroMitt)
                        //    {
                        //        r.SendErrorMessage = "Destinatario Non Interoperante";
                        //        r.SendSucceded = false;
                        //        r.MailNonInteroperante = true;
                        //    }
                        //    else
                        //    {
                        //        System.Collections.ArrayList al = new System.Collections.ArrayList();
                        //        al.Add(c);
                        //        //result.Add(c.codiceAmm, al);
                        //        //result.Add(c.codiceAOO, al);
                        //        result.Add(c.codiceAOO, al);
                        //    }
                        //    retValue.SendDocumentMailResponseList.Add(r);
                        //    //fine modifica gennaro
                        //}
                    }
                    else //da Scartare
                    {
                        //modifica gennaro
                        DocsPaVO.utente.Corrispondente c = (DocsPaVO.utente.Corrispondente)dest[i];
                        string Email = "";
                        bool interop_no_mail = false;
                        if (!string.IsNullOrEmpty(c.email))
                            Email = c.email;

                        if (Interoperabilita.InteroperabilitaUtils.InteropIntNoMail && c.tipoIE == "I")
                        {
                            c.codiceAOO = varCodiceAOO;
                            interop_no_mail = true;
                        }
                        SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(Email, true, "KO");
                        r.Destinatari = new ArrayList();
                        r.Destinatari.Add(c);

                        if (varCodiceAOO == codRegistroMitt)
                            r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";

                        if (!interop_no_mail || varCodiceAOO == codRegistroMitt)
                        {
                            if (email != emailReg)
                                r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";
                            else
                                if (emailReg != null && emailReg == "")
                                r.SendErrorMessage = "Attenzione, AOO mittente non ha alcuna mail associata.";
                            r.SendSucceded = false;
                            r.MailNonInteroperante = true;
                        }
                        else
                        {
                            System.Collections.ArrayList al = new System.Collections.ArrayList();
                            al.Add(c);
                            //MAC 3749
                            result.Add(c.codiceAOO + i.ToString(), al);
                            //result.Add(c.codiceCorrispondente, al);
                            //result.Add(c.codiceAmm, al);
                        }
                        retValue.SendDocumentMailResponseList.Add(r);
                        //fine modifica gennaro
                    }
                }
                else //OCCASIONALI sono da Scartare
                {
                    DocsPaVO.utente.Corrispondente c = (DocsPaVO.utente.Corrispondente)dest[i];
                    string Email = "";

                    if (c.email != null)
                        Email = c.email;

                    if (string.IsNullOrEmpty(Email))
                    {
                        SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(Email, true, "OK");
                        r.Destinatari = new ArrayList();
                        r.Destinatari.Add(c);
                        r.SendErrorMessage = "Destinatario Occasionale non interoperante";
                        r.SendSucceded = false;
                        r.MailNonInteroperante = true;
                        retValue.SendDocumentMailResponseList.Add(r);
                    }
                    else
                    {
                        System.Collections.ArrayList al = new System.Collections.ArrayList();
                        al.Add(dest[i]);
                        result.Add(Email, al);

                    }

                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        return result;
    }

    private static bool isMailPreferred(DocsPaVO.utente.Corrispondente corr)
    {
        System.Data.DataSet ds;

        try
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            obj.getDatiCan(out ds, corr);

            if (ds.Tables["CANALE"].Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static string GetMailAOOInteropbyDest(DocsPaVO.utente.Corrispondente corrispondente)
    {
        string rtn = string.Empty;
        if (corrispondente != null)
        {
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
            {
                if (corrispondente.tipoIE.Equals("I"))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
                    string sql = "SELECT VAR_EMAIL_REGISTRO FROM DPA_EL_REGISTRI WHERE UPPER(VAR_CODICE)='" + corrispondente.codiceAOO.ToUpper() + "' AND ID_AMM=" + corrispondente.idAmministrazione;
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql))
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    rtn = reader.GetString(0);
                            }
                        }
                    }
                }
                else if (corrispondente.tipoIE.Equals("E"))
                {
                    rtn = corrispondente.email;

                }

            }
        }
        return rtn;
    }

    /// <summary>
    /// Metodo per la spedizione di un documento per interoperabilità semplificata
    /// </summary>
    /// <param name="schedaDocumento">Informazioni sul documento da spedire</param>
    /// <param name="infoUtente">Informazioni sull'utente che sta eefettuando la spedizione</param>
    /// <param name="receivers">Informazioni sui destinatari cui è indirizzata la spedizione</param>
    /// <returns>Esito della spedizione</returns>
    public static SendDocumentResponse SendDocumentIS(DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                                    DocsPaVO.utente.InfoUtente infoUtente,
                                                    IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> receivers)
    {

        SendDocumentResponse retValue = new SendDocumentResponse();
        retValue.SendDateTime = DateTime.Now;
        retValue.SchedaDocumento = schedaDocumento;

        // Suddivisione dei destinatari per amministrazione
        List<List<DocsPaVO.Spedizione.DestinatarioEsterno>> splittedReceivers = receivers.GroupBy(r => r.DatiDestinatari[0].codiceAmm).Select(group => group.ToList()).ToList();
        SendDocumentResponse.SendDocumentMailResponse mailResponse = null;
        foreach (List<DocsPaVO.Spedizione.DestinatarioEsterno> recs in splittedReceivers)
        {
            mailResponse = SendDocumentSimpleInterop(schedaDocumento,
                                                     recs,
                                                     infoUtente,
                                                     recs[0].DatiDestinatari[0].Url[0].Url);
            //retValue.SendDocumentMailResponseList.Add(SendDocumentSimpleInterop(
            //    schedaDocumento,
            //    recs,
            //    infoUtente,
            //    recs[0].DatiDestinatari[0].Url[0].Url));
            retValue.SendDocumentMailResponseList.Add(mailResponse);

            // Inserimento nella stato invio ad ogni spedizione effettuata
            List<Corrispondente> corrs = new List<Corrispondente>();
            foreach (DocsPaVO.Spedizione.DestinatarioEsterno c in recs)
                corrs.AddRange(c.DatiDestinatari);

            InsertStatoInvioDestinatari(schedaDocumento, corrs, mailResponse);
        }

        return retValue;
    }

    /// <summary>
    /// Metodo per la spedizione di un documento per interoperabilità semplificata
    /// </summary>
    /// <param name="schedaDocumento">Documento da spedire</param>
    /// <param name="receivers">Destinatari della spedizione</param>
    /// <param name="infoUtente">Informazioni sull'utente che sta effettuando la spedizione</param>
    /// <param name="receiverUrl">Url dei destinatari della spedizione</param>
    /// <returns>Esito della spedizione</returns>
    private static SendDocumentResponse.SendDocumentMailResponse SendDocumentSimpleInterop(DocsPaVO.documento.SchedaDocumento schedaDocumento, List<DocsPaVO.Spedizione.DestinatarioEsterno> receivers, DocsPaVO.utente.InfoUtente infoUtente, String receiverUrl)
    {
        bool sendSucceded = true;
        String errorMessage = String.Empty;

        // Costruzione della lista dei destinatari
        List<Corrispondente> corrs = new List<Corrispondente>();
        foreach (DocsPaVO.Spedizione.DestinatarioEsterno c in receivers)
            corrs.AddRange(c.DatiDestinatari);

        // Risultato della spedizione
        SendDocumentResponse.SendDocumentMailResponse retValue = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(receiverUrl);

#if false   // usa libreria interoperability
        InteroperabilityMessage interoperabilityMessage = null;
        try
        {
            // Costruzione dell'oggetto con le informazioni sulla spedizione
            interoperabilityMessage = new InteroperabilitaSemplificataHelper().CreateInteroperabilityMessage(schedaDocumento, infoUtente, corrs.ToArray());

            // Invio del messaggio al sistema di interoperabilità
            InteroperabilityController interoperabilityController = new InteroperabilityController();
            IAsyncResult result = interoperabilityController.SubmitInteroperabilityMessage(interoperabilityMessage, receiverUrl, OnAnalyzeInteroperabilityMessageCompleted);
            sendSucceded = true;
        }
        catch (SenderNotInteroperableException e)
        {
            sendSucceded = false;
            errorMessage = "Per poter procedere con la spedizione è necessario che il mittente sia configurato correttamente";
            logger.Error(errorMessage);
        }
        catch (Exception e)
        {
            sendSucceded = false;
            errorMessage = "Si è verificato un errore non identificato durante la spedizione";
            logger.Error(errorMessage);
        }

#endif

        // Impostazione dell'esito della spedizione e dell'eventuale errore
        retValue.SendSucceded = sendSucceded;
        retValue.SendErrorMessage = errorMessage;

        // Per poter fare in modo che il sistema capisca che il documento è già stato spedito, viene valorizzato
        // il campo Email dei corrispondenti con l'URL del destinatario
        foreach (Corrispondente destinatario in corrs)
        {
            destinatario.email = receiverUrl;
            retValue.Destinatari.Add(destinatario);
        }
        // Restituzione del risultato
        return retValue;
    }

    /// <summary>
    /// Aggiornamento stato spedizione nella dpa_stato_invio
    /// </summary>
    /// <param name="schedaDocumento"></param>
    /// <param name="listDestinatari"></param>
    /// <param name="mailResponse"></param>
    public static void InsertStatoInvioDestinatari(DocsPaVO.documento.SchedaDocumento schedaDocumento, List<Corrispondente> listDestinatari, DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse mailResponse)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        foreach (DocsPaVO.utente.Corrispondente corr in listDestinatari)
        {
            if (schedaDocumento.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
            {
                string idDocArrivoPar = doc.GetIdDocArrivoPar(schedaDocumento.systemId, corr, "'D','C','F'");

                string mailCorrispondente = corr.email;
                if (mailCorrispondente == null)
                    mailCorrispondente = string.Empty;

                doc.InsertStatoInvio(corr, idDocArrivoPar, schedaDocumento.registro.systemId, schedaDocumento.systemId, (mailResponse == null || (mailResponse != null && !mailResponse.SendSucceded && !string.IsNullOrEmpty(mailResponse.SendErrorMessage))), mailResponse.MailAddress);
            }
        }
    }

    /// <summary>
    /// Invio del documento ad un indirizzo mail cui possono
    /// fare riferimento 1 o più destinatari
    /// </summary>
    /// <param name="schedaDocumento"></param>
    /// <param name="mailAddress"></param>
    /// <param name="listDestinatari"></param>
    /// <param name="infoUtente"></param>
    /// <param name="confermaRicezione"></param>
    /// <returns></returns>
    public static SendDocumentResponse SendDocument(DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                                    DocsPaVO.utente.Registro registroMittente,
                                                    string mailAddress,
                                                    ArrayList listDestinatari,
                                                    DocsPaVO.utente.InfoUtente infoUtente,
                                                    bool confermaRicezione)
    {

        SendDocumentResponse retValue = new SendDocumentResponse();
        retValue.SendDateTime = DateTime.Now;
        retValue.SchedaDocumento = schedaDocumento;

        Hashtable tableDestinatari = dividiDestinatari(listDestinatari, schedaDocumento.registro.codRegistro, schedaDocumento.registro.email, retValue, infoUtente.idAmministrazione);

        string mail = string.Empty;

        foreach (DictionaryEntry item in tableDestinatari)
        {
            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)((ArrayList)item.Value)[0];
            string err = "";
            //per ottenere il codAOO e codAMM in caso di ruolo/utente
            if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
            {

                corr = ((DocsPaVO.utente.Ruolo)corr).uo;
            }
            if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
            {
                corr = (DocsPaVO.utente.Utente)corr;

            }

            if (InteroperabilitaUtils.InteropIntNoMail  //ATTIVATA/NON ATTIVATA INTEROP INTERNI NO MAIL
                && corr != null
                && corr.tipoIE != null
                && corr.tipoIE == "I" //SOLO PER CORR: INTERNI ALL'AMM.
                && !corr.codiceAOO.ToUpper().Equals(schedaDocumento.registro.codRegistro.ToUpper()) //NO SE RISIEDE NELLA MIA AOO
                && corr.idAmministrazione.Trim().ToLower().Equals(schedaDocumento.registro.idAmministrazione.Trim().ToLower()))
            {
                DocsPaVO.Interoperabilita.DatiInteropAutomatica dia = null;
                //bool result = SendDocumentInteropNoMail(corr, schedaDocumento, infoUtente, out err, out dia);
                // S.Furnari - 17/01/2013 - Se è attiva la funzionalità di spedizione selettiva, l'utente non è ammesso
                // come destinatario della spedizione.
                //bool result = SendDocumentInteropNoMail(corr, schedaDocumento, infoUtente, out err, out dia);
                bool result = false;
                if (InteroperabilitaSegnatura.IsEnabledSelectiveTransmission(corr.idAmministrazione) && corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    result = false;
                    err = "Impossibile spedire il documento ad un utente.";
                }
                else
                    result = SendDocumentInteropNoMail(corr, schedaDocumento, infoUtente, out err, out dia);



                SendDocumentResponse.SendDocumentMailResponse mailResponse = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(mailAddress, result, err);
                ArrayList corrs = (ArrayList)item.Value;
                for (int i = 0; i < corrs.Count; i++)
                {
                    mailResponse.Destinatari.Add((DocsPaVO.utente.Corrispondente)corrs[i]);
                }

                //se i dati aggiuntivi dell'interoperabilità sono presenti..
                if (dia != null)
                    mailResponse.datiInteropAutomatica = dia;

                retValue.SendDocumentMailResponseList.Add(mailResponse);

            }
            //else if (InteroperabilitaUtils.InteropSemp
            //&& corr != null
            //&& corr.tipoIE == "E"
            //&& !corr.codiceAOO.ToUpper().Equals(schedaDocumento.registro.codRegistro.ToUpper()) //NO SE RISIEDE NELLA MIA AOO
            //&& !corr.idAmministrazione.Trim().ToLower().Equals(schedaDocumento.registro.idAmministrazione.Trim().ToLower()))
            //{
            //    logger.Debug("******** Inizio Interoperabilità semplificata ********");
            //    DocsPaVO.utente.Corrispondente mittente = null;
            //    if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente != null)
            //    {
            //        mittente = ((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente;

            //    }
            //    else
            //        mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

            //    string pathAttatchments = ExtractDocumentFilesToPath(schedaDocumento, infoUtente);

            //    DocsPaInteropSemplificata.WRInteropSemp.Segnatura segnatura = CreaSegnaturaObject(mittente, schedaDocumento.registro, registroMittente, schedaDocumento, mailAddress, listDestinatari, pathAttatchments, confermaRicezione); ;

            //    string[] filesToSend = System.IO.Directory.GetFiles(pathAttatchments);
            //    DocsPaInteropSemplificata.WRInteropSemp.FileAllegato documentoPrincipale = new DocsPaInteropSemplificata.WRInteropSemp.FileAllegato();

            //    int numAllegati = filesToSend.Count() - 1;
            //    DocsPaInteropSemplificata.WRInteropSemp.FileAllegato[] documentiAllegati = null;

            //    if (numAllegati > 0)
            //    {
            //        documentiAllegati = new DocsPaInteropSemplificata.WRInteropSemp.FileAllegato[numAllegati];
            //    }

            //    for (int i = 0; i < filesToSend.Count(); i++)
            //    {
            //        if (filesToSend[i].Contains("Documento_principale"))
            //        {
            //            documentoPrincipale.NomeFile = filesToSend[i];
            //            FileStream fs = File.OpenRead(filesToSend[i]);
            //            byte[] res = new byte[fs.Length];
            //            fs.Read(res, 0, (int)fs.Length);
            //            fs.Close();
            //            documentoPrincipale.Contenuto = res;
            //        }
            //        else
            //        {
            //            //Da gestire il caso in cui non c'è neanche il documento principale???
            //            //(ossia quando viene creato il file empty.TXT)
            //            //Altrimenti qui viene generata una NullPointerException
            //            if (!filesToSend[i].Contains("segnatura.xml") && !filesToSend[i].Contains("empty.TXT"))
            //            {
            //                int temp = i - 1;
            //                documentiAllegati[temp] = new DocsPaInteropSemplificata.WRInteropSemp.FileAllegato();
            //                documentiAllegati[temp].NomeFile = filesToSend[i];
            //                FileStream fs = File.OpenRead(filesToSend[i]);
            //                byte[] res = new byte[fs.Length];
            //                fs.Read(res, 0, (int)fs.Length);
            //                fs.Close();
            //                documentiAllegati[temp].Contenuto = res;
            //            }
            //        }
            //    }
            //    DocsPaInteropSemplificata.WRInteropSemp.PacchettoSpedizione pacchettoSpedizione = new DocsPaInteropSemplificata.WRInteropSemp.PacchettoSpedizione();
            //    pacchettoSpedizione.Segnatura = segnatura;
            //    pacchettoSpedizione.DocumentoPrincipale = documentoPrincipale;
            //    pacchettoSpedizione.DocumentiAllegati = documentiAllegati;
            //    string urlWSCorrispondente = "http://localhost/DocsPa30/DocsPaWS/DocsPaWSInteropSemp.asmx"; //Url del webservice da chiamare in base alla AOO del corrispondente
            //    DocsPaInteropSemplificata.InteropSempProxy.SpedisciClient(urlWSCorrispondente, pacchettoSpedizione);
            //    logger.Debug("******** Fine Interoperabilità semplificata ********");
            //}
            else
            {

                SendDocumentResponse.SendDocumentMailResponse retmail = SendDocumentMail(schedaDocumento, registroMittente, mailAddress, listDestinatari, infoUtente, confermaRicezione);
                retValue.SendDocumentMailResponseList.Add(retmail);
                // PEC 4 - requisito 5 - storico spedizioni
                //string esito = "";
                //if (retmail.SendSucceded)
                //{
                //    esito = "Spedito";
                //}
                //else
                //{
                //    esito = retmail.SendErrorMessage;
                //}
                //DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                //interop.InsertInStoricoSpedizioni(schedaDocumento.systemId, corr.systemId, esito, mailAddress);


                //retValue.SendDocumentMailResponseList.Add(SendDocumentMail(schedaDocumento, registroMittente, mailAddress, listDestinatari, infoUtente, confermaRicezione));
            }
        }

        return retValue;
    }

    /// <summary>
    /// interop no mail: In partica spedisce un documento al corr, crea il pedisposto che ne
    /// risulterebbe da una eventuale operazione di controllo casella istituzionale e spedisce le trasmissioni
    /// ai ruolo con PRAU.
    /// </summary>
    /// <param name="corr"></param>
    ///<param name="schedaDocumento"></param>
    /// <param name="err">messaggio di errore o di successo. se errore la prima parola del messaggio è "errore"</param>
    /// <returns>true se OK; false se KO</returns>
    public static bool SendDocumentInteropNoMail(DocsPaVO.utente.Corrispondente corr, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, out string err, out DocsPaVO.Interoperabilita.DatiInteropAutomatica dia)
    {
        err = "";
        bool result = false; //presume insuccesso !
        bool utenteLoggato = false;
        DocsPaVO.utente.InfoUtente infoutenteInterOp = null;
        DocsPaVO.utente.Utente ut = null;
        dia = null;

        //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        // {
        try
        {
            semInterOper.WaitOne();

            //TODO multi AMM, possono avere stessi codiceAOO ?
            DocsPaVO.utente.Registro reg = getRegistroInteropByCodAOO(corr.codiceAOO, corr.idAmministrazione);
            if (reg == null)
            {
                err = "Errore, Nessun Registro Associato all'AOO " + corr.descrizione;
            }
            // crea ruolo per interop
            DocsPaVO.utente.Ruolo ruolo = null;
            if (reg.idRuoloAOO != null && reg.idRuoloAOO != "")
            {
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(reg.idRuoloAOO);

            }
            else
            {
                err = "Errore, Il Ruolo responsabile dell'AOO (ID_RUOLO_AOO) non è valorizzato";
            }
            if (ruolo == null)
            {
                err = "Errore, Il Ruolo responsabile dell'AOO destinataria (ID_RUOLO_AOO) non è valorizzato";
            }
            else //else ruolo
            {
                //caso raro, sto facendo l'inertop con lo stesso utente che è dest e mitt  contemporaneamente
                if (reg.idUtenteAOO != null && reg.idUtenteAOO != ""
                    && infoUtente.idPeople.Equals(reg.idUtenteAOO)
                    && infoUtente.dst != null
                    && infoUtente.dst != "")
                {

                    infoutenteInterOp = infoUtente; utenteLoggato = true;
                    logger.Debug("inzio Procedura interop no mail interni per AOO " + corr.codiceAOO);
                    //*****************************************************************************************************//
                    // MODIFICA GIORDANO IACOZZILLI 10052012
                    //

                    // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento interop interna solo a UO destinataria della spedizione e non a tutta la AOO
                    // result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                    result = BusinessLogic.Interoperabilita.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty, corr);

                    //result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);


                    //
                    //OLD CODE:
                    //  result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                    //*****************************************************************************************************//
                }
                else
                {	//calcolo utente
                    ut = new DocsPaVO.utente.Utente();
                    //questo metodo non torna mai null, ma un oggetto con tutti gli attributi vuoti.

                    // S. Furnari - 16/01/2013 - Se idUtenteAOO non è valorizzato, magari per errori di configurazione
                    // deve essere segnalato un opportuno errore.
                    //ut = BusinessLogic.Utenti.UserManager.getUtente(reg.idUtenteAOO);
                    ////quindi controllo almeno se idPeople!=null penso basti.
                    //if (ut != null && ut.idPeople == null)
                    //{
                    //    err = "Attenzione, L'utente responsabile dell'AOO destinataria (ID_PEOPLE_AOO) non è valorizzato oppure è stato disabilitato.";
                    //}
                    if (!String.IsNullOrEmpty(reg.idUtenteAOO))
                        ut = BusinessLogic.Utenti.UserManager.getUtente(reg.idUtenteAOO);

                    if (ut != null && String.IsNullOrEmpty(ut.idPeople))
                    {
                        err = "Attenzione, L'utente responsabile dell'AOO destinataria (ID_PEOPLE_AOO) non è valorizzato oppure è stato disabilitato.";
                        // S. Furnari - 8/1/2013 - Se è arrivato qui, non deve andare avanti
                        return false;
                    }










                }
                if (ut != null)
                {
                    //calcolo infoutenteInertOp
                    //ricava l'infoutente del ruolo_AOO/utente_AOO operando una login per essere suciri che tale
                    //utente risulti il creatore prorpietario del documento predisposto
                    infoutenteInterOp = getInfoUtenteInterOp(ruolo, ut, out utenteLoggato);
                    if (infoutenteInterOp == null)
                    {
                        err = "Attenzione, non è stato possibile effettuare login con l'utente" + ut.userId + " per interoperare con il registro " + reg.codRegistro;
                    }
                    else
                    {
                        logger.Debug("inzio Procedura interop no mail interni per AOO " + corr.codiceAOO);
                        infoutenteInterOp.urlWA = infoUtente.urlWA;
                        //*****************************************************************************************************//
                        // MODIFICA GIORDANO IACOZZILLI 10052012
                        //
                        //modifica furnari
                        result = BusinessLogic.Interoperabilita.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty, corr);
                        //result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                        //
                        //OLD CODE:
                        //  result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                        //*****************************************************************************************************//

                        //effettuo la trasmissione dei documenti creati su registro automatico
                        // Trasmissioni.
                    }
                } //FINE else infoutente
            }//FINE else ruolo
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
            if (err == "")
                err = "Attenzione, la spedizione al destinatario non riuscita.";
            result = false;
        }
        finally
        {
            if (infoutenteInterOp != null
                && infoutenteInterOp.dst != null && !utenteLoggato) //se sono loggatto faccio logoff.
                BusinessLogic.Utenti.Login.logoff(ut.userId, ut.idAmministrazione, ut.dst);

            semInterOper.ReleaseMutex();
        }

        //    if (result)
        //    {
        //        transactionContext.Complete();
        //    }
        //}

        return result;
    }

    /// <summary>
    /// Invio di un documento ad un indirizzo mail cui possono
    /// fare riferimento 1 o più destinatari
    /// </summary>
    /// <param name="schedaDocumento"></param>
    /// <param name="registroMittente">
    /// Registro (o RF) mittente del documento
    /// </param>
    /// <param name="mailAddress"></param>
    /// <param name="listDestinatari"></param>
    /// <param name="infoUtente"></param>
    /// <param name="confermaRicezione"></param>
    /// <returns></returns>
    private static SendDocumentResponse.SendDocumentMailResponse SendDocumentMail(
                        DocsPaVO.documento.SchedaDocumento schedaDocumento,
                        DocsPaVO.utente.Registro registroMittente,
                        string mailAddress,
                        ArrayList listDestinatari,
                        DocsPaVO.utente.InfoUtente infoUtente,
                        bool confermaRicezione)
    {
        if (registroMittente == null)
            // Se non è specificato un registro (o RF) mittente,
            // viene impostato il registro di protocollo
            registroMittente = schedaDocumento.registro;

        SendDocumentResponse.SendDocumentMailResponse retValue = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(mailAddress);


        DocsPaVO.utente.Corrispondente mittente = null;

        //25/11/2009 se c'è il mittente nel protocollo, allora inserisco quest'ultimo nella segnatura.xml.
        if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente != null)
        {
            mittente = ((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente;

        }
        else
            mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

        try
        {
            //Gestione XML con allegati da creare on the fly suap
            if (schedaDocumento.template != null)
            {
                if (schedaDocumento.template.DESCRIZIONE.ToUpper() == "ENTESUAP")
                {
                    BusinessLogic.Interoperabilita.InteroperabilitaSendXmlInAttach xmlAtt = new Interoperabilita.InteroperabilitaSendXmlInAttach();
                    bool suapRetval = xmlAtt.ManageAttachXML_Suap(ref schedaDocumento, infoUtente, registroMittente.email);
                    if (!suapRetval)
                    {
                        string idSuap = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_IDENTIFICATIVO_SUAP");
                        //se manca l'idsuap nelle chiavi di configurazione allora non posso continuare ed esco
                        if (String.IsNullOrEmpty(idSuap))
                        {
                            retValue.SendErrorMessage = "ERRORE: l'identificativo suap non è configurato";
                        }
                        else
                        {
                            retValue.SendErrorMessage = "ERRORE: campi mancanti per creare l'allegato entesuap.xml. Controllare: codice pratica, registro/RF mittente e identificativo SUAP (contattare l’amministratore di sistema)";
                        }
                        retValue.SendSucceded = false;
                        return retValue;
                    }

                }
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore creando o processando l'allegato suap {0} {1}", e.Message, e.StackTrace);
        }

        // estrazione degli allegati
        Dictionary<string, string> CoppiaNomeFileENomeOriginale;
        string pathAttatchments = ExtractDocumentFilesToPath(schedaDocumento, infoUtente, out CoppiaNomeFileENomeOriginale);


        //ci sono stati dei problemi relativi all'inserimento dell'allegato
        if (string.IsNullOrEmpty(pathAttatchments))
            return null;

        DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
        DocsPaVO.amministrazione.CasellaRegistro[] caselle = BusinessLogic.Amministrazione.RegistroManager.GetMailRegistro(registroMittente.systemId);
        if (!string.IsNullOrEmpty(registroMittente.email))
        {
            foreach (DocsPaVO.amministrazione.CasellaRegistro c in caselle)
            {
                if (c.EmailRegistro.Equals(registroMittente.email))
                {
                    retValue = SendDocumentMail(schedaDocumento,
                                        mittente,
                                        registroMittente,
                                        mailAddress,
                                        listDestinatari,
                                        c,
                                        pathAttatchments,
                                        CoppiaNomeFileENomeOriginale,
                                        confermaRicezione);

                    c.RicevutaPEC = (!string.IsNullOrEmpty(c.RicevutaPEC)) ? c.RicevutaPEC : string.Empty;
                    if (!string.IsNullOrEmpty(c.RicevutaPEC) && (c.RicevutaPEC.Length > 1))
                    {
                        c.RicevutaPEC = c.RicevutaPEC.Substring(0, 1);
                        //Qui va eliminato il campo secondario dal DB nel caso esso esista.
                        obj.setRicevutaPec(registroMittente.systemId, new DocsPaVO.amministrazione.CasellaRegistro[] { c });
                    }
                    break;
                }
            }
        }
        else
        {
            retValue.Destinatari = listDestinatari;
            retValue.MailAddress = mailAddress;
            retValue.MailNonInteroperante = false;
            retValue.SendErrorMessage = " Spedizione non effettuata: è necessario selezionare un registro/rf prima di effettuare la spedizione";
            retValue.SendSucceded = false;
        }
        //cancellazione della directory
        try
        {
            DocsPaUtils.Functions.Functions.CancellaDirectory(pathAttatchments);
        }
        catch { }

        return retValue;
    }

    private static DocsPaVO.utente.Registro getRegistroInteropByCodAOO(string codAoo, string idAmministrazione)
    {
        DocsPaVO.utente.Registro reg = null;

        reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(codAoo, idAmministrazione);
        return reg;
    }

    /// <summary>
    /// metodo che ritorna un Infoutente per l'interoperabilità Interni senza mail.
    /// utilizza il ruolo e un utente ricavati dalla DPA_EL_REGISTRO:ID_RUOLO_AOO,ID_PEOPLE_AOO
    /// di solito l'utente che utilizza docspa non è lo stesso utente della ID_PEOPLE_AOO,
    /// quindi si fa una login con questo utente e si calcola dall'oggetto utente così ottenuto
    /// l'infoutente di ritorno.
    /// </summary>
    /// <param name="ruolo"></param>
    /// <param name="ut"></param>
    /// <returns></returns>
    public static DocsPaVO.utente.InfoUtente getInfoUtenteInterOp(DocsPaVO.utente.Ruolo ruolo,
        DocsPaVO.utente.Utente ut, out bool loggato)
    {
        loggato = false;
        DocsPaVO.utente.InfoUtente infoInterOp = null;

        #region calcola infoUtenteInterop

        DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();
        if (pwdConfig.IsSupportedPasswordConfig())
        {
            // Se è attivata la gestione delle configurazioni delle password,
            // non è possibile reperire e fornire la password per l'utente (è criptata).
            // Non viene effettuata la login.

        }
        else
        {
            string library = DocsPaDB.Utils.Personalization.getInstance(ut.idAmministrazione).getLibrary();
            BusinessLogic.Utenti.UserManager u = new BusinessLogic.Utenti.UserManager();
            string password = u.getPassword(ut.idPeople);
            DocsPaVO.utente.UserLogin login = new DocsPaVO.utente.UserLogin(ut.userId, password, ut.idAmministrazione);
            DocsPaVO.utente.UserLogin.LoginResult lr = new DocsPaVO.utente.UserLogin.LoginResult();
            string ipAddress = "";
            //TODO: se utente già loggato, allora usare dst...altrimenti login
            string DST = u.getDST(ut.userId);

            if (!(DST != null && DST != ""))
            {
                ut = BusinessLogic.Utenti.Login.loginMethod(login, out lr, true, "127.0.0.1", out ipAddress);
            }
            else
            {
                ut.dst = DST; loggato = true;
            }
        }

        if (ut != null)
        {
            infoInterOp = new DocsPaVO.utente.InfoUtente(ut, ruolo);
            //aggiungo DST necessario per DOCUMENTUM, ma utile anche in ETDOCS per scopi futuri.
            DocsPaDocumentale.Documentale.UserManager um = new DocsPaDocumentale.Documentale.UserManager();
            infoInterOp.dst = um.GetSuperUserAuthenticationToken();
        }

        #endregion

        return infoInterOp;

    }

    private static string ExtractDocumentFilesToPath(DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                                     DocsPaVO.utente.InfoUtente infoUtente,
                                                    out Dictionary<string, string> CoppiaNomeFileENomeOriginale)
    {
        string pathFiles = string.Empty;

        //creazione del logger
        string basePathLogger = DocsPaVO.Settings.AppSettings.Instance.LOG_PATH;
        basePathLogger = basePathLogger.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
        basePathLogger = basePathLogger + "\\Interoperabilita";
        DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(basePathLogger + "\\" + schedaDocumento.registro.codRegistro);
        string pathLogger = basePathLogger + "\\" + schedaDocumento.registro.codRegistro + "\\invio";

        logger.Debug("Destinatari:" + ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Count);
        logger.Debug("Destinatari conoscenza:" + ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Count);

        // inserimento dei file in una cartella temporanei
        string basePathFiles = DocsPaVO.Settings.AppSettings.Instance.LOG_PATH;
        basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));

        //PALUMBO-LUCIANI: modifica per Path per tck 11616
        //basePathFiles = basePathFiles + "\\Invio_files";
        basePathFiles = basePathFiles + "\\Invio_files_" + System.Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");

        pathFiles = basePathFiles + "\\" + schedaDocumento.registro.codRegistro;
        DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);

        logger.Debug("Estrazione dei file da inviare");


        if (!estrazioneFiles(infoUtente, schedaDocumento, pathFiles, out CoppiaNomeFileENomeOriginale))
        {
            DocsPaUtils.Functions.Functions.CancellaDirectory(pathFiles);
            pathFiles = string.Empty;
        }

        return pathFiles;
    }

    private static SendDocumentResponse.SendDocumentMailResponse SendDocumentMail(
                                        DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                        DocsPaVO.utente.Corrispondente mittente,
                                        DocsPaVO.utente.Registro registroMittente,
                                        string mailAddress,
                                        ArrayList listDestinatari,
                                        DocsPaVO.amministrazione.CasellaRegistro casellaMittente,
                                        string pathFiles,
                                        Dictionary<string, string> CoppiaNomeFileENomeOriginale,
                                        bool confermaRicevuta)
    {
        SendDocumentResponse.SendDocumentMailResponse singleResponse = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse();
        singleResponse.SendSucceded = true;
        singleResponse.Destinatari.AddRange(listDestinatari);
        singleResponse.MailAddress = mailAddress;
        bool isCanaleInterop = false;
        logger.Debug("Creazione del file segnatura per l'indirizzo " + mailAddress);

        DocsPaVO.utente.Corrispondente dest = (DocsPaVO.utente.Corrispondente)listDestinatari[listDestinatari.Count - 1];

        // Verifica il canale preferenziale di tipo interoperabilità
        DocsPaVO.utente.Canale canalePref = GetCanalePreferenzialeDestinatario(dest);

        if (canalePref == null ||
            (canalePref != null &&
             canalePref.descrizione != "INTEROPERABILITA" &&
             canalePref.descrizione != "MAIL" &&
             canalePref.descrizione != "PORTALE"))
        {
            singleResponse.SendSucceded = false;
            singleResponse.MailNonInteroperante = true;
            singleResponse.SendErrorMessage = "Canale preferenziale per il destinatario non di tipo MAIL o INTEROPERABILITA', impossibile spedire il documento";
        }
        else
        {
            if (canalePref.descrizione.Equals("INTEROPERABILITA"))
            {
                isCanaleInterop = true;
                string creaAllegatoSegnaturaXml = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SEGNATURA_PROTO_ALLEGATO6");
                if (string.IsNullOrEmpty(creaAllegatoSegnaturaXml) || !creaAllegatoSegnaturaXml.Equals("1"))
                {
                    creaSegnatura(mittente, schedaDocumento.registro, registroMittente, schedaDocumento, mailAddress, listDestinatari, pathFiles, CoppiaNomeFileENomeOriginale, confermaRicevuta);
                }
            }

            //creazione ed invio mail
            string porta = null;

            if (casellaMittente.PortaSMTP != 0)
                porta = casellaMittente.PortaSMTP.ToString();

            string smtp_user = (!string.IsNullOrEmpty(casellaMittente.UserSMTP)) ? casellaMittente.UserSMTP : null;
            string smtp_pwd = (!string.IsNullOrEmpty(casellaMittente.PwdSMTP)) ? casellaMittente.PwdSMTP : null;

            string ricevutaPec = string.Empty;
            ricevutaPec = (!string.IsNullOrEmpty(casellaMittente.RicevutaPEC)) ? casellaMittente.RicevutaPEC : null;
            string X_TipoRicevuta = null;
            //aggiunta la trim() per gestire la presenza di spazi bianchi nei campi VAR_USER_SMTP e VAR_PWD_SMTP
            if (smtp_user != null)
                smtp_user = smtp_user.Trim();
            if (smtp_pwd != null)
                smtp_pwd = smtp_pwd.Trim();

            if (ricevutaPec != null)
            {

                if (ricevutaPec != string.Empty)
                {
                    X_TipoRicevuta = ricevutaPec;
                    switch (ricevutaPec.Length)
                    {
                        case 1:
                            X_TipoRicevuta = ricevutaPec.Substring(0, 1);
                            break;
                        case 2:
                            //Se la len è maggiore di uno, vuol dire che ho un valore diverso da quello di default
                            //Preleverò quindi quello.
                            X_TipoRicevuta = ricevutaPec.Substring(1, 1);
                            break;
                        default:    //non si sa mai
                            X_TipoRicevuta = string.Empty;
                            break;
                    }
                    //Qui transcodifico il tipo ricevuta CHA in header
                    //(sarebbe carino metterlo in un enum per evitare hardcoding nel codice).
                    switch (X_TipoRicevuta)
                    {
                        case "C":
                            X_TipoRicevuta = "completa";
                            break;
                        case "B":
                            X_TipoRicevuta = "breve";
                            break;
                        case "S":
                            X_TipoRicevuta = "sintetica";
                            break;
                        default:
                            X_TipoRicevuta = null;
                            break;
                    }

                }

            }

            logger.Debug("Creazione ed invio del messaggio all'indirizzo " + mailAddress);


            try
            {
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI")) &&
                    !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI").Equals("0") &&
                    canalePref.descrizione.Equals("PORTALE"))
                    creaMailNotificaPortale(schedaDocumento,
                        casellaMittente.ServerSMTP,
                        casellaMittente.EmailRegistro,
                        smtp_user,
                        smtp_pwd,
                        mailAddress,
                        porta,
                        casellaMittente.SmtpSSL.ToString(),
                        casellaMittente.PopSSL.ToString(),
                        casellaMittente.SmtpSta.ToString(),
                        X_TipoRicevuta
                        );
                else
                    creaMail(schedaDocumento,
                        casellaMittente.ServerSMTP,
                        casellaMittente.EmailRegistro,
                        smtp_user,
                        smtp_pwd,
                        mailAddress,
                        pathFiles,
                        CoppiaNomeFileENomeOriginale,
                        porta,
                        casellaMittente.SmtpSSL.ToString(),
                        casellaMittente.PopSSL.ToString(),
                        casellaMittente.SmtpSta.ToString(),
                        X_TipoRicevuta,
                        casellaMittente.MessageSendMail,
                        casellaMittente.OverwriteMessageAmm,
                        isCanaleInterop
                        );
            }
            catch (Exception ex)
            {
                singleResponse.SendSucceded = false;
                singleResponse.SendErrorMessage = ex.Message;
                logger.Error("Errore in SendDocumentMail: " + ex.Message);
            }

            //cancella file segnatura
            if (File.Exists(pathFiles + "\\segnatura.xml"))
                System.IO.File.Delete(pathFiles + "\\segnatura.xml");
        }

        return singleResponse;
    }

    private static bool estrazioneFiles(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, string path, out Dictionary<string, string> CoppiaNomeFileENomeOriginale)
    {
        System.IO.FileStream fs = null;
        System.IO.FileStream fsAll = null;
        CoppiaNomeFileENomeOriginale = new Dictionary<string, string>();
        byte[] content = null;
        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
        string docPrincipaleName = "";
        try
        {
            //estrazione documento principale
            DocsPaVO.documento.Documento doc = getDocumentoPrincipale(schedaDoc);
            if (doc.fileName != null && doc.fileName != "")
            {
                //inizio modifica: il documento principale in alcuni casi non veniva allegato,
                //Ciò accedeva quando durante l'estrazione del documento principale, per allegarlo alla mail,
                //nel percorso del file è presente un ulteriore '.', oltre a quello relativo all'estensione.
                //Ad esempio con un path del genere andava in errore ENTEC\2005\EC_RU\UO1.1\Partenza\467.XLS
                char[] dot = { '.' };
                string[] parts;
                string suffix = "";
                if (!doc.fileName.ToUpper().EndsWith("P7M"))
                {
                    parts = doc.fileName.Split(dot);
                    suffix = parts[parts.Length - 1];
                    docPrincipaleName = "Documento_principale." + suffix;
                }
                else
                {
                    string appodocPrincipaleName = doc.fileName.Substring(doc.fileName.LastIndexOf("\\") + 1);
                    parts = appodocPrincipaleName.Split(dot);
                    int cont = 0;
                    for (int i = 2; i < parts.Length; i++)
                    {
                        if (parts[i].ToUpper().Equals("P7M"))
                        {
                            cont = cont + 1;
                            suffix = suffix + ".P7M";
                        }
                    }
                    suffix = parts[parts.Length - cont - 1] + suffix;
                    appodocPrincipaleName = appodocPrincipaleName.Substring(appodocPrincipaleName.ToUpper().LastIndexOf(suffix.ToUpper()));
                    docPrincipaleName = "Documento_principale." + appodocPrincipaleName;
                }
                //fine modifica
                fs = new System.IO.FileStream(path + "\\" + docPrincipaleName, System.IO.FileMode.Create);

                //byte[] content=getDocument(infoUtente,doc.docNumber,doc.version,doc.versionId,doc.versionLabel,logger);

                //modifica
                DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                fd = BusinessLogic.Documenti.FileManager.getFileFirmato(doc, infoUtente, false);
                content = fd.content;
                //fien mofica
                string NomeOriginale;
                if (String.IsNullOrEmpty(fd.nomeOriginale))
                    NomeOriginale = fd.name;
                else
                    NomeOriginale = fd.nomeOriginale;

                CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + docPrincipaleName).ToLowerInvariant(), NomeOriginale);
                //content =documentManager.GetFile(doc.docNumber, doc.version, doc.versionId, doc.versionLabel);

                if (content == null)
                {
                    logger.Error("File allegato non valido");
                    throw new Exception();
                }
            }
            else
            {
                //crea un file di nome empty.txt
                fs = new System.IO.FileStream(path + "\\empty.txt", System.IO.FileMode.Create);
                CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\empty.txt").ToLowerInvariant(), "empty.txt");
            }

            if (content != null)
            {
                fs.Write(content, 0, content.Length);
            }
            if (fs != null)
                fs.Close();

            //gestione TSR
            if (content != null)
            {
                DocsPaVO.documento.FileRequest fr = new DocsPaVO.documento.FileRequest { docNumber = doc.docNumber, versionId = doc.versionId };
                byte[] tsr = InteroperabilitaUtils.GetTSRForDocument(infoUtente, fr);
                if (tsr != null)
                {
                    if (!BusinessLogic.Interoperabilita.InteroperabilitaUtils.MatchTSR(tsr, content))
                        tsr = null;

                    if (tsr != null)
                    {
                        string NomeOriginale = String.Format(path + "\\" + docPrincipaleName).ToLowerInvariant();
                        if (CoppiaNomeFileENomeOriginale.ContainsKey(NomeOriginale.ToLowerInvariant()))
                            NomeOriginale = CoppiaNomeFileENomeOriginale[NomeOriginale.ToLowerInvariant()] + ".tsr";

                        File.WriteAllBytes(path + "\\" + docPrincipaleName + ".tsr", tsr);
                        CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + docPrincipaleName + ".tsr").ToLowerInvariant(), NomeOriginale);
                    }
                }
            }


            //estrazione degli allegati
            byte[] all_content = null;

            //luluciani modifica 19/09/2012 evitiamo di spedire allegati pec o IS.
            //for (int i = 0; i < schedaDoc.allegati.Count; i++)
            //{
            int j = 0;
            DocsPaDB.Query_DocsPAWS.Documenti docWs = new DocsPaDB.Query_DocsPAWS.Documenti();
            foreach (DocsPaVO.documento.Allegato allegato in ((DocsPaVO.documento.Allegato[])schedaDoc.allegati.ToArray(typeof(DocsPaVO.documento.Allegato))).Where(
            a => BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(a.versionId) == "0" && BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(a.versionId) == "0"
             && docWs.GetTipologiaAllegato(a.versionId) != "D")
        )
            {
                //DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)schedaDoc.allegati[i];

                //2023: a volte crea un file di segnatura.xml vuoto e in fase di spedizione invia un allegato vuoto tando problemi al destinatario che li riceve.
                //quindi se il file segnatura.xml è vuoto non lo invio.
                if (allegato.TypeAttachment == 6 && (string.IsNullOrEmpty(allegato.fileSize) || Convert.ToInt32(allegato.fileSize) == 0))
                    continue;

                j = j + 1;

                string allegatoName = "Allegato_" + (j).ToString();

                string fileExt = getEstensione(allegato.fileName);
                //string fileExt = getEstensione(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);
                if (fileExt != "")
                {
                    fsAll = new System.IO.FileStream(path + "\\" + allegatoName + "." + fileExt, System.IO.FileMode.Create);


                    documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                    DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();

                    //fd = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUtente, false);
                    fd = BusinessLogic.Documenti.FileManager.getFileFirmato(allegato, infoUtente, false);
                    all_content = fd.content;

                    if (all_content == null)
                    {
                        logger.Error("Errore nella gestione dell'interoperabilità. (estrazioneFiles)");
                        throw new Exception();
                    }

                    string NomeOriginale;
                    if (String.IsNullOrEmpty(fd.nomeOriginale))
                        NomeOriginale = fd.name;
                    else
                        NomeOriginale = fd.nomeOriginale;

                    CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + allegatoName + "." + fileExt).ToLowerInvariant(), NomeOriginale);
                }
                else
                {
                    fsAll = new System.IO.FileStream(path + "\\" + allegatoName + ".TXT", System.IO.FileMode.Create);
                    CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + allegatoName + ".TXT").ToLowerInvariant(), allegatoName + ".TXT");
                }

                if (all_content != null)
                    fsAll.Write(all_content, 0, all_content.Length);



                //gestione TSR
                if (all_content != null)
                {
                    DocsPaVO.documento.FileRequest fr = new DocsPaVO.documento.FileRequest { docNumber = allegato.docNumber, versionId = allegato.versionId };
                    byte[] tsr = InteroperabilitaUtils.GetTSRForDocument(infoUtente, fr);
                    if (tsr != null)
                    {
                        if (!BusinessLogic.Interoperabilita.InteroperabilitaUtils.MatchTSR(tsr, all_content))
                            tsr = null;


                        if (tsr != null)
                        {
                            string NomeOriginale = path + "\\" + allegatoName + "." + fileExt;
                            if (CoppiaNomeFileENomeOriginale.ContainsKey(NomeOriginale.ToLowerInvariant()))
                                NomeOriginale = CoppiaNomeFileENomeOriginale[NomeOriginale.ToLowerInvariant()] + ".tsr";

                            File.WriteAllBytes(path + "\\" + allegatoName + "." + fileExt + ".tsr", tsr);
                            CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + allegatoName + "." + fileExt + ".tsr").ToLowerInvariant(), NomeOriginale);
                        }
                    }
                }

                fsAll.Close();
            }

            return true;
        }
        catch (Exception e)
        {
            logger.Error("Estrazione del file non eseguita.Eccezione: " + e.ToString());

            if (fs != null)
            {
                fs.Close();
            }

            if (fsAll != null)
            {
                fsAll.Close();
            }

            return false;
        }
    }

    /// <summary>
    /// Reperimento del canale preferenziale per il destintaraio
    /// </summary>
    /// <param name="destintario"></param>
    /// <returns></returns>
    public static DocsPaVO.utente.Canale GetCanalePreferenzialeDestinatario(DocsPaVO.utente.Corrispondente destintario)
    {
        logger.Debug("INIT - GetCanalePreferenzialeDestinatario");

        DocsPaVO.utente.Canale canalePref = null;

        try
        {
            if (!string.IsNullOrEmpty(destintario.tipoCorrispondente)
                && destintario.tipoCorrispondente.Equals("O"))
            {
                canalePref = new DocsPaVO.utente.Canale();
                canalePref.tipoCanale = "MAIL";
                canalePref.descrizione = "MAIL";
            }
            else
                if (destintario.tipoIE == "E")
            {
                // Destinatario esterno
                canalePref = destintario.canalePref;
            }
            else if (destintario.tipoIE == "I")
            {
                // Destinatario interno

                if (destintario.GetType() == typeof(DocsPaVO.utente.Ruolo))
                {
                    // Se Ruolo prende il canale preferenziale dell'UO
                    canalePref = ((DocsPaVO.utente.Ruolo)destintario).uo.canalePref;
                }
                else if (destintario.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                {
                    canalePref = ((DocsPaVO.utente.UnitaOrganizzativa)destintario).canalePref;
                }
                else
                {
                    // Se utente, cerca il ruolo preferito (se non c'è, prende il primo della lista),
                    // quindi prende il canale preferenziale dell'UO di tale utente
                    DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)destintario;

                    DocsPaVO.utente.Ruolo[] ruoliUtente = null;

                    // Cerca il ruolo preferito (se non c'è, prende il primo ruolo disponibile)
                    if (utente.ruoli == null || (utente.ruoli != null && utente.ruoli.Count == 0))
                        ruoliUtente = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));
                    else
                        ruoliUtente = (DocsPaVO.utente.Ruolo[])utente.ruoli.ToArray(typeof(DocsPaVO.utente.Ruolo));

                    if (ruoliUtente != null && ruoliUtente.Length > 0)
                    {
                        DocsPaVO.utente.Ruolo ruoloPreferito = ruoliUtente.Where(e => e.selezionato).FirstOrDefault();

                        if (ruoloPreferito == null)
                            // Ruolo preferito non disponibile, reperimento del primo ruolo della lista
                            ruoloPreferito = ruoliUtente[0];

                        if (ruoloPreferito != null)
                        {
                            if (ruoloPreferito.uo == null)
                                logger.Debug("GetCanalePreferenzialeDestinatario - UO non definita per il ruolo preferito");
                            else
                            {
                                if (ruoloPreferito.uo != null && ruoloPreferito.uo.canalePref == null)
                                {
                                    // Reperimento del canale associato all'UO del Ruolo
                                    using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                                        canalePref = amm.GetDatiCanPref(ruoloPreferito.uo);
                                }
                                else
                                {
                                    // Reperimento del canale associato all'UO del Ruolo
                                    canalePref = ruoloPreferito.uo.canalePref;
                                }
                            }
                        }
                    }
                }
                //modifica
                if (canalePref == null &&
                    destintario.tipoIE.Contains('I'))
                {
                    canalePref = new DocsPaVO.utente.Canale();
                    canalePref.tipoCanale = "INTEROPERABILITA";
                    canalePref.descrizione = "INTEROPERABILITA";
                }
                //fine modifica
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Errore in GetCanalePreferenzialeDestinatario");

            throw new ApplicationException(string.Format("Si è verificato un errore nel reperimento del canale preferenziale per il destinatario con ID '{0}'", destintario.systemId), ex);
        }
        finally
        {
            logger.Debug("END - GetCanalePreferenzialeDestinatario");
        }

        if (canalePref != null)
        {
            if (!string.IsNullOrEmpty(canalePref.descrizione))
                canalePref.descrizione = canalePref.descrizione.ToUpper();
            if (!string.IsNullOrEmpty(canalePref.tipoCanale))
                canalePref.tipoCanale = canalePref.tipoCanale.ToUpper();
        }
        return canalePref;
    }

    private static bool creaSegnatura(DocsPaVO.utente.Corrispondente mittSegnatura, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Registro regMittente, DocsPaVO.documento.SchedaDocumento schedaDoc, string mailDest, System.Collections.ArrayList destinatari, string pathFiles, Dictionary<string, string> CoppiaNomeFileENomeOriginale, bool confermaRic)
    {
        try
        {
            bool isInteropRGS = false;
            XmlDocument xdoc = new XmlDocument();

            //Impostazione
            xdoc.XmlResolver = null;
            XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
            xdoc.AppendChild(dec);

            //NON VALIDIAMO PIù CON LA DTD MA CON L'XSD
            //XmlDocumentType dtd = xdoc.CreateDocumentType("Segnatura", null, "Segnatura.dtd", null);
            //xdoc.AppendChild(dtd);
            //logger.Debug("dtd impostato");

            //Creazione della root
            XmlElement root = xdoc.CreateElement("Segnatura");
            root.SetAttribute("xmlns", "http://www.digitPa.gov.it/protocollo/");
            xdoc.AppendChild(root);

            //Creazione dell'intestazione
            XmlElement intestazione = xdoc.CreateElement("Intestazione");
            root.AppendChild(intestazione);
            XmlElement identificatore = xdoc.CreateElement("Identificatore");
            intestazione.AppendChild(identificatore);
            XmlElement origine = xdoc.CreateElement("Origine");
            intestazione.AppendChild(origine);
            XmlElement destinazione = xdoc.CreateElement("Destinazione");

            if (confermaRic)
            {
                destinazione.SetAttribute("confermaRicezione", "si");
            }
            else
            {
                destinazione.SetAttribute("confermaRicezione", "no");
            }

            intestazione.AppendChild(destinazione);
            XmlElement oggettoInt = xdoc.CreateElement("Oggetto");
            intestazione.AppendChild(oggettoInt);

            //Identificatore
            XmlElement codiceAmm = xdoc.CreateElement("CodiceAmministrazione");
            identificatore.AppendChild(codiceAmm);
            codiceAmm.InnerText = reg.codAmministrazione;
            XmlElement codiceAOO = xdoc.CreateElement("CodiceAOO");
            identificatore.AppendChild(codiceAOO);
            codiceAOO.InnerText = reg.codRegistro;
            XmlElement codiceRegistro = xdoc.CreateElement("CodiceRegistro");
            identificatore.AppendChild(codiceRegistro);
            codiceRegistro.InnerText = reg.codRegistro;
            XmlElement numeroReg = xdoc.CreateElement("NumeroRegistrazione");
            identificatore.AppendChild(numeroReg);

            int MAX_LENGTH = 7;
            string zeroes = "";
            string numProto = schedaDoc.protocollo.numero;
            for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
            {
                zeroes = zeroes + "0";
            }
            numProto = zeroes + numProto;
            numeroReg.InnerText = numProto;

            //numeroReg.InnerText = schedaDoc.protocollo.numero;
            XmlElement dataReg = xdoc.CreateElement("DataRegistrazione");
            identificatore.AppendChild(dataReg);
            dataReg.InnerText = DocsPaUtils.Functions.Functions.CheckData_Invio(schedaDoc.protocollo.dataProtocollazione);  //DA CONVERTIRE

            //Origine
            XmlElement indirizzoTel = xdoc.CreateElement("IndirizzoTelematico");
            origine.AppendChild(indirizzoTel);
            if (regMittente != null)
                indirizzoTel.InnerText = regMittente.email;
            else
                indirizzoTel.InnerText = reg.email;
            XmlElement mittente = xdoc.CreateElement("Mittente");

            //Si riempie il campo mittente
            getCorrispondente(mittSegnatura, mittente, xdoc);
            XmlElement AOO = xdoc.CreateElement("AOO");
            mittente.AppendChild(AOO);
            XmlElement denominazioneAOO = xdoc.CreateElement("Denominazione");
            denominazioneAOO.InnerText = reg.codRegistro;
            AOO.AppendChild(denominazioneAOO);
            origine.AppendChild(mittente);

            //Destinazione
            XmlElement indirizzoTelematico = xdoc.CreateElement("IndirizzoTelematico");
            indirizzoTelematico.InnerText = mailDest;
            destinazione.AppendChild(indirizzoTelematico);

            for (int i = 0; i < destinatari.Count; i++)
            {
                XmlElement destinatario = xdoc.CreateElement("Destinatario");
                getCorrispondente((DocsPaVO.utente.Corrispondente)destinatari[i], destinatario, xdoc);
                destinazione.AppendChild(destinatario);
                logger.Debug("Destinatario aggiunto");
            }

            //Oggetto
            oggettoInt.InnerText = schedaDoc.oggetto.descrizione;

            #region Riferimenti Mittente
            //I contesti procedurali saranno due, il primo creato per mantenere la compatibilità con la vecchia versione dei CC
            //il secondo è quello ufficiale
            XmlElement riferimenti = xdoc.CreateElement("Riferimenti");
            root.AppendChild(riferimenti);

            //Primo Contesto Procedurale
            XmlElement contestoProceduraleUno = xdoc.CreateElement("ContestoProcedurale");

            XmlElement codiceAmmUno = xdoc.CreateElement("CodiceAmministrazione");
            codiceAmmUno.InnerText = reg.codAmministrazione;
            contestoProceduraleUno.AppendChild(codiceAmmUno);

            XmlElement codiceAOOUno = xdoc.CreateElement("CodiceAOO");
            codiceAOOUno.InnerText = reg.codRegistro;
            contestoProceduraleUno.AppendChild(codiceAOOUno);

            XmlElement identificativoUno = xdoc.CreateElement("Identificativo");
            if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
                identificativoUno.InnerText = schedaDoc.protocolloTitolario;
            else
                identificativoUno.InnerText = schedaDoc.riferimentoMittente;
            contestoProceduraleUno.AppendChild(identificativoUno);

            XmlElement tipoContestoProceduraleUno = xdoc.CreateElement("TipoContestoProcedurale");
            tipoContestoProceduraleUno.InnerText = "Protocollo Arma";
            contestoProceduraleUno.AppendChild(tipoContestoProceduraleUno);

            XmlElement oggettoContestoProceduraleUno = xdoc.CreateElement("Oggetto");
            oggettoContestoProceduraleUno.InnerText = "Pratica nuova";
            contestoProceduraleUno.AppendChild(oggettoContestoProceduraleUno);

            //Secondo Constesto Procedurale
            XmlElement contestoProceduraleDue = xdoc.CreateElement("ContestoProcedurale");

            XmlElement codiceAmmDue = xdoc.CreateElement("CodiceAmministrazione");
            codiceAmmDue.InnerText = reg.codAmministrazione;
            contestoProceduraleDue.AppendChild(codiceAmmDue);

            XmlElement codiceAOODue = xdoc.CreateElement("CodiceAOO");
            codiceAOODue.InnerText = reg.codRegistro;
            contestoProceduraleDue.AppendChild(codiceAOODue);

            XmlElement identificativoDue = xdoc.CreateElement("Identificativo");
            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione);
            //Se il protocollo titoalario esiste viene trasmesso con l'aggiunta del riferimento mittente che sarebbe il protocollo titolario epurato del sottonumero
            //altrimenti viene trasmesso solo il riferimento mittente
            if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
                identificativoDue.InnerText = schedaDoc.protocolloTitolario + "$" + schedaDoc.riferimentoMittente;
            else
                identificativoDue.InnerText = schedaDoc.riferimentoMittente;
            contestoProceduraleDue.AppendChild(identificativoDue);

            XmlElement tipoContestoProceduraleDue = xdoc.CreateElement("TipoContestoProcedurale");
            tipoContestoProceduraleDue.InnerText = "Codice Classifica";
            contestoProceduraleDue.AppendChild(tipoContestoProceduraleDue);

            XmlElement oggettoContestoProceduraleDue = xdoc.CreateElement("Oggetto");
            oggettoContestoProceduraleDue.InnerText = "Classificazione";
            contestoProceduraleDue.AppendChild(oggettoContestoProceduraleDue);

            riferimenti.AppendChild(contestoProceduraleUno);
            riferimenti.AppendChild(contestoProceduraleDue);

            #region Contesto procedurale RGS

            if (schedaDoc.template != null && !string.IsNullOrEmpty(schedaDoc.template.ID_CONTESTO_PROCEDURALE)
                && schedaDoc.spedizioneDocumento != null &&
                schedaDoc.spedizioneDocumento.tipoMessaggio != null &&
                !string.IsNullOrEmpty(schedaDoc.spedizioneDocumento.tipoMessaggio.ID)
                )
            {
                //Verifico se è interoperante RGS

                bool interoperanteRGS = false;
                interoperanteRGS = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.CheckIsInteroperanteRGS(((DocsPaVO.utente.Corrispondente)destinatari[0]).systemId);
                if (interoperanteRGS)
                {
                    isInteropRGS = true;
                    DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.FlussoAutomatico.ContestoProcedurale contestoProceduraleRGS = model.GetContestoProceduraleById(schedaDoc.template.ID_CONTESTO_PROCEDURALE);
                    string idProcesso = FlussoAutomatico.FlussoAutomaticoManager.GetIdProcessoFlusso(schedaDoc, schedaDoc.spedizioneDocumento.tipoMessaggio);

                    XmlElement contestoProceduraleFlussoRGS = xdoc.CreateElement("ContestoProcedurale");
                    contestoProceduraleFlussoRGS.SetAttribute("id", idProcesso);

                    XmlElement codiceAmmRGS = xdoc.CreateElement("CodiceAmministrazione");
                    codiceAmmRGS.InnerText = reg.codAmministrazione;
                    contestoProceduraleFlussoRGS.AppendChild(codiceAmmRGS);

                    XmlElement codiceAOORGS = xdoc.CreateElement("CodiceAOO");
                    codiceAOORGS.InnerText = reg.codRegistro;
                    contestoProceduraleFlussoRGS.AppendChild(codiceAOORGS);

                    XmlElement identificativoRGS = xdoc.CreateElement("Identificativo");
                    identificativoRGS.InnerText = schedaDoc.spedizioneDocumento.tipoMessaggio.ID;
                    contestoProceduraleFlussoRGS.AppendChild(identificativoRGS);

                    XmlElement tipoContestoProceduraleRGS = xdoc.CreateElement("TipoContestoProcedurale");
                    tipoContestoProceduraleRGS.InnerText = contestoProceduraleRGS.TIPO_CONTESTO_PROCEDURALE;
                    contestoProceduraleFlussoRGS.AppendChild(tipoContestoProceduraleRGS);

                    #region PiuInfo

                    XmlElement piuInfoRGS = xdoc.CreateElement("PiuInfo");
                    piuInfoRGS.SetAttribute("XMLSchema", "AttributiEstesi.xsd");

                    XmlElement metadatiInterniRGS = xdoc.CreateElement("MetadatiInterni");

                    //XmlCDataSection cDataRGS;
                    string dataRGS = "<![CDATA[<TIPOLOGIA><NOME>" + contestoProceduraleRGS.NOME + "</NOME><FAMIGLIA>" + contestoProceduraleRGS.FAMIGLIA + "</FAMIGLIA><VERSIONE>" + contestoProceduraleRGS.VERSIONE + "</VERSIONE>METADATOASSOCIATO</TIPOLOGIA>]]>";
                    string metadatoAssociato = string.Empty;

                    metadatoAssociato += "<MetadatoAssociato><Codice>TIPOLOGIA</Codice><Valore>" + schedaDoc.template.DESCRIZIONE + "</Valore></MetadatoAssociato>";

                    string codice = string.Empty;
                    string valore = string.Empty;
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in schedaDoc.template.ELENCO_OGGETTI)
                    {
                        if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                        {
                            codice = ogg.DESCRIZIONE;
                            valore = string.Empty;

                            switch (ogg.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "Link":
                                    valore = ogg.VALORE_DATABASE.Split(new string[] { "||||" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    break;
                                case "Corrispondente":
                                    // XmlCDataSection cDataCorr;
                                    Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(ogg.VALORE_DATABASE);
                                    DocsPaVO.addressbook.CorrespondentDetails corrDett = null;
                                    if (corr.dettagli)
                                    {
                                        corrDett = new DocsPaDB.Query_DocsPAWS.Utenti().getCorrespondentDetails(ogg.VALORE_DATABASE);
                                    }
                                    if (corr != null)
                                    {
                                        string dataCorr = "<![CDATA[<AnagraficaDipendente><NomeDipendente>" + corr.nome + "</NomeDipendente><CognomeDipendente>" + corr.cognome + "</CognomeDipendente>";
                                        if (corrDett != null)
                                        {
                                            dataCorr += "<DataNascitaDipendente>" + corrDett.BirthDay + "</DataNascitaDipendente>";

                                            string sessoDipendente = string.Empty;
                                            if (!string.IsNullOrEmpty(corrDett.TaxId))
                                            {
                                                sessoDipendente = Convert.ToInt32(corrDett.TaxId.Substring(9, 2)) > 40 ? "F" : "M";
                                            }
                                            dataCorr += "<SessoDipendente>" + sessoDipendente + "</SessoDipendente>";
                                            dataCorr += "<ComuneNascitaDipendente>" + corrDett.BirthPlace + "</ComuneNascitaDipendente>";
                                            dataCorr += "<CodiceFiscaleDipendente>" + corrDett.TaxId + "</CodiceFiscaleDipendente>";
                                        }
                                        dataCorr += "</AnagraficaDipendente>]]]]><![CDATA[>";
                                        valore = dataCorr;
                                    }
                                    break;
                                case "Contatore":
                                    String dataAnnullamento = String.Empty;
                                    valore = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(schedaDoc.docNumber, reg.idAmministrazione, false, out dataAnnullamento);
                                    break;
                                case "CasellaDiSelezione":
                                    foreach (string val in ogg.VALORI_SELEZIONATI)
                                    {
                                        if (!string.IsNullOrEmpty(val))
                                            valore += "<Valore>" + val + "</valore>";
                                    }
                                    break;
                                case "Data":
                                    valore = Convert.ToDateTime(ogg.VALORE_DATABASE).ToString("dd/MM/yyyy");
                                    break;
                                default:
                                    valore = ogg.VALORE_DATABASE;
                                    break;
                            }
                            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                                metadatoAssociato += "<MetadatoAssociato><Codice>" + codice + "</Codice>" + valore + "</MetadatoAssociato>";
                            else
                                metadatoAssociato += "<MetadatoAssociato><Codice>" + codice + "</Codice><Valore>" + valore + "</Valore></MetadatoAssociato>";
                        }
                    }

                    dataRGS = dataRGS.Replace("METADATOASSOCIATO", metadatoAssociato);
                    //cDataRGS = xdoc.CreateCDataSection(dataRGS);
                    //metadatiInterniRGS.AppendChild(cDataRGS);
                    metadatiInterniRGS.InnerText = dataRGS;

                    piuInfoRGS.AppendChild(metadatiInterniRGS);
                    contestoProceduraleFlussoRGS.AppendChild(piuInfoRGS);

                    #endregion

                    riferimenti.AppendChild(contestoProceduraleFlussoRGS);
                }

            }

            #endregion

            #endregion Riferimenti Mittente

            //Descrizione
            XmlElement descrizione = xdoc.CreateElement("Descrizione");
            root.AppendChild(descrizione);
            XmlElement docPrinc = xdoc.CreateElement("Documento");


            string estensioneFile = getEstensione(getDocumentoPrincipale(schedaDoc).fileName);
            string nomefile = "Documento_principale." + estensioneFile;
            if (CoppiaNomeFileENomeOriginale.ContainsKey(String.Format(pathFiles + @"\" + nomefile).ToLowerInvariant()))
                nomefile = CoppiaNomeFileENomeOriginale[String.Format(pathFiles + @"\" + nomefile).ToLowerInvariant()];

            if (estensioneFile != "")
            {
                docPrinc.SetAttribute("nome", nomefile);
            }
            else
            {
                //per ovviare al problema del documentale EtDoc (dovrà essere studiata meglio la soluzione)
                docPrinc.SetAttribute("nome", "empty.TXT");
            }
            logger.Debug(getDocumentoPrincipale(schedaDoc).fileName);
            logger.Debug("Estensione doc: " + getEstensione(getDocumentoPrincipale(schedaDoc).fileName));

            docPrinc.SetAttribute("tipoRiferimento", "MIME");

            //si aggiunge l'oggetto della schedaDocumento:
            XmlElement oggetto = xdoc.CreateElement("Oggetto");
            oggetto.InnerText = schedaDoc.oggetto.descrizione;
            docPrinc.AppendChild(oggetto);
            descrizione.AppendChild(docPrinc);

            foreach (string tsrVal in CoppiaNomeFileENomeOriginale.Values)
            {
                if (Path.GetExtension(tsrVal).ToLowerInvariant() == ".tsr")
                {
                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato { fileName = tsrVal, descrizione = "Marca Temporale TSR", versionId = "1" };
                    schedaDoc.allegati.Add(all);
                }
            }


            //si aggiungono gli allegati
            if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
            {
                int countIS = 0;
                int countPEC = 0;

                for (int i = 0; i < schedaDoc.allegati.Count; i++)
                {
                    if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) == "1")
                        countIS++;
                    if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) == "1")
                        countPEC++;
                }

                if (schedaDoc.allegati.Count - countIS - countPEC > 0)
                {
                    XmlElement allegati = xdoc.CreateElement("Allegati");
                    descrizione.AppendChild(allegati);

                    for (int i = 0; i < schedaDoc.allegati.Count; i++)
                    {

                        if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) != "1" &&
                            BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) != "1")
                        {

                            XmlElement allegato = xdoc.CreateElement("Documento");
                            string fileExt = getEstensione(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);


                            string nomefileAll = "Allegato_" + (i + 1).ToString() + "." + fileExt;
                            if (fileExt == "")
                                nomefileAll = "Allegato_" + (i + 1).ToString() + ".TXT";


                            if (CoppiaNomeFileENomeOriginale.ContainsKey(String.Format(pathFiles + @"\" + nomefileAll).ToLowerInvariant()))
                                nomefileAll = CoppiaNomeFileENomeOriginale[String.Format(pathFiles + @"\" + nomefileAll).ToLowerInvariant()];
                            else if (fileExt.ToLowerInvariant() == "tsr")  //nel caso fosse un TSR (allegato ombra) prendo il nome popolato sopra.
                                nomefileAll = Path.GetFileName(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);

                            allegato.SetAttribute("nome", nomefileAll);
                            allegato.SetAttribute("tipoRiferimento", "MIME");

                            if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione != null)
                            {
                                XmlElement titoloDoc = xdoc.CreateElement("TitoloDocumento");
                                titoloDoc.InnerText = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione;
                                allegato.AppendChild(titoloDoc);
                            }

                            allegati.AppendChild(allegato);

                            if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine != 0)
                            {
                                XmlElement numPagineAll = xdoc.CreateElement("NumeroPagine");
                                numPagineAll.InnerText = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine.ToString();
                                allegato.AppendChild(numPagineAll);
                            }
                        }
                    }
                }
            }


            //NOTE (Augusto 30/08/2011)
            string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOTE_IN_SEGNATURA");
            if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
            {
                DocsPaDB.Query_DocsPAWS.Documenti queryDoc = new DocsPaDB.Query_DocsPAWS.Documenti();
                string ultimaNotaVisibileTutti = queryDoc.GetUltimaNotaVisibileTuttiDocumento(schedaDoc.systemId);
                if (!string.IsNullOrEmpty(ultimaNotaVisibileTutti))
                {
                    XmlElement noteDescrizione = xdoc.CreateElement("Note");
                    noteDescrizione.InnerText = ultimaNotaVisibileTutti;
                    //descrizione.SetAttribute("Note", ultimaNotaVisibileTutti);
                    descrizione.AppendChild(noteDescrizione);
                }
            }

            //Salvataggio
            if (isInteropRGS)
            {
#if false   // usa HttpContext
                string xmlString = HttpContext.Current.Server.HtmlDecode(xdoc.InnerXml);
                File.WriteAllText(pathFiles + "\\segnatura.xml", xmlString);

#endif
            }
            else
            {
                System.IO.FileStream fs = new System.IO.FileStream(pathFiles + "\\segnatura.xml", System.IO.FileMode.Create);
                xdoc.Save(fs);
                fs.Close();
            }

            return true;
        }
        catch (Exception e)
        {
            logger.Error("Errore nella creazione del file di segnatura. Eccezione: " + e.ToString());

            return false;
        }
    }

    private static bool creaMailNotificaPortale(DocsPaVO.documento.SchedaDocumento schedaDoc, string server, string mailMitt, string smtp_user, string smtp_pwd, string mailDest, string port, string SmtpSsl, string PopSsl, string smtpSTA, string X_TipoRicevuta)
    {
        bool retValue = false;

        SvrPosta svr = new SvrPosta(server,
    smtp_user,
    smtp_pwd,
    port,
    Path.GetTempPath(),
        CMClientType.SMTP, SmtpSsl, PopSsl, smtpSTA);

        try
        {
            svr.connect();

            // Lista fascicoli che contengono il documento
            // Da questi si individua il procedimento che contiene il documento
            DocsPaVO.utente.Utente u = UserManager.getUtenteById(schedaDoc.creatoreDocumento.idPeople);
            DocsPaVO.utente.Ruolo r = UserManager.getRuoloById(schedaDoc.creatoreDocumento.idCorrGlob_Ruolo);
            DocsPaVO.utente.InfoUtente infoUtente = UserManager.GetInfoUtente(u, r);
            ArrayList listaFasc = Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, schedaDoc.docNumber);

            string idProcedimento = string.Empty;
            DocsPaVO.Procedimento.Procedimento proc = null;
            if (listaFasc != null && listaFasc.Count > 0)
            {
                foreach (DocsPaVO.fascicolazione.Fascicolo fasc in listaFasc)
                {
                    proc = new DocsPaVO.Procedimento.Procedimento();
                    proc = Procedimenti.ProcedimentiManager.GetProcedimentoByIdFascicolo(fasc.systemID);
                    if (proc != null && proc.Id == fasc.systemID)
                    {
                        idProcedimento = fasc.systemID;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(idProcedimento))
                {
                    proc = null;
                }
            }

            if (proc == null)
            {
                throw new Exception("Impossibile trovare il procedimento associato");
            }

            // Corpo della mail
            string rootUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ROOT_URL_PORTALE");
            string bodyMail = string.Format("E' stato ricevuto un nuovo documento relativo al procedimento \"{0}\".<br>", proc.Descrizione);
            bodyMail = bodyMail + "Il documento è consultabile nella sezione 'I miei procedimenti' del Portale dei Procedimenti MiBACT";

            if (!string.IsNullOrEmpty(rootUrl))
            {
                string linkUrl = rootUrl + "/Procedimenti/DettaglioProcedimento.aspx?i=" + proc.IdEsterno + "&r=1";
                bodyMail = bodyMail + " o selezionando il seguente link.<br>";
                bodyMail = bodyMail + "<a href='" + linkUrl + "' >Vai al dettaglio del procedimento</a>";
            }

            string subject = string.Empty;

            if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.SEGNATURA_NEL_SUBJECT) &&
                bool.Parse(DocsPaVO.Settings.AppSettings.Instance.SEGNATURA_NEL_SUBJECT))
                subject = schedaDoc.protocollo.segnatura + " - " + schedaDoc.oggetto.descrizione;
            else
                subject = schedaDoc.oggetto.descrizione;

            //aggiunta del docnumber all'oggetto delal mail per la gestione delle ricevute pec
            if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.GESTIONE_RICEVUTE_PEC) &&
                bool.Parse(DocsPaVO.Settings.AppSettings.Instance.GESTIONE_RICEVUTE_PEC))
                subject += "#" + schedaDoc.docNumber + "#";

            List<CMMailHeaders> headers = new List<CMMailHeaders>();
            if ((X_TipoRicevuta != null) && (X_TipoRicevuta != string.Empty))
                headers.Add(new CMMailHeaders { header = "X-TipoRicevuta", value = X_TipoRicevuta });

            string outErr;
            svr.sendMail(
                mailMitt,
                mailDest,
                "",
                "",
                subject,
                bodyMail,
                CMMailFormat.HTML,
                null,
                headers.ToArray(), out outErr);

            // Inserimento documento nel procedimento
            BusinessLogic.Procedimenti.ProcedimentiManager.InsertDoc(proc.Id, schedaDoc.docNumber, proc.Autore, false, proc.IdEsterno, false);

            // Automatismo CABLATO per cambio stato
            if (schedaDoc.template != null)
            {
                BusinessLogic.Procedimenti.ProcedimentiManager.CambioStatoProcedimento(proc.Id, "SPEDIZIONE", schedaDoc.template.ID_TIPO_ATTO, infoUtente);
            }

            // Consolidamento del documento - SPOSTATO ALLA RICEZIONE DELLA NOTIFICA DI LETTURA
            //try
            //{
            //    BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(infoUtente, schedaDoc.docNumber, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2, true);
            //}
            //catch (Exception ex1)
            //{
            //    logger.Error("Errore nel consolidamento! Eccezione: " + ex1.ToString());
            //}

            retValue = true;

        }
        catch (Exception ex)
        {
            logger.Error("Creazione ed invio mail non eseguito. Eccezione: " + ex.ToString());

            throw ex;
        }
        finally
        {
            svr.disconnect();
        }

        return retValue;
    }

    private static bool creaMail(DocsPaVO.documento.SchedaDocumento schedaDoc, string server, string mailMitt, string smtp_user, string smtp_pwd, string mailDest, string pathFiles, Dictionary<string, string> CoppiaNomeFileENomeOriginale, string port, string SmtpSsl, string PopSsl, string smtpSTA, string X_TipoRicevuta, string msgMailReg, bool overwriteMsgAmm, bool isCanaleInterop)
    {
        bool retValue = false;

        SvrPosta svr = new SvrPosta(server,
    smtp_user,
    smtp_pwd,
    port,
    Path.GetTempPath(),
        CMClientType.SMTP, SmtpSsl, PopSsl, smtpSTA);

        try
        {
            svr.connect();

            //body della mail
            string bodyMail = "Si trasmette come file allegato a questa e-mail il documento e gli eventuali allegati.<br>";
            bodyMail = bodyMail + "Registro: " + schedaDoc.registro.codRegistro + "<br>";
            bodyMail = bodyMail + "Numero di protocollo: " + schedaDoc.protocollo.numero + "<br>";
            bodyMail = bodyMail + "Data protocollazione: " + schedaDoc.protocollo.dataProtocollazione + "<br>";
            bodyMail = bodyMail + "Segnatura: " + schedaDoc.protocollo.segnatura + "<br>";

            //Aggiungo al body messaggi configurati in amministrazione con chiave o sulla singola pec
            string bodyMailMsgPec = string.Empty;
            if (!string.IsNullOrEmpty(msgMailReg))
                bodyMailMsgPec += "<br>" + msgMailReg.Replace("\n", "<br>") + "<br>";
            if (!overwriteMsgAmm)
            {
                //Aggiungo al body del testo configurabile via chiave di configurazione
                string bodyMailConfig = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(schedaDoc.registro.idAmministrazione, "BE_MESSAGE_SEND_PEC");
                if (!string.IsNullOrEmpty(bodyMailConfig) && !bodyMailConfig.ToString().Equals("0"))
                {
                    bodyMail += "<br>" + bodyMailConfig.Replace("\n", "<br>") + "<br>";
                }
            }
            bodyMail += bodyMailMsgPec;

            string subject = string.Empty;

            if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.SEGNATURA_NEL_SUBJECT) &&
                bool.Parse(DocsPaVO.Settings.AppSettings.Instance.SEGNATURA_NEL_SUBJECT))
                subject = schedaDoc.protocollo.segnatura + " - " + schedaDoc.oggetto.descrizione;
            else
                subject = schedaDoc.oggetto.descrizione;

            //aggiunta del docnumber all'oggetto delal mail per la gestione delle ricevute pec
            if (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.GESTIONE_RICEVUTE_PEC) &&
                bool.Parse(DocsPaVO.Settings.AppSettings.Instance.GESTIONE_RICEVUTE_PEC))
                subject += "#" + schedaDoc.docNumber + "#";

            List<CMMailHeaders> headers = new List<CMMailHeaders>();
            if ((X_TipoRicevuta != null) && (X_TipoRicevuta != string.Empty))
                headers.Add(new CMMailHeaders { header = "X-TipoRicevuta", value = X_TipoRicevuta });

            string[] files = System.IO.Directory.GetFiles(pathFiles);
            List<CMAttachment> attachLst = new List<CMAttachment>();
            foreach (string file in files)
            {
                CMAttachment att = new CMAttachment(Path.GetFileName(file), Interoperabilita.MimeMapper.GetMimeType(Path.GetExtension(file)), file);
                //Valorizzo il nome originale del file qualora esso fosse presente
                if (CoppiaNomeFileENomeOriginale.ContainsKey(file.ToLowerInvariant()))
                    att.name = CoppiaNomeFileENomeOriginale[file.ToLowerInvariant()];
                if (!att.name.Equals("segnatura.xml") || isCanaleInterop)
                    attachLst.Add(att);
            }

            string outErr;
            svr.sendMail(
                mailMitt,
                mailDest,
                "",
                "",
                subject,
                bodyMail,
                CMMailFormat.HTML,
                attachLst.ToArray(),
                headers.ToArray(), out outErr);

            retValue = true;
        }
        catch (Exception e)
        {
            logger.Error("Creazione ed invio mail non eseguito. Eccezione: " + e.ToString());

            throw e;
        }
        finally
        {
            svr.disconnect();
        }

        return retValue;
    }

    private static void getCorrispondente(DocsPaVO.utente.Corrispondente corr, XmlElement mittente, XmlDocument xdoc)
    {
        //			logger.addMessage("getCorrispondente");
        logger.Debug("getCorrispondente");

        System.Collections.ArrayList nomiUO = new System.Collections.ArrayList();
        DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
        DocsPaVO.utente.Ruolo ruolo = null;
        DocsPaVO.utente.Utente utente = null;

        if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
        {
            utente = (DocsPaVO.utente.Utente)corr;

            if (utente.ruoli != null && utente.ruoli.Count > 0)
            {
                ruolo = (DocsPaVO.utente.Ruolo)utente.ruoli[0];
                uo = ruolo.uo;
            }
            else
            {
                //					logger.addMessage("Utente sciolto");
                logger.Debug("Utente sciolto");
                uo = null;
            }
        }

        if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
        {
            ruolo = (DocsPaVO.utente.Ruolo)corr;
            uo = ruolo.uo;
        }

        if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
        {
            uo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
        }

        string indPostString = null;

        if (uo != null)
        {
            indPostString = uo.indirizzo;
        }

        while (uo != null)
        {
            if (!string.IsNullOrEmpty(uo.descrizione))
                nomiUO.Add(uo.descrizione);

            if (uo.parent != null)
            {
                uo = uo.parent;
                //perchè la uo ha solo l'id, così estraggo gli altri dati.
                uo = (DocsPaVO.utente.UnitaOrganizzativa)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(uo.systemId);
            }
            else
                break;
        }

        //			logger.addMessage("nomiUO count="+nomiUO.Count);
        logger.Debug("nomiUO count=" + nomiUO.Count);

        //costruzione dell'elemento XML
        XmlElement amministrazione = xdoc.CreateElement("Amministrazione");
        mittente.AppendChild(amministrazione);
        XmlElement amministrazioneNome = xdoc.CreateElement("Denominazione");

        if (nomiUO.Count > 0)
        {
            amministrazioneNome.InnerText = (string)nomiUO[nomiUO.Count - 1];
        }
        else
        {
            amministrazioneNome.InnerText = "Non specificato";
        }

        amministrazione.AppendChild(amministrazioneNome);
        XmlElement temp = amministrazione;

        for (int i = nomiUO.Count - 1; i > -1; i--)
        {
            XmlElement uoEl = xdoc.CreateElement("UnitaOrganizzativa");
            XmlElement uoNome = xdoc.CreateElement("Denominazione");
            uoNome.InnerText = (string)nomiUO[i];
            uoEl.AppendChild(uoNome);
            temp.AppendChild(uoEl);
            temp = uoEl;
        }

        XmlElement temp2 = temp;

        //ruolo e utente
        if (ruolo != null)
        {
            XmlElement ruo = xdoc.CreateElement("Ruolo");
            XmlElement denomRuolo = xdoc.CreateElement("Denominazione");
            denomRuolo.InnerText = ruolo.descrizione;
            ruo.AppendChild(denomRuolo);
            temp.AppendChild(ruo);
            temp = ruo;
        }

        if (utente != null)
        {
            XmlElement ut = xdoc.CreateElement("Persona");
            XmlElement denomUt = xdoc.CreateElement("Denominazione");
            denomUt.InnerText = utente.descrizione;
            ut.AppendChild(denomUt);
            temp.AppendChild(ut);
            temp = ut;
        }

        //indirizzo postale
        XmlElement indPost = xdoc.CreateElement("IndirizzoPostale");
        XmlElement denomInd = xdoc.CreateElement("Denominazione");

        if (indPostString != null)
        {
            denomInd.InnerText = indPostString;
        }
        else
        {
            denomInd.InnerText = "Non specificato";
        }

        indPost.AppendChild(denomInd);
        temp2.AppendChild(indPost);
        //			logger.addMessage("Indirizzo postale inserito");
        logger.Debug("Indirizzo postale inserito");
    }
}
