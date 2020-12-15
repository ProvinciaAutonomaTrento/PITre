<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="AddressBook_new.aspx.cs" Inherits="NttDataWA.Popup.AddressBook_new" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Import namespace="NttDataWA.DocsPaWR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        em {font-style: normal; font-weight: bold; color: #9D9D9D;}
        .col-label {width: 100px; overflow: hidden; padding: 5px 0 0 0;}
        div div.col-label:last-child {color: #f00;}
        .txt_input, .txt_input_disabled, .txt_textarea, .txt_textarea_disabled {width: 472px; height: 1.5em;}
        .txt_textarea, .txt_textarea_disabled {height: 3em; vertical-align: top;}
        .txt_input_disabled {background-color: #ffffff;}
        .txt_input_small {width: 168px;}
        .tbl tr.NormalRow:hover td {background-color: #fff;}
        .tbl tr.AltRow:hover td {background-color: #e1e9f0;}
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function acePopulatedUoCap(sender, e) {
            var behavior = $find('AutoCompleteUoCap');
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
        function aceSelectedUoCap(sender, e) {
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

            var searchText = $get('<%=uo_cap.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            document.getElementById("<%=uo_cap.ClientID%>").value = testo
            document.getElementById("<%=uo_citta.ClientID%>").focus();
        }

        function acePopulatedUoCitta(sender, e) {
            var behavior = $find('AutoCompleteUoCitta');
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
        function aceSelectedUoCitta(sender, e) {
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

            var searchText = $get('<%=uo_citta.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            document.getElementById("<%=uo_citta.ClientID%>").value = testo
            document.getElementById("<%=uo_cap.ClientID%>").focus();
        }
        // UTENTE

        function acePopulatedUserCap(sender, e) {
            var behavior = $find('AutoCompleteUserCap');
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
        function aceSelectedUserCap(sender, e) {
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

            var searchText = $get('<%=user_cap.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            document.getElementById("<%=user_cap.ClientID%>").value = testo
            document.getElementById("<%=user_citta.ClientID%>").focus();
        }

        function acePopulatedUserCitta(sender, e) {
            var behavior = $find('AutoCompleteUserCitta');
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
        function aceSelectedUserCitta(sender, e) {
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

            var searchText = $get('<%=user_citta.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            document.getElementById("<%=user_citta.ClientID%>").value = testo
            document.getElementById("<%=user_cap.ClientID%>").focus();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row">
        <div class="col col-label"><asp:Literal id="lbl_registro" runat="server" /></div>
        <div class="col-no-margin">
            <asp:DropDownList ID="ddl_registri" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_registri_SelectedIndexChanged" CssClass="chzn-select-deselect" width="300px" />
        </div>
    </div>
    <div class="row">
        <div class="col col-label"><asp:Literal id="lbl_tipocorr" runat="server" /></div>
        <div class="col-no-margin">
            <asp:DropDownList ID="ddl_tipoCorr" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect" width="200px" OnSelectedIndexChanged="ddl_tipoCorr_SelectedIndexChanged" />
        </div>
    </div>
    <div class="row">
        <div class="col col-label"><asp:Literal id="lbl_CodRubrica" runat="server" /><asp:label id="starRubrica" Runat="server">*</asp:label></div>
        <div class="col-no-margin">
            <cc1:CustomTextArea ID="txt_CodRubrica" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" MaxLength="128"/>
        </div>
    </div>
    <div class="row">
        <div class="col col-label"><asp:Literal id="lbl_preferredChannel" runat="server" /></div>
        <div class="col-no-margin">
            <asp:DropDownList ID="dd_canpref" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect" width="200px" OnSelectedIndexChanged="dd_canpref_SelectedIndexChanged"/>
        </div>
        <div class="col-left" style=" padding-left:20px; padding-top:5px;">
            <asp:CheckBox ID="cbInteroperanteRGS" runat="server" Visible="false" Checked="false" />
        </div>
    </div>
    <div class="row">
        <div class="col col-label"><asp:Literal id="lbl_codAmm" runat="server" /><asp:Literal id="starCodAmm" Runat="server" Visible="false">*</asp:Literal></div>
        <div class="col-no-margin">
            <cc1:CustomTextArea ID="txt_codAmm" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
        </div>
    </div>
    <div class="row">
        <div class="col col-label"><asp:Literal id="lbl_codAOO" runat="server" /><asp:Literal id="starCodAOO" Runat="server" Visible="false">*</asp:Literal></div>
        <div class="col-no-margin">
            <cc1:CustomTextArea ID="txt_codAOO" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcSingleMail" runat="server">
        <asp:UpdatePanel ID="updPanel1" runat="server" UpdateMode="Always" Visible="true">
            <ContentTemplate>
                <div class="row">
                    <div class="col col-label"><asp:Literal id="lbl_email" runat="server" /><asp:Literal id="starEmail" Runat="server" Visible="false">*</asp:Literal></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_email" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" Visible="false" />
                        <cc1:CustomTextArea ID="txtCasella" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" style="width: 400px;" />
                        <cc1:CustomImageButton runat="server" ID="imgAggiungiCasella" ImageUrl="../Images/Icons/add_version.png"
                            OnMouseOutImage="../Images/Icons/add_version.png" OnMouseOverImage="../Images/Icons/add_version_hover.png"
                            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/add_version_disabled.png"
                            OnClick="imgAggiungiCasella_Click" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcNoteMail" runat="server">
                    <div class="row">
                        <div class="col col-label"><asp:Literal id="lblNote" runat="server" /></div>
                        <div class="col-no-margin">
                            <cc1:CustomTextArea ID="txtNote" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:PlaceHolder>
    <asp:UpdatePanel ID="updPanelMail" runat="server" UpdateMode="Always" Visible="true">
        <ContentTemplate>
            <div id="divGridViewCaselle" runat="server" class="row">
                <asp:GridView ID="gvCaselle" runat="server" AutoGenerateColumns="False"
                    CellPadding="1" BorderWidth="1px" BorderColor="Gray"
                    style="overflow-y: scroll; overflow-x: hidden; max-height: 90px" cssclass="tbl">
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <Columns>
                        <asp:TemplateField HeaderText="SystemId" Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID ="lblSystemId" Text ='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).systemId %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email" ShowHeader="true" >
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="68%"></ItemStyle>
                            <ItemTemplate>
                                <asp:TextBox AutoPostBack="true" Width="310px" OnTextChanged="txtEmailCorr_TextChanged" ID="txtEmailCorr" runat="server" ToolTip='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>' Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:TemplateField HeaderText="Note E-mail" ShowHeader="true">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle" Width="28%"></ItemStyle>
                            <ItemTemplate>
                                <asp:TextBox AutoPostBack="true" Width="110px" MaxLength="20" OnTextChanged="txtNoteMailCorr_TextChanged" ID="txtNoteMailCorr" runat="server" ToolTip='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>' Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:TemplateField HeaderText="*" ShowHeader="true" >
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                            <ItemTemplate>
                                <asp:RadioButton ID="rdbPrincipale" runat="server" AutoPostBack="true" OnCheckedChanged="rdbPrincipale_ChecekdChanged" Checked='<%# TypeMailCorrEsterno(((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Principale) %>' />
                            </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:TemplateField HeaderText="" ShowHeader="true">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                            <ItemTemplate>
                                <cc1:CustomImageButton runat="server" ID="imgEliminaCasella" ImageUrl="../Images/Icons/delete.png"
                                    OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/delete.png"
                                    OnClick="imgEliminaCasella_Click" AutoPostBack="true" />
                            </ItemTemplate>
                        </asp:TemplateField> 
                    </Columns>
                </asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


    <asp:PlaceHolder id="pnlUO" runat="server">
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_descrizione" runat="server" />*</div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_descrizione" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" MaxLength="256" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_indirizzo" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_indirizzo" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <asp:UpdatePanel ID="UpPnlUoInfoComune" runat="server" UpdateMode="Conditional" ClientIDMode="static">
            <ContentTemplate>
                <div class="row">
                    <div class="col col-label"><asp:Literal id="lbl_cap" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="uo_cap" runat="server" CssClass="txt_input txt_input_small" MaxLength="5" OnTextChanged="uo_cap_TextChanged"
                            CssClassReadOnly="txt_input_disabled txt_input_small" AutoPostBack="true"  autocomplete="off" AutoCompleteType="Disabled"
                            />
                        <uc1:AutoCompleteExtender  runat="server" ID="RapidUoCap" TargetControlID="uo_cap"  
                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCapComuni"
                            MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                            UseContextKey="true" OnClientItemSelected="aceSelectedUoCap" BehaviorID="AutoCompleteUoCap"
                            OnClientPopulated="acePopulatedUoCap">
                        </uc1:AutoCompleteExtender>
                    </div>
                    <div class="col col-label right"><asp:Literal id="lbl_citta" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="uo_citta" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small"
                           autocomplete="off" AutoCompleteType="Disabled" AutoPostBack = "true" OnTextChanged="uo_citta_TextChanged"/>
                         <uc1:AutoCompleteExtender  runat="server" ID="RapidUoCitta" TargetControlID="uo_citta" 
                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaComuni"
                            MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                            UseContextKey="true" OnClientItemSelected="aceSelectedUoCitta" BehaviorID="AutoCompleteUoCitta"
                            OnClientPopulated="acePopulatedUoCitta">
                        </uc1:AutoCompleteExtender>
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label"><asp:Literal id="lbl_local" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="uo_local" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="128" />
                    </div>
                    <div class="col col-label right"><asp:Literal id="lbl_provincia" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="uo_provincia" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="2" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_nazione" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_nazione" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_fax" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_fax" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_telefono" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_telefono" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_telefono2" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_telefono2" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_codfisc" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_codfisc" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="16" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_partita_iva" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_partita_iva" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_codice_ipa" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_codice_ipa" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_note" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="uo_note" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder id="pnlRuolo" runat="server">
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_DescR" runat="server" />*</div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_DescR" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" MaxLength="256" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder id="pnlUtente" Runat="server">
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_titolo" runat="server" /></div>
            <div class="col-no-margin">
                <asp:DropDownList ID="ddl_titolo" runat="server" CssClass="chzn-select-deselect" width="200px" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_nome" runat="server" />*</div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_nome" runat="server" MaxLength="50" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_cognome" runat="server" />*</div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_cognome" runat="server" MaxLength="50" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_luogonascita" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_luogoNascita" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_dataNascita" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_dataNascita" runat="server" CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled" Columns="10" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbluser_indirizzo" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_indirizzo" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <asp:UpdatePanel ID="UpPnlUserInfoComune" runat="server" UpdateMode="Conditional" ClientIDMode="static">
            <ContentTemplate>
            <div class="row">
                <div class="col col-label"><asp:Literal id="lbluser_cap" runat="server" /></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="user_cap" runat="server" CssClass="txt_input" MaxLength="5" CssClassReadOnly="txt_input_disabled" AutoPostBack="true" OnTextChanged="user_cap_TextChanged"/>
                    <uc1:AutoCompleteExtender  runat="server" ID="RapidUserCap" TargetControlID="user_cap" ClientIDMode="Static"
                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCapComuni"
                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                        UseContextKey="true" OnClientItemSelected="aceSelectedUserCap" BehaviorID="AutoCompleteUserCap"
                        OnClientPopulated="acePopulatedUserCap">
                    </uc1:AutoCompleteExtender>
                </div>
            </div>
            <div class="row">
                <div class="col col-label"><asp:Literal id="lbluser_citta" runat="server" /></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="user_citta" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small"
                     AutoPostBack="true" OnTextChanged="user_citta_TextChanged"  />
                    <uc1:AutoCompleteExtender  runat="server" ID="RapidUserCitta" TargetControlID="user_citta" 
                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaComuni"
                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                        UseContextKey="true" OnClientItemSelected="aceSelectedUserCitta" BehaviorID="AutoCompleteUserCitta"
                        OnClientPopulated="acePopulatedUserCitta">
                    </uc1:AutoCompleteExtender>
                </div>
                <div class="col col-label right"><asp:Literal id="lbluser_provincia" runat="server" /></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="user_provincia" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="2" />
                </div>
            </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbluser_local" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_local" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="128" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbluser_nazione" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_nazione" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbluser_telefono" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_telefono" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbluser_telefono2" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_telefono2" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbluser_fax" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_fax" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbluser_codfisc" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_codfisc" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="16" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbluser_partita_iva" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_partita_iva" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label right"><asp:Literal id="lbluser_codice_ipa" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_codice_ipa" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbluser_note" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="user_note" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" />
            </div>
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
    <cc1:CustomButton ID="BtnInsert" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static"  OnClientClick="disallowOp('Content2');"
        onclick="BtnInsert_Click" />
          <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static"  OnClientClick="disallowOp('Content2');"
        onclick="BtnClose_Click" />
</asp:Content>
