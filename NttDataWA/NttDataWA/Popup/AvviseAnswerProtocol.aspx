<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="AvviseAnswerProtocol.aspx.cs" Inherits="NttDataWA.Popup.AvviseAnswerProtocol" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        .red {color: #f00;}
        .col-right {float: right; font-size: 0.8em;}
        ul {float: left; list-style: none; margin: 0; padding: 0; width: 90%;}
        li {display: inline; margin: 0; padding: 0;}
    </style>
    <script type="text/javascript">
        function AnswerProtocol(value) {
            parent.closeAjaxModal('AvviseProtocol', value, parent);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <uc:messager id="messager" runat="server" />
<!--
    <div class="row">
        <p align="center"><strong>ATTENZIONE!</strong></p>
    </div>
    <div class="row">
		<ul>
            <li id="opt_mitt" runat="server">Destinatario e mittente non coincidono</li>
            <li id="opt_ogg" runat="server">Gli oggetti non coincidono</li>
            <li id="opt_occ" runat="server">I documenti sono di registri differenti. I corrispondenti diventeranno occasionali</li>
            <li id="opt_occ_proto" runat="server">I documenti sono di registri differenti</li>
        </ul>
    </div>
-->
    <div class="row">
        <p align="center"><strong><asp:Literal ID="litTitle" runat="server" /></strong></p>
    </div>
    <asp:panel id="pnl_rispNoProto" Runat="server" class="row">
		<asp:RadioButtonList id="rbl_scelta" Runat="server" RepeatLayout="OrderedList">
		</asp:RadioButtonList>
	</asp:panel>
    <asp:panel ID="pnl_rispProto" Runat="server" class="row" Visible="False">
		<asp:radiobuttonlist id="rbl_scelta2" Runat="server" RepeatLayout="OrderedList">
		</asp:radiobuttonlist>
	</asp:panel>
    <asp:panel ID="pnl_sovrascriviCorr" Runat="server" class="row" Visible="False">
	    <asp:radiobuttonlist id="rbl_scelta3" Runat="server" RepeatLayout="OrderedList">
	    </asp:radiobuttonlist>
    </asp:panel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static"
        onclick="BtnOk_Click" />
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" Visible="False"
        onclick="BtnClose_Click" />
</asp:Content>
