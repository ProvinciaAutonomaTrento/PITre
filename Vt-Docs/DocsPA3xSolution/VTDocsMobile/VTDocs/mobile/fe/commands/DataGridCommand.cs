using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTDocs.mobile.fe.datagrid;

namespace VTDocs.mobile.fe.commands
{
    public abstract class DataGridCommand<C> : Command<DataGridResult<C>,DataGridInput>
    {
        public override abstract DataGridResult<C> execute(DataGridInput input);
    }
}
