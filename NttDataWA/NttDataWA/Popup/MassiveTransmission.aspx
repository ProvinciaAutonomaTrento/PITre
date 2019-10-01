<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="MassiveTransmission.aspx.cs" Inherits="NttDataWA.Popup.MassiveTransmission" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        
        .pnl_grid 
        {
            height: 150px;
            overflow-y: auto;
            overflow-x: hidden;
        }
    </style>
    <script type="text/javascript">
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

            var searchText = $get('<%=TxtDescriptionRecipient.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionRecipient.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionRecipient.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeRecipientTransmission.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionRecipient.ClientID%>").value = descrizione;

            __doPostBack('<%=TxtCodeRecipientTransmission.ClientID%>', '');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <uc:ajaxpopup Id="TransmitNewOwner" runat="server" Url="../popup/Transmission_saveNewOwner.aspx"
        Width="500" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
<asp:UpdatePanel ID="UpPnlGeneral" runat="server" class="container" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:UpdatePanel ID="upRapidTransmission" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:PlaceHolder ID="plcRapidTransmission" runat="server">
                    <fieldset class="basic2">
                        <legend><asp:Literal ID="litRapidTransmission" runat="server" /></legend>
                        <asp:UpdatePanel ID="upTemplates" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="colonnasx">
                                        <asp:Literal ID="litModel" runat="server" />
                                    </div>
                                    <div class="colonnadx">
                                        <asp:DropDownList ID="ddlTemplates" runat="server" Width="344px" CssClass="chzn-select-deselect"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlTemplates_SelectedIndexChanged" onchange="disallowOp('up');" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="upSimpleTransmission" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:PlaceHolder ID="plcSimpleTransmission" runat="server">
                    <fieldset class="basic2">
                        <legend><asp:Literal ID="litSimpleTransmission" runat="server" /></legend>
                        <asp:UpdatePanel ID="UpPnlSimpleTransmission" runat="server">
                            <ContentTemplate>
                                <div class="boxPopupSx">
                                    <div class="row">
                                        <div class="colonnasx">
                                            <strong><asp:Literal ID="litReason" runat="server" /></strong>
                                        </div>
                                        <div class="colonnadx">
                                            <asp:DropDownList ID="ddlReasons" runat="server" CssClass="chzn-select-deselect" Width="224px"
                                                AutoPostBack="True" OnSelectedIndexChanged="ddlReasons_SelectedIndexChanged"  onchange="disallowOp('up');">
                                                <asp:ListItem Selected="True" Text="" Value="-1"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="colonnasx">
                                            <strong><asp:Literal ID="litUser" runat="server" /></strong>
                                        </div>
                                        <div class="colonnadx">
                                            <asp:Literal ID="litUserInSession" runat="server" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="colonnasx">
                                            <strong><asp:Literal ID="litRole" runat="server" /></strong>
                                        </div>
                                        <div class="colonnadx">
                                            <asp:Literal ID="litRoleInSession" runat="server" />
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col">
                                            <p><span class="weight"><asp:Literal ID="TransmissionLitRecipient" runat="server" /></span></p>
                                        </div>
                                        <div class="col-right-no-margin">
                                            <asp:UpdatePanel runat="server" ID="UpPnlIconAddress" UpdateMode="Conditional" ClientIDMode="Static">
                                                <ContentTemplate>
                                                    <asp:CheckBox ID="cbLeaseRights" runat="server" />
                                                    <cc1:CustomImageButton ID="dtEntryDocImgAddressBookUser" ImageUrl="../Images/Icons/address_book.png"
                                                        runat="server" OnMouseOverImage="../Images/Icons/address_book_hover.png" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                        OnMouseOutImage="../Images/Icons/address_book.png" CssClass="clickable" OnClick="dtEntryDocImgAddressBookUser_Click"
                                                        Enabled="false"
                                                        />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <asp:UpdatePanel runat="server" ID="UpPnlRecipient" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <asp:HiddenField ID="IdRecipient" runat="server" />
                                                <div class="colHalf">
                                                    <cc1:CustomTextArea ID="TxtCodeRecipientTransmission" runat="server" CssClass="txt_addressBookLeft"
                                                        CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                        autocomplete="off" AutoPostBack="true" Enabled="false" onchange="disallowOp('up');"></cc1:CustomTextArea>
                                                </div>
                                                <div class="colHalf2">
                                                    <div class="colHalf3">
                                                        <cc1:CustomTextArea ID="TxtDescriptionRecipient" runat="server" CssClass="txt_addressBookRight"
                                                            CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" Enabled="false"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                 <uc1:AutoCompleteExtender runat="server" ID="RapidRecipient" TargetControlID="TxtDescriptionRecipient"
                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                    UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                                    OnClientPopulated="acePopulated">
                                                </uc1:AutoCompleteExtender>
                                                <asp:Button ID="btnRecipient" runat="server" Text="vai" CssClass="hidden" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                                <asp:UpdatePanel ID="UpPnlNote" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="boxPopupDx">
                                            <div class="colonnasx">
                                                <strong><asp:Literal ID="litNote" runat="server" /></strong>
                                            </div>
                                            <div class="colonnadx">
                                                <cc1:CustomTextArea ID="txtNotes" runat="server" TextMode="MultiLine" Height="70px" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="upDestinatari" runat="server" class="pnl_grid">
            <contenttemplate>
                <asp:GridView ID="dgDestinatari" runat="server" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                    AllowPaging="false" AutoGenerateColumns="false" ShowHeader="true" ShowHeaderWhenEmpty="true"
                    OnRowEditing="dgDestinatari_OnRowEditing" OnRowUpdating="dgDestinatari_OnRowUpdating" OnRowCancelingEdit="dgDestinatari_OnRowCancelingEdit"
                    OnRowDeleting="dgDestinatari_OnRowDeleting" OnRowCommand="dgDestinatari_OnRowCommand"
                >
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hfId" runat="server" Value="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Id %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblReceiverDescription" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).RecivierDescription %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="lblReason" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Reason %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="lblType" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).ExtendedType %>" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlType" runat="server" DataSource="<%# this.GetDataSourceForDDLType((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem) %>"
                                    CssClass="chzn-select-deselect" DataTextField="Text" DataValueField="Value" OnPreRender="ddlType_PreRender">
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <cc1:CustomTextArea ID="txtNote" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Note %>"></cc1:CustomTextArea>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblNote" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Note %>"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="lblDate" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).ExpirationDate %>" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <cc1:CustomTextArea ID="txtDate" CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).ExpirationDate %>" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="lblHidePrev" runat="server" Text="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).HidePreviousVersionsString %>" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkHideVers" CssClass="testo_grigio" runat="server" Checked="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).HidePreviousVersions %>" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Literal ID="ltlUsers" runat="server" Text="<%# this.GetUsers((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem) %>"></asp:Literal>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkSelectDeselectAll" Text='<%# this.GetLabel("MassiveTransmissionGridSelectDeselectAll") %>' AutoPostBack="true"
                                    runat="server" OnCheckedChanged="chkSelectDeselectAll_CheckedChanged" />
                                <asp:CheckBoxList ID="cblUsers" TextAlign="Right" runat="server" OnPreRender="cblUsers_PreRender"
                                    DataSource="<%# this.GetUsersForModifyMod((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem) %>" />
                                <br />
                                <asp:Literal ID="ltlNoEditable" runat="server" Text='<%# this.GetLabel("MassiveTransmissionGridNoEditable") %>' Visible="false" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField buttontype="Link" showeditbutton="true" />
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbDelete" runat="server" CommandName="Delete" CommandArgument="<%# ((NttDataWA.Utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Id  %>" Text='<%# this.GetLabel("MassiveTransmissionGridBtnDelete") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbNewOwner" runat="server" CommandName="NewOwner" Text='<%# this.GetLabel("MassiveTransmissionGridBtnNewOwner") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </contenttemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlReport" runat="server" CssClass="row" Visible="false">
                    <asp:GridView id="grdReport" runat="server" Width="100%" AutoGenerateColumns="False" CssClass="tbl_rounded_custom round_onlyextreme">         
                        <RowStyle CssClass="NormalRow" />
                        <AlternatingRowStyle CssClass="AltRow" />
                        <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:BoundField HeaderText='<%$ localizeByText:MassiveActionLblGrdReport%>' DataField="ObjId">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Esito" DataField="Result">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Dettagli" DataField="Details" HtmlEncode="false">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            </Columns>
                    </asp:GridView>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConfirm_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click" OnClientClick="disallowOp('Content2');" />

            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
