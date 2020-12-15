using System;
using System.Collections;
using System.Linq;
using System.Text;


namespace BusinessLogic.ExportDati
{
    public class ModelDataProcessor
    {
        public const string mittente = "mitt/dest";
        public const string TotDocumenti = "totaledocumenti";
        public const string utente = "utente";
        public const string UO = "uo";
        public const string ruoloUtente = "ruoloutente";

        public string fillData(string data, ArrayList documenti, DocsPaVO.utente.Utente ut,  DocsPaVO.utente.InfoUtente infoUser, DocsPaVO.filtri.FiltroRicerca[][] filtri)
        {
            string result = string.Empty;
            result = data;
            //data=data.Trim();
            //string oggetto = data.Substring(1, data.Length-1);
            //controllo se è un'oggetto standard
            try {
                switch (data.ToLower())
                {
                    case mittente:
                        result = string.Empty;
                        foreach (DocsPaVO.filtri.FiltroRicerca[] filterArray in filtri)
                        {
                            foreach (DocsPaVO.filtri.FiltroRicerca filterItem in filterArray)
                            {
                                if (filterItem.argomento.Equals("COD_MITT_DEST") && filterItem.valore != null && filterItem.valore != "")
                                {
                                    DocsPaVO.utente.Corrispondente corr = Utenti.UserManager.getCorrispondenteByCodRubrica(filterItem.valore, infoUser);
                                    result = corr.descrizione;
                                }
                                if (filterItem.argomento.Equals("MITT_DEST") && filterItem.valore != null && filterItem.valore != "")
                                {
                                    result = ((DocsPaVO.documento.InfoDocumentoExport)documenti[0]).mittentiDestinatari;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(result))
                            result = ((DocsPaVO.documento.InfoDocumentoExport)documenti[0]).mittentiDestinatari; 

                        break;
                    case TotDocumenti:
                        result = "n° " + documenti.Count.ToString();
                        break;
                    case utente:
                        result = ut.nome + " " + ut.cognome;
                        break;
                    case UO:
                        DocsPaVO.utente.Corrispondente corrisp = Utenti.UserManager.getCorrispondenteBySystemID(infoUser.idCorrGlobali);
                        //corrisp.
                        result = ((DocsPaVO.utente.Ruolo)corrisp).uo.descrizione;
                        break;
                    case ruoloUtente:
                        result = ((DocsPaVO.utente.Ruolo)Utenti.UserManager.getRuoloById(infoUser.idCorrGlobali)).descrizione;
                        break;
                    //Controllo se è un campo profilato
                    default:
                        //Controllo se è un campo profilato
                        result = FillCampiProfilati(data, documenti);
                        //Se torna Null il campo non è stato trovato
                        if (string.IsNullOrEmpty(result))
                            result = data;
                        break;
                
                }
                
            }
            catch (Exception e)
            {
                result = "-";
            }
            return result;
        }

        private string FillCampiProfilati(string campo, ArrayList documenti)
        {
            string result = string.Empty;
            foreach (DocsPaVO.documento.InfoDocumentoExport documento in documenti)
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(documento.docNumber);
                
                if (template != null && template.ELENCO_OGGETTI.Count != 0)
                {
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                    {
                        if (campo.Equals(oggettoCustom.DESCRIZIONE))
                        {
                            ExportDatiManager exportDatiManager = new ExportDatiManager();
                            result = exportDatiManager.getValoreOggettoCustom(oggettoCustom);
                            return result;
                        }
                    }
                    
                     
                }
            }
            return result;
            
        }
    }
}
