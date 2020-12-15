using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.ChangeRole.MVPD
{
    public interface IRolePresenter : IBasePresenter
	{
        void SetChangeRole(RuoloInfo newRole);
	}
}
