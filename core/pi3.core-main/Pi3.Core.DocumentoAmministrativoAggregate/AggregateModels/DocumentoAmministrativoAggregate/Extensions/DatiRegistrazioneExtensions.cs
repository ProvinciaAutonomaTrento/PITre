// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Metadati;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Extensions
{
    /*
    internal static class DatiRegistrazioneExtensions
    {
        public static DatiDiRegistrazioneType AsDatiRegistrazioneType(this DatiRegistrazione datiRegistrazione)
        {
            datiRegistrazione = datiRegistrazione ?? throw new ArgumentNullException(nameof(datiRegistrazione));

            var type = new DatiDiRegistrazioneType();

            type.TipologiaDiFlusso = Enum.Parse<TipologiaDiFlussoType>(datiRegistrazione!.TipologiaFlusso.ToString());

            object? item = null;

            if (datiRegistrazione.IsRegistrato)
            {
                if (datiRegistrazione is DatiRegistrazioneProtocollo)
                {
                    var datiRegistrazioneProtocollo = (DatiRegistrazioneProtocollo)datiRegistrazione;

                    item = new ProtocolloType()
                    {
                        DataProtocollazioneDocumento = datiRegistrazioneProtocollo.DataProtocollazione!.Value,
                        NumeroProtocolloDocumento = datiRegistrazioneProtocollo.NumeroProtocolloAsString,
                        CodiceRegistro = datiRegistrazioneProtocollo.CodiceRegistro,
                        TipoRegistro = "ProtocolloOrdinario_ProtocolloEmergenza"
                    };
                }
                else if (datiRegistrazione is DatiRegistrazioneRepertorio)
                {
                    var datiRegistrazioneRepertorio = (DatiRegistrazioneRepertorio)datiRegistrazione;

                    item = new NoProtocolloType()
                    {
                        DataRegistrazioneDocumento = datiRegistrazioneRepertorio.DataRegistrazione!.Value,
                        NumeroRegistrazioneDocumento = datiRegistrazioneRepertorio.NumeroRegistrazione.ToString(),
                        CodiceRegistro = datiRegistrazioneRepertorio.CodiceRegistro,
                        TipoRegistro = "Repertorio_Registro"
                    };
                }
            }

            if (item != null)
            {
                type.TipoRegistro = new TipoRegistroType()
                {
                    Item = item
                };
            }

            return type;
        }
    }

    */

    internal static class DatiRegistrazioneExtensions
    {
        public static DatiDiRegistrazioneType AsDatiRegistrazioneType(this DocumentoAmministrativo documentoAmministrativo)
        {
            var type = new DatiDiRegistrazioneType();
            if (documentoAmministrativo.DatiRegistrazione! != null! && documentoAmministrativo.DatiRegistrazione.IsRegistrato)
            {
                object? item = null;

                type.TipologiaDiFlusso = Enum.Parse<TipologiaDiFlussoType>(documentoAmministrativo.DatiRegistrazione!.TipologiaFlusso.ToString());

                if (documentoAmministrativo.DatiRegistrazione.IsRegistrato)
                {
                    if (documentoAmministrativo.DatiRegistrazione is DatiRegistrazioneProtocollo)
                    {
                        var datiRegistrazioneProtocollo = (DatiRegistrazioneProtocollo)documentoAmministrativo.DatiRegistrazione;

                        item = new ProtocolloType()
                        {
                            DataProtocollazioneDocumento = datiRegistrazioneProtocollo.DataProtocollazione!.Value,
                            NumeroProtocolloDocumento = datiRegistrazioneProtocollo.NumeroProtocolloAsString,
                            CodiceRegistro = datiRegistrazioneProtocollo.CodiceRegistro,
                            //TipoRegistro = "ProtocolloOrdinario_ProtocolloEmergenza"
                        };
                    }
                    else if (documentoAmministrativo.DatiRegistrazione is DatiRegistrazioneRepertorio)
                    {
                        var datiRegistrazioneRepertorio = (DatiRegistrazioneRepertorio)documentoAmministrativo.DatiRegistrazione;

                        item = new NoProtocolloType()
                        {
                            DataRegistrazioneDocumento = datiRegistrazioneRepertorio.DataRegistrazione!.Value,
                            NumeroRegistrazioneDocumento = datiRegistrazioneRepertorio.NumeroRegistrazione.ToString(),
                            CodiceRegistro = datiRegistrazioneRepertorio.CodiceRegistro,
                            //TipoRegistro = "Repertorio_Registro"
                        };
                    }
                }

                if (item != null)
                {
                    type.TipoRegistro = new TipoRegistroType()
                    {
                        Item = item
                    };
                }
            }
            else
            {
                type = new DatiDiRegistrazioneType
                {
                    TipologiaDiFlusso = TipologiaDiFlussoType.I,
                    TipoRegistro = new TipoRegistroType
                    {
                        Item = new NoProtocolloType
                        {
                            DataRegistrazioneDocumento = documentoAmministrativo.CreationDate,
                            NumeroRegistrazioneDocumento = documentoAmministrativo.Id,
                            CodiceRegistro = documentoAmministrativo.Registro!.Codice
                        }
                    }
                };
            }

            return type;
        }
    }
}
