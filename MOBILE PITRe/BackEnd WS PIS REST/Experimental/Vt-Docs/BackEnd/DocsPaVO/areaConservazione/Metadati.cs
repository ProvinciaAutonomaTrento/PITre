using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    /// <summary>
    /// **************************!!!SUCCESSIVAMENTE AGGIUNGERE PROTOCOLLO EMERGENZA!!!+++++++++++++++++++++++++++++
    /// </summary>
    [XmlInclude(typeof(ProtocolloEntrata))]
    [XmlInclude(typeof(ProtocolloUscita))]
    [XmlInclude(typeof(ProtocolloInterno))]
    [Serializable()]
    public class Metadati
    {
        public string dataCreazione;
        public string oraCreazione;
        public string docNumber;
        public string tipoProto; //va interpretato es: G=non protocollato
        
        /// <summary>
        /// (segnatura,numero,dataprotocollo,anno)
        /// (mittente e destinatari (solo descrizione))
        /// </summary>
        public Protocollo protocollo;
        
        public Oggetto oggetto; //(descrizione)

        public TipologiaAtto tipologiaAtto; //(descrizione)
        
        public Registro registro; //(codice)
        
        public Protocollatore protocollatore; //(nome + cognome) query (valido solo se prot)
        
        public CreatoreDocumento creatoreDocumento; //(nome + cognome) query
        
        public string descMezzoSpedizione;
        
        /// <summary>
        /// (data inserimento, version e autore)
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(Documento))]
        public System.Collections.ArrayList documenti;

        /// <summary>
        /// (data inserimento, version, descrizione e autore)
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(Allegato))]
        public System.Collections.ArrayList allegati;

        /// <summary>
        /// (descrizione)
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(ParolaChiave))]
        public ArrayList paroleChiave;

        /// <summary>
        /// Costruttore con i relativi controlli!!!
        /// </summary>
        /// <param name="schDoc"></param>
        public Metadati(DocsPaVO.documento.SchedaDocumento schDoc)
        {
            try
            {
                dataCreazione = schDoc.dataCreazione;
                oraCreazione = schDoc.oraCreazione;
                docNumber = schDoc.docNumber;
                descMezzoSpedizione = schDoc.descMezzoSpedizione;

                if (schDoc.tipoProto.ToUpper() == "G")
                {
                    tipoProto = "non protocollato";
                }
                else
                {
                    tipoProto = schDoc.tipoProto;
                }

                if (schDoc.protocollo != null)
                {
                    if (schDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
                    {
                        DocsPaVO.documento.ProtocolloEntrata protE = (DocsPaVO.documento.ProtocolloEntrata)schDoc.protocollo;
                        ProtocolloEntrata protocolloE = new ProtocolloEntrata();
                        protocolloE.anno = protE.anno;
                        protocolloE.dataProtocollazione = protE.dataProtocollazione;
                        protocolloE.dataProtocolloMittente = protE.dataProtocolloMittente;
                        protocolloE.descrizioneProtocolloMittente = protE.descrizioneProtocolloMittente;
                        protocolloE.numero = protE.numero;
                        protocolloE.segnatura = protE.segnatura;
                        if (protE.mittente != null)
                        {
                            protocolloE.mittente = new Corrispondente();
                            protocolloE.mittente.descrizione = protE.mittente.descrizione;
                        }
                        else
                        {
                            protocolloE.mittente = null;
                        }
                        protocollo = new ProtocolloEntrata();
                        protocollo = protocolloE;
                    }

                    if (schDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
                    {
                        DocsPaVO.documento.ProtocolloInterno protI = (DocsPaVO.documento.ProtocolloInterno)schDoc.protocollo;
                        ProtocolloInterno protocolloI = new ProtocolloInterno();
                        protocolloI.anno = protI.anno;
                        protocolloI.dataProtocollazione = protI.dataProtocollazione;
                        protocolloI.numero = protI.numero;
                        protocolloI.segnatura = protI.segnatura;
                        if (protI.mittente != null)
                        {
                            protocolloI.mittente = new Corrispondente();
                            protocolloI.mittente.descrizione = protI.mittente.descrizione;
                        }
                        else
                        {
                            protocolloI.mittente = null;
                        }
                        if (protI.destinatari.Count > 0)
                        {
                            protocolloI.destinatari = new ArrayList();
                            for (int i = 0; i < protI.destinatari.Count; i++)
                            {
                                DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)protI.destinatari[i];
                                Corrispondente corrCons = new Corrispondente();
                                corrCons.descrizione = corr.descrizione;
                                protocolloI.destinatari.Add(corrCons);
                            }
                        }
                        if (protI.destinatariConoscenza.Count > 0)
                        {
                            protocolloI.destinatariConoscenza = new ArrayList();
                            for (int i = 0; i < protI.destinatariConoscenza.Count; i++)
                            {
                                DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)protI.destinatariConoscenza[i];
                                Corrispondente corrCons = new Corrispondente();
                                corrCons.descrizione = corr.descrizione;
                                protocolloI.destinatariConoscenza.Add(corrCons);
                            }
                        }
                        protocollo = new ProtocolloInterno();
                        protocollo = protocolloI;
                    }

                    if (schDoc.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
                    {
                        DocsPaVO.documento.ProtocolloUscita protU = (DocsPaVO.documento.ProtocolloUscita)schDoc.protocollo;
                        ProtocolloUscita protocolloU = new ProtocolloUscita();
                        protocolloU.anno = protU.anno;
                        protocolloU.dataProtocollazione = protU.dataProtocollazione;
                        protocolloU.numero = protU.numero;
                        protocolloU.segnatura = protU.segnatura;
                        if (protU.mittente != null)
                        {
                            protocolloU.mittente = new Corrispondente();
                            protocolloU.mittente.descrizione = protU.mittente.descrizione;
                        }
                        else
                        {
                            protocolloU.mittente = null;
                        }
                        if (protU.destinatari.Count > 0)
                        {
                            protocolloU.destinatari = new ArrayList();
                            for (int i = 0; i < protU.destinatari.Count; i++)
                            {
                                DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)protU.destinatari[i];
                                Corrispondente corrCons = new Corrispondente();
                                corrCons.descrizione = corr.descrizione;
                                protocolloU.destinatari.Add(corrCons);
                            }
                        }
                        if (protU.destinatariConoscenza.Count > 0)
                        {
                            protocolloU.destinatariConoscenza = new ArrayList();
                            for (int i = 0; i < protU.destinatariConoscenza.Count; i++)
                            {
                                DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)protU.destinatariConoscenza[i];
                                Corrispondente corrCons = new Corrispondente();
                                corrCons.descrizione = corr.descrizione;
                                protocolloU.destinatariConoscenza.Add(corrCons);
                            }
                        }
                        protocollo = new ProtocolloUscita();
                        protocollo = protocolloU;
                    }
                }

                if (schDoc.oggetto != null)
                {
                    oggetto = new Oggetto();
                    oggetto.descrizione = schDoc.oggetto.descrizione;
                }

                if (schDoc.tipologiaAtto != null)
                {
                    tipologiaAtto = new TipologiaAtto();
                    tipologiaAtto.descrizione = schDoc.tipologiaAtto.descrizione;
                }

                if (schDoc.registro != null)
                {
                    registro = new Registro();
                    registro.codRegistro = schDoc.registro.codRegistro;
                }

                if (schDoc.protocollatore != null)
                {
                    protocollatore = new Protocollatore();
                    protocollatore.Nome_Cognome = schDoc.protocollatore.utente_idPeople;
                }

                if (schDoc.creatoreDocumento != null)
                {
                    creatoreDocumento = new CreatoreDocumento();
                    creatoreDocumento.Nome_Cognome = schDoc.creatoreDocumento.idPeople;
                }

                if (schDoc.documenti.Count > 0)
                {
                    documenti = new ArrayList();
                    for (int i = 0; i < schDoc.documenti.Count; i++)
                    {
                        DocsPaVO.documento.Documento doc = (DocsPaVO.documento.Documento)schDoc.documenti[i];
                        Documento docCons = new Documento();
                        docCons.autore = doc.autore;
                        docCons.dataInserimento = doc.dataInserimento;
                        docCons.descrizione = doc.descrizione;
                        docCons.version = doc.version;
                        documenti.Add(docCons);
                    }
                }

                if (schDoc.allegati.Count > 0)
                {
                    allegati = new ArrayList();
                    foreach (documento.Allegato t in schDoc.allegati)
                    {
                        allegati.Add(new Allegato()
                        {
                            autore = t.autore,
                            dataInserimento = t.dataInserimento,
                            descrizione = t.descrizione,
                            version = t.version
                        });
                    }
                }

                if (schDoc.paroleChiave.Count > 0)
                {
                    paroleChiave = new ArrayList();
                    foreach (documento.ParolaChiave t in schDoc.paroleChiave)
                        paroleChiave.Add(new ParolaChiave{descrizione = t.descrizione});
                }
            }
            catch
            {
                // Ignored
            }
        }

        public Metadati()
        {
            dataCreazione = string.Empty;
            oraCreazione = string.Empty;
            docNumber = string.Empty;
            tipoProto = string.Empty;
            descMezzoSpedizione = string.Empty;
            protocollo = new Protocollo();
            oggetto = new Oggetto();
            tipologiaAtto = new TipologiaAtto();
            registro = new Registro();
            protocollatore = new Protocollatore();
            creatoreDocumento = new CreatoreDocumento();
            documenti = null;
            allegati = null;
            paroleChiave = null;
        }
    }

    /// <summary>
    /// </summary>
    public class Protocollo
    {
        public string numero;
        public string dataProtocollazione;
        public string anno;
        public string segnatura;
    }

    /// <summary>
    /// </summary>
    public class ProtocolloEntrata : Protocollo
    {
        public Corrispondente mittente;
        public string descrizioneProtocolloMittente;
        public string dataProtocolloMittente;
    }

    /// <summary>
    /// </summary>
    public class ProtocolloUscita : Protocollo
    {
        public Corrispondente mittente;
        
        [XmlArray()]
        [XmlArrayItem(typeof(Corrispondente))]
        public System.Collections.ArrayList destinatari;

        [XmlArray()]
        [XmlArrayItem(typeof(Corrispondente))]
        public System.Collections.ArrayList destinatariConoscenza;
    }

    /// <summary>
    /// </summary>
    public class ProtocolloInterno : ProtocolloUscita
    {

    }

    /// <summary>
    /// </summary>
    public class Corrispondente
    {
        public string descrizione;
    }

    /// <summary>
	/// </summary>
    public class Oggetto
    {
        public string descrizione;
    }

    /// <summary>
    /// </summary>
    public class TipologiaAtto
    {
        public string descrizione;
    }

    /// <summary>
    /// </summary>
    public class Registro
    {
        public string codRegistro;
    }

    /// <summary>
	/// Summary description for Protocollatore.
	/// </summary>
    public class Protocollatore
    {
        //public string utente_idPeople;
        public string Nome_Cognome;

        //public string ruolo_idCorrGlobali;
        //public string uo_idCorrGlobali;
        //public string uo_codiceCorrGlobali;
    }

    /// <summary>
	/// Summary description for CreatoreDocumento.
	/// </summary>
    public class CreatoreDocumento
    {
        //public string idPeople;
        public string Nome_Cognome;

        //public string idCorrGlob_Ruolo;
        //public string idCorrGlob_UO;
        //public string uo_codiceCorrGlobali;
    }

    /// <summary>
    /// </summary>
    public class Documento : FileRequest
    {

    }

    /// <summary>
    /// </summary>
    public class Allegato : FileRequest
    {

    }

    /// <summary>
	/// </summary>
    public class FileRequest
    {
        public string dataInserimento;
        public string descrizione;
        //public string docNumber;
        //public string path;
        //public string fileName;
        public string version;
        //public string subVersion;
        //public string versionLabel;
        //public string fileSize;
        public string autore;
    }

    /// <summary>
    /// </summary>
    public class ParolaChiave
    {
        public string descrizione;
    }
}
