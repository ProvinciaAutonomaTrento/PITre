<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="ConservationArea.aspx.cs" Inherits="NttDataWA.Management.ConservationArea" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .tbl_rounded
        {
            width: 97%;
            border-collapse: collapse;
        }
        .tbl_rounded td
        {
            background: #fff;
            min-height: 1em;
            border: 1px solid #d4d4d4;
            border-top: 0;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="DocumentViewer" runat="server" Url="../Popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closeZoom');}" />
    <uc:ajaxpopup Id="ViewDetailsInstanceConservation" runat="server" Url="../Popup/ViewDetailsInstanceConservation.aspx"
        PermitClose="false" Width="1200" Height="800" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupViewDetailsInstanceConservation');}" />
    <div id="containerTop">
        <div id="containerDocumentTop">
            <div id="containerStandardTop">
                <div id="containerStandardTopSx">
                </div>
                <div id="containerStandardTopCx">
                    <p>
                        <asp:Label ID="ConservationAreaLblListInstancesOfConservation" runat="server" />
                    </p>
                </div>
                <div id="containerStandardTopDx">
                </div>
            </div>
            <div id="containerStandardBottom">
                <div id="containerProjectCxBottom">
                </div>
            </div>
        </div>
    </div>
    <div id="containerStandard">
        <div id="content">
            <div id="contentStandard1Column" style="padding-left: 15px">
                <div id="containerCbxIstances" runat="server" style="font-weight: bold;">
                    <div class="col13">
                        <asp:CheckBox ID="cbInstancesClose" CssClass="clickableLeftN" runat="server" />
                    </div>
                    <div class="col13">
                        <asp:CheckBox ID="cbInstancesAutomatic" CssClass="clickableLeftN" runat="server" />
                    </div>
                    <div class="col13">
                        <asp:CheckBox ID="cbInstancesManuals" CssClass="clickableLeftN" runat="server" />
                    </div>
                </div>
                <div id="containerGrdInstancesOfCoservation" runat="server" style="padding-top: 35px">
                    <asp:UpdatePanel ID="UpPanelGridInstancesConservation" runat="server" UpdateMode="Conditional"
                        ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:GridView ID="GrdInstancesConservation" runat="server" Width="97%" AutoGenerateColumns="false"
                                AllowPaging="True" PageSize="7" CssClass="tbl_rounded round_onlyextreme" BorderWidth="0"
                                OnRowCommand="GrdInstancesConservation_RowCommand" OnPageIndexChanging="GrdInstancesConservation_PageIndexChanging"
                                OnRowDataBound="GrdInstancesConservation_RowDataBound" Style="cursor: pointer;"
                                ShowHeaderWhenEmpty="true">
                                <RowStyle CssClass="NormalRow" />
                                <AlternatingRowStyle CssClass="AltRow" />
                                <PagerStyle CssClass="recordNavigator2" />
                                <Columns>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaInstanceId%>' Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="instanceId" runat="server" Text='<%# Bind("SystemID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaInstanceId%>'>
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_instanceId" runat="server" Text='<%#this.GetLabelInstanceId((NttDataWA.DocsPaWR.InfoConservazione) Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaConservationState%>'>
                                        <ItemTemplate>
                                            <asp:Label ID="conservationState" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label ID="headerCheckResult" runat="server" Text="Esito Verifica"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="checkResult" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaNote%>'>
                                        <ItemTemplate>
                                            <asp:Label ID="note" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaDescription%>'>
                                        <ItemTemplate>
                                            <asp:Label ID="description" runat="server" Text='<%# Bind("Descrizione") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Data_Apertura" HeaderText='<%$ localizeByText:ConservationAreaOpenDate%>'
                                        ItemStyle-Width="10px" />
                                    <asp:BoundField DataField="Data_Invio" HeaderText='<%$ localizeByText:ConservationAreaSendingDate%>'
                                        ItemStyle-Width="10px" />
                                    <asp:BoundField DataField="Data_Conservazione" HeaderText='<%$ localizeByText:ConservationAreaConservationDate%>'
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaTypeConservation%>'
                                        ItemStyle-Width="10px">
                                        <ItemTemplate>
                                            <asp:Label ID="typeConservation" runat="server" Text='<%#this.GetLabelTypeConservation((NttDataWA.DocsPaWR.InfoConservazione) Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="noteRifiuto" HeaderText='<%$ localizeByText:ConservationAreaNoteRejection%>'
                                        ItemStyle-Width="130px" />
                                    <asp:BoundField DataField="automatica" HeaderText='<%$ localizeByText:ConservationAreaAutomatic%>'
                                        ItemStyle-Width="35px" ItemStyle-HorizontalAlign="center" />
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaDetails%>' ItemStyle-Width="50px"
                                        ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <cc1:CustomImageButton ID="ConservationAreaDetailsInstance" CommandName="ViewDetailsInstance"
                                                runat="server" AllignImage="Center" ImageUrl="../Images/Icons/ico_previous_details.png"
                                                OnMouseOutImage="../Images/Icons/ico_previous_details.png" ToolTip='<%$ localizeByText:ConservationAreaDetailsToolTip%>'
                                                OnMouseOverImage="../Images/Icons/ico_previous_details_hover.png" CssClass="clickable"
                                                ImageUrlDisabled="../Images/Icons/ico_previous_details_disabled.png" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:ConservationAreaRemove%>' ItemStyle-Width="50px"
                                        ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <cc1:CustomImageButton ID="ConservationAreaRemoveInstance" CommandName="RemoveInstance"
                                                runat="server" ImageUrl="../Images/Icons/delete2.png" OnMouseOutImage="../Images/Icons/delete2.png"
                                                ToolTip='<%$ localizeByText:ConservationAreaRemoveToolTip%>' OnMouseOverImage="../Images/Icons/delete2_hover.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/delete2_disabled.png" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="HiddenRemoveInstanceConservation" runat="server" ClientIDMode="Static" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="ConservationAreaSearch" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="ConservationAreaSearch_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
