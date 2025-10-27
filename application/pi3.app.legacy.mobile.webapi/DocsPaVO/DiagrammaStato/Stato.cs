// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
    [DataContract]
    public class Stato
	{
        [DataMember]
        public int SYSTEM_ID { get; set; } = 0;
        [DataMember]
        public int ID_DIAGRAMMA { get; set; } = 0;
        [DataMember]
        public string DESCRIZIONE { get; set; } = "";
        [DataMember]
        public bool STATO_INIZIALE { get; set; } = false;
        [DataMember]
        public bool STATO_FINALE { get; set; } = false;
        [DataMember]
        public bool CONVERSIONE_PDF { get; set; } = false;
        [DataMember]
        public bool NON_RICERCABILE { get; set; } = false;
        [DataMember]
        public bool STATO_SISTEMA { get; set; } = false;
        [DataMember]
        public DocsPaVO.documento.DocumentConsolidationStateEnum STATO_CONSOLIDAMENTO { get; set; } = documento.DocumentConsolidationStateEnum.None;
        [DataMember]
        public string ID_PROCESSO_FIRMA { get; set; } = string.Empty;
        [DataMember]
        public bool PUBBLICAZIONE_FILES { get; set; } = false;
	}
}
