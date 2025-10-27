// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
	public class AssDocFascRuoli
	{
        public string ID_TIPO_DOC_FASC;
        public string ID_OGGETTO_CUSTOM;
        public string ID_GRUPPO;
        public string DIRITTI_TIPOLOGIA;
        public string INS_MOD_OGG_CUSTOM;
        public string VIS_OGG_CUSTOM;
        public string ANNULLA_REPERTORIO;

        public AssDocFascRuoli() { }		
	}
}
