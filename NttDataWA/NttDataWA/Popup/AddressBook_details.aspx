<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="AddressBook_details.aspx.cs" Inherits="NttDataWA.Popup.AddressBook_details" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Import Namespace="NttDataWA.DocsPaWR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        p
        {
            text-align: left;
        }
        em
        {
            font-style: normal;
            font-weight: bold;
            color: #9D9D9D;
        }
        .col-label
        {
            width: 100px;
            overflow: hidden;
            padding: 5px 0 0 0;
        }
        div div.col-label:last-child
        {
            color: #f00;
        }
        .txt_input, .txt_input_disabled, .txt_textarea, .txt_textarea_disabled
        {
            width: 472px;
            height: 1.5em;
        }
        .txt_textarea, .txt_textarea_disabled
        {
            height: 3em;
            vertical-align: top;
        }
        .txt_input_disabled
        {
            background-color: #ffffff;
        }
        .txt_input_small
        {
            width: 168px;
        }
        .tbl tr.NormalRow:hover td
        {
            background-color: #fff;
        }
        .tbl tr.AltRow:hover td
        {
            background-color: #e1e9f0;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function acePopulatedTxtCap(sender, e) {
            var behavior = $find('AutoCompleteTxtCap');
            behavior.set_minimumPrefixLength(3);
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
        function aceSelectedTxtCap(sender, e) {
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

            var searchText = $get('<%=txt_cap.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            document.getElementById("<%=txt_cap.ClientID%>").value = testo
            document.getElementById("<%=txt_citta.ClientID%>").focus();
        }

        function acePopulatedTxtCitta(sender, e) {
            var behavior = $find('AutoCompleteTxtCitta');
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
        function aceSelectedTxtCitta(sender, e) {
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

            var searchText = $get('<%=txt_citta.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            document.getElementById("<%=txt_citta.ClientID%>").value = testo
            document.getElementById("<%=txt_cap.ClientID%>").focus();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div class="row">
            <asp:Literal ID="lbl_nomeCorr" runat="server" Visible="false" />
        </div>
        <asp:PlaceHolder ID="pnl_registro" runat="server" Visible="false">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_registro" runat="server" /></div>
                <div class="col-no-margin">
                    <asp:Literal ID="lit_registro" runat="server" Visible="false" />
                    <asp:DropDownList ID="ddl_registri" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect" width="300px" OnSelectedIndexChanged="ddl_registri_SelectedIndexChanged" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="pnl_email" runat="server">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_codAOO" runat="server" /><asp:Literal ID="starCodAOO" runat="server"
                        Visible="false">*</asp:Literal></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_codAOO" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler" />
                </div>
            </div>
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_codAmm" runat="server" /><asp:Literal ID="starCodAmm" runat="server"
                        Visible="false">*</asp:Literal></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_codAmm" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcSingleMail" runat="server">
                <asp:UpdatePanel ID="updPanel1" runat="server" UpdateMode="Always" Visible="true">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col col-label">
                                <asp:Literal ID="lbl_email" runat="server" /><asp:Literal ID="starEmail" runat="server">*</asp:Literal></div>
                            <div class="col-no-margin">
                                <cc1:CustomTextArea ID="txt_email" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                                    OnTextChanged="DataChangedHandler" />
                                <cc1:CustomTextArea ID="txtCasella" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                                    Style="width: 400px;" />
                                <cc1:CustomImageButton runat="server" ID="imgAggiungiCasella" ImageUrl="../Images/Icons/add_version.png"
                                    OnMouseOutImage="../Images/Icons/add_version.png" OnMouseOverImage="../Images/Icons/add_version_hover.png"
                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/add_version_disabled.png"
                                    OnClick="imgAggiungiCasella_Click" />
                            </div>
                        </div>
                        <asp:PlaceHolder ID="plcNoteMail" runat="server">
                            <div class="row">
                                <div class="col col-label">
                                    <asp:Literal ID="lblNote" runat="server" /></div>
                                <div class="col-no-margin">
                                    <cc1:CustomTextArea ID="txtNote" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" TextMode="MultiLine" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:PlaceHolder>
            <asp:UpdatePanel ID="updPanelMail" runat="server" UpdateMode="Always" Visible="true">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-full">
                        <div style="margin-top:30px;">
                            <asp:GridView ID="gvCaselle" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvCaselle_RowDataBound"
                                CellPadding="1" BorderWidth="1px" BorderColor="Gray" CssClass="tbl" EnableViewState="true">
                                <RowStyle CssClass="NormalRow"  Width="100%"/>
                                <AlternatingRowStyle CssClass="AltRow" />
                                <Columns>
                                    <asp:TemplateField HeaderText="SystemId" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblSystemId" Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).systemId %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="68%"></ItemStyle>
                                        <ItemTemplate>
                                            <cc1:CustomTextArea AutoPostBack="true" Width="310px" OnTextChanged="txtEmailCorr_TextChanged"
                                                ID="txtEmailCorr" runat="server" ToolTip='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'
                                                CssClass="txt_input" CssClassReadOnly="txt_input_disabled" Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'></cc1:CustomTextArea>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Note E-mail" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="28%"></ItemStyle>
                                        <ItemTemplate>
                                            <cc1:CustomTextArea AutoPostBack="true" Width="110px" MaxLength="20" OnTextChanged="txtNoteMailCorr_TextChanged"
                                                ID="txtNoteMailCorr" runat="server" ToolTip='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'
                                                CssClass="txt_input" CssClassReadOnly="txt_input_disabled" Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'></cc1:CustomTextArea>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="*" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                                        <ItemTemplate>
                                            <asp:RadioButton ID="rdbPrincipale" runat="server" AutoPostBack="true" OnCheckedChanged="rdbPrincipale_ChecekdChanged"
                                                Checked='<%# TypeMailCorrEsterno(((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Principale) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                                        <ItemTemplate>
                                            <cc1:CustomImageButton runat="server" ID="imgEliminaCasella" ImageUrl="../Images/Icons/delete.png"
                                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/delete.png" OnClick="imgEliminaCasella_Click"  AlternateText='<%# GetLabelDelete() %>' ToolTip='<%# GetLabelDelete() %>'
                                                AutoPostBack="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:PlaceHolder ID="plcPreferredChannel" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_preferredChannel" runat="server" /></div>
                    <div class="col-no-margin">
                        <asp:DropDownList ID="dd_canpref" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dd_canpref_SelectedIndexChanged"
                            CssClass="chzn-select-deselect" Width="200px" />
                    </div>
                    <div class="col-left" style=" padding-left:20px; padding-top:5px;">
                        <asp:CheckBox ID="cbInteroperanteRGS" runat="server" Visible="false" Checked="false" OnCheckedChanged="DataChangedHandler" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="row">
                <p style="border-bottom: 1px solid #2a7bbd;">
                    &nbsp;</p>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="pnlStandard" runat="server">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_tipocorr" runat="server" /></div>
                <div class="col-no-margin">
                    <asp:DropDownList ID="ddl_tipoCorr" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                        Enabled="false" Width="200px" OnSelectedIndexChanged="ddl_tipoCorr_SelectedIndexChanged" />
                </div>
            </div>
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_CodRubrica" runat="server" /><asp:Label ID="starRubrica" runat="server">*</asp:Label></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_CodRubrica" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler" />
                </div>
            </div>
            <asp:PlaceHolder ID="pnl_descrizione" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_descrizione" runat="server" />*</div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_descrizione" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"
                            OnTextChanged="DataChangedHandler" TextMode="MultiLine" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnl_titolo" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_titolo" runat="server" /></div>
                    <div class="col-no-margin">
                        <asp:DropDownList ID="ddl_titolo" runat="server" CssClass="chzn-select-deselect"
                            Width="200px" OnSelectedIndexChanged="DataChangedHandler" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnl_nome_cogn" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_nome" runat="server" />*</div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_nome" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_cognome" runat="server" />*</div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_cognome" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnl_infonascita" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_luogonascita" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_luogoNascita" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_dataNascita" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_dataNascita" runat="server" CssClass="txt_textdata datepicker"
                            CssClassReadOnly="txt_textdata_disabled" Columns="10" OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnl_indirizzo" runat="server">
                <div class="row">
                    <div class="col col-label"><asp:Literal ID="lbl_indirizzo" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_indirizzo" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
                <asp:UpdatePanel ID="UpPnlInfoComune" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                    <ContentTemplate>
                        <div class="row">
                             <div class="col col-label"><asp:Literal ID="lbl_cap" runat="server" /></div>
                            <div class="col-no-margin">
                                <cc1:CustomTextArea ID="txt_cap" runat="server" CssClass="txt_input txt_input_small" AutoPostBack="true"
                                    CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="txt_cap_TextChanged" MaxLength="5" />
                                <uc1:AutoCompleteExtender  runat="server" ID="RapidTxtCap" TargetControlID="txt_cap" ClientIDMode="Static"
                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCapComuni"
                                    MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                    UseContextKey="true" OnClientItemSelected="aceSelectedTxtCap" BehaviorID="AutoCompleteTxtCap"
                                    OnClientPopulated="acePopulatedTxtCap">
                                </uc1:AutoCompleteExtender>
                            </div>
                            <div class="col col-label right"><asp:Literal ID="lbl_citta" runat="server" /></div>
                            <div class="col-no-margin">
                                <cc1:CustomTextArea ID="txt_citta" runat="server" CssClass="txt_input txt_input_small" AutoPostBack="true"
                                    CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="txt_citta_TextChanged" />
                                <uc1:AutoCompleteExtender  runat="server" ID="RapidTxtCitta" TargetControlID="txt_citta" 
                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaComuni"
                                    MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                    UseContextKey="true" OnClientItemSelected="aceSelectedTxtCitta" BehaviorID="AutoCompleteTxtCitta"
                                    OnClientPopulated="acePopulatedTxtCitta">
                                </uc1:AutoCompleteExtender>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-label"><asp:Literal ID="lbl_local" runat="server" /></div>
                            <div class="col-no-margin">
                                <cc1:CustomTextArea ID="txt_local" runat="server" CssClass="txt_input txt_input_small"
                                    CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="128" OnTextChanged="DataChangedHandler" />
                            </div>
                            <div class="col col-label right"><asp:Literal ID="lbl_provincia" runat="server" /></div>
                            <div class="col-no-margin">
                                <cc1:CustomTextArea ID="txt_provincia" runat="server" CssClass="txt_input txt_input_small"
                                    CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="2" OnTextChanged="DataChangedHandler" />
                            </div>
                        </div>
                        </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col col-label"><asp:Literal ID="lbl_nazione" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_nazione" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler" />
                    </div>
                    <div class="col col-label right"><asp:Literal ID="lbl_fax" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_fax" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label"><asp:Literal ID="lbl_telefono" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_telefono" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler" />
                    </div>
                    <div class="col col-label right"><asp:Literal ID="lbl_telefono2" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_telefono2" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label"><asp:Literal ID="lbl_codfisc" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_codfisc" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="16" OnTextChanged="DataChangedHandler" />
                    </div>
                    <div class="col col-label right"><asp:Literal ID="lbl_partita_iva" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_partita_iva" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" OnTextChanged="DataChangedHandler" />
                    </div>
                </div>
                <%--<div class="row">
                    <div class="col col-label"><asp:Literal ID="lbl_codice_ipa" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_codice_ipa" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" OnTextChanged="DataChangedHandler" />
                    </div>
                </div>--%>
                <div class="row">
                    <div class="col col-label"><asp:Literal ID="lbl_note" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_note" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"
                            OnTextChanged="DataChangedHandler" TextMode="MultiLine" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="pnlRuolo" runat="server">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_CodR" runat="server" /></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_CodR" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler" />
                </div>
            </div>
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_DescR" runat="server" />*</div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_DescR" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="pnlRuoliUtente" runat="server" Visible="false">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_Ruoli" runat="server" /></div>
                <div class="col col-no-margin">
                    <asp:Literal ID="lblRuoli" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="PanelListaCorrispondenti" runat="server">
            <div class="row">
                <p class="center">
                    <em>
                        <asp:Literal ID="lbl_nomeLista" runat="server" /></em>
                </p>
            </div>
            <div class="row">
                <asp:GridView ID="dg_listCorr" runat="server" Width="600" AutoGenerateColumns="False"
                    CssClass="tbl">
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <Columns>
                        <asp:BoundField DataField="CODICE" ItemStyle-Width="30%" htmlencode="false"
 />
                        <asp:BoundField DataField="DESCRIZIONE" ItemStyle-Width="70%" htmlencode="false"
/>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:PlaceHolder>
    </div>
    <!-- Popup modal confirm -->
    <script type="text/javascript">
        function ajaxConfirmModal(msg, hiddenToValorize) {
            var titleW = null;
            var input = null;
            var popupWidth = 500;
            var popupHeight = 350;
            var closeFunction = null;
            if (arguments.length > 2 && arguments[2] != null) titleW = arguments[2];
            if (arguments.length > 3 && arguments[3] != null) input = arguments[3];
            if (arguments.length > 4 && arguments[4] != null) popupWidth = arguments[4];
            if (arguments.length > 5 && arguments[5] != null) popupHeight = arguments[5];
            if (arguments.length > 6 && arguments[6] != null) closeFunction = arguments[6];
            if (closeFunction != null) { closeFunction = "$('#frame').show();" + closeFunction; } else { closeFunction = "$('#frame').show();"; }
            if (titleW == null || titleW == '') titleW = '<asp:Literal id="litConfirm" runat="server" />';

            $('#confirm_modal').empty();
            var d = $('#confirm_modal').html($('<iframe id="ifrm_confirm" frameborder="0" />'));
            d.dialog({
                open: function (event, ui) { $('.ui-dialog-titlebar-close').hide(); },
                close: function (event, ui) { if (closeFunction != null) eval(closeFunction); },
                position: { my: "center", at: "center", of: window },
                resizable: false,
                draggable: true,
                modal: true,
                show: 'puff',
                hide: 'clip',
                stack: true,
                title: titleW,
                width: popupWidth,
                height: popupHeight
            });
            $("#confirm_modal #ifrm_confirm").attr({ src: '<%=Page.ResolveClientUrl("~/Popup/confirm.aspx") %>?hidden=' + hiddenToValorize + '&msg=' + msg + '&input=' + input, width: '99%', height: '99%', marginwidth: '0', marginheight: '0', scrolling: 'auto' });

            $('#frame').hide();
        };
    </script>
    <div id="confirm_modal">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPanelButton" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnDelete" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="BtnDelete_Click" Visible="false" />
            <cc1:CustomButton ID="BtnModify" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="BtnModify_Click" Visible="false" />
            <cc1:CustomButton ID="BtnInsert" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="BtnInsert_Click" Visible="false" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="BtnClose_Click" />
            <asp:HiddenField ID="proceed_delete" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
