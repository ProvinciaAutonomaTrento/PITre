// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using iText.Kernel.Pdf;
using iText.Signatures;
using System.ComponentModel.Design;
using System.Reflection.PortableExecutable;

namespace Signing.Api;

public interface IPdfSigningService
{
    (byte[] PreparedPdf, byte[] Hash) PrepareDocumentForSigning(byte[] pdfContent, string fieldName, DateTime signDate);
    byte[] ApplySignatureToDocument(byte[] preparedPdf, byte[] signature, string fieldName);
}

public class PdfSigningService : IPdfSigningService
{
    public (byte[] PreparedPdf, byte[] Hash) PrepareDocumentForSigning(byte[] pdfContent, string fieldName, DateTime signDate)
    {
        using var inputStream = new MemoryStream(pdfContent);
        using var outputStream = new MemoryStream();

        // Crea il PdfReader per il documento originale
        var reader = new PdfReader(inputStream);
        var signer = new PdfSigner(reader, outputStream, new StampingProperties());

        // Configura il campo firma
        signer.SetFieldName(fieldName);
        signer.SetSignDate(signDate);

        // Configura l'aspetto della firma
        signer.GetSignatureAppearance()
            .SetReason("Firma digitale")
            .SetLocation("Sistema di firma remota")
            .SetReuseAppearance(false);

        // Calcola l'hash usando un container specializzato come nella tua classe di test
        var digestContainer = new DigestContainer(new SignatureData(fieldName, signDate));
        signer.SignExternalContainer(digestContainer, 8192);

        // Restituisci sia il PDF preparato che l'hash calcolato
        return (outputStream.ToArray(), digestContainer.GetHash());
    }

    public byte[] ApplySignatureToDocument(byte[] preparedPdf, byte[] signature, string fieldName)
    {
        using var inputStream = new MemoryStream(preparedPdf);
        using var outputStream = new MemoryStream();

        // Crea un nuovo PdfReader dal PDF preparato
        using var reader = new PdfReader(inputStream);
        using var pdfDoc = new PdfDocument(reader);

        // Crea un container per la firma già calcolata
        var container = new HashOnlySignatureContainer(signature);

        // Applica la firma al documento usando SignDeferred
        using var finalStream = new MemoryStream();

        // Il nome del campo firma deve essere lo stesso usato durante la fase di preparazione
        PdfSigner.SignDeferred(pdfDoc, fieldName, finalStream, container);

        return finalStream.ToArray();
    }

    private record SignatureData(string FieldName, DateTime SignDate);

    private class DigestContainer : IExternalSignatureContainer
    {
        private byte[] _hash;
        private readonly SignatureData _signatureData;

        public DigestContainer(SignatureData signatureData)
        {
            _signatureData = signatureData;
        }

        public void ModifySigningDictionary(PdfDictionary signDic)
        {
            signDic.Put(PdfName.Filter, PdfName.Adobe_PPKLite);
            signDic.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_detached);
            signDic.Put(PdfName.M, new PdfDate(_signatureData.SignDate).GetPdfObject());
        }

        public byte[] Sign(Stream data)
        {
            using var ms = new MemoryStream();
            data.CopyTo(ms);
            _hash = ms.ToArray();

            // Return empty array since we're just calculating the hash
            return new byte[0];
        }

        public byte[] GetHash() => _hash;
    }

    private class HashOnlySignatureContainer : IExternalSignatureContainer
    {
        private readonly byte[] _signature;

        public HashOnlySignatureContainer(byte[] signature)
        {
            _signature = signature;
        }

        public void ModifySigningDictionary(PdfDictionary signDic)
        {
            signDic.Put(PdfName.Filter, PdfName.Adobe_PPKLite);
            signDic.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_detached);
        }

        public byte[] Sign(Stream data) => _signature;
    }
}