<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="AnswerChooseRecipient.aspx.cs" Inherits="NttDataWA.Popup.AnswerChooseRecipient" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        .red {color: #f00;}
        .col-right {float: right; font-size: 0.8em;}
        ul {float: left; list-style: none; margin: 0; padding: 0; width: 90%;}
        li {display: inline; margin: 0; padding: 0;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row">
        <p align="center"><strong><asp:Literal ID="litTitle" runat="server" /></strong></p>
    </div>
    <div id="rowMessage" runat="server" />
    <asp:UpdatePanel ID="upPnlGridList" runat="server" class="row" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <asp:GridView ID="grdList" runat="server"
                width="98%"
                AutoGenerateColumns="False"
                AllowPaging="True"
                ClientIDMode="Static"
                CssClass="tbl"
                OnPageIndexChanging="grdList_PageIndexChanged"
            >
                <RowStyle CssClass="NormalRow" />
                <AlternatingRowStyle CssClass="AltRow" />
                <Columns>
                    <asp:BoundField HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="SYSTEM_ID" />
                    <asp:BoundField ItemStyle-CssClass="grdList_description" DataField="DESC_CORR" HeaderText="Descrizione" HtmlEncode="false" />
                    <asp:BoundField HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="TIPO_CORR" />
                    <asp:TemplateField HeaderText="Sel" ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                               <asp:RadioButton id="OptCorr" runat="server" Text="" AutoPostBack="True" GroupName="one" OnCheckedChanged="CheckOne_Click" RowIndex='<%# Container.DisplayIndex %>'></asp:RadioButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:BoundField HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="DISABLED"  />
                </Columns>
                <pagersettings mode="Numeric"
                    position="Bottom"           
                    pagebuttoncount="10" />
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static"
        onclick="BtnOk_Click" />
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" 
        onclick="BtnClose_Click" />
</asp:Content>
