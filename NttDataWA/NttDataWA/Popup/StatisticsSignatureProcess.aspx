<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatisticsSignatureProcess.aspx.cs"
    Inherits="NttDataWA.Popup.StatisticsSignatureProcess" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tbl td
        {
            cursor: default;
        }
        .tbl th
        {
            font-weight: bold;
        }
        .row3
        {
            clear: both;
            min-height: 25px;
            margin: 0 0 10px 0;
            text-align: left;
            vertical-align: top;
        }
        .col-marginSx2
        {
            float: left;
            margin: 0px 5px 0px 5px;
            text-align: left;
        }
        
        .col-marginSx2 p
        {
            margin: 0px;
            padding: 0px;
            font-weight: normal;
            margin-top: 4px;
        }
        .colHalf12
        {
            float: left;
            margin-right: 5px;
            text-align: left;
            width: 65%;
        }
        
        .colHalf13
        {
            float: left;
            margin: 0px 3px 0px 3px;
            text-align: left;
            width: 30%;
        }
        
        .colHalf
        {
            float: left;
            margin: 0px;
            text-align: left;
            width: 2%;
        }
    </style>
    <script type="text/javascript">
        function acePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoProp');
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

            var searchText = $get('<%=txtDescrizioneProponente.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneProponente.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneProponente.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceProponente.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneProponente.ClientID%>").value = descrizione;

            document.getElementById("<%=btnProponente.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:UpdatePanel runat="server" ID="UpPnlFilter" UpdateMode="Conditional">
            <ContentTemplate>
                <fieldset class="basic">
                    <div id="filter" style="margin-bottom: 20px; margin-top: 20px;">
                        <asp:Panel ID="pnlFilter" runat="server">
                            <div class="row3">
                                <%-- DATA DI AVVIO --%>
                                <div class="col2" style="margin-right: 30px">
                                    <div class="col2">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlStartDate"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                    <div class="col2">
                                        <asp:DropDownList ID="ddl_StartDate" runat="server" AutoPostBack="true" Width="140px"
                                            OnSelectedIndexChanged="ddl_StartDate_SelectedIndexChanged">
                                            <asp:ListItem Value="0"></asp:ListItem>
                                            <asp:ListItem Value="1"></asp:ListItem>
                                            <asp:ListItem Value="2"></asp:ListItem>
                                            <asp:ListItem Value="3"></asp:ListItem>
                                            <asp:ListItem Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlDaStartDate"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_initStartDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlAStartDate"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_finedataStartDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                                </div>
                                <div class="col2">
                                    <div class="col2">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlNotesStartup"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txtNotesSturtup" runat="server" Width="300px" CssClass="txt_addressBookLeft"
                                            CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                    </div>
                                </div>
                            </div>
                            <%-- ************** PROPONENTE******************** --%>
                            <asp:UpdatePanel ID="UpdPnlProponente" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row3" style="width: 72%">
                                        <div class="colHalf13">
                                            <span class="weight">
                                                <asp:Literal ID="ltlProponente" runat="server" /></span>
                                            <cc1:CustomTextArea ID="txtCodiceProponente" runat="server" CssClass="txt_addressBookLeft"
                                                AutoPostBack="true" OnTextChanged="TxtCode_OnTextChanged" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                AutoComplete="off" onchange="disallowOp('ContentPlaceHolderContent');" Width="150px">
                                            </cc1:CustomTextArea>
                                        </div>
                                        <div class="colHalf12">
                                            <asp:HiddenField ID="idProponente" runat="server" />
                                            <cc1:CustomTextArea ID="txtDescrizioneProponente" runat="server" CssClass="txt_projectRight defaultAction"
                                                CssClassReadOnly="txt_ProjectRight_disabled">
                                            </cc1:CustomTextArea>
                                        </div>
                                        <div class="colHalf">
                                            <cc1:CustomImageButton runat="server" ID="ImgAddressBookProponente" ImageUrl="../Images/Icons/address_book.png"
                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                OnClick="ImgAddressBookProponente_Click" />
                                        </div>
                                        <asp:Button ID="btnProponente" runat="server" Text="vai" Style="display: none;" />
                                        <div class="row">
                                            <div class="col-right">
                                                <asp:CheckBox ID="chkProponenteExtendHistoricized" runat="server" Checked="true"
                                                    AutoPostBack="true" OnCheckedChanged="chkProponenteExtendHistoricized_Click" />
                                            </div>
                                        </div>
                                        <uc1:AutoCompleteExtender runat="server" ID="RapidProponente" TargetControlID="txtDescrizioneProponente"
                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                            MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                            UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoProp"
                                            OnClientPopulated="acePopulated">
                                        </uc1:AutoCompleteExtender>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row3">
                                <%-- DATA CONCLUSIONE --%>
                                <div class="col2" style="margin-right: 100px">
                                    <div class="col2">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlCompletitionDate"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                    <div class="col2">
                                        <asp:DropDownList ID="ddl_CompletitionDate" runat="server" AutoPostBack="true" Width="140px"
                                            OnSelectedIndexChanged="ddl_CompletitionDate_SelectedIndexChanged">
                                            <asp:ListItem Value="0"></asp:ListItem>
                                            <asp:ListItem Value="1"></asp:ListItem>
                                            <asp:ListItem Value="2"></asp:ListItem>
                                            <asp:ListItem Value="3"></asp:ListItem>
                                            <asp:ListItem Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlDaCompletitionDate"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_initCompletitionDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlACompletitionDate"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_finedataCompletitionDate" runat="server" Width="80px"
                                            CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                                </div>
                            </div>
                            <div class="row3">
                                <%-- DATA INTERRUZIONE --%>
                                <div class="col2" style="margin-right: 20px">
                                    <div class="col2">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlInterruptionDate"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                    <div class="col2">
                                        <asp:DropDownList ID="ddl_InterruptionDate" runat="server" AutoPostBack="true" Width="140px"
                                            OnSelectedIndexChanged="ddl_InterruptionDate_SelectedIndexChanged">
                                            <asp:ListItem Value="0"></asp:ListItem>
                                            <asp:ListItem Value="1"></asp:ListItem>
                                            <asp:ListItem Value="2"></asp:ListItem>
                                            <asp:ListItem Value="3"></asp:ListItem>
                                            <asp:ListItem Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlDaInterruptionDate"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_initInterruptionDate" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                                    <div class="col2">
                                        <asp:Literal runat="server" ID="LtlAInterruptionDate"></asp:Literal>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_finedataInterruptionDate" runat="server" Width="80px"
                                            CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                                </div>
                                <div class="col2">
                                    <div class="col2">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlNotesInterruption"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txtNotesInterruption" runat="server" Width="200px" CssClass="txt_addressBookLeft"
                                            CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                    </div>
                                </div>
                            </div>
                            <div class="row3">
                                <%-- STATO --%>
                                <div class="col2" style="margin-right: 100px">
                                    <div class="col-marginSx2">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="LtlState"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                    <div class="col">
                                        <asp:CheckBoxList ID="cbxState" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="IN_EXEC" Selected="True" runat="server" id="opIN_EXEC"></asp:ListItem>
                                            <asp:ListItem Value="STOPPED" Selected="True" runat="server" id="opSTOPPED"></asp:ListItem>
                                            <asp:ListItem Value="CLOSED" Selected="True" runat="server" id="opCLOSED"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpPnlGridView" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="gridViewResult" runat="server" Width="100%" AllowPaging="false"
                    PageSize="10" AutoGenerateColumns="false" CssClass="tbl" HorizontalAlign="Center"
                    ShowHeader="true" ShowHeaderWhenEmpty="true" OnRowDataBound="gridViewResult_RowDataBound"
                    OnPreRender="gridViewResult_PreRender">
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" />
                    <Columns>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="systemIdIstanzaProcesso" Text='<%# Bind("idIstanzaProcesso") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessStartDate%>'>
                            <ItemTemplate>
                                <%# NttDataWA.Utils.dateformat.dateLength(Eval("dataAttivazione").ToString())%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessNotesStartup%>'>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="NotesStartup" Text='<%# Bind("NoteDiAvvio") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessProponent%>'
                            ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="Proponent" Text='<%#this.GetProponent((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessCompletitionDate%>'>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="CompletitionDate" Text='<%#this.GetCompletitionDate((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessInterruptionDate%>'>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="InterruptionDate" Text='<%#this.GetInterruptionDate((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessNotesInterruption%>'>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="NotesInterruption" Text='<%# Bind("MotivoRespingimento") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:StatisticsSignatureProcessState%>'>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="State" Text='<%#this.GetState((NttDataWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:PlaceHolder ID="plcNavigator" runat="server" />
                <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="StatisticsSignatureProcessAddFilter" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('Content2');" CssClassDisabled="btnDisable" OnMouseOver="btnHover"
                ClientIDMode="Static" OnClick="StatisticsSignatureProcessAddFilter_Click" />
            <cc1:CustomButton ID="StatisticsSignatureProcessRemoveFilter" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('Content2');" CssClassDisabled="btnDisable" OnMouseOver="btnHover"
                ClientIDMode="Static" OnClick="StatisticsSignatureProcessRemoveFilter_Click"
                Enabled="false" />
            <cc1:CustomButton ID="StatisticsSignatureProcessClose" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('Content2');" CssClassDisabled="btnDisable" OnMouseOver="btnHover"
                ClientIDMode="Static" OnClick="StatisticsSignatureProcessClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
