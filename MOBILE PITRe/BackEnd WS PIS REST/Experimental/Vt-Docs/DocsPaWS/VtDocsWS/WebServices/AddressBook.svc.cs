using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using log4net;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione della rubrica
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class AddressBook : IAddressBook
    {

        private ILog logger = LogManager.GetLogger(typeof(AddressBook));

        /// <summary>
        /// metodo per la creazione di un nuovo corrispondente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.AddressBook.AddCorrespondent.AddCorrespondentResponse AddCorrespondent(Services.AddressBook.AddCorrespondent.AddCorrespondentRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.AddCorrespondent.AddCorrespondentResponse response = Manager.AddressBookManager.AddCorrespondent(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Metodo per la cancellazione di un corrispondente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>

        public Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentResponse DeleteCorrespondent(Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentResponse response = Manager.AddressBookManager.DeleteCorrespondent(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Metodo per la modifica di un corrispondente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>

        public Services.AddressBook.EditCorrespondent.EditCorrespondentResponse EditCorrespondent(Services.AddressBook.EditCorrespondent.EditCorrespondentRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.EditCorrespondent.EditCorrespondentResponse response = Manager.AddressBookManager.EditCorrespondent(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Metodo per il dettaglio di un corrispondente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>

        public Services.AddressBook.GetCorrespondent.GetCorrespondentResponse GetCorrespondent(Services.AddressBook.GetCorrespondent.GetCorrespondentRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.GetCorrespondent.GetCorrespondentResponse response = Manager.AddressBookManager.GetCorrespondent(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }


        /// <summary>
        /// Metodo per la ricerca di un corrispondente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>

        public Services.AddressBook.SearchCorrespondents.SearchCorrespondentsResponse SearchCorrespondents(Services.AddressBook.SearchCorrespondents.SearchCorrespondentsRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.SearchCorrespondents.SearchCorrespondentsResponse response = Manager.AddressBookManager.SearchCorrespondents(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Metodo per la ricerca di un utente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>response</returns>
        public Services.AddressBook.SearchUsers.SearchUsersResponse SearchUsers(Services.AddressBook.SearchUsers.SearchUsersRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.SearchUsers.SearchUsersResponse response = Manager.AddressBookManager.SearchUsers(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Metodo che restituisce la lista dei filtri applicabili nella ricerca dei corrispondenti
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersResponse GetCorrespondentFilters(Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersResponse response = Manager.AddressBookManager.GetCorrespondentFilters(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Metodo che restituisce la lista dei filtri applicabili nella ricerca dei utenti
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.AddressBook.GetUserFilters.GetUserFiltersResponse GetUserFilters(Services.AddressBook.GetUserFilters.GetUserFiltersRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.GetUserFilters.GetUserFiltersResponse response = Manager.AddressBookManager.GetUserFilters(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Metodo che restituisce la lista degli id opportuniti
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.AddressBook.GetOpportunity.GetOpportunityResponse GetOpportunityList(Services.AddressBook.GetOpportunity.GetOpportunityRequest request)
        {
            logger.Info("BEGIN");
            Services.AddressBook.GetOpportunity.GetOpportunityResponse response = Manager.AddressBookManager.GetOpportunityList(request);

            Utils.CheckFaultException(response);
            logger.Info("END");

            return response;
        }
    }
}
