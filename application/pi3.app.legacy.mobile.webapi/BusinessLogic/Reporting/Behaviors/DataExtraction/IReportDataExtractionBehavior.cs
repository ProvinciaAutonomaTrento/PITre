// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Data;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting.Behaviors.DataExtraction
{
    /// <summary>
    /// Questa interfaccia definisce il contratto che deve essere implementato da una
    /// classe che intenda realizzare un behavior per l'estrazione di dati per la
    /// generazione di reportistica
    /// </summary>
    public interface IReportDataExtractionBehavior
    {
        /// <summary>
        /// Metodo per l'estrazione dei dati per la reportistica
        /// </summary>
        /// <param name="request">Informazioni sull'azione da compiere</param>
        /// <returns>DataSet con i dati estratti</returns>
        DataSet ExtractData(PrintReportRequest request);
    }
}
