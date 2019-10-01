<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlussoAutomatico.aspx.cs"
    Inherits="NttDataWA.Popup.FlussoAutomatico" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #container
        {
            position: fixed;
            top: 1px;
            left: 0px;
            bottom: 71px;
            right: 0px;
            overflow: auto;
            background: #ffffff;
            text-align: left;
            padding: 20px;
        }
        .gridViewResult
        {
            min-width: 100%;
            overflow: auto;
        }
        .gridViewResult th
        {
            text-align: center;
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
        #gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }
        
        .tbl_rounded tr.Borderrow
        {
            border-top: 2px solid #FFFFFF;
        }
        
        .tbl_rounded td
        {
            background: #fff;
            min-height: 1em;
            border: 1px solid #A8A8A8;
            border-top: 0;
            text-align: center;
        }
        
        .margin-left
        {
            padding-left: 5px;
        }
        .tbl_rounded tr.Borderrow:hover td
        {
            background-color: #b2d7f8;
        }
    </style>
    >
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlNoFlussoAutomatico" runat="server" Visible="false">
            <div class="row">
                <p>
                    <asp:Label ID="LblNoFlussoAutomatico" runat="server"></asp:Label></p>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlIdProcesso" runat="server">
            <div class="row">
                <strong>
                    <asp:Label ID="lblIdProcesso" runat="server"></asp:Label></strong> <span class="redWeight">
                        <span class="anchorTypeRed">
                            <asp:Label ID="lblIdProcessoCode" runat="server"></asp:Label></span></span>
            </div>
        </asp:Panel>
        <asp:UpdatePanel ID="UpGridFlussoAutomatico" runat="server" pdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <asp:GridView ID="GridFlussoAutomatico" runat="server" AllowSorting="true" AllowPaging="false"
                                AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                    OnRowDataBound="GridFlussoAutomatico_RowDataBound" OnPreRender="GridFlussoAutomatico_PreRender"
                    OnRowCreated="GridFlussoAutomatico_ItemCreated" OnRowCommand="GridFlussoAutomatico_RowCommand" >
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" />
                    <Columns>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblIdProfile" Text='<%# DataBinder.Eval(Container, "DataItem.INFO_DOCUMENTO.ID_PROFILE") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:FlussoAutomaticoNumProto%>' HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <span class="noLink">
                                    <asp:LinkButton ID="LblNumProto" runat="server" Text='<%#this.GetLabelNumProto((NttDataWA.DocsPaWR.Flusso) Container.DataItem) %>'
                                        CommandName="ViewDocument"></asp:LinkButton></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:FlussoAutomaticoOggetto%>' HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Literal ID="litOggetto" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.INFO_DOCUMENTO.OGGETTO") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:FlussoAutomaticoRichiesta%>' HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Literal ID="litRichiesta" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MESSAGGIO.DESCRIZIONE") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:FlussoAutomaticoDataRichiesta%>'
                            HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Literal ID="litDataRichiesta" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DATA_ARRIVO") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:FlussoAutomaticoNomeRegistroIn%>'
                            HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Literal ID="litNomeRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.INFO_DOCUMENTO.NOME_REGISTRO_IN") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:FlussoAutomaticoNumeroRegistroIn%>'
                            HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Literal ID="litNumRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.INFO_DOCUMENTO.NUMERO_REGISTRO_IN") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText='<%$ localizeByText:FlussoAutomaticoDataRegistro%>'
                            HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Literal ID="litDataRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.INFO_DOCUMENTO.DATA_REGISTRO_IN") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="FlussoAutomaticoClose" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('Content2');" CssClassDisabled="btnDisable" OnMouseOver="btnHover"
                ClientIDMode="Static" OnClick="FlussoAutomaticoClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
