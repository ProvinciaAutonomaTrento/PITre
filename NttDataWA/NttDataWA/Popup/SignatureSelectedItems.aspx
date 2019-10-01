<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignatureSelectedItems.aspx.cs"
    Inherits="NttDataWA.Popup.SignatureSelectedItems" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #container
        {
            position: fixed;
            top: 1px;
            left: 0px;
            bottom: 71px;
            right: 0px;
            overflow: auto;
            text-align: left;
            padding: 10px;
        }
        
        .message
        {
            line-height: 2em;
            width: 90%;
            min-height: 200px;
            margin: 100px 0 0 20px;
        }
        .message img
        {
            float: left;
            display: block;
            margin: 0 20px 0 0;
        }
        
        .message li
        {
            margin: 0 0 0 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    ClientIDMode="static" runat="server">
    <uc:ajaxpopup2 Id="MassiveSignatureHSM" runat="server" Url="../Popup/MassiveHSM_Signature.aspx?LF=1&objType=D"
        Width="700" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignature" runat="server" Url="../Popup/MassiveDigitalSignature.aspx?LF=1&objType=D"
        Width="650" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="MassiveDigitalSignatureApplet" runat="server" Url="../Popup/MassiveDigitalSignature_applet.aspx?LF=1&objType=D"
        Width="650" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', ''); }" />
        <uc:ajaxpopup2 Id="MassiveDigitalSignatureSocket" runat="server" Url="../Popup/MassiveDigitalSignature_Socket.aspx?LF=1&objType=D"
        Width="650" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', ''); }" />
    <div class="container">
        <asp:UpdatePanel ID="UpPnlMessage" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="plcMessage" runat="server">
                    <div class="message">
                        <asp:Image runat="server" ID="imgConfirm" />
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlReport" runat="server" CssClass="row" Visible="false">
                    <asp:GridView ID="grdReport" runat="server" Width="100%" AutoGenerateColumns="False"
                        CssClass="tbl_rounded_custom round_onlyextreme">
                        <RowStyle CssClass="NormalRow" />
                        <AlternatingRowStyle CssClass="AltRow" />
                        <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:BoundField HeaderText='<%$ localizeByText:MassiveActionLblGrdReportLibroFirma%>'
                                DataField="ObjId">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText='<%$ localizeByText:MassiveResultLblGrdReportLibroFirma%>'
                                DataField="Result">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText='<%$ localizeByText:MassiveDetailsLblGrdReportLibroFirma%>'
                                DataField="Details" HtmlEncode="false">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="SignatureSelectedItemsConfirm" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SignatureSelectedItemsConfirm_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <cc1:CustomButton ID="SignatureSelectedItemsCancel" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SignatureSelectedItemsCancel_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                Visible="false" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click"
                OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
