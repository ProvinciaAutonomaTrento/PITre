<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="TransferPolicyImpact.aspx.cs" Inherits="NttDataWA.Popup.TransferPolicyImpact" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <%--<div id="topContentPopupSearch">--%>
    <div id="topContentPopup">
    </div>
    <div id="centerContentAddressbook">
        <div id="contentTab">
            <div id="centerContentArchiveSx">
                <fieldset>
                    <!-- riga ID POLICY-->
                    <div class="row">
                        <div class="colHalf">
                            <strong>
                                <asp:Literal ID="litPolicyId" runat="server" /></strong></div>
                        <div class="col">
                            <NttDL:CustomTextArea ID="TxtPolicyId" runat="server" CssClass="txt_addressBookLeft"
                                CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false" Width="20%" />
                        </div>
                    </div>
                    <!-- riga DESCR POLICY-->
                    <div class="row">
                        <div class="colHalf">
                            <strong>
                                <asp:Literal ID="LitDescPolicy" runat="server" /></strong></div>
                        <div class="colHalf_arch">
                            <NttDL:CustomTextArea ID="TxtDescPolicy" runat="server" CssClass="txt_addressBookLeft"
                                CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false" />
                        </div>
                        <div class="col">
                        </div>
                    </div>
                    <div class="row">
                    </div>
                    <div class="row">
                        <fieldset>
                            <div class="row">
                                <strong>
                                    <asp:Literal ID="LitTipoAnalisi" runat="server" /></strong>
                            </div>
                            <asp:RadioButtonList ID="rblPolicyImpact" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblPolicyImpact_SelectedIndexChanged">
                                <asp:ListItem Text="% documenti trasferibili/copie" Selected="True" Value="1"></asp:ListItem>
                                <asp:ListItem Text="N. documenti acceduti negli ultimi 5 anni" Value="2"></asp:ListItem>
                                <asp:ListItem Text="N. documenti acceduti negli ultimi 12 mesi" Value="3"></asp:ListItem>
                                <asp:ListItem Text="Elenco documenti acceduti negli ultimi 12 mesi" Value="4"></asp:ListItem>
                            </asp:RadioButtonList>
                        </fieldset>
                    </div>
                </fieldset>
            </div>
            <div id="centerContentArchiveDx">
                <asp:UpdatePanel ID="upPnlChartPieGrid" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="pnlChartPieGrid" runat="server">
                            <fieldset>
                                <div class="row">
                                    <asp:Chart ID="ChartTransfer" Width="750" runat="server" EnableViewState="true" BackColor="255,237, 244, 248">
                                        <Titles>
                                            <asp:Title Font="Verdana,  11pt, style=Bold">
                                            </asp:Title>
                                        </Titles>
                                        <Series>
                                            <asp:Series Name="SerieTransfer" ChartType="Pie">
                                            </asp:Series>
                                        </Series>
                                        <Legends>
                                            <asp:Legend Title="Tipo trasferimento" />
                                        </Legends>
                                        <ChartAreas>
                                            <asp:ChartArea Name="ChartArea1" BackColor="255,237, 244, 248">
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
                        <asp:Panel ID="pnlNDocAcc5anni" runat="server" Visible="false" Height="400px">
                            <fieldset>
                                <asp:Panel ID="pnlErrore5Anni" runat="server" Visible="false">
                                    <div class="fieldChartErrorPolicy_arch">
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
                                        BackColor="255,237, 244, 248" Palette="SeaGreen">
                                        <Titles>
                                            <asp:Title Font="Times New Roman, 12pt, style=Bold">
                                            </asp:Title>
                                        </Titles>
                                        <Series>
                                            <asp:Series Name="SerieAcc5Anni" ChartType="Bar">
                                                <Points>
                                                    <asp:DataPoint Color="LightSeaGreen" />
                                                    <asp:DataPoint Color="SteelBlue" />
                                                    <asp:DataPoint Color="LightSlateGray" />
                                                    <asp:DataPoint Color="MediumSlateBlue" />
                                                    <asp:DataPoint Color="MediumSeaGreen" />
                                                </Points>
                                            </asp:Series>
                                        </Series>
                                        <Legends>
                                        </Legends>
                                        <ChartAreas>
                                            <asp:ChartArea Name="ChartArea2" Area3DStyle-Enable3D="true" BackColor="255,237, 244, 248">
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
                        <asp:Panel ID="pnlNDocAcc12mesi" runat="server" Visible="false" Height="400px">
                            <fieldset>
                                <asp:Panel ID="pnlErrore12mesi" runat="server" Visible="false">
                                    <div class="fieldChartErrorPolicy_arch">
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
                                        BackColor="255,237, 244, 248">
                                        <Titles>
                                            <asp:Title Font="Times New Roman, 12pt, style=Bold">
                                            </asp:Title>
                                        </Titles>
                                        <Series>
                                            <asp:Series Name="SerieAcc12Mesi" ChartType="Column">
                                                <Points>
                                                    <asp:DataPoint Color="LightSeaGreen" />
                                                    <asp:DataPoint Color="SteelBlue" />
                                                    <asp:DataPoint Color="LightSlateGray" />
                                                    <asp:DataPoint Color="MediumSlateBlue" />
                                                    <asp:DataPoint Color="MediumSeaGreen" />
                                                    <asp:DataPoint Color="LightGoldenrodYellow" />
                                                    <asp:DataPoint Color="MediumPurple" />
                                                    <asp:DataPoint Color="MediumTurquoise" />
                                                    <asp:DataPoint Color="MenuBar" />
                                                    <asp:DataPoint Color="LightSkyBlue" />
                                                    <asp:DataPoint Color="MediumOrchid" />
                                                    <asp:DataPoint Color="MediumVioletRed" />
                                                </Points>
                                            </asp:Series>
                                        </Series>
                                        <Legends>
                                        </Legends>
                                        <ChartAreas>
                                            <asp:ChartArea Name="ChartArea3" Area3DStyle-Enable3D="true" BackColor="255,237, 244, 248">
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
                        <asp:Panel ID="pnlGrdDoc12mesi" runat="server" Visible="false" Height="400px">
                            <fieldset>
                                <div class="row">
                                    <div class="col">
                                        <strong>
                                            <asp:Label ID="lblTitoloGrdDocPolicy" runat="server"></asp:Label></strong>
                                    </div>
                                </div>
                                <asp:GridView ID="GrdDoc" runat="server" Width="99%" CssClass="tbl_rounded_custom round_onlyextreme"
                                    BorderWidth="0" AllowPaging="true" OnPageIndexChanging="GrdDoc_PageIndexChanging"
                                    OnRowDataBound="GrdDoc_RowDataBound" PageSize="9" AutoGenerateColumns="false" DataKeyNames="Profile_ID">
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
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblId" runat="server" Text="Id"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblIdValue" runat="server" Text='<%#Bind("Profile_ID")%>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblOggetto" runat="server" Text="Oggetto"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblOggettoValue" runat="server" Text='<%#Bind("OggettoDocumento") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lbltipo" runat="server" Text="Tipologia"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LbltipoValue" runat="server" Text='<%#Bind("Tipologia") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblTipoDoc" runat="server" Text="TipoDoc"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblTipoDocValue" runat="server" Text='<%#Bind("TipoDocumento") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblDataCreazione" runat="server" Text="Data"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblDataCreazioneValue" runat="server" Text='<%#Bind("DataCreazione") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblAcc" runat="server" Text="Accessi"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblAccValue" runat="server" Text='<%#Bind("NAccessiUltimoAnno") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblUtenti" runat="server" Text="Utenti"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblUtentiValue" runat="server" Text='<%#Bind("NUtentiUltimoAnno") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:Label ID="lblCopia" runat="server" Text="Copia"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LblCopiaValue" runat="server" Text='<%#Bind("TipoTransferimento_Policy") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </fieldset>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <NttDL:CustomButton ID="btnPolicyImpactChiudi" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnPolicyImpactChiudi_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
