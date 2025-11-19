// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public enum StatiConsolidamentoEnum
    {
        /// <summary>
        /// Consolidato il documento in azioni fondamentali che determinano un incremento o modifica delle versioni
        /// </summary>
        Livello1 = 1,

        /// <summary>
        /// Consolidato il documento nei suoi metadati fondamentali
        /// </summary>
        Livello2 = 2,
    }

    ///// <summary>
    ///// Enumerazione delle azioni sottoposte a controllo nell'operazione di consolidamento
    ///// </summary>
    //public enum AzioniNonConsentiteConsolidaentoEnum
    //{
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    AddVersions,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    RemoveVersions,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    ModifyVersions,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    AddAllegato,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    RemoveAllegato,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    ModifyAllegato,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    Delete,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
    //    Firma,
    //    [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
    //    AnnullaProtocollo,
    //}

    ///// <summary>
    ///// Enumerazione dei metadati sottoposti a controllo nell'operazione di consolidamento
    ///// </summary>
    //public enum ModificheMetadatiNonConsentiteConsolidamentoEnum
    //{
    //    [StatoConsolidamentoAttribute(StatoConsolidamentoAttribute.)]
    //    Oggetto,                     // Oggetto documento
    //    [StatoConsolidamentoAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
    //    Mittenti,                    // Mittenti
    //    [StatoConsolidamentoAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
    //    Destintari,                 // Destinatari
    //    [StatoConsolidamentoAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
    //    DataArrivo,                // Data arrivo
    //    [StatoConsolidamentoAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
    //    OraArrivo,                // Ora arrivo
    //}

    ///// <summary>
    ///// Attributo di utilità da associare agli enumerations per determinare facilmente lo stato di consolidamento
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    //public class StatoConsolidamentoAttribute : Attribute
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="state"></param>
    //    public StatoConsolidamentoAttribute(StatiConsolidamentoEnum stato)
    //    {
    //        this.Stato = stato;
    //    }

    //    /// <summary>
    //    /// Stato di consolidamento del documento
    //    /// </summary>
    //    public StatiConsolidamentoEnum Stato
    //    {
    //        get;
    //        set;
    //    }
    //}

    public class Consolidamento : ValueObject
    {
        public Consolidamento()
        { }

        [Required]
        public StatiConsolidamentoEnum Stato { get; init; }

        [Required]
        public DateTime Data { get; init; }

        [Required]
        public Autore Autore { get; init; }
    }
}
