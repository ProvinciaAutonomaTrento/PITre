<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RepositoryView.aspx.cs" Inherits="NttDataWA.Repository.RepositoryView" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<fieldset>
    <div class="row">
        <div class="col">
                <span class="weight">
                    <asp:Literal runat="server" ID="lblSearch">Filtro ricerca</asp:Literal>
                </span>
        </div>
    </div>
    <div class="row">
        <asp:UpdatePanel ID="UpPSearch" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:PlaceHolder runat="server" ID="PnlSearch">
                    <div class="colHalf">
                        <asp:Label ID="lblSearch_FileName" runat="server" Text='Nome File:' />
                    </div>
                    <div class="colHalf2">
                        <div class="colHalf3">
                            <cc1:CustomTextArea ID="txtSearch_FileName" runat="server" CssClass="txt_addressBookRight"
                                autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled">
                            </cc1:CustomTextArea>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="PlaceHolder1">
                    <div class="colHalf">
                        <asp:Label ID="lblSearch_Description" runat="server" Text='Descrizione File:' />
                    </div>
                    <div class="colHalf2">
                        <div class="colHalf3">
                            <cc1:CustomTextArea ID="txtSearch_Description" runat="server" CssClass="txt_addressBookRight"
                                autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled">
                            </cc1:CustomTextArea>
                        </div>
                    </div>
                    <div class="col">
                        <asp:RadioButtonList ID="chkSearch_StatusType" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                            <asp:ListItem Value="Tutti" runat="server" id="optSearch_All" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="Completo" runat="server" id="optSearch_Complete"></asp:ListItem>
                            <asp:ListItem Value="Incompleto" runat="server" id="optSearch_InProgress"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <div class="col-right-no-margin">
                        <cc1:CustomImageButton runat="server" ID="imgSearch" ImageUrl="../Images/Icons/zoom.png"
                            OnMouseOutImage="../Images/Icons/zoom.png" OnMouseOverImage="../Images/Icons/zoom_hover.png"
                            CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/zoom_disabled.png" OnClick="ImgSearch_Click"/>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</fieldset>
<div class="row">
    <asp:Label ID="lblFilesComplete" runat="server" Text=''/><br />
    <asp:UpdatePanel ID="UpGrid" runat="server" class="row" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <asp:GridView ID="grdfileList" runat="server" Width="100%" CssClass="tabPopup tbl" AutoGenerateColumns="False"
                AllowPaging="True" PageSize="10" OnRowDataBound="gridViewResult_RowDataBound"
                    OnPreRender="gridViewResult_PreRender" OnPageIndexChanging="gridViewResult_PageIndexChanging"
                    OnRowCreated="gridViewResult_ItemCreated" OnRowCommand="gridViewResult_RowCommand" 
                    OnSelectedIndexChanged="gridViewResult_SelectedIndexChanged">
                <RowStyle CssClass="NormalRow" />
                <AlternatingRowStyle CssClass="AltRow" />
                <PagerStyle CssClass="recordNavigator2" />
                <Columns>
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="systemIdElemento" Text='<%# Bind("StrIdentity") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lblNomeFile" CssClass="clickable" Width="190pt"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true">
                        <ItemTemplate>
                            <asp:Label ID="lblDescrizione" runat="server" Width="150pt" />
                            <asp:LinkButton runat="server" ID="lkbDescrizione" CommandName="Description" CssClass="clickable"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true">
                        <ItemTemplate>
                            <asp:Label ID="lblDimensione" runat="server" Text='<%# Bind("FileSize") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true">
                        <ItemTemplate>
                            <asp:Image ID="imgFile" runat="server" CssClass="chzn-select-deselect" ImageUrl='' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true">
                        <ItemTemplate>
                            <asp:Label ID="lblPercentage" runat="server" Text='' />
                            <asp:Image ID="imgPercentage" runat="server" ImageUrl='' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true">
                        <ItemTemplate>
                            <cc1:CustomImageButton ID="imgDeleteFile" runat="server" CommandName="DeleteFile" ></cc1:CustomImageButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <selectedrowstyle backcolor="LightCyan" forecolor="DarkBlue" font-bold="true"/>
            </asp:GridView>
            <asp:PlaceHolder ID="plcNavigator" runat="server" />
            <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
 </div>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="RepositoryRefresh" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Aggiorna" OnClick="RepositoryRefresh_Click" />
            <cc1:CustomButton ID="RepositoryClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="RepositoryClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
