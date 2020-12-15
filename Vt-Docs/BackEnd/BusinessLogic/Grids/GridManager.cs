using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Grid;
using DocsPaVO.utente;

namespace BusinessLogic.Grids
{
    /// <summary>
    ///  Questa classe si occupa di gestire le griglie di ricerca.
    /// </summary>
    public class GridManager
    {
        /// <summary>
        /// Funzione per il caricamento delle informazioni su una griglia.
        /// </summary>
        /// <param name="gridId">Identificativo della griglia da caricare (null se si desidera caricare la griglia standard)</param>
        /// <param name="gridType">Tipo di ricerca in cui è inserita a griglia</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        /// <param name="role">Ruolo dell'utente proprietario della griglia</param>
        /// <param name="templateId">Identificativo del template (null se non c'è template associato)</param>
        /// <returns>Le informazioni sulla griglia richiesta</returns>
        public static Grid LoadGrid(
            String gridId,
            Grid.GridTypeEnumeration gridType,
            InfoUtente userInfo,
            Ruolo role,
            List<String> templatesId)
        {
            return new DocsPaDB.Query_DocsPAWS.Grids().LoadGrid(
                gridId,
                gridType,
                userInfo,
                templatesId);

        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni su una griglia.
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        /// <param name="role">Ruolo dell'utente proprietario della griglia</param>
        /// <param name="gridName">Nome da assegnare alla griglia</param>
        /// <param name="isActive">checked se è attiva</param>
        /// <param name="isTemporary">True se p temporanea</param>
        /// <returns>System id della griglia salvata</returns>
        public static String SaveGrid(
            Grid grid,
            InfoUtente userInfo,
            Ruolo role,
            String gridName,
            Boolean isTemporary,
            String isActive, String visibility)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                String toReturn = new DocsPaDB.Query_DocsPAWS.Grids().SaveGrid(
                    grid,
                    userInfo.idPeople,
                    userInfo.idGruppo,
                    userInfo.idAmministrazione,
                    gridName,
                    isTemporary,
                    isActive, visibility);
                transactionContext.Complete();

                return toReturn;
            }
        }

        public static Grid GetStandardGridForUser(InfoUtente userInfo, Grid.GridTypeEnumeration gridType)
        {
            return new DocsPaDB.Query_DocsPAWS.Grids().GetStandardGridForUser(userInfo, gridType);
        }


        public static Grid GetGridFromSearchId(InfoUtente userInfo, string searchId, Grid.GridTypeEnumeration gridType)
        {
            return new DocsPaDB.Query_DocsPAWS.Grids().GetGridFromSearchId(userInfo, searchId, gridType);
        }

        public static Grid GetEmergencyGrid(Grid.GridTypeEnumeration gridType)
        {
            return new DocsPaDB.Query_DocsPAWS.Grids().GetEmergencyGrid(gridType);
        }

        public static bool ExistGridPersonalizationFunction()
        {
            return new DocsPaDB.Query_DocsPAWS.Grids().ExistGridPersonalizationFunction();
        }
        /*
                /// <summary>
                /// Funzione per il restore della griglia standard relativa ad un utente per una specifica tipologia di ricerca
                /// </summary>
                /// <param name="userId">Identificativo dell'utente per cui ripristinare la griglia</param>
                /// <param name="roleId">Id del ruolo proprietario della griglia</param>
                /// <param name="administrationId">Identificativo dell'amministrazione cui appartiene l'utente</param>
                /// <param name="gridType">Tipo di griglia da restituire</param>
                /// <returns>Griglia standard</returns>
                public static Grid RestoreStandardGridForUser(String userId, String roleId, String administrationId, Grid.GridTypeEnumeration gridType)
                {
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        Grid toReturn = new DocsPaDB.Query_DocsPAWS.Grids().RestoreStandardGridForUser(userId, roleId, administrationId, gridType);
                        transactionContext.Complete();

                        return toReturn;
                    }
                }
        */
        /// <summary>
        /// Funzione per il restore della griglia standard relativa ad un utente per una specifica tipologia di ricerca
        /// </summary>
        /// <param name="gridId">Id della griglia da rimuovere</param>
        public static void RemoveGrid(String gridId)
        {
            new DocsPaDB.Query_DocsPAWS.Grids().RemoveGrid(gridId);
        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni di base relative alle griglie salvate per un dato utente
        /// appartenente ad un dato ruolo definito per una certa amministrazione e per un particolare tipo di ricerca
        /// </summary>
        /// <param name="userId">Identificativo dell'utente</param>
        /// <param name="roleId">Identificativo del ruolo</param>
        /// <param name="administrationId">Identificativo dell'amministrazione</param>
        /// <param name="gridType">Tipo di griglia</param>
        /// <returns>Lista di oggetti con le informazioni di base sulle griglie  definite da un utente</returns>
        public static List<GridBaseInfo> GetGridsBaseInfo(DocsPaVO.utente.InfoUtente infoUtente, Grid.GridTypeEnumeration gridType, bool allGrids)
        {
            List<GridBaseInfo> grids = new List<GridBaseInfo>();

            using (DocsPaDB.Query_DocsPAWS.Grids gridProvider = new DocsPaDB.Query_DocsPAWS.Grids())
            {
                grids = gridProvider.GetGridsBaseInfo(infoUtente, gridType, grids, allGrids);

            }

            return grids;

        }

        public static Grid GetUserGridCustom(InfoUtente userInfo, Grid.GridTypeEnumeration gridType)
        {
            return new DocsPaDB.Query_DocsPAWS.Grids().GetUserGridCustom(userInfo, gridType);
        }

        public static void RemovePreferred(string userSystemId, string roleId, string administrationId, Grid.GridTypeEnumeration gridType)
        {

        }

        /// <summary>
        /// Funzione per la cancellazione di una griglia utente
        /// </summary>
        /// <param name="gridId">Id della griglia da rimuovere</param>
        public static bool RemoveUserGrid(DocsPaVO.Grid.GridBaseInfo gridBase, DocsPaVO.utente.InfoUtente userInfo)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Grids conn = new DocsPaDB.Query_DocsPAWS.Grids();
                result = conn.RemovePreferredGrid(gridBase, userInfo);
                result = conn.RemoveUserGrid(gridBase, userInfo);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni su una griglia.
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        /// <param name="role">Ruolo dell'utente proprietario della griglia</param>
        /// <param name="gridName">Nome da assegnare alla griglia</param>
        /// <param name="isActive">checked se è attiva</param>
        /// <param name="isTemporary">True se p temporanea</param>
        /// <returns>System id della griglia salvata</returns>
        public static String SaveNewGrid(
            Grid grid,
            InfoUtente userInfo,
            String gridName,
            String visibility,
            Boolean isPreferred)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                String toReturn = new DocsPaDB.Query_DocsPAWS.Grids().SaveNewGrid(
                    grid,
                    userInfo,
                    gridName,
                    visibility,
                    isPreferred);
                transactionContext.Complete();

                return toReturn;
            }
        }

        /// <summary>
        /// Funzione per la rimozione della griglia preferita
        /// </summary>
        public static void RemovePreferredTypeGrid(
            InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                new DocsPaDB.Query_DocsPAWS.Grids().RemovePreferredTypeGrid(
                    infoUser, gridType);
                transactionContext.Complete();

            }
        }

        /// <summary>
        /// Funzione per la rimozione della griglia preferita
        /// </summary>
        public static void AddPreferredGrid(String gridId,
            InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                new DocsPaDB.Query_DocsPAWS.Grids().AddPreferredGrid(gridId,
                    infoUser, gridType);
                transactionContext.Complete();

            }
        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni su una griglia.
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        /// <param name="role">Ruolo dell'utente proprietario della griglia</param>
        /// <param name="gridName">Nome da assegnare alla griglia</param>
        /// <param name="isActive">checked se è attiva</param>
        /// <param name="isTemporary">True se p temporanea</param>
        /// <returns>System id della griglia salvata</returns>
        public static String ModifyGrid(
            Grid grid,
            InfoUtente userInfo,
            String visibility,
            Boolean isPreferred)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                String toReturn = new DocsPaDB.Query_DocsPAWS.Grids().ModifyGrid(
                    grid,
                    userInfo,
                    visibility,
                    isPreferred);
                transactionContext.Complete();

                return toReturn;
            }
        }

        /// <summary>
        /// Salvataggio di una griglia temporanea da salvataggio ricerca veloce
        /// </summary>
        /// <returns>System id della griglia salvata</returns>
        public static String SaveTempGridRapidSearch(
            Grid grid,
            InfoUtente userInfo,
            String gridName, Grid.GridTypeEnumeration gridType)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                String toReturn = new DocsPaDB.Query_DocsPAWS.Grids().SaveNewGridSearch(
                    grid,
                    userInfo,
                    gridName, gridType);
                transactionContext.Complete();

                return toReturn;
            }
        }

        /// <summary>
        /// Aggiorna tutte le griglie presenti
        /// </summary>
        public static void refreshAllGrid()
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                new DocsPaDB.Query_DocsPAWS.Grids().refreshAllGrid();
                transactionContext.Complete();

            }
        }

    }
}