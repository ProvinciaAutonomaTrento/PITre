using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using DocsPaVO.documento;
using log4net;
using System.Xml;
using System.IO;

namespace BusinessLogic.XmlParsing.suap
{
    public class SuapManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SuapManager));


        
        string _nomeTemplate;
        public string NomeTemplate
        {
            get { return _nomeTemplate; }
            set { _nomeTemplate = value; }
        }

        
        public SuapManager(string TipologiaName)
        {
            _nomeTemplate = TipologiaName;
        }


        private string getOriginalFileNameOfDocPrincipale(InfoUtente infoUtente, SchedaDocumento schedaDocumento)
        {
            string originalName = null;
            FileRequest doc = schedaDocumento.documenti[0] as FileRequest;
            if (doc != null)
                originalName = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, doc);

            return originalName;
        }


        public string ExportEnteSuapXML(InfoUtente infoUtente, SchedaDocumento schedaDocumento, string fromPecEmail)
        {
            String[] TipologiaENTESUAP = new String[] { "Codice pratica",                 // 0
                                                        "Oggetto comunicazione",            // 1
            };

            if (String.IsNullOrEmpty(fromPecEmail))
            {
                logger.Error("Manca La pec dell'ente mittente per la valorizzazione di entesuap");
                return null;
            }

            string pecSuap =fromPecEmail; // TO//RICAVARE          suap@pec.comune.trento.it
            string enteMittsuap = "";
            if (schedaDocumento.registro!=null)
                if (!String.IsNullOrEmpty ( schedaDocumento.registro.codAmministrazione ))
                    enteMittsuap = schedaDocumento.registro.codAmministrazione;
            
            string attributoOggcomunicazione = getValoreOggettoGenerico(schedaDocumento.template, TipologiaENTESUAP[1]);
            string valueOggComunicazione = schedaDocumento.oggetto.descrizione; // ""; //COSTRUITA SULLA BASE DI UNA TABELLA DI TESTI PER "TIPO-COOPERAZIONE" + "in merito a pratica " + CODICE-PRATICA + " - " + SUAP " +identificativo-suap + " " + nome_impresa  //VAR_PROF_OGGETTO
            string testoComunicazione = null;

            
            string nomeFilePrinc = getOriginalFileNameOfDocPrincipale(infoUtente, schedaDocumento);
            if (attributoOggcomunicazione.ToUpper().Trim() == "ALTRO")
                testoComunicazione = String.Format("Si trasmette comunicazione allegata ({0})", nomeFilePrinc);
            else
                testoComunicazione = String.Format("Si trasmette comunicazione {0} allegata ({1})", attributoOggcomunicazione.Replace ("-"," ") , nomeFilePrinc);
            

            //testoComunicazione = "-"; //  DA COSTRUIRE IN BASE A REGOLE DEL DPR 160/2010  - > agostinelli

            /*
            string suapCompValue = "-";//             Suap di TRENTO in delega alla CCIAA di TN   
            string codAmmSuap = "-";//                CCIAA TN
            string codAOOSuap = "-";//                TN-SUPRO
            */
            
            //Dalla chiave BE_IDENTIFICATIVO_SUAP
            string idSuap = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_IDENTIFICATIVO_SUAP");
            //se manca l'idsuap nelle chiavi di configurazione allora non posso continuare ed esco
            if (String.IsNullOrEmpty(idSuap))
            {
                logger.Error("Manca La chiave BE_IDENTIFICATIVO_SUAP per la valorizzazione di entesuap");
                return null;
            }

            if (idSuap.Equals("0"))
            {
                logger.Error("La chiave BE_IDENTIFICATIVO_SUAP per la valorizzazione di entesuap  è posta a 0 quindi non valida");
                return null;
            }

            string codicePratica = getValoreOggettoGenerico(schedaDocumento.template, TipologiaENTESUAP[0]);
            //se manca il codice pratica non genero l'xml ed esco
            if (string.IsNullOrEmpty(codicePratica))
            {
                logger.Error("Manca il codice pratica per la creazione di entesuap.xml");
                return null;
            }

            SUAPEnte.CooperazioneEnteSUAP enteSuap = new SUAPEnte.CooperazioneEnteSUAP
            {
                infoschema = new SUAPEnte.CooperazioneEnteSUAPInfoschema { data = DateTime.Now, versione = "1.0.0" }
            };

            if (schedaDocumento.template ==null)
                return string.Empty;

            if (schedaDocumento.template.DESCRIZIONE!= _nomeTemplate)
                return string.Empty;

            List<object> itemscittaStraniera = new List<object>();
            itemscittaStraniera.Add("-");
            enteSuap.intestazione = new SUAPEnte.CooperazioneEnteSUAPIntestazione
            {
                codicepratica = codicePratica,
                oggettocomunicazione = new SUAPEnte.OggettoCooperazione { tipocooperazione = attributoOggcomunicazione, Value = valueOggComunicazione },
                testocomunicazione = testoComunicazione,
                entemittente = new SUAPEnte.EstremiEnte
                {
                    pec = pecSuap,
                    Value = enteMittsuap
                },
                suapcompetente = new SUAPEnte.EstremiSuap
                {
                    identificativosuap = idSuap,

                    codiceamministrazione = "-",
                    codiceaoo = "-",

                    //Value = suapCompValue
                },

                impresa = new SUAPEnte.AnagraficaImpresa
                {
                    formagiuridica = new SUAPEnte.FormaGiuridica { codice = SUAPEnte.FormaGiuridicaCodice.AA, Value = "-" },
                    ragionesociale = "-",
                    legalerappresentante = new SUAPEnte.AnagraficaRappresentante1
                    {
                        cognome = "-",
                        nome = "-",
                        codicefiscale = "AAAAAA00A00A000A",
                        carica = new SUAPEnte.Carica1 { codice = SUAPEnte.CaricaCodice.ACP, Value = "-" }
                        
                    },
                    indirizzo = new SUAPEnte.IndirizzoConRecapiti
                    {
                        stato = new SUAPEnte.Stato { codice = "0", Value = "-" },
                        denominazionestradale = "-",
                        numerocivico = "-",
                        Items = itemscittaStraniera,
                    }

                },
                oggettopratica = new SUAPEnte.OggettoComunicazione { Value = "-" },
                protocollopraticasuap = new SUAPEnte.ProtocolloSUAP
                {
                    numeroregistrazione = "0000000",
                    dataregistrazione = DateTime.MinValue,
                    codiceaoo = "-",
                    codiceamministrazione = "-"
                }
  
            };
            
            if (schedaDocumento.protocollo!=null)
            {
                string codAMM = schedaDocumento.registro.codAmministrazione;
                string codAOO = schedaDocumento.registro.codRegistro;
                DateTime dataProt =  DocsPaUtils.Functions.Functions.ToDate(schedaDocumento.protocollo.dataProtocollazione);
                string protoSchDoc =  schedaDocumento.protocollo.numero.PadLeft(7,'0') ;
                SUAPEnte.ProtocolloSUAP protocollo = new SUAPEnte.ProtocolloSUAP { codiceamministrazione = codAMM, codiceaoo = codAOO, dataregistrazione = dataProt, numeroregistrazione = protoSchDoc };
                enteSuap.intestazione.protocollo = protocollo;
            }

            //gestione allegati
            List<SUAPEnte.AllegatoCooperazione> allegati = new List<SUAPEnte.AllegatoCooperazione>();
           
            {
                if (schedaDocumento.documenti != null && schedaDocumento.documenti.Count > 0)
                {
                    FileRequest doc = schedaDocumento.documenti[0] as FileRequest;
                    if (doc != null)
                    {
                        string descr = "---";
                        if (!String.IsNullOrEmpty(doc.descrizione))
                            descr = doc.descrizione;

                        string originalName = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, doc);
                        if ((originalName !=null) && (!originalName.ToLowerInvariant().Equals("entesuap.xml")))
                        {
                            string contentType = BusinessLogic.Documenti.FileManager.getContentType(originalName);
                            allegati.Add(new SUAPEnte.AllegatoCooperazione { cod = "ALLEG", descrizione = descr, dimensione = doc.fileSize.ToString(), mime = contentType, nomefile = originalName, nomefileoriginale = originalName });
                        }
                    }
                }
            }


            foreach (Allegato all in schedaDocumento.allegati)
            {
                string originalName = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, all);
                if (originalName != null)
                {
                    //solo allegati utente
                    if (all.TypeAttachment != 1)
                        continue;

                    if (originalName.ToLowerInvariant().Equals("entesuap.xml"))
                        continue;

                    string contentType = BusinessLogic.Documenti.FileManager.getContentType(originalName);

                    allegati.Add(new SUAPEnte.AllegatoCooperazione { cod = "ALLEG", descrizione = all.descrizione, dimensione = all.fileSize.ToString(), mime = contentType, nomefile = originalName, nomefileoriginale = originalName });
                }
            }

            //aggiungo gli allegati
            enteSuap.allegato = allegati;

            string retval = enteSuap.Serialize();
            retval = removeNodiInutilizzati(retval);
            return BusinessLogic.XmlParsing.XmlParserManager.BeautifyXml(retval);
        }

 
        public bool ImportSuapEnteXMLIntoTemplate(InfoUtente infoUtente, Ruolo ruoloUtente, SchedaDocumento schedaDoc, byte[]  xmlcontent)
        {
            string xml = System.Text.UTF8Encoding.UTF8.GetString(xmlcontent);
            string xsdFile = AppDomain.CurrentDomain.BaseDirectory + "XML/pratica_suap-1.0.1.xsd";

            if (File.Exists(xsdFile))
            {
                BusinessLogic.report.ProtoASL.ReportXML.XmlValidator xmlV  = new report.ProtoASL.ReportXML.XmlValidator ();
                bool esito = xmlV.ValidateXmlString (xml,xsdFile,null);
                if (!esito)
                {
                    logger.DebugFormat("Validazione *.SUAP.XML fallita: {0}", xmlV.ValidationErrors);
                    return false;
                }
            }

            String[] TipologiaSUAPENTE = new String[] { "Procedimento",                 // 0
                                                        "Tipo procedimento",            // 1
                                                        "Tipo intervento",              // 2
                                                        "Codice pratica",               // 3
                                                        "Impresa",                      // 4
                                                        "Codice REA",                   // 5
                                                        "Legale Rappresentante",        // 6
                                                        "Dichiarante",                  // 7
                                                        "Domicilio elettronico",        // 8
                                                        "Impianto produttivo",          // 9
                                                        "Procura speciale",             //10
                                                        "Allegati"};                    //11
            //Reperire una tipologia di tipo "SUAP"
            DocsPaVO.ProfilazioneDinamica.Templates templatesuap = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(_nomeTemplate, infoUtente.idAmministrazione);
            
            // se esiste il valore ID_TIPO_ATTO allora è censita e esiste

            if (String.IsNullOrEmpty(templatesuap.ID_TIPO_ATTO))
                return false;

            string nuovoOggetto =null;
            try
            {
                SUAPPratica.RiepilogoPraticaSUAP riep = SUAPPratica.RiepilogoPraticaSUAP.Deserialize(xml);

                string procedimento = ComponiProcedimento(riep.struttura);
                string tipoProcedimento = riep.intestazione.oggettocomunicazione.tipoprocedimento.ToString();
                string tipoIntervento = riep.intestazione.oggettocomunicazione.tipointervento.ToString();
                string codicePratica = riep.intestazione.codicepratica;
                string impresa = ComponiImpresa(riep.intestazione.impresa);
                string codiceREA = riep.intestazione.impresa.codiceREA.Value;
                string legaleRapp = ComponiLegaleRapp(riep.intestazione.impresa.legalerappresentante);
                string Dichiarante = ComponiDichiarante(riep.intestazione.dichiarante);
                string domicilioElettronico = riep.intestazione.domicilioelettronico;
                string ImpiantoProduttivo = ComponiIndirizzo(riep.intestazione.impiantoproduttivo.indirizzo);
                string procuraSpeciale = riep.intestazione.procuraspeciale.nomefile;
                string allegati = ComponiAllegati(riep.struttura);

                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[0], procedimento);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[1], tipoProcedimento);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[2], tipoIntervento);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[3], codicePratica );
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[4], impresa );
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[5], codiceREA);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[6], legaleRapp);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[7], Dichiarante);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[8], domicilioElettronico);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[9], ImpiantoProduttivo);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[10], procuraSpeciale);
                popolaValoreTipologiaGenerico(templatesuap, TipologiaSUAPENTE[11], allegati);
                nuovoOggetto = costruisciNuovoOggetto(schedaDoc, riep);

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore facendo il parsing di: *.suap.xml {0} {1}", e.Message, e.StackTrace);
                return false;
            }

            //Associare il template alla scheda documento
            schedaDoc.template = templatesuap;
            schedaDoc.tipologiaAtto = new TipologiaAtto { descrizione = templatesuap.DESCRIZIONE, systemId = templatesuap.ID_TIPO_ATTO };
            schedaDoc.daAggiornareTipoAtto = true;
           
            
            if (!String.IsNullOrEmpty(nuovoOggetto))
            {
                schedaDoc.oggetto.descrizione = nuovoOggetto;
                schedaDoc.oggetto.daAggiornare = true;
            }
            

            //Salvare il documento
            bool daAggiornareUffRef;
            SchedaDocumento retval = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDoc, false, out daAggiornareUffRef, ruoloUtente);
            
            

            //non ha salvato
            if (retval == null)
                return false;

            //Tornare True o false a seconda dell'Esito
            return true;
        }
        #region compositoriCampiSuap
        private static string ComponiProcedimento(List<SUAPPratica.AdempimentoSUAP> strutturaList)
        {
            string retval = string.Empty;
            foreach (SUAPPratica.AdempimentoSUAP struttura in strutturaList)
            {
                string[] nomiProcedimento = struttura.nome.Replace ("++","§").Split ('§');
                foreach (string np in nomiProcedimento)
                    retval += np.Trim() + "\r\n";
            }

            retval = removeLastCRLF(retval);
            return retval;
        }

        private static string ComponiAllegati(List<SUAPPratica.AdempimentoSUAP> strutturaList)
        {
            string retval =string.Empty;
            foreach (SUAPPratica.AdempimentoSUAP struttura in strutturaList)
            {
                foreach (SUAPPratica.AllegatoGenerico allegato in struttura.documentoallegato)
                    retval += allegato.nomefile.Trim() + "\r\n";

                if (struttura.distintamodelloattivita != null)
                {
                    if (!String.IsNullOrEmpty (struttura.distintamodelloattivita.nomefile ))
                        retval += struttura.distintamodelloattivita.nomefile + "\r\n";

                    if (struttura.distintamodelloattivita.tracciatoxml!=null)
                        if (!String.IsNullOrEmpty (struttura.distintamodelloattivita.tracciatoxml.nomefile ))
                            retval += struttura.distintamodelloattivita.tracciatoxml.nomefile + "\r\n";
                }
            }
            retval = removeLastCRLF(retval);
            return retval;
        }

        private static string ComponiImpresa(SUAPPratica.AnagraficaImpresa impresa)
        {
            string PIString = string.Empty;
            string CFString = string.Empty;
            if (!String.IsNullOrEmpty(impresa.codicefiscale))
                CFString = String.Format("CF:{0}", impresa.codicefiscale);

            if (!String.IsNullOrEmpty(impresa.partitaiva))
            {
                CFString += "  -  ";
                PIString = String.Format("PI:{0}", impresa.partitaiva);
            }
            
            return String.Format("{0}\r\n{2}{3}\r\n{1}\r\n{4}", impresa.ragionesociale, impresa.formagiuridica.Value, CFString, PIString, ComponiIndirizzo(impresa.indirizzo));
        }

        private static string ComponiDichiarante(SUAPPratica.EstremiDichiarante dichiarante)
        {
            string retval = String.Format ("{0} {1} - {2}\r\n{3}\r\n{4} {5}",dichiarante.cognome,dichiarante.nome,dichiarante.codicefiscale, dichiarante.qualifica ,dichiarante.pec ,dichiarante.telefono);
            return retval;
        }

        private static string ComponiLegaleRapp(SUAPPratica.AnagraficaRappresentante legaleRapp)
        {
            string retval = String.Format("{0} {1} - {2} - {3}", legaleRapp.cognome, legaleRapp.nome, legaleRapp.codicefiscale, legaleRapp.carica.Value);
            return retval;
        }

        private static string  ComponiIndirizzo(SUAPPratica.Indirizzo indirizzo)
        {
            string comune="";
            string provincia = "";
            foreach (object o in indirizzo.Items)
            {
                SUAPPratica.Comune comunestr = o as SUAPPratica.Comune;
                SUAPPratica.Provincia provinciastr = o as SUAPPratica.Provincia;

                if (comunestr != null)
                    comune = " " + comunestr.Value;

                if (provinciastr != null)
                    provincia = "(" + provinciastr.sigla + ")";

            }

            string retval = String.Format("{0} {1} {2} {3} {4} {5}", indirizzo.toponimo, indirizzo.denominazionestradale, indirizzo.numerocivico, indirizzo.cap, comune, provincia);
            return retval;
        }
        #endregion

        #region popolatori campi profilati

        private bool popolaValoreTipologiaCampoTestuale(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {

            if (String.IsNullOrEmpty(valore))
                return false;

            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);
            if (ogg == null)
                return false;

            if (ogg != null)
            {
                ogg.VALORE_DATABASE = valore;
                return true;
            }
            return false;
        }

        private bool popolaValoreTipologiaDropDown(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {
            if (String.IsNullOrEmpty(valore))
                return false;

            if (valore.Contains('-'))
                valore = valore.Replace('-', '_');

            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);

            if (ogg == null)
                return false;

            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoDef = null;
            foreach (DocsPaVO.ProfilazioneDinamica.ValoreOggetto valo in ogg.ELENCO_VALORI)
            {
                if (valo.VALORE.ToLower().Equals(valore.ToLower()) &&
                    valo.ABILITATO == 1
                    )
                {
                    if (valo.VALORE_DI_DEFAULT == "SI")
                        valoDef = valo;
                    ogg.VALORE_DATABASE = valo.VALORE;
                    return true;
                }
            }
            //popolo il default
            ogg.VALORE_DATABASE = valoDef.VALORE;

            return false;
        }

        private DocsPaVO.ProfilazioneDinamica.OggettoCustom trovaOggettoPerNome(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
        {
            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in t.ELENCO_OGGETTI)
                if (ogg.DESCRIZIONE.ToLower().Equals(nome.ToLower()))
                    return ogg;

            return null;
        }

        private bool popolaValoreTipologiaGenerico(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {
            if (String.IsNullOrEmpty(valore))
                return false;

            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);

            if (ogg == null)
                return false;

            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina"))
                return popolaValoreTipologiaDropDown(t, nome, valore);

            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("CampoDiTesto"))
                return popolaValoreTipologiaCampoTestuale(t, nome, valore);

            return false;
        }

        private string getValoreOggettoGenerico(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
        {
            string retval = string.Empty;
            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);
            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina"))
            {
                retval = ogg.VALORE_DATABASE;
                if (retval.Contains('_'))
                    retval= retval.Replace('_', '-');

            } else

                retval = ogg.VALORE_DATABASE;

            return retval;
        }




        #endregion

        #region utilita
        string costruisciNuovoOggetto(SchedaDocumento sd, SUAPPratica.RiepilogoPraticaSUAP riepilogoPratica )
        {

            try
            {
                if (riepilogoPratica == null)
                    return null;

                if (riepilogoPratica.struttura == null)
                    return null;

                List<string> nomiProcList = new List<string>();
                List<string> mailentiList = new List<string>();
                string procedimentoCompleto = string.Empty;
                string procedimento = "";
                foreach (SUAPPratica.AdempimentoSUAP struttura in riepilogoPratica.struttura)
                {
                    procedimentoCompleto = struttura.nome;
                    string[] nomiProcedimento = procedimentoCompleto.Replace("++", "§").Split('§');
                    foreach (string np in nomiProcedimento)
                        nomiProcList.Add(np.Trim());

                    foreach (SUAPPratica.EstremiEnte EnteConinvoto in struttura.entecoinvolto)
                        mailentiList.Add(EnteConinvoto.pec);
                }

                string mailDest = getMailDestinatarioPredisposto(sd);
                if (!String.IsNullOrEmpty(mailDest))
                {
                    bool found = false;
                    if (nomiProcList.Count == mailentiList.Count)
                    {
                        int counter = 0;
                        foreach (string email in mailentiList)
                        {
                            if (email.ToUpper().Trim() == mailDest.ToUpper().Trim())
                            {
                                found = true;
                                break;
                            }
                            counter++;
                        }
                        if (nomiProcList.Count > counter)
                        {
                            if (found)
                                procedimento = nomiProcList[counter];
                        }

                        if (found == false)
                            procedimento = procedimentoCompleto;
                    }
                    else
                    {
                        procedimento = procedimentoCompleto;
                    }
                }
                else
                {
                    procedimento = procedimentoCompleto;
                }

                string impresa = "";
                string ImpiantoProduttivo = "";
                if (riepilogoPratica.intestazione != null)
                {
                    if (riepilogoPratica.intestazione.impresa != null)
                        if (!String.IsNullOrEmpty(riepilogoPratica.intestazione.impresa.ragionesociale))
                            impresa = riepilogoPratica.intestazione.impresa.ragionesociale;



                    ImpiantoProduttivo = ComponiIndirizzo(riepilogoPratica.intestazione.impiantoproduttivo.indirizzo);
                }

                String retval = String.Format("{0} | {1} | {2}", procedimento, impresa, ImpiantoProduttivo);

                return retval;
            }
            catch (Exception e)
            {
                logger.DebugFormat("Errore creando oggetto per docnumber {0}  err {1} stk {2}", sd.docNumber, e.Message, e.StackTrace);
            }
            return null;
        }

        string getMailDestinatarioPredisposto(SchedaDocumento sd)
        {
            string retval = null;
            try
            {
                System.Data.DataSet ds = BusinessLogic.interoperabilita.InteroperabilitaManager.GetAssDocAddress(sd.docNumber);
                if (ds != null)
                {
                    foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
                    {
                        retval = (string)dataRow["MAIL"];
                        break;
                    }
                }
            } catch (Exception e) {
                logger.ErrorFormat("Errore repernedo la mail dal docnumber {0} err {1} stk {2}",sd.docNumber, e.Message, e.StackTrace);
            }
            return retval;
        }


        public static bool compareEnteSuapXml(string xml1, string xml2)
        {
            xml1= removeInfoschema(xml1);
            xml2 = removeInfoschema(xml2);
            if (xml1 == xml2)
                return true;

            return false;
        }


        static string removeInfoschema(string xml)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xml);

            XmlNode node = xd.SelectSingleNode("//info-schema");
            node.ParentNode.RemoveChild(node);

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

            xd.WriteTo(xmlTextWriter);
            return stringWriter.ToString();
        }


        /// <summary>
        /// Rimuove il nodo impresa dall'XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        static string removeNodiInutilizzati(string xml)
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(xml);

                XmlNode nodeRea = xd.SelectSingleNode("//impresa//codice-REA");
                nodeRea.ParentNode.RemoveChild(nodeRea);
                XmlNode nodeNazionalita = xd.SelectSingleNode("//impresa//legale-rappresentante//nazionalita");
                nodeNazionalita.ParentNode.RemoveChild(nodeNazionalita);


                //node = xd.SelectSingleNode("//protocollo-pratica-suap");
                //node.ParentNode.RemoveChild(node);
                XmlNode noderi = xd.SelectSingleNode("//protocollo-ri");
                noderi.ParentNode.RemoveChild(noderi);

                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

                xd.WriteTo(xmlTextWriter);
                return stringWriter.ToString();
            }
            catch (Exception e)
            {
                logger.DebugFormat("Errore rimuovendo i nodi inutilizzati {0} {1}", e.Message, e.StackTrace);
                return xml;
            }
        }

        private static string removeLastCRLF(string retval)
        {
            if (retval.EndsWith("\r\n"))
                retval = retval.Substring(0, retval.Length - 2);
            return retval;
        }

        #endregion
    }
}
