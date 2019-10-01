<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Prints.aspx.cs" Inherits="NttDataWA.Management.Prints"
    MasterPageFile="../MasterPages/Base.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function creatorePopulated(sender, e) {
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

            var searchText = $get('<%=txt_descUO.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_descUO.ClientID%>").focus();
            document.getElementById("<%=this.txt_descUO.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_codUO.ClientID%>").value = codice;
            document.getElementById("<%=txt_descUO.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txt_codUO.ClientID%>', '');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=doc"
        Width="600" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="visualReport_iframe" runat="server" Url="../popup/visualReport_iframe.aspx?fr=P"
        Width="400" Height="350" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="TransmissionsPrint" runat="server" 
        Url="../popup/transmissions_print_iframe.aspx" IsFullScreen="true" PermitClose="false"
        PermitScroll="true" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="PrintsTitle" runat="server"></asp:Literal></p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <!-- filters -->
                                <div class="row" id="divRegistri">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col">
                                                <asp:Label runat="server" ID="lblRegistro" Font-Bold="True"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:DropDownList runat="server" ID="ddl_registri" CssClass="chzn-select-deselect"
                                                    Width="400" OnSelectedIndexChanged="ddl_registri_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                                <!-- end divRegistri -->
                                <div class="row" id="divReports">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col">
                                                <span class="weight">
                                                    <asp:Literal runat="server" ID="LitPrintsManagementSelect"></asp:Literal>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:DropDownList ID="ddl_report" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                    OnSelectedIndexChanged="ddl_report_SelectedIndexChanged" Width="400">
                                                    <asp:ListItem Selected="True" Value="T"></asp:ListItem>
                                                    <asp:ListItem Value="E"></asp:ListItem>
                                                    <asp:ListItem Value="TR"></asp:ListItem>
                                                    <asp:ListItem Value="DR"></asp:ListItem>
                                                    <asp:ListItem Value="DG"></asp:ListItem>
                                                    <asp:ListItem Value="B"></asp:ListItem>
                                                    <asp:ListItem Value="F"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                                <asp:Panel ID="pnlInput" runat="server" Visible="False">
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:Label ID="ManagementPrintsLabel" runat="server" CssClass="weight"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <cc1:CustomTextArea ID="txtInput" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnl_trasmUO" runat="server" Visible="False">
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:Label ID="lblOggettoTrasmesso" runat="server" Font-Bold="true"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList ID="DDLOggettoTab1" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                        Width="400">
                                                        <asp:ListItem Selected="True" Value="D"></asp:ListItem>
                                                        <asp:ListItem Value="F"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <asp:Label ID="lblDataTrasmissione" runat="server" CssClass="weight"></asp:Label>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row nowrap">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_dataTrasm" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                        OnSelectedIndexChanged="ddl_dataTrasm_SelectedIndexChanged" Width="150">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="LitSearchDocumentFrom" runat="server" Visible="false"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="TxtDateFrom" runat="server" ClientIDMode="Static" CssClass="txt_textdata datepicker"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="LitSearchDocumentTo" runat="server" Visible="false"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="TxtDateTo" runat="server" ClientIDMode="Static" CssClass="txt_textdata datepicker"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="AddFilterLblReasonTransmission" runat="server"></asp:Literal></p>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList ID="ddl_ragioni" runat="server" CssClass="chzn-select-deselect"
                                                        Width="400">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </asp:Panel>
                                <!--DOCUMENTI REGISTRO-->
                                <asp:Panel ID="pnl_DocumentiRegistro" runat="server" Visible="False">
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="LblAddFilterNumProtocol" runat="server"></asp:Literal>
                                                        </span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_numProt_E" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                        Width="150px" OnSelectedIndexChanged="ddl_numProt_E_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Selected="True" Value="1"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lblDAnumprot_E" runat="server"></asp:Label>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initNumProt_E" runat="server" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lblAnumprot_E" runat="server"></asp:Label>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineNumProt_E" runat="server" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="LblAddFilterDateProtocol" runat="server"></asp:Literal></span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_dataProt_E" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                        Width="150px" OnSelectedIndexChanged="ddl_dataProt_E_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_initdataProt_E" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initDataProt_E" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_finedataProt_E" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineDataProt_E" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                    <div class="row">
                                        <!--tabella mittente/destinatario -->
                                        <fieldset class="basic">
                                            <asp:UpdatePanel ID="upPnlUO" runat="server" UpdateMode="Conditional" ClientIdMode="static">
                                                <ContentTemplate>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal ID="litUO" runat="server"></asp:Literal></span></p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <input id="hd_systemIdUo" runat="server" name="hd_systemIdUO" size="1" type="hidden" />
                                                            <asp:CheckBox ID="chk_uo" runat="server" CssClass="clickableLeftN" />
                                                            <cc1:CustomImageButton ID="ImgProprietarioAddressBook" runat="server" CssClass="clickableLeft"
                                                                ImageUrl="../Images/Icons/address_book.png" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                OnClick="ImgProprietarioAddressBook_Click" OnMouseOutImage="../Images/Icons/address_book.png"
                                                                OnMouseOverImage="../Images/Icons/address_book_hover.png" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="txt_codUO" runat="server" AutoPostBack="True" CssClass="txt_addressBookLeft" OnTextChanged="TxtCode_OnTextChanged"
                                                                CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="txt_descUO" runat="server" CssClass="txt_addressBookLeft"
                                                                    CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                    </div>
                                                      <uc1:AutoCompleteExtender runat="server" ID="RapidCreatore" TargetControlID="txt_descUO"
                                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                        MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                        UseContextKey="true" OnClientItemSelected="creatoreSelected" BehaviorID="AutoCompleteExIngressoCreatore"
                                                                        OnClientPopulated="creatorePopulated" Enabled="false">
                                                                    </uc1:AutoCompleteExtender>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </fieldset>
                                    </div>
                                    <div class="row">
                                        <!--tabella mittente/destinatario -->
                                        <fieldset class="basic">
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="DocumentLitTypeDocument" runat="server"></asp:Literal>
                                                        </span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-full">
                                                    <asp:DropDownList ID="ddl_tipoAttoDR" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                        Width="97%">
                                                        <asp:ListItem Text=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </asp:Panel>
                                <!--DOCUMENTI GRIGI-->
                                <asp:Panel ID="pnl_DocumentiGrigi" runat="server" Visible="False">
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:Label ID="LblAddDocNumDoc" runat="server" CssClass="weight"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_idDoc_E" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                        Width="150px" OnSelectedIndexChanged="ddl_idDoc_E_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Selected="True" Value="1"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_initIdDoc_E" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initIdDoc_E" runat="server" ClientIDMode="Static" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_fineIdDoc_E" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineIdDoc_E" runat="server" ClientIDMode="Static" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <!-- Fine Da searchDocument-->
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <asp:Label ID="LblAddDocDtaDoc" runat="server" CssClass="weight"></asp:Label></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_dataCreazioneG_E" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                        Width="150px" OnSelectedIndexChanged="ddl_dataCreazioneG_E_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_initDataCreazioneG_E" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initDataCreazioneG_E" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_fineDataCreazioneG_E" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineDataCreazioneG_E" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Label ID="DocumentLitTypeDocument1" runat="server"></asp:Label></span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-full">
                                                    <asp:DropDownList ID="ddl_tipoAttoDG" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                        OnSelectedIndexChanged="ddl_tipoAttoDG_SelectedIndexChanged" Width="97%">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </asp:Panel>
                                <!--STAMPA BUSTE-->
                                <asp:Panel ID="pnl_StampaBuste" runat="server" Visible="False">
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <asp:Label ID="LitAnnoSearchProject" runat="server" CssClass="weight"></asp:Label>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <cc1:CustomTextArea ID="txt_Anno_B" runat="server" MaxLength="4" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <asp:Label ID="LblAddDocNumDoc1" runat="server" CssClass="weight"></asp:Label>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_numProt_B" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                        OnSelectedIndexChanged="ddl_numProt_B_SelectedIndexChanged" Width="140px">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Selected="True" Value="1"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lblDAnumprot_B" runat="server"></asp:Label>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initNumProt_B" runat="server" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lblAnumprot_B" runat="server"></asp:Label>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineNumProt_B" runat="server" CssClass="txt_textdata"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <asp:Label ID="LblAddDocDtaDoc1" runat="server" CssClass="weight">Da</asp:Label>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_dataProt_B" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                        OnSelectedIndexChanged="ddl_dataProt_B_SelectedIndexChanged" Width="140px">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Value="1"></asp:ListItem>
                                                        <asp:ListItem Value="2"></asp:ListItem>
                                                        <asp:ListItem Value="3"></asp:ListItem>
                                                        <asp:ListItem Value="4"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_initdataProt_B" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_initDataProt_B" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_finedataProt_B" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_fineDataProt_B" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </asp:Panel>
                                <!-- REPORT AVANZATI -->
                                <asp:Panel ID="pnl_reportAvanzati" runat="server" Visible="false">
                                    <div class="row">
                                        <fieldset>
                                            <div class="row" id="td_rep_av_ruolo" runat="server">
                                                <div class="col">
                                                    <asp:Label ID="TypeRole" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList ID="ddl_rep_av_ruolo" runat="server" CssClass="chzn-select-deselect">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                    <div class="row">
                                        <fieldset>
                                            <div class="row">
                                                <div class="col" id="td_rep_av_rag_trasm" runat="server">
                                                    <asp:Literal ID="TransmissionLitReasonExtended" runat="server"></asp:Literal>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:DropDownList ID="ddl_rep_av_rag_trasm" runat="server" CssClass="chzn-select-deselect">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col" id="td_rep_av_data" runat="server">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal ID="ReferDate" runat="server"></asp:Literal></span></p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col2">
                                                    <asp:DropDownList ID="ddl_rep_av_data" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect"
                                                        OnSelectedIndexChanged="ddl_rep_av_data_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"></asp:ListItem>
                                                        <asp:ListItem Selected="True" Value="1"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_rep_av_da" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_rep_av_initData" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <asp:Literal ID="lit_rep_av_a" runat="server"></asp:Literal>
                                                </div>
                                                <div class="col4">
                                                    <cc1:CustomTextArea ID="txt_rep_av_fineData" runat="server" ClientIDMode="Static"
                                                        CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled">
                                                    </cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                    <!-- Fine Da searchDocument-->
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="UpdPanelPrint" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:HiddenField ID="rowIndex" runat="server" ClientIDMode="Static" />
                        <div class="row">
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnPrint" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnPrint_Click" Visible="True" Enabled="True" />
            <asp:HiddenField runat="server" ID="HiddenVerifyBox" ClientIDMode="Static" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
