using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using Service.Shared.Clients;

namespace TestRefit.Network
{
    public abstract class WS
    {       
        readonly List<CancellationTokenSource> ctsCollection = new List<CancellationTokenSource>();
        
        public INetworkAPI api;
        public INetworkAPI apiPrelogin;

        public WS()
        {                       
            var httpClient = new HttpClient(new HttpLoggingHandler())
            {
                BaseAddress = new Uri(NetworkConstants.GetUrlPrefered())
            };
            api = RestService.For<INetworkAPI>(httpClient);

            var httpClientOld = new HttpClient(new HttpLoggingHandler())
            {
                BaseAddress = new Uri(NetworkConstants.GetBaseUrl())
            };
            apiPrelogin = RestService.For<INetworkAPI>(httpClientOld);
        }

        protected CancellationToken getCancellationToken(int timeout = 60_000)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            if (timeout > 0)
            {
                cts.CancelAfter(timeout);
            }
            ctsCollection.Add(cts);
            return cts.Token;
        }

        public void Dispose()
        {
            
            foreach (var cts in ctsCollection)
            {
                cts.Cancel();
                cts.Dispose();
            }
            ctsCollection.Clear();
        }

        #region Manage error
        
        protected void ResolveError(BaseResponseModel response, Exception e)
        {
            if (IsCertificatePinningViolation(e))
            {
                ManageSSLError(response, e);
            }
            else if (e is OperationCanceledException)
            {
                ManageOperationCanceledException(response, (OperationCanceledException)e);
            }
            else
            {
                ManageGenericError(response, e);
            }
            Dispose();
        }

        private Boolean IsCertificatePinningViolation(Exception e)
        {
            return e != null && e.InnerException != null && e.InnerException.Message != null &&
                   e.InnerException.Message.Contains("SecureChannelFailure");
        }
        
        private void ManageOperationCanceledException(BaseResponseModel response, OperationCanceledException oce)
        {
            CancellationToken ct = oce.CancellationToken;
            if (ct.IsCancellationRequested)
            {
                response.IsCancelled = true;
            }
            else
            {
                response.Error = LocalizedString.ERROR_TIMEOUT.Get();
            }
        }

        private void ManageSSLError(BaseResponseModel response, Exception e){
            Debug.WriteLine("CERTIFICATE PINNING VIOLATION: " + e.Message);
            response.Error = LocalizedString.SSL_ERROR.Get();
        }

        private void ManageGenericError(BaseResponseModel response, Exception e)
        {
            Debug.WriteLine("Errore durante la chiamata a servizio: "+e.Message);
            response.Error = LocalizedString.SERVICE_ERROR.Get();
        }
        
        #endregion
        
    }
}
