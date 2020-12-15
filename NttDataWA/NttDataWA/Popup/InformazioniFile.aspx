<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InformazioniFile.aspx.cs"
    Inherits="NttDataWA.Popup.InformazioniFile" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    ClientIDMode="static" runat="server">
    <div class="container"  style="padding: 20px">
        <asp:UpdatePanel ID="UpPnlGridInfoFile" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="GridInfoFile" runat="server" AutoGenerateColumns="false"  CssClass="tbl_rounded_custom round_onlyextreme"
                    BorderWidth="0" OnRowDataBound ="GridInfoFile_OnRowDataBound" Width="100%" >
                        <RowStyle CssClass="NormalRow" Height="50" />
                        <AlternatingRowStyle CssClass="AltRow" />
                        <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:TemplateField  HeaderText="Cod." ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5%" HeaderStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="LblCodice" Text='<%#this.GetCodice((NttDataWA.DocsPaWR.InfoFile) Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField ItemStyle-Width="25%" DataField="Oggetto" HeaderText="Oggetto" />
                            <asp:BoundField ItemStyle-Width="20%" DataField="NomeFile" HeaderText="Nome file"/>
                            <%--<asp:BoundField ItemStyle-Width="50px" DataField="Estensione" HeaderText="Estensione" ItemStyle-HorizontalAlign="Center" />--%>
                             <asp:TemplateField  HeaderText="Data acquisizione" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5%" HeaderStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="dataAcquisizione" Text='<%#this.GetDataAcquisizione((NttDataWA.DocsPaWR.InfoFile) Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                                <HeaderTemplate>
                                    <asp:Label ID="lblConforme" Text="Conforme" ToolTip="Conforme" runat="server"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Image ID="imgConforme" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                </ItemTemplate>
                            </asp:TemplateField>                         
                             <asp:TemplateField ItemStyle-Width="25%" HeaderText="Descrizione non conformità">
                                <ItemTemplate>
                                    <asp:Label ID="lblNonConforme" Text='<%#this.GetDescrizioneNonConforme((NttDataWA.DocsPaWR.InfoFile) Container.DataItem) %>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
  <%--                          <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                                <HeaderTemplate>
                                    <asp:Label ID="lblEstConforme" Text="Est. Conforme" ToolTip="Estensione Conforme" runat="server"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Image ID="imgEstConforme" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                                <HeaderTemplate>
                                    <asp:Label ID="lblMacro" Text="Macro" ToolTip="Macro" runat="server"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Image ID="imgMacro" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                                <HeaderTemplate>
                                    <asp:Label ID="lblForms" Text="Forms" ToolTip="Forms" runat="server"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Image ID="imgForms" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHavascript" Text="Javascript" ToolTip="Javascipt" runat="server"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Image ID="imgJavascript" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                        </Columns>
                    </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnInformationiFileClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnInformationFileClose_Click"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>