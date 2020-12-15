using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace DocsPaRESTWS.Model.Decorators
{
    public class PaginatorDecorator<C> : ListDecorator<C>
    {
        private int _requestedPage;
        private int _pageSize;
        private int _totalResultCount;
        private ILog logger = LogManager.GetLogger(typeof(PaginatorDecorator<C>));

        public PaginatorDecorator(int requestedPage, int pageSize,List<C> list) : base(list)
        {
            _requestedPage = requestedPage;
            _pageSize = pageSize;
        }

        public PaginatorDecorator(int requestedPage, int pageSize, ListDecorator<C> decorator)
            : base(decorator)
        {
            _requestedPage = requestedPage;
            _pageSize = pageSize;
        }


        private int getBegin(List<C> input)
        {
            int temp = (_requestedPage - 1) * _pageSize;
            if (temp > input.Count - 1) return 0;
            return temp;
        }

        private int getEnd(List<C> input)
        {
            if (getBegin(input) > input.Count - 1) return 0;
            int temp = getBegin(input) + _pageSize;
            return Math.Min(input.Count, temp);
        }

        public int TotalResultCount
        {
            get
            {
                return _totalResultCount;
            }
        }

        protected override List<C> executeList(List<C> input)
        {
            logger.Info("begin");
            List<C> res = new List<C>();
            _totalResultCount = input.Count;
            int begin = getBegin(input);
            int end = getEnd(input);
            logger.Debug("totalResultCount: " + _totalResultCount + ", begin: " + begin + ", end: " + end);
            for (int i = begin; i < end; i++) res.Add(input[i]);
            return res;
        }
    }

}