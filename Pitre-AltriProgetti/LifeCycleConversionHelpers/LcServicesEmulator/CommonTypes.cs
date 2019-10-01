using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LcEmulatorServices
{


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://adobe.com/idp/services")]
    public partial class XML
    {

        private string documentField;

        private string elementField;

        /// <remarks/>
        public string document
        {
            get
            {
                return this.documentField;
            }
            set
            {
                this.documentField = value;
            }
        }

        /// <remarks/>
        public string element
        {
            get
            {
                return this.elementField;
            }
            set
            {
                this.elementField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://adobe.com/idp/services")]
    public partial class BLOB
    {

        private string contentTypeField;

        private byte[] binaryDataField;

        private string attachmentIDField;

        private string remoteURLField;

        /// <remarks/>
        public string contentType
        {
            get
            {
                return this.contentTypeField;
            }
            set
            {
                this.contentTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] binaryData
        {
            get
            {
                return this.binaryDataField;
            }
            set
            {
                this.binaryDataField = value;
            }
        }

        /// <remarks/>
        public string attachmentID
        {
            get
            {
                return this.attachmentIDField;
            }
            set
            {
                this.attachmentIDField = value;
            }
        }

        /// <remarks/>
        public string remoteURL
        {
            get
            {
                return this.remoteURLField;
            }
            set
            {
                this.remoteURLField = value;
            }
        }
    }
}