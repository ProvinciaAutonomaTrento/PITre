// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model.Decorators
{
    public abstract class ListDecorator<C>
    {
        protected ListDecorator<C> _decorator;
        protected List<C> _list;

        protected ListDecorator(ListDecorator<C> decorator)
        {
            this._decorator = decorator;
        }

        protected ListDecorator(List<C> list)
        {
            this._list = list;
        }

        protected ListDecorator()
        {
        }

        public virtual List<C> execute(){
            if(_list==null && _decorator==null) return null;
            List<C> input=_list;
            if (_list == null) input = _decorator.execute();
            return executeList(input);
        }

        protected abstract List<C> executeList(List<C> input);

    }
}