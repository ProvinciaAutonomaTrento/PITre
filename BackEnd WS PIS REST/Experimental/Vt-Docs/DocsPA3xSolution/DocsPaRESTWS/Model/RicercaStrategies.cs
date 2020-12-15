using DocsPaVO.filtri;
using DocsPaVO.Mobile;
using DocsPaVO.Mobile.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using System.Collections;
using DocsPaVO.documento;
using DocsPaVO.amministrazione;
using DocsPaVO.filtri.ricerca;
using DocsPaVO.fascicolazione;
using DocsPaRESTWS.Model.Decorators;

namespace DocsPaRESTWS.Model
{
    public class RicercaStrategies
    {
        public abstract class RicercaStrategy
        {

            private ILog logger = LogManager.GetLogger(typeof(RicercaStrategy));

            public abstract bool canSolve(RicercaRequest request);
            public abstract void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response);
            protected string buildMethodName = "buildResponse";
            private static RicercaStrategy[] _allStrategies = new RicercaStrategy[] { 
            new RicercaSalvataFascicoloStrategy(), 
            new RicercaSalvataDocumentoStrategy(),
            new RicercaTestoFascicoloStrategy(),
            new RicercaAdlTestoFascicoloStrategy(),
            new RicercaTestoDocumentoStrategy(),
            new RicercaAdlTestoDocumentoStrategy(),
            new RicercaTestoDocFascStrategy(),
            new RicercaAdlDocFascStrategy(),
            new FolderContentStrategy() };

            public static RicercaStrategy GetStrategy(RicercaRequest request)
            {
                foreach (RicercaStrategy strategy in _allStrategies)
                {
                    if (strategy.canSolve(request)) return strategy;
                }
                return null;
            }

            protected FiltroRicerca[][] getFiltri(string input)
            {
                Type t = typeof(ArrayOfArrayOfFiltroRicerca);
                System.Xml.Serialization.XmlSerializer _serializer = new System.Xml.Serialization.XmlSerializer(t);
                System.IO.StringReader sr = new System.IO.StringReader(input);

                System.Xml.XmlTextReader _reader = new System.Xml.XmlTextReader(sr);
                ArrayOfArrayOfFiltroRicerca res = (ArrayOfArrayOfFiltroRicerca)_serializer.Deserialize(_reader);
                return res.Filtri;
            }

            protected FiltroRicerca[] getFiltriDoc(RicercaRequest request, InfoUtente iu, bool inAdl)
            {
                logger.Debug("Impostazione filtri documento");
                List<FiltroRicerca> filtriDoc = new List<FiltroRicerca>(); ;
                if (!string.IsNullOrEmpty(request.Text))
                {
                    FiltroRicerca fOgg = new FiltroRicerca();
                    fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                    fOgg.valore = request.Text;//
                    filtriDoc.Add(fOgg);
                    /*
                    FiltroRicerca fOggAnno = new FiltroRicerca();
                    fOggAnno.argomento = listaArgomenti.ANNO_PROTOCOLLO.ToString();
                    fOggAnno.valore = System.DateTime.Now.Year.ToString();
                    filtri.Add(fOggAnno);
                     */
                }
                listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                listaArgomenti.PROT_ARRIVO,
                listaArgomenti.PROT_PARTENZA,
                listaArgomenti.PROT_INTERNO
            };
                foreach (listaArgomenti arg in defaultArgomenti)
                {
                    FiltroRicerca temp = new FiltroRicerca();
                    temp.argomento = arg.ToString();
                    temp.valore = "true";
                    filtriDoc.Add(temp);
                }

                if(string.IsNullOrWhiteSpace(request.DataProtoDa) && string.IsNullOrWhiteSpace(request.DataProtoA) && 
                    string.IsNullOrWhiteSpace(request.NumProto) )
                {
                    FiltroRicerca temp = new FiltroRicerca();
                    temp.argomento = listaArgomenti.GRIGIO.ToString();
                    temp.valore = "true";
                    filtriDoc.Add(temp);
                    temp = new FiltroRicerca();
                    temp.argomento = listaArgomenti.PREDISPOSTO.ToString();
                    temp.valore = "true";
                    filtriDoc.Add(temp);
                }
                Ruolo AdlRuolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(iu.idGruppo);

                if (inAdl)
                {
                    FiltroRicerca fAdldoc = new FiltroRicerca();
                    fAdldoc.argomento = listaArgomenti.DOC_IN_ADL.ToString();
                    fAdldoc.valore = String.Format("{0}@{1}", iu.idPeople, iu.idCorrGlobali);
                    filtriDoc.Add(fAdldoc);

                    //Filtro per Registro attivo:
                    string listaRegistri = string.Empty;
                    foreach (DocsPaVO.utente.Registro reg in AdlRuolo.registri)
                    {
                        if (!reg.flag_pregresso)
                        {
                            listaRegistri += "," + reg.systemId.ToString();
                        }

                    }
                    listaRegistri = listaRegistri.Substring(1);


                    FiltroRicerca fAdlReg = new FiltroRicerca();
                    fAdlReg.argomento = listaArgomenti.REGISTRO.ToString();
                    fAdlReg.valore = listaRegistri;
                    filtriDoc.Add(fAdlReg);
                }
                //Filtro ricerca temporale -1anno
                FiltroRicerca fDataPrec = new FiltroRicerca();
                FiltroRicerca fDataSuccAl = new FiltroRicerca();
                if (!string.IsNullOrWhiteSpace(request.DataDa) && !string.IsNullOrWhiteSpace(request.DataA))
                {
                    logger.DebugFormat("Presenti i valori DataDa {0} e DataA {1}", request.DataDa, request.DataA);
                    fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    fDataPrec.valore = request.DataA;

                    fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    fDataSuccAl.valore = request.DataDa;
                }
                else if (!string.IsNullOrWhiteSpace(request.DataDa))
                {
                    logger.DebugFormat("Presenti il valore DataDa {0}",request.DataDa);
                    
                    fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    fDataSuccAl.valore = request.DataDa;
                    fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
                    fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    fDataSuccAl.valore = DateTime.Now.AddMonths(-6).Date.ToString("dd/MM/yyyy");
                }
                filtriDoc.Add(fDataPrec);
                filtriDoc.Add(fDataSuccAl);

                if (!string.IsNullOrWhiteSpace(request.DataProtoA))
                {

                    FiltroRicerca filtrodataa = new FiltroRicerca();
                    filtrodataa.argomento = "DATA_PROT_PRECEDENTE_IL";
                    filtrodataa.valore = request.DataProtoA;
                    // +" 23:59:59";
                    filtriDoc.Add(filtrodataa);
                }

                if (!string.IsNullOrWhiteSpace(request.DataProtoDa))
                {
                    FiltroRicerca filtrodatada = new FiltroRicerca();
                    filtrodatada.argomento = "DATA_PROT_SUCCESSIVA_AL";
                    filtrodatada.valore = request.DataProtoDa;
                    // +" 00:00:00";
                    filtriDoc.Add(filtrodatada);

                }

                if (!string.IsNullOrWhiteSpace(request.IdDocumento))
                {
                    FiltroRicerca filtroId = new FiltroRicerca();
                    filtroId.argomento = listaArgomenti.DOCNUMBER.ToString();
                    filtroId.valore = request.IdDocumento;
                    filtriDoc.Add(filtroId);
                }

                if (!string.IsNullOrWhiteSpace(request.NumProto))
                {
                    FiltroRicerca filtroNumProto = new FiltroRicerca();
                    filtroNumProto.argomento = listaArgomenti.NUM_PROTOCOLLO.ToString();
                    filtroNumProto.valore = request.NumProto;
                    filtriDoc.Add(filtroNumProto);
                }
                filtriDoc.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ORACLE_FIELD_FOR_ORDER.ToString(), valore = "NVL (a.dta_proto, a.creation_time)" });
                filtriDoc.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.SQL_FIELD_FOR_ORDER.ToString(), valore = "ISNULL (a.dta_proto, a.creation_time)" });


                return filtriDoc.ToArray();
            }

            protected FiltroRicerca[] getFiltriFasc(RicercaRequest request, InfoUtente iu, bool inAdl)
            {
                logger.Debug("Impostazione filtri fascicolo");
                List<FiltroRicerca> filtriFasc = new List<FiltroRicerca>(); ;
                if (!string.IsNullOrEmpty(request.Text))
                {
                    FiltroRicerca fOgg = new FiltroRicerca();
                    fOgg.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.TITOLO.ToString();
                    fOgg.valore = request.Text;//
                    filtriFasc.Add(fOgg);
                    /*
                    FiltroRicerca fOggAnno = new FiltroRicerca();
                    fOggAnno.argomento = listaArgomenti.ANNO_PROTOCOLLO.ToString();
                    fOggAnno.valore = System.DateTime.Now.Year.ToString();
                    filtri.Add(fOggAnno);
                     */
                }
                List<OrgTitolario> titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(iu.idAmministrazione).Cast<OrgTitolario>().ToList();
                string listaTitolari = string.Empty;
                foreach (OrgTitolario tit in titolari)
                    listaTitolari += "," + tit.ID.ToString();

                listaTitolari = listaTitolari.Substring(1);
                
                FiltroRicerca fTemp = new FiltroRicerca();
                fTemp.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.ID_TITOLARIO.ToString();
                fTemp.valore = listaTitolari;
                filtriFasc.Add(fTemp);

                Ruolo AdlRuolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(iu.idGruppo);

                if (inAdl)
                {
                    FiltroRicerca fAdldoc = new FiltroRicerca();
                    fAdldoc.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.DOC_IN_FASC_ADL.ToString();
                    fAdldoc.valore = String.Format("{0}@{1}", iu.idPeople, iu.idCorrGlobali);
                    filtriFasc.Add(fAdldoc);
                }
                //Filtro ricerca temporale -1anno
                FiltroRicerca fDataPrec = new FiltroRicerca();
                FiltroRicerca fDataSuccAl = new FiltroRicerca();
                if (!string.IsNullOrWhiteSpace(request.DataDa) && !string.IsNullOrWhiteSpace(request.DataA))
                {
                    logger.DebugFormat("Presenti i valori DataDa {0} e DataA {1}", request.DataDa, request.DataA);
                    fDataPrec.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString();
                    fDataPrec.valore = request.DataA;

                    fDataSuccAl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString();
                    fDataSuccAl.valore = request.DataDa;
                }
                else if (!string.IsNullOrWhiteSpace(request.DataDa))
                {
                    logger.DebugFormat("Presenti il valore DataDa {0}", request.DataDa);

                    fDataSuccAl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString();
                    fDataSuccAl.valore = request.DataDa;
                    fDataPrec.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString();
                    fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    fDataPrec.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString();
                    fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
                    fDataSuccAl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString();
                    fDataSuccAl.valore = DateTime.Now.AddMonths(-6).Date.ToString("dd/MM/yyyy");
                }
                filtriFasc.Add(fDataPrec);
                filtriFasc.Add(fDataSuccAl);
                            

                return filtriFasc.ToArray();
            }
        }

        #region Strategies per le ricerche salvate
        internal class RicercaSalvataFascicoloStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaSalvataFascicoloStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return !string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicercaSalvata == RicercaSalvataType.RIC_FASCICOLO) && string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                logger.Debug("ricerca di tipo fascicolo");
                int numRec;
                int numTotPage;
                SearchItem si = BusinessLogic.Documenti.InfoDocManager.GetSearchItem(Int32.Parse(request.IdRicercaSalvata));
                FiltroRicerca[][] filtri = getFiltri(si.filtri);

                List<SearchResultInfo> idProjectList = null;

                ArrayList result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtri[0], request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                if (result != null && result.Count > 0)
                {
                    foreach (Fascicolo temp in result)
                    {
                        response.Risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
            }
        }

        internal class RicercaSalvataDocumentoStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaSalvataDocumentoStrategy));
            public override bool canSolve(RicercaRequest request)
            {
                return !string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicercaSalvata == RicercaSalvataType.RIC_DOCUMENTO);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                int numRec;
                int numTotPage;
                List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();
                SearchItem si = BusinessLogic.Documenti.InfoDocManager.GetSearchItem(Int32.Parse(request.IdRicercaSalvata));
                FiltroRicerca[][] filtri = getFiltri(si.filtri);
                ArrayList result = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(iu.idGruppo, iu.idPeople, filtri, false, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                if (result != null && result.Count > 0)
                {
                    foreach (InfoDocumento temp in result)
                    {
                        response.Risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
            }

        }
        #endregion
        #region Strategies per le ricerche testuali
        internal class RicercaTestoFascicoloStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaTestoFascicoloStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_FASCICOLO) && string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                logger.Debug("ricerca di fascicolo per testo");
                int numRec;
                int numTotPage;


                FiltroRicerca[] filtri = getFiltriFasc(request, iu, false);
                    //filtri[0] = new FiltroRicerca();
                    //filtri[0].argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.TITOLO.ToString();
                    //filtri[0].valore = request.Text;



                    //Filtro ricerca temporale -1anno
                    /*FiltroRicerca fascDataPrec = new FiltroRicerca();
                    fascDataPrec.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString();
                    fascDataPrec.valore = DateTime.Now.Date.AddDays(1).ToShortDateString();
                    FiltroRicerca fascDataSuccAl = new FiltroRicerca();
                    fascDataSuccAl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString();
                    fascDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToShortDateString();

                    filtri[1] = fascDataSuccAl;
                    filtri[2] = fascDataPrec;
                    */


                    List<SearchResultInfo> idProjectList = null;
                    logger.Debug("testo da cercare: " + request.Text);
                    ArrayList result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtri, request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
                    logger.Debug("numero totale risultati: " + numRec);
                    response.TotalRecordCount = numRec;
                    response.Risultati = new List<RicercaElement>();
                    if (result != null && result.Count > 0)
                    {
                        foreach (Fascicolo temp in result)
                        {
                            response.Risultati.Add(RicercaElement.buildInstance(temp));
                        }
                    }
                
            }
        }

        internal class RicercaAdlTestoFascicoloStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaAdlTestoFascicoloStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_FASCICOLO_ADL) && string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                logger.Debug("ricerca in Adl di fascicolo per testo");
                int numRec;
                int numTotPage;

                FiltroRicerca[] filtri = getFiltriFasc(request, iu, true);

                List<SearchResultInfo> idProjectList = null;
                logger.Debug("testo da cercare: " + request.Text);
                ArrayList result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtri, request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
                logger.Debug("numero totale risultati: " + numRec);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                if (result != null && result.Count > 0)
                {
                    foreach (Fascicolo temp in result)
                    {
                        response.Risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
            }
        }

        internal class RicercaTestoDocumentoStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaTestoDocumentoStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_DOCUMENTO) && string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                logger.Debug("ricerca di documento per testo");
                int numRec;
                int numTotPage;


                //if (string.IsNullOrEmpty(request.Text))
                //{
                //    response.TotalRecordCount = 0;
                //    response.Risultati = new List<RicercaElement>();
                //}
                //else
                //{
                FiltroRicerca[][] filtriArray = new FiltroRicerca[1][];
                //    List<FiltroRicerca> filtri = new List<FiltroRicerca>(); ;
                //    if (!string.IsNullOrEmpty(request.Text))
                //    {
                //        FiltroRicerca fOgg = new FiltroRicerca();
                //        fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                //        fOgg.valore = request.Text;//
                //        filtri.Add(fOgg);
                //        /*
                //        FiltroRicerca fOggAnno = new FiltroRicerca();
                //        fOggAnno.argomento = listaArgomenti.ANNO_PROTOCOLLO.ToString();
                //        fOggAnno.valore = System.DateTime.Now.Year.ToString();
                //        filtri.Add(fOggAnno);
                //         */
                //    }
                //    listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                //    listaArgomenti.PROT_ARRIVO,
                //    listaArgomenti.PROT_PARTENZA,
                //    listaArgomenti.PROT_INTERNO,
                //    listaArgomenti.GRIGIO,
                //    listaArgomenti.PREDISPOSTO
                //};
                //    foreach (listaArgomenti arg in defaultArgomenti)
                //    {
                //        FiltroRicerca temp = new FiltroRicerca();
                //        temp.argomento = arg.ToString();
                //        temp.valore = "true";
                //        filtri.Add(temp);
                //    }


                //    //Filtro ricerca temporale -1anno
                //    FiltroRicerca fDataPrec = new FiltroRicerca();
                //    fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                //    fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
                //    FiltroRicerca fDataSuccAl = new FiltroRicerca();
                //    fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                //    fDataSuccAl.valore = DateTime.Now.AddDays(-31).Date.ToString("dd/MM/yyyy");


                //    filtri.Add(fDataPrec);
                //    filtri.Add(fDataSuccAl);

                //    filtriArray[0] = filtri.ToArray();
                filtriArray[0] = getFiltriDoc(request, iu, false);
                List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();
                ArrayList result = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(iu.idGruppo, iu.idPeople, filtriArray, false, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                if (result != null && result.Count > 0)
                {
                    foreach (InfoDocumento temp in result)
                    {
                        response.Risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
                //}
            }
        }


        internal class RicercaAdlTestoDocumentoStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaAdlTestoDocumentoStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_DOCUMENTO_ADL) && string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                logger.Debug("ricerca in Adl di documento per testo");
                int numRec;
                int numTotPage;
                FiltroRicerca[][] filtriArray = new FiltroRicerca[1][];
                //    List<FiltroRicerca> filtri = new List<FiltroRicerca>(); ;
                //    if (!string.IsNullOrEmpty(request.Text))
                //    {
                //        FiltroRicerca fOgg = new FiltroRicerca();
                //        fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                //        fOgg.valore = request.Text;//
                //        filtri.Add(fOgg);
                //        /*
                //        FiltroRicerca fOggAnno = new FiltroRicerca();
                //        fOggAnno.argomento = listaArgomenti.ANNO_PROTOCOLLO.ToString();
                //        fOggAnno.valore = System.DateTime.Now.Year.ToString();
                //        filtri.Add(fOggAnno);
                //         */
                //    }
                //    listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                //    listaArgomenti.PROT_ARRIVO,
                //    listaArgomenti.PROT_PARTENZA,
                //    listaArgomenti.PROT_INTERNO,
                //    listaArgomenti.GRIGIO,
                //    listaArgomenti.PREDISPOSTO
                //};
                //    foreach (listaArgomenti arg in defaultArgomenti)
                //    {
                //        FiltroRicerca temp = new FiltroRicerca();
                //        temp.argomento = arg.ToString();
                //        temp.valore = "true";
                //        filtri.Add(temp);
                //    }

                //    //Per l'adl magari non mettiamo il limete a un anno
                //    /*
                //    //Filtro ricerca temporale -1anno
                //    FiltroRicerca fDataPrec = new FiltroRicerca();
                //    fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                //    fDataPrec.valore = DateTime.Now.Date.ToShortDateString();
                //    FiltroRicerca fDataSuccAl = new FiltroRicerca();
                //    fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                //    fDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToShortDateString();


                //    filtri.Add(fDataPrec);
                //    filtri.Add(fDataSuccAl);
                //    */

                //    //Filtro ADL
                //    //            DOC_IN_ADL
                //    //String.Format("{0}@{1}", infoUt.idPeople, ruolo.systemId),
                //    FiltroRicerca fAdldoc = new FiltroRicerca();
                //    fAdldoc.argomento = listaArgomenti.DOC_IN_ADL.ToString();
                //    Ruolo AdlRuolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(iu.idGruppo);
                //    fAdldoc.valore = String.Format("{0}@{1}", iu.idPeople,iu.idCorrGlobali);
                //    filtri.Add(fAdldoc);

                //    //Filtro per Registro attivo:
                //    string listaRegistri = string.Empty;
                //    foreach (DocsPaVO.utente.Registro reg in AdlRuolo.registri)
                //    {
                //        if (!reg.flag_pregresso)
                //        {
                //            listaRegistri += "," + reg.systemId.ToString();
                //        }

                //    }
                //    listaRegistri = listaRegistri.Substring(1);


                //    FiltroRicerca fAdlReg = new FiltroRicerca();
                //    fAdlReg.argomento = listaArgomenti.REGISTRO.ToString();
                //    fAdlReg.valore = listaRegistri;
                //    filtri.Add(fAdlReg);

                //    bool filtroData = false;
                //    if (!string.IsNullOrWhiteSpace(request.DataA) && !string.IsNullOrWhiteSpace(request.DataDa))
                //    {
                //        filtroData = true;
                //        FiltroRicerca filtrodatada = new FiltroRicerca();
                //        filtrodatada.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                //        filtrodatada.valore = request.DataDa;
                //        // +" 00:00:00";
                //        filtri.Add(filtrodatada);

                //        FiltroRicerca filtrodataa = new FiltroRicerca();
                //        filtrodataa.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                //        filtrodataa.valore = request.DataA;
                //        // +" 23:59:59";
                //        filtri.Add(filtrodataa);
                //    }

                //    filtriArray[0] = filtri.ToArray();
                filtriArray[0] = getFiltriDoc(request, iu, true);
                List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();
                ArrayList result = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(iu.idGruppo, iu.idPeople, filtriArray, false, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                if (result != null && result.Count > 0)
                {
                    foreach (InfoDocumento temp in result)
                    {
                        response.Risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
            }
        }



        internal class RicercaTestoDocFascStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaTestoDocFascStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_DOC_FASC) && string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                int numRec;
                int numTotPage;

                logger.Debug("ricerca di documenti e fascicoli per testo");
                List<RicercaElement> risultati = new List<RicercaElement>();
                FiltroRicerca[] filtriFasc = getFiltriFasc(request, iu, false);
                logger.Debug("testo da cercare: " + request.Text);
                List<SearchResultInfo> idProjectList = null;
                //ArrayList resultFasc = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtriFasc, request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
                bool saltaFascicoli = false;
                if (!string.IsNullOrWhiteSpace(request.IdDocumento) ||
                    !string.IsNullOrWhiteSpace(request.NumProto) ||
                    !string.IsNullOrWhiteSpace(request.DataProtoA) ||
                    !string.IsNullOrWhiteSpace(request.DataProtoDa))
                    saltaFascicoli = true;
                if (!saltaFascicoli)
                {
                    ArrayList resultFasc = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoli(iu, null, filtriFasc, request.EnableUfficioRef, request.EnableProfilazione, true, null, null, null);
                    logger.Debug("numero totale risultati: " + resultFasc.Count);
                    foreach (Fascicolo temp in resultFasc)
                    {
                        risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
                FiltroRicerca[][] filtriArrayDoc = new FiltroRicerca[1][];
                //    List<FiltroRicerca> filtriDoc = new List<FiltroRicerca>(); ;

                //    if (!string.IsNullOrEmpty(request.Text))
                //    {
                //        FiltroRicerca fOgg = new FiltroRicerca();
                //        fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                //        fOgg.valore = request.Text;
                //        filtriDoc.Add(fOgg);
                //    }
                //    listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                //    listaArgomenti.PROT_ARRIVO,
                //    listaArgomenti.PROT_PARTENZA,
                //    listaArgomenti.PROT_INTERNO,
                //    listaArgomenti.GRIGIO,
                //    listaArgomenti.PREDISPOSTO
                //};
                //    foreach (listaArgomenti arg in defaultArgomenti)
                //    {
                //        FiltroRicerca temp = new FiltroRicerca();
                //        temp.argomento = arg.ToString();
                //        temp.valore = "true";
                //        filtriDoc.Add(temp);
                //    }


                //    //Filtro ricerca temporale -1anno
                //    FiltroRicerca fDataPrec = new FiltroRicerca();
                //    FiltroRicerca fDataSuccAl = new FiltroRicerca();
                //    if (!string.IsNullOrWhiteSpace(request.DataDa) && !string.IsNullOrWhiteSpace(request.DataA))
                //    {
                //        fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                //        fDataPrec.valore = request.DataA;

                //        fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                //        fDataSuccAl.valore = request.DataDa;
                //    }
                //    else
                //    {
                //        fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                //        fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
                //        fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                //        fDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToString("dd/MM/yyyy");
                //    }
                //    filtriDoc.Add(fDataPrec);
                //    filtriDoc.Add(fDataSuccAl);
                //    List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();

                //    filtriArrayDoc[0] = filtriDoc.ToArray();
                filtriArrayDoc[0] = getFiltriDoc(request, iu, false);
                //ArrayList resultDoc = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(request.IdGruppo, request.UserInfo.IdPeople, filtriArrayDoc, true, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
                ArrayList resultDoc = BusinessLogic.Documenti.InfoDocManager.getQuery(iu.idGruppo, iu.idPeople, filtriArrayDoc);
                if (resultDoc != null && resultDoc.Count > 0)
                {
                    foreach (InfoDocumento temp in resultDoc)
                    {
                        risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
                risultati.Sort(new RicercaElementComparer());
                PaginatorDecorator<RicercaElement> pag = new PaginatorDecorator<RicercaElement>(request.RequestedPage, request.PageSize, risultati);
                response.Risultati = pag.execute();
                response.TotalRecordCount = pag.TotalResultCount;
            }
        }

        internal class RicercaAdlDocFascStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(RicercaAdlDocFascStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_DOC_FASC_ADL) && string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                logger.Debug("ricerca in Adl di fascicolo per testo");
                int numRec;
                int numTotPage;
                FiltroRicerca[] filtri = getFiltriFasc(request, iu, true);

                List<SearchResultInfo> idProjectList = null;
                logger.Debug("testo da cercare: " + request.Text);
                ArrayList result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtri, request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
                logger.Debug("numero totale risultati: " + numRec);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                if (result != null && result.Count > 0)
                {
                    foreach (Fascicolo temp in result)
                    {
                        response.Risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }

                logger.Debug("ricerca in Adl di documento per testo");
                FiltroRicerca[][] filtriArray = new FiltroRicerca[1][];
                //    List<FiltroRicerca> filtriDoc = new List<FiltroRicerca>(); ;
                //    if (!string.IsNullOrEmpty(request.Text))
                //    {
                //        FiltroRicerca fOgg = new FiltroRicerca();
                //        fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                //        fOgg.valore = request.Text;//
                //        filtriDoc.Add(fOgg);
                //        /*
                //        FiltroRicerca fOggAnno = new FiltroRicerca();
                //        fOggAnno.argomento = listaArgomenti.ANNO_PROTOCOLLO.ToString();
                //        fOggAnno.valore = System.DateTime.Now.Year.ToString();
                //        filtri.Add(fOggAnno);
                //         */
                //    }
                //    listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                //    listaArgomenti.PROT_ARRIVO,
                //    listaArgomenti.PROT_PARTENZA,
                //    listaArgomenti.PROT_INTERNO,
                //    listaArgomenti.GRIGIO,
                //    listaArgomenti.PREDISPOSTO
                //};
                //    foreach (listaArgomenti arg in defaultArgomenti)
                //    {
                //        FiltroRicerca temp = new FiltroRicerca();
                //        temp.argomento = arg.ToString();
                //        temp.valore = "true";
                //        filtriDoc.Add(temp);
                //    }

                //    FiltroRicerca fAdldoc = new FiltroRicerca();
                //    fAdldoc.argomento = listaArgomenti.DOC_IN_ADL.ToString();
                //    Ruolo AdlRuolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(iu.idGruppo);
                //    fAdldoc.valore = String.Format("{0}@{1}", iu.idPeople, iu.idCorrGlobali);
                //    filtriDoc.Add(fAdldoc);

                //    //Filtro per Registro attivo:
                //    string listaRegistri = string.Empty;
                //    foreach (DocsPaVO.utente.Registro reg in AdlRuolo.registri)
                //    {
                //        if (!reg.flag_pregresso)
                //        {
                //            listaRegistri += "," + reg.systemId.ToString();
                //        }

                //    }
                //    listaRegistri = listaRegistri.Substring(1);


                //    FiltroRicerca fAdlReg = new FiltroRicerca();
                //    fAdlReg.argomento = listaArgomenti.REGISTRO.ToString();
                //    fAdlReg.valore = listaRegistri;
                //    filtriDoc.Add(fAdlReg);
                //    if (!string.IsNullOrWhiteSpace(request.DataA) && !string.IsNullOrWhiteSpace(request.DataDa))
                //    {
                //        filtroData = true;
                //        FiltroRicerca filtrodatada = new FiltroRicerca();
                //        filtrodatada.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                //        filtrodatada.valore = request.DataDa;
                //        // +" 00:00:00";
                //        filtriDoc.Add(filtrodatada);

                //        FiltroRicerca filtrodataa = new FiltroRicerca();
                //        filtrodataa.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                //        filtrodataa.valore = request.DataA;
                //        // +" 23:59:59";
                //        filtriDoc.Add(filtrodataa);
                //    }

                //    if (!string.IsNullOrWhiteSpace(request.DataProtA) && !string.IsNullOrWhiteSpace(request.DataProtDa))
                //    {
                //        filtroData = true;
                //        FiltroRicerca filtrodatada = new FiltroRicerca();
                //        filtrodatada.argomento = listaArgomenti.DATA_PROT_SUCCESSIVA_AL.ToString();
                //        filtrodatada.valore = request.DataDa;
                //        // +" 00:00:00";
                //        filtriDoc.Add(filtrodatada);

                //        FiltroRicerca filtrodataa = new FiltroRicerca();
                //        filtrodataa.argomento = listaArgomenti.DATA_PROT_PRECEDENTE_IL.ToString();
                //        filtrodataa.valore = request.DataA;
                //        // +" 23:59:59";
                //        filtriDoc.Add(filtrodataa);
                //    }

                //    if (!string.IsNullOrWhiteSpace(request.IdDocumento))
                //    {
                //        FiltroRicerca filtroId = new FiltroRicerca();
                //        filtroId.argomento = listaArgomenti.DOCNUMBER.ToString();
                //        filtroId.valore = request.IdDocumento;
                //        filtriDoc.Add(filtroId);
                //    }

                //    if(!string.IsNullOrWhiteSpace(request.NumProto))
                //    {
                //        FiltroRicerca filtroNumProto = new FiltroRicerca();
                //        filtroNumProto.argomento = listaArgomenti.NUM_PROTOCOLLO.ToString();
                //        filtroNumProto.valore = request.NumProto;
                //        filtriDoc.Add(filtroNumProto);
                //    }

                //    filtriArray[0] = filtriDoc.ToArray();
                filtriArray[0] = getFiltriDoc(request, iu, true);
                List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();
                ArrayList resultDoc = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(iu.idGruppo, iu.idPeople, filtriArray, false, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
                response.TotalRecordCount += numRec;
                if (resultDoc != null && resultDoc.Count > 0)
                {
                    foreach (InfoDocumento temp in resultDoc)
                    {
                        response.Risultati.Add(RicercaElement.buildInstance(temp));
                    }
                }
            }
        }
        #endregion

        internal class FolderContentStrategy : RicercaStrategy
        {
            private ILog logger = LogManager.GetLogger(typeof(FolderContentStrategy));

            public override bool canSolve(RicercaRequest request)
            {
                return !string.IsNullOrEmpty(request.ParentFolderId);
            }

            public override void buildResponse(InfoUtente iu, RicercaRequest request, RicercaResponse response)
            {
                logger.Debug("contenuto del folder con id " + request.ParentFolderId);
                FascicoloContentDecorator<RicercaElement> fcDec = new FascicoloContentDecorator<RicercaElement>(iu.idPeople, iu.idGruppo, request.ParentFolderId, request.FascId, request.Text, RicercaElement.buildInstance, RicercaElement.buildInstance);
                PaginatorDecorator<RicercaElement> pag = new PaginatorDecorator<RicercaElement>(request.RequestedPage, request.PageSize, fcDec);
                response.Risultati = pag.execute();
                response.TotalRecordCount = pag.TotalResultCount;
            }
        }

        internal class RicercaElementComparer : IComparer<RicercaElement>
        {

            public int Compare(RicercaElement x, RicercaElement y)
            {
                //if (x.Oggetto == null) return -1;
                //if (y.Oggetto == null) return 1;
                //return x.Oggetto.CompareTo(y.Oggetto);
                return y.Id.CompareTo(x.Id);
            }
        }
    }
}