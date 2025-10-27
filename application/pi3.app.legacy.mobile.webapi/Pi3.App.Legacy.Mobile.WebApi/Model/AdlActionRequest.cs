// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class AdlActionRequest
{
    public string ADLAction { get; set; }
    public string IdElemento { get; set; }
    public string TipoElemento { get; set; }

    //public ADLActions AdlAction
    //{
    //    get;
    //    set;
    //}

    //public DocInfo DocInfo
    //{
    //    get;
    //    set;
    //}
    
    //public enum ADLActions
    //{
    //    ADD,
    //    REMOVE
    //}
}