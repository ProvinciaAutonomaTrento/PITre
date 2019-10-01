using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace BusinessLogic.ProfilazioneAvanzata
{
    public sealed class ProfilazioneAvanzata
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="profilazioneDinamicaLite"></param>
        /// <returns></returns>
        private static DocsPaVO.ProfilazioneDinamica.Templates getTemplateCompleto(DocsPaVO.utente.InfoUtente infoUtente, 
            DocsPaVO.RicercaLite.CampiProfilati profilazioneDinamicaLite)
        {
            
            DocsPaVO.ProfilazioneDinamica.Templates templates = null;
            ArrayList tempTemplas = null;
            if (profilazioneDinamicaLite != null)
            {
                 tempTemplas = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplates(infoUtente.idAmministrazione);
                 foreach (DocsPaVO.ProfilazioneDinamica.Templates templates1 in tempTemplas)
                 {
                     if (templates1.DESCRIZIONE.ToUpper().Equals(profilazioneDinamicaLite.nomeDocumento.ToUpper()))
                     {
                         templates = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(templates1.SYSTEM_ID.ToString());
                         break;
                     }

                 }
            }

            if (templates != null)
            {
                foreach (DocsPaVO.RicercaLite.CampoProfilatoAvanzata campi in profilazioneDinamicaLite.campiProfilati)
                {
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in templates.ELENCO_OGGETTI)
                    {
                        if(oggetto.DESCRIZIONE.ToUpper().Equals(campi.nomeCampo.ToUpper()))
                        {
                            if(campi.IsIntervalloDa ==1)
                                oggetto.VALORE_DATABASE = campi.valoreCampo;
                            else
                                if (campi.IsIntervalloA == 1)
                                    oggetto.VALORE_DATABASE += "@"+campi.valoreCampo;
                                else
                                    oggetto.VALORE_DATABASE = campi.valoreCampo;

                            break;
                        }
                    }
                }
            }

            return templates;
        }

        

        private static DocsPaVO.filtri.FiltroRicerca[] getFiltriProfilazioneDimanica(DocsPaVO.ProfilazioneDinamica.Templates templates)
        {
            List<DocsPaVO.filtri.FiltroRicerca> lista = new List<DocsPaVO.filtri.FiltroRicerca>();

            if (templates != null)
            {
                DocsPaVO.filtri.FiltroRicerca filtro = new DocsPaVO.filtri.FiltroRicerca();
                filtro.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO_ATTO.ToString();
                filtro.valore = templates.ID_TIPO_ATTO;
                lista.Add(filtro);

                filtro = new DocsPaVO.filtri.FiltroRicerca();
                filtro.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROFILAZIONE_DINAMICA.ToString();
                filtro.template = templates;
                filtro.valore = "Profilazione Dinamica";
                lista.Add(filtro);
            }

            return lista.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <param name="FiltriBase"></param>
        /// <param name="filtriProfilazioneDinamica"></param>
        /// <returns></returns>
        private static DocsPaVO.filtri.FiltroRicerca[][] getFiltriDocumenti( 
            DocsPaVO.filtri.FiltroRicerca[] filtriRicerca,
            DocsPaVO.filtri.FiltroRicerca[] filtriProfilazioneDinamica)
        {
            if(filtriRicerca == null)
                throw new Exception("Non è stato valorizzato nessun filtro. E' obbligatorio valorizzare il filtro TIPO");
            
            
            DocsPaVO.filtri.FiltroRicerca[][] listaFiltri = new DocsPaVO.filtri.FiltroRicerca[1][];
            listaFiltri[0] = new DocsPaVO.filtri.FiltroRicerca[1];
            DocsPaVO.filtri.FiltroRicerca[] fVList = new DocsPaVO.filtri.FiltroRicerca[filtriRicerca.Length+filtriProfilazioneDinamica.Length];
            int indice =0;

            bool ricercaBase = false;

            if (filtriRicerca != null && filtriRicerca.Length > 0)
            {
                for (int i = 0; i < filtriRicerca.Length; i++)
                {
                    if (filtriRicerca[i].argomento.Equals("TIPO"))
                        ricercaBase = true;

                    fVList[indice] = filtriRicerca[i];
                    indice++;
                }
            }

            if(!ricercaBase)
                throw new Exception("Non è stato valorizzato il filtro TIPO. E' obbligatorio valorizzare il filtro TIPO");

            if (filtriProfilazioneDinamica != null
             && filtriProfilazioneDinamica.Length > 0)
            {
                for (int i = 0; i < filtriProfilazioneDinamica.Length; i++)
                {
                    fVList[indice] = filtriProfilazioneDinamica[i];
                    indice++;
                }
            }

            listaFiltri[0] = fVList;

            return listaFiltri;
        }

        public static DocsPaVO.filtri.FiltroRicerca[][] ricercaProfilazioneAvanzata(
            DocsPaVO.utente.InfoUtente infoUtente, 
            DocsPaVO.filtri.FiltroRicerca[] filtroRicerca, 
            DocsPaVO.RicercaLite.CampiProfilati CampiProfilati)
        {
            DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca = null;
            DocsPaVO.ProfilazioneDinamica.Templates templates = BusinessLogic.ProfilazioneAvanzata.ProfilazioneAvanzata.getTemplateCompleto(infoUtente, CampiProfilati);
            DocsPaVO.filtri.FiltroRicerca[] filtriRicercaProfilazioneDinamica = BusinessLogic.ProfilazioneAvanzata.ProfilazioneAvanzata.getFiltriProfilazioneDimanica(templates);
            filtriRicerca = BusinessLogic.ProfilazioneAvanzata.ProfilazioneAvanzata.getFiltriDocumenti(filtroRicerca, filtriRicercaProfilazioneDinamica);

            if (CampiProfilati != null
               && !string.IsNullOrEmpty(CampiProfilati.nomeDocumento)
               && templates == null)
                filtriRicerca = null;

            return filtriRicerca;
        }

        public static List<DocsPaVO.RicercaLite.Profilo> getProfiloAvanzato(DocsPaVO.utente.InfoUtente infoUtente, ArrayList infoDocumento)
        {
            List<DocsPaVO.RicercaLite.Profilo> lista = new List<DocsPaVO.RicercaLite.Profilo>();

            foreach (DocsPaVO.documento.InfoDocumento infodoc in infoDocumento)
            {
                DocsPaVO.RicercaLite.Profilo profilo =
                new DocsPaVO.RicercaLite.Profilo()
                {
                    acquisitaImmagine = infodoc.acquisitaImmagine,
                    allegato = infodoc.allegato,
                    autore = infodoc.autore,
                    cha_firmato = infodoc.cha_firmato,
                    codRegistro = infodoc.codRegistro,
                    contatore = infodoc.contatore,
                    daProtocollare = infodoc.daProtocollare,
                    dataAnnullamento = infodoc.dataAnnullamento,
                    dataArchiviazione = infodoc.dataArchiviazione,
                    dataApertura = infodoc.dataApertura,
                    docNumber = infodoc.docNumber,
                    evidenza = infodoc.evidenza,
                    idProfile = infodoc.idProfile,
                    idRegistro = infodoc.idRegistro,
                    idTipoAtto = infodoc.idTipoAtto,
                    inADL = infodoc.inADL,
                    inArchivio = infodoc.inArchivio,
                    inCestino = infodoc.inCestino,
                    inConservazione = infodoc.inConservazione,
                    isCatenaTrasversale = infodoc.isCatenaTrasversale,
                    isRimovibile = infodoc.isRimovibile,
                    mittDest = infodoc.mittDest,
                    mittDoc = infodoc.mittDoc,
                    noteCestino = infodoc.noteCestino,
                    numProt = infodoc.numProt,
                    numSerie = infodoc.numSerie,
                    oggetto = infodoc.oggetto,
                    personale = infodoc.personale,
                    privato = infodoc.privato,
                    protocolloTitolario = infodoc.protocolloTitolario,
                    segnatura = infodoc.segnatura,
                    tipoAtto = infodoc.tipoAtto,
                    tipoProto = infodoc.tipoProto,
                    CampiProfilati = infodoc.CampiProfilati,
                    linkDocumento = string.Empty
                };
                string url = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_URL_WA");
                if (profilo != null
                    && !string.IsNullOrEmpty(url)
                    && !string.IsNullOrEmpty(profilo.docNumber)
                    && !string.IsNullOrEmpty(profilo.idProfile))
                    profilo.linkDocumento = String.Format(
                                "<a href='{0}/visualizzaLink.aspx?groupId={1}&docNumber={2}&idProfile={3}&numVersion='></a>",
                                url, infoUtente.idGruppo, profilo.docNumber, profilo.idProfile);
                lista.Add(profilo);
            }
            return lista;
        }

    }
}
