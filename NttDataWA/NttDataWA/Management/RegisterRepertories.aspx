<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="RegisterRepertories.aspx.cs" Inherits="NttDataWA.Management.RegisterRepertories" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>

<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">

        .tbl_rounded
        {
            border-left:1px solid #d4d4d4;
            border-right:1px solid #d4d4d4;
            border-bottom:1px solid #d4d4d4;
        }
        
        .divRegRepertoriesPrint fieldset
        {
            border: 1px solid #FF4500;
            margin-top:5px;
            margin-bottom:10px;
            width: 96%;
            border-radius: 8px;
            -ms-border-radius: 8px; /* ie */
            -moz-border-radius: 8px; /* firefox */
            -webkit-border-radius: 8px; /* safari, chrome */
            -o-border-radius: 8px; /* opera */
        }
        
    </style>
</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
            <asp:Label ID="RegisterRepertorieslbl" runat="server"/>
       </p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
       
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard">
                <div id="content">
                    <div id="contentStandard1Column">
    <asp:UpdatePanel ID="panelRegistersRepertories" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
             <asp:GridView ID="GrdRegisterRepertories" runat="server"  Width="97%" style="cursor:pointer;" AutoGenerateColumns="false" CssClass="tbl_rounded round_onlyextreme" BorderWidth="0" 
                     OnRowDataBound="GrdRegisterRepertories_RowDataBound" AllowPaging="True" PageSize = "7" OnPageIndexChanging="GrdRegisterRepertories_PageIndexChanging">
                    <RowStyle CssClass="NormalRow" Height="40" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" Height="30px" />
                    <Columns>
                        <asp:TemplateField HeaderText ='<%$ localizeByText:RegisterRepertoriesRegister%>' ItemStyle-Width="25%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblRegister" Text='<%# Bind("RegistroRepertorio.TipologyDescription") %>' />
                            </ItemTemplate>
                         </asp:TemplateField>
                        <asp:TemplateField HeaderText ='<%$ localizeByText:RegisterRepertoriesAoo/RF%>' ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblAooRf" Text='<%# Bind("RegistroRepertorioSingleSettings.RegistryOrRfDescription") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText ='<%$ localizeByText:RegisterRepertoriesState%>' ItemStyle-Width="10%" ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Image ID="btnImageState" runat="server" ImageUrl="<%#this.GetImageState((ColsRegisterRepertories) Container.DataItem)%>" ImageAlign="Middle"/>
                                <asp:Label runat="server" ID="lblRegistarReperoryState" Text='<%#this.GetLabelState((ColsRegisterRepertories) Container.DataItem) %>' Width="70%" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText ='<%$ localizeByText:RegisterRepertoriesResponsible%>' ItemStyle-Width="30%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblRegRepertoryResponsible" Text='<%# Bind("RegistroRepertorioSingleSettings.RoleAndUserDescription") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText ='<%$ localizeByText:RegisterRepertoriesDateLastPrint%>' ItemStyle-Width="15%" HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDateLastPrint" Text='<%#this.GetLabelDateLastPrint((ColsRegisterRepertories) Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
            </asp:GridView>
        <div id="divRegRepertoriesPrint" class="divRegRepertoriesPrint" runat="server" align="center">
                <fieldset>
                    <div style="float:left; margin-top:0; margin-bottom:0; margin-left:5%;">
                        <asp:Image ID="ImgWarning1" runat="server" ImageUrl="../Images/Common/messager_warning.png" />
                    </div>
                    <div style=" float:left; margin-top:5px; margin-left:10%; margin-right:10%;">
                            <asp:Panel ID="panelRegRepertoriesPrint" runat="server" HorizontalAlign="Left">
                                    <span class="weight" ID="RegisterRepertoriesPrintTxt" runat="server" style=" color:#FF4500;"></span>
                                    <asp:BulletedList ID="blDocList" runat="server" Width="98%" ></asp:BulletedList>
                            </asp:Panel>
                    </div>
                    <div style="float:right; margin-top:0; margin-bottom:0; margin-right:5%">
                        <asp:Image ID="ImgWarning2" runat="server" ImageUrl="../Images/Common/messager_warning.png" />
                    </div>
                </fieldset>
             </div>

            <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" /> 
        </ContentTemplate>
    </asp:UpdatePanel>   
 
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="RegisterRepertoriesBtnPrint" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="RegisterRepertoriesBtnPrint_Click" />
            <cc1:CustomButton ID="RegisterRepertoriesBtnChangesState" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="RegisterRepertoriesBtnChangesState_Click" />   
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
