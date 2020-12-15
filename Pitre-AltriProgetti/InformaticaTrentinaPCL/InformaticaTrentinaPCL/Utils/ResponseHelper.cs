using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;

namespace InformaticaTrentinaPCL.Utils
{
    public class ResponseHelper
    {
        IGeneralView view;
        BaseResponseModel response;
        IReachability reachability;

        public ResponseHelper(IGeneralView view, BaseResponseModel response, IReachability reachability)
        {
            this.view = view;
            this.response = response;
            this.reachability = reachability;
        }

        /// <summary>
        /// Ons the response received.
        /// </summary>
        /// <returns><c>true</c>, if the response is valid, <c>false</c> otherwise.</returns>
        public bool IsValidResponse()
        {
            //if (this.iReachability.CheckNetwork() == MyStatusNetwork.ServerDown)
            //{
            //    view.ShowError("Al momento il server risulta non raggiungibile. Ci scusiamo per il disagio.");
            //    return false;
            //}

            //if (this.iReachability.CheckNetwork() == MyStatusNetwork.Disconnect)
            //{
            //    view.ShowError("Controllare la connesione di rete al momento non risulti connesso.");
            //    return false;
            //}

            if (response.IsCancelled)
            {
                return false;
            }

            if (response.Error != null)
            {
                view.OnUpdateLoader(false);
                view.ShowError(response.Error);
                return false;
            }
#if !CUSTOM

            if (!response.StatusOk())
            {
                view.ShowError(LocalizedString.SERVICE_ERROR.Get());
                return false;
            }
#endif

            return true;
        }



    }
}
