using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Services.Protocols;

namespace Publisher.Hosts
{
    /// <summary>
    /// Servizi Web per la gestione dei servizi di pubblicazione dei contenuti dal sistema documentale
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VTDocs/Publisher")]
    public class PublisherWebService : WebService
    {
        #region Public Members

        /// <summary>
        /// Avvia l'esecuzione di un canale di pubblicazione a partire dall'identificativo univoco
        /// </summary>
        /// <param name="channelId"></param>
        [WebMethod()]
        public void StartChannelById(int channelId)
        {
            try
            {
                PublisherServiceControl.StartChannel(channelId);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Avvia l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        [WebMethod()]
        public void StartChannel(ChannelRefInfo channelRef)
        {
            try
            {
                PublisherServiceControl.StartChannel(channelRef);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Riavvia l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        [WebMethod()]
        public void ReStartChannel(ChannelRefInfo channelRef)
        {
            try
            {
                PublisherServiceControl.ReStartChannel(channelRef);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Riavvia l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelId"></param>
        [WebMethod()]
        public void ReStartChannelById(int channelId)
        {
            try
            {
                PublisherServiceControl.ReStartChannel(channelId);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Riavvio di tutti i canali di pubblicazione in stato di fermo inaspettato
        /// </summary>
        /// <remarks>
        /// Lo stato dei canali sarà ripristinato direttamente sui server in cui erano stati avviati
        /// </remarks>
        [WebMethod()]
        public void RestartUnexpectedStoppedChannels()
        {
            try
            {
                PublisherServiceControl.RestartUnexpectedStoppedChannels();
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Sospende l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        [WebMethod()]
        public void StopChannel(ChannelRefInfo channelRef)
        {
            try
            {
                PublisherServiceControl.StopChannel(channelRef);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Sospende l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelId"></param>
        [WebMethod()]
        public void StopChannelById(int channelId)
        {
            try
            {
                PublisherServiceControl.StopChannel(channelId);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento dei canali di pubblicazione dell'amministrazione
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod()]
        public ChannelRefInfo[] GetAdminChannelList(int id)
        {
            try
            {
                ChannelRefInfo[] list = DataAccess.PublisherDataAdapter.GetAdminChannelList(id);

                foreach (ChannelRefInfo channel in list)
                {
                    PublisherServiceControl.RefreshChannelState(channel);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento dei canali di pubblicazione
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public ChannelRefInfo[] GetChannelList()
        {
            try
            {
                ChannelRefInfo[] list = DataAccess.PublisherDataAdapter.GetChannelList();

                foreach (ChannelRefInfo channel in list)
                {
                    PublisherServiceControl.RefreshChannelState(channel);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento di un canale di pubblicazione
        /// </summary>
        /// <param name="idChannel"></param>
        /// <returns></returns>
        [WebMethod()]
        public ChannelRefInfo GetChannel(int idChannel)
        {
            try
            {
                ChannelRefInfo channel = DataAccess.PublisherDataAdapter.GetChannel(idChannel);

                PublisherServiceControl.RefreshChannelState(channel);

                return channel;
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Aggiornamento dati di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        /// <remarks>
        /// L'operazione può essere effettuata solamente se il canale risulta in stato di fermo
        /// </remarks>
        /// <returns></returns>
        [WebMethod()]
        public ChannelRefInfo SaveChannel(ChannelRefInfo channelRef)
        {
            try
            {
                channelRef = DataAccess.PublisherDataAdapter.SaveChannel(channelRef);

                PublisherServiceControl.RefreshChannelState(channelRef);

                return channelRef;
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Rimozione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        /// <remarks>
        /// L'operazione può essere effettuata solamente se il canale risulta in stato di fermo
        /// </remarks>
        [WebMethod()]
        public ChannelRefInfo RemoveChannel(ChannelRefInfo channelRef)
        {
            try
            {
                return DataAccess.PublisherDataAdapter.RemoveChannel(channelRef);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento degli eventi impostati in un'istanza di pubblicazione
        /// </summary>
        /// <param name="idChannel"></param>
        /// <returns></returns>
        [WebMethod()]
        public EventInfo[] GetEventList(int idChannel)
        {
            try
            {
                return DataAccess.PublisherDataAdapter.GetEventList(idChannel);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento di un evento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod()]
        public EventInfo GetEvent(int id)
        {
            try
            {
                return DataAccess.PublisherDataAdapter.GetEvent(id);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Aggiornamento dati dell'evento
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        /// <remarks>
        /// L'operazione può essere effettuata solamente se il canale di pubblicazione dell'evento risulta in stato di fermo
        /// </remarks>
        [WebMethod()]
        public EventInfo SaveEvent(EventInfo eventInfo)
        {
            try
            {
                return DataAccess.PublisherDataAdapter.SaveEvent(eventInfo);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Rimozione evento
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        /// <remarks>
        /// L'operazione può essere effettuata solamente se il canale di pubblicazione dell'evento risulta in stato di fermo
        /// </remarks>
        [WebMethod()]
        public EventInfo RemoveEvent(EventInfo eventInfo)
        {
            try
            {
                return DataAccess.PublisherDataAdapter.RemoveEvent(eventInfo);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento degli errori nel canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        /// <returns></returns>
        [WebMethod()]
        public ErrorInfo[] GetErrors(ChannelRefInfo channelRef)
        {
            try
            {
                return DataAccess.PublisherDataAdapter.GetErrors(channelRef);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        #endregion

        #region Private Members

        #endregion
    }
}
