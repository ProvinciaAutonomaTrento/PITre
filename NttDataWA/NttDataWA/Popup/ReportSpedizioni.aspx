<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="ReportSpedizioni.aspx.cs" Inherits="NttDataWA.Popup.ReportSpedizioni" %>
<%@ Register src="../UserControls/Correspondent.ascx" tagname="Correspondent" tagprefix="uc2" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .chzn-container-single
        {
            margin-top: 5px;
        }
    </style>

<script type="text/javascript">
    $("[src*=collapsed]").live("click", function () {
       
        $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
        $(this).attr("src", "../Images/Icons/expanded.png");
    });
    $("[src*=expanded]").live("click", function () {
        $(this).attr("src", "../Images/Icons/collapsed.png");
        $(this).closest("tr").next().remove();
    });

    $("[src*=massive_add_in_project]").live("click", function () {
        espanditutti();
    });

    $("[src*=project_hover]").live("click", function () {
        chiuditutti();
    });
    
</script>

<script type="text/javascript">
    $("#<%=ckbTipoRicevutaTutti.ClientID %>").click(function () {
        $("#<%=ckbTipoRicevuta.ClientID %> input").each(function () {
            if (!this.checked) { Alert("aaaa"); }
        }
        );
    });

</script>
<script type="text/javascript">
    function espanditutti() {
        $("[src*=collapsed]").each(function () {
            $(this).click();
            //            $(this).attr("src", "../Images/Icons/expanded.png");
            //            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>");


        });
    }
    </script>
<script type="text/javascript">
    function chiuditutti() {
        $("[src*=expanded]").each(function () {
            $(this).attr("src", "../Images/Icons/collapsed.png");
            $(this).closest("tr").next().remove();
        });
    }
</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">

    <uc:ajaxpopup2 Id="ReportGenerator" runat="server" Url="../popup/ReportGenerator.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="745" Height="400"
        CloseFunction="function (event, ui)  {__doPostBack('UpSend','');}" />
    <uc:messager id="messager" runat="server" />  

    <%--PANNELLI FILTRI--%>

    <div class="row">
   <fieldset class="filterAddressbook" style="width:99%;">

     <div class="col" style="width:70%;">
        <%-- FILTRO TIPO RICEVUTA--%> 
            <asp:Panel ID="filtristd" runat="server">

                <asp:UpdatePanel ID="updPnlFiltriStd" runat="server" >
                <ContentTemplate>
                        <div class="row">
                        <div class="col"><span class="weight"><asp:Literal ID="LtlTipoRicevuta" runat="server" Text="Tipo Ricevuta"></asp:Literal></span></div>
                    </div>
                    <div class="row">
                        <div class="col">
                        <asp:CheckBox ID="ckbTipoRicevutaTutti" runat="server" AutoPostBack="true" 
                                Text ="Seleziona/Deseleziona Tutti" 
                                oncheckedchanged="ckbTipoRicevutaTutti_CheckedChanged"/>
                        <%--<asp:CheckBoxList ID="ckbTipoRicevutaTutti" runat="server" RepeatColumns="1" RepeatDirection="Horizontal">
                            <asp:ListItem Value="TUTTI" Text ="Seleziona/Deseleziona Tutti" ></asp:ListItem>
                        </asp:CheckBoxList> --%>
                        </div>
                    </div>
                        <div class="row">
                        <asp:CheckBoxList ID="ckbTipoRicevuta" runat="server" RepeatColumns="4" 
                                RepeatDirection="Horizontal" CssClass="tbl_rounded">
                            <asp:ListItem Value="ACCETTAZIONE" Text ="Accettazione" ></asp:ListItem>
                            <asp:ListItem Value="AVVENUTA_CONSEGNA" Text = "Avvenuta Consegna"></asp:ListItem>
                            <asp:ListItem Value="MANCATA_ACCETTAZIONE" Text="Mancata Accettazione"></asp:ListItem>
                            <asp:ListItem Value="MANCATA_CONSEGNA" Text="Mancata Consegna"></asp:ListItem>
                            <asp:ListItem Value="CONFERMA_RICEZIONE" Text="Conferma Ricezione"></asp:ListItem>
                            <asp:ListItem Value="ANNULLA_PROTOCOLLAZIONE" Text="Annullamento Protocollazione"></asp:ListItem>
                            <asp:ListItem Value="ECCEZZIONI" Text="Eccezioni"></asp:ListItem>
                                <asp:ListItem Value="CONERRORI" Text="Con Errori"></asp:ListItem>
                                </asp:CheckBoxList>
                    </div>
                    </ContentTemplate>
                    </asp:UpdatePanel> 

            </asp:Panel>
    </div>
    <div class="col">
        <%--FILTRO ESITO--%>
        <asp:Panel ID="filtriEsito" runat="server">
        <asp:UpdatePanel ID="updPnlFiltriEsito" runat="server">
        <ContentTemplate>
        <div class="row">
            <div class="col"><span class="weight"><asp:Literal ID="Literal1" runat="server" Text="Esito Complessivo Spedizione"></asp:Literal></span></div>
                </div>
                <div class="row">
                <table class="tbl_rounded"><tr><td >
                <asp:CheckBox ID="ckbEsitoOK" Text="OK" runat="server" AutoPostBack="true" />
                </td></tr><tr>
                <td>
                <asp:CheckBox ID="ckbEsitoAttesa" Text="Attendere" runat="server" AutoPostBack="true" />
                </td></tr><tr>
                <td>
                <asp:CheckBox ID="ckbEsitoKO" Text="Verificare e rispedire" runat="server" AutoPostBack="true" />
                </td>
                </tr></table>
            </div>
        </ContentTemplate>
        </asp:UpdatePanel>
        </asp:Panel>

     </div>
     </fieldset> 
    </div>



    <%--   ALTRI FILTRI--%>
        <asp:Panel ID="pnlAltriFiltri" runat="server" >
            
    <div class="row">
        <fieldset class="filterAddressbook"  style="width:99%;">
        <asp:UpdatePanel ID="upPnlData" runat="server" >
        <ContentTemplate>
            <div class="row">
                <span class="weight"><asp:Literal ID="LtlAltriFiltri" runat="server" Text="Altri Filtri"></asp:Literal></span>
            </div>
            <div class="row">
                    <div class="col">
                       <table>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlTipoFiltro" runat="server" CssClass ="chzn-select-deselect" AutoPostBack="true" OnSelectedIndexChanged ="ddlTipoFiltro_OnSelectedIndexChanged" >
                                <asp:ListItem Value="ValoreSingolo" Text ="Valore Singolo" Selected="True"></asp:ListItem>   
                                <asp:ListItem Value="Intervallo" Text ="Intervallo"></asp:ListItem> 
                                <asp:ListItem Value="Oggi" Text ="Oggi" ></asp:ListItem>
                                <asp:ListItem Value="SettimanaCorrente" Text ="Settimana Corrente" ></asp:ListItem> 
                                <asp:ListItem Value="MeseCorrente" Text ="Mese Corrente"></asp:ListItem>              
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblDataDa" runat="server" Text="Da "></asp:Label>
                        </td>
                        <td>
                            <cc1:CustomTextArea ID="txtDataDa" runat="server" Width="80px" CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                        </td>
                        <td>
                            <asp:Label ID="lblDataA" runat="server" Text="A "></asp:Label>
                        </td>
                        <td>
                            <cc1:CustomTextArea ID="txtDataA" runat="server" Width="80px" CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                        </td> 
                        </tr>
                    </table>
                    </div>
                    <div class="col">
                        <asp:RadioButtonList ID="rl_visibilita" runat="server" 
                            RepeatDirection="Horizontal"  AutoPostBack="true" 
                            onselectedindexchanged="rl_visibilita_SelectedIndexChanged">
                        <asp:ListItem Value="doc_by_ruolo" Text ="Documenti Spediti dal Ruolo" ></asp:ListItem>
                        <asp:ListItem Value="doc_all" Text ="Tutti i Documenti Spediti" Selected="True"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
            </div>
            </ContentTemplate>
            </asp:UpdatePanel>
        </fieldset>
            </div>
        </asp:Panel>

    <!-- Filtro Registro Casella mittente -->
    <asp:Panel ID="pnlRegistroCasella" runat="server">
            <div class ="row">
    <fieldset class="filterAddressbook"  style="width:99%;">
    <asp:UpdatePanel ID="upPnlRegistroCasella" runat="server" UpdateMode="Conditional"><ContentTemplate>
    <div class="row">
                <span class="weight"><asp:Literal ID="ltlRegistroCasellaTitle" runat="server" Text="Registro e Casella Mittente"></asp:Literal></span>
            </div>
            <div id="SenderDivRegistri" runat="server">
                        
                            <asp:Label ID="SenderLblRegistriRF" runat="server" CssClass="lblspec" ></asp:Label><asp:DropDownList ID="cboRegistriRF" runat="server" CssClass="chzn-select-deselect" Width="300px" onselectedindexchanged="cboRegistriRF_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            
                            <asp:Label ID="SenderLblCaselle" runat="server" CssClass="lblspec"></asp:Label>
                            <asp:DropDownList ID="ddl_caselle" runat="server" CssClass="chzn-select-deselect" Width="300px" Enabled="false" AutoPostBack="true"></asp:DropDownList>
                        
                    </div>
                    </ContentTemplate></asp:UpdatePanel>
            </fieldset>
                    </div>
    </asp:Panel>
    

   <%-- <div class="row"><br /></div>--%>

    <%--GRIGLIA DEI RISULTATI--%>
     <div class="row">
 <div id="Div1" style="width: 100%;" align="center" runat="server">
            <asp:UpdatePanel runat="server" ID="UpPnlDest" UpdateMode="Conditional">
                <ContentTemplate>
                <table id="tblEspandiChiudi" runat="server" style="width:100%"><tr><td style="text-align:left">
                <img src="../Images/Icons/massive_add_in_project.png"  style="cursor: pointer" class="clickableRight" title="Espandi tutti i documenti" />
                <img src="../Images/Icons/project_hover.png" style="cursor: pointer" class="clickableRight" title="Chiudi tutti i documenti" />
                 </td>
                 <td style="text-align:right">Documenti trovati: <asp:Label runat="server" ID="lblNumDocTrovati"></asp:Label></td></tr></table>
                  
                    <asp:GridView ID="gvlistaDocumenti" 
                        CssClass="tbl_rounded_custom round_onlyextreme" runat="server"
                        Visible="False" AutoGenerateColumns="False"
                        Width="100%" PageSize="1" 
                        EnableViewState="true" 
                        BorderWidth="0px"
                        OnRowDataBound ="OnRowDataBound" onrowcommand="gvlistaDocumenti_RowCommand"
                        
                        >
                        <RowStyle CssClass="NormalRow" Height="50" />
                        <AlternatingRowStyle CssClass="AltRow" />
                        <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="20px">
                                <ItemTemplate>
                                    <img alt = "" style="cursor: pointer" src="../Images/Icons/collapsed.png" />
                                    <asp:Panel ID="pnlSpedizioni" runat="server" style="display:none">
                                        <asp:GridView ID="gvSpedizioni" runat="server" 
                                        AutoGenerateColumns="false" 
                                        CssClass = "ChildGrid" 
                                        BorderWidth="0" 
                                        OnRowDataBound ="gvSpedizioni_OnRowDataBound"
                                        Width="100%" >
                                        <PagerStyle BorderWidth ="0" CssClass="tbl_rounded_custom round_onlyextreme"/>
                                            <Columns>
                                                <asp:BoundField ItemStyle-Width="240px" DataField="NominativoDestinatario" HeaderText="Destinatari" />
                                                <asp:BoundField ItemStyle-Width="50px" DataField="TipoDestinatario" HeaderText="Tipo"
                                                    ItemStyle-HorizontalAlign="Center" />
                                                <asp:BoundField ItemStyle-Width="150px" DataField="MezzoSpedizione" HeaderText="Mezzo"
                                                    ItemStyle-HorizontalAlign="Center" />
                                                <asp:BoundField ItemStyle-Width="150px" DataField="EMailMittente" HeaderText="Mail Mitt"
                                                    ItemStyle-HorizontalAlign="Center" />
                                                <asp:BoundField ItemStyle-Width="150px" DataField="EMailDestinatario" HeaderText="Mail Dest"
                                                    ItemStyle-HorizontalAlign="Center" />
                                                <asp:BoundField ItemStyle-Width="150px" DataField="DataSpedizione" HeaderText="Ultima Sped"
                                                    ItemStyle-HorizontalAlign="Center" />
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                                    ItemStyle-Width="42px">
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblHdrAccettazione" Text="Acc" ToolTip="Accettazione"
                                                            runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgAccettazione" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                                    ItemStyle-Width="42px">
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblHdrConsegna" Text="Cons" ToolTip="Consegna"
                                                            runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgConsegna" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                                    ItemStyle-Width="42px">
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblHdrConferma" Text="Conf" ToolTip="Conferma"
                                                            runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgConferma" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                                    ItemStyle-Width="42px">
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblHdrAnnullamento" Text="Ann" ToolTip="Annullamento"
                                                            runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgAnnullamento" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                                    ItemStyle-Width="42px">
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblHdrEccezione" Text="Ecc" ToolTip="Eccezione"
                                                            runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgEccezione" runat="server" Width="32" Height="32" HeaderStyle-HorizontalAlign="Center" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px" HeaderText="Azione/Info" >
                                                <ItemTemplate>
                                                <asp:Label id="labelAzioneInfoSped" runat="server"></asp:Label></ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:BoundField ItemStyle-Width="100px" DataField="Azione_Info" HeaderText="Azione/Info" ItemStyle-HorizontalAlign ="Center"/>
                                            --%></Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-HorizontalAlign = "Left"  HeaderText="Elenco documenti spediti">
                                <ItemTemplate>
                                    <table>
                                        <tr>
                                            <td style="border-width: 0px">
                                                <asp:Image ID="imgAlert" runat="server" Width="32px" Height="32px"></asp:Image>
                                            </td>
                                            <td style="border-width: 0px">
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="border-width: 0px">
                                                <cc1:CustomImageButton ID="IndexImgDetailsDocument" CommandName="ViewDetailsDocument"
                                                CommandArgument='<%# Eval("IDDocumento") %>'
                                                    runat="server" ImageUrl="../Images/Icons/search_response_documents.png" OnMouseOutImage="../Images/Icons/search_response_documents.png"
                                                    OnMouseOverImage="../Images/Icons/search_response_documents_hover.png" CssClass="clickable"
                                                    ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png" Height="22px"
                                                    ToolTip="Vai al documento"   OnClientClick="disallowOp('Content2');" />
                                                    
                                            </td>
                                            <td style="border-width: 0px">
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="border-width: 0px">
                                                <span class="weight">
                                                    <asp:Label ID="lblNumeroDocumento" runat="server" Text='<%# Eval("Protocollo") %>'></asp:Label></span>
                                            </td>
                                            <td style="border-width: 0px">
                                                -
                                            </td>
                                            <td style="border-width: 0px">
                                                <asp:Label ID="lblOggettoDocumento" runat="server" Text='<%# Eval("DescrizioneDocumento") %>'></asp:Label>
                                            </td>
                                            
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate >
                            <span class="weight">
                            <% if (string.IsNullOrEmpty(IdDocumento))
                               { %>
                            <asp:Literal ID="litEmptyDocs" runat="server" Text="Nessun Documento Trovato."></asp:Literal>
                            <% }
                               else
                               { %>
                               <asp:Literal ID="litEmptySingleDoc" runat="server" Text="Nessun elemento trovato per i filtri impostati."></asp:Literal>
                            
                            <% } %>
                            </span>
                        </EmptyDataTemplate>
                        <PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN"></PagerStyle>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
</div>



</asp:Content>

<%--Buttom Controls--%>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <%--Buttom Applica Filtri--%>
    <asp:UpdatePanel runat="server" ID="UpSend" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="btnApplicaFiltri" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Cerca" OnClick="btnApplicaFiltri_OnClick"  OnClientClick="disallowOp('Content2');" />
            <%--  Buttom Esporta--%>
            <cc1:CustomButton ID="btnEsporta" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Esporta" OnClick="btnExport_Click"  OnClientClick="disallowOp('Content2');" />
            <%--    Buttom Chiudi--%>
            <cc1:CustomButton ID="btnChiudi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="btnChiudi_Click"   OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>