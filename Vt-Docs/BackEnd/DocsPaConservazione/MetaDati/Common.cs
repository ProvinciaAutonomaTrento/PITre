using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaConservazione.Metadata.Common
{


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class UnitaOrganizzativa
    {

        private UnitaOrganizzativa[] unitaOrganizzativa1Field;
        private string codiceUOField;
        private string descrizioneUOField;
        private string livelloField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UnitaOrganizzativa")]
        public UnitaOrganizzativa[] UnitaOrganizzativa1
        {
            get
            {
                return this.unitaOrganizzativa1Field;
            }
            set
            {
                this.unitaOrganizzativa1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceUO
        {
            get
            {
                return this.codiceUOField;
            }
            set
            {
                this.codiceUOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneUO
        {
            get
            {
                return this.descrizioneUOField;
            }
            set
            {
                this.descrizioneUOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Livello
        {
            get
            {
                return this.livelloField;
            }
            set
            {
                this.livelloField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Creatore
    {

        private string codiceRuoloField;
        private string descrizioneRuoloField;
        private string codiceUtenteField;
        private string descrizioneUtenteField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceRuolo
        {
            get
            {
                return this.codiceRuoloField;
            }
            set
            {
                this.codiceRuoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneRuolo
        {
            get
            {
                return this.descrizioneRuoloField;
            }
            set
            {
                this.descrizioneRuoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceUtente
        {
            get
            {
                return this.codiceUtenteField;
            }
            set
            {
                this.codiceUtenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneUtente
        {
            get
            {
                return this.descrizioneUtenteField;
            }
            set
            {
                this.descrizioneUtenteField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Amministrazione
    {

        private string codiceAmministrazioneField;
        private string descrizioneAmministrazioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceAmministrazione
        {
            get
            {
                return this.codiceAmministrazioneField;
            }
            set
            {
                this.codiceAmministrazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneAmministrazione
        {
            get
            {
                return this.descrizioneAmministrazioneField;
            }
            set
            {
                this.descrizioneAmministrazioneField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Tipologia
    {

        private CampoTipologia[] campoTipologiaField;
        private string nomeTipologiaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CampoTipologia")]
        public CampoTipologia[] CampoTipologia
        {
            get
            {
                return this.campoTipologiaField;
            }
            set
            {
                this.campoTipologiaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NomeTipologia
        {
            get
            {
                return this.nomeTipologiaField;
            }
            set
            {
                this.nomeTipologiaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class CampoTipologia
    {

        private string nomeCampoField;
        private string valoreCampoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NomeCampo
        {
            get
            {
                return this.nomeCampoField;
            }
            set
            {
                this.nomeCampoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ValoreCampo
        {
            get
            {
                return this.valoreCampoField;
            }
            set
            {
                this.valoreCampoField = value;
            }
        }
    }
}
