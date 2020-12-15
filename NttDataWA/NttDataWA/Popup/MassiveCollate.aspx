<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="MassiveCollate.aspx.cs" Inherits="NttDataWA.Popup.MassiveCollate" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
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

            var searchText = $get('<%=txt_DescFascicolo.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_DescFascicolo.ClientID%>").focus();
            document.getElementById("<%=this.txt_DescFascicolo.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_CodFascicolo.ClientID%>").value = codice;
            document.getElementById("<%=txt_DescFascicolo.ClientID%>").value = descrizione;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
<div class="container">
    <asp:UpdatePanel ID="UpPnlMessage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcMessage" runat="server" Visible="false">
                <div class="row">
                    <p><asp:Literal ID="litMessage" runat="server" /></p>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel runat="server" ID="UpCodFasc" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcProject" runat="server">
                <div class="row">
                    <div class="col">
                        <p>
                            <span class="weight">
                                <asp:Literal runat="server" ID="LtlCodFascGenProc"></asp:Literal>
                            </span>
                        </p>
                    </div>
                    <div class="col-right-no-margin">
                        <cc1:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                            runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                            OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                            OnClientClick="return parent.ajaxModalPopupOpenTitolarioMassive();" CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png" />
                        <cc1:CustomImageButton ID="DocumentImgSearchProjects" ImageUrl="../Images/Icons/search_projects.png"
                            runat="server" OnMouseOverImage="../Images/Icons/search_projects_hover.png" OnMouseOutImage="../Images/Icons/search_projects.png"
                            ImageUrlDisabled="../Images/Icons/search_projects_disabled.png" alt="Titolario"
                            OnClientClick="return parent.ajaxModalPopupSearchProjectMassive();" CssClass="clickableLeftN" />
                    </div>
                </div>
                <div class="row">
                    <asp:HiddenField ID="IdProject" runat="server" />
                    <div class="colHalf">
                        <cc1:CustomTextArea ID="txt_CodFascicolo" runat="server" CssClass="txt_addressBookLeft" onchange="disallowOp('Content2');"
                            OnTextChanged="txt_CodFascicolo_OnTextChanged" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled">
                        </cc1:CustomTextArea>
                    </div>
                    <div class="colHalf2">
                        <div class="colHalf3">
                            <cc1:CustomTextArea ID="txt_DescFascicolo" runat="server" ClientIDMode="Static"
                                CssClass="txt_addressBookRight" CssClassReadOnly="txt_addressBookRight_disabled">
                            </cc1:CustomTextArea>
                        </div>
                    </div>
                   <uc1:AutoCompleteExtender runat="server" ID="RapidSenderDescriptionProject" TargetControlID="txt_DescFascicolo"
                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListDescriptionProject"
                        MinimumPrefixLength="6" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                        UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                        OnClientPopulated="acePopulatedCod">
                    </uc1:AutoCompleteExtender>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
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
                        <asp:BoundField HeaderText="Dettagli" DataField="Details">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
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
            <asp:HiddenField  ID="HiddenPublicFolder" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
