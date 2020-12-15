using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Navigation
{
    public class NavigationObject : ICloneable
    {
        public string NamePage {  get; set; }
        public FiltroRicerca[][] SearchFilters { get; set; }
        public string IdObject { get; set; }
        public string Type { get; set; }
        public string Link { get;set; }
        public string CodePage { get; set; }
        public string Page { get; set; }
        public string NumPage { get; set; }
        public string DxTotalNumberElement { get; set; }
        public string DxTotalPageNumber { get; set; }
        public string DxPositionElement { get; set; }
        public Registro RegistryFilter { get; set; }
        public FascicolazioneClassificazione Classification { get; set; }
        public string PageSize { get; set; }
        public bool ViewResult { get; set; }
        public string OriginalObjectId { get; set; }
        public Folder folder { get; set; }
        public FiltroRicerca[][] SearchFiltersOrder { get; set; }
        public string idProject { get; set; }
        public bool SearchTransmission { get; set; }
        public string TypeTransmissionSearch { get; set; }
        public List<Notification> ListNotification { get; set; }
        public bool FromNotifyCenter { get; set; }
        public string IdInstance { get; set; }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }



}