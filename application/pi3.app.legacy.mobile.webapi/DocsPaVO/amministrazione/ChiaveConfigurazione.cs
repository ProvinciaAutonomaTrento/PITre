// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
    [DataContract]
    public class ChiaveConfigurazione
    {
        [DataMember]
        public string IDChiave { get; set; } = string.Empty;
        [DataMember]
        public string Codice { get; set; } = string.Empty;
        [DataMember]
        public string Descrizione { get; set; } = string.Empty;
        [DataMember]
        public string Valore { get; set; } = string.Empty;
        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;
        [DataMember]
        public string TipoChiave { get; set; } = string.Empty;
        [DataMember]
        public string Visibile { get; set; } = string.Empty;
        [DataMember]
        public string Modificabile { get; set; } = string.Empty;
        [DataMember]
        public string IsGlobale { get; set; } = string.Empty;
        [DataMember]
        public string IsConservazione { get; set; } = string.Empty;
    }

    public class ConfigRepository : Hashtable
    {
        ArrayList listaChiavi = null;

        public ArrayList ListaChiavi
        {
            get
            {
                return listaChiavi;
            }

            set
            {
                listaChiavi = value;
                for (int i = 0; listaChiavi != null && i < listaChiavi.Count; i++)
                    //Add(((ChiaveConfigurazione)listaChiavi[i]).Codice, listaChiavi[i]);
                    //memorizzo solo il valore, non l'intera chiave. Non serve
                    Add(((ChiaveConfigurazione)listaChiavi[i]).Codice, ((ChiaveConfigurazione)listaChiavi[i]).Valore);
            }
        }

    }
}
