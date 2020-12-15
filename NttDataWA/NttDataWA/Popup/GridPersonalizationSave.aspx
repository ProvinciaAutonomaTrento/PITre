<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="GridPersonalizationSave.aspx.cs" Inherits="NttDataWA.Popup.GridPersonalizationSave" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
                <asp:UpdatePanel ID="upTemplates" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <fieldset class="basic2">
                            <legend><asp:Literal ID="l_actions" runat="server"></asp:Literal></legend>
                            <asp:RadioButtonList ID="rbl_save" runat="server" Width="368px"
                                AutoPostBack="true" OnSelectedIndexChanged="ChangeMode">
                                    <asp:ListItem Value="new" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="mod"></asp:ListItem>
                            </asp:RadioButtonList>
                        </fieldset>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upChangeGrid" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <fieldset class="basic2">
                            <asp:PlaceHolder ID="pnl_scelta" runat="server" Visible="false">
                                <legend><asp:Literal ID="l_GridModify" runat="server"></asp:Literal></legend>
                                <asp:DropDownList ID="ddl_ric_griglie" runat="server"  Width="99" AutoPostBack="true" OnSelectedIndexChanged="ChangeSelectedGrid" CssClass="chzn-select" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="nuova_g" runat="server" Visible="false">
                                <legend><asp:Literal ID="l_GridNew" runat="server"></asp:Literal></legend>
                                <cc1:CustomTextArea ID="txt_titolo" runat="server"  Width="98%" MaxLength="30" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" TextMode="SingleLine"></cc1:CustomTextArea>
                             </asp:PlaceHolder>
                        </fieldset>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upChangeGridName" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <asp:PlaceHolder ID="modify_g" runat="server" Visible="false">
                             <fieldset class="basic2">
                                <legend><asp:Literal ID="l_GridName" runat="server"></asp:Literal></legend>
                                <cc1:CustomTextArea ID="txt_name_mod" runat="server"  Width="98%" MaxLength="30" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" TextMode="SingleLine"></cc1:CustomTextArea>
                            </fieldset>
                        </asp:PlaceHolder>
                    </contenttemplate>
                </asp:UpdatePanel>
                <fieldset class="basic2">
                    <legend><asp:Literal ID="l_RendiDisponibile" runat="server"></asp:Literal></legend>
                    <asp:UpdatePanel ID="pnl_visibility" runat="server" UpdateMode="Conditional">
                        <contenttemplate>
                            <asp:RadioButtonList ID="rblVisibilita" runat="server">
                                <asp:ListItem Value="user" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="role"></asp:ListItem>
                            </asp:RadioButtonList>
                        </contenttemplate>
                    </asp:UpdatePanel>
                </fieldset>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div id="imposta">
                            <asp:CheckBox runat="server" ID="chk_pref" TextAlign="right" />
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
<asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="GridPersonalizationSaveBtnInserisci" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2')"
                OnClick="GridPersonalizationSaveBtnInserisci_Click" />
            <cc1:CustomButton ID="GridPersonalizationSaveBtnClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="GridPersonalizationSaveBtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
       <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
