﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;
using DocsPaConservazione.Metadata.Common;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 

namespace DocsPaConservazione.Metadata.Istanza
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Istanza
    {

        private SoggettoProduttore soggettoProduttoreField;
        private ResponsabileConservazione responsabileConservazioneField;
        private string idField;
        private string descrizioneField;
        private string dataCreazioneField;
        private string dataInvioField;
        private string dataChiusuraField;
        private string tipologiaField;

        /// <remarks/>
        public SoggettoProduttore SoggettoProduttore
        {
            get
            {
                return this.soggettoProduttoreField;
            }
            set
            {
                this.soggettoProduttoreField = value;
            }
        }

        /// <remarks/>
        public ResponsabileConservazione ResponsabileConservazione
        {
            get
            {
                return this.responsabileConservazioneField;
            }
            set
            {
                this.responsabileConservazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Descrizione
        {
            get
            {
                return this.descrizioneField;
            }
            set
            {
                this.descrizioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataCreazione
        {
            get
            {
                return this.dataCreazioneField;
            }
            set
            {
                this.dataCreazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataInvio
        {
            get
            {
                return this.dataInvioField;
            }
            set
            {
                this.dataInvioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataChiusura
        {
            get
            {
                return this.dataChiusuraField;
            }
            set
            {
                this.dataChiusuraField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Tipologia
        {
            get
            {
                return this.tipologiaField;
            }
            set
            {
                this.tipologiaField = value;
            }
        }






    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SoggettoProduttore
    {
        private Amministrazione amministrazioneField;
        private UnitaOrganizzativa[] gerarchiaUOField;
        private Creatore creatoreField;

        /// <remarks/>
        public Amministrazione Amministrazione
        {
            get
            {
                return this.amministrazioneField;
            }
            set
            {
                this.amministrazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public UnitaOrganizzativa[] GerarchiaUO
        {
            get
            {
                return this.gerarchiaUOField;
            }
            set
            {
                this.gerarchiaUOField = value;
            }
        }

        /// <remarks/>
        public Creatore Creatore
        {
            get
            {
                return this.creatoreField;
            }
            set
            {
                this.creatoreField = value;
            }
        }
    }



  

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ResponsabileConservazione
    {
        private string utenteField;
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Utente
        {
            get
            {
                return this.utenteField;
            }
            set
            {
                this.utenteField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class GerarchiaUO
    {
        private UnitaOrganizzativa[] unitaOrganizzativaField;
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UnitaOrganizzativa")]
        public UnitaOrganizzativa[] UnitaOrganizzativa
        {
            get
            {
                return this.unitaOrganizzativaField;
            }
            set
            {
                this.unitaOrganizzativaField = value;
            }
        }
    }

}