// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StampaPDF;

public class StampaPDF
{
    public static DocumentPDF createDocPDF(StampaVO.Document templateDoc, string overrideFooter)
    {
        DocumentPDF docPDF = null;
        if (templateDoc == null)
            throw new ReportException(ErrorCode.InvalidValueObject, "Template di formato non valido");
#if false
        try
        {
            try
            {//Creazione del Documento vuoto
                docPDF = new DocumentPDF();
                docTmp = templateDoc;
                writer = PdfWriter.GetInstance(docPDF, docPDF.memoryStream);
                PDFPageEvents events = new PDFPageEvents();
                writer.PageEvent = events;

            }
            catch (Exception e)
            {
                writer.Close();
                throw new ReportException(ErrorCode.BadPDFFile, e.Message);
            }

            try
            {
                //Settaggio delle proprietà della pagina
                docPDF = setPage(templateDoc, docPDF);
            }
            catch (Exception ex)
            {
                writer.Close();
                throw new ReportException(ErrorCode.IncompletePDFFile, "Fallita impostazione della Pagina: " + ex.Message);
            }

            try
            {
                //Creazione dell'header
                docPDF = setHeader(templateDoc, docPDF);
            }
            catch (Exception ex)
            {
                writer.Close();
                throw new ReportException(ErrorCode.IncompletePDFFile, "Fallita impostazione dell'Header: " + ex.Message);
            }

            try
            {
                //Creazione del footer
                docPDF = setFooter(templateDoc, docPDF, overrideFooter);
            }
            catch (Exception ex)
            {
                writer.Close();
                throw new ReportException(ErrorCode.IncompletePDFFile, "Fallita impostazione del Footer: " + ex.Message);
            }

            try
            {
                if (!docPDF.AddCreationDate())
                    throw new Exception();
            }
            catch (Exception ex)
            {
                throw new ReportException(ErrorCode.IncompletePDFFile, "Fallita impostazione della Data di Stampa: " + ex.Message);
            }

            docPDF.Open();

            try
            {
                docPDF = setLogo(templateDoc, docPDF);
            }
            catch (Exception ex)
            {
                docPDF.Close();
                writer.Close();
                throw new ReportException(ErrorCode.IncompletePDFFile, "Fallita impostazione del Logo: " + ex.Message);
            }
        }
        catch (ReportException re)
        {
            docPDF.Close();
            writer.Close();
            docPDF = null;
            throw re;
        }
        catch (Exception e)
        {
            docPDF.Close();
            writer.Close();
            docPDF = null;
            throw new ReportException(ErrorCode.GenericError, e.Message);
        } 
#endif

        return docPDF;

    }

}
