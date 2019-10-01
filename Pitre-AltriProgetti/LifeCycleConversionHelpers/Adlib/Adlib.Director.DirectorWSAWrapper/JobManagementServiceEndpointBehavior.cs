using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Adlib.Director.DirectorWSAWrapper
{
    public class JobManagementServiceEndpointBehavior : IEndpointBehavior
    {
        private JobManagementServiceClientMessageInspector _clientMessageInspector;

        public JobManagementServiceClientMessageInspector ClientMessageInspector
        {
            get
            {
                if (this._clientMessageInspector == null)
                {
                    this._clientMessageInspector = new JobManagementServiceClientMessageInspector();
//                    this._clientMessageInspector.ClientMessages.Clear();
                }

                return this._clientMessageInspector;
            }
        }

        public JobManagementServiceEndpointBehavior()
        {
            this._clientMessageInspector = new JobManagementServiceClientMessageInspector();
//            this._clientMessageInspector.ClientMessages.Clear();
        }

        ~JobManagementServiceEndpointBehavior()
        {
            this._clientMessageInspector = null;
        }

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        
        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.ClientRuntime behavior)
        {
            behavior.MessageInspectors.Add(this.ClientMessageInspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint serviceEndpoint) { }
    }
}
