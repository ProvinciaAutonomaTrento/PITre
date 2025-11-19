// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Trasmissioni;
using Serilog;

namespace Pi3.App.Legacy.Mobile.WebApi.Model.Decorators
{
    public class PaginatorDecorator<C> : ListDecorator<C>
    {
        private int _requestedPage;
        private int _pageSize;
        private int _totalResultCount;
        private static Serilog.ILogger logger = Log.ForContext(typeof(PaginatorDecorator<C>));

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
            int tempBegin = (_requestedPage - 1) * _pageSize;
            if (tempBegin > input.Count - 1) return 0;
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
            logger.Information("begin");
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