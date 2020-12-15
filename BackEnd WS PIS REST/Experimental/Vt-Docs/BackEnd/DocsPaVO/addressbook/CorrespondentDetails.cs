using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.addressbook
{
    public class CorrespondentDetails
    {

        #region Fields

        private string systemId = string.Empty;
        private string idCorr = string.Empty;
        private string address = string.Empty;
        private string zipCode = string.Empty;
        private string district = string.Empty;
        private string country = string.Empty;
        private string phone = string.Empty;
        private string phone2 = string.Empty;
        private string fax = string.Empty;
        private string note = string.Empty;
        private string taxId = string.Empty;
        private string city = string.Empty;
        private string place = string.Empty;
        private string birthPlace = string.Empty;
        private string birthDay = string.Empty;
        private string title = string.Empty;
        private string commercialId = string.Empty;

        #endregion

        #region Properties

        public string SystemId
        {
            get
            {
                return this.systemId;
            }
            set
            {
                this.systemId = value;
            }
        }

        public string IdCorr
        {
            get
            {
                return this.idCorr;
            }
            set
            {
                this.idCorr = value;
            }
        }

        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
            }
        }

        public string ZipCode
        {
            get
            {
                return this.zipCode;
            }
            set
            {
                this.zipCode = value;
            }
        }

        public string District
        {
            get
            {
                return this.district;
            }
            set
            {
                this.district = value;
            }
        }

        public string Country
        {
            get
            {
                return this.country;
            }
            set
            {
                this.country = value;
            }
        }

        public string Phone
        {
            get
            {
                return this.phone;
            }
            set
            {
                this.phone = value;
            }
        }

        public string Phone2
        {
            get
            {
                return this.phone2;
            }
            set
            {
                this.phone2 = value;
            }
        }

        public string Fax
        {
            get
            {
                return this.fax;
            }
            set
            {
                this.fax = value;
            }
        }

        public string Note
        {
            get
            {
                return this.note;
            }
            set
            {
                this.note = value;
            }
        }

        public string TaxId
        {
            get
            {
                return this.taxId;
            }
            set
            {
                this.taxId = value;
            }
        }

        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                this.city = value;
            }
        }

        public string Place
        {
            get
            {
                return this.place;
            }
            set
            {
                this.place = value;
            }
        }

        public string BirthPlace
        {
            get
            {
                return this.birthPlace;
            }
            set
            {
                this.birthPlace = value;
            }
        }

        public string BirthDay
        {
            get
            {
                return this.birthDay;
            }
            set
            {
                this.birthDay = value;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        public string CommercialId
        {
            get
            {
                return this.commercialId;
            }
            set
            {
                this.commercialId = value;
            }
        }

        #endregion


        public CorrespondentDetails()
        {

		}

    }
}
