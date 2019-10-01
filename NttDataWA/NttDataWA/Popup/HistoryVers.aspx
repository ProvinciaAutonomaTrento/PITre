<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="HistoryVers.aspx.cs" Inherits="NttDataWA.Popup.HistoryVers" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container" style="height: 100%">
        <asp:Panel runat="server" ID="PnlHistoryVersStatus">
            <fieldset>
                <div>
                    <p>
                        <span class="weight">
                            <asp:Label runat="server" ID="HistoryVersLblCaption"></asp:Label>
                        </span>
                    </p>
                    <asp:Label runat="server" ID="HistoryVersLblStatus"></asp:Label>
                </div>
            </fieldset>
        </asp:Panel>     
        <asp:UpdatePanel runat="server" ID="UpPnlHistoryVersXml" UpdateMode="Conditional" style="height: 70%; padding-top: 10px;">
        <ContentTemplate>
            <asp:PlaceHolder runat="server" ID="PlcHistoryVersXml" Visible="false">
                <div>
                    <span class="weight">
                        <asp:Label runat="server" ID="HistoryVersLblRecover"></asp:Label>
                        <asp:Label runat="server" ID="HistoryVersLblReport"></asp:Label>
                    </span>
                </div>
                <div class="contenteViewer" style=" width:98%; height:98%; margin:5px;">
                    <iframe width="100%"  height="100%" frameborder="0" marginheight="0" marginwidth="0" id="frame"
                            runat="server" clientidmode="Static" style="z-index: 0;" ></iframe>
                </div>
            </asp:PlaceHolder>
          </ContentTemplate>
         </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpPnlButtons" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
                <cc1:CustomButton ID="HistoryVersBtnRecupero" runat="server" CssClass="btnEnable" ClientIDMode="Static"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="HistoryVersBtnRecupero_Click" OnClientClick="disallowOp('Content2');" />
                <cc1:CustomButton ID="HistoryVersBtnRapporto" runat="server" CssClass="btnEnable" ClientIDMode="Static"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="HistoryVersBtnRapporto_Click" OnClientClick="disallowOp('Content2');" />
                <cc1:CustomButton ID="HistoryVersBtnChiudi" runat="server" CssClass="btnEnable" ClientIDMode="Static"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="HistoryVersBtnChiudi_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>

