using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Utenti;
using DocsPaVO.utente;
using BusinessLogic.NotificationCenterOperationServices;
using log4net;

namespace BusinessLogic.NotificationCenter
{
    /// <summary>
    /// Classe di interfaccia con il centro notifiche. Questa classe offre funzionalità
    /// per l'interazione con il servizio WCF per la gestione del centro notifiche
    /// </summary>
    public class NotificationCenterHelper
    {
        private static ILog logger = LogManager.GetLogger(typeof(NotificationCenterHelper));

        /// <summary>
        /// Metodo per il recupero dello stato di abilitazione del centro notifiche
        /// </summary>
        /// <param name="administrationId">Id dell'amministrazione</param>
        /// <returns>Stato di abilitazione del centro servizi</returns>
        public static bool IsEnabled(String administrationId)
        {
            String valoreChiaveInteropUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(administrationId, "ENABLED_NOTIFICATION_CENTER");
            if (string.IsNullOrEmpty(valoreChiaveInteropUrl))
                valoreChiaveInteropUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLED_NOTIFICATION_CENTER");

            return valoreChiaveInteropUrl == "1";
        }

        /// <summary>
        /// Inserimento di un item nel centro notifiche
        /// </summary>
        /// <param name="author">Autpre dell'item</param>
        /// <param name="text">Contenuto dell'item</param>
        /// <param name="title">Titolo dell'item</param>
        /// <param name="userId">Id dell'utente a cui notificare l'item</param>
        /// <param name="channelLabel">Label del canale da associare all'item</param>
        /// <param name="idProfile">Id profile del documento legato all'item da inserire</param>
        /// <param name="registrationNumber">Numero di protocollo associato all'item da inserire</param>
        /// <param name="adminCode">Codice dell'amministrazione</param>
        public static void InsertItem(String author, String text, String title, int userId, String channelLabel, int idProfile, int registrationNumber, String adminCode)
        {
            try
            {
                using (NotificationServiceOperationsClient notificationServiceOperationsClient = new NotificationServiceOperationsClient())
                {
                    // Recupero del canale
                    Channel channel = notificationServiceOperationsClient.LoadChannelByLabel(new LoadChannelByLabelRequest(channelLabel)).LoadChannelByLabelResult;

                    // Se il canale è stato trovato, viene costruito ed inserito un nuovo item
                    if (channel != null)
                    {
                        Item item = new Item()
                        {
                            Author = author,
                            FeedLink = "",
                            Text = text,
                            Title = title,
                            MessageId = idProfile,
                            MessageNumber = registrationNumber

                        };

                        // Recupero delle informazioni sull'istanza
                        Instance instance = notificationServiceOperationsClient.LoadInstanceByName(new LoadInstanceByNameRequest(adminCode)).LoadInstanceByNameResult;

                        if (instance != null)
                        {
                            List<User> user = new List<User>() { new User() { UserId = userId, InstanceId = instance.Id } };

                            // Inserimento item nel centro notifiche
                            notificationServiceOperationsClient.InsertItem(new InsertItemRequest(item, channel.Id, user));
                        }
                    }

                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante l'inserimento di una notifica nel centro notifiche.", e);


            }

        }
    }
}