<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="Consolidation.aspx.cs" Inherits="NttDataWA.Popup.Consolidation" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="row">
        <div class="col">
            <div class="container">
                <div id="consolidation">
                    <uc:messager ID="messager" runat="server" />
                    <asp:UpdatePanel runat="server" ID="UpTypeProtocol" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:PlaceHolder ID="PlcBothConsolidation" runat="server" Visible="false">
                                <div class="row">
                                    <fieldset>
                                        <table cellspacing="2" cellpadding="2" border="0" width="100%">
                                            <tr>
                                                <td align="center" valign="top">
                                                    <asp:RadioButton ID="Step1" runat="server" GroupName="Step" OnCheckedChanged="Step1_CheckedChanged"
                                                        AutoPostBack="true" />
                                                </td>
                                                <td valign="top">
                                                    <p>
                                                        <asp:Label ID="ConsolidationLblTitleStep1" runat="server"></asp:Label></p>
                                                    <asp:Label ID="ConsolidationLblStep1" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </div>
                                <fieldset>
                                    <table cellspacing="2" cellpadding="2" border="0" width="100%">
                                        <tr>
                                            <td align="center" valign="top">
                                                <asp:RadioButton ID="Step2" runat="server" GroupName="Step" OnCheckedChanged="Step2_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </td>
                                            <td valign="top">
                                                <p>
                                                    <asp:Label ID="ConsolidationLblTitleStep2" runat="server"></asp:Label></p>
                                                <asp:Label ID="ConsolidationLblStep2" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="PnlSelectedConsolidation" runat="server" Visible="false">
                                <fieldset>
                                    <table cellspacing="2" cellpadding="2" border="0" width="100%">
                                        <tr>
                                            <td valign="top">
                                                <p>
                                                    <asp:Label ID="ConsolidationLblTitleOneChoise" runat="server"></asp:Label></p>
                                                <asp:Label ID="ConsolidationLblOneChoise" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </asp:PlaceHolder>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left">
        <asp:UpdatePanel ID="UpBtnOk" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cc1:CustomButton ID="ConsolidationBtnOk" runat="server" Text="OK" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="ConsolidationBtnOk_Click" OnClientClick="disallowOp('Content2');" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div style="float: left">
        <cc1:CustomButton ID="ConsolidationBtnChiudi" runat="server" CssClass="btnEnable"
            CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="ConsolidationBtnChiudi_Click" />
    </div>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
