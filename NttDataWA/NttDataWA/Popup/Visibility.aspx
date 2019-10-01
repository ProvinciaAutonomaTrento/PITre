<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="Visibility.aspx.cs" Inherits="NttDataWA.Popup.Visibility" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        .corr_grey {color: #ccc;}
        #divDetails {text-align: left; margin: 5px 0 0 0; padding: 5px 0 0 0; font-size: 0.9em; border-top: 1px solid #ccc;}
        #divDetails ul {list-style-type: disc; margin: 0; padding: 0;}
        #divDetails ul li {margin: 0 0 0 15px;}
        .tbl td {vertical-align: top; cursor: pointer;}
        .tbl tr.nopointer td {cursor: default;}
        .tbl th { white-space: nowrap;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="VisibilityRemove" runat="server" Url="../popup/VisibilityRemove.aspx"
        Width="600" Height="350" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('HiddenField1', ''); }" />
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdPanelVisibility" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="GridDocuments" runat="server" ClientIDMode="Static" CssClass="tbl"
                        Width="99%" AutoGenerateColumns="False" AllowPaging="True" OnRowCommand="GridDocuments_RowCommand"
                        OnSelectedIndexChanging="GridDocuments_SelectedIndexChanging" OnPageIndexChanging="GridDocuments_PageIndexChanging"
                        DataKeyNames="idCorr" OnPreRender="GridDocuments_PreRender">
                            <RowStyle CssClass="NormalRow" />
                            <AlternatingRowStyle CssClass="AltRow" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                        <asp:Image ID="imgTipo" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Literal ID="litRole" runat="server" Text='<%# Bind("Ruolo") %>'></asp:Literal>
                                    <div id="divDetails" runat="server" visible="false">
                                        <div><strong><asp:Literal ID="lblDetailsUser" runat="server" Visible="false" /></strong></div>
                                        <asp:Literal ID="LblDetails" runat="server"></asp:Literal>
                                        <asp:Literal ID="VisibilityLblDetails" runat="server" />
                                        <asp:Literal ID="LblDetailsInfo" runat="server" />
                                        <asp:HiddenField ID="hdnTipo" runat="server" Value='<%#Eval("Tipo") %>' />
                                        <asp:HiddenField ID="hdnCodRubrica" runat="server" Value='<%#Eval("CodiceRubrica") %>' />
                                    </div>
                                </ItemTemplate>
                                <ControlStyle Width="120px" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="LblDiritto" runat="server" Text='<%# Bind("diritti") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="TipoDiritto" />
                            <asp:BoundField DataField="DataInsSecurity" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="LblEndDate" runat="server" Text='<%# Bind("DataFine") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="NoteSecurity" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImgDelete" runat="server" CommandName="Erase" ImageAlign="Middle"
                                        ImageUrl="~/Images/Icons/delete.png" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField visible="false">
                                <HeaderTemplate>
                                    <asp:ImageButton ID="ImgHistory" runat="server" CommandName="History" ImageUrl="~/Images/Icons/history.gif"
                                        OnClientClick="return ajaxModalPopupVisibilityHistory();" />
                                </HeaderTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField Visible="False">
                                <ItemTemplate>
                                    <asp:Label ID="LblRemoved" runat="server" Text='<%# Bind("Rimosso") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Note" Visible="False" />
                            <asp:BoundField HeaderText="idCorr" Visible="False" DataField="idCorr" />
                            <asp:TemplateField Visible="False">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImgDetails" runat="server" CommandName="Details" ImageAlign="Middle"
                                        ImageUrl="~/Images/Icons/docDetails.png" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:HiddenField ID="HiddenField1" runat="server" ClientIDMode="Static" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:HiddenField ID="rowIndex" runat="server" ClientIDMode="Static" />
            <asp:Button ID="btnDetails" runat="server" ClientIDMode="Static" CssClass="hidden" OnClick="GridDocuments_Details" />
            <div class="row">
                <asp:Label ID="VisibilityLblDelVis" runat="server" Visible="False"></asp:Label>
                <br />
                <asp:Label ID="lblResult" runat="server" ForeColor="Red" Visible="False"></asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" onclick="BtnClose_Click" />
</asp:Content>
