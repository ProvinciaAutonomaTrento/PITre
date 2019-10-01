<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="Registers.aspx.cs" Inherits="NttDataWA.Management.Registers" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #containerDetailsRegister
        {
            text-align: left;
            margin: 5px 0 0 0;
            padding: 5px 0 0 0;
            font-size: 0.9em;
            border-top: 1px solid #ccc;
        }
        .rf
        {
            float: left;
            margin-left: 16px;
            margin-top: -12px;
            padding-left: 10px;
            padding-top: 23px;
            border-bottom: 1px solid #ccc;
            border-left: 1px solid #ccc;
            position: absolute;
        }
        .tbl_rounded
        {
            border-left:1px solid #d4d4d4;
            border-right:1px solid #d4d4d4;
            border-bottom:1px solid #d4d4d4;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="RegisterModify" runat="server" Url="../popup/RegisterModify.aspx"
        Width="400" Height="350" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup Id="ViewPrintRegister" runat="server" Url="../popup/ViewPrintRegister.aspx"
        Width="400" Height="350" IsFullScreen="True" PermitClose="false" PermitScroll="true"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup Id="CheckMailboxReport" runat="server" Url="../popup/CheckMailboxReport.aspx"
        Width="1200" Height="1000" IsFullScreen="False" PermitClose="false" PermitScroll="true" ForceDontClose="true"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', 'closeCheckMailReport');}" />
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
                                        <asp:Label ID="RegistersManagementRegisters" runat="server" />
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
                        <asp:UpdatePanel ID="panelRegisters" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <%--<asp:Timer runat="server" id="TimerUpdateMailbox" Interval="10000" OnTick="Timer_Tick" Enabled="false"></asp:Timer>--%>
                                <asp:GridView ID="GrdRegisters" runat="server" Width="97%" AutoGenerateColumns="false"
                                    AllowPaging="True" PageSize="7" CssClass="tbl_rounded round_onlyextreme" BorderWidth="0"
                                    OnPageIndexChanging="GrdRegisters_PageIndexChanging" OnRowDataBound="GrdRegisters_RowDataBound"
                                    Style="cursor: pointer;">
                                    <RowStyle CssClass="NormalRow" Height="50" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:RegistersGrdCode%>' HeaderStyle-Width="10%">
                                            <ItemTemplate>
                                                <div style="margin: 0; padding-left: 5px">
                                                    <div id="rf" class="rf" runat="server" visible="false"></div>
                                                    <asp:Image ID="btnImageRegister" runat="server" ImageUrl="<%#this.GetImage((NttDataWA.DocsPaWR.Registro) Container.DataItem)%>"
                                                        ImageAlign="Middle" />
                                                    <asp:Label runat="server" ID="lblCodiceRegistro" Text='<%# Bind("codRegistro") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:RegistersGrdDescription%>' HeaderStyle-Width="350px">
                                            <ItemTemplate>
                                                <div style="margin-left: 3px">
                                                    <asp:Label runat="server" ID="lblDescrizione" Text='<%# Bind("descrizione") %>' />
                                                    <asp:Panel ID="pnlDetails" runat="server" Visible="false">
                                                        <p>
                                                            <div id="containerDetailsRegister" style="margin: 0;">
                                                                <span style="font-size: 0.8em"><span class="weight">
                                                                    <asp:Label ID="lblRegisterOpeningDate" runat="server" Text='<%$ localizeByText:RegistersGrdOpeningDate%>' />
                                                                </span>
                                                                    <%# NttDataWA.Utils.dateformat.dateLength(Eval("dataApertura").ToString())%><br />
                                                                    <span class="weight">
                                                                        <asp:Label ID="lblRegisterClosingDate" runat="server" Text='<%$ localizeByText:RegistersGrdClosingDate%>' />
                                                                    </span>
                                                                    <%# NttDataWA.Utils.dateformat.dateLength(Eval("dataChiusura").ToString())%><br />
                                                                    <span class="weight">
                                                                        <asp:Label ID="lblRegisterDateLastProtocol" runat="server" Text='<%$ localizeByText:RegistersGrdDateLastProtocol%>' />
                                                                    </span>
                                                                    <%# NttDataWA.Utils.dateformat.dateLength(Eval("dataUltimoProtocollo").ToString())%><br />
                                                                    <span class="weight">
                                                                        <asp:Label ID="lblRegisterDateNextProtocol" runat="server" Text='<%$ localizeByText:RegistersGrdDateNextProtocol%>' />
                                                                    </span>
                                                                    <asp:Label runat="server" ID="lblDateNextProtocol2" Text='<%# Bind("ultimoNumeroProtocollo") %>' />
                                                                </span>
                                                            </div>
                                                        </p>
                                                    </asp:Panel>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText='<%$ localizeByText:RegistersGrdState%>'
                                            HeaderStyle-Width="7%" ItemStyle-Wrap="false">
                                            <ItemTemplate>
                                                <div style="margin: 0; padding-left: 5px">
                                                    <asp:Image ID="btnImageState" runat="server" ImageUrl="<%#this.GetImageState((NttDataWA.DocsPaWR.Registro) Container.DataItem)%>"
                                                        ImageAlign="Middle" />
                                                    <asp:Label runat="server" ID="lblRegistarState" Text='<%#this.GetLabelRegistarState((NttDataWA.DocsPaWR.Registro) Container.DataItem) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%" ItemStyle-Width="20%"
                                            HeaderText='<%$ localizeByText:RegistersGrdEmail%>'>
                                            <ItemTemplate>
                                                <div class="row">
                                                    <div class="col-full">
                                                        <p>
                                                            <asp:DropDownList ID="DdlEmailRegister" runat="server" CssClass="chzn-select-deselect"
                                                                Width="250px" OnSelectedIndexChanged="ddlEmailRegister_SelectedIndexChanged"
                                                                AutoPostBack="true">
                                                            </asp:DropDownList>
                                                        </p>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30%" HeaderText='<%$ localizeByText:RegistersGrdVerifyEmail%>'
                                            HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Panel ID="upPnlVerifyEmail" runat="server" ClientIDMode="Static">
                                                    <div id="containerVerifyEmail" runat="server" visible="false">
                                                        <div id="containerProgressBar" runat="server" visible="false">
                                                            <asp:Panel ID="progressBar" runat="server" Height="25px" Width="300px" Style="margin-top: 5px;
                                                                margin-left: 35px; background: url(../Images/Uploader/progress-deactive.png) repeat-x top left;">
                                                                <asp:Panel runat="server" ID="progressBarInternal" Height="25px" Style="background: url(../Images/Uploader/progress-active.png) repeat-x top left;
                                                                    position: relative;">
                                                                </asp:Panel>
                                                            </asp:Panel>
                                                            <div id="progressBarInfoMailProcessed" runat="server" style="text-align: center">
                                                                <div style="margin-left: 35px; float: left">
                                                                    <asp:Label Style="text-align: center; font-size: 0.8em" runat="server" ID="lblNumberMailProcessed" />
                                                                </div>
                                                                <div style="margin-right: 35px; float: right">
                                                                    <asp:Label Style="text-align: center; font-size: 0.8em" runat="server" ID="lblNumberMailProcessedDate" />
                                                                </div>
                                                            </div>
                                                            <div id="progressBarWait" runat="server" style="text-align: center" visible="false">
                                                                <span class="weight">
                                                                    <asp:Label runat="server" ID="lblWait" Style="font-size: 0.8em"  />
                                                                </span>
                                                            </div>
                                                        </div>
                                                        <div id="viewLinkReport" runat="server" style="text-align: center" visible="false">
                                                            <asp:HiddenField ID="idCheckMailbox" runat="server" ClientIDMode="Static" />
                                                            <asp:LinkButton ID="BtnViewReport" OnClientClick="return ajaxModalPopupCheckMailboxReport()"
                                                                runat="server" Text='<%$ localizeByText:RegistersViewReport%>'></asp:LinkButton>
                                                        </div>
                                                        <asp:HiddenField ID="checkMailbox" runat="server" ClientIDMode="Static" />
                                                    </div>
                                                </asp:Panel>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="RegistersBtnBox" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="RegistersBtnBox_Click" />
            <cc1:CustomButton ID="RegistersBtnUpdateStateBox" runat="server" CssClass="btnEnable"
                Enabled="false" CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="RegistersBtnUpdateStateBox_Click" />
            <cc1:CustomButton ID="RegistersBtnChangesState" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="RegistersBtnChangesState_Click" />
            <cc1:CustomButton ID="RegistersBtnModify" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="RegistersBtnRegisterModify_Click" />
            <cc1:CustomButton ID="RegistersBtnPrint" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="RegistersBtnPrint_Click" />
            <asp:HiddenField runat="server" ID="HiddenVerifyBox" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
