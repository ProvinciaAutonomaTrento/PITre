// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using Pi3.App.AggregazioneDocumentale.WebApi.Application;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using System.Text;

namespace System;

public static class Helpers
{
    public static bool IsValidAggregateId(this string id) { 
        if (string.IsNullOrWhiteSpace(id)) return false;

        if (!int.TryParse(id, out var identifier))
            return false;
        else if (identifier <= 0) return false;
        else return true;
    }

    public static async Task<string> GetRawBodyAsync(
        this HttpRequest request,
        Encoding encoding = null)
    {
        if (!request.Body.CanSeek)
        {
            // We only do this if the stream isn't *already* seekable,
            // as EnableBuffering will create a new stream instance
            // each time it's called
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

        var body = await reader.ReadToEndAsync().ConfigureAwait(false);

        request.Body.Position = 0;

        return body;
    }

    public static IEnumerable<Link> GetLinksUrl(string instanceId, string? idAggregato = null, 
        string idDocumento = null, string idSottofascicolo = null, string idNota = null,
        string idTrasmissione = null, string codiceModello = null) {
        
        var result = JsonConvert.DeserializeObject<IEnumerable<Link>>(Files.IndexGetHeader_Actual);

        foreach (var link in result) {
            if (!string.IsNullOrWhiteSpace(idAggregato))
                link.url = link.url.Replace("{idAggregato}", idAggregato);
            if(!string.IsNullOrWhiteSpace(instanceId))
                link.url = link.url.Replace("{instance}", instanceId);
            if (!string.IsNullOrWhiteSpace(idDocumento))
                link.url = link.url.Replace("{idDocumento}", idDocumento);
            if (!string.IsNullOrWhiteSpace(idSottofascicolo))
                link.url = link.url.Replace("{idSottofascicolo}", idSottofascicolo);
            if (!string.IsNullOrWhiteSpace(idNota))
                link.url = link.url.Replace("{idNota}", idNota);
            if (!string.IsNullOrWhiteSpace(codiceModello))
                link.url = link.url.Replace("{codiceModello}", idNota);
        }
        return result;
    }

}
