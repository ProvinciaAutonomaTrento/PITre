using System;

namespace DocsPAWA.SitoAccessibile.Paging
{
	/// <summary>
	/// Classe che specifica i criteri relativi alle ricerche paginate
	/// </summary>
	[Serializable()]
	public class PagingContext
	{
		private int _pageNumber=0;
		private int _pageCount=0;
		private int _recordCount=0;
        private int _pageSize = 10;

        public PagingContext()
        {
        }

        public PagingContext(int pageNumber)
        {
            this.PageNumber = pageNumber;
        }

        public PagingContext(int pageNumber, int pageSize)
            : this(pageNumber)
        {
            this.PageSize = pageSize;
        }

		public int PageNumber
		{
			get
			{
				return this._pageNumber;
			}
			set
			{
				this._pageNumber=value;

				if (this._pageNumber<0)
					this._pageNumber=0;
			}
		}

        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                this._pageSize = value;

                if (this._pageSize < 0)
                    this._pageSize = 10;
            }
        }
		public int PageCount
		{
			get
			{
				return this._pageCount;
			}
			set
			{
				this._pageCount=value;

				if (this._pageCount<0)
					this._pageCount=0;
			}
		}

		public int RecordCount
		{
			get
			{
				return this._recordCount;
			}
			set
			{
				this._recordCount=value;

				if (this._recordCount<0)
					this._recordCount=0;
			}
		}

        public String[] IdProfilesList { get; set; }
        public bool GetIdProfilesList { get; set; }
	}
}
