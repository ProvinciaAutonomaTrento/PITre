using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace NttDatalLibrary
{
    public class CloserTagXMLWriter : XmlTextWriter
    {
        public CloserTagXMLWriter(Stream stream) : base(stream, Encoding.UTF8)
        {

        }
        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }
    }
}
