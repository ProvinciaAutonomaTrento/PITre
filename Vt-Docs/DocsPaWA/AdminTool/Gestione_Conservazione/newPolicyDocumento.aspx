<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newPolicyDocumento.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.newPolicyDocumento" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Crea una nuova Policy per i Documenti</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }
        // L'URL della finestra per creare una nuova policy
        var _url_new_policy = '<%=UrlDocumentFormat %>';

        var _urlChooseProject = '<%=UrlChooseProject %>';

        var _urlCampiProfilati = '<%=UrlCampiProfilati %>';

        function OpenDocumentFormat() {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.showModalDialog(_url_new_policy, 'OpenDocumentFormat', 'dialogWidth:600px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {
                document.getElementById("mod_form").value = retval;
            }
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

        //RICERCA RUOLI SOTTOPOSTI
        function _ApriRubricaRicercaRuoliSottoposti() {
            var r = new Rubrica();
            r.MoreParams = "tipo_corr=" + "U";
            r.CallType = r.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI;
            var res = r.Apri();
        }

        function OpenCampiProfilati(idTemplate) {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.showModalDialog(_urlCampiProfilati + "&id=" + idTemplate, 'OpenCampiProfilati', 'dialogWidth:750px;dialogHeight:600px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {

            }
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
            height: 900px;
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
            height: 845px;
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
            width: 630px;
        }
        #formati_documenti_sx
        {
            float: left;
            width: 50px;
            border-right: 1px solid #cccccc;
        }
        #formati_documenti_dx
        {
            float: right;
            width: 570px;
            font-size: 11px;
            color: #333333;
        }
        .testo_chk
        {
            font-size: 9px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            font-weight: normal;
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
            <asp:Label runat="server" ID="titlePage" Text="Crea una nuova Policy" CssClass="title"></asp:Label>
            <div id="content_field">
                <asp:UpdatePanel ID="upNamePolicy" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" id="pnl_name">
                            <fieldset>
                                <legend>Nome della Policy*</legend>
                                    <asp:TextBox id="txt_nome" runat="server" Width="99%" CssClass="testo_grigio" MaxLength="100"></asp:TextBox>
                            </fieldset>
                    </asp:Panel>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upTemplates" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" visible="false" id="pnl_profilazione">
                            <fieldset>
                                <legend>Tipologia del documento</legend>
                                    <asp:DropDownList ID="ddl_type_documents" runat="server" CssClass="testo_grigio" Width="610"  AutoPostBack="true" OnSelectedIndexChanged="ChangeTypeDocument">
                                    </asp:DropDownList>
                                    <asp:ImageButton ID="btnCampiProfilati" runat="server" OnClick="ViewCampiProlilati" ImageUrl="../../images/proto/ico_oggettario.gif" Enabled="false" ToolTip="Cerca per campi profilati" AlternateText="Cerca per campi profilati"/>
                            </fieldset>
                    </asp:Panel>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upStateTypeDocument" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Stato del documento</legend>
                                    <asp:DropDownList ID="ddl_state_document" runat="server" CssClass="testo_grigio" Width="100%"  AutoPostBack="true" Enabled="false">
                                    </asp:DropDownList>
                            </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upAOO" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>AOO Creatore</legend>
                                    <asp:DropDownList ID="ddl_aoo" runat="server" Width="100%" CssClass="testo_grigio"  AutoPostBack="true">
                                    </asp:DropDownList>
                            </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upTemplates3" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>RF Creatore</legend>
                                    <asp:DropDownList ID="ddl_rf" runat="server" Width="100%" CssClass="testo_grigio"  AutoPostBack="true">
                                    </asp:DropDownList>
                            </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upAddress" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                            <div class="contenitore_box_due">
                                    <fieldset>
                                        <legend>UO creatore</legend>
                                        <asp:TextBox ID="txtCodRuolo" runat="server" CssClass="testo_grigio" Width="90" MaxLength="30" OnTextChanged="txtCodRuolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        <asp:TextBox ID="txtDescRuolo" runat="server" CssClass="testo_grigio" Width="490" MaxLength="30" Enabled="false"></asp:TextBox>
                                         <cc1:ImageButton ID="btnApriRubrica" Width="30px" AlternateText="Seleziona da Rubrica" ToolTip="Seleziona da Rubrica"
                                            ImageUrl="../../images/proto/rubrica.gif" runat="server" Height="20px" DisabledUrl="../../images/proto/rubrica.gif"></cc1:ImageButton>                                       
                                    <asp:HiddenField ID="id_corr" runat="server" />
                                    <br />
                                    <asp:CheckBox ID="chk_sottoposti" runat="server" Text="Includi anche sottoposti" CssClass="testo_chk" />
                                    </fieldset>
                            </div>
                          </contenttemplate>
                </asp:UpdatePanel>
                 <asp:UpdatePanel ID="upTitolari" runat="server" UpdateMode="Conditional">
                     <contenttemplate>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" id="pnl_titolari">
                            <fieldset>
                                <legend>Titolari</legend>
                                    <asp:DropDownList ID="ddl_titolari" runat="server" CssClass="testo_grigio" Width="100%">
                                    </asp:DropDownList>
                            </fieldset>
                    </asp:Panel>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upClassificationPanel" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Classificazione</legend>
                                    <asp:TextBox ID="txtCodFascicolo" runat="server" CssClass="testo_grigio" Width="90" MaxLength="30" OnTextChanged="txtCodFascicolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:TextBox ID="txtDescFascicolo" runat="server" CssClass="testo_grigio" Width="520" MaxLength="30" Enabled="false"></asp:TextBox>
                                    <asp:RadioButtonList runat="server" ID="chk_tipo_class" TextAlign="right" RepeatDirection="Horizontal" CssClass="testo_chk" Enabled="false">
                                        <asp:ListItem Value="1" Text="Solo nel fascicolo generale" runat="server" id="rbl_1"></asp:ListItem>
									    <asp:ListItem Value="2" Text="Nel fasciolo generale e nel procedimentale" runat="server" id="rbl_2"></asp:ListItem>
									    <asp:ListItem Value="3" Text="Solo nel fascicolo procedimentale" runat="server" id="rbl_3"></asp:ListItem>     
                                    </asp:RadioButtonList>  
                                    <asp:CheckBox ID="chk_includiSottoNodi" runat="server" Text="Includi nella ricerca anche i sotto nodi" CssClass="testo_chk" Enabled="false" />
                                </fieldset>
                                <asp:HiddenField ID="is_fasc" runat="server" />
                                <asp:HiddenField ID="id_Fasc" runat="server" />
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upDocuments" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="Panel5" runat="server">
                                <fieldset>
                                    <legend>Tipo di documento</legend>
                                        <asp:CheckBoxList runat="server" ID="chkList" TextAlign="right" RepeatDirection="Horizontal" CssClass="testo_grigio"> 
                                        <asp:ListItem Value="A" runat="server" id="chk_Arr"></asp:ListItem>
									    <asp:ListItem Value="P" runat="server" id="chk_Part"></asp:ListItem>
										<asp:ListItem Value="I" runat="server" id="chk_Int"></asp:ListItem>
									    <asp:ListItem Value="G" runat="server" id="chk_Grigio"></asp:ListItem>         
                                   </asp:CheckBoxList>
                                </fieldset>
                            </asp:Panel>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upDigitale" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="Panel1" runat="server">
                                <fieldset>
                                    <legend>Documenti digitali</legend>
                                        <asp:CheckBox ID="chkDigitale" runat="server" Text="Includi solo i documenti digitali" CssClass="testo_chk" Checked="true"/>
                                </fieldset>
                            </asp:Panel>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upFirmato" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="Panel3" runat="server">
                                <fieldset>
                                    <legend>Documenti firmati</legend>
                                        <asp:CheckBox ID="chkFirmati" runat="server" Text="Includi solo i documenti firmati" CssClass="testo_chk" Checked="true"/>
                                </fieldset>
                            </asp:Panel>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upCreationDate" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Data di creazione</legend>
                                    <asp:dropdownlist id="ddl_dataCreazione_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="150px" OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                <asp:ListItem Value="5">Anno Corrente</asp:ListItem>
											</asp:dropdownlist>
                                         <asp:label id="lblDa" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label>
                                         <uc4:Calendario id="lbl_dataCreazioneDa" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                         <asp:label id="lblA" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label>
                                         <uc4:Calendario id="lbl_dataCreazioneA" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress="return MaxCaratteri(this, 0);"/>
                                </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upProtDate" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Data di protocollazione</legend>
                                    <asp:dropdownlist id="ddl_dataProt_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="150px" OnSelectedIndexChanged="ddl_dataProt_E_SelectedIndexChanged">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                <asp:ListItem Value="5">Anno Corrente</asp:ListItem>
											</asp:dropdownlist>
                                         <asp:label id="lblDaP" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label>
                                         <uc4:Calendario id="lbl_dataCreazioneDaP" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                         <asp:label id="lblAP" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label>
                                         <uc4:Calendario id="lbl_dataCreazioneAP" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress="return MaxCaratteri(this, 0);"/>
                                </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upChangeGridName3" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="Panel2" runat="server">
                                <fieldset>
                                    <legend>Formati documenti</legend>
                                    <div id="formati_documenti">
                                        <div id="formati_documenti_sx" >
                                   <asp:ImageButton id="btn_img_doc" runat="server" ImageUrl="../Images/icon_admin.gif" AlternateText="Formati documenti" ToolTip="Formati documenti" />
                                        </div>
                                        <div id="formati_documenti_dx">
                                        <asp:Label id="lbl_documents_format" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                     <asp:HiddenField ID="mod_form" runat="server" />
                                </fieldset>
                            </asp:Panel>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanelConfirm" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                <table id="Table3" align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_salva" runat="server" CssClass="cbtn" Text="Salva" OnClick="BtnSaveDocument_Click"></asp:Button>
                        </td>
                        <td>
                            <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi"
                                OnClientClick="window.close();"></asp:Button>
                        </td>
                    </tr>
                </table>
                   </contenttemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
