<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newPolicyStampePARER.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.newPolicyStampePARER" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Nuova Policy Stampe</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            alert('Salvataggio policy eseguito con successo');
            window.returnValue = ret;
            window.close();
        }

        // L'URL della finestra per creare una nuova policy

        function getPolicyError() {
            alert('Errore nel caricamento della policy.');
            window.close();
        }

        // inserimento ruolo responsabile
        function OpenAddressBook() {

            var r = new Rubrica();

            r.MoreParams = "ajaxPage";
            r.CallType = r.CALLTYPE_RUOLO_RESP_REPERTORI;
            r.CorrType = r.Interni;

            var res = r.Apri();
        }

    </script>
    <script src="../../LIBRERIE/rubrica.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
            overflow-x: hidden;
            overflow-y: scroll;
        }
        #container
        {
            float: left;
            width: 99%;
            background-color: #ffffff;
            /*height: 900px;*/
        }
        #content
        {
            border: 1px solid #cccccc;
            text-align: center;
            width: 97%;
            float: left;
            margin-left: 6px;
            margin-top: 5px;
        }
        .title
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: center;
            width: 100%;
            float: left;
            padding-top: 4px;
            padding-bottom: 4px;
            background-color: #810101;
            color: #ffffff;
        }
        #content_field
        {
            width: 100%;
            float: left;
            background-color: #fbfbfb;
            /*height: 845px;*/
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
        }
        .contenitore_box fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 0px;
        }
        
        .contenitore_box legend
        {
            font-size: 12px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
        }

        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
        }
        
        .contenitore_box_due fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 5px;
        }
        
        .contenitore_box_due legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }

        .contenitore_box_tre 
        {
            margin: 5px;
            padding-left: 10px;
        }

        .tabInput
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            padding: 10px;
            width: 100%;
        }
        
        table.tabInput td.legend
        {
            width: 20%;
            text-align: left;
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 10px;
        }
        
        #imposta
        {
            float: left;
            width: 100%;
            clear: both;
            margin-left: 10px;
            text-align: left;
            margin-bottom: 15px;
        }
        .cbtn
        {
            font-family: Verdana;
            font-size: 11px;
            margin: 0px;
            padding: 0px;
            padding: 2px;
            width: 120px;
            height: 25px;
            color: #ffffff;
            border: 1px solid #ffffff;
            background-image: url('../images/bg_button.gif');
        }
        .titolo_scheda
        {
            font-size: 11px;
            color: #666666;
            font-weight: bold;
        }
        .testo_grigio
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
        }
        #formati_documenti
        {
            float: left;
            width: 80%;
        }
        #formati_documenti_sx
        {
            float: left;
            width: 10%;
            border-right: 1px solid #cccccc;
        }
        #formati_documenti_dx
        {
            float: right;
            width: 85%;
            font-size: 11px;
            color: #333333;
        }
        .testo_chk
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            font-weight: normal;
        }
        .testo_chk_periodicity
        {
            text-align: left;
            font-size: 10px;
            color: #4b4b4b;
            font-family: Verdana;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="Form" method="post" runat="server">
        <asp:HiddenField ID="hid_tab_est" runat="server" />
        <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
        </asp:ScriptManager>

        <div id="container">
            <div id="content">
                <asp:Label runat="server" ID="titlePage" Text="Creazione nuova Policy" CssClass="title"></asp:Label>
                <div id="content_field">
                    <table class="tabInput">
                        <!-- CODICE -->
                        <tr>
                            <td class="legend">Codice</td>
                            <td>
                                <asp:TextBox id="txtCodPolicy" runat="server" Width="100px" CssClass="testo_grigio" MaxLength="16"></asp:TextBox>
                            </td>
                        </tr>
                        <!-- DESCRIZIONE -->
                        <tr>
                            <td class="legend">Descrizione</td>
                            <td>
                                <asp:TextBox id="txtDescrPolicy" runat="server" Width="99%" CssClass="testo_grigio" MaxLength="100"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <asp:UpdatePanel ID="UpdatePanelSelCrit" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Criteri di selezione</legend>
                                    <table class="tabInput">
                                        <!-- STATO CONSERVAZIONE -->
                                        <tr>
                                            <td class="legend">Stato conservazione</td>
                                            <td>
                                                <asp:RadioButtonList ID="rbl_stato_versamento" runat="server" TextAlign="Right" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="" runat="server" Text="Non conservato" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="R" runat="server" Text="Rifiutato"></asp:ListItem>
                                                    <asp:ListItem Value="F" runat="server" Text="Versamento fallito"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <!-- TIPO REGISTRO -->
                                        <tr>
                                            <td class="legend">Tipo registro</td>
                                            <td>
                                                <asp:RadioButtonList runat="server" ID="rblRegType" TextAlign="Right" RepeatDirection="Horizontal" CssClass="testo_chk" Enabled="true" OnSelectedIndexChanged="rblRegType_SelectedIndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Value="R" Text="Registro di protocollo" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="C" Text="Registro di repertorio"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <!-- REPERTORIO -->
                                        <tr>
                                            <td class="legend">Repertorio</td>
                                            <td>
                                                <asp:DropDownList ID="ddl_rep" runat="server" Width="100%" CssClass="testo_grigio" AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <!-- REGISTRO -->
                                        <tr>
                                            <td class="legend">Registro/AOO</td>
                                            <td>
                                                <asp:DropDownList ID="ddl_reg_aoo" runat="server" Width="100%" CssClass="testo_grigio"  AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <!-- RF -->
                                        <tr>
                                            <td class="legend">RF</td>
                                            <td>
                                                <asp:DropDownList ID="ddl_rf" runat="server" Width="100%" CssClass="testo_grigio"  AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <!-- ANNO -->
                                        <tr>
                                            <td class="legend">Anno di stampa</td>
                                            <td>
                                                <asp:TextBox ID="txtYear" runat="server" CssClass="testo_grigio" width="10%" MaxLength="4"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <!-- DATA DI STAMPA -->
                                        <tr>
                                            <td class="legend">Data di stampa</td>
                                            <td>
                                                <asp:DropDownList ID="ddl_dataStampa_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="30%" OnSelectedIndexChanged="ddl_dataStampa_E_SelectedIndexChanged">
                                                    <asp:ListItem Value="S">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="R">Intervallo</asp:ListItem>
									                <asp:ListItem Value="T">Oggi</asp:ListItem>
								                    <asp:ListItem Value="W">Settimana Corrente</asp:ListItem>
								                    <asp:ListItem Value="M">Mese Corrente</asp:ListItem>
                                                    <asp:ListItem Value="Y">Anno Corrente</asp:ListItem>
                                                    <asp:ListItem Value="B">Ieri</asp:ListItem>
                                                    <asp:ListItem Value="V">Settimana Precedente</asp:ListItem>
                                                    <asp:ListItem Value="N">Mese Precedente</asp:ListItem>
                                                    <asp:ListItem Value="X">Anno Precedente</asp:ListItem>
                                                    <asp:ListItem Value="P">"n" giorni prima</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:label id="lblDa" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label>
                                                <uc4:Calendario id="lbl_dataStampaDa" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                                <asp:label id="lblA" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label>
                                                <uc4:Calendario id="lbl_dataStampaA" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress="return MaxCaratteri(this, 0);"/>
                                                <asp:Label ID="lblDaysPr" runat="server" CssClass="testo_grigio" Visible="false">Numero giorni</asp:Label>
                                                <asp:TextBox runat="server" CssClass="testo_grigio" ID="txt_days_pr" MaxLength="4" Width="10%" Visible="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel id="PanelStructure" runat="server">
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Configurazione custom SACER</legend>
                                <table class="tabInput">
                                    <tr>
                                        <td class="legend">Ente</td>
                                        <td>
                                            <asp:TextBox ID="txt_custom_ente" runat="server" Width="60%" CssClass="testo_grigio" MaxLength="64"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="legend">Struttura</td>
                                        <td>
                                            <asp:TextBox ID="txt_custom_struttura" runat="server" Width="60%" CssClass="testo_grigio" MaxLength="64"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </asp:Panel>
                    <asp:UpdatePanel ID="UpdatePanelPeriodicity" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Periodicità</legend>
                                    <table class="tabInput">
                                        <tr>
                                            <td class="legend">
                                                <asp:CheckBox runat="server" ID="chk_exec_daily" CssClass="testo_chk_periodicity" Text="Giornaliera" OnCheckedChanged="Periodicity_CheckedChanged" AutoPostBack="true" />
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="legend">
                                                <asp:CheckBox runat="server" ID="chk_exec_weekly" CssClass="testo_chk_periodicity" Text="Settimanale" OnCheckedChanged="Periodicity_CheckedChanged" AutoPostBack="true" />
                                            </td>
                                            <td>
                                                <asp:DropDownList runat="server" ID="ddl_weekday" Width="20%" CssClass="testo_grigio"  AutoPostBack="true" Enabled="false">
                                                    <asp:ListItem Text="Lunedì" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Martedì" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Mercoledì" Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="Giovedì" Value="4"></asp:ListItem>
                                                    <asp:ListItem Text="Venerdì" Value="5"></asp:ListItem>
                                                    <asp:ListItem Text="Sabato" Value="6"></asp:ListItem>
                                                    <asp:ListItem Text="Domenica" Value="7"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="legend">
                                                <asp:CheckBox runat="server" ID="chk_exec_monthly" CssClass="testo_chk_periodicity" Text="Mensile" OnCheckedChanged="Periodicity_CheckedChanged" AutoPostBack="true"  />
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lbl_month" CssClass="testo_grigio" Text="Il giorno: "></asp:Label>
                                                <asp:DropDownList runat="server" ID="ddl_day_month" CssClass="testo_grigio" AutoPostBack="true" Enabled="false"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="legend">
                                                <asp:CheckBox runat="server" ID="chk_exec_yearly" CssClass="testo_chk_periodicity" Text="Annuale" OnCheckedChanged="Periodicity_CheckedChanged" AutoPostBack="true"  />
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lbl_year" CssClass="testo_grigio" Text="Il giorno: "></asp:Label>
                                                <asp:DropDownList runat="server" ID="ddl_day_year" CssClass="testo_grigio" Width="10%" AutoPostBack="true" Enabled="false"></asp:DropDownList>
                                                <asp:DropDownList runat="server" ID="ddl_month_year" CssClass="testo_grigio" Width="20%" AutoPostBack="true" Enabled="false" OnSelectedIndexChanged="ddl_month_year_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">Gennaio</asp:ListItem>
                                                    <asp:ListItem Value="2">Febbraio</asp:ListItem>
                                                    <asp:ListItem Value="3">Marzo</asp:ListItem>
                                                    <asp:ListItem Value="4">Aprile</asp:ListItem>
                                                    <asp:ListItem Value="5">Maggio</asp:ListItem>
                                                    <asp:ListItem Value="6">Giugno</asp:ListItem>
                                                    <asp:ListItem Value="7">Luglio</asp:ListItem>
                                                    <asp:ListItem Value="8">Agosto</asp:ListItem>
                                                    <asp:ListItem Value="9">Settembre</asp:ListItem>
                                                    <asp:ListItem Value="10">Ottobre</asp:ListItem>
                                                    <asp:ListItem Value="11">Novembre</asp:ListItem>
                                                    <asp:ListItem Value="12">Dicembre</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="legend">
                                                <asp:CheckBox runat="server" ID="chk_exec_once" CssClass="testo_chk_periodicity" Text="Una Tantum" OnCheckedChanged="Periodicity_CheckedChanged" AutoPostBack="true"  />
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lbl_once" CssClass="testo_grigio" Text="Il giorno: "></asp:Label>
                                                <uc4:Calendario id="lbl_cal_once" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);' EnableBtnCal="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpdatePanelRoleResp" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Ruolo Responsabile della policy</legend>
                                    <div class="contenitore_box_tre">
                                        <asp:TextBox ID="txtCodRuoloResp" runat="server" CssClass="testo_grigio" Width="15%" MaxLength="30" OnTextChanged="txtCodRuoloResp_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        <asp:TextBox ID="txtDescRuoloResp" runat="server" CssClass="testo_grigio" Width="70%" MaxLength="30" Enabled="false"></asp:TextBox>
                                        <cc1:ImageButton ID="btnApriRubricaRole" Width="30px" AlternateText="Seleziona da Rubrica" ToolTip="Seleziona da Rubrica" OnClick="btnRubricaRuoloResp_Click"
                                            ImageUrl="../../images/proto/rubrica.gif" runat="server" Height="20px" DisabledUrl="../../images/proto/rubrica.gif"></cc1:ImageButton>
                                        <asp:HiddenField ID="id_corr_resp" runat="server" />
                                    </div>
                                </fieldset>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpdatePanelConfirm" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table id="Table3" align="center">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btn_test" runat="server" CssClass="cbtn" Text="Test Esecuzione" OnClick="BtnExecTest_Click" 
                                            OnClientClick="return confirm('Attenzione, l\'operazione richiesta potrebbe durare diversi minuti. Procedere con l\'operazione?');"></asp:Button>
                                    </td>
                                    <td align="center">
                                        <asp:Button ID="btn_salva" runat="server" CssClass="cbtn" Text="Salva" OnClick="BtnSavePolicy_Click"></asp:Button>
                                    </td>
                                    <td align="center">
                                        <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi" OnClientClick="window.close();"></asp:Button>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
