<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaReport.aspx.cs"
    Inherits="ConservazioneWA.RicercaReport" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Menu.ascx" TagName="Menu" TagPrefix="uc6" %>
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<%@ Register Assembly="MessageBox" Namespace="Utilities" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ricerca Report</title>
    <link href="CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <script src="Script/jquery-1.10.2.min.js"
        type="text/javascript"></script>

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
            border: 1px; /* solid #202020*/
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
            font-size: x-small;
            margin: 0px;
            top: 100%;
            position: absolute;
        }
        
        
        
        /*Sub sub level menu list items */
        .horizontalcssmenu ul li ul li li
        {
            font-size: x-small;
            position: relative;
            display: list-item;
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
        * html .horizontalcssmenu ul li
        {
            float: left;
            height: 1%;
        }
        * html .horizontalcssmenu ul li a
        {
            height: 1%;
        }
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


        var cssmenuids = ["cssmenu6"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
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
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <div id="container">
        <div id="header">
            <img src="Img/logo_conservazione.jpg" alt="Pitre - Gestione Conservazione" />
            <h2>
                <asp:Label ID="lbl_amm" runat="server"></asp:Label></h2>
        </div>
        <uc6:menu id="menuTop" runat="server" paginachiamante="REGISTRO" />
        <div id="title">
            <h1>
                Ricerca Report</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upFiltriRicerca" runat="server" UpdateMode="always">
                <ContentTemplate>
                    <div class="box_cerca">
                        <div align="center">
                            <fieldset>
                                <legend>Tipologie di report</legend>
                                <div align="left">
                                    <asp:RadioButtonList ID="RadioButtonReport" RepeatDirection="Vertical" CellPadding="3"
                                        CellSpacing="2" runat="server" CssClass="testo_grigio_no_spazio" OnSelectedIndexChanged="RadioButtonReport_SelectedIndexChanged"
                                        AutoPostBack="true">
                                        <asp:ListItem Selected="true">Report sulle istanze di conservazione</asp:ListItem>
                                        <asp:ListItem>Report sui documenti</asp:ListItem>
                                        <asp:ListItem>Report sulle verifiche</asp:ListItem>
                                        <asp:ListItem>Report sulla storia di conservazione della istanza</asp:ListItem>
                                        <asp:ListItem>Report sulla storia di conservazione dei documenti</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div>
                <asp:UpdatePanel ID="UpdateReport1" runat="server" UpdateMode="always">
                    <ContentTemplate>
                        <div class="box_cerca">
                            <div align="center">
                                <fieldset id="Fieldset1" runat="server" visible="true">
                                    <legend>Opzioni di ricerca</legend>
                                    <asp:Panel ID="pnl_prova" Visible="true" runat="server">
                                        <table id="tabFiltriNotificheInConservazione" runat="server" class="tabDate2">
                                            <tr>
                                                <td class="tabDateSx">
                                                    Data di invio in conservazione:
                                                </td>
                                            </tr>
                                        </table>
                                        <table class="tabDate2">
                                            <tr>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddl_invio1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_invio1_SelectedIndexChanged"
                                                        CssClass="testo_grigio" Width="130px">
                                                        <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                        <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                        <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                        <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                        <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:Label ID="lblDaNotifiche" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                    <uc1:calendario id="lbl_dCreazioneDa" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                        visible="true" />
                                                    <asp:Label ID="lblANotifiche" runat="server" Width="18px" CssClass="testo_grigio">A</asp:Label>
                                                    <uc1:calendario id="lbl_dCreazioneA" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                        visible="false" />
                                                </td>
                                            </tr>
                                        </table>
                                        <table id="Table1" runat="server" class="tabDate2">
                                            <tr>
                                                <td class="tabDateSx">
                                                    Data di chiusura:
                                                </td>
                                            </tr>
                                        </table>
                                        <table class="tabDate2">
                                            <tr>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddl_chiusura1" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_chiusura1_SelectedIndexChanged"
                                                        Width="130px">
                                                        <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                        <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                        <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                        <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                        <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:Label ID="Label6" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                    <uc1:calendario id="lbl_dCreazioneDa1" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                        visible="true" />
                                                    <asp:Label ID="Label7" runat="server" Width="18px" CssClass="testo_grigio">A</asp:Label>
                                                    <uc1:calendario id="lbl_dCreazioneA1" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                        visible="false" />
                                                </td>
                                            </tr>
                                        </table>
                                        <table id="Table2" runat="server" class="tabDate2">
                                            <tr>
                                                <td class="tabDateSx">
                                                    Data di rifiuto:
                                                </td>
                                            </tr>
                                        </table>
                                        <table class="tabDate2">
                                            <tr>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddl_rifiuto1" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_rifiuto1_SelectedIndexChanged"
                                                        Width="130px">
                                                        <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                        <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                        <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                        <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                        <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:Label ID="Label1" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                    <uc1:calendario id="lbl_dCreazioneDa2" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                        visible="true" />
                                                    <asp:Label ID="Label8" runat="server" Width="18px" CssClass="testo_grigio">A</asp:Label>
                                                    <uc1:calendario id="lbl_dCreazioneA2" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                        visible="false" />
                                                </td>
                                            </tr>
                                        </table>
                                        
                                    </asp:Panel>
                                </fieldset>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <asp:UpdatePanel ID="UpdateReport2" runat="server" UpdateMode="always">
                <ContentTemplate>
                    <div class="box_cerca">
                        <div align="center">
                            <fieldset id="Fieldset2" runat="server" visible="false">
                                <legend>Opzioni di ricerca</legend>
                                <asp:Panel ID="pnl_prova2" Visible="false" runat="server">
                                    <table id="Table3" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                ID istanza:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_idIstanza" runat="server" CssClass="testo_grigio" Width="130px" OnSelectedIndexChanged="ddl_idIstanza_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="lblDAIdIst" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:Label>
                                                <asp:TextBox ID="txt_initIdIst" runat="server" CssClass="calendarBox"></asp:TextBox>
                                                <asp:Label ID="lblAIdIst" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:Label>
                                                <asp:TextBox ID="txt_fineIdIst" runat="server" CssClass="calendarBox" Visible="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="Table4" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                Data di invio in conservazione:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_invioc" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_invioc_SelectedIndexChanged"
                                                    Width="130px">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label10" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneDa4" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="true" />
                                                <asp:Label ID="Label11" runat="server" Width="18px" CssClass="testo_grigio" Visible="false">A</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneA4" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="Table5" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                Data di chiusura:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_chiusura2" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_chiusura2_SelectedIndexChanged"
                                                    Width="130px">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label12" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneDa5" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="true" />
                                                <asp:Label ID="Label13" runat="server" Width="18px" CssClass="testo_grigio" Visible="false">A</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneA5" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="Table6" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                Data di rifiuto:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_rifiuto2" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_rifiuto2_SelectedIndexChanged"
                                                    Width="130px">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label14" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneDa6" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="true" />
                                                <asp:Label ID="Label15" runat="server" Width="18px" CssClass="testo_grigio" Visible="false" >A</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneA6" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    
                                </asp:Panel>
                            </fieldset>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdateReport3" runat="server" UpdateMode="always">
                <ContentTemplate>
                    <div>
                        <div align="left">
                            <fieldset id="Fieldset3" runat="server" visible="false">
                                <asp:Panel ID="pnl_prova3" Visible="false" runat="server">
                                    <table id="contenitore">
                                        <tr>
                                            <td>
                                            <div  class="div_mio">
                                                <table id="Table7" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            ID istanza:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_idIstanza2" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_idIstanza2_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label2" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:Label>
                                                            <asp:TextBox ID="TextBox1" runat="server" CssClass="calendarBox"></asp:TextBox>
                                                            <asp:Label ID="Label3" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:Label>
                                                            <asp:TextBox ID="TextBox2" runat="server" CssClass="calendarBox" Visible="False"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table8" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Data di invio in conservazione:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_invio3" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_invio3_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label9" runat="server" CssClass="testo_grigio" Width="18px">Il</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneDa7" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="true" />
                                                            <asp:Label ID="Label16" runat="server" CssClass="testo_grigio" Width="18px" Visible ="false">A</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneA7" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table9" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Data di chiusura istanza:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_chiusura3" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_chiusura3_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label17" runat="server" CssClass="testo_grigio" Width="18px">Il</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneDa8" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="true" />
                                                            <asp:Label ID="Label18" runat="server" CssClass="testo_grigio" Width="18px" Visible="false">A</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneA8" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Tipo di verifica:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="cboTipiConservazione" runat="server" DataValueField="Codice"
                                                                DataTextField="Descrizione" Width="200">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem Value ="L" >Leggibilità </asp:ListItem>
                                                                <asp:ListItem Value ="I">Integrità </asp:ListItem>
                                                                <asp:ListItem Value ="U" >Unificata </asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                                </div>
                                            </td>
                                            <td>
                                            <div class="div_mio">
                                                <table id="Table11" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            ID supporto:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_idSupporto" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_idSupporto_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label21" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:Label>
                                                            <asp:TextBox ID="TextBox3" runat="server" CssClass="calendarBox"></asp:TextBox>
                                                            <asp:Label ID="Label22" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:Label>
                                                            <asp:TextBox ID="TextBox4" runat="server" CssClass="calendarBox" Visible="False"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table12" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Data generazione supporto:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_generazioneSupporto" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_generazioneSupporto_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label23" runat="server" CssClass="testo_grigio" Width="18px">Il</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneDa10" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="true" />
                                                            <asp:Label ID="Label24" runat="server" CssClass="testo_grigio" Width="18px" Visible="false">A</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneA10" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table13" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Data esecuzione verifica:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_esecuzioneVerifica" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_esecuzioneVerifica_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label25" runat="server" CssClass="testo_grigio" Width="18px">Il</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneDa11" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="true" />
                                                            <asp:Label ID="Label26" runat="server" CssClass="testo_grigio" Width="18px" Visible="false">A</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneA11" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table15" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Esito della verifica:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="DropDownList14" runat="server" DataValueField="Codice" DataTextField="Descrizione"
                                                                Width="200">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem Value ="1" >Positiva </asp:ListItem>
                                                                <asp:ListItem Value ="0">Negativa </asp:ListItem>
                                                                
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                
                            </div>
                            </fieldset>
                            
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdateReport4" runat="server" UpdateMode="always">
                <ContentTemplate>
                    <div class="box_cerca">
                        <div align="center">
                            <fieldset id="Fieldset4" runat="server" visible="false">
                                <legend>Opzioni di ricerca</legend>
                                <asp:Panel ID="pnl_prova4" Visible="false" runat="server">
                                    <table id="Table10" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                ID istanza:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_idIstanza3" runat="server" CssClass="testo_grigio" Width="130px" OnSelectedIndexChanged="ddl_idIstanza3_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label4" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:Label>
                                                <asp:TextBox ID="TextBox5" runat="server" CssClass="calendarBox"></asp:TextBox>
                                                <asp:Label ID="Label19" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:Label>
                                                <asp:TextBox ID="TextBox6" runat="server" CssClass="calendarBox" Visible="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="Table14" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                Data di invio in conservazione:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_invioc3" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_invioc3_SelectedIndexChanged"
                                                    Width="130px">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label20" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneDa12" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="true" />
                                                <asp:Label ID="Label27" runat="server" Width="18px" CssClass="testo_grigio" Visible="false">A</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneA12" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="Table16" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                Data di chiusura:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_chiusura4" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_chiusura4_SelectedIndexChanged"
                                                    Width="130px">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label28" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneDa13" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="true" />
                                                <asp:Label ID="Label29" runat="server" Width="18px" CssClass="testo_grigio" Visible="false">A</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneA13" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="Table17" runat="server" class="tabDate2">
                                        <tr>
                                            <td class="tabDateSx">
                                                Data di rifiuto:
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="tabDate2">
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_rifiuto3" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_rifiuto3_SelectedIndexChanged"
                                                    Width="130px">
                                                    <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                    <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                    <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                    <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label30" runat="server" Width="18px" CssClass="testo_grigio">Il</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneDa14" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="true" />
                                                <asp:Label ID="Label31" runat="server" Width="18px" CssClass="testo_grigio" Visible="false">A</asp:Label>
                                                <uc1:calendario id="lbl_dCreazioneA14" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                    visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    
                                </asp:Panel>
                            </fieldset>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdateReport5" runat="server" UpdateMode="always">
                <ContentTemplate>
                    <div >
                        <div align="left">
                            <fieldset id="Fieldset5" runat="server" visible="false">
                                <legend>Opzioni di ricerca</legend>
                                <asp:Panel ID="pnl_prova5" Visible="false" runat="server">
                                    <table id="Table18" >
                                        <tr>
                                            <td  >
                                            <div  class="div_mio">
                                                <table id="Table19" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            ID istanza:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_idIstanza4" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_idIstanza4_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label5" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:Label>
                                                            <asp:TextBox ID="TextBox7" runat="server" CssClass="calendarBox"></asp:TextBox>
                                                            <asp:Label ID="Label32" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:Label>
                                                            <asp:TextBox ID="TextBox8" runat="server" CssClass="calendarBox" Visible="False"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table20" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Data di invio in conservazione:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_invioc5" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_invioc5_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label33" runat="server" CssClass="testo_grigio" Width="18px">Il</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneDa16" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="true" />
                                                            <asp:Label ID="Label34" runat="server" CssClass="testo_grigio" Width="18px" Visible="false">A</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneA16" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table21" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Data di chiusura istanza:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_chiusura5" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_chiusura5_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
                                                                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label35" runat="server" CssClass="testo_grigio" Width="18px">Il</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneDa15" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="true" />
                                                            <asp:Label ID="Label36" runat="server" CssClass="testo_grigio" Width="18px" Visible="false">A</asp:Label>
                                                            <uc1:calendario id="lbl_dCreazioneA15" runat="server" pagina_chiamante="RICERCAISTANZE"
                                                                visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                               
                                                </div>
                                            </td>
                                            <td >
                                            <div class="div_mio" >
                                                <table id="Table23" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            ID documento:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_idDocumento" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_idDocumento_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label37" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:Label>
                                                            <asp:TextBox ID="TextBox9" runat="server" CssClass="calendarBox"></asp:TextBox>
                                                            <asp:Label ID="Label38" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:Label>
                                                            <asp:TextBox ID="TextBox10" runat="server" CssClass="calendarBox" Visible="False"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="Table22" runat="server" class="tabDate2">
                                                    <tr>
                                                        <td class="tabDateSx">
                                                            Protocollo:
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="tabDate2">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddl_protocollo" runat="server" AutoPostBack="true" CssClass="testo_grigio" OnSelectedIndexChanged="ddl_protocollo_SelectedIndexChanged"
                                                                Width="130px">
                                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="Label39" runat="server" CssClass="testo_grigio_no_spazio">Il</asp:Label>
                                                            <asp:TextBox ID="TextBox11" runat="server" CssClass="calendarBox"></asp:TextBox>
                                                            <asp:Label ID="Label40" runat="server" CssClass="testo_grigio_no_spazio" Visible="False">A</asp:Label>
                                                            <asp:TextBox ID="TextBox12" runat="server" CssClass="calendarBox" Visible="False"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>                                              
                                               
                                               
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                </fieldset>
                            </div>
                           
                        </div>
                </ContentTemplate>
            </asp:UpdatePanel>

     <div align="center" style="margin-top: 5px; padding-top: 5px;">
                  
     <asp:Button ID="btnStampa" runat="server" Text="Stampa" CssClass="cbtn" OnClick="btnStampa_Click" />
     <asp:Button ID="btnReset" runat="server" Text="Reset filtri" CssClass="cbtn" OnClick="btnReset_Click" />
     
         <asp:RadioButtonList ID="RadioButtonStampaReport" runat="server" class="testo_grigio_scuro" RepeatDirection="Horizontal"
             AutoPostBack="false" 
             OnSelectedIndexChanged="RadioButtonStampaReport_SelectedIndexChanged">
             <asp:ListItem Value="XLS" Selected="True" >Formato Excel</asp:ListItem>
                <asp:ListItem Value="PDF">Formato PDF</asp:ListItem>
         </asp:RadioButtonList>
     </div>
    <!-- PopUp Wait-->
   <cc2:modalpopupextender id="mdlPopupWait" runat="server" targetcontrolid="Wait" popupcontrolid="Wait"
        backgroundcssclass="modalBackground" behaviorid="mdlWait" />
    <div id="Wait" runat="server" style="font-weight: normal; font-size: 13px; font-family: Arial;
        text-align: center; display: none;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <ContentTemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="Img/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
