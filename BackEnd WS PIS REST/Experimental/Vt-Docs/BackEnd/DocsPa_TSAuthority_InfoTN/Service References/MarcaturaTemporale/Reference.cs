﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DocsPa_TSAuthority_InfoTN.MarcaturaTemporale {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1098.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.regione.taa.it/FaultType/schemas")]
    public partial class FaultType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string errorCodeField;
        
        private TipologiaFaultType typeField;
        
        private string userMessageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
        public string errorCode {
            get {
                return this.errorCodeField;
            }
            set {
                this.errorCodeField = value;
                this.RaisePropertyChanged("errorCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TipologiaFaultType type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
                this.RaisePropertyChanged("type");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string userMessage {
            get {
                return this.userMessageField;
            }
            set {
                this.userMessageField = value;
                this.RaisePropertyChanged("userMessage");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1098.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.regione.taa.it/FaultType/schemas")]
    public enum TipologiaFaultType {
        
        /// <remarks/>
        USER_FAULT,
        
        /// <remarks/>
        SYSTEM_FAULT,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1098.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas")]
    public partial class MarcaType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private byte[] marcaField;
        
        private System.DateTime dataOraMarcaField;
        
        private long serialNumberField;
        
        private string timestampAuthorityField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", Order=0)]
        public byte[] marca {
            get {
                return this.marcaField;
            }
            set {
                this.marcaField = value;
                this.RaisePropertyChanged("marca");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime dataOraMarca {
            get {
                return this.dataOraMarcaField;
            }
            set {
                this.dataOraMarcaField = value;
                this.RaisePropertyChanged("dataOraMarca");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long serialNumber {
            get {
                return this.serialNumberField;
            }
            set {
                this.serialNumberField = value;
                this.RaisePropertyChanged("serialNumber");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string timestampAuthority {
            get {
                return this.timestampAuthorityField;
            }
            set {
                this.timestampAuthorityField = value;
                this.RaisePropertyChanged("timestampAuthority");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/service", ConfigurationName="MarcaturaTemporale.MarcaturaTemporalePortType")]
    public interface MarcaturaTemporalePortType {
        
        // CODEGEN: Generating message contract since the wrapper namespace (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions) of message EmissioneMarcaTemporaleRequest does not match the default value (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/service)
        [System.ServiceModel.OperationContractAttribute(Action="/EmissioneMarcaTemporale", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.FaultType), Action="/EmissioneMarcaTemporale", Name="WSFault", Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.EmissioneMarcaTemporaleResponse EmissioneMarcaTemporale(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.EmissioneMarcaTemporaleRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions) of message VerificaDisponibilitaMarcheRequest does not match the default value (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/service)
        [System.ServiceModel.OperationContractAttribute(Action="/VerificaDisponibilitaMarche", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.FaultType), Action="/VerificaDisponibilitaMarche", Name="WSFault", Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaDisponibilitaMarcheResponse VerificaDisponibilitaMarche(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaDisponibilitaMarcheRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions) of message VerificaMarcaturaRequest does not match the default value (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/service)
        [System.ServiceModel.OperationContractAttribute(Action="/VerificaMarcatura", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.FaultType), Action="/VerificaMarcatura", Name="WSFault", Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaMarcaturaResponse VerificaMarcatura(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaMarcaturaRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions) of message MarcaTemporaleDaHashRequest does not match the default value (http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/service)
        [System.ServiceModel.OperationContractAttribute(Action="/MarcaTemporaleDaHash", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.FaultType), Action="/MarcaTemporaleDaHash", Name="WSFault", Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaTemporaleDaHashResponse MarcaTemporaleDaHash(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaTemporaleDaHashRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="EmissioneMarcaTemporale", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class EmissioneMarcaTemporaleRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas", DataType="base64Binary")]
        public byte[] file;
        
        public EmissioneMarcaTemporaleRequest() {
        }
        
        public EmissioneMarcaTemporaleRequest(byte[] file) {
            this.file = file;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="EmissioneMarcaTemporaleResponse", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class EmissioneMarcaTemporaleResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", Order=0)]
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType marcaTemporale;
        
        public EmissioneMarcaTemporaleResponse() {
        }
        
        public EmissioneMarcaTemporaleResponse(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType marcaTemporale) {
            this.marcaTemporale = marcaTemporale;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1098.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas")]
    public partial class DisponibilitaType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private long marcheConsumateField;
        
        private long marcheDisponibiliField;
        
        private string utenteField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long marcheConsumate {
            get {
                return this.marcheConsumateField;
            }
            set {
                this.marcheConsumateField = value;
                this.RaisePropertyChanged("marcheConsumate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long marcheDisponibili {
            get {
                return this.marcheDisponibiliField;
            }
            set {
                this.marcheDisponibiliField = value;
                this.RaisePropertyChanged("marcheDisponibili");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string utente {
            get {
                return this.utenteField;
            }
            set {
                this.utenteField = value;
                this.RaisePropertyChanged("utente");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="VerificaDisponibilitaMarche", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class VerificaDisponibilitaMarcheRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", Order=0)]
        public string identificativoRichiesta;
        
        public VerificaDisponibilitaMarcheRequest() {
        }
        
        public VerificaDisponibilitaMarcheRequest(string identificativoRichiesta) {
            this.identificativoRichiesta = identificativoRichiesta;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="VerificaDisponibilitaMarcheResponse", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class VerificaDisponibilitaMarcheResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", Order=0)]
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.DisponibilitaType disponibilita;
        
        public VerificaDisponibilitaMarcheResponse() {
        }
        
        public VerificaDisponibilitaMarcheResponse(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.DisponibilitaType disponibilita) {
            this.disponibilita = disponibilita;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="VerificaMarcatura", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class VerificaMarcaturaRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas", DataType="base64Binary")]
        public byte[] marca;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas", DataType="base64Binary")]
        public byte[] file;
        
        public VerificaMarcaturaRequest() {
        }
        
        public VerificaMarcaturaRequest(byte[] marca, byte[] file) {
            this.marca = marca;
            this.file = file;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="VerificaMarcaturaResponse", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class VerificaMarcaturaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", Order=0)]
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType marca;
        
        public VerificaMarcaturaResponse() {
        }
        
        public VerificaMarcaturaResponse(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType marca) {
            this.marca = marca;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="MarcaTemporaleDaHash", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class MarcaTemporaleDaHashRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporaleType/schemas")]
        public string hash;
        
        public MarcaTemporaleDaHashRequest() {
        }
        
        public MarcaTemporaleDaHashRequest(string hash) {
            this.hash = hash;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="MarcaTemporaleDaHashResponse", WrapperNamespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", IsWrapped=true)]
    public partial class MarcaTemporaleDaHashResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.marcaturatemporale.regione.taa.it/MarcaturaTemporale/definitions", Order=0)]
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType marcaTemporale;
        
        public MarcaTemporaleDaHashResponse() {
        }
        
        public MarcaTemporaleDaHashResponse(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType marcaTemporale) {
            this.marcaTemporale = marcaTemporale;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface MarcaturaTemporalePortTypeChannel : DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MarcaturaTemporalePortTypeClient : System.ServiceModel.ClientBase<DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType>, DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType {
        
        public MarcaturaTemporalePortTypeClient() {
        }
        
        public MarcaturaTemporalePortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MarcaturaTemporalePortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MarcaturaTemporalePortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MarcaturaTemporalePortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.EmissioneMarcaTemporaleResponse DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType.EmissioneMarcaTemporale(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.EmissioneMarcaTemporaleRequest request) {
            return base.Channel.EmissioneMarcaTemporale(request);
        }
        
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType EmissioneMarcaTemporale(byte[] file) {
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.EmissioneMarcaTemporaleRequest inValue = new DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.EmissioneMarcaTemporaleRequest();
            inValue.file = file;
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.EmissioneMarcaTemporaleResponse retVal = ((DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType)(this)).EmissioneMarcaTemporale(inValue);
            return retVal.marcaTemporale;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaDisponibilitaMarcheResponse DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType.VerificaDisponibilitaMarche(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaDisponibilitaMarcheRequest request) {
            return base.Channel.VerificaDisponibilitaMarche(request);
        }
        
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.DisponibilitaType VerificaDisponibilitaMarche(string identificativoRichiesta) {
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaDisponibilitaMarcheRequest inValue = new DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaDisponibilitaMarcheRequest();
            inValue.identificativoRichiesta = identificativoRichiesta;
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaDisponibilitaMarcheResponse retVal = ((DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType)(this)).VerificaDisponibilitaMarche(inValue);
            return retVal.disponibilita;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaMarcaturaResponse DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType.VerificaMarcatura(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaMarcaturaRequest request) {
            return base.Channel.VerificaMarcatura(request);
        }
        
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType VerificaMarcatura(byte[] marca, byte[] file) {
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaMarcaturaRequest inValue = new DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaMarcaturaRequest();
            inValue.marca = marca;
            inValue.file = file;
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.VerificaMarcaturaResponse retVal = ((DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType)(this)).VerificaMarcatura(inValue);
            return retVal.marca;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaTemporaleDaHashResponse DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType.MarcaTemporaleDaHash(DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaTemporaleDaHashRequest request) {
            return base.Channel.MarcaTemporaleDaHash(request);
        }
        
        public DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaType MarcaTemporaleDaHash(string hash) {
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaTemporaleDaHashRequest inValue = new DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaTemporaleDaHashRequest();
            inValue.hash = hash;
            DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaTemporaleDaHashResponse retVal = ((DocsPa_TSAuthority_InfoTN.MarcaturaTemporale.MarcaturaTemporalePortType)(this)).MarcaTemporaleDaHash(inValue);
            return retVal.marcaTemporale;
        }
    }
}
