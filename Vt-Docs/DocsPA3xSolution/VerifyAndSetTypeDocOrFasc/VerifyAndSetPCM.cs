using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.ProfilazioneDinamica;
using System.Xml;
using System.Xml.Linq;

namespace VerifyAndSetTypeDocOrFasc
{
    public class TIPO_PROVVEDIMENTO
    {
        #region Fields

        private string _tipo;
        private string _oggetto;
        private string _descrizione;
        private string _ultDescrizione;

        #endregion

        #region Properties

        public virtual string Tipo
        {
            get
            {
                return _tipo;
            }

            set
            {
                _tipo = value;
            }
        }

        public virtual string Oggetto
        {
            get
            {
                return _oggetto;
            }

            set
            {
                _oggetto = value;
            }
        }

        public virtual string Descrizione
        {
            get
            {
                return _descrizione;
            }

            set
            {
                _descrizione = value;
            }
        }

        public virtual string UltDescrizione
        {
            get
            {
                return _ultDescrizione;
            }

            set
            {
                _ultDescrizione = value;
            }
        }

        #endregion

        #region Costruttore

        public TIPO_PROVVEDIMENTO()
        {
        }

        #endregion

        #region PublicMethods



        #endregion
    }

    class VerifyAndSetPCM : VerifyAndSetManager
    {
        public override string verifyTipoDoc(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            string message = string.Empty;

            if (schedaDocumento != null)
            {
                DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare = schedaDocumento;

                //Controllo Preventivo
                if (schedaDocumentoDaVerificare != null &&
                    schedaDocumentoDaVerificare.template != null &&
                    schedaDocumentoDaVerificare.template.DESCRIZIONE.ToUpper().Equals("DOCUMENTOUBR")
                    )
                {
                    ControlloTipologiaDocControlloPreventivo(ref schedaDocumentoDaVerificare, ref message, infoUtente);

                    //Se i controlli sono andati tutti a buon fine aggiorno la scheda documento
                    if (string.IsNullOrEmpty(message))
                        schedaDocumento = schedaDocumentoDaVerificare;
                }
            }

            return message;
        }

        public override string verifyTipoFasc(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            return string.Empty;
        }

        private void ControlloTipologiaDocControlloPreventivo(ref DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare, ref string message, DocsPaVO.utente.InfoUtente infoUtente)
        {
            //**************************************************************************************************************************************
            //MODIFICA EVOLUTIVA: GIORDANO IACOZZILLI: 16/05/2012
            //Bisogna aggiungere 2 controlli al tipo documento UBR:
            //1) Ragione Proveddimento non è obbligatorio nei casi in cui 
            //   tipo provvedimento =
            //   Rendiconto
            //   Altro Tipo 
            //2) Numero impegno è obbligatorio solo se:
            //  tipo provvedimento =
            //  Impegno
            //  O/P su impegno
            //**************************************************************************************************************************************
            OggettoCustom[] elencoOggetti = (OggettoCustom[])schedaDocumentoDaVerificare.template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom));

            OggettoCustom _tipoprovvedimento = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Replace(" ", "").Equals("TIPODIPROVVEDIMENTO")).FirstOrDefault();
            OggettoCustom _OggettoProvvedimento = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Replace(" ", "").Equals("OGGETTO")).FirstOrDefault();
            OggettoCustom _Descrizioneprovvedimento = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Replace(" ", "").Equals("DESCRIZIONEPROVVEDIMENTO")).FirstOrDefault();
            OggettoCustom _UlterioreDescrizioneprovvedimento = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Replace(" ", "").Equals("ULTDESCRIZIONEPROVVEDIMENTO")).FirstOrDefault();


            //1) Numero impegno è obbligatorio solo se:
            //  tipo provvedimento =
            //  Impegno
            //  O/P su impegno
            if ((_tipoprovvedimento.VALORE_DATABASE.ToUpper().Replace(" ", "") == "IMPEGNO") ||
               (_tipoprovvedimento.VALORE_DATABASE.ToUpper().Replace(" ", "") == "O/PSUIMPEGNO"))
            {
                OggettoCustom _numeroimpegno = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Replace(" ", "").Equals("NUMEROIMPEGNO")).FirstOrDefault();
                if (string.IsNullOrEmpty(_numeroimpegno.VALORE_DATABASE))
                    message += "\\n\\nIl campo [Numero Impegno] è obbligatorio nei casi in cui il [tipo provvedimento] sia uguale a [Impegno] e [o/p su Impegno] ";
            }

            //2) Il campo importo deve essere un decimal.
            OggettoCustom _importo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Replace(" ", "").Equals("IMPORTO")).FirstOrDefault();

            if (!string.IsNullOrEmpty(_importo.VALORE_DATABASE))
            {
                try
                {
                    decimal _dec = Convert.ToDecimal(_importo.VALORE_DATABASE.Replace(".", ",").ToString());
                }
                catch
                {
                    message += "\\n\\nIl campo [Importo] deve essere di tipo Decimal";
                }
            }


            //3) Modifica di congruità dei valori inseriti nelle 4 combobox:

            //1) Tipo Provvedimento
            //2) Oggetto
            //3) Descrizione
            //4) ulteriore descrizione

            //Get XML Dictionary:
            XDocument myxml = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"Xml\PCM_TIPO_PROVVEDIMENTI.xml");
            TIPO_PROVVEDIMENTO _typeprov = new TIPO_PROVVEDIMENTO();
            List<TIPO_PROVVEDIMENTO> _lsttipo = new List<TIPO_PROVVEDIMENTO>();
            //Inserisco l'xml nella mia lista per filtrarlo con linq:
            foreach (var tipiprovvedimenti in myxml.Descendants("Records"))
            {
                string tipo = tipiprovvedimenti.Element("TIPO") == null ? string.Empty : tipiprovvedimenti.Element("TIPO").Value;
                string oggetto = tipiprovvedimenti.Element("OGGETTO") == null ? string.Empty : tipiprovvedimenti.Element("OGGETTO").Value;
                string descrizione = tipiprovvedimenti.Element("DESCRIZIONE") == null ? string.Empty : tipiprovvedimenti.Element("DESCRIZIONE").Value;
                string ultdescrizione = tipiprovvedimenti.Element("ULTDESCRIZIONE") == null ? string.Empty : tipiprovvedimenti.Element("ULTDESCRIZIONE").Value;

                _lsttipo.Add(new TIPO_PROVVEDIMENTO { Tipo = tipo, Oggetto = oggetto, Descrizione = descrizione, UltDescrizione = ultdescrizione });
            }
            //Chiamo la funzione di filtro passandogli il messaggio per gli errori di ritorno.
            message += VerifyComboProvvedimenti(_lsttipo, _tipoprovvedimento, _OggettoProvvedimento, _Descrizioneprovvedimento, _UlterioreDescrizioneprovvedimento);


            //Se i controlli sono andati tutti a buon fine aggiorno i campi
            if (string.IsNullOrEmpty(message))
                schedaDocumentoDaVerificare.template.ELENCO_OGGETTI = new System.Collections.ArrayList(elencoOggetti);
        }

        private string VerifyComboProvvedimenti(List<TIPO_PROVVEDIMENTO> _lsttipo, OggettoCustom _tipoprovvedimento, OggettoCustom _ragioneProvvedimento, OggettoCustom _Descrizioneprovvedimento, OggettoCustom _UlterioreDescrizioneprovvedimento)
        {
            try
            {
                //Filtro con Linq!
                //Il primo filtro massivo è sul valore di tipo provvedimento:
                var queryTipoProvvedimento = from mylistQ in _lsttipo
                                             where mylistQ.Tipo == _tipoprovvedimento.VALORE_DATABASE.ToString()
                                             select mylistQ;

                //Se filtro provvedimento è valorizzato, vado a verificare il secondo 
                //livello, cioè l'oggetto
                if (queryTipoProvvedimento.Count() > 0)
                {
                    //Vado a filtrare per l'oggetto:
                    var queryOggetto = (from mylistOgg in queryTipoProvvedimento
                                        where mylistOgg.Oggetto == _ragioneProvvedimento.VALORE_DATABASE.ToString()
                                        select mylistOgg).Distinct();

                    if (queryOggetto.Count() > 0)
                    {
                        //Vado a filtrare per la descrizione:
                        var queryDescrizione = (from mylistDescr in queryOggetto
                                                where mylistDescr.Descrizione == _Descrizioneprovvedimento.VALORE_DATABASE.ToString()
                                                select mylistDescr).Distinct();

                        if (queryDescrizione.Count() > 0)
                        {
                            //Vado a filtrare per la Ulteriore descrizione:
                            var queryULTDescrizione = (from mylistUltDescr in queryDescrizione
                                                       where mylistUltDescr.UltDescrizione == _UlterioreDescrizioneprovvedimento.VALORE_DATABASE.ToString()
                                                       select mylistUltDescr).Distinct();

                            if (queryULTDescrizione.Count() > 0)
                            {
                                return "";
                            }
                            else
                            {
                                string returnMessage = string.Empty;
                                foreach (var _TP in queryDescrizione)
                                {
                                    if (string.IsNullOrEmpty(_TP.UltDescrizione))
                                        //Se non ho nessun valore, vuol dire che tutte le combo non devono essere valorizzate!
                                        return "\\n\\nCon il valore del campo [Descrizione provvedimento] inserito, il valore del campo [ult Descrizione provvedimento] non deve essere valorizzato";
                                    else
                                        if (!returnMessage.Contains(_TP.UltDescrizione))
                                            returnMessage += "\\n" + _TP.UltDescrizione;
                                }
                                return "\\n\\nIl valore del campo [Ult descrizione provvedimento] selezionato, non è compatibile con il valore del campo [descrizione] inserito."
                                + "\\n\\nElenco valori ammessi per la descrizione provvedimento selezionata:"
                                + returnMessage;
                            }
                        }
                        else
                        {
                            string returnMessage = string.Empty;
                            foreach (var _TP in queryOggetto)
                            {
                                if (string.IsNullOrEmpty(_TP.Descrizione))
                                    //Se non ho nessun valore, vuol dire che tutte le combo non devono essere valorizzate!
                                    return "\\n\\nCon il valore del campo [Oggetto] inserito i valori  [descrizione provvedimento], [ult Descrizione provvedimento] non devono essere valorizzati";
                                else
                                    if (!returnMessage.Contains(_TP.Descrizione))
                                        returnMessage += "\\n" + _TP.Descrizione;
                            }
                            return "\\n\\nIl valore del campo [descrizione provvedimento] selezionato, non è compatibile con il valore del campo [oggetto] inserito."
                            + "\\n\\nElenco valori ammessi per il campo oggetto selezionato:"
                            + returnMessage;
                        }
                    }
                    else
                    {
                        string returnMessage = string.Empty;
                        foreach (var _TP in queryTipoProvvedimento)
                        {
                            if (string.IsNullOrEmpty(_TP.Oggetto))
                                //Se non ho nessun valore, vuol dire che tutte le combo non devono essere valorizzate!
                                return "\\nCon il valore del campo [Tipo di Provvedimento] inserito i valori [oggetto], [descrizione provvedimento], [ult Descrizione provvedimento] non devono essere valorizzati";
                            else
                                if (!returnMessage.Contains(_TP.Oggetto))
                                    returnMessage += "\\n" + _TP.Oggetto;
                        }
                        return "\\n\\nIl valore del campo [oggetto] selezionato, non è compatibile con il valore del campo [Tipo di Provvedimento] inserito."
                        + "\\n\\nElenco valori ammessi per il tipo provvedimento selezionato:"
                        + returnMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }
    }


}
