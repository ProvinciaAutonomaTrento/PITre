<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newPolicyFascicolo.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.newPolicyFascicolo" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crea una nuova Policy per i Fascicoli</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }
        // L'URL della finestra per creare una nuova policy

        var _urlChooseProject = '<%=UrlChooseProject %>';

        var _urlCampiProfilati = '<%=UrlCampiProfilati %>';

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
            width: 100%;
            background-color: #ffffff;
            height: 630px;
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
            height: 580px;
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
        .testo_chk
        {
            font-size: 9px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            font-weight: normal;
            padding-bottom: 5px;
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
                                <legend>Tipologia del fascicolo</legend>
                                    <asp:DropDownList ID="ddl_type_documents" runat="server" CssClass="testo_grigio" Width="610"  AutoPostBack="true" OnSelectedIndexChanged="ChangeTypeDocument">
                                    </asp:DropDownList>
                                    <asp:ImageButton ID="btnCampiProfilati" runat="server" ImageUrl="../../images/proto/ico_oggettario.gif" Enabled="false" ToolTip="Cerca per campi profilati" AlternateText="Cerca per campi profilati"/>
                            </fieldset>
                    </asp:Panel>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upStateTypeDocument" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <fieldset>
                                <legend>Stato del fascicolo</legend>
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
                                            <br />                                     
                                    <asp:HiddenField ID="id_corr" runat="server" />
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
                            <asp:Panel ID="Panel1" runat="server">
                                <fieldset>
                                    <legend>Classificazione*</legend>
                                    <asp:TextBox ID="txtCodFascicolo" runat="server" CssClass="testo_grigio" Width="90" MaxLength="30" OnTextChanged="txtCodFascicolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:TextBox ID="txtDescFascicolo" runat="server" CssClass="testo_grigio" Width="520" MaxLength="30" Enabled="false"></asp:TextBox>
                                    <br />
                                     <asp:CheckBox ID="chk_includiSottoNodi" runat="server" Text="Includi nella ricerca anche i sotto nodi" CssClass="testo_chk" Enabled="false" />
                                      <asp:HiddenField ID="is_fasc" runat="server" />
                                    <asp:HiddenField ID="id_Fasc" runat="server" />
                                </fieldset>
                                  
                            </asp:Panel>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upDigitale" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="Panel2" runat="server">
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
