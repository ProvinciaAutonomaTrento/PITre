using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Documenti.DigitalSignature.PKCS_Utils
{
    public interface ITimeStampedContainer
    {
        global::BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile create(global::BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile p7mFile, global::BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile[] tsrFiles);
        global::BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile Data { get; set; }
        void explode(byte[] fileContents);
        global::BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile cryptoFile { get; set; }
        global::System.Collections.Generic.List<global::BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile> TSR { get; set; }
    }
}
