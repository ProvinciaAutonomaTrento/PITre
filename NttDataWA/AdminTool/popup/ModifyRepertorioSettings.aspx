<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModifyRepertorioSettings.aspx.cs"
    Inherits="DocsPAWA.popup.ModifyRepertorioSettings" %>

<%@ Register TagPrefix="uc1" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/AjaxMessageBox.ascx" TagName="AjaxMessageBox" TagPrefix="uc2" %>
<%@ Register Src="../AdminTool/UserControl/RuoloResponsabile.ascx" TagName="RuoloResponsabile"
    TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../AdminTool/CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script src="../LIBRERIE/rubrica.js" type="text/javascript"></script>
    <script src="../LIBRERIE/date.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        // Funzione per il controllo di validità delle date inserite nel pannello dei dettagli dei
        // registri di repertorio.
        function checkDates(source, arguments) {
            // Pulizia validatori
            cleanValidators();

            // Valizazione delle date
            arguments.IsValid = validateDates();

        }

        // Funzione per la pulizia dei validatori
        function cleanValidators() {
            // Pulizia dei risultati pregressi
            if (typeof (Page_Validators) != "undefined") {
                for (var i = 0; i < Page_Validators.length; i++) {
                    var validator = Page_Validators[i];
                    validator.isvalid = true;
                    ValidatorUpdateDisplay(validator);
                }
            }

            ddlValidator.style.visibility = 'hidden';
        }

        // Funzione per la validazione delle date
        function validateDates() {
            // Costruzione id degli oggetti da controllare
            var ddlPrintFrequency = document.getElementById('dvRegistryDetails_ddlPrintFrequency');
            var txtStart = document.getElementById('dvRegistryDetails_txtStart');
            var txtFinish = document.getElementById('dvRegistryDetails_txtFinish');
            var lblNextPrint = document.getElementById('dvRegistryDetails_lblNextPrint');

            // Recupero data corrente e trasformazione delle eventuali date
            // impostate nei campi
            var now = new Date();
            var dateStart = new Date();
            var dateFinish = new Date();
            if (txtStart.value != undefined && txtStart.value.trim() != '')
                dateStart = Date.parseExact(txtStart.value, 'd/M/yyyy');
            if (txtFinish.value != undefined && txtFinish.value.trim() != '')
                dateFinish = Date.parseExact(txtFinish.value, 'd/M/yyyy');

            // Risultato della validazione
            var valid = true;

            // Se l'item selezionato non è N, viene effettuato il contollo sulle date
            if (ddlPrintFrequency.value != 'N') {
                // La data di fine deve essere maggiore di oggi e della data di inizio
                var res = ((dateFinish - now) >= 0) && ((dateFinish - dateStart) >= 0);
                if (!res) {
                    valid = false;
                } else {
                    // La data di fine, rispetto al maggiore fra la data di oggi e la data di inizio stampa, 
                    // deve consentire almeno una stampa rispetto alla frequenza di stampa
                    if ((dateFinish - getNextPrint(Date.getMaxDate(dateStart, now), ddlPrintFrequency.value)) < 0) {
                        valid = false;
                    }
                }
            }

            // Calcolo della data prevista per la prossima stampa automatica
            if (ddlPrintFrequency.value != 'N')
                lblNextPrint.innerText = getNextPrint(Date.getMaxDate(dateStart, now), ddlPrintFrequency.value).toString("dd/MM/yyyy");
            else
                lblNextPrint.innerText = '-';

            // Se la validazione è passata, viene mostrato uno stile normale altrimenti, viene
            // mostrata una data in grassetto
            if (valid)
                lblNextPrint.style.fontWeight = 'normal';
            else
                lblNextPrint.style.fontWeight = 'bold';

            return valid;
        }

        // Metodo per il calcolo della prossima data di stampa rispetto alla data odierna ed alla frequenza di stampa
        function getNextPrint(date, printFreq) {
            var nextDate = Date.parseExact(date.toString('d/M/yyyy'), 'd/M/yyyy');

            switch (printFreq) {
                case 'D':
                    nextDate.addDays(1);
                    break;
                case 'W':
                    nextDate.addDays(7);
                    break;
                case 'FD':
                    nextDate.addDays(15);
                    break;
                case 'M':
                    nextDate.addMonths(1);
                    break;
            }

            return nextDate;

        }

        // Quando cambia l'item selezionato viene aggiornata la data prevista per la prossima stampa automatica
        // e vengono abilitate / disabilitate le due caselle di testo delle date
        function ddlPrintFrequency_change() {
            // Pulizia dei validatori
            cleanValidators();

            // Gestione delle date
            manageDateFields();

            // Validazione delle date
            var validationResult = validateDates();

            // Se ci sono stati problemi di validazione, viene mostrata la casella dell'errore
            if (!validationResult)
                ddlValidator.style.visibility = '';

        }

        // Funzione per la gestione dell'abilitatione / disabilitazione delle caselle delle date
        function manageDateFields() {
            // Recupero di un riferimento alla drop down list
            var ddlPrintFrequency = document.getElementById('dvRegistryDetails_ddlPrintFrequency');
            // Recupero di un riferimento alla due text box per la definizione delle
            // date di inizio e fine attività del servizio di stampa automatica
            var txtStart = document.getElementById('dvRegistryDetails_txtStart');
            var txtFinish = document.getElementById('dvRegistryDetails_txtFinish');
            var lblNextPrint = document.getElementById('dvRegistryDetails_lblNextPrint');

            // Se il valore selezionato è 'N', vengono disabilitate le due text box
            // che consentono di specificare l'intervallo temporale di attività
            // del servizio di stampa automatica
            if (ddlPrintFrequency.value != 'N') {
                txtStart.disabled = '';
                txtFinish.disabled = '';
            }
            else {
                txtStart.disabled = 'disabled';
                txtFinish.disabled = 'disabled';
            }
        }

        // Funzione per l'inizializzazione dei controlli della pagina
        function initialize() {
            try {
                var ddlPrintFrequency = document.getElementById('dvRegistryDetails_ddlPrintFrequency');
                ddlPrintFrequency.onchange = ddlPrintFrequency_change;

                // Gestione abilitazione / disabilitazione delle date
                manageDateFields();

            } catch (e) { }

        }

        function OpenAddressBook() {

            var r = new Rubrica();

            // Impostazione del tipo di corrispondenti da ricercare (Solo interni)
            r.CorrType = r.Interni;

            // Impostazione del calltype
            r.CallType = r.CALLTYPE_RUOLO_RESP_REPERTORI;

            r.MoreParams = "ajaxPage";

            // Apertura della rubrica
            var res = r.Apri();
        }

    </script>
</head>
<body onload="initialize()">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <script type="text/javascript" language="javascript">
        // Registrazione degli eventi di inizio e fine request
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        // Script per il mantenimento dello scrolling nella finestra del dettaglio
        // trasmissione
        var scrollReciviers;

        // Funzione per il salvataggio della posizione di scrolling della gridview destinatari
        function BeginRequestHandler(sender, args) {
            var divDetails = $get('<%=pnlRegistersRF.ClientID %>');
            if (divDetails != null) {
                scrollReciviers = divDetails.scrollTop;
            }
        }

        // Script per il ripristino della posizione di scrolling
        function EndRequestHandler(sender, args) {
            var divDetails = $get('<%=pnlRegistersRF.ClientID %>');
            if (divDetails != null) {
                divDetails.scrollTop = scrollReciviers;
            }

            initialize();
        }
        </script>
    <asp:Panel HorizontalAlign="Center" Width="97%" runat="server" ID="pnlWindow">
        <div class="titolo_pnl" style="text-align: center; border-color: Black; border-style: solid;
            border-width: 1px; width: 100%;">
            Dettagli registro di repertorio
        </div>
        <div style="width: 100%; text-align: center; padding-bottom: 5px;" class="testo">
            <div style="float: left; padding-top: 6px;">
                Impostazioni:</div>
            <div style="float: left;">
                <asp:UpdatePanel ID="upResponsable" runat="server">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="rblResponsableType" runat="server" RepeatDirection="Horizontal"
                            OnSelectedIndexChanged="rblResponsableType_SelectedIndexChanged" AutoPostBack="True">
                            <asp:ListItem Text="Uniche per tutta la tipologia" Value="G" />
                            <asp:ListItem Text="Differenti per RF / Registro di AOO" Value="S" />
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="clear: both;" />
        </div>
        <asp:UpdatePanel ID="upRegistriesRF" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlRegistersRF" runat="server" Visible="true" Height="200px" ScrollBars="Vertical">
                    <asp:DataGrid ID="dgRegistersRF" runat="server" Width="99%" BorderWidth="1px" CellPadding="1"
                        BorderColor="Gray" AutoGenerateColumns="False" OnItemCommand="dgRegistersRF_ItemCommand"
                        DataSourceID="MinimalSettingsDataSource">
                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="bg_grigioA" HorizontalAlign="Left" VerticalAlign="Middle">
                        </AlternatingItemStyle>
                        <ItemStyle CssClass="bg_grigioN" HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                        <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn HeaderText="Descrizione" DataField="RegistryOrRfDescription" />
                            <asp:BoundColumn HeaderText="Ruolo responsabile" DataField="RoleAndUserDescription"
                                ItemStyle-HorizontalAlign="Center" />
                            <asp:TemplateColumn>
                                <HeaderStyle Width="10%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:UpdatePanel ID="upSelect" runat="server">
                                        <ContentTemplate>
                                            <asp:ImageButton CommandName="Select" runat="server" ID="imgDetails" ImageUrl="~/AdminTool/Images/lentePreview.gif"
                                                ToolTip="Dettagli impostazione" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:HiddenField ID="hfRegistryId" runat="server" Value="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorioSettingsMinimalInfo)Container.DataItem).RegistryId %>" />
                                    <asp:HiddenField ID="hfRfId" runat="server" Value="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorioSettingsMinimalInfo)Container.DataItem).RfId %>" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="upSettingsDetails" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlSettingsDetails" runat="server" HorizontalAlign="Left">
                    <asp:DetailsView ID="dvRegistryDetails" runat="server" AutoGenerateRows="False" EnableModelValidation="True"
                        DefaultMode="Edit" Width="99%" CellPadding="4" GridLines="None" 
                        DataSourceID="SettingsDataSource">
                        <FieldHeaderStyle CssClass="testo_grigio_scuro" Width="30%" />
                        <EditRowStyle CssClass="testo" />
                        <Fields>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfRegistryId" runat="server" />
                                    <asp:HiddenField ID="hfRFId" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Responsabile del registro di repertorio">
                                <ItemTemplate>
                                    <uc3:RuoloResponsabile ID="cRoleResp" runat="server" RoleSystemId="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).RoleRespId %>"
                                        ShowUserSelection="false" />
                                    &nbsp;
                                    Diritti:
                                    &nbsp;
                                    <asp:DropDownList ID="ddlRights" runat="server" CssClass="testo" OnPreRender="ddlRights_PreRender">
                                        <asp:ListItem Text="Lettura" Value="R" />
                                        <asp:ListItem Text="Lettura / Scrittura" Value="RW" />
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Responsabile delle stampe di repertorio">
                                <ItemTemplate>
                                    <uc3:RuoloResponsabile ID="cPrinterRole" runat="server" RoleSystemId="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).PrinterRoleRespId %>"
                                        UserSystemId="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).PrinterUserRespId %>"
                                        ShowUserSelection="true" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stato">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlCounterState" CssClass="testo" runat="server" OnPreRender="ddlCounterState_PreRender">
                                        <asp:ListItem Text="Aperto" Value="O" />
                                        <asp:ListItem Text="Chiuso" Value="C" />
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Frequenza di stampa">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlPrintFrequency" runat="server" CssClass="testo" OnPreRender="ddlPrintFrequency_PreRender">
                                        <asp:ListItem Text="Disattivata" Value="N" />
                                        <asp:ListItem Text="Giornaliera" Value="D" />
                                        <asp:ListItem Text="Settimanale" Value="W" />
                                        <asp:ListItem Text="Quindicinale" Value="FD" />
                                        <asp:ListItem Text="Mensile" Value="M" />
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Inizio validità stampa automatica">
                                <ItemTemplate>
                                    <asp:TextBox Enabled="<%# this.EnableDateTextBox((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem) %>"
                                        ID="txtStart" CssClass="testo" runat="server" />
                                    <uc1:CalendarExtender ID="CalendarExtender1" TargetControlID="txtStart" runat="server" SelectedDate="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).DateAutomaticPrintStart != DateTime.MinValue ? ((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).DateAutomaticPrintStart : DateTime.Now %>"
                                        Format="dd/MM/yyyy"></uc1:CalendarExtender>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Fine validità stampa automatica">
                                <ItemTemplate>
                                    <asp:TextBox Enabled="<%# this.EnableDateTextBox((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem) %>"
                                        ID="txtFinish" CssClass="testo" runat="server" />
                                    <uc1:CalendarExtender ID="CalendarExtender2" TargetControlID="txtFinish" runat="server" SelectedDate="<%# ((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).DateAutomaticPrintFinish != DateTime.MinValue ? ((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).DateAutomaticPrintFinish : DateTime.Now %>"
                                        Format="dd/MM/yyyy"></uc1:CalendarExtender>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Data prevista per la prossima stampa automatica">
                                <ItemTemplate>
                                    <asp:Label ID="lblNextPrint" runat="server" CssClass="testo" Text="<%# this.FormatDate(((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).DateNextAutomaticPrint) %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Data ultima stampa">
                                <ItemTemplate>
                                    <asp:Label ID="lblLastPrint" runat="server" CssClass="testo" Text="<%# this.FormatDate(((DocsPAWA.DocsPaWR.RegistroRepertorioSingleSettings)Container.DataItem).DateLastPrint) %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CustomValidator runat="server" ID="validateAutomaticPrintStart" Display="Dynamic"
                                        ControlToValidate="txtStart" ValidationGroup="repertoriValidation" Text="Controllare la validità dell'intervallo temporale specificato."
                                        ClientValidationFunction="checkDates" />
                                    <asp:CustomValidator runat="server" ID="validateAutomaticPrintFinish" Display="Dynamic"
                                        ControlToValidate="txtFinish" ValidationGroup="repertoriValidation" Text="Controllare la validità dell'intervallo temporale specificato."
                                        ClientValidationFunction="checkDates" />
                                    <span id="ddlValidator" style="visibility: hidden; color: Red;">Controllare la validità
                                        dell'intervallo temporale specificato.</span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ItemStyle-HorizontalAlign="Center" UpdateText="Salva" CancelText="Reset"
                                ShowEditButton="true" ButtonType="Button" ControlStyle-CssClass="testo_btn_org">
                                <ControlStyle CssClass="testo_btn_org" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:CommandField>
                        </Fields>
                        <RowStyle CssClass="contenitore" />
                    </asp:DetailsView>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:ObjectDataSource ID="SettingsDataSource" runat="server" SelectMethod="GetRegisterSettings"
            TypeName="DocsPAWA.utils.RegistriRepertorioUtils" UpdateMethod="SaveRegisterSettings"
            OnSelected="SettingsDataSource_Selected" 
            OnUpdating="SettingsDataSource_Updating" 
            onupdated="SettingsDataSource_Updated" >
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="counterId" Type="String" />
                <asp:Parameter Name="registryId" Type="String" />
                <asp:Parameter Name="rfId" Type="String" />
                <asp:Parameter Name="tipologyKind" Type="Object" />
                <asp:Parameter Name="settingsType" Type="Object" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="settings" Type="Object" />
                <asp:Parameter Name="counterId" Type="String" />
                <asp:Parameter Name="tipologyKind" Type="Object" />
                <asp:Parameter Name="settingsType" Type="Object" />
                <asp:Parameter Name="idAmm" Type="String" />
                <asp:Parameter Direction="Output" Name="validationResult" Type="Object" />
            </UpdateParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="MinimalSettingsDataSource" runat="server" SelectMethod="GetSettingsMinimalInfo"
            TypeName="DocsPAWA.utils.RegistriRepertorioUtils" OnSelected="MinimalSettingsDataSource_Selected"
            OnSelecting="MinimalSettingsDataSource_Selecting">
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="counterId" Type="String" />
                <asp:Parameter Name="tipologyKind" Type="Object" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    <uc2:AjaxMessageBox ID="AjaxMessageBox" runat="server" />
    </form>
</body>
</html>
