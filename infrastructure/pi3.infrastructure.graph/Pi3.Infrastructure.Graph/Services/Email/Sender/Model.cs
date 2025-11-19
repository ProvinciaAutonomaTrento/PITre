// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Graph.Services.Email.Sender
{

    public class MSGetTokenResponse
    {
        /*Indicates the token type value. The only type that Azure AD supports is Bearer.*/
        public string token_type { get; set; }

        /*How long the access token is valid (in seconds).*/
        public int expires_in { get; set; }

        /*Used to indicate an extended lifetime for the access token and to support resiliency when the token issuance service is not responding.*/
        public int ext_expires_in { get; set; }

        /*The requested access token. Your app can use this token in calls to Microsoft Graph.*/
        public string access_token { get; set; }
    }

    public class MSSendMessageRequest
    {
        public MSMessage Message { get; set; }
    }

    public class MSMessage
    {
        public string Subject { get; set; }
        public MSItemBody Body { get; set; }
        public MSRecipient From { get; set; }
        public List<MSRecipient> ToRecipients { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSRecipient> CcRecipients { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSRecipient> BccRecipients { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSAttachment> Attachments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSInternetMessageHeader> InternetMessageHeaders { get; set; }
    }

    public class MSRecipient
    {
        public MSEmailAddress EmailAddress { get; set; }
    }

    public class MSEmailAddress
    {
        public string Address { get; set; }
    }

    public class MSItemBody
    {
        public string ContentType { get; set; }
        public string Content { get; set; }

    }

    public class MSInternetMessageHeader
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class MSAttachment
    {
        public string ContentType { get; set; }
        public string Name { get; set; }

        [JsonProperty(PropertyName = "@odata.type")]
        public string OdataType { get; set; }
        public string ContentBytes { get; set; }
    }
}
