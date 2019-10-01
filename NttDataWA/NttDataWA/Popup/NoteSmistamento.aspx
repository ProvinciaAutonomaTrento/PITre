<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoteSmistamento.aspx.cs"
    Inherits="NttDataWA.Popup.NoteSmistamento" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc2" TagName="Calendar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container
        {
           margin-left:5px;
        }
        .container img
        {
            padding-left: 2px;
        }
    </style>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div class="row">
        </div>
        <div class="row">
            <div class="col">
                <span class="weight">
                    <asp:Literal ID="lblNoteInd" runat="server" /></span>
            </div>
        </div>
        <div class="row">
            <div class="col-full">
                <cc1:CustomTextArea ID="txtNoteInd" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                    CssClassReadOnly="txt_textarea_disabled" MaxLength="250" ClientIDMode="Static" Width="98%">
                </cc1:CustomTextArea>
            </div>
        </div>
        <div class="row">
            <div class="col-right-no-margin">
                <span class="charactersAvailable">
                    <asp:Literal ID="LtrNoteInd" runat="server" ClientIDMode="Static"> </asp:Literal>
                    <span id="txtNoteInd_chars" clientidmode="Static" runat="server"></span>
                </span>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <span class="weight">
                <asp:Label ID="lblCorr" runat="server"></asp:Label>
                    </span>
            </div>
            <div class="col">
                <span class="weight">
                    <asp:Literal runat="server" ID="LitNoteSmistamentoScadTitle"></asp:Literal>
                </span>
            </div>
            <div class="col2">
                <cc1:CustomTextArea ID="txt_dtaScadenza" runat="server" CssClass="txt_textdata datepicker"
                    CssClassReadOnly="txt_textdata_disabled">
                </cc1:CustomTextArea>
            </div>
        </div>
        <asp:UpdatePanel ID="UpPnlTypeTrasm" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnl_tipoTrasm" runat="server">
                    <div class="row">
                        <div class="col">
                            <span class="weight">
                                <asp:Literal ID="LitNoteSmistamentoType" runat="server" />
                            </span>
                        </div>
                        <div class="col">
                            <asp:DropDownList ID="ddl_tipo" runat="server" AutoPostBack="true" Width="140px"
                                CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_tipo_SelectedIndexChanged">
                                <asp:ListItem Value="S" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="T"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick="disallowOp('Content2');" OnClick="BtnSave_Click" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover"  OnClick="BtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
