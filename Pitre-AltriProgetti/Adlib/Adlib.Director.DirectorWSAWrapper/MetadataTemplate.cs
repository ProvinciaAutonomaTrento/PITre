////////////////////////////////////////////////////////////////////////////
/*
Copyright (C) 1997-2010 Adlib Software
All rights reserved.

DISCLAIMER OF WARRANTIES:
 
Permission is granted to copy this Sample Code for internal use only, 
provided that this permission notice and warranty disclaimer appears in all copies.
 
SAMPLE CODE IS LICENSED TO YOU AS-IS.
 
ADLIB SOFTWARE AND ITS SUPPLIERS AND LICENSORS DISCLAIM ALL WARRANTIES, 
EITHER EXPRESS OR IMPLIED, IN SUCH SAMPLE CODE, INCLUDING THE WARRANTY OF 
NON-INFRINGEMENT AND THE IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A 
PARTICULAR PURPOSE. IN NO EVENT WILL ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS 
BE LIABLE FOR ANY DAMAGES ARISING OUT OF THE USE OF OR INABILITY TO USE THE SAMPLE 
APPLICATION OR SAMPLE CODE, DISTRIBUTION OF THE SAMPLE APPLICATION OR SAMPLE CODE, 
OR COMBINATION OF THE SAMPLE APPLICATION OR SAMPLE CODE WITH ANY OTHER CODE. 
IN NO EVENT SHALL ADLIB SOFTWARE OR ITS LICENSORS AND SUPPLIERS BE LIABLE FOR ANY 
LOST REVENUE, LOST PROFITS OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, 
EVEN IF ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using Adlib.Director.DirectorWSAWrapper.JobManagementService;

namespace Adlib.Director.DirectorWSAWrapper
{
    public class MetadataDefinition
    {
        public MetadataDefinition() { }
        private List<string> _MetadataValueList = new List<string>();

        public string Name = null;
        [XmlArray("ValueList")]
        [XmlArrayItem("Value")]
        public List<string> ValueList
        {
            get
            {
                if (_MetadataValueList == null)
                {
                    _MetadataValueList = new List<string>();
                }
                return _MetadataValueList;
            }
            set
            {
                _MetadataValueList = value;
            }
        }
        public bool CanEditName = false;
        public bool CanEditValue = false;
        //public bool MandatoryData = false;
        public string Description = null;

        override public string ToString(){return Name;}
    }
    public class MetadataTemplate
    {
        public MetadataTemplate() { }
        private List<MetadataDefinition> _MetadataDefinitionList = new List<MetadataDefinition>();

        [XmlArray("MetadataDefinitionList")]
        [XmlArrayItem("MetadataDefinition")]
        public List<MetadataDefinition> MetadataDefinitionList
        {
            get
            {
                if (_MetadataDefinitionList == null)
                {
                    _MetadataDefinitionList = new List<MetadataDefinition>();
                }
                return _MetadataDefinitionList;
            }
            set
            {
                _MetadataDefinitionList = value;
            }
        }

        public void InitializeIfEmpty()
        {
            if (_MetadataDefinitionList == null)
            {
                _MetadataDefinitionList = new List<MetadataDefinition>();
            }
            if (_MetadataDefinitionList.Count == 0)
            {
                MetadataDefinition metadataDef = new MetadataDefinition();
                metadataDef.Name = "Set.Your.Metadata.Name.Here";
                metadataDef.CanEditName = true;
                metadataDef.CanEditValue = true;
                metadataDef.Description = "Empty metadata item. At least Name should be set.";
                metadataDef.ValueList = new List<string>();
                metadataDef.ValueList.Add("Add your value here");
                _MetadataDefinitionList.Add(metadataDef);

            }
        }

    }
    [XmlRoot(Namespace = "http://adlibsoftware.com/")]
    public class UserMetadataList
    {
        public UserMetadataList() { }
        List<Metadata> _MetadataList = new List<Metadata>();
        [XmlArray("MetadataList")]
        [XmlArrayItem("Metadata")]
        public List<Metadata> MetadataList
        {
            get
            {
                if (_MetadataList == null)
                {
                    _MetadataList = new List<Metadata>();
                }
                return _MetadataList;
            }
            set
            {
                _MetadataList = value;
            }
        }
    }
}
