using System;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class TrasmInfo
    {
        [JsonProperty("Accettata")]
        public bool accettata { get; set; }

        [JsonProperty("Data")]
        public string data { get; set; }

        [JsonProperty("HasWorkflow")]
        public bool hasWorkflow { get; set; }

        [JsonProperty("IdTrasm")]
        public string idTrasm { get; set; }

        [JsonProperty("IdTrasmUtente")]
        public string idTrasmUtente { get; set; }

        [JsonProperty("Mittente")]
        public string mittente { get; set; }

        [JsonProperty("NoteGenerali")]
        public string noteGenerali { get; set; }

        [JsonProperty("NoteIndividuali")]
        public string noteIndividuali { get; set; }

        [JsonProperty("Ragione")]
        public string ragione { get; set; }

        [JsonProperty("Rifiutata")]
        public bool rifiutata { get; set; }

        public TrasmInfo()
        {
        }

        public TrasmInfo(bool accettata, string data, bool hasWorkflow, string idTrasm, 
                         string idTrasmUtente, string mittente, string noteGenerali, 
                         string noteIndividuali, string ragione, bool rifiutata)
        {
            this.accettata = accettata;
            this.data = data;
            this.hasWorkflow = hasWorkflow;
            this.idTrasm = idTrasm;
            this.idTrasmUtente = idTrasmUtente;
            this.mittente = mittente;
            this.noteGenerali = noteGenerali;
            this.noteIndividuali = noteIndividuali;
            this.ragione = ragione;
            this.rifiutata = rifiutata;
        }
    }
}
