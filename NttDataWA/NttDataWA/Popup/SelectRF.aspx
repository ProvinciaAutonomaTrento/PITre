<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="SelectRF.aspx.cs" Inherits="NttDataWA.Popup.selectRF" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="row">
        <fieldset class="fieldRf">
            <div class="col">
                <div class="container">
                    <p>
                        <span class="weight">
                            <asp:Label ID="lbl_doc_rf" runat="server" Visible="True"></asp:Label>
                        </span>
                    </p>
                    <p>
                        <asp:UpdatePanel runat="server" ID="UpSelectPnl">
                            <ContentTemplate>
                                <asp:DropDownList ID="ddl_regRF" runat="server" Visible="true" CssClass="chzn-select-deselect"
                                    Width="95%" AutoPostBack="true" OnSelectedIndexChanged="Ddl_regRF_SelectIndexChange">
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </p>
                </div>
            </div>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
  <div style="float: left">
    <asp:UpdatePanel runat="server" ID="UpPnlButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="SelectRFBtnOk" runat="server" Text="OK" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SelectRFBtnOk_Click"
                OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
    <div style="float: left">
        <cc1:CustomButton ID="SelectRFBtnChiudi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" OnClick="SelectRFBtnChiudi_Click" />
    </div>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
