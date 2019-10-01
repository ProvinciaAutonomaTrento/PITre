<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransferReport.aspx.cs"
    Inherits="NttDataWA.Popup.TransferReport" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/jscript">

    function resizeTableDoc() {
        if ($('.GrdDocResult').length > 0 && $('.GrdDocResult')[0].offsetTop) {
            var height = document.documentElement.clientHeight;
            var rowHeight = (height) / ($('.GrdDocResult')[0].rows.length);
            for (var i = 1; i < $('.GrdDocResult')[0].rows.length; i++)
                $('.GrdDocResult')[0].rows[i].style.height = rowHeight + 'px';

            window.onresize = resizeIframe
        }
    };

    function resizeTableFasc() {
        if ($('.GrdFascResult').length > 0 && $('.GrdFascResult')[0].offsetTop) {
            var height = document.documentElement.clientHeight;
            var rowHeight = (height) / ($('.GrdFascResult')[0].rows.length);
            for (var i = 1; i < $('.GrdFascResult')[0].rows.length; i++)
                $('.GrdFascResult')[0].rows[i].style.height = rowHeight + 'px';

            window.onresize = resizeIframe
        }
    };

</script>
    <script src="../Scripts/jquery.ui.multidraggable-1.8.8.js" type="text/javascript"></script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">

    <div id="container">
        <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpdatePanelTop">
            <ContentTemplate>
                <div class="boxTopReport">
                    <fieldset class="filterAddressbook">
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="litIdVersamento" runat="server"></asp:Literal></strong>
                            </div>
                            <div class="col">
                                <NttDL:CustomTextArea ID="txtIdVersamento" runat="server" CssClass="txt_input_half"
                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="False"></NttDL:CustomTextArea>
                            </div>
                        </div>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="litDescrVersamento" runat="server"></asp:Literal></strong>
                            </div>
                            <div class="colHalf_arch">
                                <NttDL:CustomTextArea ID="TxtDescrVersamento" runat="server" CssClass="txt_input_half"
                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="False"></NttDL:CustomTextArea>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="contentAddressBook">
            <div id="topContentAddressBook">
                <asp:UpdatePanel ID="UpTypeResult" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <ul>
                            <li class="addressTab" id="liReportDocumenti" runat="server">
                                <asp:LinkButton runat="server" ID="DocTabLinkList" OnClick="AddressBookLinkList_Click"></asp:LinkButton></li>
                            <li class="otherAddressTab" id="liReportFascicoli" runat="server">
                                <asp:LinkButton runat="server" ID="FascTabLinkList" OnClick="AddressBookLinkOrg_Click"
                                    OnClientClick="disallowOp('Content2')"></asp:LinkButton></li>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="centerContentAddressbook">
                <div class="marginLeft">
               
                    <asp:UpdatePanel ID="UpPnlGridDocResult" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                        <asp:Label ID="lblerror" runat="server"></asp:Label>
                            <asp:Panel ID="PnlGridDocResult" runat="server">
                            
                            <div class="row">
                               <div class="col-right">
                                    <NttDL:CustomImageButton ID="cImgBtnExportDoc" ImageUrl="../Images/Icons/export excel-pdf.png"
                                        runat="server" OnMouseOverImage="../Images/Icons/export-excel-pdf_hover.png"
                                        OnMouseOutImage="../Images/Icons/export excel-pdf.png" 
                                        CssClass="clickableLeft"  OnClick="btnExportDoc_Click" />
                                </div>
                                
                             </div>
                             <div class="row">
                                <asp:GridView ID="GrdDocResult" runat="server" Width="99%" AutoGenerateColumns="False" OnRowDataBound="GrdDocResult_RowDataBound"
                                    AllowPaging="True" CssClass="tbl_rounded_custom round_onlyextreme GrdDocResult" PageSize="9"
                                    BorderWidth="0" OnPageIndexChanging="GrdDocResult_PageIndexChanging" DataKeyNames="Profile_ID">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblRegistrogvDocumentInAUTHHeader" Text='Registro' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="registro" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblTipogvDocumentInAUTHHeader" Text='UO' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="UO" runat="server" Text='<%# Bind("UO") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='ID Doc.' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="ID_Profile" runat="server" Text='<%# Bind("Profile_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Proto.' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Proto" runat="server" Text='<%# Bind("Proto") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Anno Proto.' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="ProtoDate" runat="server" Text='<%# Bind("ProtoDate") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Data Creazione.' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="CreateDate" runat="server" Text='<%# Bind("CreateDate") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Oggetto' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="oggetto" runat="server" Text='<%# Bind("Oggetto") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Tipo' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Tipo" runat="server" Text='<%# Bind("Tipo") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Mitt/Dest' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Corr" runat="server" Text='<%# Bind("MittDest") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                         <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Tipologia' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="tipologia" runat="server" Text='<%# Bind("Tipologia") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Codice Fasc' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="ProjectCode" runat="server" Text='<%# Bind("ProjectCode") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='Tipo Transfer' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="idProfile" runat="server" Text='<%# Bind("TipoTransfer") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpPnlGridFascResult" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="PnlFascReport" Visible="false">
                            
                            <div class="row">
                                <div class="col-right">
                                    <NttDL:CustomImageButton ID="cImgBtnExportFasc" ImageUrl="../Images/Icons/export excel-pdf.png"
                                        runat="server" OnMouseOverImage="../Images/Icons/export-excel-pdf_hover.png"
                                        OnMouseOutImage="../Images/Icons/export excel-pdf.png" ImageUrlDisabled="../Images/Icons/export excel-pdf.png"
                                        CssClass="clickableLeft" OnClick="btnExportFasc_Click" />
                                </div>
                                </div>
                                <div class="row">
                                <asp:GridView ID="GrdFascResult" runat="server" Width="99%" AutoGenerateColumns="False"
                                    AllowPaging="True" CssClass="tbl_rounded_custom round_onlyextreme GrdFascResult" PageSize="9" DataKeyNames="Project_ID"
                                    BorderWidth="0" OnPageIndexChanging="GrdFascResult_PageIndexChanging" OnRowDataBound="GrdFascResult_RowDataBound">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblRegistrogvDocumentInAUTHHeader" Text='Registro' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="registro" runat="server" Text='<%# Bind("Registro") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblTipogvDocumentInAUTHHeader" Text='UO' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="tipo" runat="server" Text='<%# Bind("UO") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='ID fasc.' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="iD_Protocollo" runat="server" Text='<%# Bind("Project_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblTipogvDocumentInAUTHHeader" Text='Descrizione' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="data" runat="server" Text='<%# Bind("Descrizione") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='data creazione' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="oggetto" runat="server" Text='<%# Bind("startDate") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='data chiusura' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="idProfile" runat="server" Text='<%# Bind("closeDate") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='tipologia' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="idProfile" runat="server" Text='<%# Bind("Tipologia") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:Label ID="LblIdDocumentogvDocumentInAUTHHeader" Text='TipoTransfer' runat="server"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="idProfile" runat="server" Text='<%# Bind("TipoTransfer") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
            <NttDL:CustomButton ID="btnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnClose_Click" OnClientClick="disallowOp('Content2')" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
