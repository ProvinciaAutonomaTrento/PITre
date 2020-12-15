using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.ChangeRole.MVPD
{

	public interface IRoleView : IGeneralView
	{
        void OnChangeRoleOK(RuoloInfo role);
	}
}
