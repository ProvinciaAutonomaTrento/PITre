<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindAndReplaceModelliTrasmissione.aspx.cs"
    Inherits="DocsPAWA.popup.FindAndReplaceModelliTrasmissione" %>

<%@ Register Src="../UserControls/AjaxMessageBox.ascx" TagName="AjaxMessageBox" TagPrefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
    <link type="text/css" href="../CSS/docspa_30.css" rel="Stylesheet" />
    <style type="text/css">
        .field_set
        {
            width: 95%;
        }
        
        .title
        {
            background-color: #fafafa;
            border: 1px solid #cccccc;
            text-align: center;
            font: Tahoma;
            font-size: medium;
            margin-left: 5px;
            margin-top: 5px;
            padding: 5px;
        }
        
        .wizard
        {
            background-color: #EFF3FB;
            border-color: #B5C7DE;
            border-width: 1px;
            font-family: Verdana;
            font-size: 10pt;
            width: 98%;
            margin-top: 8px;
        }
        
        .wizard_header
        {
            background-color: #284E98;
            border-color: #EFF3FB;
            border-style: solid;
            border-width: 2px;
            font-weight: bold;
            font-size: 8pt;
            color: White;
            text-align: center;
        }
        
        .wizard_navigation
        {
            background-color: White;
            border-color: #507CD1;
            border-style: solid;
            border-width: 1px;
            font-family: Verdana;
            font-size: 8pt;
            color: #284E98;
        }
        
        .wizard_sidebar_button
        {
            font-family: Verdana;
            color: Black;
        }
        
        .wizard_step
        {
            background-color: #eaeaea;
            font-size: 10pt;
            color: #333333;
        }
        
        .wizard_navigation
        {
            background-color: #eaeaea;
            border-style: none;
        }
        
        .datagrid
        {
            background-color: White;
            border-color: #DEDFDE;
            border-style: none;
            border-width: 1px;
            padding: 4;
            color: Black;
            width: 98%;
        }
        
        .datagrid_alternating
        {
            background-color: White;
        }
        
        .datagrid_footer
        {
            background-color: #CCCC99;
        }
        
        .datagrid_header
        {
            background-color: #6B696B;
            font-weight: bold;
            color: White;
        }
        
        .datagrid_item
        {
            background-color: #F7F7DE;
        }
        
        .content
        {
            float: left;
            margin-left: 5px;
        }
        
        .clear
        {
            clear: both;
        }
        
        .content_checkbox
        {
            float: left;
            vertical-align: bottom;
        }
    </style>
    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script type="text/javascript" language="javascript">
        function ApriRubricaFind() {
            var r = new Rubrica();
            r.CallType = r.CALLTYPE_FIND_ROLE;
            r.MoreParams = "ajaxPage";
            var res = r.Apri();
            form1.btnRefresh.click();
        }

        function ApriRubricaReplace() {
            var r = new Rubrica();
            r.CallType = r.CALLTYPE_REPLACE_ROLE;
            r.MoreParams = "ajaxPage";
            var res = r.Apri();
            form1.btnRefresh.click();
        }

        /*
        * Cancellazione della descrizione di un corrispondente se il codice è vuoto
        */
        function clearCorrData(codTxt, descrTxtId) {
            if (codTxt.value == '')
                document.getElementById(descrTxtId).value = '';

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="upAllPage" runat="server">
        <contenttemplate><asp:Button  ID="btnRefresh" runat="server" Width="0" Height="0"/>
            <fieldset class="field_set">
                <legend class="title">Trova e sostituisci</legend>
                <asp:UpdatePanel ID="upButtons" runat="server">
                    <ContentTemplate>
                        <div class="content" style="text-align:right; width:98%;">
                            <asp:Button ID="btnExport" runat="server" CssClass="PULSANTE" Text="Esporta" OnClick="btnExport_Click" />
                            <asp:Button ID="btnClose" runat="server" Text="Chiudi" OnClick="btnClose_Click" OnClientClick="window.close();" CssClass="PULSANTE" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upWizard" runat="server">
                    <contenttemplate>
                        <asp:Wizard ID="wzWizard" runat="server" CssClass="wizard" 
                            OnNextButtonClick="wzWizard_NextButtonClick" 
                            onfinishbuttonclick="wzWizard_FinishButtonClick"
                            OnPreviousButtonClick="wzWizard_PreviousButtonClick"
                            CancelButtonText="Annulla" FinishCompleteButtonText="Conferma" 
                            FinishPreviousButtonText="Precedente" StartNextButtonText="Successivo" 
                            StepNextButtonText="Successivo" StepPreviousButtonText="Precedente">
                            <HeaderStyle CssClass="wizard_header" />
                            <SideBarButtonStyle CssClass="wizard_sidebar_button"  />
                            <SideBarStyle CssClass="pulsanti" Width="20%" />
                            <StepStyle CssClass="wizard_step" />
                            <NavigationStyle CssClass="wizard_navigation" />
                            <NavigationButtonStyle CssClass="PULSANTE" />
                            <SideBarTemplate>
                                <asp:DataList ID="SideBarList" runat="server" OnItemDataBound="SideBarList_ItemDataBound">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="SideBarButton" runat="server" ForeColor="Black"></asp:LinkButton>
                                    </ItemTemplate>
                                    <SelectedItemStyle Font-Bold="true"/>
                                </asp:DataList>
                            </SideBarTemplate>  
                            <WizardSteps>
                                <asp:WizardStep StepType="Start" ID="wzStepFind" runat="server" Title="Cerca" AllowReturn="false">
                                    <div class="content">
                                        <asp:Panel runat="server" ID="pnlRegistry">
                                            <span class="testo_piccoloB">Registro in cui ricercare il corrispondente</span>
                                            <br />
                                            <asp:DropDownList ID="ddlRegistry" CssClass="testo_grigio" runat="server" />
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlFind">
                                            <span class="testo_piccoloB">Ruolo da ricercare</span>
                                            <br />
                                            <asp:TextBox ID="txtFindCodice" Width="20%" CssClass="testo_grigio" runat="server" />
                                            &nbsp;<asp:ImageButton ID="imgSrcFind" runat="server" AlternateText="Selezione per codice inserito"
                                                ImageUrl="~/images/rubrica/b_arrow_right.gif" OnClick="imgSrcFind_Click" Style="height: 12px" />
                                            &nbsp;&nbsp;<asp:TextBox ID="txtFindDescrizione" Width="50%" runat="server" ReadOnly="true" CssClass="testo_grigio" />
                                            <asp:ImageButton ID="imgRubricaFind" runat="server" Visible="True" ImageUrl="~/images/proto/rubrica.gif"
                                                AlternateText="Seleziona corrispondente da sostituire" OnClick="imgRubricaFind_Click" /><br />
                                        </asp:Panel>
                                        <div class="clear" />
                                        <asp:Panel ID="pnlReplace" runat="server" >
                                            <span class="testo_piccoloB">Ruolo con cui sostituire</span>
                                            <br />
                                            <asp:TextBox ID="txtReplaceCodice" Width="20%" runat="server" CssClass="testo_grigio" />
                                            &nbsp;<asp:ImageButton ID="imgSrcReplace" runat="server" AlternateText="Selezione per codice inserito"
                                                ImageUrl="~/images/rubrica/b_arrow_right.gif" OnClick="imgSrcReplace_Click" />
                                            &nbsp;&nbsp;<asp:TextBox ID="txtReplaceDescrizione" runat="server" CssClass="testo_grigio"
                                                ReadOnly="true" Width="50%" />
                                            <asp:ImageButton ID="imgRubricaReplace" runat="server" ImageUrl="~/images/proto/rubrica.gif"
                                                AlternateText="Seleziona il ruolo con cui sostituire" OnClick="imgRubricaReplace_Click" />
                                        </asp:Panel>
                                    </div>
                                    <div class="clear" />
                                    <div class="content_checkbox">
                                        <asp:CheckBox ID="chkCopyNotes" runat="server" Text="Copia note di trasmissione" CssClass="testo_grigio" />
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="WizardStep2" runat="server" Title="Sostituisci" AllowReturn="false" 
                                    StepType="Finish">
                                </asp:WizardStep>
                            </WizardSteps>
                        </asp:Wizard>
                    </contenttemplate>
                </asp:UpdatePanel>
            </fieldset>
            <fieldset class="field_set">
                <asp:UpdatePanel ID="upSummary" runat="server">
                    <contenttemplate>
                        <asp:Literal ID="ltlNoModels" runat="server" Text="Nessun modello individuato con i filtri di ricerca impostati."
                            Visible="false" />
                        <asp:DataGrid ID="dgResult" runat="server" GridLines="Vertical" SkinID="datagrid"
                            AutoGenerateColumns="false" >
                            <AlternatingItemStyle CssClass="datagrid_alternating" />
                            <FooterStyle CssClass="datagrid_footer" />
                            <HeaderStyle CssClass="datagrid_header" />
                            <ItemStyle CssClass="datagrid_item" />
                            <Columns>
                                <asp:BoundColumn ItemStyle-Font-Size="Small" DataField="CodiceModello" HeaderText="Codice modello" ItemStyle-Width="20%" />
                                <asp:BoundColumn ItemStyle-Font-Size="Small" DataField="DescrizioneModello" HeaderText="Descrizione modello" ItemStyle-Width="30%" />
                                <asp:BoundColumn ItemStyle-Font-Size="Small" DataField="SyntheticResult" HeaderText="Esito" ItemStyle-Width="10%" />
                                <asp:BoundColumn ItemStyle-Font-Size="Small" DataField="Message" HeaderText="Messaggio" ItemStyle-Width="40%" />
                            </Columns>
                        </asp:DataGrid>
                    </contenttemplate>
                </asp:UpdatePanel>
            </fieldset>
            <uc1:AjaxMessageBox ID="MessageBox" runat="server" />
        </contenttemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
