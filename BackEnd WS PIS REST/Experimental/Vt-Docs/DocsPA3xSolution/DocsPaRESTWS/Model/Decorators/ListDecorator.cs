using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model.Decorators
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