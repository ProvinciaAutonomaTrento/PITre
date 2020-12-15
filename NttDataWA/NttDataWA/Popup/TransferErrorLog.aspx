<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="TransferErrorLog.aspx.cs" Inherits="NttDataWA.Popup.TransferErrorLog" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="selectKeyword">
        <fieldset>
            <asp:UpdatePanel ID="UpdPnlListLogTransfer" runat="server" UpdateMode="Conditional"
                ClientIDMode="static">
                <contenttemplate>
                    <div class="row">
                        <div class="col">
                            <span class="weight">
                                <asp:Label ID="SelectLogTransferLbl" runat="server"></asp:Label></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-left-no-margin-key">
                               <asp:ListBox ID="LstLogTransfer" runat="server" SelectionMode="Multiple" Height="200px"
                                    CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"></asp:ListBox>
                        </div>
                    </div>
                </contenttemplate>
            </asp:UpdatePanel>
        </fieldset>
        <fieldset>
            <asp:UpdatePanel ID="UpdPnlListLogTransferPolicy" runat="server" UpdateMode="Conditional"
                ClientIDMode="static">
                <contenttemplate>
                    <div class="row">
                        <div class="col">
                            <span class="weight">
                                <asp:Label ID="SelectTransferPolicyLbl" runat="server"></asp:Label></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-left-no-margin-key">
                                <asp:ListBox ID="LstTransferPolicy" runat="server" SelectionMode="Multiple" Height="200px"
                                    CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"></asp:ListBox>
                        </div>
                    </div>
                </contenttemplate>
            </asp:UpdatePanel>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left">
        <nttdl:custombutton id="BtnChiudi" runat="server" cssclass="btnEnable"
            cssclassdisabled="btnDisable" onmouseover="btnHover" OnClick="BtnChiudi_click" />
    </div>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>