<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaIstanzeEsibizione.aspx.cs" Inherits="ConservazioneWA.Esibizione.RicercaIstanzeEsibizione" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Menu.ascx" TagName="Menu" TagPrefix="uc6" %>
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<%@ Register Assembly="MessageBox" Namespace="Utilities" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script src="../Script/jquery-1.10.2.min.js" type="text/javascript"></script>
    <title>Conservazione</title>
    <script src="../Script/jquery-1.10.2.min.js"
        type="text/javascript"></script>
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        #header 
        {
            background: url(../Img/bg_header.png) repeat-x scroll;
        }
        
        #menutop
        {
            background: url(../Img/bg_menutop.png) repeat-x scroll;
        }
        
        .altro a:link
        {
            background-image: url('../Img/bg_menutop_no_hover.png');
        }
        
        .altro a:visited
        {
            background-image: url('../Img/bg_menutop_no_hover.png');
        }
        
        .altro a:hover
        {
            background-image: url('../Img/bg_menutop_hover.png');
        }
        
        .sonoqui a:link
        {
            background-image: url('../Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:visited
        {
            background-image: url('../Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:hover
        {
            background-image: url('../Img/sono_qui_hover.png');
        }
        
        .cbtn
        {
            background-image: url('../Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('../Img/bg_button_hover.jpg');
        }
        
        /*GM 26-7-2013
         modifica per evitare border doppio nel radiobuttonlist */
        .testoCheck td
        {
            border:none;
        }
        
        .tab_istanze_header
        {
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('../Img/bg_content.jpg');
        }
        
        .menu_pager_grigio
        {
            background-image: url('../Img/bg_pager_table.jpg');
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

        function confirmThis(btnValue) {
            //RICHIEDI CERTIFICAZIONE
            if (btnValue == 'richiediCert') {
                if (confirm('Richiedere la certificazione dell\'istanza di esibizione?')) {
                    form1.hd_richiediCert.value = true;
                }
                else {
                    form1.hd_richiediCert.value = false;
                }
            }
            //CHIUDI ISTANZA
            if (btnValue == 'chiudi') {
                if (confirm('Chiudere l\'istanza di esibizione?\nUna volta chiusa l\'istanza non sarà più possibile richiederne la certificazione.')) {
                    form1.hd_chiudi.value = true;
                }
                else {
                    form1.hd_chiudi.value = false;
                }
            }


            //RIABILITA ISTANZA
            if (btnValue == 'riabilita') {
                if (confirm('Riabilitare l\'istanza di esibizione?')) {
                    form1.hd_riabilita.value = true;
                }
                else {
                    form1.hd_richiediCert.value = false;
                }
            }

            //ELIMINA DOCUMENTO
            if (btnValue == 'elimina') {
                if (confirm('Rimuovere i documenti selezionati dall\'istanza di esibizione?')) {
                    form1.hd_eliminaDoc.value = true;
                }
                else {
                    form1.hd_eliminaDoc.value = false;
                }
            }

            //ELIMINA ISTANZA
            if (btnValue == 'eliminaIst') {
                if (confirm("Eliminare l'istanza di esibizione?")) {
                    form1.hd_eliminaIst.value = true;
                }
                else {
                    form1.hd_eliminaIst.value = false;
                }
            }
        }

        function controllaDescrizione() {

            alert("Inserire una descrizione per l'istanza.");
        }

        function controllaRiabilitazione() {
            alert("Esiste già un\'istanza in stato nuova. Non è possibile riabilitare l\'istanza.");
        }

        function riabilitaOK() {
            alert("L\'istanza è stata riabilitata con successo.");
        }

        function showDialogFirma(idIstanza,idCertificazione,daFirmare) {
            if (daFirmare == 'y') {
                var returnValue = window.showModalDialog("../CertificaIstanza.aspx?idEsibizione=" + idIstanza + "&idCertificazione=" + idCertificazione + "&firma=" + daFirmare, "", "dialogWidth:640px;dialogHeight:640px;status:no;resizable:no;scroll:no;center:yes;help:no");
            }
            else {
                var returnValue = window.showModalDialog("../CertificaIstanza.aspx?idEsibizione=" + idIstanza + "&idCertificazione=" + idCertificazione + "&firma=" + daFirmare, "", "dialogWidth:700px;dialogHeight:620px;status:no;resizable:no;scroll:no;center:yes;help:no");
            }
        }

        function showDialogRifiuta() {

            var returnValue = window.showModalDialog("popupRifiuto.aspx", "", "dialogWidth:400px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no");
            form1.hd_noteRifiuto.value = returnValue;

        }

        function downloadurl(url) {
            window.open(url);
        }

        function alertPar() {

            var pattern = /^[0-9]+$/;
            var patternDate = /^([0-9]{2})([\/])([0-9]{2})([\/])([0-9]{4})$/;
            if (window.form1.txt_idIstanza.value != "") {
                if (!pattern.test(window.form1.txt_idIstanza.value)) {
                    alert('Formato id istanza non valido');
                    return false;
                }
            }

            if (document.getElementById('txt_dataAp_da_txt_Data') != null) {
                if (document.getElementById('txt_dataAp_da_txt_Data').value != "") {
                    if (!patternDate.test(document.getElementById('txt_dataAp_da_txt_Data').value)) {
                        alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                        window.form1.txt_dataAp_da_txt_Data.focus();
                        return false;
                    }
                }
            }
            if (document.getElementById('txt_dataAp_a_txt_Data') != null) {
                if (document.getElementById('txt_dataAp_a_txt_Data').value != "") {
                    if (!patternDate.test(document.getElementById('txt_dataAp_a_txt_Data').value)) {
                        alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                        window.form1.txt_dataAp_a_txt_Data.focus();
                        return false;
                    }
                }
            }
            if (document.getElementById('dataInvio_da_txt_Data') != null) {
                if (document.getElementById('dataInvio_da_txt_Data').value != "") {
                    if (!patternDate.test(document.getElementById('dataInvio_da_txt_Data').value)) {
                        alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                        window.form1.dataInvio_da_txt_Data.focus();
                        return false;
                    }
                }
            }
            if (document.getElementById('dataInvio_a_txt_Data') != null) {
                if (document.getElementById('dataInvio_a_txt_Data').value != "") {
                    if (!patternDate.test(document.getElementById('dataInvio_a_txt_Data').value)) {
                        alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                        window.form1.dataInvio_a_txt_Data.focus();
                        return false;
                    }
                }
            }
            if (document.getElementById('dataCons_da_txt_Data') != null) {
                if (document.getElementById('dataCons_da_txt_Data').value != "") {
                    if (!patternDate.test(document.getElementById('dataCons_da_txt_Data').value)) {
                        alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                        window.form1.dataCons_da_txt_Data.focus();
                        return false;
                    }
                }
            }
            if (document.getElementById('dataCons_a_txt_Data') != null) {
                if (document.getElementById('dataCons_a_txt_Data').value != "") {
                    if (!patternDate.test(document.getElementById('dataCons_a_txt_Data').value)) {
                        alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                        window.form1.dataCons_a_txt_Data.focus();
                        return false;
                    }
                }
            }
        }

        var abilita = 'false';

        var cssmenuids = ["cssmenu9"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
        var csssubmenuoffset = -2 //Offset of submenus from main menu. Default is 0 pixels.

        function createcssmenu9() {
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
            window.addEventListener("load", createcssmenu9, false)
        else if (window.attachEvent)
            window.attachEvent("onload", createcssmenu9)

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
            <img src="../Img/logo_conservazione.jpg" alt="Pitre - Gestione Conservazione" />
            <asp:Panel ID="pnl_cons" runat="server" Visible="true">
                <h2>
                    <asp:Label ID="lbl_amm_cons" runat="server"></asp:Label>
                </h2>
            </asp:Panel>
            <!-- MEV CS 1.4 - Esibizione -->
                <asp:UpdatePanel ID="uPnl_SelectedRole" runat="server" UpdateMode="Always" Visible="false">
                    <ContentTemplate>
                        <h3>
                            <asp:Label ID="lbl_amm" runat="server"></asp:Label><br />
                            <asp:Label ID="lbl_selectedRole" runat="server"></asp:Label>
                        </h3>
                    </ContentTemplate>
                </asp:UpdatePanel>

        </div>
        <uc6:menu id="menuTop" runat="server" paginachiamante="ESIBIZIONE" />
        <div id="title">
            <h1>
                Ricerca Istanze di Esibizione</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upContent" runat="server">
                <contenttemplate>
                    <div class="box_cerca">
                        <div align="center">
                            <fieldset>
                                <legend>Opzioni di ricerca</legend>
                                <asp:CheckBoxList runat="server" ID="chkTipo" RepeatDirection="Horizontal" CssClass="testoCheck">
                                    <asp:ListItem Text="Nuove" Value="N"></asp:ListItem>
                                    <asp:ListItem Text="Da certificare" Value="I"></asp:ListItem>
                                    <asp:ListItem Text="Rifiutate" Value="R"></asp:ListItem>
                                    <asp:ListItem Text="In chiusura" Value="T"></asp:ListItem>
                                    <asp:ListItem Text="Chiuse" Value="C"></asp:ListItem>
                                </asp:CheckBoxList>
                                <table class="tabDate">
                                    <tr>
                                        <td class="tabDateSx">
                                            Numero istanza:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:TextBox ID="txt_idIstanza" runat="server" CssClass="testo_grigio_spazio"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Descrizione:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:TextBox ID="txt_descIstanza" runat="server" CssClass="testo_grigio_spazio"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Tipologia:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:CheckBoxList runat="server" ID="chkTipologia" RepeatDirection="Horizontal" CssClass="testoCheck" Style="border: none;" >
                                                <asp:ListItem Text="Con certificazione" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Senza certificazione" Value="-1"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Data Creazione:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:DropDownList runat="server" ID="ddl_dataCr" AutoPostBack="true" CssClass="ddl_list" Width="140px" OnSelectedIndexChanged="ddl_DataCr_IndexChanged">
                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="lbl_dataCr_da" runat="server" Text="Da" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataCr_da" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                            <asp:Label ID="lbl_dataCr_a" runat="server" Text="a" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataCr_a" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Data Certificazione:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:DropDownList runat="server" ID="ddl_dataCert" AutoPostBack="true" CssClass="ddl_list" Width="140px" OnSelectedIndexChanged="ddl_DataCert_IndexChanged">
                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="lbl_dataCert_da" runat="server" Text="Da" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataCert_da" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                            <asp:Label ID="lbl_dataCert_a" runat="server" Text="a" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataCert_a" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Data Chiusura:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:DropDownList runat="server" ID="ddl_dataChiusura" AutoPostBack="true" CssClass="ddl_list" Width="140px" OnSelectedIndexChanged="ddl_DataChiusura_IndexChanged">
                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="lbl_dataChiusura_da" runat="server" Text="Da" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataChiusura_da" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                            <asp:Label ID="lbl_dataChiusura_a" runat="server" Text="a" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataChiusura_a" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Data Rifiuto:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:DropDownList runat="server" ID="ddl_dataRifiuto" AutoPostBack="true" CssClass="ddl_list" Width="140px" OnSelectedIndexChanged="ddl_DataRifiuto_IndexChanged">
                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Intervallo</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="lbl_dataRifiuto_da" runat="server" Text="Da" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataRifiuto_da" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                            <asp:Label ID="lbl_dataRifiuto_a" runat="server" Text="a" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataRifiuto_a" runat="server" Visible="true" PAGINA_CHIAMANTE="ISTANZEESIBIZIONE" />
                                        </td>
                                    </tr>
                                </table>
                                <div align="center" style="margin-top: 5px; padding-top: 5px; border-top: 1px dashed #eaeaea;">
                                    <asp:Button ID="btnFind" runat="server" OnClick="BtnSearch_Click" Text="Cerca" CssClass="cbtn" />
                                    <asp:Button ID="btnReset" runat="server" OnClick="BtnReset_Click" Text="Reset Filtri" CssClass="cbtn" />
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="upIstanze" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DataGrid ID="gv_IstanzeEs" runat="server" CssClass="tab_istanze" AllowSorting="true"
                             AutoGenerateColumns="false" Width="100%" SkinID="datagrid" AllowPaging="true" OnPageIndexChanged="SelectedIndexChangedIst"
                             OnPreRender="gv_IstanzeEs_PreRender">
                             <HeaderStyle CssClass="tab_istanze_header" />
                             <AlternatingItemStyle CssClass="tab_istanze_a" />
                             <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                             <SelectedItemStyle CssClass="selected_row" />
                             <PagerStyle CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom" />
                             <Columns>
                                <asp:TemplateColumn HeaderText="ID">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblIdIstanza" Text='<%#DataBinder.Eval(Container.DataItem,"SystemId") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Stato">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblChaStato" Visible="false" Text='<%#DataBinder.Eval(Container.DataItem,"statoEsibizione") %>'></asp:Label>
                                        <asp:Label runat="server" ID="lblStatoIstanza" Text='<%# this.GetStatoIstanza(DataBinder.Eval(Container.DataItem,"statoEsibizione")) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Descrizione">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDescrizione" Text='<%#DataBinder.Eval(Container.DataItem,"Descrizione") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Note">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblNote" Text='<%#DataBinder.Eval(Container.DataItem,"Note") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="Data_Creazione" HeaderText="Data Creazione"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Data_Certificazione" HeaderText="Data Certificazione"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Data_Chiusura" HeaderText="Data Chiusura"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Tipologia">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCertif" Text='<%# this.GetCertificazione(DataBinder.Eval(Container.DataItem,"isRichiestaCertificazione"), DataBinder.Eval(Container.DataItem,"statoEsibizione")) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Richiedente">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRichiedente" Text='<%# this.SetRichiedente(DataBinder.Eval(Container.DataItem,"richiedente")) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Dettaglio">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" ID="btn_visualizza" ImageUrl="~/Img/dett_lente.gif" ToolTip="Visualizza contenuto istanza"
                                        OnClick="btnSelezionaIstanza_Click" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="false">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblIdCert" Visible="false" Text='<%#DataBinder.Eval(Container.DataItem, "idProfileCertificazione") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Note Rifiuto" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblNoteRifiuto" Visible="false" Text='<%#DataBinder.Eval(Container.DataItem, "NoteRifiuto") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>                    
                             </Columns>
                            </asp:DataGrid>
                        </ContentTemplate>
                    </asp:UpdatePanel> 
                    <asp:UpdatePanel ID="upDettaglio" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hd_idIstanza" />
                            <asp:HiddenField runat="server" ID="hd_stato" />
                            <asp:HiddenField runat="server" ID="hd_idCertificazione" />
                            <input type="hidden" runat="server" id="hd_noteRifiuto" />
                            <asp:Panel runat="server" ID="panel_dettaglio" Visible="false">
                                <div class="box_cerca">
                                    <div align="center" style="margin-top: 5px;">
                                        <fieldset>
                                            <legend>Dettaglio istanza</legend>
                                            <table class="tabDate">
                                                <tr>
                                                    <td class="tabDateSx">
                                                        Descrizione:*
                                                    </td>
                                                    <td class="tabDateDx">
                                                        <asp:TextBox runat="server" ID="txtIstDescr" width="90%" CssClass="testo_grigio_scuro"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tabDateSx">
                                                        Note:
                                                    </td>
                                                    <td class="tabDateDx">
                                                        <asp:TextBox runat="server" ID="txtIstNote" width="90%" CssClass="testo_grigio_scuro"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="rowNoteRifiuto" visible="false">
                                                    <td class="tabDateSx">
                                                        Note rifiuto:
                                                    </td>
                                                    <td class="tabDateDx">
                                                        <asp:TextBox runat="server" ID="txtIstNoteRifiuto" Width="90%" CssClass="testo_grigio_scuro" Enabled="false"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="margin-top: 5px;">
                                                <asp:Button runat="server" ID="btnSalva" Text="Salva" width="100px" OnClick="btnSalva_Click" />
                                                <asp:Button runat="server" ID="btnDocumenti" Text="Visualizza" width="100px" OnClick="btnDocumenti_Click" />
                                                <asp:Button runat="server" ID="btnRichiediCertificazione" Text="Richiedi certif." width="100px"
                                                OnClientClick="confirmThis('richiediCert');" OnClick="btnRichiediCertificazione_Click"/>
                                                <asp:Button runat="server" ID="btnCertifica" Text="Certifica" Width="100px" />
                                                <asp:Button runat="server" ID="btnChiudi" Text="Chiudi" width="100px" OnClick="btnChiudi_Click"
                                                OnClientClick="confirmThis('chiudi');" />
                                                <asp:Button runat="server" ID="btnElimina" Text="Elimina" Width="100px" OnClick="btnElimina_Click" 
                                                OnClientClick="confirmThis('eliminaIst');" />
                                                <asp:Button runat="server" ID="btnRifiuta" Text="Rifiuta" width="100px" OnClick="btnRifiuta_Click" 
                                                OnClientClick="showDialogRifiuta();" />
                                                <asp:Button runat="server" ID="btnScarica" Text="Download" Width="100px" OnClick="btnScarica_Click" />
                                                <asp:Button runat="server" ID="btnRiabilita" Text="Riabilita" Width="100px" OnClick="btnRiabilita_Click"
                                                OnClientClick="confirmThis('riabilita');" />
                                                <asp:Button runat="server" ID="btnDocCert" Text="Certificazione" Width="100px" />
                                                <input type="hidden" id="hd_eliminaDoc" runat="server" />
                                                <input type="hidden" id="hd_eliminaIst" runat="server" />
                                                <input type="hidden" runat="server" id="hd_richiediCert" />
                                                <input type="hidden" runat="server" id="hd_chiudi" />
                                                <input type="hidden" runat="server" id="hd_riabilita" />
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="panel_Documenti" Visible="true">
                                <table style="width: 100%; ">
                                    <tr>
                                        <td class="testo_grigio_scuro" style="width: 10%;">
                                            <asp:Label runat="server" ID="lblDimensioneIstanza"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:DataGrid runat="server" ID="gv_DocumentiEs" Width="100%" AutoGenerateColumns="false"
                                            AllowPaging="true" PageSize="10" PagerStyle-Font-Size="9px" Style="margin-top: 0px;"
                                            OnPreRender="gv_DocumentiEs_PreRender" OnPageIndexChanged="SelectedIndexChangedDoc">
                                                <HeaderStyle CssClass="tab_istanze_header" />
                                                <AlternatingItemStyle CssClass="tab_istanze_a" />
                                                <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                                                <PagerStyle CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom" />
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText="ID" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblIdDoc" Text='<%#DataBinder.Eval(Container.DataItem,"SystemId") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="Data_Ins" HeaderText="Data Ins."></asp:BoundColumn>
                                                    <asp:TemplateColumn HeaderText="Tipo" HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblTipo" Text='<%#this.SetTipo(DataBinder.Eval(Container.DataItem,"TipoDoc")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="ID/Segn. Data" HeaderStyle-Width="20%">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblDataProtOrCrea" Text='<%#
                                                            this.GetIdSegnaturaData(DataBinder.Eval(Container.DataItem,"numProt_or_id"),DataBinder.Eval(Container.DataItem,"data_prot_or_create"))     
                                                            %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="numProt" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblNumProt" Text='<%#DataBinder.Eval(Container.DataItem,"numProt") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="desc_oggetto" HeaderText="Oggetto" HeaderStyle-Width="20%"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="numAllegati" HeaderText="N°. all." HeaderStyle-Width="5%"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CodFasc" HeaderText="Cod. Fasc." HeaderStyle-Width="10%"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="tipoFile" HeaderText="Tipo File" HeaderStyle-Width="5%"></asp:BoundColumn>
                                                    <asp:TemplateColumn HeaderText="Dim. File" HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblSize" Text='<%#this.GetTotalSize(DataBinder.Eval(Container.DataItem,"SizeItem")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="ID_Conservazione" HeaderText="Istanza Cons." HeaderStyle-Width="10%"></asp:BoundColumn>
                                                    <asp:TemplateColumn HeaderText="Seleziona" HeaderStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" ID="chkElimina" />
                                                            <!--<asp:ImageButton runat="server" ID="btnEliminaDoc" ImageUrl="~/Img/cancella.gif" OnClick="btnEliminaDoc_Click" 
                                                            ToolTip="Rimuovi documento dall'istanza di esibizione" OnClientClick="confirmThis('eliminaDoc');" /> -->
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                </Columns>                                               
                                            </asp:DataGrid> 
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Panel ID="pnl_bottoniera" runat="server" Visible="false">
                                                <div id="box_button">
                                                    <asp:button ID="btnEliminaDoc" runat="server" Text="Elimina Documenti" OnClientClick="confirmThis('elimina');" 
                                                    OnClick="btnEliminaDoc_Click" />
                                                </div>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>         
                </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- PopUp Wait-->
    <cc2:modalpopupextender id="mdlPopupWait" runat="server" targetcontrolid="Wait" popupcontrolid="Wait"
        backgroundcssclass="modalBackground" behaviorid="mdlWait" />
    <div id="Wait" runat="server" style="font-weight: normal; font-size: 13px; font-family: Arial;
        text-align: center; display: none;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <contenttemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="../Img/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </contenttemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
