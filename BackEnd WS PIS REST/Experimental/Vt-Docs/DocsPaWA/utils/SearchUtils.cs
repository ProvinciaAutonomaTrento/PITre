using System;
using System.Linq;
using DocsPAWA.DocsPaWR;
using DocsPAWA.UserControls.ScrollElementsList;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.utils
{
    /// <summary>
    /// Questa classe fornisce funzionalità di supporto per le nuove ricerche.
    /// </summary>
    public class SearchUtils
    {
        /// <summary>
        /// Funzione per il reperimento dell'indice del documento di cui si sta visualizzando il dettaglio
        /// </summary>
        /// <param name="elementList">Lista dei documenti restituiti dalla ricerca</param>
        /// <returns>Indice del documento di cui si sta vedendo il dettaglio</returns>
        public static int GetIndexOfSelectedDocument(ObjScrollElementsList elementList)
        {
            // Indice da restituire
            int index = -1;

            string tipo = string.Empty;

            foreach (object obj in elementList.objList)
            {
                if (obj.GetType() == typeof(SearchObject))
                {
                    tipo = "SearchObject";
                    break;
                }
                else
                {
                    tipo = "InfoDocumento";
                    break;
                }
            }

            if (tipo.Equals("SearchObject"))
            {
                // Recupero della lista dei documenti restituiti dalla ricerca
                SearchObject[] result = (SearchObject[])elementList.objList.ToArray(typeof(SearchObject));

                // Ricerca dell'oggetto InfoDocumento con id profile uguale a quello selezionato
                SearchObject document = result.Where(e => e.SearchObjectID ==
                    DocumentManager.getRisultatoRicerca(null).idProfile).FirstOrDefault();

                // Se il documento è stato trovato, viene calcolato l'indice occupato
                // dal documento nell'array
                if (document != null)
                    index = Array.IndexOf(result, document);
            }
            else
            {
                // Recupero della lista dei documenti restituiti dalla ricerca
                InfoDocumento[] result = (InfoDocumento[])elementList.objList.ToArray(typeof(InfoDocumento));

                // Ricerca dell'oggetto InfoDocumento con id profile uguale a quello selezionato
                InfoDocumento document = result.Where(e => e.idProfile ==
                    DocumentManager.getRisultatoRicerca(null).idProfile).FirstOrDefault();

                // Se il documento è stato trovato, viene calcolato l'indice occupato
                // dal documento nell'array
                if (document != null)
                    index = Array.IndexOf(result, document);
            }
          


            // Restituzione dell'indice
            return index;
        }

        /// <summary>
        /// Funzione per il reperimento dell'indice del fascicolo di cui si sta visualizzando il dettaglio
        /// </summary>
        /// <param name="elementList">Lista dei fascicoli restituiti dalla ricerca</param>
        /// <returns>Indice del fascicolo di cui si sta vedendo il dettaglio</returns>
        public static int GetIndexOfSelectedProject(ObjScrollElementsList elementList)
        {
            // Valore da restituire
            int index = -1;

            // Recupero della lista dei fascicolo restituiti dalla ricerca
            SearchObject[] result = (SearchObject[])elementList.objList.ToArray(typeof(SearchObject));

            // Ricerca dell'oggetto Fascicolo con id uguale a quello selezionato
            SearchObject project = result.Where(e => e.SearchObjectID ==
                FascicoliManager.getFascicoloSelezionato().systemID).FirstOrDefault();

            // Se il documento è valorizzato viene calcolato l'indice occupato
            // dal fascicolo all'interno dell'array
            if (project != null)
                index = Array.IndexOf(result, project);

            // Restituzione dell'indice calcolato
            return index;
 
        }

        /// <summary>
        /// Funzione per il salvataggio dell'identificativo dell'oggetto selezionato
        /// </summary>
        /// <param name="objectId">Identificativo dell'oggetto selezionato</param>
        public static void SetObjectId(String objectId)
        {
            CallContextStack.CallerContext.ContextState["ObjectId"] = objectId;
        }

        /// <summary>
        /// Funzione per il reperimento dell'identificativo dell'oggetto selezionato
        /// </summary>
        /// <returns>System id dell'oggetto selezionato</returns>
        public static String GetObjectId()
        {
            // Valore da restituire
            String toReturn = String.Empty;

            if (CallContextStack.CurrentContext.ContextState.ContainsKey("ObjectId"))
            {
                toReturn = CallContextStack.CurrentContext.ContextState["ObjectId"].ToString();

                // Rimozione dell'identificativo dal context state
                CallContextStack.CurrentContext.ContextState.Remove("ObjectId");
            }
            return toReturn;
        }
    }
}
