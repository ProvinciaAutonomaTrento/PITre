﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace DepositoServiceConsumer.DepositoServicesWR {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1098.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="DepServiceSoap", Namespace="http://tempuri.org/")]
    public partial class DepService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback DO_getIdProfileByDataOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public DepService() {
            this.Url = global::DepositoServiceConsumer.Properties.Settings.Default.DepositoServiceConsumer_DepositoServicesWR_DepService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event DO_getIdProfileByDataCompletedEventHandler DO_getIdProfileByDataCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DO_getIdProfileByData", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int DO_getIdProfileByData(string numProto, string AnnoProto, string idRegistro, string idGruppo, string idPeople) {
            object[] results = this.Invoke("DO_getIdProfileByData", new object[] {
                        numProto,
                        AnnoProto,
                        idRegistro,
                        idGruppo,
                        idPeople});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void DO_getIdProfileByDataAsync(string numProto, string AnnoProto, string idRegistro, string idGruppo, string idPeople) {
            this.DO_getIdProfileByDataAsync(numProto, AnnoProto, idRegistro, idGruppo, idPeople, null);
        }
        
        /// <remarks/>
        public void DO_getIdProfileByDataAsync(string numProto, string AnnoProto, string idRegistro, string idGruppo, string idPeople, object userState) {
            if ((this.DO_getIdProfileByDataOperationCompleted == null)) {
                this.DO_getIdProfileByDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDO_getIdProfileByDataOperationCompleted);
            }
            this.InvokeAsync("DO_getIdProfileByData", new object[] {
                        numProto,
                        AnnoProto,
                        idRegistro,
                        idGruppo,
                        idPeople}, this.DO_getIdProfileByDataOperationCompleted, userState);
        }
        
        private void OnDO_getIdProfileByDataOperationCompleted(object arg) {
            if ((this.DO_getIdProfileByDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DO_getIdProfileByDataCompleted(this, new DO_getIdProfileByDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1098.0")]
    public delegate void DO_getIdProfileByDataCompletedEventHandler(object sender, DO_getIdProfileByDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1098.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DO_getIdProfileByDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DO_getIdProfileByDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591