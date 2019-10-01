<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmistamentoDocumenti.aspx.cs"
    Inherits="NttDataWA.Popup.SmistamentoDocumenti" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc9" TagName="ViewDocument" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

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

        function GetCommentoRifiuto() {
            var msg = prompt('Per poter rifiutare questa trasmissione, inserire un messaggio di rifiuto', '');

            if (msg == null) {

            }

            if (msg == '') {
                alert("Attenzione, per poter rifiutare un documento bisogna inserire alcune note di rifiuto");
            }

            if ((msg != '' && msg != null && msg != 'null' && msg != 'undefined')) {
                document.Form1.hd_msg_rifiuto.value = msg;
            }
        }


        function resizeSmistamento() {
            var height = 0;
            var height2 = 0;
            var totalHeight = document.documentElement.clientHeight;
            var availableHeight = totalHeight - 354;
            height = Math.round((availableHeight / 5)) * 3;

            if (document.getElementById("<%=this.grdUOApp.ClientID%>")) {
                document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.height = height + "px";
                document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.overflow = 'auto';
            }

            if (document.getElementById("<%=this.grdUOInf.ClientID%>") || document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.height != 0) {
                height2 = availableHeight - height;
                document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.height = height2 + "px";
                document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.overflow = 'auto';
                try {
                    var prova = document.getElementById("<%=this.grdUOInf.ClientID%>").style.height;

                    if (document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.height < height2) {

                        var height3 = height + height2 - document.getElementById("<%=this.grdUOInf.ClientID%>").style.height;
                        document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.height = height3 + "px";
                        document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.overflow = 'auto';
                    }
                }

                catch (ex) {
                    document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.height = availableHeight + "px";
                    document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.overflow = 'auto';
                    document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.height = 0 + "px";
                    document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.overflow = 'hidden';
                }
            }
            else {
                document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.height = availableHeight + "px";
                document.getElementById("<%=this.smistamentoGriglia1Table.ClientID%>").style.overflow = 'auto';
                document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.height = 0 + "px";
                document.getElementById("<%=this.smistamentoGriglia2Table.ClientID%>").style.overflow = 'hidden';
            }


        };

        function setTooltipNoteGen() {
            $get('<%=txtNoteGen.ClientID %>').title = $get('<%=txtNoteGen.ClientID %>').value;
        }

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
    </script>
    <style type="text/css">
        .tbl_rounded_custom th
        {
            white-space: nowrap;
            font-size: 11px;
            padding: 0px;
        }
        
        .tbl_rounded_custom td
        {
            padding: 0px;
            font-size: 11px;
            line-height: 13px;
            padding: 2px;
        }
        .tbl_rounded_custom td img
        {
            border: 0px;
        }
        .col12
        {
            margin: 0px;
            padding: 0px;
        }
        .col-marginSx2
        {
            float: left;
            margin: 0px 5px 0px 0px;
            text-align: left;
            white-space: nowrap;
        }
        
        .col-marginSx2 p
        {
            margin: 0px;
            padding: 0px;
            font-weight: normal;
            margin-top: 4px;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx"
        PermitClose="false" Width="600" Height="400" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup Id="SearchProject" runat="server" Url="../popup/SearchProject.aspx?caller=search"
        PermitClose="false" Width="600" Height="400" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup Id="NoteSmistamento" runat="server" Url="../popup/NoteSmistamento.aspx"
        PermitClose="true" PermitScroll="false" IsFullScreen="false" Width="650" Height="430"
        Title="Inserimento dati aggiuntivi smistamento" CloseFunction="function (event, ui)  {$('#btnNoteSmistamento').click();}" />
    <uc:ajaxpopup Id="SmistaDocSelectionsDetails" runat="server" Url="../Popup/SmistaDocSelectionsDetails.aspx"
        PermitClose="false" Width="850" Height="650" PermitScroll="false" IsFullScreen="true"
        Title="Selezioni smistamento" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup Id="RejectTransmissions" runat="server" Url="../popup/RejectTransmissions.aspx"
        PermitClose="true" PermitScroll="false" IsFullScreen="false" Width="530" Height="430"
        Title="Rifiuto Trasmissione" CloseFunction="function (event, ui)  {$('#btnRejectTransmission').click();}" />
    <uc:ajaxpopup Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup Id="ViewNoteGen" runat="server" Url="../popup/ViewNote.aspx?tiponota=g" Width="600"
        Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup Id="ViewNoteInd" runat="server" Url="../popup/ViewNote.aspx?tiponota=i&readonly=true" Width="600"
        Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', '');}" />
    <div class="containerSmistaDoc">
        <div class="contentSmistaDocTopSx">
            <div id="ContentSmistamentoSx">
                <asp:UpdatePanel ID="upPnlInfoDocument" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="smistaFieldsetContent">
                            <div class="smistaInnerCont">
                                <div class="rowSmista2">
                                    <div class="col2">
                                        <asp:Label ID="lbl_segnatura" runat="server" />
                                    </div>
                                    <asp:PlaceHolder runat="server" Visible="false" ID="PlcType">
                                        <div class="col2">
                                            - <span class="weight">
                                                <asp:Literal ID="ltlRepertorio" runat="server"></asp:Literal></span>
                                        </div>
                                        <div class="col2">
                                            <asp:Label ID="lbl_segn_repertorio" runat="server" CssClass="redWeight" />
                                        </div>
                                        <div class="col2">
                                            - <span class="weight">
                                                <asp:Literal ID="ltlTipologia" runat="server"></asp:Literal></span>
                                        </div>
                                        <div class="col2">
                                            <asp:Label ID="lblTipology" runat="server" />
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                                <div class="rowSmista2">
                                    <div class="col2">
                                        <span class="weight">
                                            <asp:Literal ID="ltl_dataCreazione" runat="server" />
                                        </span>
                                        <asp:Label ID="lbl_dataCreazione" runat="server" />
                                    </div>
                                    <div class="col2">
                                        -
                                        <asp:Label ID="lbl_allegati" runat="server" />
                                        <span class="weight">
                                            <asp:Literal ID="ltl_attachments" runat="server" />
                                        </span>
                                    </div>
                                    <div class="col2">
                                        -
                                        <asp:Label ID="lbl_versioni" runat="server" />
                                        <span class="weight">
                                            <asp:Literal ID="ltl_versions" runat="server" />
                                        </span>
                                    </div>
                                    <div class="col2">
                                        - <span class="weight">
                                            <asp:Literal ID="ltl_descRagTrasm" runat="server" />
                                        </span>
                                        <asp:Label ID="lbl_descRagTrasm" runat="server" />
                                    </div>
                                </div>
                                <asp:PlaceHolder runat="server" Visible="false" ID="PlcSender">
                                    <div class="rowSmista2">
                                        <div class="col2" style="width: 98%">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal ID="ltl_mittente" runat="server" />
                                                </span>
                                            </div>
                                            <div class="col12" style="width: 97%">
                                                <div class="esmaninaTextOverflow">
                                                    <asp:Label ID="lbl_mittente" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible="false" ID="PlcRecipient">
                                    <div class="rowSmista2">
                                        <div class="col2">
                                            <span class="weight">
                                                <asp:Literal ID="ltl_destinatario" runat="server" />
                                            </span>
                                            <asp:Label ID="lbl_destinatario" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <div class="rowSmista2">
                                    <div class="col2">
                                        <span class="weight">
                                            <asp:Literal ID="ltl_oggetto" runat="server" />
                                        </span>
                                        <asp:Label ID="lbl_oggetto" runat="server" CssClass="clickableright"></asp:Label>
                                    </div>
                                </div>
                                <asp:PlaceHolder runat="server" ID="PlcMittTrasm">
                                    <div class="rowSmista2">
                                        <div class="col2" style="width: 98%">
                                            <div class="col-marginSx2">
                                                <span class="weight">
                                                    <asp:Literal ID="ltl_mitt_trasm" runat="server" />
                                                </span>
                                            </div>
                                            <div class="col12" style="width: 97%">
                                                <div class="esmaninaTextOverflow" >
                                                    <asp:Label ID="lbl_mitt_trasm" runat="server" ></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                        <div class="rowSmista">
                            <asp:Panel ID="pnl_navigationButtons" runat="server">
                                <asp:UpdatePanel ID="UpPnlCheckDoc" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                                    <ContentTemplate>
                                        <div class="colNavigation">
                                            <table class="tableSmis">
                                                <tr>
                                                    <td class="tableSmis10">
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <cc1:CustomImageButton runat="server" ID="btn_first" ImageUrl="../Images/Icons/smistamento_left_left.png"
                                                            CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_left_left.png"
                                                            ImageUrlDisabled="../Images/Icons/smistamento_left_left_disabled.png" OnMouseOverImage="../Images/Icons/smistamento_left_left_hover.png"
                                                            OnClick="btn_first_Click" OnClientClick="disallowOp('Content2');" />
                                                    </td>
                                                    <td>
                                                        <cc1:CustomImageButton runat="server" ID="btn_previous" ImageUrl="~/Images/Icons/smistamento_left.png"
                                                            CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_left.png" ImageUrlDisabled="../Images/Icons/smistamento_left_disabled.png"
                                                            OnMouseOverImage="../Images/Icons/smistamento_left_hover.png" OnClick="btn_previous_Click"
                                                            OnClientClick="disallowOp('Content2');" />
                                                    </td>
                                                    <td>
                                                        <span class="weight">
                                                            <asp:Label ID="lbl_contatore" runat="server" CssClass="blueNavigation"></asp:Label></span>
                                                    </td>
                                                    <td>
                                                        <cc1:CustomImageButton runat="server" ID="btn_next" ImageUrl="../Images/Icons/smistamento_right.png"
                                                            CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_right.png"
                                                            ImageUrlDisabled="../Images/Icons/smistamento_right_disabled.png" OnMouseOverImage="../Images/Icons/smistamento_right_hover.png"
                                                            OnClick="btn_next_Click" OnClientClick="disallowOp('Content2');" />
                                                    </td>
                                                    <td>
                                                        <cc1:CustomImageButton runat="server" ID="btn_last" ImageUrl="../Images/Icons/smistamento_right_right.png"
                                                            CssClass="clickable" OnMouseOutImage="../Images/Icons/smistamento_right_right.png"
                                                            ImageUrlDisabled="../Images/Icons/smistamento_right_right_disabled.png" OnMouseOverImage="../Images/Icons/smistamento_right_right_hover.png"
                                                            OnClick="btn_last_Click" OnClientClick="disallowOp('Content2');" />
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chk_showDoc" runat="server" TextAlign="Right" AutoPostBack="True"
                                                            OnCheckedChanged="chk_showDoc_CheckedChanged" onclick="disallowOp('Content2');"
                                                            CssClass="clickable"></asp:CheckBox>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chk_mantieniSel" runat="server" TextAlign="Right" AutoPostBack="True"
                                                            CssClass="clickable"></asp:CheckBox>
                                                    </td>
                                                    <td>
                                                        <cc1:CustomImageButton runat="server" ID="btn_clearFlag" ImageUrl="../Images/Icons/refresh_smistamento.png"
                                                            OnMouseOutImage="../Images/Icons/refresh_smistamento.png" OnMouseOverImage="../Images/Icons/refresh_smistamento_hover.png"
                                                            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/refresh_smistamento_disabled.png"
                                                            OnClick="btn_clearFlag_Click" OnClientClick="disallowOp('Content2');" />
                                                        <cc1:CustomImageButton ID="btn_selezioniSmistamento" ImageUrl="../images/Icons/view_smistamento.png"
                                                            runat="server" OnMouseOverImage="../images/Icons/view_smistamento_hover.png"
                                                            OnMouseOutImage="../images/Icons/view_smistamento.png" CssClass="clickableLeft"
                                                            ImageUrlDisabled="../images/Icons/view_smistamento_disabled.png" OnClientClick="disallowOp('Content2');"
                                                            OnClick="btn_selezioniSmistamento_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </asp:Panel>
                        </div>
                        <div class="rowSmista">
                            <fieldset class="smistaFieldsetContent">
                                <asp:UpdatePanel ID="upPnlNote" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="rowSmista">
                                            <div class="colHalfSmist">
                                                <span class="weight">
                                                    <asp:Literal ID="LitNoteGeneral" runat="server" /></span></div>
                                            <div class="colHalfSmist2">
                                                <%--<cc1:CustomTextArea ID="txtNoteGen" runat="server"
                                                        CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static"></cc1:CustomTextArea>--%>
                                                <cc1:CustomTextArea ID="txtNoteGen" runat="server" CssClass="txt_input_full" ClientIDMode="Static"
                                                    CssClassReadOnly="txt_input_full_disabled" onchange="return setTooltipNoteGen();" /></div>
                                            <div class="col-right-no-marginSmista">
                                                <cc1:CustomImageButton runat="server" ID="DocumentImgNoteGenDetail" ImageUrl="../Images/Icons/ico_objects.png"
                                                OnMouseOutImage="../Images/Icons/ico_objects.png" OnMouseOverImage="../Images/Icons/ico_objects_hover.png"
                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/ico_objects_disabled.png"
                                                OnClick="DocumentImgNoteGenDetail_Click" OnClientClick="disallowOp('Content2');"  />
                                            </div>
                                            <div class="row">
                                                <div class="col-right-no-margin">
                                                    <span class="charactersAvailable">
                                                        <asp:Literal ID="LtrNote" runat="server" ClientIDMode="Static"> </asp:Literal>
                                                        <span id="txtNoteGen_chars" clientidmode="Static" runat="server"></span></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="rowSmista">
                                            <div class="colHalfSmist">
                                                <span class="weight">
                                                    <asp:Literal ID="LitNoteIndividual" runat="server" /></span></div>
                                            <div class="colHalfSmist2">
                                                <cc1:CustomTextArea ID="txtAreaNoteInd" runat="server" ReadOnly="true" CssClass="clickable txt_input_full"
                                                    CssClassReadOnly="txt_input_full_disabled" /></div>
                                            <div class="col-right-no-marginSmista">
                                                <cc1:CustomImageButton runat="server" ID="DocumentImgNoteIndDetail" ImageUrl="../Images/Icons/ico_objects.png"
                                                OnMouseOutImage="../Images/Icons/ico_objects.png" OnMouseOverImage="../Images/Icons/ico_objects_hover.png"
                                                CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/ico_objects_disabled.png"
                                                OnClick="DocumentImgNoteIndDetail_Click"  OnClientClick="disallowOp('Content2');" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                            <fieldset style="margin-top: 3px; margin-bottom: 3px;" class="smistaFieldsetContent">
                                <div class="rowSmista">
                                    <div class="col">
                                        <span class="weight">
                                            <asp:Literal ID="DocumentLitTransmRapid" runat="server"></asp:Literal></span>
                                    </div>
                                </div>
                                <div class="rowSmista">
                                    <asp:UpdatePanel runat="server" ID="UpPnlTransmissionsModel" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_trasm_rapida" runat="server" CssClass="chzn-select-deselect"
                                                    Width="99%" OnSelectedIndexChanged="ddl_trasm_rapida_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                            <asp:HiddenField runat="server" ID="HiddenControlPrivateTrans" ClientIDMode="Static" />
                                            <asp:HiddenField runat="server" ID="HiddenControlPrivateClass" ClientIDMode="Static" />
                                            <asp:HiddenField runat="server" ID="HiddenControlPrivateTypeOperation" ClientIDMode="Static" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <asp:UpdatePanel ID="UpPnlProjectRapid" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                    <ContentTemplate>
                                        <div class="rowSmista">
                                            <div class="col" style="margin-top: 5px;">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="DocumentLitClassificationRapid"></asp:Literal></span><asp:Label
                                                        ID="LblClassRequired" CssClass="little" Visible="false" runat="server">*</asp:Label></p>
                                            </div>
                                            <div class="col-right-no-marginSmista">
                                                <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                    runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                    OnMouseOutImage="../Images/Icons/classification_scheme.png" CssClass="clickableLeft"
                                                    ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png" OnClientClick="return ajaxModalPopupOpenTitolario();" />
                                                <cc1:CustomImageButton ID="DocumentImgSearchProjects" ImageUrl="../Images/Icons/search_projects.png"
                                                    runat="server" OnMouseOverImage="../Images/Icons/search_projects_hover.png" OnMouseOutImage="../Images/Icons/search_projects.png"
                                                    ImageUrlDisabled="../Images/Icons/search_projects_disabled.png" CssClass="clickableLeft"
                                                    OnClick="DocumentImgSearchProjects_Click" />
                                                <cc1:CustomImageButton ID="ImgAddProjects" ImageUrl="../Images/Icons/add_sub_folder.png"
                                                    runat="server" OnMouseOverImage="../Images/Icons/add_sub_folder_hover.png" OnMouseOutImage="../Images/Icons/add_sub_folder.png"
                                                    ImageUrlDisabled="../Images/Icons/add_sub_folder_disabled.png" alt="Titolario" Visible = "false"
                                                    CssClass="clickableLeftN" OnClick="ImgAddProjects_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
                                            </div>
                                        </div>
                                        <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="rowSmista">
                                                    <asp:PlaceHolder runat="server" ID="PnlProject">
                                                        <asp:HiddenField ID="IdProject" runat="server" />
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft"
                                                                AutoComplete="off" OnTextChanged="TxtCodeProject_OnTextChanged" AutoPostBack="true"
                                                                CssClassReadOnly="txt_addressBookLeft_disabled" onchange="disallowOp('Content2');">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_addressBookRight"
                                                                    CssClassReadOnly="txt_addressBookRight_disabled">
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
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="rowSmista">
                    <!-- GRID UO-->
                    <div class="smistamentoGriglia1" id="smistamentoGriglia1Table" runat="server" clientidmode="static">
                        <asp:UpdatePanel runat="server" ID="updatePanelUOAppartenenza" UpdateMode="Conditional"
                            class="UpnlGrid">
                            <ContentTemplate>
                                <asp:Panel ID="pnlContainerUoAppartenenza" runat="server" ScrollBars="Auto">
                                    <asp:GridView ID="grdUOApp" runat="server" Width="99%" AutoGenerateColumns="False"
                                        CssClass="tbl_rounded_custom round_onlyextreme" AllowCustomPaging="true" ShowHeaderWhenEmpty="True"
                                        OnRowCommand="grdUOApp_RowCommand" DataKeyNames="id,idcorr,type">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <Columns>
                                            <asp:BoundField DataField="ID" Visible="false" HeaderText="ID" />
                                            <asp:BoundField DataField="TYPE" Visible="false" HeaderText="TYPE" />
                                            <asp:TemplateField HeaderText='<%$ localizeByText:SmistamentoUoAppartenenza%>'>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescUO" runat="server" Text='<%# Bind("DESCRIZIONE") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Width="60%" />
                                                <ItemStyle VerticalAlign="Middle" Wrap="True" />
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="30" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="CustomImageButton1" runat="server" CommandName="navigaUO_up"
                                                        ImageUrl="../Images/Icons/smistamento_up_arrow.png" OnMouseOutImage="../Images/Icons/smistamento_up_arrow.png"
                                                        OnMouseOverImage="../Images/Icons/smistamento_up_arrow_hover.png" CssClass="clickableLeft"
                                                        ImageUrlDisabled="../Images/Icons/smistamento_up_arrow_disabled.png" ToolTip="Vai alla UO Superiore"
                                                        OnClientClick="disallowOp('Content2');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <HeaderStyle Width="25%"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="hd_id" Text='<%# DataBinder.Eval(Container, "DataItem.PARENT") %>' />
                                                    <asp:Label runat="server" ID="hdGerarchia" Text='<%# DataBinder.Eval(Container, "DataItem.GERARCHIA") %>' />
                                                    <asp:Label runat="server" ID="hd_disablendTrasm" Text='<%# DataBinder.Eval(Container, "DataItem.DISABLED_TRASM") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="TRASM">
                                                <HeaderStyle Width="30" HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="ImgButtonTrasmesso" runat="server" ImageAlign="Middle"
                                                        ImageUrl="../Images/Icons/flag_ok.png" OnMouseOutImage="../Images/Icons/flag_ok.png"
                                                        OnMouseOverImage="../Images/Icons/flag_ok.png" CssClass="clickableLeft"
                                                        ImageUrlDisabled="../Images/Icons/flag_ok.png" ToolTip="Già raggiunto da trasmissione" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="COMP">
                                                <HeaderStyle Width="30" HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkComp" runat="server" TextAlign="Right" AutoPostBack="true" OnCheckedChanged="OpzioniUoAppartenenza_CheckedChanged"
                                                        onclick="disallowOp('Content2');" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="CC">
                                                <HeaderStyle Width="30" HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkCC" runat="server" TextAlign="Right" AutoPostBack="true" OnCheckedChanged="OpzioniUoAppartenenza_CheckedChanged"
                                                        onclick="disallowOp('Content2');" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Not.">
                                                <HeaderStyle Width="30"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkNotifica" runat="server" TextAlign="Right" AutoPostBack="true"
                                                        Enabled="false" OnCheckedChanged="OpzioniUoAppartenenza_CheckedChanged" onclick="disallowOp('Content2');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Note">
                                                <HeaderStyle Width="30"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="imgNote" CommandName="imgNote" runat="server" ImageUrl="../Images/Icons/ico_objects.png"
                                                        OnMouseOutImage="../Images/Icons/ico_objects.png" OnMouseOverImage="../Images/Icons/ico_objects.png"
                                                        CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/ico_objects_disabled.png"
                                                        ToolTip="Inserimento dati aggiuntivi" Visible="false" OnClientClick="disallowOp('Content2');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>                                         
                                            <asp:BoundField DataField="IDCORR" Visible="false" HeaderText="IDCORR" />
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <!-- GRID UFF -->
                <div class="smistamentoGriglia2" id="smistamentoGriglia2Table" runat="server" clientidmode="static">
                    <asp:UpdatePanel runat="server" ID="updatePanelUOInf" UpdateMode="Conditional" class="UpnlGrid">
                        <ContentTemplate>
                            <asp:Panel ID="pnlContainerUoInferiori" runat="server">
                                <asp:GridView ID="grdUOInf" runat="server" Width="99%" AutoGenerateColumns="False"
                                    CssClass="tbl_rounded_custom round_onlyextreme" AllowCustomPaging="true" ShowHeaderWhenEmpty="True"
                                    OnRowCommand="grdUOInf_RowCommand" OnSelectedIndexChanged="grdUOInf_SelectedIndexChanged"
                                    DataKeyNames="id,idcorr,type">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <Columns>
                                        <asp:BoundField Visible="false" DataField="ID" HeaderText="ID" />
                                        <asp:BoundField Visible="False" DataField="TYPE" HeaderText="TYPE" />
                                        <asp:TemplateField HeaderText='<%$ localizeByText:SmistamentoUoInf%>'>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescUOInf" runat="server" Text='<%# Bind("DESCRIZIONE") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="70%" />
                                            <ItemStyle VerticalAlign="Middle" Wrap="True" />
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-Width="30" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <cc1:CustomImageButton runat="server" CommandName="navigaUO_down" ImageUrl="../Images/Icons/smistamento_down_arrow.png"
                                                    OnMouseOutImage="../Images/Icons/smistamento_down_arrow.png" OnMouseOverImage="../Images/Icons/smistamento_down_arrow_hover.png"
                                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/smistamento_down_arrow_disabled.png"
                                                    ToolTip="Naviga UO Inferiore" OnClientClick="disallowOp('Content2');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="COMP">
                                            <HeaderStyle Width="30"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkComp" runat="server" TextAlign="Right" Text="" AutoPostBack="true"
                                                    Enabled="false" OnCheckedChanged="OpzioniUoInferiori_CheckedChanged" onclick="disallowOp('Content2');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CC">
                                            <HeaderStyle Width="30"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkCC" runat="server" TextAlign="Right" AutoPostBack="true" OnCheckedChanged="OpzioniUoInferiori_CheckedChanged"
                                                    onclick="disallowOp('Content2');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Note">
                                            <HeaderStyle Width="30"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <cc1:CustomImageButton ID="imgNote" CommandName="imgNote" runat="server" ImageUrl="../Images/Icons/ico_objects.png"
                                                    OnMouseOutImage="../Images/Icons/ico_objects.png" OnMouseOverImage="../Images/Icons/ico_objects.png"
                                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/ico_objects_disabled.png"
                                                    ToolTip="Inserimento dati aggiuntivi" Visible="false" OnClientClick="disallowOp('Content2');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="">
                                            <HeaderStyle Width="30"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="false">
                                            <HeaderStyle Width="25%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="hd_isAllowedSmista" Text='<%# DataBinder.Eval(Container, "DataItem.DISABLED_TRASM") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>                                      
                                        <asp:BoundField DataField="idcorr" Visible="false" HeaderText="idcorr" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div class="contentSmistaDocDx">
            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpPnlViewer">
                <ContentTemplate>
                    <div id="contentDocumentViewer" align="center">
                        <uc9:ViewDocument runat="server" PageCaller="SMISTAMENTO" ID="ViewDocument" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnSmistamentoZoom" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnSmistamentoZoom_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSmistamentoDetSign" runat="server" CssClass="btnEnable"
                Enabled="false" CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="BtnSmistamentoDetSign_Click"
                OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSmistamentoAdlU" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnSmistamentoAdlU_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSmistamentoAdlR" runat="server" CssClass="btnEnable" OnClick="BtnSmistamentoAdlR_Click"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSmistamentoAccept" runat="server" CssClass="btnEnable clickableLargeLeft"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="BtnSmistamentoAccept_Click"
                OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSmistamentoAcceptLF" runat="server" CssClass="btnEnable clickable"
                CssClassDisabled="btnDisable" OnClientClick="disallowOp('Content2');" OnMouseOver="btnHover"
                OnClick="BtnSmistamentoAcceptLF_Click" />
            <cc1:CustomButton ID="BtnSmistamentoReject" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnSmistamentoReject_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSmistamentoSmista" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnSmistamentoSmista_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSmistamentoClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnSmistamentoClose_Click" OnClientClick="disallowOp('Content2');" />
            <asp:HiddenField ID="hdUOapp" runat="server" />
            <asp:HiddenField ID="hdIsEnabledNavigaUO" runat="server" />
            <asp:HiddenField ID="txtNoteInd" runat="server" />
            <asp:HiddenField ID="hdCheckCompetenza" runat="server" />
            <asp:HiddenField ID="hdDescrCompetenza" runat="server" />
            <asp:HiddenField ID="hdDescrConoscenza" runat="server" />
            <asp:HiddenField ID="hdCheckConoscenza" runat="server" />
            <asp:HiddenField ID="hdCheckedUtenti" runat="server" />
            <asp:HiddenField ID="hdTipoDestCompetenza" runat="server" />
            <asp:HiddenField ID="hdTipoDestConoscenza" runat="server" />
            <asp:HiddenField ID="hdCountNavigaDown" runat="server" Value="0" />
            <asp:HiddenField ID="hdSelSmista" runat="server" />
            <asp:HiddenField ID="hd_msg_rifiuto" runat="server" />
            <asp:Button ID="btnProjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnProjectPostback_Click" />
            <asp:Button ID="btnNoteSmistamento" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnNoteSmistamento_Click" />
            <asp:Button ID="btnSmistaDocSelectionsDetailsPostback" runat="server" CssClass="hidden"
                ClientIDMode="Static" OnClick="btnSmistaDocSelectionsDetailsPostback_Click" />
            <asp:Button ID="btnRejectTransmission" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnRejectTransmission_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
