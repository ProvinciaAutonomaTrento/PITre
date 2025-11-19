// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.utente;
using Serilog;
using System.Xml;

namespace BusinessLogic.XmlParsing.suap;

public class SuapManager
{
    private static ILogger logger = Log.ForContext(typeof(SuapManager));


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

        string pecSuap = fromPecEmail; // TO//RICAVARE          suap@pec.comune.trento.it
        string enteMittsuap = "";
        if (schedaDocumento.registro != null)
            if (!String.IsNullOrEmpty(schedaDocumento.registro.codAmministrazione))
                enteMittsuap = schedaDocumento.registro.codAmministrazione;

        string attributoOggcomunicazione = getValoreOggettoGenerico(schedaDocumento.template, TipologiaENTESUAP[1]);
        string valueOggComunicazione = schedaDocumento.oggetto.descrizione; // ""; //COSTRUITA SULLA BASE DI UNA TABELLA DI TESTI PER "TIPO-COOPERAZIONE" + "in merito a pratica " + CODICE-PRATICA + " - " + SUAP " +identificativo-suap + " " + nome_impresa  //VAR_PROF_OGGETTO
        string testoComunicazione = null;


        string nomeFilePrinc = getOriginalFileNameOfDocPrincipale(infoUtente, schedaDocumento);
        if (attributoOggcomunicazione.ToUpper().Trim() == "ALTRO")
            testoComunicazione = String.Format("Si trasmette comunicazione allegata ({0})", nomeFilePrinc);
        else
            testoComunicazione = String.Format("Si trasmette comunicazione {0} allegata ({1})", attributoOggcomunicazione.Replace("-", " "), nomeFilePrinc);


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

        if (schedaDocumento.template == null)
            return string.Empty;

        if (schedaDocumento.template.DESCRIZIONE != _nomeTemplate)
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

        if (schedaDocumento.protocollo != null)
        {
            string codAMM = schedaDocumento.registro.codAmministrazione;
            string codAOO = schedaDocumento.registro.codRegistro;
            DateTime dataProt = DocsPaUtils.Functions.Functions.ToDate(schedaDocumento.protocollo.dataProtocollazione);
            string protoSchDoc = schedaDocumento.protocollo.numero.PadLeft(7, '0');
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
                    if ((originalName != null) && (!originalName.ToLowerInvariant().Equals("entesuap.xml")))
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

    private string getValoreOggettoGenerico(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
    {
        string retval = string.Empty;
        DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);
        if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina"))
        {
            retval = ogg.VALORE_DATABASE;
            if (retval.Contains('_'))
                retval = retval.Replace('_', '-');

        }
        else

            retval = ogg.VALORE_DATABASE;

        return retval;
    }

    private string getOriginalFileNameOfDocPrincipale(InfoUtente infoUtente, SchedaDocumento schedaDocumento)
    {
        string originalName = null;
        FileRequest doc = schedaDocumento.documenti[0] as FileRequest;
        if (doc != null)
            originalName = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, doc);

        return originalName;
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
            logger.Debug("Errore rimuovendo i nodi inutilizzati {0} {1}", e.Message, e.StackTrace);
            return xml;
        }
    }

    private DocsPaVO.ProfilazioneDinamica.OggettoCustom trovaOggettoPerNome(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
    {
        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in t.ELENCO_OGGETTI)
            if (ogg.DESCRIZIONE.ToLower().Equals(nome.ToLower()))
                return ogg;

        return null;
    }

    public static bool compareEnteSuapXml(string xml1, string xml2)
    {
        xml1 = removeInfoschema(xml1);
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


}
