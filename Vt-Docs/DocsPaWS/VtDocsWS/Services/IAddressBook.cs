using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per la gestione della rubrica dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IAddressBook
    {

        [OperationContract]
        VtDocsWS.Services.AddressBook.AddCorrespondent.AddCorrespondentResponse AddCorrespondent(VtDocsWS.Services.AddressBook.AddCorrespondent.AddCorrespondentRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentResponse DeleteCorrespondent(VtDocsWS.Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.EditCorrespondent.EditCorrespondentResponse EditCorrespondent(VtDocsWS.Services.AddressBook.EditCorrespondent.EditCorrespondentRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.GetCorrespondent.GetCorrespondentResponse GetCorrespondent(VtDocsWS.Services.AddressBook.GetCorrespondent.GetCorrespondentRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.SearchCorrespondents.SearchCorrespondentsResponse SearchCorrespondents(VtDocsWS.Services.AddressBook.SearchCorrespondents.SearchCorrespondentsRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.SearchUsers.SearchUsersResponse SearchUsers(VtDocsWS.Services.AddressBook.SearchUsers.SearchUsersRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersResponse GetCorrespondentFilters(VtDocsWS.Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.GetUserFilters.GetUserFiltersResponse GetUserFilters(VtDocsWS.Services.AddressBook.GetUserFilters.GetUserFiltersRequest request);

        [OperationContract]
        VtDocsWS.Services.AddressBook.GetOpportunity.GetOpportunityResponse GetOpportunityList(VtDocsWS.Services.AddressBook.GetOpportunity.GetOpportunityRequest request);
    }
}