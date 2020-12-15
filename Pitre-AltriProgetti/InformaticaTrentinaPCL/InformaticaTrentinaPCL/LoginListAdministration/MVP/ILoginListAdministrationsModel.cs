using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;

namespace InformaticaTrentinaPCL.LoginListAdministration.MVP
{
    public interface ILoginListAdministrationsModel : IBaseModel
    {
		Task<ListaAmministrazioniResponseModel> GetListaAmministrazioniByUser(ListaAmministrazioniRequestModel request);
	}
}
