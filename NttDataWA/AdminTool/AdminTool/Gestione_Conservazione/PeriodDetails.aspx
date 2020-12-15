<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PeriodDetails.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_Conservazione.PeriodDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Periodicità Policy</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }

        // Funzione per l'apertura della pagina della rubrica per effettuare 
        // la ricerca del corrispondente a cui inviare la trasmissione
        function openTransmissionAddressBook() {

            // Dichiarazione e creazione di un oggetto per la gestione della rubrica
            var r = new Rubrica();

            // Impostazione del tipo di corrispondenti da ricercare (Solo interni)
            r.CorrType = r.Interni;

            r.CallType = r.CALLTYPE_TUTTI_RUOLI;      // Tutti

            r.MoreParams = "tipo_corr=" + "R";
            // Apertura della rubrica
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
            width: 100%;
            background-color: #ffffff;
            height: 780px;
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
            height: 810px;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 5px;
            background-color: #ffffff;
            padding-bottom: 5px;
            padding-left: 5px;
            padding-right: 5px;
            font-size: 12px;
        }
        .contenitore_box fieldset
        {
            border: 1px dashed #eaeaea;
            margin: 0px;
            padding: 5px;
        }
        
        .contenitore_box legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }
        
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 5px;
            background-color: #ffffff;
            border: 1px solid #cccccc;
            padding-bottom: 5px;
            padding-left: 5px;
            padding-right: 5px;
        }
        
        .contenitore_box_due fieldset
        {
            border: 1px dashed #eaeaea;
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
        .testo_grigio2
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
            margin-top: 5px;
        }
        .tab_period
        {
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            width: 100%;
        }
        .tab_period td
        {
            padding: 5px;
            background-color: #ffffff;
        }
        .t1
        {
            width: 5%;
            background-color: #fbfbfb;
            border: 1px solid #cccccc;
        }
        .t2
        {
            width: 95%;
            background-color: #ffffff;
            border: 1px solid #cccccc;
        }
        .inp1
        {
            width: 20px;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 11px;
            color: #333333;
        }
        .inp2
        {
            width: 50px;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 11px;
            color: #333333;
        }
        .contenitore_box_due p
        {
            margin: 0px;
            font-size: 11px;
            margin-top: 5px;
            padding: 0px;
            margin-left: 8px;
            color: #333333;
        }
        .contenitore_box p
        {
            margin: 0px;
            font-size: 11px;
            margin-top: 5px;
            padding: 0px;
            margin-left: 8px;
            color: #333333;
        }
        .ddl_d
        {
            margin-left: 10px;
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
            <asp:Label runat="server" ID="titlePage" Text="Periodicità Policy" CssClass="title"></asp:Label>
            <div id="content_field">
                <asp:UpdatePanel ID="upPeriod" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="Panel1" runat="server">
                                <fieldset>
                                    <legend>Attiva la periodicità della Policy</legend>
                                    <asp:CheckBox runat="server" Text="Seleziona per attivare/disattivare la periodicità della Policy"
                                        ID="chk_attiva" CssClass="testo_grigio" OnCheckedChanged="Chk_attivaChecked" />
                                </fieldset>
                            </asp:Panel>
                        </div>
                        <div align="center" style="margin: 5px;">
                            <table class="tab_period">
                                <tr>
                                    <td class="t1">
                                        <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value="giorno" runat="server"
                                            id="type1" />
                                    </td>
                                    <td class="t2">
                                        <div class="contenitore_box">
                                            <fieldset>
                                                <legend>Giornaliera</legend>
                                                <p>
                                                    Ogni
                                                    <asp:TextBox ID="txtNumGiorni" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox>
                                                    ore dalle
                                                    <asp:TextBox ID="txtOreGiorni" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox>
                                                    :
                                                    <asp:TextBox ID="txtMinutiGiorni" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox></p>
                                            </fieldset>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="t1">
                                        <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value="settimana"
                                            runat="server" id="type2" />
                                    </td>
                                    <td class="t2">
                                        <div class="contenitore_box">
                                            <fieldset>
                                                <legend>Settimanale</legend>
                                                <asp:CheckBoxList ID="chk_days" runat="server" TextAlign="Right" RepeatDirection="Horizontal"
                                                    CssClass="testo_grigio">
                                                    <asp:ListItem Text="Lunedì" id="chK_lunedi"></asp:ListItem>
                                                    <asp:ListItem Text="Martedì" id="chK_martedi"></asp:ListItem>
                                                    <asp:ListItem Text="Mercoledì" id="chK_mercoledi"></asp:ListItem>
                                                    <asp:ListItem Text="Giovedì" id="chK_giovedi"></asp:ListItem>
                                                    <asp:ListItem Text="Venerdì" id="chK_venerdi"></asp:ListItem>
                                                    <asp:ListItem Text="Sabato" id="chK_sabato"></asp:ListItem>
                                                    <asp:ListItem Text="Domenica" id="chK_domenica"></asp:ListItem>
                                                </asp:CheckBoxList>
                                                <p>
                                                    Dalle
                                                    <asp:TextBox MaxLength="2" CssClass="inp1" runat="server" Text="" ID="txtOreSettimana"></asp:TextBox>
                                                    :
                                                    <asp:TextBox MaxLength="2" CssClass="inp1" runat="server" Text="" ID="txtMinutiSettimana"></asp:TextBox></p>
                                            </fieldset>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="t1">
                                        <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value="mese" runat="server"
                                            id="type3" />
                                    </td>
                                    <td class="t2">
                                        <div class="contenitore_box">
                                            <fieldset>
                                                <legend>Mensile</legend>
                                                <p>
                                                    Il giorno
                                                    <asp:TextBox ID="txt_day" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox>
                                                    dalle
                                                    <asp:TextBox ID="txtOreMesi" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox>
                                                    :
                                                    <asp:TextBox ID="txtMinutiMese" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox></p>
                                            </fieldset>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="t1">
                                        <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value="anno" runat="server"
                                            id="type4" />
                                    </td>
                                    <td class="t2">
                                        <div class="contenitore_box">
                                            <fieldset>
                                                <legend>Annuale</legend>
                                                <p>
                                                    Numero mese
                                                    <asp:TextBox ID="txt_a_mese" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox>
                                                    il giorno
                                                    <asp:TextBox ID="txt_a_day" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox>
                                                    dalle
                                                    <asp:TextBox ID="txt_a_ore" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox>
                                                    :
                                                    <asp:TextBox ID="txt_a_mm" MaxLength="2" CssClass="inp1" runat="server" Text=""></asp:TextBox></p>
                                            </fieldset>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <asp:UpdatePanel ID="upAddress" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="contenitore_box_due">
                                    <fieldset>
                                        <legend>Ruolo e utente proprietario dell'istanza di conservazione*</legend>
                                        <asp:TextBox ID="txtCodRuolo" runat="server" CssClass="testo_grigio" Width="90" MaxLength="30"
                                            OnTextChanged="txtCodRuolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        <asp:TextBox ID="txtDescRuolo" runat="server" CssClass="testo_grigio" Width="470"
                                            MaxLength="30" Enabled="false"></asp:TextBox>
                                        <cc1:ImageButton ID="btnApriRubrica" Width="30px" AlternateText="Seleziona da Rubrica"
                                            ToolTip="Seleziona da Rubrica" ImageUrl="../../images/proto/rubrica.gif" runat="server"
                                            Height="20px" DisabledUrl="../../images/proto/rubrica.gif"></cc1:ImageButton>
                                        <asp:HiddenField ID="id_corr" runat="server" />
                                        <asp:DropDownList ID="ddl_role_users" runat="server" CssClass="testo_grigio2" Width="30%">
                                        </asp:DropDownList>
                                    </fieldset>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Crea l'istanza e inviala al centro servizi automaticamente</legend>
                                <asp:CheckBox runat="server" Text="Invio automatico" ID="chk_invio_automatico" CssClass="testo_grigio" AutoPostBack="true" OnCheckedChanged="chk_invio_automatico_CheckedChanged"/>
                            </fieldset>
                        </div>
                        <asp:UpdatePanel ID="upPnlConversioneAutomatica" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="contenitore_box_due">
                                    <fieldset>
                                        <legend>Converti i documenti in caso di formati non ammessi ma convertibili</legend>
                                        <asp:CheckBox runat="server" Text="Conversione automatica" ID="chk_conversione_automatica"
                                            Enabled="false" CssClass="testo_grigio" />
                                    </fieldset>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Tipologia conservazione</legend>
                                <asp:DropDownList ID="ddl_tipoCons" runat="server" DataValueField="Codice" DataTextField="Descrizione"
                                    CssClass="testo_grigio" Width="100%" OnSelectedIndexChanged="SelectTipoConservazione"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </fieldset>
                        </div>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Consolida i documenti</legend>
                                <asp:CheckBox runat="server" Text="Consolidare i documenti" ID="chk_consolidamento"
                                    CssClass="testo_grigio" />
                            </fieldset>
                        </div>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Avviso di verifica integrità dei supporti</legend>
                                <p>
                                    Numero mesi
                                    <asp:TextBox ID="txtAvvisoMesi" MaxLength="30" CssClass="inp2" runat="server" Text=""></asp:TextBox>
                            </fieldset>
                        </div>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Avviso di verifica leggibilità dei supporti</legend>
                                <p>
                                    Numero Mesi
                                    <asp:TextBox ID="txtAvvisoMesiLegg" MaxLength="30" CssClass="inp2" runat="server"
                                        Text=""></asp:TextBox>
                            </fieldset>
                        </div>
                        <table id="Table3" align="center">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btn_salva" runat="server" CssClass="cbtn" Text="Salva" OnClick="BtnSave_Click">
                                    </asp:Button>
                                </td>
                                <td>
                                    <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi" OnClientClick="window.close();">
                                    </asp:Button>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
            </asp:UpdatePanel> </div>
        </div>
    </div>
    </form>
</body>
</html>
