using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.LoginListAdministration.MVP
{
    public interface ILoginListAdministrationsView : IGeneralView
    {
		void UpdateList(List<AmministrazioneModel> list);
	}
}
