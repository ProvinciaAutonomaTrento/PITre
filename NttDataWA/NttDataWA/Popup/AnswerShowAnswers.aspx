<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="AnswerShowAnswers.aspx.cs" Inherits="NttDataWA.Popup.AnswerShowAnswers" enableEventValidation="false" %>
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
    <div class="row">
        <p style="text-align: center;"><strong><asp:Literal id="litTitle" runat="server" /></strong></p>
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
            >
                <RowStyle CssClass="NormalRow" />
                <AlternatingRowStyle CssClass="AltRow" />
                <Columns>
                    <asp:TemplateField HeaderText="Doc. Data" ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>' Font-Bold="True" ForeColor='<%# GetColor(DataBinder.Eval(Container, "DataItem.segnatura")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Registro" ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Oggetto" ItemStyle-CssClass="grdList_description">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Destinatario" ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittDest") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipo" ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sel" ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <cc1:CustomImageButton runat="server" ID="ImgGoDocument" ImageUrl="../Images/Icons/ico_view_document.png"
                                OnMouseOutImage="../Images/Icons/ico_view_document.png" OnMouseOverImage="../Images/Icons/ico_view_document_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_view_document_disabled.png" onclick="ImgGoDocument_Click" RowIndex='<%# Container.DisplayIndex %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="chiave" />
                    <asp:TemplateField HeaderText="Del" ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <cc1:CustomImageButton runat="server" ID="ImgDelAnswer" ImageUrl="../Images/Icons/delete.png"
                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/delete_disabled.png" onclick="ImgDelAnswer_Click" RowIndex='<%# Container.DisplayIndex %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="segnatura" />
                </Columns>
            </asp:GridView>
            <asp:PlaceHolder id="plcNavigator" runat="server" />
            <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static">
                <ContentTemplate>
                    <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" Text="Chiudi" ClientIDMode="Static" 
        onclick="BtnClose_Click" />
</asp:Content>
