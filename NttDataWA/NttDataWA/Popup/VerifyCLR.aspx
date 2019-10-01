<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="VerifyCLR.aspx.cs" Inherits="NttDataWA.Popup.VerifyCLR" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        .message {text-align: left; line-height: 2em; width: 90%; min-height: 100px; margin: 80px auto 0 auto;}
        .message img {float: left; display: block; margin: 0 20px 20px 0;}
    </style>
        <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <asp:UpdatePanel ID="UpPnlMessage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcDate" Visible="false" runat="server">
                <div style=" padding-top:10px">
                    <div class="col2">
                        <asp:Literal runat="server" ID="LtlDateCheck"></asp:Literal>
                    </div>
                    <div class="col4">
                        <cc1:CustomTextArea ID="txt_DateCheck" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                    </div>
                    <div class="col2" style =" margin-left:10px;">
                        <p>
                            <asp:Literal ID="litMandateNewHour" runat="server" />
                        </p>
                    </div>
                    <div class="col">
                        <asp:DropDownList ID="ddlHourFrom" runat="server" CssClass="chzn-select-deselect" Width="80">
                            <asp:ListItem Value="" Text="" Selected="True" />
                            <asp:ListItem Value="00" Text="00" />
                            <asp:ListItem Value="01" Text="01" />
                            <asp:ListItem Value="02" Text="02" />
                            <asp:ListItem Value="03" Text="03" />
                            <asp:ListItem Value="04" Text="04" />
                            <asp:ListItem Value="05" Text="05" />
                            <asp:ListItem Value="06" Text="06" />
                            <asp:ListItem Value="07" Text="07" />
                            <asp:ListItem Value="08" Text="08" />
                            <asp:ListItem Value="09" Text="09" />
                            <asp:ListItem Value="10" Text="10" />
                            <asp:ListItem Value="11" Text="11" />
                            <asp:ListItem Value="12" Text="12" />
                            <asp:ListItem Value="13" Text="13" />
                            <asp:ListItem Value="14" Text="14" />
                            <asp:ListItem Value="15" Text="15" />
                            <asp:ListItem Value="16" Text="16" />
                            <asp:ListItem Value="17" Text="17" />
                            <asp:ListItem Value="18" Text="18" />
                            <asp:ListItem Value="19" Text="19" />
                            <asp:ListItem Value="20" Text="20" />
                            <asp:ListItem Value="21" Text="21" />
                            <asp:ListItem Value="22" Text="22" />
                            <asp:ListItem Value="23" Text="23" />
                        </asp:DropDownList>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcMessage" runat="server">
                <div class="message"  style=" float:left">
                    <asp:Literal ID="litMessage" runat="server" />
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
        <cc1:CustomButton ID="BtnCheck" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnCheck_Click" Visible="false" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
