// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Entities;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Services.DigitalPreservation;
using Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.ParER.Services.DigitalPreservation
{
    public static class ServiceExtensions
    {
        public static TipologiaUnitaDocumentariaEnum? GetTipologiaUnitaDocumentaria(this DocumentoAmministrativo d)
        {
            if (d.DatiRegistrazione is not null && d.DatiRegistrazione is DatiRegistrazioneProtocollo && d.DatiRegistrazione.IsRegistrato)
                return TipologiaUnitaDocumentariaEnum.DocumentoProtocollato;

            if(d.Profiles != null && d.Profiles.Any() 
                && d.ConservaTipologia()
                && d.GetContatore() != null
                && !string.IsNullOrEmpty(d.GetContatore().Value.ToString())
                && d.ConservaContatore())
            {
                var tipologia = d.Profiles.FirstOrDefault()?.Name.Value;

                if (tipologia.ToUpper() == Resources.Resources.DescrizioneTipologiaFatturaElettronica.ToUpper()) return TipologiaUnitaDocumentariaEnum.FatturaElettronica;
                if (tipologia.ToUpper() == Resources.Resources.DescrizioneTipologiaLottoDiFatture.ToUpper()) return TipologiaUnitaDocumentariaEnum.LottoDiFatture;
                if (tipologia.ToUpper() == Resources.Resources.DescrizioneTipologiaFatturaElettronicaAttiva.ToUpper()) return TipologiaUnitaDocumentariaEnum.FatturaAttiva;
                if (tipologia.ToUpper() == Resources.Resources.DescrizioneTipologiaLottoDiFattureAttive.ToUpper()) return TipologiaUnitaDocumentariaEnum.LottoDiFattureAttive;
                if (tipologia.ToUpper() == Resources.Resources.DescrizioneTipologiaVerbaleDiSedutaOrgani.ToUpper()) return TipologiaUnitaDocumentariaEnum.VerbaleSinteticoDiSeduta;

                return TipologiaUnitaDocumentariaEnum.DocumentoRepertoriato;
            }

            if (d.DatiStampa is not null) return TipologiaUnitaDocumentariaEnum.StampaRegistro;

            return TipologiaUnitaDocumentariaEnum.DocumentoNonProtocollato;
        }

        public static ChiaveType? GetChiaveVersamento(this DocumentoAmministrativo d)
        {
            switch (d.GetTipologiaUnitaDocumentaria())
            {
                case TipologiaUnitaDocumentariaEnum.DocumentoProtocollato:
                    var datiRegistrazioneProtocollo = d.DatiRegistrazione as DatiRegistrazioneProtocollo;

                    return new ChiaveType
                    {
                        Numero = datiRegistrazioneProtocollo.NumeroProtocollo.ToString(),
                        Anno = datiRegistrazioneProtocollo.DataProtocollazione.Value.Year.ToString(),
                        TipoRegistro = string.Format(Resources.Resources.TipoRegistroDocumentoProtocollato, datiRegistrazioneProtocollo.CodiceRegistro.AsSIPIndexHeader())
                    };

                case TipologiaUnitaDocumentariaEnum.DocumentoRepertoriato:
                    var datiRegistrazioneRepertorio = d.DatiRegistrazione as DatiRegistrazioneRepertorio;

                    if (datiRegistrazioneRepertorio is null) return null;

                    //var contatore = d.Profiles.Where(p => p.)

                    return new ChiaveType
                    {
                        Numero = datiRegistrazioneRepertorio.NumeroRegistrazione.ToString(),
                        Anno = datiRegistrazioneRepertorio.DataRegistrazione!.Value.Year.ToString(),
                    };

                case TipologiaUnitaDocumentariaEnum.DocumentoNonProtocollato:
                    return new ChiaveType
                    {
                        Numero = d.Id,
                        Anno = d.CreationDate.Year.ToString(),
                        TipoRegistro = Resources.Resources.TipoRegistroDocumentoNonProtocolllato
                    };

                case TipologiaUnitaDocumentariaEnum.StampaRegistro:
                    if (d.DatiStampa.TipoStampa == TipologieStampaEnum.StampaRegistroProtocollo)
                    {
                        return new ChiaveType
                        {
                            Numero = d.Id,
                            Anno = d.CreationDate.Year.ToString(),
                            TipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroProtocollo, d.DatiStampa.CodiceRegistro)
                        };
                    }
                    else
                    {
                        switch (d.DatiStampa.TipoContatore)
                        {
                            case TipologieContatoriRepertorioEnum.AOO:
                                return new ChiaveType
                                {
                                    Numero = d.Id,
                                    Anno = d.CreationDate.Year.ToString(),
                                    TipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroRepertorioAOO, d.DatiStampa.CodiceRegistro!.AsSIPIndexHeader())
                                };
                            case TipologieContatoriRepertorioEnum.RF:
                                return new ChiaveType
                                {
                                    Numero = $"{d.DatiStampa.CodiceRegistro} - {d.Id}",
                                    Anno = d.CreationDate.Year.ToString(),
                                    TipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroRepertorioRF, "") // +Tipologia
                                };
                            default:
                                return null;
                        }
                    }


                default:
                    return null;
            }

        }


        internal static string AsSIPIndexHeader(this string text)
        {
            return text.Replace("/", "-")
                .Replace("\\", "-")
                .Replace("|", "-")
                .Replace("(", "-")
                .Replace(")", "-")
                .Replace(",", " ")
                .Replace(";", " ")
                .Replace(":", " ")
                .Replace("'", " ")
                .Replace("À", "A")
                .Replace("Á", "A")
                .Replace("È", "E")
                .Replace("É", "E")
                .Replace("É", "E")
                .Replace("Ì", "I")
                .Replace("Í", "I")
                .Replace("Ò", "O")
                .Replace("Ó", "O")
                .Replace("Ù", "U")
                .Replace("Ú", "U")
                .Replace("à", "a")
                .Replace("á", "a")
                .Replace("è", "e")
                .Replace("é", "e")
                .Replace("ì", "i")
                .Replace("í", "i")
                .Replace("ò", "o")
                .Replace("ó", "o")
                .Replace("ù", "u")
                .Replace("ú", "u");
        }

        internal static string AsSIPIndexField(this string text)
        {
            return text
                // Caratteri speciali (cod. 160-191)
                .Replace("`", "&#096;")
                .Replace("¡", "&#161;")
                .Replace("¢", "&#162;")
                .Replace("£", "&#163;")
                .Replace("¤", "&#164;")
                .Replace("¥", "&#165;")
                .Replace("¦", "&#166;")
                .Replace("§", "&#167;")
                .Replace("¨", "&#168;")
                .Replace("©", "&#169;")
                .Replace("ª", "&#170;")
                .Replace("«", "&#171;")
                .Replace("¬", "&#172;")
                .Replace("®", "&#174;")
                .Replace("¯", "&#175;")
                .Replace("°", "&#176;")
                .Replace("±", "&#177;")
                .Replace("²", "&#178;")
                .Replace("³", "&#179;")
                .Replace("´", "&#180;")
                .Replace("µ", "&#181;")
                .Replace("¶", "&#182;")
                .Replace("·", "&#183;")
                .Replace("¸", "&#184;")
                .Replace("¹", "&#185;")
                .Replace("º", "&#186;")
                .Replace("»", "&#187;")
                .Replace("¼", "&#188;")
                .Replace("½", "&#189;")
                .Replace("¾", "&#190;")
                .Replace("¿", "&#191;")
                // Lettere accentate e simili (cod. 192-255)
                .Replace("À", "&#192;")
                .Replace("Á", "&#193;")
                .Replace("Â", "&#194;")
                .Replace("Ã", "&#195;")
                .Replace("Ä", "&#196;")
                .Replace("Å", "&#197;")
                .Replace("Æ", "&#198;")
                .Replace("Ç", "&#199;")
                .Replace("È", "&#200;")
                .Replace("É", "&#201;")
                .Replace("Ê", "&#202;")
                .Replace("Ë", "&#203;")
                .Replace("Ì", "&#204;")
                .Replace("Í", "&#205;")
                .Replace("Î", "&#206;")
                .Replace("Ï", "&#207;")
                .Replace("Ð", "&#208;")
                .Replace("Ñ", "&#209;")
                .Replace("Ò", "&#210;")
                .Replace("Ó", "&#211;")
                .Replace("Ô", "&#212;")
                .Replace("Õ", "&#213;")
                .Replace("Ö", "&#214;")
                .Replace("×", "&#215;")
                .Replace("Ø", "&#216;")
                .Replace("Ù", "&#217;")
                .Replace("Ú", "&#218;")
                .Replace("Û", "&#219;")
                .Replace("Ü", "&#220;")
                .Replace("Ý", "&#221;")
                .Replace("Þ", "&#222;")
                .Replace("ß", "&#223;")
                .Replace("à", "&#224;")
                .Replace("á", "&#225;")
                .Replace("â", "&#226;")
                .Replace("ã", "&#227;")
                .Replace("ä", "&#228;")
                .Replace("å", "&#229;")
                .Replace("æ", "&#230;")
                .Replace("ç", "&#231;")
                .Replace("è", "&#232;")
                .Replace("é", "&#233;")
                .Replace("ê", "&#234;")
                .Replace("ë", "&#235;")
                .Replace("ì", "&#236;")
                .Replace("í", "&#237;")
                .Replace("í", "&#238;")
                .Replace("í", "&#239;")
                .Replace("í", "&#240;")
                .Replace("í", "&#241;")
                .Replace("ò", "&#242;")
                .Replace("ó", "&#243;")
                .Replace("ô", "&#244;")
                .Replace("õ", "&#245;")
                .Replace("ö", "&#246;")
                .Replace("÷", "&#247;")
                .Replace("ø", "&#248;")
                .Replace("ù", "&#249;")
                .Replace("ú", "&#250;")
                .Replace("û", "&#251;")
                .Replace("ü", "&#252;")
                .Replace("ý", "&#253;")
                .Replace("þ", "&#254;")
                .Replace("ÿ", "&#255;");
                
        }

        internal static string? AsSIPIndexDateString(this DateTime? date)
        {
            if (date.HasValue) return date.Value.AsSIPIndexDateString();
            else return null;
        }

        internal static string AsSIPIndexDateString(this DateTime date)
        {
            //return $"{date.Year}-{date.Month}-{date.Day}";
            return date.ToString("yyyy-MM-dd");
        }

        internal static string? AsSIPIndexTimeString(this DateTime? date)
        {
            if (date.HasValue) return date.Value.AsSIPIndexTimeString();
            else return null;
        }

        internal static string AsSIPIndexTimeString(this DateTime date)
        {
            return date.ToString("HH:mm:ss");
        }

        internal static string GetDescription(this Enum e)
        {
            var field = e.GetType().GetField(e.ToString());
            if (field == null)
                return e.ToString();

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }

            return e.ToString();
        }

        internal static string ComputeSHA256Hash(this byte[] bytes)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256.ToString()))
            return BitConverter.ToString(hashAlgorithm.ComputeHash(bytes)).Replace("-", string.Empty);
        }

        internal static DigitalPreservationStatusEnum AsDigitalPreservationStatus(this ECEsitoExtType e)
        {
            if (e == ECEsitoExtType.NEGATIVO) return DigitalPreservationStatusEnum.Rejected;
            else return DigitalPreservationStatusEnum.Accepted;
        }

        internal static string GetTipoProtocollo(this DocumentoAmministrativo d)
        {
            switch(d.TipologiaFlusso)
            {
                case TipologiaFlussoEnum.E:
                    return "A";
                case TipologiaFlussoEnum.U:
                    return "P";
                case TipologiaFlussoEnum.I:
                    return "I";
                default:
                    return "G";
            }
        }

        internal static ElementField? GetContatore(this DocumentoAmministrativo d)
        {
            return d.Profiles?.FirstOrDefault()?.Fields.FirstOrDefault(f => f.Type.ToUpper() == "CONTATORE") ?? null;
        }

        internal static bool IsRepertorioConservato(this DocumentoAmministrativo d)
        {
            if(d.Profiles is null) return false;

            var field = d.Profiles.First().Fields.Where(f => f.Value.GetType() == typeof(ElementFieldValue)).FirstOrDefault();

            return false;
        }
        
        internal static bool ConservaContatore(this DocumentoAmministrativo d)
        {
            var contatore = d.GetContatore();

            return (contatore.Metadata.GetValueOrDefault("CHA_CONS_REPERTORIO") ?? string.Empty) == "1" &&
                        contatore.Metadata.GetValueOrDefault("DTA_ANNULLAMENTO") is null;

        }

        internal static bool ConservaTipologia(this DocumentoAmministrativo d)
        {
            var profile = d.Profiles[0];

            return (profile.Metadata.GetValueOrDefault("CHA_INVIO_CONSERVAZIONE") ?? string.Empty) == "1";

        }

        internal static string? GetFieldValueOrNull(this DocumentoAmministrativo d, string fieldName)
        {
            var value = d.Profiles[0].Fields
                .FirstOrDefault(f => f.Name.Value.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                ?.Value?.ToString();

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        internal static Allegato? GetAllegatoFattura(this DocumentoAmministrativo d)
        {
            Allegato? allegato = null;

            if (d.Allegati!.Any())
            {
                var allegatoFattura = d.Allegati!.Where(a => a.Descrizione.Value != null
                                                        && a.IdDoc.FileName != null
                                                        && (a.IdDoc.FileName.ToUpper().EndsWith("XML") || a.IdDoc.FileName.ToUpper().EndsWith("XML.P7M")))
                                                .FirstOrDefault();

                if (allegatoFattura is not null)
                    allegato = allegatoFattura;
            }

            return allegato;
        }

    }
}
