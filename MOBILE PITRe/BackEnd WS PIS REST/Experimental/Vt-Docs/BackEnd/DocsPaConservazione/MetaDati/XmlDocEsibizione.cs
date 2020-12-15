using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaConservazione.Metadata.Common;
using DocsPaConservazione.Metadata.Documento;
using DocsPaVO.areaConservazione;
using DocsPaVO.amministrazione;
using BusinessLogic.Utenti;
using DocsPaVO.documento;
using BusinessLogic.ExportDati;
using BusinessLogic.Documenti.DigitalSignature;


namespace DocsPaConservazione.Metadata
{
    public class XmlDocEsibizione
    {

        DocsPaConservazione.Metadata.Documento.Documento documento;
        public string XmlFile
        {
            get
            {
                return Utils.SerializeObject<DocsPaConservazione.Metadata.Documento.Documento>(documento, true);
            }
        }

        public DocsPaConservazione.Metadata.Documento.Documento Documento
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

        public XmlDocEsibizione(InfoEsibizione infoEs, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.documento.FileRequest objFileRequest)
        {
            if (this.documento == null)
                documento = new Documento.Documento();

            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoEs.IdRuoloInUo);
            DocsPaVO.utente.InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(UserManager.getUtente(infoEs.IdPeople), ruolo);
            DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa = ruolo.uo;

            List<UnitaOrganizzativa> uoL = new List<UnitaOrganizzativa>();
            UnitaOrganizzativa uoXML = Utils.convertiUO(unitaOrganizzativa);
            uoL.Add(uoXML);

            InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoEs.IdAmm);
            
            documento.SoggettoProduttore = new SoggettoProduttore
            {
                Amministrazione = new Metadata.Common.Amministrazione { CodiceAmministrazione = infoAmm.Codice, DescrizioneAmministrazione = infoAmm.Descrizione },
                GerarchiaUO = new GerarchiaUO { UnitaOrganizzativa = uoL.ToArray() },
                Creatore = new Creatore() {
                    CodiceRuolo = ruolo.codiceRubrica,
                    DescrizioneRuolo = ruolo.descrizione,
                    CodiceUtente = UserManager.getUtente(infoEs.IdPeople).userId,
                    DescrizioneUtente = UserManager.getUtente(infoEs.IdPeople).descrizione}
            };

            documento.IDdocumento = schDoc.systemId;
            documento.Oggetto = schDoc.oggetto.descrizione;
            documento.Tipo = Utils.convertiTipoPoto(schDoc);
            //  documento.DataCreazione = Utils.formattaData(Utils.convertiData(schDoc.dataCreazione));
            documento.DataCreazione = schDoc.dataCreazione;

            if (schDoc.privato != null && schDoc.privato.Equals("1"))
                documento.LivelloRiservatezza = "privato";
            else
                documento.LivelloRiservatezza = string.Empty;

            documento.File = getFileDetail(fileDoc, objFileRequest,infoUtente);
            documento.Registrazione = getRegistrazione(infoEs, schDoc, ruolo);
            documento.ContestoArchivistico = getContestoArchivistico(infoEs, schDoc, ruolo, infoUtente);

            if (schDoc.template != null)
            {
                Tipologia t = new Tipologia { NomeTipologia = schDoc.template.DESCRIZIONE, CampoTipologia = Utils.getCampiTipologia(schDoc.template) };
                documento.Tipologia = t;
            }
            documento.Allegati = getAllegati(schDoc, infoUtente);

        }

        private File getFileDetail(DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string impronta;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.GetImpronta(out impronta, objFileRequest.versionId, objFileRequest.docNumber);

            string algo = "N.A";
            if (impronta == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDoc.content))
                algo = "SHA256";
            else if (impronta == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(fileDoc.content))
                algo = "SHA1";

            File F = new File
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

        private Registrazione getRegistrazione(InfoEsibizione infoEs, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo)
        {
            if (schDoc.protocollo != null)
            {
                Registrazione registrazione = new Registrazione();
                registrazione.DataProtocollo = Utils.formattaData(Utils.convertiData(schDoc.protocollo.dataProtocollazione));
                registrazione.OraProtocollo = Utils.formattaOra(Utils.convertiData(schDoc.protocollo.dataProtocollazione));
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
                    DocsPaVO.utente.Utente userProt = UserManager.getUtente(schDoc.protocollatore.utente_idPeople);
                    DocsPaVO.utente.Ruolo ruoloProt = BusinessLogic.Utenti.UserManager.getRuolo(infoEs.IdRuoloInUo);
                    Protocollista protocollista = new Protocollista();

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

        private ContestoArchivistico getContestoArchivistico(InfoEsibizione infoEs, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            ContestoArchivistico retval = new ContestoArchivistico();
            List<Fascicolazione> fasList = new List<Fascicolazione>();
            List<Classificazione> titList = new List<Classificazione>();
            object[] fasAList = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, schDoc.systemId).ToArray();
            foreach (object fo in fasAList)
            {
                DocsPaVO.fascicolazione.Fascicolo fas = fo as DocsPaVO.fascicolazione.Fascicolo;

                if (fas != null)
                {
                    if (fas.tipo == "P")
                    {
                        Fascicolazione fascicolazione = new Fascicolazione();

                        fascicolazione.DescrizioneFascicolo = fas.descrizione;
                        fascicolazione.CodiceFascicolo = fas.codice;

                        fascicolazione.CodiceSottofascicolo = null;
                        fascicolazione.DescrizioneSottofascicolo = null;


                        fasList.Add(fascicolazione);
                        if (fas.idTitolario != null)
                        {
                            OrgNodoTitolario nodo = BusinessLogic.Amministrazione.TitolarioManager.getNodoTitolario(fas.idTitolario);
                            fascicolazione.TitolarioDiRierimento = nodo.Descrizione;
                        }


                        foreach (DocsPaVO.fascicolazione.Folder f in BusinessLogic.Fascicoli.FolderManager.GetFoldersDocument(schDoc.systemId, fas.systemID))
                        {
                            Fascicolazione fasFolder = new Fascicolazione();
                            fasFolder.CodiceFascicolo = fas.descrizione;
                            fasFolder.DescrizioneFascicolo = fas.codice;
                            fasFolder.CodiceSottofascicolo = f.systemID;
                            fasFolder.DescrizioneSottofascicolo = f.descrizione;
                            fasFolder.TitolarioDiRierimento = fascicolazione.TitolarioDiRierimento;
                            fasList.Add(fasFolder);
                        }

                    }
                    else
                    {
                        OrgNodoTitolario nodo = BusinessLogic.Amministrazione.TitolarioManager.getNodoTitolario(fas.idTitolario);
                        Classificazione cl = new Classificazione();
                        cl.TitolarioDiRiferimento = nodo.Descrizione;
                        cl.CodiceClassificazione = nodo.Codice;
                        titList.Add(cl);
                    }


                }
            }



            List<DocumentoCollegato> lstDocColl = new List<DocumentoCollegato>();
            if (schDoc.rispostaDocumento != null)
            {
                if ((schDoc.rispostaDocumento.docNumber != null) && (schDoc.rispostaDocumento.idProfile != null))
                {
                    SchedaDocumento sc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, schDoc.rispostaDocumento.idProfile, schDoc.rispostaDocumento.docNumber);
                    DocumentoCollegato docColl = new DocumentoCollegato
                    {
                        IDdocumento = schDoc.rispostaDocumento.idProfile,
                        DataCreazione = Utils.formattaData(Utils.convertiData(sc.dataCreazione)),
                        Oggetto = sc.oggetto.descrizione,


                    };
                    if (sc.protocollo != null)
                    {
                        docColl.DataProtocollo = Utils.formattaData(Utils.convertiData(sc.protocollo.dataProtocollazione));
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

        private Metadata.Documento.Allegato[] getAllegati(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (schDoc.allegati == null)
                return null;
            if (schDoc.allegati.Count == 0)
                return null;

            List<Metadata.Documento.Allegato> lstAll = new List<Documento.Allegato>();
            foreach (object a in schDoc.allegati)
            {
                DocsPaConservazione.Metadata.Documento.Allegato allegato = new Metadata.Documento.Allegato();
                DocsPaVO.documento.Allegato all = a as DocsPaVO.documento.Allegato;
                if (all != null)
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

                    FileDocumento fd = BusinessLogic.Documenti.FileManager.getFile(all, infoUtente);
                    allegato.File = getFileDetail(fd, all, infoUtente);
                    lstAll.Add(allegato);
                }
            }
            return lstAll.ToArray();
        }

        private static void EstraiDatiProtoEntrata(DocsPaVO.documento.SchedaDocumento schDoc, Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloEntrata protEnt = schDoc.protocollo as DocsPaVO.documento.ProtocolloEntrata;
            if (protEnt != null)
            {
                registrazione.ProtocolloMittente = new ProtocolloMittente
                {
                    Protocollo = protEnt.numero,
                    MezzoSpedizione = protEnt.mezzoSpedizione.ToString(),
                    Data = protEnt.dataProtocollazione
                };

                DocsPaVO.utente.Corrispondente corr = protEnt.mittente;
                List<Mittente> mittList = new List<Mittente>();

                if (protEnt.mittenti != null)
                {
                    foreach (object c in protEnt.mittenti)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        Mittente m = new Mittente
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
                    Mittente m = new Mittente
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
                    Mittente m = new Mittente
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
        private static void EstraiDatiProtoUscita(DocsPaVO.documento.SchedaDocumento schDoc, Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloUscita protUsc = schDoc.protocollo as DocsPaVO.documento.ProtocolloUscita;
            if (protUsc != null)
            {
                List<Destinatario> destList = new List<Destinatario>();
                if (protUsc.destinatari != null)
                {
                    foreach (object c in protUsc.destinatari)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        Destinatario d = new Destinatario
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
                        Destinatario d = new Destinatario
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
                    List<Mittente> mittList = new List<Mittente>();

                    Mittente m = new Mittente
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
        private static void EstraiDatiProtoInterno(DocsPaVO.documento.SchedaDocumento schDoc, Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloInterno protInt = schDoc.protocollo as DocsPaVO.documento.ProtocolloInterno;
            if (protInt != null)
            {
                List<Destinatario> destList = new List<Destinatario>();
                if (protInt.destinatari != null)
                {
                    foreach (object c in protInt.destinatari)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        Destinatario d = new Destinatario
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
                        Destinatario d = new Destinatario
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
                    List<Mittente> mittList = new List<Mittente>();

                    Mittente m = new Mittente
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
        private static MarcaTemporale extractMarcaTemporale(DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            MarcaTemporale mt = null;
            var ts = BusinessLogic.Documenti.TimestampManager.getTimestampsDoc(infoUtente, objFileRequest);
            if (ts.Count > 0)
            {
                mt = new MarcaTemporale();
                DocsPaVO.documento.TimestampDoc timestampDoc = ts[0] as DocsPaVO.documento.TimestampDoc;
                if (timestampDoc != null)
                {
                    mt.NumeroSerie = timestampDoc.NUM_SERIE;
                    mt.SNCertificato = timestampDoc.S_N_CERTIFICATO;
                    mt.ImprontaDocumentoAssociato = timestampDoc.TSR_FILE;
                    mt.DataInizioValidita = Utils.formattaData(Utils.convertiData(timestampDoc.DTA_CREAZIONE));
                    mt.DataFineValidita = Utils.formattaData(Utils.convertiData(timestampDoc.DTA_SCADENZA));
                    mt.TimeStampingAuthority = timestampDoc.SOGGETTO;
                    mt.Data = Utils.formattaData(Utils.convertiData(timestampDoc.DTA_CREAZIONE)); ;
                    mt.Ora = Utils.formattaOraScondi(Utils.convertiData(timestampDoc.DTA_CREAZIONE));
                }
            }

            return mt;
        }
        private static FirmaDigitale extractFirmaDigitale(DocsPaVO.documento.FileDocumento fileDoc)
        {

            VerifySignature verifySignature = new VerifySignature();
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


            FirmaDigitale fd = null;

            if (fileDoc.signatureResult.PKCS7Documents != null)
            {
                fd = new FirmaDigitale();
                foreach (PKCS7Document p7m in fileDoc.signatureResult.PKCS7Documents)
                {
                    if (p7m.SignersInfo != null)
                    {
                        DocsPaVO.documento.SignerInfo si = p7m.SignersInfo.FirstOrDefault();
                        CnipaParser cp = new CnipaParser();
                        cp.ParseCNIPASubjectInfo(ref si.SubjectInfo, si.CertificateInfo.SubjectName);
                        fd.Certificato = String.Format("{0} {1} {2}", "<![CDATA[", Utils.SerializeObject<CertificateInfo>(si.CertificateInfo, true), "]]>");
                        fd.DatiFirma = String.Format("{0} {1} {2}", "<![CDATA[", Utils.SerializeObject<SubjectInfo>(si.SubjectInfo, true), "]]>");

                        fd.Titolare = new Titolare
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
        private static void CopySignedFileToInputFolder(DocsPaVO.documento.FileDocumento fileDoc, string inputFile)
        {
            System.IO.FileStream stream = new System.IO.FileStream(inputFile, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write);
            stream.Write(fileDoc.content, 0, fileDoc.content.Length);
            stream.Flush();
            stream.Close();
            stream = null;
        }


    }
}
