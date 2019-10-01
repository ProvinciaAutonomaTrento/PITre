<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaDocumenti.aspx.cs"
    Inherits="ConservazioneWA.RicercaDocumenti" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Menu.ascx" TagName="Menu" TagPrefix="uc6" %>
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<%@ Register Assembly="MessageBox" Namespace="Utilities" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ricerca Documenti</title>
     <script src="Script/jquery-1.10.2.min.js"
        type="text/javascript"></script>
    <link href="CSS/Conservazione.css" type="text/css" rel="stylesheet" />

    <style type="text/css">
        #header
        {
            background: url(Img/bg_header.png) repeat-x scroll;
        }
        
        #menutop
        {
            background: url(Img/bg_menutop.png) repeat-x scroll;
        }
        
        .altro a:link
        {
            background-image: url('Img/bg_menutop_no_hover.png');
        }
        
        .altro a:visited
        {
            background-image: url('Img/bg_menutop_no_hover.png');
        }
        
        .altro a:hover
        {
            background-image: url('Img/bg_menutop_hover.png');
        }
        
        .sonoqui a:link
        {
            background-image: url('Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:visited
        {
            background-image: url('Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:hover
        {
            background-image: url('Img/sono_qui_hover.png');
        }
        
        .cbtn
        {
            background-image: url('Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('Img/bg_button_hover.jpg');
        }
        
        .tab_istanze_header
        {
            background-image: url('Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('Img/bg_content.jpg'); 
        }
        
         .menu_pager_grigio
        {
            background-image: url('Img/bg_pager_table.jpg');
            background-repeat: repeat-x;
        }
        
        /* Stile aggiunto da A. Sigalot e C.Fuccia per menu orizzontale */


        /*Top level list items*/
        .horizontalcssmenu ul li
        {
            
            position: relative;
            display: inline;
            float: left;
        } 

        /*Top level menu link items style*/
        .horizontalcssmenu ul li a
        {
            
            display: block;
            width: 120px; /*Width of top level menu link items*/
            border: 1px ;/* solid #202020*/
            border-left-width: 0;
            text-decoration: none;
            
        }
              
        .horizontalcssmenu ul li li 
        {
            max-height: 35px;   
        }
	
        /*Sub level menu*/
        /*border-top: 1px solid #202020; */
       .horizontalcssmenu ul li ul
       {
           font-size :x-small;
           
           margin: 0px;
           top: 100%;
           position: absolute;
       }

        

        /*Sub sub level menu list items */
        .horizontalcssmenu ul li ul li li
        {
        
         font-size :x-small;
        position: relative;
        
        display:list-item; 
        
       
        float: left;
        }

                
        .horizontalcssmenu .arrowdiv
        {
                 
        margin-top: 0px;
        position: relative;
        right: 0;
        background: transparent url(menuarrow.gif) no-repeat center left;
        }
         
                    
	
        /* Holly Hack for IE \*/
        * html .horizontalcssmenu ul li { float: left; height: 1%; }
        * html .horizontalcssmenu ul li a { height: 1%; }
        /* End */

        /* Fine aggiunta  stile menu Orizzontale */
        
        
    </style>
    <script type="text/javascript" language="javascript">
        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }
        //        //RICERCA PROPRIETARIO
        //        function _ApriRubricaRicercaProprietario() {
        //            var r = new Rubrica();
        //            r.CallType = r.CALLTYPE_OWNER_AUTHOR;
        //            var res = r.Apri();
        //        }

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

        function SingleSelect(regex, current) {
            re = new RegExp(regex);
            for (i = 0; i < document.forms[0].elements.length; i++) {

                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm != current && re.test(elm.name)) {
                    elm.checked = false;
                }
            }
        }

        function SingleSelect2(regex, current) {
            re = new RegExp(regex);
            for (i = 0; i < document.forms[0].elements.length; i++) {

                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm != current && re.test(elm.name)) {
                    elm.checked = false;
                }
            }
        }

        function showFile(idDocumento, indiceAllegato) {
            window.open("VisualizzaDocumento.aspx?idDocumento=" + idDocumento + "&indiceAllegato=" + indiceAllegato,
                        "", 
                        "dialogWidth:600px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        function OpenCampiProfilati(idTemplate) {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.showModalDialog(_urlCampiProfilati + "&id=" + idTemplate, 'OpenCampiProfilati', 'dialogWidth:750px;dialogHeight:600px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {

            }
        }

        var cssmenuids = ["cssmenu4"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
        var csssubmenuoffset = -2 //Offset of submenus from main menu. Default is 0 pixels.

        function createcssmenu2() {
            for (var i = 0; i < cssmenuids.length; i++) {
                var ultags = document.getElementById(cssmenuids[i]).getElementsByTagName("ul")
                for (var t = 0; t < ultags.length; t++) {
                    ultags[t].style.top = ultags[t].parentNode.offsetHeight - 1 + 'px'
                    var spanref = document.createElement("span")
                    spanref.className = "arrowdiv"
                    spanref.innerHTML = "&nbsp"
                    ultags[t].parentNode.getElementsByTagName("a")[0].appendChild(spanref)
                    ultags[t].parentNode.onmouseover = function () {
                        this.style.zIndex = 100
                        this.getElementsByTagName("ul")[0].style.visibility = "visible"
                        this.getElementsByTagName("ul")[0].style.zIndex = 0
                    }
                    ultags[t].parentNode.onmouseout = function () {
                        this.style.zIndex = 0
                        this.getElementsByTagName("ul")[0].style.visibility = "hidden"
                        this.getElementsByTagName("ul")[0].style.zIndex = 100
                    }

                    if ($('#' + cssmenuids[i] + ' ul.visibility').length > 0) {
                        $('#' + cssmenuids[i] + ' ul.visibility').each(function () {
                            this.style.visibility = "hidden";
                        });
                        $('#' + cssmenuids[i] + ' ul').removeClass('visibility');
                    }
                }
            }
        }

        if (window.addEventListener)
            window.addEventListener("load", createcssmenu2, false)
        else if (window.attachEvent)
            window.attachEvent("onload", createcssmenu2)

    </script>
</head>
<body>
    <form id="form1" runat="server" defaultbutton="btnFind">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <div id="container">
        <div id="header">
            <img src="Img/logo_conservazione.jpg" alt="Pitre - Gestione Conservazione" />
            <h2><asp:Label ID="lbl_amm" runat="server"></asp:Label></h2>
        </div>
        <uc6:Menu ID="menuTop" runat="server" PaginaChiamante="DOCUMENTI" />
        <div id="title">
            <h1>
                Ricerca Documenti</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upFiltriRicerca" runat="server" UpdateMode="Conditional">
                <contenttemplate>             
                    <div class="box_cerca">
                    <div align="center">
                        <fieldset>
                            <legend>Opzioni di ricerca</legend>
                            <asp:CheckBox ID="chkRicercaNotifiche" runat="server" CssClass="testoCheck" AutoPostBack="true" Text="Ricerca notifiche" OnCheckedChanged="chkRicercaNotifiche_CheckedChanged" />
                            <table id="tabFiltriNotificheInConservazione" runat="server" class="tabDate2">
                            <tr>
                                <td class="tabDateSx">Data di creazione:</td>
                                <td class="tabDateDx">
                                     <asp:dropdownlist id="ddl_dataCreazione_E_notifiche" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="130px" OnSelectedIndexChanged="ddl_dataCreazione_E_notifiche_SelectedIndexChanged">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
								    </asp:dropdownlist>
                                    <asp:label id="lblDaNotifiche" runat="server"  Width="18px">Il</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneDaNotifiche" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                    <asp:label id="lblANotifiche" runat="server" Width="18px">A</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneANotifiche" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                </td>
                            </tr>
                              <tr>
                                <td class="tabDateSx">Numero istanza:</td>
                                <td class="tabDateDx">
                                    <asp:textbox id="txt_num_istanza" runat="server" Cssclass="testo_grigio_no_spazio" maxlength="10"></asp:textbox>       
                                </td>
                            </tr>
                            </table>

                            <table id="tabFiltriDocumentiInConservazione" runat="server" class="tabDate2">
                            <tr>
                                <td class="tabDateSx">Tipologia:</td>
                                <td class="tabDateDx">
                                    <asp:CheckBox Text="Arrivo" Value="A" id="chk_Arr" runat="server" />
                                    <asp:CheckBox Text="Partenza" Value="P" id="chk_Part" runat="server" />
                                    <asp:CheckBox Text="Interno" Value="I" id="chk_Int" runat="server"/>
                                   <asp:CheckBox Text="Non Protocollato" Value="G" id="chk_Grigio" runat="server"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="tabDateSx">Data di creazione:</td>
                                <td class="tabDateDx">
                                     <asp:dropdownlist id="ddl_dataCreazione_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="130px" OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
								    </asp:dropdownlist>
                                    <asp:label id="lblDa" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneDa" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                    <asp:label id="lblA" runat="server" CssClass="testo_grigio_no_spazio">A</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneA" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Data di protocollazione:</td>
                            <td class="tabDateDx">
                                    <asp:dropdownlist id="ddl_dataProt_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="130px" OnSelectedIndexChanged="ddl_dataProt_E_SelectedIndexChanged">
										<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
									    <asp:ListItem Value="1">Intervallo</asp:ListItem>
									    <asp:ListItem Value="2">Oggi</asp:ListItem>
								        <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								        <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
									</asp:dropdownlist>
                                    <asp:label id="lblDaP" runat="server"  CssClass="testo_grigio_no_spazio">Il</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneDaP" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                    <asp:label id="lblAP" runat="server" CssClass="testo_grigio_no_spazio">A</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneAP" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Numero protocollo:</td>
                            <td class="tabDateDx">  
                               <asp:dropdownlist id="ddl_numProt_E" runat="server" autopostback="True" cssclass="testo_grigio" width="130px" OnSelectedIndexChanged="ddl_numProt_E_SelectedIndexChanged">
							        <asp:ListItem Value="0" Selected="True">Valore Singolo</asp:ListItem>
									<asp:ListItem Value="1">Intervallo</asp:ListItem>
							   </asp:dropdownlist>
                                   <asp:label id="lblDAnumprot_E" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:label>
                                   <asp:textbox id="txt_initNumProt_E" runat="server" Cssclass="calendarBox"></asp:textbox>
                                   <asp:label id="lblAnumprot_E" runat="server" CssClass="testo_grigio_no_spazio">A</asp:label>
                                   <asp:textbox id="txt_fineNumProt_E" runat="server" Cssclass="calendarBox"></asp:textbox>
                                </td>
                            </tr>
                            <tr>
                             <td class="tabDateSx">Id documento:</td>
                            <td class="tabDateDx"> 
                                <asp:dropdownlist id="ddl_idDocumento_C" runat="server" CssClass="testo_grigio" Width="130px" AutoPostBack="true" OnSelectedIndexChanged="ddl_idDocumento_C_SelectedIndexChanged" >
							        <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
								    <asp:ListItem Value="1">Intervallo</asp:ListItem>
							    </asp:dropdownlist>
                                <asp:label id="lblDAidDoc_C" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:label>
                                <asp:textbox id="txt_initIdDoc_C" runat="server" CssClass="calendarBox"></asp:textbox>
                                <asp:label id="lblAidDoc_C" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:label>
                                <asp:textbox id="txt_fineIdDoc_C" runat="server" CssClass="calendarBox" Visible="False"></asp:textbox>
                            </td>
                            </tr> 
                            <tr>
                            <td class="tabDateSx">Registro:</td>
                            <td class="tabDateDx">  
                                <asp:DropDownList ID="ddl_aoo" runat="server" CssClass="ddl_list_no_space"  AutoPostBack="true">
                                    </asp:DropDownList>
                            </td>
                            </tr>
                             <tr>
                            <td class="tabDateSx">Anno:</td>
                            <td class="tabDateDx">  
                               <asp:textbox id="tbAnnoProtocollo" runat="server" Cssclass="testo_grigio_no_spazio" maxlength="4"></asp:textbox>
                               </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Proprietario:</td>
                            <td class="tabDateDx">
                                <asp:TextBox ID="txtCodRuolo" runat="server" CssClass="testo_grigio" Width="90" MaxLength="30" Height="16" OnTextChanged="txtCodRuolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                <asp:TextBox ID="txtDescRuolo" runat="server" CssClass="testo_grigio" Width="370" MaxLength="30" Enabled="false" Height="16"></asp:TextBox>
                               <%-- <asp:ImageButton ID="btnApriRubrica" Width="30px" AlternateText="Seleziona da Rubrica" ToolTip="Seleziona da Rubrica" ImageUrl="Img/rubrica.gif" runat="server" Height="20px"></asp:ImageButton>                                       --%>
                                <asp:HiddenField ID="id_corr" runat="server" />
                                <asp:HiddenField ID="tipo_corr" runat="server" />
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Oggetto:</td>
                            <td class="tabDateDx">
                            <asp:textbox id="txt_oggetto" runat="server"  Width="470" Height="32px" TextMode="MultiLine"></asp:textbox></td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Mittente/Destinatario:</td>
                            <td class="tabDateDx">
                                <asp:TextBox ID="txtCodMitt" runat="server" CssClass="testo_grigio" Width="90" Height="16" MaxLength="30" OnTextChanged="txtCodMitt_TextChanged" AutoPostBack="true"></asp:TextBox>
                                <asp:TextBox ID="txtDescMitt" runat="server" CssClass="testo_grigio" Width="370" Height="16" MaxLength="30" Enabled="false"></asp:TextBox>
                               <%-- <asp:ImageButton ID="btnApriRubrica" Width="30px" AlternateText="Seleziona da Rubrica" ToolTip="Seleziona da Rubrica" ImageUrl="Img/rubrica.gif" runat="server" Height="20px"></asp:ImageButton>                                       --%>
                                <asp:HiddenField ID="id_corr_mitt" runat="server" />
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Codice Classifica:</td>
                            <td class="tabDateDx">
                             <asp:TextBox ID="txtCodFascicolo" runat="server" CssClass="testo_grigio" Width="90" Height="16" MaxLength="30" OnTextChanged="txtCodFascicolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:TextBox ID="txtDescFascicolo" runat="server" CssClass="testo_grigio" Height="16" Width="370" MaxLength="30" Enabled="false"></asp:TextBox>
                                <asp:HiddenField ID="is_fasc" runat="server" />
                                <asp:HiddenField ID="id_Fasc" runat="server" />
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Tipologia del documento</td>
                            <td class="tabDateDx">  
                            <asp:DropDownList ID="ddl_type_documents" runat="server" CssClass="ddl_list_no_space" Width="440" OnSelectedIndexChanged="ChangeTypeDocument" AutoPostBack="true">
                                    </asp:DropDownList>
                                    <asp:ImageButton ID="btnCampiProfilati" runat="server" OnClick="ViewCampiProlilati" ImageUrl="~/Img/ico_oggettario.gif" Enabled="false" ToolTip="Cerca per campi profilati" AlternateText="Cerca per campi profilati"/>                         
                            </td>
                            </tr>
                             <tr>
                            <td class="tabDateSx">Segnatura</td>
                            <td class="tabDateDx">  
                            <asp:textbox id="txt_segnatura" Width="470" CssClass="testo_grigio" Runat="server" Height="16"></asp:textbox>           
                            </td>
                            </tr>
                             <tr>
                            <td class="tabDateSx">Segnatura di Emergenza</td>
                            <td class="tabDateDx">  
                            <asp:textbox id="txt_protoEme" runat="server" CssClass="testo_grigio" Width="470" Height="16"></asp:textbox>          
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Tipo file acquisito</td>
                            <td class="tabDateDx">
                             <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" CssClass="ddl_list_no_space_little">
							     <asp:ListItem></asp:ListItem>
							 </asp:DropDownList>
                             <asp:CheckBox ID="chkFirmato" runat="server" CssClass="testo_grigio"  Text="Firmati (P7M)" onclick="SingleSelect('chk',this);"/>
							 <asp:CheckBox ID="chkNonFirmato" runat="server" CssClass="testo_grigio" Text="Non firmati" onclick="SingleSelect('chk',this);" />
                            </td>
                            </tr>
                             <tr>
                            <td class="tabDateSx">Timestamp</td>
                            <td class="tabDateDx">
                             <asp:CheckBox ID="ctTimestamp" runat="server" CssClass="testo_grigio"  Text="Con Timestamp" onclick="SingleSelect2('ct',this);"/>
							 <asp:CheckBox ID="ctNoTimestamp" runat="server" CssClass="testo_grigio" Text="Senza Timestamp" onclick="SingleSelect2('ct',this);" />
                            </td>
                            </tr>
                            </table>
                            <div align="center" style="margin-top:5px;padding-top:5px;">
                            <a href="#risultatoButton" title=""></a>
                                <asp:Button ID="btnFind" runat="server" OnClick="BtnSearch_Click" Text="Cerca" CssClass="cbtn"/>
                            </div>
                        </fieldset>
                        </div>
                    </div>
                </contenttemplate>
            </asp:UpdatePanel>
            <a name="#risultato" id="risultato"></a>
            <asp:UpdatePanel ID="upRisultati" runat="server" UpdateMode="Conditional">
                <contenttemplate>   
                <asp:Panel id="pnl_result" visible="false" runat="server">
                   <asp:DataGrid ID="dgResult" runat="server" Width="100%" AllowSorting="false" AutoGenerateColumns="false"
                    HorizontalAlign="Center" AllowPaging="true" PageSize="10" AllowCustomPaging="true"
                    ShowHeader="true" 
                        OnPageIndexChanged="dgResult_SelectedPageIndexChanged"
                        OnItemCommand="dgResult_ItemCommand"
                        OnItemCreated="dgResult_ItemCreated" 
                        OnPreRender="dgResult_PreRender"
                     CssClass="tab_istanze">
                    <HeaderStyle CssClass="tab_istanze_header"  />
                    <AlternatingItemStyle CssClass="tab_istanze_a"  />
                    <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                    <PagerStyle CssClass="menu_pager_grigio"  Mode="NumericPages" Position="TopAndBottom"/>
                     <Columns>
                        <asp:TemplateColumn Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetSystemID((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="12%" HeaderText="Data Creazione">
                            <ItemTemplate>
                                <asp:Label ID="dataCreazione" runat="server" Text='<%# this.GetDataCreazione((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="7%" HeaderText="TIPO">
                            <ItemTemplate>
                                <asp:Label ID="tipo" runat="server" Text='<%# this.GetTipo((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="10%" HeaderText="ID/SEGNATURA">
                            <ItemTemplate>
                                <asp:Label ID="idSegnatura" runat="server" Visible="<%#!this.IsRicercaNotifiche()%>" Text='<%# this.GetIdSegnatura((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                                <asp:LinkButton ID="idSegnaturaLink" runat="server" ToolTip="Visualizza il documento di notifica" Visible="<%#this.IsRicercaNotifiche()%>" CommandName="VISUALIZZA_REPORT_PDF" Text='<%# this.GetIdSegnatura((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="15%" HeaderText="OGGETTO">
                            <ItemTemplate>
                                <asp:Label ID="oggetto" runat="server" Text='<%# this.GetOggetto((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="10%" HeaderText="REGISTRO">
                            <ItemTemplate>
                                <asp:Label ID="registro" runat="server" Text='<%# this.GetRegistro((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="10%" HeaderText="TIPO FILE">
                            <ItemTemplate>
                                <asp:Label ID="tipoFile" runat="server" Text='<%# this.GetTipoFile((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="15%" HeaderText="TIPOLOGIA">
                            <ItemTemplate>
                                <asp:Label ID="tipologia" runat="server" Text='<%# this.GetTipologia((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="21%" HeaderText="ISTANZE">
                            <ItemTemplate>
                                <%# this.GetIstanze((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>
                            </ItemTemplate>
                        </asp:TemplateColumn>
<%--                        <asp:TemplateColumn ItemStyle-Width="10%">
                            <ItemTemplate>
                                <asp:Button ID="btnVisualizzaReportPdf" runat="server" CommandName="VISUALIZZA_REPORT_PDF" Text="Visualizza" CssClass="cbtn"/>
                                <asp:Button ID="btnVisualizzaFileChiusura" runat="server" CommandName="VISUALIZZA_FILE_CHIUSURA_XML" Text="File di chiusura" CssClass="cbtn"/>
                                <asp:Button ID="btnVisualizzaFileChiusuraFirmato" runat="server" CommandName="VISUALIZZA_FILE_CHIUSURA_P7M" Text="File di chiusura firmato" CssClass="cbtn"/>
                                <asp:Button ID="btnVisualizzaMarca" runat="server" CommandName="VISUALIZZA_FILE_MARCATO" Text="Marca temporale" CssClass="cbtn"/>
                            </ItemTemplate>
                        </asp:TemplateColumn>
--%>                    </Columns>
                </asp:DataGrid>
                         </asp:Panel>
                 </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- PopUp Wait-->
    <cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
    <div id="Wait" runat="server" style="font-weight: normal; font-size: 13px; font-family: Arial;
        text-align: center; display: none;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <contenttemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="Img/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </contenttemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
