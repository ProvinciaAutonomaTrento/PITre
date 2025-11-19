// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using iText.Kernel.Pdf;
using iText.Kernel.XMP;
using Pi3.Infrastructure.IText.Decorator;

namespace Pi3.App.Legacy.WebApi.Application.Sign
{
    internal class Pades
    {
        /// <summary>
        /// Predisone un file Pades alla firma, se il parametro signature è vuoto torna l'hash del file da firmare, se è pieno lo firma
        /// </summary>
        /// <param name="data">i dati del PDF</param>
        /// <param name="signature">i dati della firma, se null calcola solo l'hash</param>
        /// <returns></returns>
        //public static byte[] SignPadesFile(byte[] data, byte[] signature)
        //{
        //    MemoryStream outMs = new MemoryStream();

        //    PdfReader reader = new PdfReader(data);
        //    PdfStamper stp = null;
        //    bool isPades = true;
        //    //bool isPades = IsPdfPades(reader);
        //    bool isPdfA = IsPDFA(reader);

        //    if (isPades)  //se pades vado in append.
        //        stp = PdfStamper.CreateSignature(reader, outMs, '\0', null, true);
        //    else
        //        stp = PdfStamper.CreateSignature(reader, outMs, '\0');

        //    if (isPdfA)
        //        stp.Writer.PDFXConformance = PdfWriter.PDFA1A;


        //    PdfSignatureAppearance sap = stp.SignatureAppearance;

        //    if (isPdfA)
        //    {
        //        //BaseFont bf = BaseFont.CreateFont(@"c:\windows\fonts\arial.ttf", BaseFont.WINANSI, true);
        //        //forse va sistemato questo path.
        //        //Pades_Utils.dpaItextSharp.iTextSharp.text.pdf.fonts.Helvetica.afm
        //        //DPA.DigitalSignature.Itextsharp.iTextSharp.text.pdf.fonts.Helvetica.afm
        //        Stream fo = BaseFont.GetResourceStream("Pades_Utils.dpaItextSharp.iTextSharp.text.pdf.fonts.Helvetica.afm");
        //        byte[] fb = new BinaryReader(fo).ReadBytes((int)fo.Length);
        //        BaseFont bf = BaseFont.CreateFont("helvetica.afm", BaseFont.WINANSI, true, false, fb, fb);
        //        sap.Layer2Font = new Font(bf);
        //        //BaseFont bf1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);

        //        // bf = BaseFont.CreateFont (


        //    }

        //    PdfSignature dic = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);
        //    sap.CryptoDictionary = dic;

        //    //generazione nuovo pdf
        //    int csize = 10000;
        //    Dictionary<PdfName, int> exc = new Dictionary<PdfName, int>();
        //    exc[PdfName.CONTENTS] = csize * 2 + 2;
        //    Hashtable dic_hasht = new Hashtable(exc);
        //    sap.PreClose(dic_hasht);
        //    /*
        //    //tolgo l'id
        //    stp.Reader.Trailer.Put(dpaItextSharp.text.pdf.PdfName.ID, null);
        //    dpaItextSharp.text.pdf.PdfDictionary dict = (dpaItextSharp.text.pdf.PdfDictionary)stp.Reader.Trailer.GetAsDict(dpaItextSharp.text.pdf.PdfName.INFO);
        //    dict.Put(dpaItextSharp.text.pdf.PdfName.MODDATE, null);
        //    stp.Writer.Info.Put(dpaItextSharp.text.pdf.PdfName.MODDATE, null);
        //    */

        //    Stream s = sap.RangeStream;
        //    MemoryStream ss = new MemoryStream();
        //    int read = 0;
        //    byte[] buff = new byte[8192];
        //    while ((read = s.Read(buff, 0, 8192)) > 0)
        //        ss.Write(buff, 0, read);


        //    //se signature è vuota a me interessa SOLO l'hash sha256 e lo ritorno
        //    if (signature == null)
        //    {
        //        return Pkcs.getSha256(ss.ToArray());
        //    }

        //    // ho una firma, procedo con l'append della firma sul file.
        //    byte[] outc = new byte[csize];

        //    PdfDictionary dic2 = new PdfDictionary();

        //    Array.Copy(signature, 0, outc, 0, signature.Length);

        //    dic2.Put(PdfName.CONTENTS, new PdfString(outc).SetHexWriting(true));
        //    sap.Close(dic2, true);
        //    outMs.Position = 0;
        //    BinaryReader br = new BinaryReader(outMs);
        //    byte[] retval = br.ReadBytes((int)outMs.Length);
        //    outMs.Close();
        //    return retval;
        //}


        //private static byte[] GetDocumentBytes(PdfSigner signer)
        //{
        //    using var ms = new MemoryStream();
        //    signer.GetDocument().CopyTo(ms);
        //    return ms.ToArray();
        //}
        //private static bool IsPDFA(PdfReader r)
        //{
        //    bool retval = false;
        //    try
        //    {
        //        byte[] metadata = r.Metadata;
        //        if (metadata != null)
        //        {
        //            try
        //            {
        //                string meta = System.Text.ASCIIEncoding.Default.GetString(metadata);
        //                XmlDocument xdoc = new XmlDocument();
        //                xdoc.LoadXml(meta);
        //                XmlNodeList nodeList = xdoc.GetElementsByTagName("pdfaid:conformance");
        //                if (nodeList.Item(0).FirstChild.Value.ToUpper() == "A")
        //                    retval = true;

        //            }
        //            catch { }
        //        }
        //    }
        //    catch { }
        //    return retval;
        //}

        //public static byte[] SignPadesFile(byte[] data, byte[] signature)
        //{
        //    MemoryStream outMs = new MemoryStream();

        //    var reader = new PdfReader(new MemoryStream(data));
        //    var signer = new CustomPdfSigner(reader, outMs, new StampingProperties());

        //    if (signature == null)
        //    {
        //        // Se la firma è null, calcola solo l'hash SHA256
        //        byte[] dataStream = GetDocumentBytes(signer);
        //        byte[] hash = Pkcs.getSha256(dataStream);
        //        return hash;
        //    }

        //    // Carica il certificato
        //    string pfxPath = "path/to/your/certificate.pfx";
        //    string pfxPassword = "your_password";
        //    Pkcs12Store pk12 = new Pkcs12Store(new FileStream(pfxPath, FileMode.Open, FileAccess.Read), pfxPassword.ToCharArray());
        //    string alias = null;
        //    foreach (string tAlias in pk12.Aliases)
        //    {
        //        if (pk12.IsKeyEntry(tAlias))
        //        {
        //            alias = tAlias;
        //            break;
        //        }
        //    }
        //    ICipherParameters pk = pk12.GetKey(alias).Key;
        //    var chain = pk12.GetCertificateChain(alias);
        //    var certs = new BCrypto1::Org.BouncyCastle.X509.X509Certificate[chain.Length];
        //    for (int k = 0; k < chain.Length; ++k)
        //    {
        //        certs[k] = chain[k].Certificate;
        //    }

        //    // Crea l'apparenza della firma
        //    var appearance = signer.GetCustomSignatureAppearance();

        //    // To store reason/location in the signature dictionary:
        //    signer.GetSignatureDictionary().Put(PdfName.Reason, new PdfString("Document signed"));
        //    signer.GetSignatureDictionary().Put(PdfName.Location, new PdfString("Location"));

        //    // Crea la firma
        //    IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
        //    IExternalDigest digest = new BouncyCastleDigest();

        //    // Firma il documento
        //    signer.SignDetached(digest, pks, certs, null, null, null, 0, PdfSigner.CryptoStandard.CMS);

        //    return outMs.ToArray();
        //}

        //private static byte[] GetDocumentBytes(PdfSigner signer)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        signer.GetDocument().CopyTo(ms);
        //        return ms.ToArray();
        //    }
        //}

        //private static byte[] ReadFully(Stream input)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        input.CopyTo(ms);
        //        return ms.ToArray();
        //    }
        //}

        //public byte[] SignPdf(byte[] pdfBytes)
        //{
        //    using var ms = new MemoryStream(); 
        //    var reader = new PdfReader(new MemoryStream(pdfBytes)); 
        //    var signer = new CustomPdfSigner(reader, ms, new StampingProperties());
        //    // To store reason/location in the signature dictionary:
        //    signer.GetSignatureDictionary().Put(PdfName.Reason, new PdfString("Document signed"));
        //    signer.GetSignatureDictionary().Put(PdfName.Location, new PdfString("Location"));

        //    // To update visible text on the appearance layer:
        //    var appearance = signer.GetCustomSignatureAppearance();
        //    appearance.SetContent("Signed by X\nReason: Document signed\nLocation: Someplace");
        //    // (No SetReason/SetLocation methods in iText7)

        //    // Do your chain loading, private key signing, etc. here
        //    // signer.SignDetached(...);

        //    return ms.ToArray();
        //}

        //public static byte[] SignPadesFile(byte[] data, byte[] signature)
        //{
        //    using var output = new MemoryStream();
        //    using var reader = new PdfReader(new MemoryStream(data));

        //    // Crea il PdfSigner in modalità append
        //    var stampingProps = new StampingProperties().UseAppendMode();
        //    var signer = new PdfSigner(reader, output, stampingProps);

        //    // Se signature == null, restituisce l’hash SHA-256
        //    if (signature == null)
        //    {
        //        byte[] docBytes = GetDocumentBytes(signer);
        //        return Pkcs.getSha256(docBytes);
        //    }

        //    // Carica certificato e chiave privata dal byte array
        //    var pkcs12Store = new Pkcs12Store(new MemoryStream(signature), "password".ToCharArray());
        //    string alias = null;
        //    foreach (string tAlias in pkcs12Store.Aliases)
        //    {
        //        if (pkcs12Store.IsKeyEntry(tAlias))
        //        {
        //            alias = tAlias;
        //            break;
        //        }
        //    }
        //    ICipherParameters pk = pkcs12Store.GetKey(alias).Key;

        //    // Converti ICipherParameters in IPrivateKey
        //    IPrivateKey privateKey = new BouncyCastlePrivateKey((AsymmetricKeyParameter)pk);

        //    // Costruisce la catena dei certificati
        //    var chain = pkcs12Store.GetCertificateChain(alias);
        //    var certs = new BCrypto1::Org.BouncyCastle.X509.X509Certificate[chain.Length];
        //    for (int i = 0; i < chain.Length; i++)
        //    {
        //        certs[i] = new BCrypto1::Org.BouncyCastle.X509.X509Certificate(chain[i].Certificate.GetSignature());
        //    }

        //    // Inserisce reason e location
        //    signer.GetSignatureDictionary().Put(PdfName.Reason, new PdfString("Document signed"));
        //    signer.GetSignatureDictionary().Put(PdfName.Location, new PdfString("Location"));

        //    // Firma il PDF
        //    IExternalSignature externalSignature = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
        //    IExternalDigest externalDigest = new BouncyCastleDigest();

        //    signer.SignDetached(
        //        externalDigest,
        //        externalSignature,
        //        certs,
        //        null, // CRL
        //        null, // OCSP
        //        null, // TSA
        //        0,
        //        PdfSigner.CryptoStandard.CMS
        //    );

        //    return output.ToArray();
        //}

        //private static byte[] GetDocumentBytes(PdfSigner signer)
        //{
        //    using var ms = new MemoryStream();
        //    var pdfDoc = signer.GetDocument();
        //    pdfDoc.Close();
        //    ms.Position = 0;
        //    return ms.ToArray();
        //}

        //public static byte[] SignPadesFile(byte[] data, byte[] signature)
        //{
        //    using (MemoryStream outMs = new MemoryStream())
        //    {
        //        PdfReader reader = new PdfReader(new MemoryStream(data));
        //        PdfWriter writer = new PdfWriter(outMs);
        //        PdfDocument pdfDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());

        //        bool isPdfA = IsPDFA(pdfDoc);
        //        PdfSigner signer = new PdfSigner(pdfDoc, writer, new StampingProperties());

        //        if (isPdfA)
        //        {
        //            try
        //            {
        //                // Register and embed the Helvetica font
        //                PdfFont helveticaFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
        //                signer.SetLayer2Font(helveticaFont);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error embedding Helvetica font: {ex.Message}");
        //            }
        //        }

        //        PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
        //        appearance.SetReason("Digitally Signed");
        //        appearance.SetLocation("Location");

        //        // Create signature dictionary
        //        PdfSignature dic = new PdfSignature(PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached);
        //        signer.SetCryptoDictionary(dic);

        //        // Set external signing data
        //        byte[] signedData = GetRangeStreamBytes(signer);

        //        if (signature == null)
        //        {
        //            return PKCS_Utils.Pkcs.getSha256(signedData);
        //        }

        //        // Append the signature
        //        byte[] paddedSignature = new byte[10000];
        //        Array.Copy(signature, 0, paddedSignature, 0, Math.Min(signature.Length, paddedSignature.Length));

        //        signer.SetSignatureDictionary(dic);
        //        signer.Close(new PdfDictionary { { PdfName.CONTENTS, new PdfString(paddedSignature).SetHexWriting(true) } });

        //        pdfDoc.Close();
        //        return outMs.ToArray();
        //    }
        //}

        //private static byte[] GetRangeStreamBytes(PdfSigner signer)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        Stream s = signer.GetRangeStream();
        //        s.CopyTo(ms); // Simplified using CopyTo method
        //        return ms.ToArray();
        //    }
        //}

        //private static bool IsPDFA(PdfDocument pdfDoc)
        //{
        //    try
        //    {
        //        return pdfDoc.GetPdfVersion() == PdfVersion.PDF_A_1B; // Check PDF/A compliance directly
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public static byte[] SignPadesFile(byte[] data, byte[] signature)
        {
            return SigningHelper.SignPadesFile(data, signature);

            //using (MemoryStream outMs = new MemoryStream())
            //using (PdfReader reader = new PdfReader(new MemoryStream(data)))
            //{
            //    // Configurazione PdfSigner
            //    var signer = new CustomPdfSigner(reader, outMs, new StampingProperties().UseAppendMode());

            //    var document = signer.GetDocument();

            //    bool isPdfA = IsPDFA(document);

            //    // Gestione PDF/A
            //    if (isPdfA)
            //    {
            //        signer.GetDocument().SetTagged();
            //        signer.GetDocument().GetCatalog().SetLang(new PdfString("en-US"));
            //    }

            //    // Ottieni l'oggetto SignatureAppearance corretto
            //    var sap = signer.GetCustomSignatureAppearance();

            //    // Configurazione font per PDF/A
            //    if (isPdfA)
            //    {
            //        PdfFont font = PdfFontFactory.CreateFont(
            //            StandardFonts.HELVETICA,
            //            PdfEncodings.WINANSI,
            //            PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
            //        );
            //        sap.SetFont(font);
            //    }

            //    // Pre-close per preparare la firma
            //    int csize = 10000;
            //    Dictionary<PdfName, int?> exc = new Dictionary<PdfName, int?>();
            //    exc[PdfName.Contents] = csize * 2 + 2; // Reserve space for signature bytes
            //    signer.CustomPreClose(exc);

            //    // Configurazione firma
            //    // 1. Configure Signature Filter/Subfilter (Crypto Dictionary)
            //    // ----------------------------------------------------------
            //    // Get the signature dictionary from the PdfSigner
            //    PdfSignature pdfSignature = signer.GetSignatureDictionary();

            //    // Set filter and subfilter (equivalent to old crypto dictionary setup)
            //    pdfSignature.Put(PdfName.Filter, PdfName.Adobe_PPKLite);
            //    pdfSignature.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_detached);


            //    // Calcolo hash se non abbiamo la firma
            //    if (signature == null)
            //    {
            //        //using (Stream s = signer.GetRangeStream())
            //        //{
            //        //    return ComputeSha256(s);
            //        //}
            //        return GenerateSignature(signer);
            //    }

            //    // Applicazione della firma
            //    PdfDictionary dic2 = new PdfDictionary();
            //    byte[] paddedSignature = new byte[csize];
            //    Array.Copy(signature, 0, paddedSignature, 0, signature.Length);

            //    dic2.Put(PdfName.Contents, new PdfString(paddedSignature).SetHexWriting(true));
            //    signer.CustomClose(dic2);

            //    return outMs.ToArray();
            //}
        }
    }
}
