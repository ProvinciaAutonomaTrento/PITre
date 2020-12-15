<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newPolicyDocPARER.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.newPolicyDocPARER" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="head" runat="server">
    <title>Nuova Policy Documenti</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            alert('Salvataggio policy eseguito con successo');
            window.returnValue = ret;
            window.close();
        }
        // L'URL della finestra per creare una nuova policy

        var _urlChooseProject = 'ChooseProject.aspx';
        var _url_new_policy = '<%=UrlDocumentFormat %>';
        var _urlCampiProfilati = '<%=UrlCampiProfilati %>';

        function OpenDocumentFormat() {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.showModalDialog(_url_new_policy, 'OpenDocumentFormat', 'dialogWidth:600px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {
                document.getElementById("mod_form").value = retval;
            }
        }

        function getPolicyError() {
            alert('Errore nel caricamento della policy.');
            window.close();
        }

        function OpenSceltaFascicoli() {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.showModalDialog(_urlChooseProject, 'OpenSceltaFascicoli', 'dialogWidth:800px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {
                document.getElementById("is_fasc").value = retval;
                window.document.getElementById('<%= Page.Form.ClientID %>').submit();
            }
        }

        function OpenCampiProfilati(idTemplate) {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.showModalDialog(_urlCampiProfilati + "&id=" + idTemplate, 'OpenCampiProfilati', 'dialogWidth:750px;dialogHeight:600px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {

            }
        }

         // inserimento ruolo responsabile
         function OpenAddressBook() {

                var r = new Rubrica();

                r.MoreParams = "ajaxPage";
                r.CallType = r.CALLTYPE_RUOLO_RESP_REPERTORI;
                r.CorrType = r.Interni;

                var res = r.Apri();
            }

            function _ApriRubricaRicercaRuoliSottoposti() {
                
                var r = new Rubrica();
                r.MoreParams = "tipo_corr=" + "U";
                r.CallType = r.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI;
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
                            <asp:TextBox id="txtDescrPolicy" runat="server" Width="96%" CssClass="testo_grigio" MaxLength="100"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanelSelCrit" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                <div class="contenitore_box_due">
                    <fieldset>
                        <legend>Criteri di selezione</legend>
                        <table class="tabInput">
                            <!-- STATO VERSAMENTO -->
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
                                          
                            <!-- TIPO DOCUMENTO -->
                            <tr runat="server" id="rowTypeDocument">
                                <td class="legend">Tipo Documento</td>
                                <td>
                                    <asp:CheckBoxList id="chk_tipo" runat="server" TextAlign="Right" CssClass="testo_grigio" RepeatDirection="Horizontal" OnSelectedIndexChanged="GestioneCampiTipoProto" AutoPostBack="true">
                                        <asp:ListItem Value="A" Selected="True" runat="server" Text="Arrivo"></asp:ListItem>
                                        <asp:ListItem Value="P" Selected="True" runat="server" Text="Partenza"></asp:ListItem>
                                        <asp:ListItem Value="I" Selected="True" runat="server" Text="Interno"></asp:ListItem>
                                        <asp:ListItem Value="G" Selected="True" runat="server" Text="Non protocollato"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <!-- TIPOLOGIA -->
                            <tr>
                                <td class="legend">Tipologia</td>
                                <td>
                                    <asp:DropDownList ID="ddl_type_documents" runat="server" CssClass="testo_grigio" Width="80%" AutoPostBack="true" OnSelectedIndexChanged="ChangeTypeDocument"></asp:DropDownList>
                                    <asp:ImageButton ID="btnCampiProfilati" runat="server" ImageUrl="../../images/proto/ico_oggettario.gif" Enabled="false" ToolTip="Cerca per campi profilati" AlternateText="Cerca per campi profilati"/>
                                </td>
                            </tr>
                            <!-- STATO DOC. -->
                            <tr>
                                <td class="legend">Stato del documento</td>
                                <td>
                                    <asp:DropDownList ID="ddl_state_document_op" runat="server" CssClass="testo_grigio" width="20%" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="1">Uguale a</asp:ListItem>
                                        <asp:ListItem Value="0">Diverso da</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:DropDownList ID="ddl_state_document" runat="server" CssClass="testo_grigio" Width="79%"  AutoPostBack="true" Enabled="false"></asp:DropDownList>
                                </td>
                            </tr>
                            <!-- REGISTRO/AOO -->
                            <tr>
                                <td class="legend">Registro/AOO</td>
                                <td>
                                    <asp:DropDownList ID="ddl_reg_aoo" runat="server" Width="100%" CssClass="testo_grigio"  AutoPostBack="true" OnSelectedIndexChanged="ddl_reg_aoo_SelectedIndexChanged"></asp:DropDownList>
                                </td>
                            </tr>
                            <!-- RF -->
                            <tr>
                                <td class="legend">RF</td>
                                <td>
                                    <asp:DropDownList ID="ddl_rf" runat="server" Width="100%" CssClass="testo_grigio"  AutoPostBack="true"></asp:DropDownList>
                                </td>
                            </tr>
                            <!-- UO CREATORE -->
                            <tr>
                                <td class="legend">UO Creatrice</td>
                                <td>
                                    <asp:TextBox ID="txtCodUO" runat="server" CssClass="testo_grigio" Width="20%" MaxLength="30" OnTextChanged="txtCodUO_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:TextBox ID="txtDescUO" runat="server" CssClass="testo_grigio" Width="70%" MaxLength="30" Enabled="false"></asp:TextBox>
                                    <cc1:ImageButton ID="btnApriRubricaUO" Width="30px" AlternateText="Seleziona da Rubrica" ToolTip="Seleziona da Rubrica" OnClick="btnRubricaUO_Click"
                                            ImageUrl="../../images/proto/rubrica.gif" runat="server" Height="20px" DisabledUrl="../../images/proto/rubrica.gif"></cc1:ImageButton>
                                    <asp:HiddenField ID="id_corr_uo" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="legend">&nbsp;</td>
                                <td>
                                    <asp:CheckBox runat="server" ID="chk_uo_sottoposte" CssClass="testo_chk" Text="Includi UO sottoposte" />
                                </td>
                            </tr>
                            <!-- TITOLARIO -->
                            <tr>
                                <td class="legend">Titolario</td>
                                <td>
                                    <asp:DropDownList ID="ddl_titolari" runat="server" Width="100%" CssClass="testo_grigio"  AutoPostBack="true"></asp:DropDownList>
                                </td>
                            </tr>
                            <!-- CLASSIFICAZIONE -->
                            <tr>
                                <td class="legend">Classificazione</td>
                                <td>
                                    <asp:TextBox ID="txtCodFascicolo" runat="server" CssClass="testo_grigio" Width="20%" MaxLength="30" OnTextChanged="txtCodFascicolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:TextBox ID="txtDescFascicolo" runat="server" CssClass="testo_grigio" Width="77%" MaxLength="30" Enabled="false"></asp:TextBox>
                                    <asp:RadioButtonList runat="server" ID="chk_tipo_class" TextAlign="right" RepeatDirection="Horizontal" CssClass="testo_chk" Enabled="false">
                                        <asp:ListItem Value="C" Text="classificati" runat="server" id="rbl_1"></asp:ListItem>
									    <asp:ListItem Value="F" Text="fascicolati" runat="server" id="rbl_2"></asp:ListItem>
									    <asp:ListItem Value="CF" Text="classificati e fascicolati" runat="server" id="rbl_3"></asp:ListItem>     
                                    </asp:RadioButtonList>
                                </td>
                                <asp:HiddenField ID="is_fasc" runat="server" />
                                <asp:HiddenField ID="id_Fasc" runat="server" />
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:CheckBox runat="server" ID="chk_doc_digitali" CssClass="testo_chk" Text="includi solo documenti digitali" />&nbsp;
                                    <asp:CheckBox runat="server" ID="chk_fatture" CssClass="testo_chk" Text="escludi fatture e lotti di fatture" Checked="true" />
                                </td>
                            </tr>
                            <!-- FORMATO -->
                            <tr>
                                <td class="legend">Formato file</td>
                                <td>
                                    <div id="formati_documenti">
                                        <div id="formati_documenti_sx">
                                            <asp:ImageButton id="btn_img_doc" runat="server" ImageUrl="../Images/icon_admin.gif" AlternateText="Formati documenti" ToolTip="Formati documenti" />
                                        </div>
                                        <div id="formati_documenti_dx">
                                            <asp:Label id="lbl_documents_format" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <!-- DIMENSIONI -->
                            <tr>
                                <td class="legend">File di grandi dimensioni</td>
                                <td>
                                    <asp:RadioButtonList ID="rbl_bigFiles" runat="server" TextAlign="Right" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="0" Text="Nessun filtro" runat="server" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Escludi big files" runat="server"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Includi solo big files" runat="server"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <!-- FIRMA -->
                            <tr>
                                <td class="legend">Firme digitali</td>
                                <td>
                                    <asp:CheckBoxList id="chk_firma" runat="server" TextAlign="Right" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="1" Text="Documenti firmati" runat="server" id="chk_doc_f" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Documenti non firmati" runat="server" id="chk_doc_nf" Selected="True"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <!-- MARCA -->
                            <tr>
                                <td class="legend">Timestamp</td>
                                <td>
                                    <asp:CheckBoxList id="chk_timestamp" runat="server" TextAlign="Right" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="1" Text="Documenti marcati" runat="server" id="chk_ts" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Documenti non marcati" runat="server" id="chk_no_ts" Selected="True"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td class="legend">&nbsp;</td>
                                <td>
                                    <asp:Label id="lblScadenza" runat="server" CssClass="testo_grigio">Scadenza:</asp:Label>
                                    <asp:DropDownList runat="server" ID="ddl_timestamp_expiry" CssClass="testo_grigio" Width="30%">
                                        <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="E">Scaduti</asp:ListItem>
                                        <asp:ListItem Value="W">Entro la settimana corrente</asp:ListItem>
                                        <asp:ListItem Value="M">Entro il mese corrente</asp:ListItem>
                                        <asp:ListItem Value="Y">Entro l'anno corrente</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <!-- DATA CREAZIONE -->
                            <tr>
                                <td class="legend">Data creazione</td>
                                <td>
                                    <asp:dropdownlist id="ddl_dataCreazione_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="30%" OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
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
                                    </asp:dropdownlist>
                                    <asp:label id="lblDa" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label>
                                    <uc4:Calendario id="lbl_dataCreazioneDa" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                    <asp:label id="lblA" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label>
                                    <uc4:Calendario id="lbl_dataCreazioneA" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress="return MaxCaratteri(this, 0);"/>
                                    <asp:Label ID="lblDaysCr" runat="server" CssClass="testo_grigio" Visible="false">Numero giorni</asp:Label>
                                    <asp:TextBox runat="server" CssClass="testo_grigio" ID="txt_days_cr" MaxLength="4" Width="10%" Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <!-- DATA PROTOCOLLAZIONE -->
                            <tr>
                                <td class="legend">Data protocollazione</td>
                                <td>
                                    <asp:dropdownlist id="ddl_dataProt_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="30%" OnSelectedIndexChanged="ddl_dataProt_E_SelectedIndexChanged">
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
									</asp:dropdownlist>
                                    <asp:label id="lblDaP" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label>
                                    <uc4:Calendario id="lbl_dataCreazioneDaP" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                    <asp:label id="lblAP" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label>
                                    <uc4:Calendario id="lbl_dataCreazioneAP" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress="return MaxCaratteri(this, 0);"/>
                                    <asp:Label ID="lblDaysPr" runat="server" CssClass="testo_grigio" Visible="false">Numero giorni</asp:Label>
                                    <asp:TextBox runat="server" CssClass="testo_grigio" ID="txt_days_pr" MaxLength="4" Width="10%" Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <!-- DATA FIRMA DIGITALE -->
                            <tr>
                                <td class="legend">Data firma in P.I.Tre.</td>
                                <td>
                                    <asp:Label ID="lblDaysSign" runat="server" CssClass="testo_grigio">Numero giorni</asp:Label>
                                    <asp:TextBox runat="server" CssClass="testo_grigio" ID="txt_days_sign" MaxLength="4" Width="10%"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Panel ID="PanelStructure" runat="server">
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
                                <td>
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
