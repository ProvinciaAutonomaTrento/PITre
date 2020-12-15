using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.InstanceAccess;
using DocsPaVO.documento;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessLogic.InstanceAccess
{
    class XmlDocument
    {
        DocsPaVO.InstanceAccess.Metadata.Document  documento;
        public string XmlFile
        {
            get
            {
                return SerializeObject<DocsPaVO.InstanceAccess.Metadata.Document>(documento);
            }
        }

        public DocsPaVO.InstanceAccess.Metadata.Document Documento
        {
            get
            {
                return documento;
            }
            set
            {
                value = documento;
            }
        }

        public XmlDocument(DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.documento.FileRequest objFileRequest, InstanceAccessDocument instanceDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (this.documento == null)
                documento = new DocsPaVO.InstanceAccess.Metadata.Document();

            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(schDoc.creatoreDocumento.idCorrGlob_Ruolo);
            //DocsPaVO.utente.InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(BusinessLogic.Utenti.UserManager.getUtente(schDoc.creatoreDocumento.idPeople), ruolo);
            DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa = ruolo.uo;

            List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa> uoL = new List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa>();
            DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa uoXML = convertiUO(unitaOrganizzativa);
            uoL.Add(uoXML);
            
            documento.SoggettoProduttore = new DocsPaVO.InstanceAccess.Metadata.SoggettoProduttore
            {
                Amministrazione = getInfoAmministrazione(ruolo.idAmministrazione),
                GerarchiaUO = new DocsPaVO.InstanceAccess.Metadata.GerarchiaUO { UnitaOrganizzativa = uoL.ToArray() },
                Creatore = getCreatore(schDoc, ruolo)
            };

            documento.IDdocumento = schDoc.systemId;
            documento.Oggetto = schDoc.oggetto.descrizione;
            documento.Tipo = convertiTipoPoto(schDoc);
            //  documento.DataCreazione = Utils.formattaData(Utils.convertiData(schDoc.dataCreazione));
            documento.DataCreazione = schDoc.dataCreazione;

            if (schDoc.privato != null && schDoc.privato.Equals("1"))
                documento.LivelloRiservatezza = "privato";
            else
                documento.LivelloRiservatezza = string.Empty;
            if(instanceDoc.ENABLE && fileDoc != null)
                documento.File = getFileDetail(fileDoc, objFileRequest,infoUtente);
            documento.Registrazione = getRegistrazione(schDoc, ruolo);
            documento.ContestoArchivistico = getContestoArchivistico(schDoc, ruolo, infoUtente);

            if (schDoc.template != null)
            {
                DocsPaVO.InstanceAccess.Metadata.Tipologia t = new DocsPaVO.InstanceAccess.Metadata.Tipologia { NomeTipologia = schDoc.template.DESCRIZIONE, CampoTipologia = getCampiTipologia(schDoc.template) };
                documento.Tipologia = t;
            }
            documento.Allegati = getAllegati(schDoc, instanceDoc, infoUtente);
            documento.TipoRichiesta = instanceDoc.TYPE_REQUEST;
        }

        private DocsPaVO.InstanceAccess.Metadata.Allegato[] getAllegati(DocsPaVO.documento.SchedaDocumento schDoc, InstanceAccessDocument doc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (schDoc.allegati == null)
                return null;
            if (schDoc.allegati.Count == 0)
                return null;

            List<DocsPaVO.InstanceAccess.Metadata.Allegato> lstAll = new List<DocsPaVO.InstanceAccess.Metadata.Allegato>();
            foreach (object a in schDoc.allegati)
            {
                DocsPaVO.InstanceAccess.Metadata.Allegato allegato = new DocsPaVO.InstanceAccess.Metadata.Allegato();
                DocsPaVO.documento.Allegato all = a as DocsPaVO.documento.Allegato;
                if (all != null && (from att in doc.ATTACHMENTS where att.ID_ATTACH.Equals(all.docNumber) select att.ENABLE).FirstOrDefault())
                {
                    allegato.Descrizione = all.descrizione;
                    allegato.ID = all.docNumber;
                    //allegato.Tipo = "manuale"; //Cablato , per ora.. poi si vedrà
                    string tipoAllegato = "";
                    switch (all.TypeAttachment)
                    {
                        case 1:
                            tipoAllegato = "Allegato Utente";
                            break;
                        case 2:
                            tipoAllegato = "Allegato PEC";
                            break;
                        case 3:
                            tipoAllegato = "Allegato IS";
                            break;
                        case 4:
                            tipoAllegato = "Allegato Esterno";
                            break;
                        default:
                            tipoAllegato = "Non specificato";
                            break;
                    }
                    allegato.Tipo = tipoAllegato;

                    if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                    {
                        FileDocumento fd = BusinessLogic.Documenti.FileManager.getFile(all, infoUtente);
                        allegato.File = getFileDetail(fd, all, infoUtente);
                    }
                    lstAll.Add(allegato);
                }
            }
            return  lstAll.ToArray();
        }

        public static DocsPaVO.InstanceAccess.Metadata.CampoTipologia[] getCampiTipologia(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            if (template == null)
                return null;

            List<DocsPaVO.InstanceAccess.Metadata.CampoTipologia> ctlist = new List<DocsPaVO.InstanceAccess.Metadata.CampoTipologia>();

            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
            {
                BusinessLogic.ExportDati.ExportDatiManager exportDatiManager = new BusinessLogic.ExportDati.ExportDatiManager();
                DocsPaVO.InstanceAccess.Metadata.CampoTipologia ct = new DocsPaVO.InstanceAccess.Metadata.CampoTipologia
                {
                    NomeCampo = oggettoCustom.DESCRIZIONE,
                    ValoreCampo = exportDatiManager.getValoreOggettoCustom(oggettoCustom)
                };
                ctlist.Add(ct);
            }
            return ctlist.ToArray();
        }


        private DocsPaVO.InstanceAccess.Metadata.File getFileDetail(DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string impronta;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.GetImpronta(out impronta, objFileRequest.versionId, objFileRequest.docNumber);

            string algo = "N.A";
            if (impronta == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDoc.content))
                algo = "SHA256";
            else if (impronta == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(fileDoc.content))
                algo = "SHA1";

             DocsPaVO.InstanceAccess.Metadata.File F = new DocsPaVO.InstanceAccess.Metadata.File
            {
                Impronta = impronta,
                Dimensione = objFileRequest.fileSize,
                Formato = fileDoc.contentType,
                AlgoritmoHash = algo,
                FirmaDigitale = extractFirmaDigitale(fileDoc),
                MarcaTemporale = extractMarcaTemporale(objFileRequest, infoUtente)
            };
             return F;
        }


        private DocsPaVO.InstanceAccess.Metadata.Registrazione getRegistrazione(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo)
        {
            if (schDoc.protocollo != null)
            {
                DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione = new DocsPaVO.InstanceAccess.Metadata.Registrazione();
                registrazione.DataProtocollo = formattaData(convertiData(schDoc.protocollo.dataProtocollazione));
                registrazione.OraProtocollo = formattaOra(convertiData(schDoc.protocollo.dataProtocollazione));
                registrazione.NumeroProtocollo = schDoc.protocollo.numero;
                registrazione.SegnaturaProtocollo = schDoc.protocollo.segnatura;
                registrazione.TipoProtocollo = schDoc.tipoProto;

                registrazione.CodiceRF = null;
                registrazione.DescrizioneRF = null;

                if (schDoc.datiEmergenza != null)
                    registrazione.SegnaturaEmergenza = schDoc.datiEmergenza.protocolloEmergenza;

                if (schDoc.registro != null)
                {
                    registrazione.CodiceAOO = schDoc.registro.codRegistro;
                    registrazione.DescrizioneAOO = schDoc.registro.descrizione;
                }

                EstraiDatiProtoEntrata(schDoc, registrazione);
                EstraiDatiProtoUscita(schDoc, registrazione);
                EstraiDatiProtoInterno(schDoc, registrazione);

                if (schDoc.protocollatore != null)
                {
                    DocsPaVO.utente.Utente userProt = BusinessLogic.Utenti.UserManager.getUtente(schDoc.protocollatore.utente_idPeople);
                    DocsPaVO.utente.Ruolo ruoloProt = BusinessLogic.Utenti.UserManager.getRuolo(schDoc.creatoreDocumento.idCorrGlob_Ruolo);
                    DocsPaVO.InstanceAccess.Metadata.Protocollista protocollista = new DocsPaVO.InstanceAccess.Metadata.Protocollista();

                    protocollista.DescrizioneUtente = userProt.descrizione;
                    protocollista.CodiceUtente = userProt.userId;
                    protocollista.DescrizioneRuolo = ruoloProt.descrizione;
                    protocollista.CodiceRuolo = ruoloProt.codiceRubrica;
                    protocollista.UOAppartenenza = ruolo.uo.codiceRubrica;
                    registrazione.Protocollista = protocollista;
                }
                return registrazione;
            }
            return null;
        }
        private DocsPaVO.InstanceAccess.Metadata.ContestoArchivistico getContestoArchivistico(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.InstanceAccess.Metadata.ContestoArchivistico retval = new DocsPaVO.InstanceAccess.Metadata.ContestoArchivistico();
            List<DocsPaVO.InstanceAccess.Metadata.Fascicolazione> fasList = new List<DocsPaVO.InstanceAccess.Metadata.Fascicolazione>();
            List<DocsPaVO.InstanceAccess.Metadata.Classificazione> titList = new List<DocsPaVO.InstanceAccess.Metadata.Classificazione>();
            object[] fasAList = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, schDoc.systemId).ToArray();
            foreach (object fo in fasAList )
            {
                DocsPaVO.fascicolazione.Fascicolo fas = fo as DocsPaVO.fascicolazione.Fascicolo;

                if (fas != null)
                {
                    if (fas.tipo == "P")
                    {
                        DocsPaVO.InstanceAccess.Metadata.Fascicolazione fascicolazione = new DocsPaVO.InstanceAccess.Metadata.Fascicolazione();

                        fascicolazione.DescrizioneFascicolo = fas.descrizione;
                        fascicolazione.CodiceFascicolo = fas.codice;

                        fascicolazione.CodiceSottofascicolo = null;
                        fascicolazione.DescrizioneSottofascicolo = null;

                        
                        fasList.Add(fascicolazione);
                        if (fas.idTitolario != null)
                        {
                            DocsPaVO.amministrazione.OrgNodoTitolario nodo = BusinessLogic.Amministrazione.TitolarioManager.getNodoTitolario(fas.idTitolario);
                            fascicolazione.TitolarioDiRierimento = nodo.Descrizione;
                        }

                  
                        foreach (DocsPaVO.fascicolazione.Folder f in BusinessLogic.Fascicoli.FolderManager.GetFoldersDocument(schDoc.systemId, fas.systemID) )
                        {
                            DocsPaVO.InstanceAccess.Metadata.Fascicolazione fasFolder = new DocsPaVO.InstanceAccess.Metadata.Fascicolazione();
                            fasFolder.CodiceFascicolo =fas.descrizione;
                            fasFolder.DescrizioneFascicolo = fas.codice;
                            fasFolder.CodiceSottofascicolo = f.systemID;
                            fasFolder.DescrizioneSottofascicolo = f.descrizione;
                            fasFolder.TitolarioDiRierimento = fascicolazione.TitolarioDiRierimento;
                            fasList.Add(fasFolder);
                        }

                    }
                    else
                    {
                        DocsPaVO.amministrazione.OrgNodoTitolario nodo = BusinessLogic.Amministrazione.TitolarioManager.getNodoTitolario(fas.idTitolario);
                        DocsPaVO.InstanceAccess.Metadata.Classificazione cl = new DocsPaVO.InstanceAccess.Metadata.Classificazione();
                        cl.TitolarioDiRiferimento = nodo.Descrizione;
                        cl.CodiceClassificazione = nodo.Codice;
                        titList.Add(cl);
                    }
                }
            }
   
           List<DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato> lstDocColl = new List<DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato>();
           if (schDoc.rispostaDocumento != null)
           {
               if ((schDoc.rispostaDocumento.docNumber != null) && (schDoc.rispostaDocumento.idProfile != null))
               {
                   SchedaDocumento sc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, schDoc.rispostaDocumento.idProfile, schDoc.rispostaDocumento.docNumber);
                   DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato docColl = new DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato
                   {
                       IDdocumento = schDoc.rispostaDocumento.idProfile,
                       DataCreazione = formattaData(convertiData(sc.dataCreazione)),
                       Oggetto = sc.oggetto.descrizione,


                   };
                   if (sc.protocollo != null)
                   {
                       docColl.DataProtocollo = formattaData(convertiData(sc.protocollo.dataProtocollazione));
                       docColl.NumeroProtocollo = sc.protocollo.numero;
                       docColl.SegnaturaProtocollo = sc.protocollo.segnatura;
                   }
                   lstDocColl.Add(docColl);
               }
           }

            

           retval.Fascicolazione = fasList.ToArray();
           retval.Classificazione = titList.ToArray();
           retval.DocumentoCollegato = lstDocColl.ToArray();
           return retval;
        }

        private static void EstraiDatiProtoEntrata(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloEntrata protEnt = schDoc.protocollo as DocsPaVO.documento.ProtocolloEntrata;
            if (protEnt != null)
            {
                registrazione.ProtocolloMittente = new DocsPaVO.InstanceAccess.Metadata.ProtocolloMittente
                {
                    Protocollo = protEnt.numero,
                    MezzoSpedizione = protEnt.mezzoSpedizione.ToString(),
                    Data = protEnt.dataProtocollazione
                };

                DocsPaVO.utente.Corrispondente corr = protEnt.mittente;
                List<DocsPaVO.InstanceAccess.Metadata.Mittente> mittList = new List<DocsPaVO.InstanceAccess.Metadata.Mittente>();

                if (protEnt.mittenti != null)
                {
                    foreach (object c in protEnt.mittenti)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            ProtocolloMittente = protEnt.numero,
                            DataProtocolloMittente = protEnt.dataProtocolloMittente
                        };
                        mittList.Add(m);
                    }
                }
                if (protEnt.mittenteIntermedio != null)
                {
                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = protEnt.mittenteIntermedio.codiceRubrica,
                        IndirizzoMail = protEnt.mittenteIntermedio.email,
                        Descrizione = protEnt.mittenteIntermedio.descrizione,
                        ProtocolloMittente = protEnt.numero,
                        DataProtocolloMittente = protEnt.dataProtocolloMittente
                    };
                    mittList.Add(m);
                }
                {
                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = corr.codiceRubrica,
                        IndirizzoMail = corr.email,
                        Descrizione = corr.descrizione,
                        ProtocolloMittente = protEnt.numero,
                        DataProtocolloMittente = protEnt.dataProtocolloMittente
                    };
                    mittList.Add(m);
                }
                registrazione.Mittente = mittList.ToArray();

            }

        }
        private static void EstraiDatiProtoUscita (DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloUscita protUsc = schDoc.protocollo as DocsPaVO.documento.ProtocolloUscita;
            if (protUsc != null)
            {
                List<DocsPaVO.InstanceAccess.Metadata.Destinatario> destList = new List<DocsPaVO.InstanceAccess.Metadata.Destinatario>();
                if (protUsc.destinatari != null)
                {
                    foreach (object c in protUsc.destinatari)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protUsc.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }
                if (protUsc.destinatariConoscenza != null)
                {
                    foreach (object c in protUsc.destinatariConoscenza)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protUsc.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }

                if (protUsc.mittente != null)
                {
                    List<DocsPaVO.InstanceAccess.Metadata.Mittente> mittList = new List<DocsPaVO.InstanceAccess.Metadata.Mittente>();

                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = protUsc.mittente.codiceRubrica,
                        IndirizzoMail = protUsc.mittente.email,
                        Descrizione = protUsc.mittente.descrizione,
                        ProtocolloMittente = null,
                        DataProtocolloMittente = null
                    };
                    mittList.Add(m);
                    registrazione.Mittente = mittList.ToArray();
                }
                registrazione.Destinatario = destList.ToArray();
            }
        }
        private static void EstraiDatiProtoInterno(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloInterno protInt = schDoc.protocollo as DocsPaVO.documento.ProtocolloInterno;
            if (protInt != null)
            {
                List<DocsPaVO.InstanceAccess.Metadata.Destinatario> destList = new List<DocsPaVO.InstanceAccess.Metadata.Destinatario>();
                if (protInt.destinatari != null)
                {
                    foreach (object c in protInt.destinatari)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protInt.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }
                if (protInt.destinatariConoscenza != null)
                {
                    foreach (object c in protInt.destinatariConoscenza)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protInt.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }

                if (protInt.mittente != null)
                {
                    List<DocsPaVO.InstanceAccess.Metadata.Mittente> mittList = new List<DocsPaVO.InstanceAccess.Metadata.Mittente>();

                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = protInt.mittente.codiceRubrica,
                        IndirizzoMail = protInt.mittente.email,
                        Descrizione = protInt.mittente.descrizione,
                        ProtocolloMittente = null,
                        DataProtocolloMittente = null
                    };
                    mittList.Add(m);
                    registrazione.Mittente = mittList.ToArray();
                }
                registrazione.Destinatario = destList.ToArray();
            }
        }
        private static DocsPaVO.InstanceAccess.Metadata.MarcaTemporale extractMarcaTemporale(DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.InstanceAccess.Metadata.MarcaTemporale mt = null;
            var ts = BusinessLogic.Documenti.TimestampManager.getTimestampsDoc (infoUtente,objFileRequest);
            if (ts.Count > 0)
            {
                mt = new DocsPaVO.InstanceAccess.Metadata.MarcaTemporale();
                DocsPaVO.documento.TimestampDoc timestampDoc = ts[0] as DocsPaVO.documento.TimestampDoc;
                if (timestampDoc != null)
                {
                    mt.NumeroSerie = timestampDoc.NUM_SERIE;
                    mt.SNCertificato = timestampDoc.S_N_CERTIFICATO;
                    mt.ImprontaDocumentoAssociato = timestampDoc.TSR_FILE;
                    mt.DataInizioValidita = formattaData(convertiData(timestampDoc.DTA_CREAZIONE));
                    mt.DataFineValidita = formattaData(convertiData(timestampDoc.DTA_SCADENZA));
                    mt.TimeStampingAuthority = timestampDoc.SOGGETTO;
                    mt.Data = formattaData(convertiData(timestampDoc.DTA_CREAZIONE)); ;
                    mt.Ora = formattaOraScondi(convertiData(timestampDoc.DTA_CREAZIONE)); 
                }
            }
          
            return mt;
        }
        private static DocsPaVO.InstanceAccess.Metadata.FirmaDigitale extractFirmaDigitale(DocsPaVO.documento.FileDocumento fileDoc)
        {
       
            BusinessLogic.Documenti.DigitalSignature.VerifySignature verifySignature = new BusinessLogic.Documenti.DigitalSignature.VerifySignature();
            string inputDirectory = verifySignature.GetPKCS7InputDirectory();
            // Creazione cartella di appoggio nel caso non esista
            if (!System.IO.Directory.Exists(inputDirectory))
                System.IO.Directory.CreateDirectory(inputDirectory);

            string inputFile = string.Concat(inputDirectory, fileDoc.name);

            // Copia del file firmato dalla cartella del documentale
            // alla cartella di input utilizzata dal ws della verifica
            CopySignedFileToInputFolder(fileDoc, inputFile);

            fileDoc.signatureResult = verifySignature.Verify(fileDoc.name);

            try
            {
                // Rimozione del file firmato dalla cartella di input
                System.IO.File.Delete(inputFile);
            }
            catch
            {
            }

            if (fileDoc.signatureResult == null)
                return null;


            DocsPaVO.InstanceAccess.Metadata.FirmaDigitale fd = null;

            if (fileDoc.signatureResult.PKCS7Documents != null)
            {
                fd = new DocsPaVO.InstanceAccess.Metadata.FirmaDigitale();
                foreach (PKCS7Document p7m in fileDoc.signatureResult.PKCS7Documents)
                {
                    if (p7m.SignersInfo != null)
                    {
                        DocsPaVO.documento.SignerInfo si = p7m.SignersInfo.FirstOrDefault();
                        CnipaParser cp = new CnipaParser();
                        cp.ParseCNIPASubjectInfo(ref si.SubjectInfo, si.CertificateInfo.SubjectName); 
                        fd.Certificato = String.Format("{0} {1} {2}", "<![CDATA[", SerializeObject<CertificateInfo>(si.CertificateInfo), "]]>"); 
                        fd.DatiFirma = String.Format("{0} {1} {2}", "<![CDATA[", SerializeObject<SubjectInfo>(si.SubjectInfo), "]]>"); 
                        
                        fd.Titolare = new DocsPaVO.InstanceAccess.Metadata.Titolare
                        {
                            CodiceFiscale = si.SubjectInfo.CodiceFiscale,
                            Nome = si.SubjectInfo.Nome,
                            Cognome = si.SubjectInfo.Cognome
                            
                        };
                    }
                }
            }
            return fd;
        }

        public static DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa convertiUO(DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa)
        {
            DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa uoXML = new DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa();
            uoXML.CodiceUO = unitaOrganizzativa.codiceRubrica;
            uoXML.DescrizioneUO = unitaOrganizzativa.descrizione;
            uoXML.Livello = unitaOrganizzativa.livello;

            if (unitaOrganizzativa.parent == null)
                return uoXML;

            DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa uoXML_Padre = convertiUO(unitaOrganizzativa.parent);
            List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa> uoXMLLst = new List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa>();
            uoXMLLst.Add(uoXML_Padre);
            uoXML.UnitaOrganizzativa1 = uoXMLLst.ToArray();

            return uoXML;
        }

        public static DocsPaVO.InstanceAccess.Metadata.Amministrazione getInfoAmministrazione(string idAmm)
        {
            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
            return new DocsPaVO.InstanceAccess.Metadata.Amministrazione { CodiceAmministrazione = infoAmm.Codice, DescrizioneAmministrazione = infoAmm.Descrizione };
        }

        public static DocsPaVO.InstanceAccess.Metadata.Creatore getCreatore(SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.InstanceAccess.Metadata.Creatore creatore = new DocsPaVO.InstanceAccess.Metadata.Creatore();
            creatore.CodiceRuolo = ruolo.codiceRubrica;
            creatore.DescrizioneRuolo = ruolo.descrizione;
            creatore.CodiceUtente = schDoc.userId;
            DocsPaVO.utente.Utente user = BusinessLogic.Utenti.UserManager.getUtente(schDoc.creatoreDocumento.idPeople);
            if(user != null)
                creatore.DescrizioneUtente = user.descrizione;
            return creatore;
        }

        public static string convertiTipoPoto(DocsPaVO.documento.SchedaDocumento schDoc)
        {
            string retval = schDoc.tipoProto;
            switch (schDoc.tipoProto)
            {

                case "A":
                case "P":
                case "I":
                    {
                        retval = "Protocollato";
                        if (schDoc.protocollo != null)
                            if (string.IsNullOrEmpty(schDoc.protocollo.segnatura))
                                retval = "Predisposto";
                    }
                    break;

                case "G":
                    retval = "Grigio";
                    break;
            }
            return retval;
        }

        private static void CopySignedFileToInputFolder(DocsPaVO.documento.FileDocumento fileDoc, string inputFile)
        {
            System.IO.FileStream stream = new System.IO.FileStream(inputFile, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write);
            stream.Write(fileDoc.content, 0, fileDoc.content.Length);
            stream.Flush();
            stream.Close();
            stream = null;
        }

        private static String formattaData(DateTime data)
        {
            return String.Format("{0}-{1}-{2}", data.Year, data.Month.ToString().PadLeft(2, '0'), data.Day.ToString().PadLeft(2, '0'));
        }

        public static String formattaOra(DateTime data)
        {
            return String.Format("{0}:{1}", data.Hour.ToString().PadLeft(2, '0'), data.Minute.ToString().PadLeft(2, '0'));
        }
        private static DateTime convertiData(string data)
        {
            DateTime dt = DateTime.Now;
            DateTime.TryParse(data, out dt);
            return dt;
        }

        private static String formattaOraScondi(DateTime data)
        {
            return String.Format("{0}:{1}:{2}", data.Hour.ToString().PadLeft(2, '0'), data.Minute.ToString().PadLeft(2, '0'), data.Second.ToString().PadLeft(2, '0'));
        }
        public static String SerializeObject<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(xmlTextWriter, pObject, ns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                memoryStream.Position = 0;
                //XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                StreamReader sr = new StreamReader(memoryStream);
                XmlizedString = sr.ReadToEnd();
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }
    }

    public class CnipaParser
    {

        private string GetSubjectItem(Hashtable subjectItems, string itemKey)
        {
            if (subjectItems.ContainsKey(itemKey))
                return subjectItems[itemKey].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Estrazione dei singoli elementi che compongono
        /// la descrizione del firmatario del certificato
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        private Hashtable ParseCNIPASubjectItems(string subject)
        {
            Hashtable retValue = new Hashtable();

            string[] items = subject.Split(',');
            Regex regex = new Regex("[A-Za-z]*=[^\"][^,]*");
            MatchCollection matchColl = regex.Matches(subject);
            for (int i = 0; i < matchColl.Count; i++)
            {
                string item = matchColl[i].Value;
                int indexOfValue = item.IndexOf("=");
                string itemKey = item.Substring(0, indexOfValue).ToUpper().Trim();
                string itemValue = item.Substring(indexOfValue + 1).ToUpper().Trim();
                retValue[itemKey] = itemValue;
            }
            Regex regex2 = new Regex("[A-Za-z]*=\"[\\d\\D]*\"");
            MatchCollection matchColl2 = regex2.Matches(subject);
            for (int i = 0; i < matchColl2.Count; i++)
            {
                string item = matchColl2[i].Value;
                int indexOfValue = item.IndexOf("=");
                string itemKey = item.Substring(0, indexOfValue).ToUpper().Trim();
                string itemValue = item.Substring(indexOfValue + 1).ToUpper().Trim();
                retValue[itemKey] = itemValue;
            }

            return retValue;
        }

        /// <summary>
        /// Parse Certificate Description
        /// </summary>
        /// <param name="sd">a structure filled with results</param>
        /// <param name="subject">the input string</param>
        public void ParseCNIPASubjectInfo(ref DocsPaVO.documento.SubjectInfo subjectInfo, string subject)
        {
            Regex r = new Regex(".*?CN=(?<cognome>.*?)/(?<nome>.*?)/(?<cf>.*?)/(?<cid>.*?),.*", RegexOptions.IgnoreCase);
            Match match = r.Match(subject);

            if (match.Success)
            {
                // Formato supportato dalla normativa 2000
                string commonName = match.Groups["cognome"].Value + " " + match.Groups["nome"].Value;
                if (commonName.Trim() != string.Empty)
                    subjectInfo.CommonName = commonName;

                subjectInfo.CodiceFiscale = match.Groups["cf"].Value;
                subjectInfo.CertId = match.Groups["cid"].Value;
            }

            r = new Regex(@"Description=\""C=(?<cognome>.*?)/N=(?<nome>.*?)/D=(?<g>\d+)-(?<m>\d+)-(?<a>\d+)(/R=(?<r>.*?))?\""(.*C=(?<country>.*?)$)?", RegexOptions.IgnoreCase);
            match = r.Match(subject);

            if (match.Success)
            {
                // Formato supportato dalla normativa 2000
                subjectInfo.Cognome = match.Groups["cognome"].Value;
                subjectInfo.Nome = match.Groups["nome"].Value;
                subjectInfo.DataDiNascita =
                    new DateTime(Int32.Parse(match.Groups["a"].Value),
                                 Int32.Parse(match.Groups["m"].Value),
                                 Int32.Parse(match.Groups["g"].Value)).ToShortDateString();

                subjectInfo.Ruolo = match.Groups["r"].Value;
                subjectInfo.Country = match.Groups["country"].Value;
            }
            else
            {
                // Formato supportato dalla normativa del 17/02/2005
                Hashtable subjectItems = this.ParseCNIPASubjectItems(subject);

                subjectInfo.CommonName = this.GetSubjectItem(subjectItems, "CN");
                subjectInfo.Cognome = this.GetSubjectItem(subjectItems, "SN");
                subjectInfo.Nome = this.GetSubjectItem(subjectItems, "G");
                subjectInfo.CertId = this.GetSubjectItem(subjectItems, "DNQUALIFIER");
                subjectInfo.SerialNumber = this.GetSubjectItem(subjectItems, "OID.2.5.4.5");
                
                // Se il country code è "IT", il "SerialNumber" contiene il codice fiscale del titolare del certificato
                if (subjectInfo.SerialNumber.StartsWith("IT:"))
                    subjectInfo.CodiceFiscale = subjectInfo.SerialNumber.Substring(subjectInfo.SerialNumber.IndexOf(":") + 1);
                subjectInfo.Country = this.GetSubjectItem(subjectItems, "C");

                if (subjectInfo.CodiceFiscale == null)
                    subjectInfo.CodiceFiscale = this.GetSubjectItem(subjectItems, "SERIALNUMBER");

                subjectItems.Clear();
                subjectItems = null;
            }
        }
    }
}
