<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Mandate.aspx.cs" Inherits="NttDataWA.Mandate.Mandate"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .gridViewResult th
        {
            text-align: center;
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
        .row
        {
            min-height: 25px;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function SingleSelect(regex, current) {
            re = new RegExp(regex);
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];
                if (elm.type == 'radio' && elm != current && re.test(elm.name)) {
                    elm.checked = false;
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
     <uc:ajaxpopup2 Id="NewMandate" runat="server" Url="../Popup/NewEditMandate.aspx" IsFullScreen="true" 
        PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpContainerProjectTab', '');}" />
    <uc:ajaxpopup2 Id="EditMandate" runat="server" Url="../Popup/NewEditMandate.aspx" IsFullScreen="true" 
        PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpContainerProjectTab', '');}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx"></div>
                                <div id="containerStandardTopCx">
                                    <p><asp:Literal ID="MandateLitMandate" runat="server"></asp:Literal></p>
                                </div>
                                <div id="containerStandardTopDx"></div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom"></div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                            <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                                    <ul>
                                        <li id="MandateLiReceived" runat="server" class="prjIAmProfile">
                                            <asp:LinkButton ID="MandateLinkReceived" runat="server" OnClick="MandateLinkReceived_OnClick"></asp:LinkButton></li>
                                        <li id="MandateLiAssigned" runat="server" class="prjOther">
                                            <asp:LinkButton ID="MandateLinkAssigned" runat="server" OnClick="MandateLinkAssigned_OnClick"></asp:LinkButton></li>
                                    </ul>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <fieldset>
                                <div class="row">
                                    <div class="col">
                                        <p>
                                            <span class="weight">
                                                <asp:Literal runat="server" ID="MandateLitState"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                </div>
                                <asp:UpdatePanel runat="server" ID="UpPnlState" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="MandateDdlState" runat="server" CssClass="chzn-select-deselect"
                                                    Width="100%">
                                                    <asp:ListItem id="optActive" Value="A" />
                                                    <asp:ListItem id="optSet" Value="I" />
                                                    <asp:ListItem id="optExpire" Value="S" />
                                                    <asp:ListItem id="optAll" value="T" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpNameDelegate" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="MandateLitName"></asp:Literal>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <cc1:CustomTextArea ID="TxtName" runat="server" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled"
                                                    ClientIDMode="Static">
                                                </cc1:CustomTextArea>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpPnlRoleSelect" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder runat="server" ID="PlcRoleSelect" Visible="false">
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="MandateLitRole"></asp:Literal>
                                                        </span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-full">
                                                    <asp:DropDownList ID="MandateDdlRole" runat="server" CssClass="chzn-select-deselect"
                                                        Width="100%">
                                                        <asp:ListItem id="optAllM"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </div>
                    </div>
                    <div id="contentDx">
                        <div id="contentDxTopProject">
                            <asp:UpdatePanel runat="server" ID="UpPnlMessage" UpdateMode="Conditional" class="row">
                                <ContentTemplate>
                                    <asp:Literal ID="lbl_messaggio" runat="server"></asp:Literal>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="col-full">
                                    <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                                        <ContentTemplate>
                                            <asp:GridView ID="gridViewResult" runat="server" Width="99%" AllowPaging="false" PageSize="10"
                                                AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                                HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbSel" runat="server" AutoPostBack="true" OnCheckedChanged="cbSel_CheckedChanged" />
                                                            <asp:RadioButton ID="rbSel" runat="server" AutoPostBack="true" Visible="false" onclick="SingleSelect('rbS',this);"
                                                                OnCheckedChanged="rbSel_CheckedChanged" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="dataDecorrenza" ItemStyle-Width="5%"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField DataField="dataScadenza" ItemStyle-Width="5%"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField DataField="id_utente_delegato" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="cod_utente_delegato" ItemStyle-Width="20%"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="id_ruolo_delegato" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="cod_ruolo_delegato" ItemStyle-Width="20%"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="id_utente_delegante" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="cod_utente_delegante" ItemStyle-Width="20%"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="id_ruolo_delegante" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="cod_ruolo_delegante" ItemStyle-Width="20%"
                                                        ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="id_delega" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="id_uo_delegato" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="inEsercizio" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="id_people_corr_globali" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" ReadOnly="true"
                                                        HeaderStyle-HorizontalAlign="Left" />
                                                    <asp:TemplateField ItemStyle-Width="5%">
                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:Label ID="stato" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:PlaceHolder ID="plcNavigator" runat="server" />
                                            <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="MandateBtnSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="MandateBtnSearch_Click" />
            <cc1:CustomButton ID="MandateBtnExecute" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="MandateBtnExecute_Click" />
            <cc1:CustomButton ID="MandateBtnNew" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Visible="false" OnClick="MandateBtnNew_Click" />
            <cc1:CustomButton ID="MandateBtnDelete" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Visible="false" OnClick="MandateBtnDelete_Click" />
            <cc1:CustomButton ID="MandateBtnEdit" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Visible="false" OnClick="MandateBtnEdit_Click" />

            <asp:HiddenField ID="proceed_delete" runat="server" ClientIDMode="Static" />
            <asp:Button ID="btnChangePage" runat="server" ClientIDMode="Static" CssClass="hidden" OnClick="btnChangePage_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
