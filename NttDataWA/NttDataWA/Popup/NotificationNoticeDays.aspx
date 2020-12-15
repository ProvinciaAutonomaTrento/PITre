<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotificationNoticeDays.aspx.cs" Inherits="NttDataWA.Popup.NotificationNoticeDays" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function changeBgImage(image, id) {
            var element = document.getElementById(id);
            element.style.backgroundImage = "url(" + image + ")";
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" ClientIDMode="static"  runat="server">
    <div style="width: 99%">
        <uc:messager id="NotificationNoticeDaysLt" runat="server" />    
    </div>
    <div style="text-align: center; padding-top: 20px; width: 95%">

          <asp:Panel ID="pnlVisibleRemoveNotifyMessage" runat="server" >
        <asp:Literal runat="server" ID="ltlDateNoticeDays"></asp:Literal>
        <div class="col4" style="position: absolute; left: 50%; margin: 15px 0 0 -50px;">
                <cc1:CustomTextArea ID="txt_dateNoticeDays" runat="server" Width="80px" CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
        </div>
           </asp:Panel>
    </div>
    <div style="position: absolute; left: 50%; margin: 70px 0 0 -235px;">
            <asp:Panel ID="pnlVisibleRemoveNotifyCheckBox" runat="server">
        <asp:CheckBox ID="cbRemovePendingTransmission" runat="server" Checked="true" />
            </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="float: left;">
           <asp:Panel ID="pnlVisibleRemoveNotifyButtom" runat="server">
                <cc1:CustomButton ID="NotificationNoticeDaysBtnRemove" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" ClientIDMode="Static" OnClick="NotificationNoticeDaysBtnRemove_Click" />
            </asp:Panel>
            </div>
            <div style="float: left;">
            <cc1:CustomButton ID="NotificationNoticeDaysBtnCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="NotificationNoticeDaysBtnCancel_Click"/>
            </div>
            <script type="text/javascript">
                $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
                $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

