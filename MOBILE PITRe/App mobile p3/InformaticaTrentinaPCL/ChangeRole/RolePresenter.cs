using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using InformaticaTrentinaPCL.ChangeRole.MVPD;
using InformaticaTrentinaPCL.ChangeRole.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Resource;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.ChangeRole
{
    public class RolePresenter : IRolePresenter
    {
        IRoleView view;
        IRoleModel roleModel;
        SessionData sessionData;
        IReachability reachability;


        public RolePresenter(IRoleView view, INativeFactory nativeFactory)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            roleModel = new DummyRoleModel();
#else
            roleModel = new RoleModel();
#endif
        }

        public async void SetChangeRole(RuoloInfo newRole)
        {
            this.view.OnUpdateLoader(true);
            var request = new ChangeRoleRequestModel(sessionData.userInfo, newRole);
            ChangeRoleResponseModel response = await roleModel.ChangeRolesAsync(request);
            view.OnUpdateLoader(false);
            ManageChangeRoleResponse(response, newRole);
        }

        public void Dispose()
        {
            roleModel.Dispose();
        }

        private void ManageChangeRoleResponse(ChangeRoleResponseModel response, RuoloInfo newRole)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response,this.reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        if (NetworkConstants.NETWORK_TOKEN_EMPTY.Equals(response.userInfo.token))
                        {
                            view.ShowError(LocalizedString.CHANGE_ROLE_ID_ERROR.Get());
                        }
                        else if (NetworkConstants.RESPONSE_ERROR.Equals(response.userInfo.token))
                        {
                            view.ShowError(LocalizedString.TOKEN_ERROR.Get());
                        }
                        else
                        {
                            UpdateRoleInfo(newRole,response.userInfo, response.todoListRemoval, response.shareAllowed);
                            view.OnChangeRoleOK(newRole);
                        }
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }
               
        private void UpdateRoleInfo(RuoloInfo newRole, UserInfo newUserInfo, string todoListRemoval, bool shareAllowed)
        {
            try
            {
                sessionData.SetUserInfo(newUserInfo);
                sessionData.isTodoListRemovalManual = todoListRemoval == "Manual";
                sessionData.shareAllowed = shareAllowed;
                int newRoleIndex = newUserInfo.ruoli.IndexOf(newRole);
                RuoloInfo prevRole = newUserInfo.ruoli[0];
                newUserInfo.ruoli[0] = newRole;
                newUserInfo.ruoli[newRoleIndex] = prevRole;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[RolePresenter] - Error to manage roles array :"+e.Message);
            }
        }
    }
}
