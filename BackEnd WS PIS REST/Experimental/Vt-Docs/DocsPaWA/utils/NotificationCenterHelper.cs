using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPAWA.NotificationCenterReference;
using System.Web.UI;

namespace DocsPAWA.utils
{
    public class NotificationCenterHelper
    {
        /// <summary>
        /// Metodo per il caricamento degli item non visualizzati da un particolare utente.
        /// </summary>
        /// <param name="administrationId">Id dell'amministrazione cui appartiene l'utente</param>
        /// <param name="administrationCode">Codice dell'amministrazione cui appartiene l'utente</param>
        /// <param name="userId">Id dell'utente per cui caricare gli item</param>
        /// <param name="itemCount">Numero di item restituiti dalla ricerca</param>
        /// <param name="pageNumber">Numero di pagina da caricare</param>
        /// <param name="pageSize">Dimensione della pagina</param>
        /// <returns>Lista degli item non ancora visti da un utente</returns>
        public static List<Item> LoadItemsNotViewedByUser(int administrationId, String administrationCode, int userId, int pageNumber, int pageSize, out int itemCount)
        {
            List<Item> items = new List<Item>();
            itemCount = 0;

            using (NotificationServiceOperationsClient client = new NotificationServiceOperationsClient())
            {
                // Ricerca dell'istanza con codice administrationCode
                int instanceId = client.LoadInstanceByName(new LoadInstanceByNameRequest(administrationCode)).LoadInstanceByNameResult.Id;

                // Caricamento dei canali registrati per l'amministrazione passata per parametro
                List<Channel> channels = client.LoadChannelsRelatedToInstance(
                    new LoadChannelsRelatedToInstanceRequest(instanceId)).LoadChannelsRelatedToInstanceResult;

                // Caricamento degli item per i canali individuati
                if (channels != null)
                    foreach (var channel in channels)
                    {
                        SearchItemsNotViewedByUserResponse itemsResponse =
                            client.SearchItemsNotViewedByUser(
                                new SearchItemsNotViewedByUserRequest(
                                    userId,
                                    channel.Id,
                                    pageSize,
                                    pageNumber,
                                    instanceId));
                        itemCount += itemsResponse.itemCount;
                        items.AddRange(itemsResponse.SearchItemsNotViewedByUserResult);
                    }
            }

            return items;

        }

        /// <summary>
        /// Proprietà utilizzata per indicare se un ruolo è abilitato alla visualizzazione ed alla gestione
        /// del centro notifiche
        /// </summary>
        public static bool IsUserEnabledToViewNotificationsCenter(Page page)
        {

            String enabled = DocsPAWA.utils.InitConfigurationKeys.GetValue(
                UserManager.getInfoUtente().idAmministrazione,
                "ENABLED_NOTIFICATION_CENTER");
            if (String.IsNullOrEmpty(enabled))
                enabled = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "ENABLED_NOTIFICATION_CENTER");

            bool authorized = false;

            if (UserManager.getRuolo() == null && UserManager.getUtente().ruoli != null && UserManager.getUtente().ruoli.Length > 0)
            {
                UserManager.setRuolo(UserManager.getUtente().ruoli[0]);
                authorized = UserManager.ruoloIsAutorized(page, "NOTIFICATION_CENTER");
                UserManager.setRuolo(null);
            }
            else
                authorized = UserManager.ruoloIsAutorized(page, "NOTIFICATION_CENTER");

            return !String.IsNullOrEmpty(enabled) && enabled == "1" && authorized;



        }

        /// <summary>
        /// Metodo per il recupero del numero di item ancora non visti da un utente
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="adminCode"></param>
        /// <returns></returns>
        public static int CountNotViewedItems(int userId, String adminCode)
        {
            using (NotificationServiceOperationsClient client = new NotificationServiceOperationsClient())
            {
                int instanceId = client.LoadInstanceByName(new LoadInstanceByNameRequest(adminCode)).LoadInstanceByNameResult.Id;

                return client.CountNotViewedItems(new CountNotViewedItemsRequest(userId, instanceId)).CountNotViewedItemsResult;
            }
        }

        /// <summary>
        /// Metodo per l'impostazione di un item come visto
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <param name="adminCode"></param>
        public static void SetItemAsViewed(int itemId, int userId, String adminCode)
        {
            using (NotificationServiceOperationsClient client = new NotificationServiceOperationsClient())
            {
                int instanceId = client.LoadInstanceByName(new LoadInstanceByNameRequest(adminCode)).LoadInstanceByNameResult.Id;
                client.SetItemViewed(new SetItemViewedRequest(itemId, userId, instanceId));
            }
        }

        /// <summary>
        /// Metodo per la ricerca di item che soddisfano determinati parametri
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchForMessageId"></param>
        /// <param name="lowMessageId"></param>
        /// <param name="hightMessageId"></param>
        /// <param name="searchForDate"></param>
        /// <param name="lowDate"></param>
        /// <param name="hightDate"></param>
        /// <param name="searchForTitle"></param>
        /// <param name="itemText"></param>
        /// <param name="adminCode"></param>
        /// <returns></returns>
        public static List<Item> SearchItem(int userId, bool searchForMessageId, int lowMessageId, int hightMessageId,
            bool searchForDate, DateTime lowDate, DateTime hightDate,
            bool searchForTitle, String itemText, String adminCode)
        {
            using (NotificationServiceOperationsClient client = new NotificationServiceOperationsClient())
            {
                try
                {
                    int instanceId = client.LoadInstanceByName(new LoadInstanceByNameRequest(adminCode)).LoadInstanceByNameResult.Id;
                    return client.SearchItem(new SearchItemRequest(
                        userId, searchForMessageId, lowMessageId, hightMessageId, searchForDate, lowDate, hightDate, searchForTitle, itemText, instanceId)).SearchItemResult;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

    }
}