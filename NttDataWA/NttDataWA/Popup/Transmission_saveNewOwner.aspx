<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="Transmission_saveNewOwner.aspx.cs" Inherits="NttDataWA.Popup.Transmission_saveNewOwner" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        .red {color: #f00;}
        .col-right {float: right; font-size: 0.8em;}
        #txtTitle {width: 100%; height: 3em;}
        ul {list-style: none; margin: 0; padding: 0;}
        ul.li {margin: 0; padding: 0;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
            <div class="container">
                <div id="rowMessage" runat="server" />
                <p><asp:Literal ID="TransmissionTransferRightsDescription" runat="server" />
                </p>
                <p><asp:RadioButtonList ID="rblUsers" runat="server" RepeatLayout="UnorderedList" ClientIDMode="Static" ValidationGroup="rblUsers" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="rblUsers" EnableClientScript="false" ForeColor="Red"></asp:RequiredFieldValidator>
                </p>
            </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
            <cc1:CustomButton ID="TransmissionBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="TransmissionBtnSave_Click" OnClientClick="disallowOp('Content2')" />
            <cc1:CustomButton ID="TransmissionBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="TransmissionBtnClose_Click" />
</asp:Content>
