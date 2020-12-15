<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="SearchProject.aspx.cs" Inherits="NttDataWA.Popup.SearchProject" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
    <style type="text/css">
        .container
        {
            width: 98%;
            margin: 0 auto;
        }

        #search_dialog
        {
            display: block;
            float: none;
        }
        
        #search_dialog div
        {
            background: #ffffff;
            padding: 10px;
            height: 30px;
            width: 90%;
        }
        
        .tbl_rounded_custom
        {
            width: 87%;
        }
        .tbl_rounded_custom tr td
        {
            padding: 5px;
            height: 10px;
        }
        
        .tbl_rounded_custom tr
        {
            cursor: pointer;
        }
        
        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 td
        {
            background: #eee;
            border: 0;
        }
        .recordNavigator2, .recordNavigator2 td
        {
            border: 0;
        }
        
        
        /* --------------------------------- */
        /* jstree                            */
        /* --------------------------------- */
        .jstree-draggable
        {
            cursor: pointer;
        }
        
        /* .jstree-draggable:hover
        {
            background: #E7F4F9;
        }*/
        
        .tbl_rounded tr.jstree-draggable-selected td, .tbl_rounded tr.jstree-draggable-selected td, .tbl_rounded tr.jstree-draggable-selected:hover td, .tbl_rounded tr.jstree-draggable-selected:hover td
        {
            background: #FFFC52;
        }
        
        /*.tbl_rounded tr.selectedrow td
        {
            background-color:  #FFFC52;
        }*/
        
        #tree {overflow: auto;}
        #tree a {max-width: 400px; overflow: hidden; text-overflow: ellipsis;}
    </style>
    <script type="text/javascript">
//        $(function () {
//            $('#centerContentAddressbookSx input, #centerContentAddressbookSx textarea').keypress(function (e) {
//                if (e.which == 13) {
//                    e.preventDefault();
//                    $('.defaultAction').click();
//                }
//            });

//            $('#centerContentAddressbookSx select').change(function (e) {
//                if (e.which == 13) {
//                    e.preventDefault();
//                    $('.defaultAction').click();
//                }
//            });
//                });

        $(function () {
            $('.defaultAction').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('#BtnSearch').click();
                }
            });
        });


        function closeAjaxModal(id, retval) { // chiude il popup modale [id] e imposta il valore di ritorno [retval] nel campo hidden
            var p = parent.fra_main;
            if (arguments.length > 2 && arguments[2] != null) {
                p = arguments[2];
            }
            else {
                try {
                    var e = p.$('iframe').get(0);

                    if (e.id != 'ifrm_' + id) {
                        p = e.contentWindow;
                        e = p.$('iframe').get(0);

                        if (e.id != 'ifrm_' + id) {
                            p = e.contentWindow;
                            e = p.$('iframe').get(0);
                        }
                    }
                }
                catch (err) {
                    try {
                        p = parent.fra_main;
                    }
                    catch (err2) {
                        p = parent;
                    }
                }
            }

            if (arguments.length > 1) {
                this.$('.retval' + id + ' input').get(0).value = retval;
            }
            this.$('#' + id + '_panel').dialog('close');
        }


        function acePopulatedCod(sender, e) {
            var behavior = $find('AutoCompleteDescriptionProject');
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }

        function aceSelectedDescr(sender, e) {
            var value = e.get_value();
            if (!value) {
                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }

            var searchText = $get('<%=TxtDescriptionProject.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeProject.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionProject.ClientID%>").value = descrizione;
        }


        function acePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoBIS');
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }


        function aceSelected(sender, e) {
            var value = e.get_value();
            if (!value) {

                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {

                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }


            var searchText = $get('<%=txtDescrizioneProprietario.ClientID %>').value;
            searchText = searchText.replace('null', '');
            //            alert(searchText);
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneProprietario.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneProprietario.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceProprietario.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneProprietario.ClientID%>").value = descrizione;
        }


        function creatorePopulated(sender, e) {

            //alert('ciao');

            var tbPosition = $common.getLocation($get('<%=txtDescrizioneCreatore.ClientID %>'));
            var offset = $get('<%=pnlCreatorAuto.ClientID %>').offsetTop;
            $('<%=pnlCreatorAuto.ClientID %>').offsetHeight = tbPosition.y + offset;
            var behavior = $find('AutoCompleteExIngressoCreatore');
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }

        function creatoreSelected(sender, e) {
            var value = e.get_value();
            if (!value) {

                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {

                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }

            var searchText = $get('<%=txtDescrizioneCreatore.ClientID %>').value;
            searchText = searchText.replace('null', '');
            //            alert(searchText);
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneCreatore.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneCreatore.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceCreatore.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneCreatore.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceCreatore.ClientID%>', '');
        }


        function collocazionePopulated(sender, e) {
            //            alert('ciao');

            var tbPosition = $common.getLocation($get('<%=txtDescrizioneCollocazione.ClientID %>'));

            var offset = $get('<%=pnlColl.ClientID %>').offsetTop;
            //            alert(tbPosition.x);
            //            alert(offset);
            var behavior = $find('AutoCompleteExIngressoCollocazione');
            $('<%=pnlColl.ClientID %>').offsetHeight = tbPosition.y + offset;
            var ex = $get('<%=pnlColl.ClientID %>');
            //ex.offset(offset);
            //alert(this.Html.offsetHeight.toString());
            //$common.setLocation(ex, new Sys.UI.Point(tbPosition.x, tbPosition.y));
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                //                alert(prefix);
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        //                        alert(target);
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }

        function collocazioneSelected(sender, e) {
            //            alert('ciao');
            var value = e.get_value();
            if (!value) {

                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {

                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }

            var searchText = $get('<%=txtDescrizioneCollocazione.ClientID %>').value;

            searchText = searchText.replace('null', '');
            //            alert(searchText);
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneCollocazione.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneCollocazione.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceCollocazione.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneCollocazione.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceCollocazione.ClientID%>', '');
        }


        $(document).ready(function () {
            var active = $('#<%= advFilters.ClientID %>').val();
            var currentindex = 0;
            if (active != "") {
                if (active == "false") {
                }
                else {
                    $("#pnlAdvFilters").css("display", "block");
                }

                currentindex = active;
            }



            //            $("#pnlAdvFilters").accordion(
            //                  {
            //                      heightStyle: "content",
            //                      icons: false,
            //                      collapsible: true,


            //                      change: function (event, ui) {
            //                          var i = $("#pnlAdvFilters").accordion("option", "active");
            //                          $('#<%= advFilters.ClientID %>').val(i);

            //                      }
            //                  }
            //                );


            //            $("#pnlAdvFilters").accordion("activate", parseInt(currentindex));
        }
    );

        $(function () {
            $('#lnkAdvFilters').click(function () {
                var active = $('#<%= advFilters.ClientID %>').val();
                $('#pnlAdvFilters').toggle();
                $('#<%= advFilters.ClientID %>').val((active == "false") ? "true" : "false");
                var active2 = $('#<%= advFilters.ClientID %>').val();
                return false;
            });
        });


        function SingleSelect(regex, current) {
            re = new RegExp(regex);
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];
                if (elm.type == 'radio' && elm != current && re.test(elm.name)) {
                    elm.checked = false;
                }
            }
        }

        function resizeDiv() {
            var height = document.documentElement.clientHeight;
            height -= 170; /* whatever you set your body bottom margin/padding to be */
            document.getElementById('centerContentAddressbookSx').style.height = height + "px";
        }
    </script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { $('#btnAddressBookPostback').click(); }" />
    <%--<uc:ajaxpopup2 Id="AddressBook2" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false" 
PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { $('#btnAddressBookPostback2').click(); }" />--%>
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) { __doPostBack('UpPnlRapidCollation', ''); }" />
    <uc:ajaxpopup2 Id="SearchSubset" runat="server" Url="../popup/ProjectSearchSubset.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="400" Height="300"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="CreateNode" runat="server" Url="../popup/ProjectDataentryNode.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="400" Height="350"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="ModifyNode" runat="server" Url="../popup/ProjectDataentryNode.aspx?t=modify"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="400" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <div class="container">
        <!-- header -->
        <div id="divObjSegn" class="row" runat="server" visible="false">
            <div class="colTitle" id="Oggetto" runat="server">
                <strong>
                    <asp:Literal runat="server" ID="LitSearchProjectPopupObject"></asp:Literal></strong></div>
            <div class="col">
                <label id="LblOggetto" runat="server">
                </label>
            </div>
            <div class="colTitle" id="Segnatura" runat="server">
                <strong>
                    <asp:Literal runat="server" ID="LitSearchProjectPopupSignature"></asp:Literal></strong></div>
            <div class="col redWeight">
                <label id="LblSegnatura" runat="server">
                </label>
            </div>
        </div>
        <div id="divDataProt" class="row" runat="server" visible="false">
            <div class="colTitle">
                <strong>
                    <asp:Literal runat="server" ID="LitSerachProkjectPopupDateSignature"></asp:Literal></strong></div>
            <div class="col">
                <label id="LblDataSegnatura" runat="server">
                </label>
            </div>
        </div>
        <!-- tab -->
        <div id="contentAddressBook">
            <div id="topContentPopupSearch">
                <asp:UpdatePanel ID="UpTypeResult" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <ul>
                            <li class="addressTab" id="liAddressBookLinkList" runat="server">
                                <asp:LinkButton runat="server" ID="AddressBookLinkList"><asp:Literal ID="LitPopupSearchProjectSearch" runat="server"></asp:Literal></asp:LinkButton></li>
                            <li id="lbl_countRecord" class="blue" runat="server">
                                <asp:Literal ID="LitPopupSearchProjectFound" runat="server"></asp:Literal>
                                <asp:Literal ID="FolderFoundCount" runat="server" />
                                <asp:Literal ID="LitPopupSearchProjectFound2" runat="server"></asp:Literal></li>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="centerContentAddressbook">
                <div id="contentTab">
                    <div id="centerContentAddressbookSx" style="overflow:auto;">
                        <fieldset class="basic3">
                            <asp:PlaceHolder ID="plcRegistry" runat="server">
                                <asp:UpdatePanel ID="UpPnlRegistry" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="colHalf">
                                                <strong>
                                                    <asp:Literal ID="litRegistry" runat="server" /></strong></div>
                                            <div class="col">
                                                <asp:DropDownList runat="server" ID="DdlRegistries" CssClass="chzn-select-deselect defaultAction"
                                                    Width="90" AutoPostBack="true" OnSelectedIndexChanged="DdlRegistries_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </asp:PlaceHolder>
                            <asp:UpdatePanel ID="UpPnlRapidCollation" runat="server" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-right-no-margin">
                                            <cc1:CustomImageButton ID="btnclassificationschema" TabIndex="0" ImageUrl="../Images/Icons/classification_scheme.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png"
                                                OnClientClick="return ajaxModalPopupOpenTitolario();" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <!-- titolario -->
                            <asp:UpdatePanel ID="UpPnlTitolario" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:PlaceHolder ID="plcTitolario" runat="server">
                                        <div class="row">
                                            <div class="colHalf">
                                                <strong>
                                                    <asp:Literal ID="litTitolario" runat="server" /></strong></div>
                                            <div class="col">
                                                <asp:DropDownList ID="ddlTitolario" runat="server" CssClass="chzn-select-deselect"
                                                    Width="300" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder runat="server" ID="PnlProject">
                                            <asp:HiddenField ID="IdProject" runat="server" />
                                            <div class="colHalf">
                                                <strong>
                                                    <asp:Literal ID="ltrCode" runat="server" /></strong></div>
                                            <div class="colHalf">
                                                <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft"
                                                    OnTextChanged="TxtCodeProject_OnTextChanged" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                     AutoComplete="off" onchange="disallowOp('Content2');">
                                                </cc1:CustomTextArea>
                                            </div>
                                            <div class="colHalf4">
                                                <div class="colHalf3">
                                                    <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_projectRight defaultAction"
                                                        CssClassReadOnly="txt_ProjectRight_disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                            </div>
                                              <uc1:AutoCompleteExtender runat="server" ID="RapidSenderDescriptionProject" TargetControlID="TxtDescriptionProject"
                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListDescriptionProject"
                                                MinimumPrefixLength="6" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                                                OnClientPopulated="acePopulatedCod">
                                            </uc1:AutoCompleteExtender>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row">
                                <div class="colHalf">
                                    <strong>
                                        <asp:Literal ID="LtrDescr" runat="server" /></strong></div>
                                <div class="colHalf2">
                                    <cc1:CustomTextArea ID="txt_Description" runat="server" CssClass="txt_addressBookLeft defaultAction" CssClassReadOnly="txt_input_disabled" /></div>
                            </div>
                            <div class="row">
                                <div class="colHalf">
                                    <strong>
                                        <asp:Literal ID="litStatus" runat="server" /></strong></div>
                                <div class="col">
                                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="chzn-select-deselect" Width="190">
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem id="opt_statusA" Value="A" />
                                        <asp:ListItem id="opt_statusC" Value="C" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col">
                                    <asp:CheckBox ID="chkInADL" runat="server" TextAlign="Right" /></div>
                            </div>
                            <div class="row">
                                <div class="colHalf">
                                    <strong>
                                        <asp:Literal ID="litNum" runat="server" /></strong></div>
                                <div class="col-no-margin">
                                    <cc1:CustomTextArea ID="TxtNumProject" runat="server" CssClass="txt_input_half defaultAction" CssClassReadOnly="txt_input_half_disabled" />
                                </div>
                                <div class="col-no-margin">
                                    <strong>
                                        <asp:Literal ID="litYear" runat="server" /></strong></div>
                                <div class="col-no-margin">
                                    <cc1:CustomTextArea ID="TxtYear" runat="server" CssClass="txt_input_half defaultAction" CssClassReadOnly="txt_input_half_disabled" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="colHalf">
                                    <strong>
                                        <asp:Literal ID="litSubset" runat="server" /></strong></div>
                                <div class="colHalf2">
                                    <cc1:CustomTextArea ID="txt_subProject" runat="server" CssClass="txt_input defaultAction" CssClassReadOnly="txt_input_disabled" /></div>
                            </div>
                            <asp:PlaceHolder runat="server" ID="PlcTypeProject" Visible="false">
                                <div class="row">
                                    <div class="colHalf">
                                        <strong>
                                            <asp:Literal ID="ltrTipoFasc" runat="server" /></strong></div>
                                    <div class="col">
                                        <asp:DropDownList ID="ddl_typeProject" runat="server" CssClass="chzn-select-deselect"
                                            Width="400">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem id="opt_typeG" Value="G" />
                                            <asp:ListItem id="opt_typeP" Value="P" Selected="True" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="row">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="colHalf">
                                                &nbsp;</div>
                                            <div class="col">
                                                <asp:RadioButtonList ID="rblOwnerType" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList"
                                                    AutoPostBack="false">
                                                    <asp:ListItem id="optUO" Value="U" runat="server" Text="UO" Selected="True" />
                                                    <asp:ListItem id="optRole" Value="R" runat="server" Text="Ruolo" />
                                                    <asp:ListItem id="optUser" Value="P" runat="server" Text="Utente" />
                                                </asp:RadioButtonList>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <cc1:CustomImageButton runat="server" ID="DocumentImgSenderAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                    OnClick="DocumentImgSenderAddressBook_Click" />
                                            </div>
                                        </div>
                                        <asp:HiddenField ID="idProprietario" runat="server" />
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitProprietario" runat="server" /></strong></div>
                                        <div class="colHalf">
                                            <cc1:CustomTextArea ID="txtCodiceProprietario" runat="server" CssClass="txt_addressBookLeft"
                                                AutoPostBack="true" OnTextChanged="TxtCode_OnTextChanged" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                AutoComplete="off" onchange="disallowOp('Content2');">
                                            </cc1:CustomTextArea>
                                        </div>
                                        <div class="colHalf4">
                                            <div class="colHalf3">
                                                <cc1:CustomTextArea ID="txtDescrizioneProprietario" runat="server" CssClass="txt_projectRight defaultAction"
                                                    CssClassReadOnly="txt_ProjectRight_disabled">
                                                </cc1:CustomTextArea>
                                            </div>
                                        </div>
                                      <uc1:AutoCompleteExtender runat="server" ID="RapidSenderDescriptionProprietario"
                                            TargetControlID="txtDescrizioneProprietario" CompletionListCssClass="autocomplete_completionListElement"
                                            CompletionListItemCssClass="single_item" CompletionListHighlightedItemCssClass="single_item_hover"
                                            ServiceMethod="GetListaCorrispondentiVeloce" MinimumPrefixLength="7" CompletionInterval="1000"
                                            EnableCaching="true" CompletionSetCount="20" DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx"
                                            ShowOnlyCurrentWordInCompletionListItem="true" UseContextKey="true" OnClientItemSelected="aceSelected"
                                            BehaviorID="AutoCompleteExIngressoBIS" OnClientPopulated="acePopulated">
                                        </uc1:AutoCompleteExtender>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight"><span class="black">
                                                        <asp:Literal ID="lit_dtaOpen" runat="server" /></span></span></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:DropDownList ID="ddl_dtaOpen" runat="server" CssClass="chzn-select-deselect"
                                                            Width="130" data-placeholder="Seleziona" OnSelectedIndexChanged="ddl_dtaOpen_SelectedIndexChanged"
                                                            AutoPostBack="true" onchange="disallowOp('Content2');">
                                                            <asp:ListItem id="dtaOpen_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaOpen_opt1" Value="1" />
                                                            <asp:ListItem id="dtaOpen_opt2" Value="2" />
                                                            <asp:ListItem id="dtaOpen_opt3" Value="3" />
                                                            <asp:ListItem id="dtaOpen_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </span>
                                                </p>
                                            </div>
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:Literal ID="lblInit_DtaOpen" runat="server" />
                                                        <cc1:CustomTextArea ID="txtInitDtaOpen" runat="server" CssClass="txt_date datepicker defaultAction"
                                                            CssClassReadOnly="txt_date_disabled" />
                                                    </span>
                                                </p>
                                            </div>
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:Literal ID="lblEnd_DtaOpen" runat="server" Visible="false" />
                                                        <cc1:CustomTextArea ID="txtEndDtaOpen" runat="server" CssClass="txt_date datepicker defaultAction"
                                                            CssClassReadOnly="txt_date_disabled" Visible="false" />
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight"><span class="black">
                                                        <asp:Literal ID="lit_dtaClose" runat="server" /></span></span></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:DropDownList ID="ddl_dtaClosed" runat="server" CssClass="chzn-select-deselect"
                                                            Width="130" data-placeholder="Seleziona" OnSelectedIndexChanged="ddl_dtaClosed_SelectedIndexChanged"
                                                            AutoPostBack="true" onchange="disallowOp('Content2');">
                                                            <asp:ListItem id="dtaClosed_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaClosed_opt1" Value="1" />
                                                            <asp:ListItem id="dtaClosed_opt2" Value="2" />
                                                            <asp:ListItem id="dtaClosed_opt3" Value="3" />
                                                            <asp:ListItem id="dtaClosed_opt4" Value="4" />
                                                        </asp:DropDownList>
                                                    </span>
                                                </p>
                                            </div>
                                            <%--<div class="col-no-margin">--%>
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:Literal ID="lblInit_DtaClosed" runat="server" />
                                                        <cc1:CustomTextArea ID="txtInitDtaClosed" runat="server" CssClass="txt_date datepicker defaultAction"
                                                            CssClassReadOnly="txt_date_disabled" />
                                                    </span>
                                                </p>
                                            </div>
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:Literal ID="lblEnd_DtaClosed" runat="server" Visible="false" />
                                                        <cc1:CustomTextArea ID="txtEndDtaClosed" runat="server" CssClass="txt_date datepicker defaultAction"
                                                            CssClassReadOnly="txt_date_disabled" Visible="false" />
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row">
                                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight"><span class="black">
                                                        <asp:Literal ID="lit_dtaCreate" runat="server" /></span></span></p>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <p>
                                                <span class="black">
                                                    <asp:DropDownList ID="ddl_dtaCreated" runat="server" CssClass="chzn-select-deselect"
                                                        Width="130" data-placeholder="Seleziona" OnSelectedIndexChanged="ddl_dtaCreated_SelectedIndexChanged"
                                                        AutoPostBack="true" onchange="disallowOp('Content2');">
                                                        <asp:ListItem id="dtaCreate_opt0" Selected="True" Value="0" />
                                                        <asp:ListItem id="dtaCreate_opt1" Value="1" />
                                                        <asp:ListItem id="dtaCreate_opt2" Value="2" />
                                                        <asp:ListItem id="dtaCreate_opt3" Value="3" />
                                                        <asp:ListItem id="dtaCreate_opt4" Value="4" />
                                                    </asp:DropDownList>
                                                </span>
                                            </p>
                                        </div>
                                        <div class="col">
                                            <p>
                                                <span class="black">
                                                    <asp:Literal ID="lblInit_DtaCreated" runat="server" />
                                                    <cc1:CustomTextArea ID="txtInitDtaCreated" runat="server" CssClass="txt_date datepicker defaultAction"
                                                        CssClassReadOnly="txt_date_disabled" />
                                                </span>
                                            </p>
                                        </div>
                                        <div class="col">
                                            <p>
                                                <span class="black">
                                                    <asp:Literal ID="litEnd_DataCreated" runat="server" Visible="false" />
                                                    <cc1:CustomTextArea ID="txtEndDtaCreated" runat="server" CssClass="txt_date datepicker defaultAction"
                                                        CssClassReadOnly="txt_date_disabled" Visible="false" />
                                                </span>
                                            </p>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row">
                                <div class="colHalf">
                                    &nbsp;</div>
                                <div class="col">
                                    <asp:RadioButtonList ID="rblFilterNote" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList">
                                        <asp:ListItem id="optNoteAny" runat="server" Text="Qualsiasi" Value="Q" Selected="True" />
                                        <asp:ListItem id="optNoteAll" runat="server" Text="Tutti" Value="T" />
                                        <asp:ListItem id="optNoteRole" runat="server" Text="Ruolo" Value="R" />
                                        <asp:ListItem id="optNoteRF" runat="server" Text="RF" Value="F" />
                                        <asp:ListItem id="optNotePersonal" runat="server" Text="Personali" Value="P" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="colHalf">
                                    <strong>
                                        <asp:Literal ID="LitNote" runat="server" /></strong></div>
                                <div class="colHalf2">
                                    <cc1:CustomTextArea ID="TxtNoteProject" runat="server" CssClass="txt_input defaultAction" CssClassReadOnly="txt_input_disabled"></cc1:CustomTextArea>
                                </div>
                            </div>
                        </fieldset>
                        <div class="row">
                            <fieldset class="azure" style="width: 96%;">
                                <h2 class="expand">
                                    <asp:Literal ID="SearchDocumentLitTypology" runat="server" /></h2>
                                <div class="collapse shown" style="border-width: 0">
                                    <asp:UpdatePanel ID="UpPnlDocType" runat="server">
                                        <ContentTemplate>
                                            <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <asp:DropDownList ID="DocumentDdlTypeDocument" runat="server" AutoPostBack="True"
                                                                CssClass="chzn-select-deselect" Width="450px" OnSelectedIndexChanged="DocumentDdlTypeDocument_SelectedIndexChanged"
                                                                onchange="disallowOp('Content2');">
                                                                <asp:ListItem Text=""></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <%--</div>--%>
                                                    </div>
                                                    <asp:PlaceHolder ID="PnlStateDiagram" runat="server" Visible="false">
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <asp:DropDownList ID="ddlStateCondition" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                                    Width="500px">
                                                                    <asp:ListItem id="opt_StateConditionEquals" Value="Equals" />
                                                                    <asp:ListItem id="opt_StateConditionUnequals" Value="Unequals" />
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-full">
                                                                <div class="col">
                                                                    <asp:DropDownList ID="DocumentDdlStateDiagram" runat="server" CssClass="chzn-select-deselect"
                                                                        Width="500px" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <asp:PlaceHolder runat="server" ID="PnlDocumentStateDiagramDate" Visible="false">
                                                            <div class="row">
                                                                <div class="col">
                                                                    <p>
                                                                        <span class="weight">
                                                                            <asp:Literal ID="DocumentDateStateDiagram" runat="server"></asp:Literal>
                                                                        </span>
                                                                    </p>
                                                                </div>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>
                                                    <span class="black">
                                                        <asp:PlaceHolder ID="PnlTypeDocument" runat="server"></asp:PlaceHolder>
                                                    </span>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </fieldset>
                        </div>
                        <div class="row">
                            <div class="col">
                                <p>
                                    <a href="#" id="lnkAdvFilters">
                                        <asp:Literal runat="server" ID="LitPopupSearchProjectFilters"></asp:Literal></a></p>
                            </div>
                        </div>
                        <div class="row">
                            <asp:Panel ID="pnlAdvFilters" CssClass="basic3 hidden" ClientIDMode="Static" runat="server">
                                <div class="row">
                                    <fieldset class="basic4">
                                        <asp:HiddenField ID="advFilters" Value="false" runat="server" />
                                        <h2 class="expand">
                                            <asp:Literal ID="litCollocation" runat="server" /></h2>
                                        <div id="pnlAdvFiltersCollapse" runat="server" class="collapse">
                                            <asp:UpdatePanel runat="server" ID="upPnlCollocationAddr" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <asp:HiddenField ID="idCollocationAddr" runat="server" />
                                                        <div class="colHalf">
                                                            <strong>
                                                                <asp:Literal ID="litCollocationAddr" runat="server" /></strong></div>
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="txtCodiceCollocazione" runat="server" CssClass="txt_addressBookLeft"
                                                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCodeColl_OnTextChanged"
                                                                AutoCompleteType="Disabled" onchange="disallowOp('Content2');">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div id="pnlColl" runat="server" style="position: relative">
                                                            <div class="colHalf7">
                                                                <div class="colHalf3">
                                                                    <cc1:CustomTextArea ID="txtDescrizioneCollocazione" runat="server" CssClass="txt_projectRight defaultAction"
                                                                        CssClassReadOnly="txt_ProjectRight_disabled" Style="width: 90%; float: left;"></cc1:CustomTextArea>
                                                                    <cc1:CustomImageButton runat="server" ID="DocumentImgCollocationAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                        OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                        OnClick="DocumentImgCollocationAddressBook_Click" />
                                                                </div>
                                                            </div>
                                                           <uc1:AutoCompleteExtender runat="server" ID="RapidCollocazione" TargetControlID="txtDescrizioneCollocazione"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="collocazioneSelected" BehaviorID="AutoCompleteExIngressoCollocazione"
                                                                OnClientPopulated="collocazionePopulated ">
                                                            </uc1:AutoCompleteExtender>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight"><span class="black">
                                                            <asp:Literal ID="lit_dtaCollocation" runat="server" /></span></span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <asp:UpdatePanel runat="server" ID="UpPnlDateCollocation" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <div class="col">
                                                            <p>
                                                                <span class="black">
                                                                    <asp:DropDownList ID="ddl_dtaCollocation" runat="server" Width="130" AutoPostBack="True"
                                                                        CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_dtaCollocation_SelectedIndexChanged"
                                                                        onchange="disallowOp('Content2');">
                                                                        <asp:ListItem id="dtaCollocation_opt0" Selected="True" Value="0" />
                                                                        <asp:ListItem id="dtaCollocation_opt1" Value="1" />
                                                                        <asp:ListItem id="dtaCollocation_opt2" Value="2" />
                                                                        <asp:ListItem id="dtaCollocation_opt3" Value="3" />
                                                                        <asp:ListItem id="dtaCollocation_opt4" Value="4" />
                                                                    </asp:DropDownList>
                                                                </span>
                                                            </p>
                                                        </div>
                                                        <div class="col">
                                                            <p>
                                                                <span class="black">
                                                                    <asp:Label ID="lbl_dtaCollocationFrom" runat="server"></asp:Label>
                                                                    <cc1:CustomTextArea ID="dtaCollocation_TxtFrom" runat="server" CssClass="txt_date datepicker defaultAction"
                                                                        CssClassReadOnly="txt_date_disabled" ClientIDMode="Static"></cc1:CustomTextArea>
                                                                </span>
                                                            </p>
                                                        </div>
                                                        <div class="col">
                                                            <p>
                                                                <span class="black">
                                                                    <asp:Label ID="lbl_dtaCollocationTo" runat="server" Visible="False"></asp:Label>
                                                                    <cc1:CustomTextArea ID="dtaCollocation_TxtTo" runat="server" CssClass="txt_date datepicker defaultAction"
                                                                        CssClassReadOnly="txt_date_disabled" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset class="basic4">
                                        <asp:UpdatePanel ID="upPnlCreatore" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="colHalf">
                                                        &nbsp;</div>
                                                    <div class="col">
                                                        <asp:RadioButtonList ID="rblOwnerTypeCreator" runat="server" CssClass="rblHorizontal"
                                                            RepeatLayout="UnorderedList" AutoPostBack="false">
                                                            <asp:ListItem id="optUOC" runat="server" Value="U" />
                                                            <asp:ListItem id="optRoleC" runat="server" Value="R" Selected="True" />
                                                            <asp:ListItem id="optUserC" runat="server" Value="P" />
                                                        </asp:RadioButtonList>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <cc1:CustomImageButton runat="server" ID="ImgCreatoreAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                            OnClick="ImgCreatoreAddressBook_Click" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <asp:HiddenField ID="idCreatore" runat="server" />
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="ltlCreator" runat="server" /></strong></div>
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="txtCodiceCreatore" runat="server" CssClass="txt_addressBookLeft"
                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCodeColl_OnTextChanged"
                                                            AutoCompleteType="Disabled" onchange="disallowOp('Content2');">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                    <div id="pnlCreatorAuto" runat="server" style="position: relative">
                                                        <div class="colHalf7">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="txtDescrizioneCreatore" runat="server" CssClass="txt_projectRight defaultAction"
                                                                    CssClassReadOnly="txt_ProjectRight_disabled" Width="300px" Style="float: left;">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                        <uc1:AutoCompleteExtender runat="server" ID="RapidCreatore" TargetControlID="txtDescrizioneCreatore"
                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="creatoreSelected" BehaviorID="AutoCompleteExIngressoCreatore"
                                                            OnClientPopulated="creatorePopulated">
                                                        </uc1:AutoCompleteExtender>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset class="basic4">
                                        <asp:UpdatePanel ID="UpPnlConservation" runat="server" UpdateMode="Conditional" class="row">
                                            <ContentTemplate>
                                                <asp:CheckBox ID="chkConservation" runat="server" AutoPostBack="true" OnCheckedChanged="chkConservation_CheckedChanged" />
                                                <asp:CheckBox ID="chkConservationNo" runat="server" AutoPostBack="true" OnCheckedChanged="chkConservation_CheckedChanged" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset class="basic4">
                                        <asp:UpdatePanel ID="UpPnlViewAll" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="col">
                                                    <strong>
                                                        <asp:Literal ID="litViewAll" runat="server" /></strong></div>
                                                <div class="col">
                                                    <asp:RadioButton ID="rbViewAllYes" runat="server" AutoPostBack="true" Checked="true"
                                                        OnCheckedChanged="rbViewAllYes_CheckedChanged" />
                                                    <asp:RadioButton ID="rbViewAllNo" runat="server" AutoPostBack="true" OnCheckedChanged="rbViewAllYes_CheckedChanged" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </div>
                                <fieldset class="basic4">
                                    <asp:UpdatePanel ID="upPnlIntervals" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="black"><strong>
                                                            <asp:Literal ID="ltlDtaExpire" runat="server" /></strong></span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="black">
                                                            <asp:DropDownList ID="ddl_dtaExpire" runat="server" Width="130" AutoPostBack="True"
                                                                CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_dtaExpire_SelectedIndexChanged"
                                                                onchange="disallowOp('Content2');">
                                                                <asp:ListItem id="dtaExpire_opt0" Selected="True" Value="0" />
                                                                <asp:ListItem id="dtaExpire_opt1" Value="1" />
                                                                <asp:ListItem id="dtaExpire_opt2" Value="2" />
                                                                <asp:ListItem id="dtaExpire_opt3" Value="3" />
                                                                <asp:ListItem id="dtaExpire_opt4" Value="4" />
                                                            </asp:DropDownList>
                                                        </span>
                                                    </p>
                                                </div>
                                                <div class="col">
                                                    <p>
                                                        <span class="black">
                                                            <asp:Literal ID="lbl_dtaExpireFrom" runat="server"></asp:Literal>
                                                            <cc1:CustomTextArea ID="dtaExpire_TxtFrom" runat="server" CssClass="txt_date datepicker defaultAction"
                                                                CssClassReadOnly="txt_date_disabled" ClientIDMode="Static"></cc1:CustomTextArea></span></p>
                                                </div>
                                                <div class="col">
                                                    <p>
                                                        <span class="black">
                                                            <asp:Label ID="lbl_dtaExpireTo" runat="server" Visible="False"></asp:Label>
                                                            <cc1:CustomTextArea ID="dtaExpire_TxtTo" runat="server" CssClass="txt_date datepicker defaultAction"
                                                                CssClassReadOnly="txt_date_disabled" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea></span></p>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                                <asp:PlaceHolder ID="plcExcelFilter" runat="server" Visible="false">
                                    <asp:UpdatePanel ID="UpPnlExcelFilter" runat="server">
                                        <ContentTemplate>
                                            <div class="row">
                                                <fieldset class="azure" style="width: 96%;">
                                                    <h2 class="expand">
                                                        <asp:Literal ID="litExcelFilter" runat="server" /></h2>
                                                    <div id="pnlMainExcelFilter" runat="server" class="collapse" style="border-width: 0">
                                                        <div class="row">
                                                            <div class="colonnasx">
                                                                <div class="col">
                                                                    <asp:Literal ID="litExcelFilterFile" runat="server" /></div>
                                                            </div>
                                                            <div class="colonnadx">
                                                                <div class="col-right">
                                                                    <a href="../ImportDati/TemplateFiltroFascicoli.xls" target="_blank">
                                                                        <img src="../Images/Icons/home_export.png" alt="" id="SearchProjectBtnDownload" runat="server" title="Scarica" border="0"/></a></div>
                                                                <div class="col">
                                                                    <asp:FileUpload ID="uploadedFile" runat="server" size="30" name="uploadedFile"></asp:FileUpload></div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="colonnasx">
                                                                &nbsp;</div>
                                                            <div class="colonnadx">
                                                                <div class="col nowrap">
                                                                    <asp:Label ID="lbl_fileExcel" runat="server" /></div>
                                                                <div class="col-right">
                                                                    <cc1:CustomButton ID="UploadBtn" Text="Carica" CssClass="buttonAbort" CssClassDisabled="buttonAbortDisable"
                                                                        OnMouseOver="buttonAbortHover" runat="server" OnClick="UploadBtn_Click" /></div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="colonnasx">
                                                                &nbsp;</div>
                                                            <div class="colonnadx">
                                                                <div class="col-right">
                                                                    <cc1:CustomButton ID="btn_elimina_excel" Text="Elimina" CssClass="buttonAbort" CssClassDisabled="buttonAbortDisable"
                                                                        OnMouseOver="buttonAbortHover" runat="server" Visible="false" OnClick="btn_elimina_excel_Click" /></div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="colonnasx">
                                                                <asp:Label ID="lbl_attributo" runat="server" /></div>
                                                            <div class="colonnadx">
                                                                <asp:DropDownList ID="ddl_attributo" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                                    Width="343px" OnSelectedIndexChanged="ddl_attributo_SelectedIndexChanged">
                                                                    <asp:ListItem Value="" Selected="True"></asp:ListItem>
                                                                    <asp:ListItem Value="NUMERO_FASCICOLO"></asp:ListItem>
                                                                    <asp:ListItem Value="DATA_APERTURA"></asp:ListItem>
                                                                    <asp:ListItem Value="DESCRIZIONE_FASCICOLO"></asp:ListItem>
                                                                    <asp:ListItem Value="CODICE_NODO"></asp:ListItem>
                                                                    <asp:ListItem Value="TIPOLOGIA_FASCICOLO"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <asp:Panel ID="pnl_tipoFascExcel" runat="server" Visible="false" CssClass="row">
                                                            <div class="colonnasx">
                                                                <asp:Label ID="lbl_typeFasc" runat="server" />
                                                            </div>
                                                            <div class="colonnadx">
                                                                <asp:DropDownList ID="ddl_tipoFascExcel" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                                    Width="343px" OnSelectedIndexChanged="ddl_tipoFascExcel_SelectedIndexChanged" />
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Panel runat="server" ID="pnl_attrTipoFascExcel" Visible="false" CssClass="row">
                                                            <div class="colonnasx">
                                                                <asp:Label ID="lbl_attrTypeFasc" runat="server" />
                                                            </div>
                                                            <div class="colonnadx">
                                                                <asp:DropDownList ID="ddl_attrTipoFascExcel" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                                    Width="343px" />
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="UploadBtn" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </asp:PlaceHolder>
                            </asp:Panel>
                        </div>
                    </div>
                    <input id="txtSystemIdCreatore" type="hidden" runat="server" />
                    <asp:HiddenField ID="IdCodCreatoreRecipient" runat="server" />
                    <input id="txtTipoCorrispondente" type="hidden" runat="server" />
                    <%--<script language="javascript">
                    esecuzioneScriptUtente();
                </script>--%>
                    <div id="centerContentAddressbookDx" style="overflow:auto;">
                        <asp:UpdatePanel ID="UpPnlGridResult" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="GrdFascResult" runat="server" Width="100%" AutoGenerateColumns="False"
                                    AllowPaging="True" AllowCustomPaging="false" CssClass="tbl_rounded_custom round_onlyextreme"
                                    PageSize="9" BorderWidth="0" OnPageIndexChanging="changPageGrdFound_click" OnRowCommand="GrdFascResult_RowCommand"
                                    OnRowDataBound="GrdFascResult_RowDataBound" OnSelectedIndexChanged="GrdFascResult_SelectedIndexChanged"
                                    OnPreRender="GrdFascResult_PreRender">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="idSpace" Text="&nbsp;"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="false" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rbSel" runat="server" OnCheckedChanged="rbSel_CheckedChanged"
                                                    CssClass="clickable" AutoPostBack="true" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="false" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSel" runat="server" OnCheckedChanged="cbSel_CheckedChanged" AutoPostBack="true"
                                                    CssClass="clickable" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCodiceFasc" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'></asp:Literal>
                                            </ItemTemplate>
                                            <ControlStyle Width="60px" />
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="370px">
                                            <ItemTemplate>
                                                <%# NttDataWA.Utils.utils.TruncateString(DataBinder.Eval(Container, "DataItem.Descrizione")) %>
                                            </ItemTemplate>
                                            <ControlStyle Width="100px" />
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Literal ID="litFascDataOpen" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FascDataOpen") %>'></asp:Literal>
                                            </ItemTemplate>
                                            <ControlStyle Width="90px" />
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" Visible="false">
                                            <ItemTemplate>
                                                <asp:Literal ID="litFascDataClose" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FascDataClose") %>'></asp:Literal>
                                            </ItemTemplate>
                                            <ControlStyle Width="60px" />
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblFascKey" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>' />
                                            </ItemTemplate>
                                            <ControlStyle Width="60px" />
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litFascDataClose" runat="server" Text='<%# Bind("FascDataClose") %>'></asp:Literal>
                                                    </ItemTemplate>
                                                    <ControlStyle Width="60px" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>--%>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:HiddenField ID="rowIndex" runat="server" ClientIDMode="Static" />
                        <asp:HiddenField ID="HiddenPublicFolder" runat="server" ClientIDMode="Static"/>
                        <asp:HiddenField ID="HiddenContinuaFascicolazioneInPubblico" runat="server" ClientIDMode="Static"/>
                        <asp:UpdatePanel runat="server" ID="UpPnlButtonDetails">
                            <ContentTemplate>
                                <asp:Button ID="btnDetails" runat="server" ClientIDMode="Static" CssClass="hidden"
                                    OnClick="GridDocuments_Details" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <!-- structure -->
                        <p id="P2">
                        </p>
                        <div class="row">
                            <p id="P1">
                            </p>
                            <asp:UpdatePanel runat="server" ID="upnlStruttura" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:PlaceHolder runat="server" ID="PlcStructur" Visible="false">
                                        <div class="row">
                                            <div class="col-right-no-margin nowrap">
                                                <div class="linkTree">
                                                    <a href="#expand" id="expand_all">
                                                        <asp:Literal ID="litTreeExpandAll" runat="server" /></a> <a href="#collapse" id="collapse_all">
                                                            <asp:Literal ID="litTreeCollapseAll" runat="server" /></a>
                                                </div>
                                                <cc1:CustomImageButton ID="ImgFolderSearch" runat="server" ClientIDMode="Static"
                                                    ImageUrl="../Images/Icons/search_projects.png" CssClass="clickable" OnMouseOverImage="../Images/Icons/search_projects_hover.png"
                                                    OnMouseOutImage="../Images/Icons/search_projects.png" OnClientClick="return ajaxModalPopupSearchSubset();" />
                                                <cc1:CustomImageButton ID="ImgFolderAdd" runat="server" ClientIDMode="Static" ImageUrl="../Images/Icons/add_sub_folder.png"
                                                    CssClass="clickable" OnMouseOverImage="../Images/Icons/add_sub_folder_hover.png"
                                                    OnMouseOutImage="../Images/Icons/add_sub_folder.png" ImageUrlDisabled="../Images/Icons/add_sub_folder_disabeld.png" />
                                                <cc1:CustomImageButton ID="ImgFolderModify" runat="server" ClientIDMode="Static"
                                                    ImageUrl="../Images/Icons/edit_project.png" CssClass="clickable" OnMouseOverImage="../Images/Icons/edit_project_hover.png"
                                                    OnMouseOutImage="../Images/Icons/edit_project.png" ImageUrlDisabled="../Images/Icons/edit_project_disabeld.png" />
                                                <cc1:CustomImageButton ID="ImgFolderRemove" runat="server" ClientIDMode="Static"
                                                    ImageUrl="../Images/Icons/remove_sub_folder.png" CssClass="clickable" OnMouseOverImage="../Images/Icons/remove_sub_folder_hover.png"
                                                    OnMouseOutImage="../Images/Icons/remove_sub_folder.png" ImageUrlDisabled="../Images/Icons/remove_sub_folder_disabeld.png" />
                                                <%--<cc1:CustomImageButton ID="ImgFolderSearch" runat="server" ClientIDMode="Static" 
                                            ImageUrl="../Images/Icons/search_projects.png" CssClass="clickable"
                                            OnMouseOverImage="../Images/Icons/search_projects_hover.png"
                                            OnMouseOutImage="../Images/Icons/search_projects.png" 
                                            />--%>
                                            </div>
                                        </div>
                                        <fieldset style="background-color: White; width: auto; height: auto">
                                            <div class="row">
                                                <div class="col">
                                                    <asp:UpdatePanel ID="upPnlStruttura" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <span class="weight">
                                                                <asp:Literal ID="litStruttura" runat="server" /></span>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                            <div class="row" style="background-color: White; width: 100%; height: 100%">
                                                <div id="tree">
                                                </div>
                                                <p id="log2">
                                                </p>
                                                <asp:HiddenField ID="treenode_sel" runat="server" ClientIDMode="Static" />
                                                <asp:Button ID="BtnChangeSelectedFolder" runat="server" CssClass="hidden" OnClick="BtnChangeSelectedFolder_Click"
                                                    ClientIDMode="Static" />
                                                <%--<asp:button ID="BtnRebindGrid" runat="server" CssClass="hidden" OnClick="BtnRebindGrid_Click" ClientIDMode="Static" />--%>
                                            </div>
                                            <%--<div id="search_dialog" title="Cerca fascicolo">
                                        <div>
                                            <input type="text" id="search_intree" />
                                            <input type="button" id="search_submit" value="Cerca" />
                                        </div>
                                    </div>--%>
                                        </fieldset>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnSearch" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="BtnSearch_Click"
                OnClientClick="disallowOp('Content2');" ClientIDMode="Static" />
            <cc1:CustomButton ID="BtnCollate" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnCollate_Click" Enabled="false" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnClose_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:Button ID="btnAddressBookPostback2" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback2_Click" />
            <asp:Button ID="BtnHiddenSelectAll" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="BtnHiddenSelectAll_Click" />
            <asp:HiddenField runat="server" ID="HiddenRemoveNode" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });
        });
    </script>
    <script type="text/javascript">
        function JsTree() {
            $(function () {
                //$("#tree").empty();

                $("#tree")
                .jstree({
                    "core": {
                        "strings": {
                            loading: "Caricamento in corso...",
                            new_node: "Nuovo fascicolo"
                        }
                    },
                    "html_data": {
                        "ajax": {
                            "url": "../Project/SearchProject_getFolders.aspx?d" + new Date().getTime(),
                            "data": function (n) {
                                return { id: n.attr ? n.attr("id") : 0 };
                            }
                        }
                    },
                    "themes": {
                        "theme": "classic",
                        "dots": true,
                        "icons": true
                    },
                    "types": {
                        "valid_children": "all",
                        "types": {
                            "default": {
                                "valid_children": ["default", "nosons"]
                            }
                        }
                    },
                    "ui": {
                        "select_multiple_modifier": false
                    },
                    "crrm": {
                        "move": {
                            "check_move": function (m) {
                                if (m.o.hasClass('jstree-unamovable')) {
                                    return false;
                                }
                                else if (m.p == "before" || m.p == "after") {
                                    return false;
                                }
                                else {
                                    return true;
                                }
                            }
                        }
                    },
                    "dnd": {
                        "drag_check": function (data) {
                            if (data.r.attr("rel") == "nosons") {
                                return false;
                            }
                            return {
                                after: false,
                                before: false,
                                inside: true
                            };
                        },
                        "drag_finish": function (data) {
                            var iDs = "";
                            for (var i = 0; i < $('.jstree-draggable-selected').length; i++) {
                                if ($('.jstree-draggable-selected')[i].id != '') {
                                    if (iDs.length > 0) iDs += ",";
                                    iDs += $('.jstree-draggable-selected')[i].id;
                                }
                            }

                            var titles = "";
                            $(".jstree-draggable-selected").each(function () {
                                if ($(this).attr("title") != undefined) titles += "\n" + $(this).attr("title");
                            });

                            if ($('.jstree-draggable-selected').length > 0 && confirm('<asp:Literal id="litConfirmMoveDocuments" runat="server" />'.replace('##', titles.replace('\'', '\\\'')))) {
                                disallowOp('content');
                                $.ajax({
                                    'async': true,
                                    'timeout': 2160000,
                                    'type': 'POST',
                                    'url': "../Project/SearchProject_getFolders.aspx",
                                    'data': {
                                        "operation": "drag_documents",
                                        "ids": iDs,
                                        "id": data.o.id,
                                        "ref": data.r[0].id
                                    },
                                    'success': function (r) {
                                        $('#log2').html(r);
                                    },
                                    'error': function (jqXHR, textStatus, errorThrown) {
                                        $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
                                    },
                                    'complete': function (r) {
                                        $('.jstree-draggable').removeClass('jstree-draggable-selected');
                                        reallowOp();
                                    }
                                });
                            }
                        }
                    },
                    "plugins": ["themes", "html_data", "dnd", "crrm", "types", "ui"]
                })
                    .bind("before.jstree", function (e, data) {
                        if (data.func == "move_node"
                        && data.args[1] == false
                        && data.plugin == "core"
                        && data.args[0].o != undefined
                    ) {
                            var selfDrop = false;
                            if (data.args[0].r != undefined && data.args[0].o[0].id == data.args[0].r[0].id) selfDrop = true;

                            var permitDrop = true;
                            if (data.args[0].ot._get_type(data.args[0].o) == 'root' && data.args[0].ot._get_type(data.args[0].r) != 'root') permitDrop = false;
                            if (data.args[0].cr === -1) permitDrop = false;
                            if (data.args[0].np[0].id == data.args[0].op[0].id) permitDrop = false;

                            if (!$(data.args[0].o[0]).hasClass('jstree-unamovable') && !selfDrop && permitDrop) {
                                if (!confirm('<asp:Literal id="litConfirmMoveFolder" runat="server" />'.replace('##', $(data.args[0].o[0]).attr('data-title').replace('\'', '\\\'')))) {
                                    e.stopImmediatePropagation();
                                    return false;
                                }
                            }
                            else {
                                if (!$(data.args[0].o[0]).hasClass('jstree-unamovable')) alert('<asp:Literal id="litTreeAlertOperationNotAllowed" runat="server" />');
                                e.stopImmediatePropagation();
                                return false;
                            }
                        }
                    })
	            .bind("move_node.jstree", function (e, data) {
	                data.rslt.o.each(function (i) {
	                    /*
	                    data.rslt
	                    .o - the node being moved
	                    .r - the reference node in the move
	                    .ot - the origin tree instance
	                    .rt - the reference tree instance
	                    .p - the position to move to (may be a string - "last", "first", etc)
	                    .cp - the calculated position to move to (always a number)
	                    .np - the new parent
	                    .oc - the original node (if there was a copy)
	                    .cy - boolen indicating if the move was a copy
	                    .cr - same as np, but if a root node is created this is -1
	                    .op - the former parent
	                    .or - the node that was previously in the position of the moved node 
	                    */

	                    var iDs = "";
	                    for (var i = 0; i < data.rslt.o.length; i++) {
	                        if (iDs.length > 0) iDs += ",";
	                        iDs += data.rslt.o[i].id;
	                    }

	                    if (data.rslt.cr === -1 || data.rslt.np.attr("id") == data.rslt.op.attr("id")) {
	                        e.stopImmediatePropagation();
	                        JsTree();
	                        return false;
	                    }
	                    else {
	                        disallowOp('content');
	                        $.ajax({
	                            'async': true,
	                            'type': 'POST',
	                            'timeout': 2160000,
	                            'url': "../Project/SearchProject_getFolders.aspx",
	                            'data': {
	                                "operation": "move_node",
	                                "ids": iDs,
	                                "id": $(this).attr("id"),
	                                "ref": data.rslt.r.attr("id"),
	                                "parent": data.rslt.cr === -1 ? 1 : data.rslt.np.attr("id"),
	                                "position": data.rslt.p
	                            },
	                            'success': function (r) {
	                                $('#log2').html(r);
	                            },
	                            'error': function (jqXHR, textStatus, errorThrown) {
	                                $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
	                            },
	                            'complete': function () {
	                                reallowOp();
	                            }
	                        });
	                    }
	                });
	            })
                .bind("select_node.jstree", function (event, data) {
                    if ($.jstree._focused().get_selected().attr('id') != $('#treenode_sel').val()) {
                        $('#treenode_sel').val($.jstree._focused().get_selected().attr('id'));
                        $('#tree .jstree-search').removeClass("jstree-search");
                        $('#BtnChangeSelectedFolder').click();
                    }
                })
                .bind("loaded.jstree", function (e, data) {
                    $('#tree').jstree('open_all');
                    if ($('#treenode_sel').val().length > 0) $("#tree").jstree("select_node", '#' + $('#treenode_sel').val())

                    // assign tooltip and css class to a
                    /*
                    $('.jstree-leaf a').addClass('clickableRight');
                    $('.jstree-last a').addClass('clickableRight');
                    $('.jstree-leaf a').each(function (i) {
                    var $e = $(this);
                    $e.attr('title', $e.text());
                    });
                    $('.jstree-last a').each(function (i) {
                    var $e = $(this);
                    $e.attr('title', $e.text());
                    });
                    */
                    tooltipTipsy();
                    $('.jstree-leaf').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-last').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-open').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-closed').each(function (i) {
                        $(this).attr('title', '');
                    });

                    var qry = $('.retvalSearchSubset input').get(0).value;
                    if (qry.length > 0) searchTree(qry);
                });

                // resolve bug, see https://github.com/vakata/jstree/issues/174
                $("#jstree-marker-line").remove();

                $('.jstree-draggable')
		        .click(function (e) {
		            if (e.ctrlKey) {
		                var row = $('.tbl_rounded tr').index($(this));
		                var index = row + 1;

		                $(this).toggleClass('jstree-draggable-selected');
		                $($(".tbl_rounded tr").get(index)).toggleClass("jstree-draggable-selected");
		            }
		            else if (e.shiftKey) {
		                var iStart = $('.jstree-draggable-selected:first').index();
		                var iCurrent = $('.tbl_rounded tr').index($(this));
		                var index = iCurrent + 1;

		                if (iStart == -1) {
		                    $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                    $(this).addClass('jstree-draggable-selected');
		                    $($(".tbl_rounded tr").get(index)).addClass("jstree-draggable-selected");
		                }
		                else if (iStart < iCurrent) {
		                    $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iStart - 1) + ')').addClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iCurrent + 1) + ')').removeClass('jstree-draggable-selected');
		                }
		                else if (iCurrent < iStart) {
		                    $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iCurrent - 1) + ')').addClass('jstree-draggable-selected');
		                    $('.tbl_rounded tr:gt(' + (iStart + 1) + ')').removeClass('jstree-draggable-selected');
		                }
		                else if (iStart == iCurrent) {
		                    $(this).toggleClass('jstree-draggable-selected');
		                    $($(".tbl_rounded tr").get(index)).toggleClass("jstree-draggable-selected");
		                }
		            }
		            else {
		                var row = $('.tbl_rounded tr').index($(this));
		                var index = row + 1;

		                $('.tbl_rounded tr').removeClass('jstree-draggable-selected');
		                $(this).addClass('jstree-draggable-selected');
		                $($(".tbl_rounded tr").get(index)).addClass("jstree-draggable-selected");
		            }
		        });


                // collapse/expand all noded
                $("#collapse_all").click(function (e) {
                    e.stopImmediatePropagation();
                    $('#tree').jstree('close_all');
                    return false;
                });
                $("#expand_all").click(function (e) {
                    e.stopImmediatePropagation();
                    $('#tree').jstree('open_all');
                    return false;
                });

                // dataentry operation 
                $("#ImgFolderAdd").click(function (e) {
                    e.stopImmediatePropagation();
                    if ($('#tree .jstree-clicked').length > 0) {
                        //$('#tree').jstree('create');

                        $.post(
			                "../Project/SearchProject_getFolders.aspx",
			                {
			                    "operation": "create_node",
			                    "id": $('#treenode_sel').val(),
			                    "type": 'default'
			                },
			                function (r) {
			                    $('#log2').html(r);
			                }
		                );
                    }
                    return false;
                });
                $("#ImgFolderModify").click(function (e) {
                    e.stopImmediatePropagation();
                    if ($('#treenode_sel').val().indexOf('root_') >= 0) return false;

                    if ($('#tree .jstree-clicked').length > 0) {
                        $.post(
			                "../Project/SearchProject_getFolders.aspx",
			                {
			                    "operation": "rename_node",
			                    "id": $('#treenode_sel').val(),
			                    "type": 'default'
			                },
			                function (r) {
			                    $('#log2').html(r);
			                }
		                );
                    }
                    return false;
                });
                $("#ImgFolderRemove").click(function (e) {
                    e.stopImmediatePropagation();
                    if ($('#treenode_sel').val().indexOf('root_') >= 0) return false;

                    if ($('#tree .jstree-clicked').length > 0) {
                        $.ajax({
                            async: false,
                            type: 'POST',
                            timeout: 2160000,
                            url: "../Project/SearchProject_getFolders.aspx",
                            data: {
                                "operation": "remove_node",
                                "id": $('#treenode_sel').val()
                            },
                            success: function (r) {
                                $('#log2').html(r);
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
                            }
                        });
                    }
                    return false;
                });

                var nodes;
                function searchTree(qry) {
                    // set all nodes color back to normal
                    $("#tree").find('.jstree-search').removeClass('jstree-search');
                    $("#tree").find('.jstree-clicked').removeClass('jstree-clicked');

                    $.getJSON('../Project/SearchProject_getFolders.aspx', { 'q': qry }, function (data) {
                        if (data != null) {
                            // server returns a list of object that is { string id, bool isResult }.  
                            // the results contains any elements in the search and all of the parents of that element.  
                            // all these elements are necessary to expand all the branches. 
                            // only search results return a true value for isResult
                            nodes = data;
                            var i = 0;

                            /*
                            $('#tree').bind('open_node.jstree', function (e, d) {
                            var index = -1;
                            for (var j = 0; j < data.length; j++) if (data[j].id == d.rslt.obj[0].id) index = j;
                            searchTreeFromIndex(index);
                            });
                            */
                            if (data.length == 1 && false) {// delete false condition to autoselect folder when result is one
                                $("#tree").jstree("select_node", "#" + data[0].id).trigger("select_node.jstree");
                            }
                            else {
                                while (i < data.length) {
                                    var node = '#' + data[i].id;
                                    if (data[i].isResult == 'true') {
                                        $(node).find('a').addClass('jstree-search');
                                        i++;
                                    }
                                    else {
                                        $('#tree').jstree('open_node', $(node));
                                        i++;
                                    }
                                }
                            }
                        }
                    })
                    .error(function (jqXHR, textStatus, errorThrown) {
                        $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
                    });

                    // open the tree
                    $("#tree").jstree('open_all');

                    $('.retvalSearchSubset input').get(0).value = '';
                    //                    }
                }

                function searchTreeFromIndex(index) {
                    for (var j = index; j < nodes.length; j++) {
                        if (nodes[j]) {
                            var node = '#' + nodes[j].id;
                            if (nodes[j].isResult == 'true') {
                                $(node).find('a').addClass('jstree-search');
                            }
                            else {
                                $('#tree').jstree('open_node', $(node));
                            }
                        }
                    }
                }

                function DocumentsToggle(o) {
                    $('#documents tbody input').attr('checked', o.checked);
                }

                $(".tbl_rounded tr.NormalRow td").hover(function () {
                    var row = $(this).parent().parent().children().index($(this).parent());
                    var index = row + 1;
                    if (row % 2 == 0) index = row - 1;
                    $($(".tbl_rounded tr").get(index)).addClass("NormalRowHover");
                }, function () {
                    var row = $(this).parent().parent().children().index($(this).parent());
                    var index = row + 1;
                    if (row % 2 == 0) index = row - 1;
                    $($(".tbl_rounded tr").get(index)).removeClass("NormalRowHover");
                });

                $(".tbl_rounded tr.AltRow td").hover(function () {
                    var row = $(this).parent().parent().children().index($(this).parent());
                    var index = row + 1;
                    if (row % 2 == 0) index = row - 1;
                    $($(".tbl_rounded tr").get(index)).addClass("AltRowHover");
                }, function () {
                    var row = $(this).parent().parent().children().index($(this).parent());
                    var index = row + 1;
                    if (row % 2 == 0) index = row - 1;
                    $($(".tbl_rounded tr").get(index)).removeClass("AltRowHover");
                });
            });
        };
      
    </script>
</asp:Content>
