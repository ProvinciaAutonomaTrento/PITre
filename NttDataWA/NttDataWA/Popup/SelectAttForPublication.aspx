<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" 
    CodeBehind="SelectAttForPublication.aspx.cs" Inherits="NttDataWA.Popup.SelectAttForPublication" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="container">
        <fieldset>
            <asp:GridView ID="grdAllegati" runat="server" Width="98%" AutoGenerateColumns="False"
                                        AllowPaging="True" CssClass="tbl_rounded round_onlyextreme" PageSize="100" BorderWidth="0"
                                        OnRowDataBound="grdAllegati_RowDataBound" 
                                        OnPageIndexChanging="grdAllegati_PageIndexChanging">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:BoundField DataField="versionLabel" ItemStyle-CssClass="grdAllegati_code" runat="server"
                                                HeaderText='<%$ localizeByText:AttachmentsCod%>' />
                                            <asp:TemplateField ItemStyle-CssClass="grdAllegati_date" runat="server" HeaderText='<%$ localizeByText:AttachmentsDate%>'>
                                                <ItemTemplate>
                                                    <%# NttDataWA.Utils.dateformat.dateLength(Eval("dataInserimento").ToString())%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="descrizione" ItemStyle-CssClass="grdAllegati_description"
                                                runat="server" HeaderText='<%$ localizeByText:AttachmentsDescription%>' />
                                            <asp:BoundField DataField="numeroPagine" ItemStyle-CssClass="grdAllegati_pages" runat="server"
                                                HeaderText='<%$ localizeByText:AttachmentsPage%>' />
                                            <asp:TemplateField ItemStyle-CssClass="grdAllegati_icon">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkPubblicazione" runat="server" />
                                                    <asp:HiddenField ID="attachId" runat="server" Value='<%#Eval("versionId").ToString() %>' />
                                                    <asp:HiddenField ID="attDocNum" runat="server" Value='<%#Eval("docNumber").ToString() %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="HiddenRemoveAttachment" runat="server" ClientIDMode="Static" />
                               
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
            <cc1:CustomButton ID="SelectAttForPubOK" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static"
                OnClick="SelectAttForPubOK_Click"/>
            <cc1:CustomButton ID="SelectAttForPubAnnulla" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static"
                OnClick="SelectAttForPubAnnulla_Click"/>
            </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>
