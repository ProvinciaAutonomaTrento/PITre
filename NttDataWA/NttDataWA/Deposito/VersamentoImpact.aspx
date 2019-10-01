<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="VersamentoImpact.aspx.cs" Inherits="NttDataWA.Deposito.VersamentoImpact" %>

<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ACT" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<%@ Register Src="~/UserControls/VersamentiTab.ascx" TagPrefix="uc2" TagName="HeaderVersamento" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.collapsed" });
        });
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <contenttemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="LitVersamentoImpProject" runat="server"></asp:Literal></p>
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </contenttemplate>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:HeaderVersamento ID="VersamentiTabs" runat="server" PageCaller="IMPACT" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                            <div class="row">
                            </div>
                        </div>
                    </div>
                </div>
                <div id="containerStandardTabDxBorder">
                </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="containerStandard2">
        <div id="contentSx">
            <asp:UpdatePanel runat="server" ID="upTextBoxTransferDetails" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="LitVersamentoId" runat="server" /></strong></div>
                            <div class="colHalf">
                                <NttDL:CustomTextArea ID="TxtIdVersamento" runat="server" CssClass="txt_addressBookLeft"
                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="False">
                                </NttDL:CustomTextArea>
                            </div>
                        </div>
                        <div class="row">
                        </div>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="LitVersamentoDescrizione" runat="server" /></strong></div>
                            <div class="colHalf2">
                                <NttDL:CustomTextArea ID="TxtDescrizioneVersamento" runat="server" CssClass="txt_input_full"
                                    CssClassReadOnly="txt_input_full_disabled" Enabled="false">
                                </NttDL:CustomTextArea>
                            </div>
                        </div>
                        <div class="row">
                        </div>
                        <div class="row">
                            <div class="colHalf">
                                <strong>
                                    <asp:Literal ID="LitVersamentoNote" runat="server" /></strong></div>
                            <div class="colHalf2">
                                <NttDL:CustomTextArea ID="TxtNoteVersamento" runat="server" CssClass="txt_textarea"
                                    CssClassReadOnly="txt_textarea_disabled" Enabled="false">
                                </NttDL:CustomTextArea>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="upPnl" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                        <div class="row">
                            <strong>
                                <asp:Literal ID="LitTipoAnalisi" runat="server" /></strong>
                        </div>
                        <div class="row">
                            <asp:RadioButtonList ID="rblTransferImpact" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblTransferImpact_SelectedIndexChanged">
                                <asp:ListItem Text="% documenti trasferibili/copie" Selected="True" Value="1"></asp:ListItem>
                                <asp:ListItem Text="N. documenti acceduti negli ultimi 5 anni" Value="2"></asp:ListItem>
                                <asp:ListItem Text="N. documenti acceduti negli ultimi 12 mesi" Value="3"></asp:ListItem>
                                <asp:ListItem Text="Elenco documenti acceduti negli ultimi 12 mesi" Value="4"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="contentDx">
            <asp:UpdatePanel ID="upPnlChartPieGrid" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnlChartPieGrid" runat="server">
                        <fieldset>
                            <div class="row">
                                <asp:Chart ID="ChartTransfer" Width="750" runat="server" EnableViewState="true" BackColor="255,238, 238, 238">
                                    <Titles>
                                        <asp:Title Font="Verdana,  11pt, style=Bold">
                                        </asp:Title>
                                    </Titles>
                                    <Series>
                                        <asp:Series Name="SerieTransfer" ChartType="Pie">
                                        </asp:Series>
                                    </Series>
                                    <Legends>
                                        <asp:Legend Title="Tipo trasferimento" TitleFont="Verdana,  11pt, style=Bold" />
                                    </Legends>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" BackColor="255,238, 238, 238">
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </div>
                            <div class="row">
                                <asp:Panel ID="pnlGrid" runat="server">
                                    <asp:GridView ID="GrdVincoli" runat="server" Width="99%" CssClass="tbl_rounded_custom round_onlyextreme"
                                        BorderWidth="0" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblVincolo" runat="server" Text="Tipo Vincolo"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblVincoloValue" runat="server" Text='<%#Bind("Vincolo") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblPercent" runat="server" Text="% documenti"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblPercentValue" runat="server" Text='<%#DataBinder.Eval(Container.DataItem ,"Percentuale") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </div>
                        </fieldset>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="upPnlNDocAcc5anni" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnlNDocAcc5anni" runat="server" Visible="false">
                        <fieldset>
                            <asp:Panel ID="pnlErrore5Anni" runat="server" Visible="false">
                                <div class="fieldChartError_arch">
                                    <div style="text-align: center;">
                                        <strong>
                                            <asp:Label ID="Label1" Text='ATTENZIONE' runat="server" Visible="true"></asp:Label>
                                        </strong>
                                    </div>
                                    <div style="text-align: center;">
                                        <asp:Label ID="Label4" Text="Non ci sono dati per popolare il report" runat="server"
                                            Visible="true"></asp:Label>
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="pnlChart5Anni" runat="server" Visible="false">
                                <asp:Chart ID="ChartAcc5Anni" Width="750" Height="350" runat="server" EnableViewState="true"
                                    BackColor="255,238, 238, 238" Palette="SeaGreen">
                                    <Titles>
                                        <asp:Title>
                                        </asp:Title>
                                    </Titles>
                                    <Series>
                                        <asp:Series Name="SerieAcc5Anni" ChartType="Bar">
                                            <Points>
                                            </Points>
                                        </asp:Series>
                                    </Series>
                                    <Legends>
                                    </Legends>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea2" Area3DStyle-Enable3D="true" BackColor="255,238, 238, 238">
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </asp:Panel>
                        </fieldset>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="upPnlNDocAcc12mesi" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnlNDocAcc12mesi" runat="server" Visible="false">
                        <fieldset>
                            <asp:Panel ID="pnlErrore12mesi" runat="server" Visible="false">
                                <div class="fieldChartError_arch">
                                    <div style="text-align: center;">
                                        <strong>
                                            <asp:Label ID="Label2" Text='ATTENZIONE' runat="server" Visible="true"></asp:Label>
                                        </strong>
                                    </div>
                                    <div style="text-align: center;">
                                        <asp:Label ID="Label3" Text="Non ci sono dati per popolare il report" runat="server"
                                            Visible="true"></asp:Label>
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="pnlChart12mesi" runat="server" Visible="false">
                                <asp:Chart ID="ChartAcc12Mesi" Width="750" Height="350" runat="server" EnableViewState="true"
                                    BackColor="255,238, 238, 238">
                                    <Titles>
                                        <asp:Title Font="Times New Roman, 12pt, style=Bold">
                                        </asp:Title>
                                    </Titles>
                                    <Series>
                                        <asp:Series Name="SerieAcc12Mesi" ChartType="Column" Palette="BrightPastel">
                                            <Points>
                                            </Points>
                                        </asp:Series>
                                    </Series>
                                    <Legends>
                                    </Legends>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea3" Area3DStyle-Enable3D="true" BackColor="255,238, 238, 238">
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </asp:Panel>
                        </fieldset>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="upPnlGrdDoc12mesi" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnlGrdDoc12mesi" runat="server" Visible="false">
                        <fieldset>
                            <div class="row">
                                <div class="col">
                                <strong>
                                    <asp:Label ID="lblTitoloGrdDoc" runat="server"></asp:Label></strong>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="true" OnCheckedChanged="addAll_Click" />
                                    <asp:Label ID="lblSelectAll" runat="server"></asp:Label>
                                </div>
                                <div class="col-right">
                                    <NttDL:CustomButton ID="btnConferma" runat="server" CssClass="buttonAbort" CssClassDisabled="buttonAbortDisable"
                                        OnMouseOver="buttonAbortHover" OnClick="btnConferma_Click" Enabled="false" />
                                </div>
                                <div class="row">
                                </div>
                                <div class="row">
                                    <asp:GridView ID="GrdDoc" runat="server" Width="99%" CssClass="tbl_rounded_custom round_onlyextreme"
                                        BorderWidth="0" AllowPaging="true" OnPageIndexChanging="GrdDoc_PageIndexChanging"
                                        DataKeyNames="Profile_ID" OnRowDataBound="GrdDoc_RowDataBound" PageSize="9" AutoGenerateColumns="false">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <SelectedRowStyle CssClass="SelectedRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblAOO" runat="server" Text="AOO"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblAOOValue" runat="server" Text='<%#Bind("Registro") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblId" runat="server" Text="Id"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblIdValue" runat="server" Text='<%#Bind("Profile_ID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblOggetto" runat="server" Text="Oggetto"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblOggettoValue" runat="server" Text='<%#Bind("OggettoDocumento") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lbltipo" runat="server" Text="Tipologia"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LbltipoValue" runat="server" Text='<%#Bind("Tipologia") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblTipoDoc" runat="server" Text="TipoDoc"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblTipoDocValue" runat="server" Text='<%#Bind("TipoDocumento") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblDataCreazione" runat="server" Text="Data"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblDataCreazioneValue" runat="server" Text='<%#Bind("DataCreazione") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblAcc" runat="server" Text="Accessi"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblAccValue" runat="server" Text='<%#Bind("NAccessiUltimoAnno") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblUtenti" runat="server" Text="Utenti"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblUtentiValue" runat="server" Text='<%#Bind("NUtentiUltimoAnno") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblCopia" runat="server" Text="Copia"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkCopia" runat="server" AutoPostBack="true" OnCheckedChanged="chkCopia_CheckedChange" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                        </fieldset>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="UpTranfetButtons">
        <ContentTemplate>
            <NttDL:CustomButton ID="btnVersamentoNuovo" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Enabled="false" />
            <NttDL:CustomButton ID="btnVersamentoAnalizza" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" ClientIDMode="Static" />
            <NttDL:CustomButton ID="btnVersamentoProponi" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" ClientIDMode="Static" />
            <NttDL:CustomButton ID="btnVersamentoApprova" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" ClientIDMode="Static" />
            <NttDL:CustomButton ID="btnVersamentoEsegui" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" ClientIDMode="Static" />
            <NttDL:CustomButton ID="btnVersamentoModifica" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" ClientIDMode="Static" />
            <NttDL:CustomButton ID="btnVersamentoElimina" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Enabled="false" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
