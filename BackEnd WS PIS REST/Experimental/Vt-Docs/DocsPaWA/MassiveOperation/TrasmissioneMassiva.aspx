<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrasmissioneMassiva.aspx.cs"
    Inherits="DocsPAWA.MassiveOperation.TrasmissioneMassiva" MasterPageFile="~/MassiveOperation/MassiveMasterPage.Master" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc3" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>
<asp:Content ContentPlaceHolderID="Head" ID="Head" runat="server">
    <style type="text/css">
        /* Stili per l'accordion panel */
        
        /* Header quando il pannello è collassato */
        .accordionHeader
        {
            border: 1px solid #2F4F4F;
            color: white;
            background-color: #2E4d7B;
            font-family: Arial, Sans-Serif;
            font-size: 12px;
            font-weight: bold;
            padding: 5px;
            margin-top: 5px;
            cursor: pointer;
        }
        
        /* Link all'interno dell'header quando il pannello è collassato */
        .accordionHeader a
        {
            color: #FFFFFF;
            background: none;
            text-decoration: none;
        }
        
        /* Hover su link dell'header quando il pannello è collassato */
        .accordionHeader a:hover
        {
            background: none;
            text-decoration: underline;
        }
        
        /* Header quando il pannello è espanso */
        .accordionHeaderSelected
        {
            border: 1px solid #2F4F4F;
            color: white;
            background-color: #5078B3;
            font-family: Arial, Sans-Serif;
            font-size: 12px;
            font-weight: bold;
            padding: 5px;
            margin-top: 5px;
            cursor: pointer;
        }
        
        /* Link nel'header quando il pannello è espanso */
        .accordionHeaderSelected a
        {
            color: #FFFFFF;
            background: none;
            text-decoration: none;
        }
        
        /* Hover sul link quando il pannello è espanso */
        .accordionHeaderSelected a:hover
        {
            background: none;
            text-decoration: underline;
        }
        
        /* Contenuto del pannello */
        .accordionContent
        {
            background-color: #FFFFFF;
            border: 1px dashed black;
            border-top: none;
        }
        
        /* Div formattato */
        .formattedDiv
        {
            font-family: Tahoma, Arial, sans-serif;
            font-size: 75%;
            border: 1px solid #810d06;
            padding: 5px 5px 5px 5px;
            margin: 5px 5px 5px 5px;
        }
        
        #scrollone fieldset
        {
            border: 1px solid #cccccc;
            margin: 0px;
            padding: 2px;
        }
        
        #scrollone legend
        {
            font-size: 10px;
            font-weight: bold;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
        }
        
        .contenitore_box
        {
            text-align: left;
            background-color: #fafafa;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 75%;
            margin: 5px;
            padding: 5px;
            border: 1px solid #eaeaea;
        }
        
        #trasm_cont
        {
            float: left;
            width: 800px;
        }
        
        #trasm_contsx
        {
            float: left;
            width: 430px;
            background-color: #000000;
        }
        
        #trasm_contdx
        {
            float: right;
            width: 370px;
            background-color: #990000;
        }
        
        .contenitore_top
        {
            float: left;
            width: 877px;
            clear: both;
            text-align: left;
        }
        
        .contenitore_box_sx
        {
            text-align: left;
            background-color: #fafafa;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 75%;
            margin: 5px;
            padding: 5px;
            border: 1px solid #eaeaea;
            float: left;
            width: 430px;
            height: 79px;
        }
        
        .contenitore_box_dx
        {
            text-align: left;
            background-color: #fafafa;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 75%;
            margin: 5px;
            padding: 5px;
            border: 1px solid #eaeaea;
            float: right;
            width: 400px;
            height: 79px;
        }
        
        .contenitore_bottomsx
        {
            width: 430px;
            margin: 5px;
            padding: 5px;
            border: 1px solid #eaeaea;
            background-color: #fafafa;
            height: 100px;
            float: left;
        }
        
        .contenitore_bottomdx
        {
            text-align: right;
            background-color: #fafafa;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 75%;
            margin: 5px;
            padding: 5px;
            border: 1px solid #eaeaea;
            float: right;
            width: 400px;
            height: 100px;
        }
    </style>
    <script type="text/javascript" language="javascript">

        // Funzione per il calcolo del numero di caratteri restanti
        function updateRemainingChars(sender, maxChar) {
            // La dimensione del testo
            var textLength = 0;
            
            // Prelevamento del testo e calcolo del numero di caratteri che lo compongono
            textLength = sender.value.length;

            // Viene scritto nella casella di testo con inRemaining,
            // il numero di caratteri rimanenti
            var inRemain = document.getElementById('inRemaining');
            inRemain.value = maxChar - textLength;

            // Se ci sono più caratteri di quelli ammessi, viene troncato il testo
            if (inRemain.value < 0) {
                inRemain.value = 0;
                sender.value = sender.value.substring(0, maxChar);
            }
        }

        // Funzione per l'apertura della pagina della rubrica per effettuare 
        // la ricerca del corrispondente a cui inviare la trasmissione
        function openTransmissionAddressBook(reason) {

            // Il tipo di destinatario (Ultimo carattere di reason)
            var recivierType = reason.substring(reason.length - 1, reason.length);  //reason[reason.length - 1];

            // Dichiarazione e creazione di un oggetto per la gestione della rubrica
            var r = new Rubrica();

            // Impostazione del tipo di corrispondenti da ricercare (Solo interni)
            r.CorrType = r.Interni;

            // A seconda del tipo di corrispondente, viene impostato un call type
            // apposito
            switch (recivierType) {
                case 'T':
                    r.CallType = r.CALLTYPE_TRASM_ALL;      // Tutti
                    break;
                case 'I':
                    r.CallType = r.CALLTYPE_TRASM_INF;      // Inferiori
                    break;

                case 'S':
                    r.CallType = r.CALLTYPE_TRASM_SUP;      // Superiori
                    break;

                case 'P':
                    r.CallType = r.CALLTYPE_TRASM_PARILIVELLO;  // Parilivello
                    break;
            }

            r.MoreParams = "objtype=";
            // Apertura della rubrica
            var res = r.Apri();

        }

    </script>
    <script src="../LIBRERIE/rubrica.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Form" ContentPlaceHolderID="Form" runat="server">
    <div id="scrollone" style="overflow-y: scroll; height: 500px; width: 900px;">
        <script type="text/javascript" language="javascript">
            // Registrazione degli eventi di inizio e fine request
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

            // Script per il mantenimento dello scrolling nella finestra del dettaglio
            // trasmissione
            var scrollReciviers;

            // Funzione per il salvataggio della posizione di scrolling della gridview destinatari
            function BeginRequestHandler(sender, args) {
                var divDetails = $get('<%=pnlTransmissionDetails.ClientID %>');
                if (divDetails != null) {
                    scrollReciviers = divDetails.scrollTop;
                }
            }

            // Script per il ripristino della posizione di scrolling
            function EndRequestHandler(sender, args) {
                var divDetails = $get('<%=pnlTransmissionDetails.ClientID %>');
                if (divDetails != null) {
                    divDetails.scrollTop = scrollReciviers;
                }
            }
        </script>
        <fieldset>
            <legend>Trasmissione rapida</legend>
            <asp:Panel ID="pnlRapida" runat="server" CssClass="contenitore_box">
                <asp:UpdatePanel ID="upTemplates" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                                    <asp:Label ID="Label7" runat="server" CssClass="testo_grigio">Modello</asp:Label> <asp:DropDownList ID="ddlTemplates" TabIndex="420" runat="server" CssClass="testo_grigio"
                                        Width="344px" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplates_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </contenttemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </fieldset>
        <fieldset>
            <legend>Trasmissione semplice</legend>
            <div class="contenitore_top">
                <asp:Panel ID="Panel1" runat="server" CssClass="contenitore_box_sx">
                    <asp:Label ID="l1" runat="server" CssClass="testo_grigio" Width="60px">Ruolo:</asp:Label>
                    <asp:TextBox ID="txtSenderRole" CssClass="testo_grigio" Width="310px" runat="server"
                        ReadOnly="True" BackColor="White"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label1" runat="server" CssClass="testo_grigio" Width="60px">Utente:</asp:Label>
                    <asp:TextBox ID="txtSenderUser" CssClass="testo_grigio" Width="310px" runat="server"
                        BackColor="White"></asp:TextBox>
                    <asp:UpdatePanel ID="upRecivierByCode" runat="server" UpdateMode="Conditional">
                        <contenttemplate>
                                    <asp:Label ID="Label2" runat="server" CssClass="testo_grigio" Width="60px">Ragione:</asp:Label>
                                    <asp:DropDownList ID="ddlReasons" runat="server" CssClass="testo_grigio" Width="224px"
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlReasons_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Text="Seleziona..." Value="-1"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <asp:Label ID="Label3" runat="server" CssClass="testo_grigio">Destinatari per codice</asp:Label>
                                    <asp:TextBox ID="txtRecivierCode" CssClass="testo_grigio" Width="95px" runat="server"
                                        Enabled="false"></asp:TextBox>
                                    <asp:ImageButton ID="ibSearch" runat="server" AlternateText="Selezione per codice inserito"
                                        ImageUrl="../images/rubrica/b_arrow_right.gif" Enabled="false" Style="height: 12px"
                                        OnClick="ibSearch_Click"></asp:ImageButton>
                                    <cc1:ImageButton ID="btnReciviersAddressBook" Width="30px" AlternateText="Seleziona da Rubrica"
                                        ImageUrl="../images/proto/rubrica.gif" runat="server" Height="20px" DisabledUrl="../images/proto/rubrica.gif"
                                        Enabled="false"></cc1:ImageButton>
                                        
                                    <asp:CheckBox ID="cbLeaseRights" runat="server" CssClass="testo_grigio" Text="Cede diritti" />
                                </contenttemplate>
                        <triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlReasons" EventName="SelectedIndexChanged" />
                                </triggers>
                    </asp:UpdatePanel>
                </asp:Panel>
                <asp:Panel ID="Panel2" runat="server" CssClass="contenitore_box_dx">
                    <asp:Label ID="Label4" runat="server" CssClass="testo_grigio">Descrizione:</asp:Label>
                    <asp:UpdatePanel ID="upDescriptionSection" runat="server" UpdateMode="Conditional">
                        <contenttemplate>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="testo_grigio" Width="365"
                                        ReadOnly="True" TextMode="MultiLine" Rows="4" Height="49px" MaxLength="250"></asp:TextBox>
                                </contenttemplate>
                        <triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlReasons" EventName="SelectedIndexChanged" />
                                </triggers>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>
            <div class="contenitore_top">
                <asp:UpdatePanel ID="Update_panel_note" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                           <asp:Panel ID="Panel3" runat="server" CssClass="contenitore_bottomsx">   
                               <asp:Label ID="Label5" runat="server" CssClass="testo_grigio">Note generali:</asp:Label>
                               <br />
                                <asp:TextBox ID="txtNotes" CssClass="testo_grigio" onkeyup="updateRemainingChars(this,'250')" runat="server"
                                   Width="365" TextMode="MultiLine" Height="60px" MaxLength="250"></asp:TextBox>
                                   <br />
                                <asp:Label ID="Label6" runat="server" CssClass="testo_grigio">&nbsp; caratteri disponibili:&nbsp;</asp:Label><input class="testo_grigio" readonly="readonly"
                                    type="text" size="2" value="250" name="inRemaining" id="inRemaining" />
                            </asp:Panel>
                            <asp:Panel ID="Panel4" runat="server" CssClass="contenitore_bottomdx"> 
                            </asp:Panel>
                        </contenttemplate>
                </asp:UpdatePanel>
            </div>
        </fieldset>
        <span class="titolo_scheda">Destinatari:</span>
        <asp:Panel ID="pnlReceiver" runat="server" CssClass="contenitore" BorderColor="#810d06"
            BorderWidth="1px" BorderStyle="Solid">
            <asp:UpdateProgress ID="uppDestinatari" runat="server">
                <progresstemplate>
                    <div style="vertical-align: middle; text-align: center;">
                        <asp:Image ID="imgLoading" runat="server" ImageAlign="Middle" ImageUrl="~/MassiveOperation/loadin.gif" />
                    </div>
                </progresstemplate>
            </asp:UpdateProgress>
            <asp:UpdatePanel ID="upDestinatari" runat="server">
                <contenttemplate>
                    <asp:Panel ID="pnlTransmissionDetails" runat="server" Height="150px" ScrollBars="Auto"
                        Visible="true" BorderStyle="None">
                        <asp:DataGrid ID="dgDestinatari" runat="server" CellPadding="4" SkinID="datagrid"
                            CssClass="contenitore" GridLines="Horizontal" AutoGenerateColumns="false" Width="100%"
                            BackColor="White" BorderColor="Gray" BorderWidth="1px"
                            OnItemCommand="dgDestinatari_OnItemCommand">
                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
		                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
		                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                            <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn>
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfId" runat="server" Value="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Id %>" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Destinatario" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="lblReceiverDescription" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).RecivierDescription %>" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Ragione">
                                    <ItemTemplate>
                                        <asp:Label ID="lblReason" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Reason %>" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Tipo">
                                    <ItemTemplate>
                                        <asp:Label ID="lblType" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).ExtendedType %>" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlType" CssClass="testo_grigio" runat="server" DataSource="<%# this.GetDataSourceForDDLType((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem) %>"
                                            DataTextField="Text" DataValueField="Value" OnPreRender="ddlType_PreRender">
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Note">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtNote" CssClass="testo_grigio" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Note %>"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblNote" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Note %>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Data Scadenza">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).ExpirationDate %>" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtDate" CssClass="testo_grigio" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).ExpirationDate %>" />
                                        <cc3:CalendarExtender TargetControlID="txtDate" runat="server" Format="dd/MM/yyyy">
                                        </cc3:CalendarExtender>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Nascondi vers. prec.">
                                    <ItemTemplate>
                                        <asp:Label ID="lblHidePrev" runat="server" Text="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).HidePreviousVersionsString %>" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkHideVers" CssClass="testo_grigio" runat="server" Checked="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).HidePreviousVersions %>" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Utenti" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                    <ItemTemplate>
                                        <asp:Literal ID="ltlUsers" runat="server" Text="<%# this.GetUsers((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem) %>"></asp:Literal>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkSelectDeselectAll" Text="Sel / Desel tutti" AutoPostBack="true"
                                            runat="server" OnCheckedChanged="chkSelectDeselectAll_CheckedChanged" />
                                        <asp:CheckBoxList ID="cblUsers" TextAlign="Right" runat="server" OnPreRender="cblUsers_PreRender"
                                            DataSource="<%# this.GetUsersForModifyMod((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem) %>" />
                                        <br />
                                        <asp:Literal ID="ltlNoEditable" runat="server" Text="Lista disabilitata in quanto il modello non prevede notifiche utente"
                                            Visible="false"></asp:Literal>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:EditCommandColumn ButtonType="LinkButton" ItemStyle-ForeColor="Black" EditText="Modifica" UpdateText="Salva"
                                    CancelText="Annulla"></asp:EditCommandColumn>
                                <asp:TemplateColumn HeaderText="Cancella" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                    <ItemTemplate>
                                        <asp:LinkButton ForeColor="Black" ID="lbDelete" runat="server" CommandName="Delete" CommandArgument="<%# ((DocsPAWA.utils.MassiveOperationTransmissionDetailsElement)Container.DataItem).Id  %>"
                                            Text="Cancella" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                               <asp:TemplateColumn HeaderText="Nuovo proprietario" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle" Visible="false">
                                    <ItemTemplate>
                                        <asp:LinkButton ForeColor="Black" ID="lbNewOwner" runat="server" CommandName="NewOwner" Text="Nuovo proprietario" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </asp:Panel>
                </contenttemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
</asp:Content>
