// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.FlussoAutomatico
{
    [DataContract]
    public class ContestoProcedurale
    {
        private string systemId;
        private string tipoContestoProcedurale;
        private string nome;
        private string famiglia;
        private string versione;

        [DataMember]
        public string SYSTEM_ID
        {
            get
            {
                return systemId;
            }
            set
            {
                systemId = value;
            }
        }

        [DataMember]
        public string TIPO_CONTESTO_PROCEDURALE
        {
            get
            {
                return tipoContestoProcedurale;
            }
            set
            {
                tipoContestoProcedurale = value;
            }
        }

        [DataMember]
        public string NOME
        {
            get
            {
                return nome;
            }
            set
            {
                nome = value;
            }
        }

        [DataMember]
        public string FAMIGLIA
        {
            get
            {
                return famiglia;
            }
            set
            {
                famiglia = value;
            }
        }

        [DataMember]
        public string VERSIONE
        {
            get
            {
                return versione;
            }
            set
            {
                versione = value;
            }
        }
    }
}
