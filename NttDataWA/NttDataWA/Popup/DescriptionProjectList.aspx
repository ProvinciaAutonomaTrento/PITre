<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DescriptionProjectList.aspx.cs"
    Inherits="NttDataWA.Popup.DescriptionProjectList" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="boxFilterPopup">
        <div class="row">
        <fieldset class="filterRefresh">
            <div class="boxFilterPopupContainer">
                <div class="boxFilterPopupContainerSx">
                    <div class="row">
                        <asp:Label CssClass="NormalBold" ID="ObjectLblRegistry" runat="server"></asp:Label>
                    </div>
                    <div class="row">
                        <asp:UpdatePanel ID="UpdPnlRegistry" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DdlRegRf" runat="server" Width="80%" CssClass="chzn-select-deselect">
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="boxFilterPopupContainerDx">
                    <asp:UpdatePanel ID="UpdPnlBtnInitialize" runat="server">
                        <ContentTemplate>
                            <cc1:CustomImageButton runat="server" ID="ObjectBtnInitialize" ImageUrl="../Images/Icons/icon_refresh.png"
                                OnMouseOutImage="../Images/Icons/icon_refresh.png" OnMouseOverImage="../Images/Icons/icon_refresh_hover.png"
                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/icon_refreshdisabled.png" OnClick="ObjectBtnInitialize_Click" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </fieldset>
        <fieldset class="filterRefresh2">
            <div class="row">
                <asp:Label CssClass="NormalBold" ID="ObjectLblCodObject" runat="server" Width="145"></asp:Label>
                <asp:Label CssClass="NormalBold" ID="ObjectLblVoiceObject" runat="server"></asp:Label>
                &nbsp;<asp:UpdatePanel ID="UpdPnlCodeObject" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="codeObjectSx">
                            <cc1:CustomTextArea ID="TxtCodObject" runat="server" Width="120px" CssClass="txt_objectLeft"></cc1:CustomTextArea>
                        </div>
                        <div class="codeObjectDx">
                            <cc1:CustomTextArea ID="TxtDescObject" runat="server" Height="100%" Width="600" TextMode="MultiLine"
                                ClientIDMode="Static" CssClass="txt_objectRight"> </cc1:CustomTextArea>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-right-no-margin">
                        <span class="charactersAvailable">
                            <asp:Literal ID="DocumentLitObjectChAv" runat="server" ClientIDMode="Static"> </asp:Literal>
                            <span id="TxtDescObject_chars" clientidmode="Static" runat="server"></span>
                        </span>
                    </div>
                </div>
            </div>
        </fieldset>
            </div>
        <div class="row">
            <asp:UpdatePanel ID="UpdPnlGridDescriptionProject" runat="server" class="row" UpdateMode="Conditional" ClientIDMode="Static">
                <ContentTemplate>
                    <div class="gridResultPopup2">
                        <asp:GridView ID="GridDescriptionProject" runat="server" AllowPaging="False" AutoGenerateColumns="False"
                            HorizontalAlign="Center" HeaderStyle-CssClass="tableHeaderPopup" CssClass="tabPopup tbl" PageSize="5"
                            OnRowDataBound="GridDescriptionProject_RowDataBound" ShowHeaderWhenEmpty="true"
                            Width="100%" ClientIDMode="Static">
                            <SelectedRowStyle CssClass="selectedrow" />
                            <HeaderStyle CssClass="tableHeaderPopup" />
                            <RowStyle CssClass="NormalRow" />
                            <AlternatingRowStyle CssClass="AltRow" />
                            <PagerStyle CssClass="recordNavigator2" />
                            <Columns>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSystemid" runat="server" Text='<%# Bind("systemId") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText='<%$ localizeByText:DescriptionProjectListCode%>' HeaderStyle-Width="30%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCodObject" runat="server" Text='<%# Bind("codice") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText='<%$ localizeByText:DescriptionProjectListDescription%>' HeaderStyle-Width="50%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescObject" runat="server" Text='<%# Bind("descrizione") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText='<%$ localizeByText:DescriptionProjectListReg%>' ItemStyle-Width="20">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRegCode" runat="server" Text='<%# Bind("codRegistro") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdRegistro" runat="server" Text='<%# Bind("idRegistro") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdAmm" runat="server" Text='<%# Bind("idAmm") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <SelectedRowStyle BackColor="Yellow" />
                        </asp:GridView>
                        <asp:PlaceHolder ID="plcNavigator" runat="server" />
                        <asp:UpdatePanel ID="upPnlGridPageIndex" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                            <ContentTemplate>                             
                                <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left">
        <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <cc1:CustomButton ID="ObjectBtnSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClick="ObjectBtnSearch_Click" OnClientClick="disallowOp('Content2')" />
                <cc1:CustomButton ID="ObjectBtnInsert" OnClick="btnInsert_Click" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClientClick="disallowOp('Content2')" />
                <cc1:CustomButton ID="ObjectBtnUpdate" runat="server" OnClick="btnUpdate_Click" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="disallowOp('Content2')" />
                <cc1:CustomButton ID="ObjectBtnDelete" runat="server" OnClick="btnDelete_Click" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="disallowOp('Content2')" />
                <cc1:CustomButton ID="BtnOk" runat="server" Text="OK" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnOk_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div style="float: left">
        <cc1:CustomButton ID="ObjectBtnChiudi" runat="server" OnClick="ObjectBtnChiudi_Click"
            CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
    </div>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
