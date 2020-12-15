<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="Object.aspx.cs" Inherits="NttDataWA.Popup.Object" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $("#TxtDescObject").focus();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="boxFilterPopup">
        <fieldset class="filterRefresh">
            <div class="boxFilterPopupContainer">
                <div class="boxFilterPopupContainerSx">
                    <p>
                        <asp:Label CssClass="NormalBold" ID="ObjectLblRegistry" runat="server"></asp:Label>
                    </p>
                    <p>
                        &nbsp;<asp:UpdatePanel ID="UpdPnlRegistry" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DdlRegRf" runat="server" Width="80%" CssClass="chzn-select-deselect">
                                </asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="gridSearchObject" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </p>
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
            <p>
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
                <div class="availableCarachtersObject">
                    <span>  <asp:Literal ID="DocumentLitObjectChAv" runat="server"></asp:Literal></span>&nbsp;<span id="TxtDescObject_chars"></span>
                </div>
            </p>
        </fieldset>
        <asp:UpdatePanel ID="UpdPnlGridSearch" runat="server" class="row" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <div class="gridResultPopup2">
                    <asp:GridView ID="GridSearchObject" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                        HorizontalAlign="Center" HeaderStyle-CssClass="tableHeaderPopup" CssClass="tabPopup tbl" PageSize="9"
                        OnRowDataBound="gridSearchObject_RowDataBound"
                        OnPageIndexChanging="gridSearchObject_PageIndexChanging"
                        Width="100%" ClientIDMode="Static" >
                        <SelectedRowStyle CssClass="selectedrow" />
                        <HeaderStyle CssClass="tableHeaderPopup" />
                        <RowStyle CssClass="NormalRow" />
                        <AlternatingRowStyle CssClass="AltRow" />
                        <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="systemid" runat="server" Text='<%# Bind("systemId") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Oggetto">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditCodOggetto" runat="server" Text='<%# Bind("codOggetto") %>'
                                        Width="96%"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtInsertCodOggetto" runat="server" Visible="False" Width="96%"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblCodObject" runat="server" Text='<%# Bind("codOggetto") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Voce Oggettario">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditDescrObject" runat="server" Text='<%# Bind("descrizione") %>'
                                        Width="96%"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtInsertDescrObject" runat="server" Height="4em" MaxLength="2000"
                                        TextMode="MultiLine" Visible="False" Width="96%"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDescObject" runat="server" Text='<%# Bind("descrizione") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Registro" ItemStyle-Width="90px">
                                <ItemTemplate>
                                    <asp:Label ID="lblRegCode" runat="server" Text='<%# Bind("codregistro") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="False" Visible="False">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ibDettaglio" runat="server" CausesValidation="False" CommandName="Select"
                                        ImageUrl="~/Images/Common/icon_round_arrow_left.png" Text="Seleziona" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <SelectedRowStyle BackColor="Yellow" />
                    </asp:GridView>
                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:Label ID="ObjectLblFindRis" runat="server" Font-Bold="True" Visible="False"></asp:Label>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left">
        <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <cc1:CustomButton ID="ObjectBtnSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClick="ObjectBtnSearch_Click" OnClientClick="disallowOp('Content2')"/>
                <cc1:CustomButton ID="ObjectBtnInsert" OnClick="btnInsert_Click" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClientClick="disallowOp('Content2')"/>
                <cc1:CustomButton ID="ObjectBtnUpdate" runat="server" OnClick="btnUpdate_Click" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="disallowOp('Content2')"/>
                <cc1:CustomButton ID="ObjectBtnDelete" runat="server" OnClick="btnDelete_Click" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="disallowOp('Content2')"/>
                <cc1:CustomButton ID="BtnOk" runat="server" Text="OK" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnOk_Click"/>
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
