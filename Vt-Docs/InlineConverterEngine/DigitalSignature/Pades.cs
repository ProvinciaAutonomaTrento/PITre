using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPA.ITextSharp.DigitalSignature;

namespace InlineConverterEngine.DigitalSignature
{
    public class Pades
    {
        public Pades() { }
        public bool IsPdfPades(byte[] content)
        {
            // Valore da restituire
            bool toReturn = false;

            try
            {
                DPA.ITextSharp.DigitalSignature.Pades pades = new DPA.ITextSharp.DigitalSignature.Pades();
                toReturn = pades.IsPdfPades(content);
            }
            catch(Exception e)
            {
                toReturn = false;
            }

            return toReturn;

        }
    }
}
