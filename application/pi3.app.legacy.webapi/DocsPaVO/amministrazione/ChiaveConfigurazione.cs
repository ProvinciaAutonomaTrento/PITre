// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
    public class ChiaveConfigurazione
    {
        public string IDChiave = string.Empty;
        public string Codice = string.Empty;
        public string Descrizione = string.Empty;
        public string Valore = string.Empty;
        public string IDAmministrazione = string.Empty;
        public string TipoChiave = string.Empty;
        public string Visibile = string.Empty;
        public string Modificabile = string.Empty;
        public string IsGlobale = string.Empty;
        public string IsConservazione = string.Empty;
    }

    public class ConfigRepository : Hashtable
    {
        ChiaveConfigurazione[] listaChiavi = null;

        public ChiaveConfigurazione[] ListaChiavi
        {
            get
            {
                return listaChiavi;
            }

            set
            {
                listaChiavi = value;
                for (int i = 0; listaChiavi != null && i < listaChiavi.Length; i++)
                    //Add(((ChiaveConfigurazione)listaChiavi[i]).Codice, listaChiavi[i]);
                    //memorizzo solo il valore, non l'intera chiave. Non serve
                    Add(((ChiaveConfigurazione)listaChiavi[i]).Codice, ((ChiaveConfigurazione)listaChiavi[i]).Valore);
            }
        }

    }
}
