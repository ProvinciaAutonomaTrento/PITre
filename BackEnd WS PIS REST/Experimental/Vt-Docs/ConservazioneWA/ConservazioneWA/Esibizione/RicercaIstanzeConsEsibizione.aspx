<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaIstanzeConsEsibizione.aspx.cs" Inherits="ConservazioneWA.Esibizione.RicercaIstanzeConsEsibizione" %>

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
        function showModalDialogFirma() {
            var returnValue = window.showModalDialog("~/DialogFirma.aspx?idIstanza=" + form1.hd_idIstanza.value, "", "dialogWidth:600px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no");
            form1.hd_firma.value = returnValue;
        }

        function showDialogMassiveOperation() {

            var returnValue = window.showModalDialog("PopupMassiveOpeartion.aspx", "", "dialogWidth:600px;dialogHeight:400px;status:no;resizable:no;center:yes;help:no");
        }

        function controllo() {
            var returnvalue = window.showModalDialog("~/PopUp/Note.aspx", "", "dialogWidth:400px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no");
            form1.hf_note.value = returnvalue;
        }

        function notifyEsitoVerificaFirma(isValid, totale, validi, nonValidi) {
            var message = "";

            if (!isValid) {
                message = "L'istanza contiene " + nonValidi + " documento/i e/o allegati la cui CRL della firma digitale risulta non valida.";
            }
            else {
                if (validi == 0 && nonValidi == 0) {
                    message = "Nessun file firmato presente.";
                }
                else {
                    message = "La CRL della firma digitale risulta valida per tutti i documenti firmati digitalmente.";
                }
            }

            alert(message);
        }

        function notifyEsitoVerificaMarca(isValid, totale, validi, nonValidi) {
            var message = "";

            if (!isValid)
                message = "L'istanza contiene " + nonValidi + " documento/i e/o allegati la cui marca temporale risulta non valida.";
            else
                if (validi == 0 && nonValidi == 0) {
                    message = "Nessun file marcato temporalmente.";
                }
                else {
                    message = "La marca temporale risulta valida per tutti i documenti marcati.";
                }

            alert(message);
        }

        function notifyEsitoVerificaFormati(isValid) {
            var message = "";

            if (!isValid)
                message = "L'istanza contiene documenti e/o allegati il cui formato non risponde alle policy di conservazione del Centro Servizi.";
            else
                message = "Tutti i documenti dell'istanza hanno il formato rispondente alle policy di conservazione del Centro Servizi.";

            alert(message);
        }

        function notifyInvalidIstanzaConservazione() {
            var message = "Attenzione!\nL'istanza contiene documenti il cui formato non risponde\nalle policy di conservazione di questo Centro Servizi.\nSi è veramente sicuri di metterla in lavorazione in deroga alle policy?";

            if (confirm(message)) {
                // Istanza creata in deroga alle policy di conservazione
                form1.hd_istanza_in_deroga.value = "true";

                showModalDialogLavorazione();
            }
        }

        function showModalDialogLavorazione(policyInValid) {

            if (policyInValid)
                var returnValue = window.showModalDialog("~/PopUp/NumeroCopie.aspx?policyInvalid=true", "", "dialogWidth:400px;dialogHeight:270px;status:no;resizable:no;scroll:no;center:yes;help:no");
            else
                var returnValue = window.showModalDialog("~/PopUp/NumeroCopie.aspx?policyInvalid=false", "", "dialogWidth:400px;dialogHeight:130px;status:no;resizable:no;scroll:no;center:yes;help:no");

            form1.hd_lavorazione.value = returnValue;
            form1.submit();



        }

        //Funzione per la selezione della modalità di esecuzione della verifica leggibilità (per passare da firmata a chiusa)
        function showTestLeggibilita(idConservazione) {
            var returnvalue = window.showModalDialog("~/PopUp/TestLeggibilita.aspx?idConservazione=" + idConservazione, "", "dialogWidth:800px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        //Funzione per la selezione della modalità di esecuzione della verifica leggibilità (per passare da firmata a chiusa)
        //Test metodo per aggiornamento pagina.
        function showTestLeggibilita2() {
            var returnvalue = window.showModalDialog("~/PopUp/TestLeggibilita.aspx?idConservazione=" + form1.hd_idIstanza.value, "", "dialogWidth:800px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        //Funzione per la verifica leggibilità su di un singolo file
        function showVerificaLeggibilita(idConservazione, file, numero) {
            var returnvalue = window.showModalDialog("~/PopUp/VerificaLeggibilita.aspx?idCons=" + idConservazione + "&file=" + file + "&num=" + numero, "", "dialogWidth:800px;dialogHeight:700px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        //funzione per l'alert in caso di messa in lavorazione senza validare le policy. WIP
        function alertPolicyNonValidata() {
            var returnValue = window.showModalDialog("~/PopUp/AvvisoPolicyNonValida.aspx", "", "dialogWidth:400px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no");
            form1.hd_lavorazione_policy.value = returnValue;

            //            form1.hd_lavorazione.value = returnValue;
            //            form1.submit();
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
        function closePagina() {
            if (confirm("Si desidera uscire dall'applicazione?")) {
                parent.close();
                form1.hd_logOff.value = "true";
                return true;
            }
            else {
                return false;
            }
        }

        var abilita = 'false';

        function showModalDialogVerificaDocumentoSuSupportoRegistrato(idIstanza, idSupporto, idDocumento) {
            var returnValue = window.showModalDialog("~/PopUp/VerificaSupportoRegistrato.aspx?idIstanza=" + idIstanza + "&idSupporto=0&idDocumento=" + idDocumento,
                            "",
                            "dialogWidth:600px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no");


            return returnValue;
        }

        //Funzione per la verifica dell'integrita su di un singolo file all'interno dello storage remoto.
        function showVerificaIntegritaStorage(idConservazione, idSupporto, idDocumento) {
            var returnvalue = window.showModalDialog("~/PopUp/VerificaSupporto.aspx?idIstanza=" + idConservazione + "&idSupporto=" + idSupporto + "&idDocumento=" + idDocumento, "", "dialogWidth:200px;dialogHeight:50px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        var cssmenuids = ["cssmenu2"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
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
            <img src="../Img/logo_conservazione.jpg" alt="Pitre - Gestione Conservazione" />
            <%--<h2>
                <asp:Label ID="lbl_amm" runat="server"></asp:Label></h2>--%>
            <!-- MEV CS 1.4 - Esibizione -->
            <h3>
                <asp:Label ID="lbl_amm" runat="server"></asp:Label>
                <br />
                <asp:UpdatePanel ID="uPnl_SelectedRole" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Label ID="lbl_selectedRole" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </h3>
            <!-- MEV CS 1.4 - Esibizione -->
        </div>
        <uc6:menu id="menuTop" runat="server" paginachiamante="ISTANZE_ESIBIZIONE" />
        <div id="title">
            <h1>
                Ricerca Istanze</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upContent" runat="server">
                <contenttemplate>
                    <div class="box_cerca">
                        <div align="center">
                            <fieldset>
                                <legend>Opzioni di ricerca</legend>
                                <asp:CheckBoxList runat="server" ID="chkTipo" RepeatDirection="Horizontal" CssClass="testoCheck" Visible="false">
                                    <%--<asp:ListItem Text="Nuove" Value="I"></asp:ListItem>--%>
                                    <%--<asp:ListItem Text="In lavorazione" Value="L"></asp:ListItem>--%>
                                    <%--<asp:ListItem Text="Rifiutate" Value="R"></asp:ListItem>--%>
                                    <%--<asp:ListItem Text="Firmate" Value="F"></asp:ListItem>--%>
                                    <asp:ListItem Text="Conservata" Value="V"></asp:ListItem>
                                    <asp:ListItem Text="Chiusa" Value="C"></asp:ListItem>
                                    <%--<asp:ListItem Text="Errore" Value="E"></asp:ListItem>--%>
                                </asp:CheckBoxList>
                                <table class="tabDate">
                                    <tr>
                                        <td class="tabDateSx">
                                            Data apertura:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:Label ID="lbl" runat="server" Text="Da" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataAp_da" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZECONS_ESIBIZIONE" />
                                            <asp:Label ID="Label3" runat="server" Text="a" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="txt_dataAp_a" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZECONS_ESIBIZIONE" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Data invio:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:Label ID="Label6" runat="server" Text="Da" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="dataInvio_da" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZECONS_ESIBIZIONE" />
                                            <asp:Label ID="Label5" runat="server" Text="a" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="dataInvio_a" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZECONS_ESIBIZIONE" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Tipo di conservazione:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:DropDownList ID="cboTipiConservazione" runat="server" DataValueField="Codice"
                                                DataTextField="Descrizione" CssClass="ddl_list">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">
                                            Numero istanza:
                                        </td>
                                        <td class="tabDateDx">
                                            <asp:TextBox ID="txt_idIstanza" runat="server" CssClass="testo_grigio_spazio"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <div align="center" style="margin-top: 5px; padding-top: 5px; border-top: 1px dashed #eaeaea;">
                                    <asp:Button ID="btnFind" runat="server" OnClick="BtnSearch_Click" Text="Cerca" CssClass="cbtn" />
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="upIstanze" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DataGrid ID="gv_istanzeCons" runat="server" CssClass="tab_istanze" AllowSorting="True"
                                AutoGenerateColumns="False" Width="100%" SkinID="datagrid" 
                                AllowPaging="True" OnPageIndexChanged="SelectedIndexChanged"
                              OnPreRender="gv_istanzeCons_PreRender" 
                                OnItemDataBound="DataGrid_ItemCreated">
                                <HeaderStyle CssClass="tab_istanze_header" />
                                <AlternatingItemStyle CssClass="tab_istanze_a" />
                                <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                                <SelectedItemStyle CssClass="selected_row" />
                                <PagerStyle CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom" />
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Stato">
                                        <ItemTemplate>
                                            <asp:Label ID="lb_statoIstanza" runat="server" Text='<%# Bind("stato") %>'></asp:Label>
                                            <asp:Label ID="lbl_IsInPreparazione" runat="server" Text='<%# Bind("isInPreparazione") %>'
                                                Visible="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Tipo Cons.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_tipoCons" runat="server" Text='<%#this.GetTipologiaIstanza(Container.DataItem)%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn DataField="note" HeaderText="Note" />
                                    <asp:BoundColumn DataField="descrizione" HeaderText="Descrizione" 
                                        ItemStyle-HorizontalAlign="Left" >
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="supporto" HeaderText="Supporto" Visible="false" />
                                    
                               <%--    <asp:BoundColumn DataField="data_apertura" HeaderText="Aperta il" />
                                        <asp:BoundColumn DataField="data_invio" HeaderText="Inviata il" />
                                         <asp:BoundColumn DataField="data_cons" HeaderText="Chiusa il" />

                                         <asp:BoundColumn DataField="data_riv" HeaderText="Data riversamento" Visible="false" />
                                         <asp:BoundColumn DataField="data_prox_ver" HeaderText="Data prossima verifica" Visible="false" />
                                         <asp:BoundColumn DataField="data_ultima_ver" HeaderText="Data ultima verifica" Visible="false" />
                                 --%>
                                    <asp:TemplateColumn HeaderText="Aperta il">
                                         <ItemTemplate>
                                              <asp:Literal ID="DataApertura" runat="server" Text='<%# Eval("data_apertura", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                         </ItemTemplate>
                                    </asp:TemplateColumn>
                                     <asp:TemplateColumn HeaderText="Inviata il">
                                         <ItemTemplate>
                                              <asp:Literal ID="DataInvio" runat="server" Text='<%# Eval("data_invio", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                         </ItemTemplate>
                                    </asp:TemplateColumn>
                                     <asp:TemplateColumn HeaderText="Chiusa il">
                                         <ItemTemplate>
                                              <asp:Literal ID="DataCons" runat="server" Text='<%# Eval("data_cons", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                         </ItemTemplate>
                                    </asp:TemplateColumn>                                  
                                    <asp:TemplateColumn HeaderText="Data riversamento" Visible="false">
                                         <ItemTemplate>
                                              <asp:Literal ID="data_riv" runat="server" Text='<%# Eval("data_riv", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                         </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Data prossima verifica" Visible = "false">
                                         <ItemTemplate>
                                              <asp:Literal ID="data_ultima_ver" runat="server" Text='<%# Eval("data_ultima_ver", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                         </ItemTemplate>
                                    </asp:TemplateColumn>
                                     <asp:TemplateColumn HeaderText="Data ultima verifica" Visible = "false">
                                         <ItemTemplate>
                                              <asp:Literal ID="data_prox_ver" runat="server" Text='<%# Eval("data_prox_ver", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                         </ItemTemplate>
                                    </asp:TemplateColumn>                                   
                                    <asp:BoundColumn DataField="marca_temp" HeaderText="Marca temporale" Visible="false" />
                                    <asp:BoundColumn DataField="firma" HeaderText="Firma" Visible="false" />
                                    <asp:BoundColumn DataField="loc_fisica" HeaderText="Locazione fisica" Visible="false" />
                                    <asp:TemplateColumn HeaderText="Istanza N.">
                                        <EditItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idIstanza") %>'></asp:Label>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_idIstanza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idIstanza") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn DataField="copie" HeaderText="Numero copie" Visible="false" />
                                    <asp:TemplateColumn HeaderText="Richiedente">
                                        <EditItemTemplate>
                                            <asp:Label ID="lbl_utRuolo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ruolo") %>'></asp:Label>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_utenteRuolo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ruolo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    
                                    <asp:TemplateColumn HeaderText="Contenuto">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btn_visualizza" runat="server" ImageUrl="../Img/dett_lente.gif"
                                                ToolTip="Visualizza contenuto istanza" OnClick="BtnSelezionaIstanza_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Invio Esibizione">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btn_invio_esib_massivo" runat="server" ImageUrl="../Img/conservazione_d.gif"
                                                ToolTip="Invio contenuto istanza in Esibizione" OnClick="BtnIvioEsibMass_Click" Visible="<%#this.IsContenutoIstanzaVisible(Container.DataItem) %>" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn Visible="false">
                                        <ItemTemplate>
                                            <asp:Button ID="btn_dettSupp" runat="server" Text="Visualizza supporti"
                                                OnClick="VediSupporto" Enabled="<%#this.IsSupportiAvailable(Container.DataItem)%>"
                                                ToolTip="Visualizza i supporti dell'istanza" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="idpolicyvalidazione" ItemStyle-Width="100px" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="idpolicyvalidazione" runat="server" Text='<%# Bind("idPolicyValidata") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Verifiche" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_validationMask" runat="server" Text='<%# Bind("validation_mask") %>' Visible="false"></asp:Label>
                                            <asp:Image ID="img_validationMask" runat="server" Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                            </asp:DataGrid>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="upDettaglio" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="Panel_dettaglio" runat="server" Visible="false">
                                <asp:HiddenField ID="hd_idIstanza" runat="server" />
                                <asp:HiddenField ID="hd_statoIstanza" runat="server" />
                                <asp:HiddenField ID="hd_idPolicy" runat="server" />
                                <asp:HiddenField ID="hd_istanza_in_deroga" runat="server" />
                                <asp:HiddenField ID="hd_filtro" runat="server" />
                                <asp:HiddenField ID="hd_verificaFirmaEffettuata" runat="server" />
                                <asp:HiddenField ID="hd_verificaMarcaEffettuata" runat="server" />
                                <asp:HiddenField ID="hd_verificaFormatiEffettuata" runat="server" />
                                <asp:HiddenField ID="hd_almostOneInvalidDocumentFormat" runat="server" />
                                <input type="hidden" name="hd_lavorazione" id="hd_lavorazione" runat="server" />
                                <input type="hidden" name="hd_lavorazione_policy" id="hd_lavorazione_policy" runat="server" />
                                <input type="hidden" name="hf_note" id="hf_note" runat="server" />
                                <input type="hidden" name="hd_firma" id="hd_firma" runat="server" />
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="testo_grigio_scuro">
                                            <asp:Label ID="lblDimesioneIstanza" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:DataGrid ID="gv_dettaglioCons" runat="server" AutoGenerateColumns="False" Width="100%"
                                                PagerStyle-Font-Size="9px" AllowPaging="True" PageSize="10" ondatabound="gv_dettaglioCons_DataBound"
                                                onpageindexchanging="gv_dettaglioCons_PageIndexChanging" Style="margin-top: 0px"
                                                OnPreRender="gv_dettaglioCons_PreRender" OnItemCommand="gv_dettaglioCons_OnItemCommand"
                                                OnPageIndexChanged="SelectedIndexChangedDoc" OnItemDataBound="ImageCreatedRender">
                                                <HeaderStyle CssClass="tab_istanze_header" />
                                                <AlternatingItemStyle CssClass="tab_istanze_a" />
                                                <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                                                <PagerStyle CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom" />
                                                <Columns>
                                                    <asp:BoundColumn DataField="docNumber" HeaderText="ID DOCUMENTO" Visible="false" />
                                                    <asp:TemplateColumn HeaderText="Id profile" Visible="False">
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_idProfile" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
<%--                                                      <asp:BoundColumn DataField="data_ins" HeaderText="DATA INSERIMENTO" HeaderStyle-Width="10%" />--%>
                                                    <asp:TemplateColumn HeaderText="DATA INSERIMENTO">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="data_ins" runat="server" Text='<%# Eval("data_ins", "{0:dd/MM/yyyy}") %>'></asp:Literal>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                  
                                                    <asp:BoundColumn DataField="tipo_doc" HeaderText="TIPO" HeaderStyle-Width="5%" />
                                                    <asp:TemplateColumn HeaderText="ID/SEGNATURA DATA" HeaderStyle-Width="20%">
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data_prot_or_crea") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_data_prot_or_crea" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data_prot_or_crea") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="oggetto" HeaderText="OGGETTO" ItemStyle-HorizontalAlign="Left"
                                                        HeaderStyle-Width="20%" />
                                                    <asp:BoundColumn DataField="codice_fasc" HeaderText="CODICE FASC." HeaderStyle-Width="10%" />
                                                    <asp:BoundColumn DataField="numAllegati" HeaderText="N°. ALL" HeaderStyle-Width="5%" />
                                                    <asp:BoundColumn DataField="tipoFile" HeaderText="Tipo File" HeaderStyle-Width="5%" />
                                                    <asp:TemplateColumn HeaderText="Dim. file" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTotalSize" runat="server" Text="<%#this.GetTotalSize(Container.DataItem)%>"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Num Prot." Visible="False">
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.num_prot") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_numProt" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.num_prot") %>'
                                                                Font-Bold="true"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblInvalidFileFormat" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.invalidFileFormat") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Verifiche" HeaderStyle-Width="13%" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Image ID="imgEsitoValidazionefirma" runat="server" Visible="false" />
                                                            <asp:Image ID="imgEsitoValidazioneTipo" runat="server" Visible="false" />
                                                            <asp:Image ID="imgEsitoValidazionePolicy" runat="server" Visible="false" />
                                                            <asp:Label ID="lblEsitoValidazioneFirma" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.esitoValidazioneFirma") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="policyValida" Visible="False">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_policyValida" runat="server" Text='<%# Bind("policyValida") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderStyle-Width="10%" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Button ID="btnVerificaIntegrita" runat="server" ToolTip="Verifica integrità del documento su un supporto esterno"
                                                                Text="Verifica integrità su supporto esterno" Enabled="<%#this.IsEnabledVerificaIntegritaDocumento()%>"
                                                                CommandName="VERIFICA_INTEGRITA_DOCUMENTO"  />
                                                            <asp:Button ID="btnVerificaIntegritaStorage" runat="server" ToolTip="Verifica integrità del documento all'interno dello storage remoto"
                                                                Text="Verifica Integrità" Enabled="<%# this.isEnabledVerificaLeggibilitaDocumento()%>"
                                                                CommandName="VERIFICA_INTEGRITA_DOCUMENTO_STORAGE" />
                                                            <asp:Button ID="btnVerificaLeggibilita2" runat="server" ToolTip="Verifica Leggibilità"
                                                                Text="Verifica Leggibilità" Enabled="<%# this.isEnabledVerificaLeggibilitaDocumento()%>"
                                                                CommandName="VERIFICA_LEGGIBILITA_DOCUMENTO" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Mask Policy" Visible="False">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblMaskPolicy" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.checkMaskPolicy") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Invia Esibizione" Visible="true">
                                                        <ItemTemplate>
                                                        <asp:ImageButton ID="btnInviaAreaEsibizione" runat ="server" ToolTip="Invia in Esibizione"
                                                        ImageUrl="../Img/InvioEsib.gif" Enabled="true" CommandName="INVIA_AREA_ESIBIZIONE" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.idProfile") + "$" + DataBinder.Eval(Container, "DataItem.idProject") %>'
                                                        />
                                                            <%--<asp:Button ID="btnInviaAreaEsibizione" runat="server" ToolTip="Invia in Esibizione"
                                                                Text="Invia Esibizione" Enabled="true"
                                                                CommandName="INVIA_AREA_ESIBIZIONE" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'/>--%>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Panel ID="pnl_bottoniera" runat="server" Visible="false">
                                                <div class="box_cerca">
                                                    <div align="center">
                                                        <fieldset>
                                                            <legend>Verifiche</legend>
                                                            <asp:UpdatePanel ID="upValidate" runat="server" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <table class="tab_policy">
                                                                        <tr>
                                                                            <td class="tab_policy_sx">
                                                                                Policy
                                                                            </td>
                                                                            <td class="tab_policy_dx1">
                                                                                <asp:DropDownList ID="ddl_policy" runat="server" CssClass="testo_grigio" Width="450px"
                                                                                    AutoPostBack="true" OnSelectedIndexChanged="ChangePolicy">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                            <asp:Button ID="btn_verificaFirma" runat="server" Text="Verifica CRL firmati" ToolTip="Verifica la validità della CRL dei file firmati (.P7M)"
                                                                CssClass="cbtn" OnClick="btn_verificaFirma_Click" OnClientClick="return confirm('Si desidera effettuare la verifica di validità della CRL della firma sui documenti?');" />
                                                            <asp:Button ID="btn_verificaMarca" runat="server" Text="Verifica marcati" ToolTip="Verifica la validità dei file marcati"
                                                                CssClass="cbtn" OnClick="btn_verificaMarca_Click" OnClientClick="return confirm('Si desidera effettuare la verifica di validità della marca sui documenti?');" />
                                                            <asp:Button ID="btn_verificaFormati" runat="server" Text="Verifica formati" ToolTip="Avvia la procedura di verifica di corrispondenza tra formato file e suo contenuto"
                                                                CssClass="cbtn" OnClick="btn_verificaFormati_Click" OnClientClick="return confirm('Si desidera effettuare la verifica di validità dei formati sui documenti?');" />
                                                        </fieldset>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                            <div id="box_button">
                                                <asp:Button ID="btn_rifiuta" runat="server" Text="Rifiuta" OnClientClick="controllo();"
                                                    OnClick="BtnRifiuta_Click" ToolTip="Rifiuta l'istanza" Visible="false"/>
                                                <asp:Button ID="btn_lavorazione" runat="server" Text="Passa in Lavorazione"
                                                    OnClick="btn_lavorazione_OnClick" ToolTip="Metti l'istanza in lavorazione" Visible="false"/>
                                                <asp:Button ID="btn_firma" runat="server" Text="Firma" OnClick="btn_firma_Click"
                                                    OnClientClick="showModalDialogFirma();" ToolTip="Firma l'istanza" Visible="false"/>
                                                 <asp:Button ID="btn_verificaLeggibilita" runat="server" Text="Verifica Leggibilità"
                                                    ToolTip="Verifica la leggibilità dei documenti in conservazione" 
                                                    onclick="btn_verificaLeggibilita_Click" OnClientClick="showTestLeggibilita2()" Visible="false" />
                                                <asp:Button ID="btn_standBy" runat="server" Text="Rigenera marca"
                                                    OnClick="btn_standBy_Click" ToolTip="Rigenera la marca dell'istanza" Visible="false"/>
                                            </div>
                                            <asp:Panel ID="pnlStatoInLavorazione" runat="server" Style="text-align: center" Visible="false">
                                                <asp:Label ID="lblStatoInLavorazione" runat="server" Text="Istanza in fase di preparazione."></asp:Label>
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
