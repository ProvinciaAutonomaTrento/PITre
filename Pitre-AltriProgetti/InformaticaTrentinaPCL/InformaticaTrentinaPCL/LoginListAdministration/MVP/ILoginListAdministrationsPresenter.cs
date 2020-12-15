using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.LoginListAdministration.MVP
{
    public interface ILoginListAdministrationsPresenter : IBasePresenter
    {
		void UpdateList();
	}
}
