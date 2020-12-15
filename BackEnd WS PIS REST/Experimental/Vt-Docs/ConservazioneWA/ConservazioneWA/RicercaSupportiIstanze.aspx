<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaSupportiIstanze.aspx.cs"
    Inherits="ConservazioneWA.RicercaSupportiIstanze" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Menu.ascx" TagName="Menu" TagPrefix="uc6" %>
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<%@ Register Assembly="MessageBox" Namespace="Utilities" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Conservazione</title>
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
        
        .link_download a:link
        {
            background-image: url('Img/bg_button.jpg');
        }
                .link_download a:visited
        {
            background-image: url('Img/bg_button.jpg');
        }
                .link_download a:hover
        {
            background-image: url('Img/bg_button_hover.jpg');
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
    <script type="text/javascript">
        /*
        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }
        */

        function showModalDialogRegistraSupportoRimovibile(idIstanza, idSupporto) {
            var returnValue = window.showModalDialog("PopUp/RegistraSupporto.aspx?idIstanza=" + idIstanza + "&idSupporto=" + idSupporto,
                            "",
                            "dialogWidth:600px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no");


            return returnValue;
        }

        function showModalDialogVerificaSupportoRegistrato(idIstanza, idSupporto) {
            var returnValue = window.showModalDialog("PopUp/VerificaSupportoRegistrato.aspx?idIstanza=" + idIstanza + "&idSupporto=" + idSupporto,
                            "",
                            "dialogWidth:700px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no");


            return returnValue;
        }

        function showTestLeggibilita(idConservazione) {
            var returnvalue = window.showModalDialog("PopUp/TestLeggibilita.aspx?idConservazione=" + idConservazione, "", "dialogWidth:800px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        function showVerificaIntegrita(idConservazione, idSupporto) {
            var returnvalue = window.showModalDialog("PopUp/VerificaSupporto.aspx?idIstanza=" + idConservazione + "&idSupporto=" + idSupporto, "", "dialogWidth:600px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        function showVerificheIL(idConservazione, idSupporto) {
            var returnvalue = window.showModalDialog("PopUp/VerificheIL.aspx?idConservazione=" + idConservazione + "&idSupporto=" + idSupporto, "", "dialogWidth:800px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }

        var cssmenuids = ["cssmenu3"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
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

        function notifyRigeneraIstanza(idConservazione, idSupporto) {
            var message = "Attenzione!\nper il supporto è stata già rigenerata almeno un'istanza.\nSi è veramente sicuri di volerne creare un'altra?";

            if (confirm(message)) {
                // Istanza creata in deroga alle policy di conservazione
                form1.hd_istanza_da_rigenerare.value = "true";
                form1.hd_ID_istanza_da_rigenerare.value = idConservazione;
                form1.hd_supporto_da_rigenerare.value = idSupporto;
                form1.submit();

            }
            else {
                form1.hd_istanza_da_rigenerare.value = "false";
                form1.hd_supporto_da_rigenerare.value = "";
                form1.hd_ID_istanza_da_rigenerare = "";
            }
        }
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
            <h2>
                <asp:Label ID="lbl_amm" runat="server"></asp:Label></h2>
        </div>
        <uc6:Menu ID="menuTop" runat="server" PaginaChiamante="SUPPORTI" />
        <div id="title">
            <h1>
                Ricerca Supporti</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upContent" runat="server">
                <contenttemplate>             
                    <div class="box_cerca">
                    <div align="center">
                        <fieldset>
                            <legend>Opzioni di ricerca</legend>
                            <table class="tabDate">
                                <tr>
                                    <td colspan="2" style="text-align:left;">
                                        <asp:CheckBoxList ID="cblFilterTipiSupporto" runat="server" DataValueField="SystemId" DataTextField="Descrizione" RepeatLayout="Flow" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align:left;">
                                        <asp:CheckBoxList ID="cblFilterStatiSupporto" runat="server" DataValueField="Codice" DataTextField="Descrizione" RepeatLayout="Flow" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Id Istanza:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:TextBox ID="txtFilterIdIstanza" runat="server" CssClass="testo_grigio_no_spazio"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Id Supporto:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:TextBox ID="txtFilterIdSupporto" runat="server" CssClass="testo_grigio_no_spazio"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Collocazione fisica:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:TextBox ID="txtFilterCollocazioneFisica" runat="server" CssClass="testoTxtLungo"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Note:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:TextBox ID="txtFilterNote" runat="server" CssClass="testoTxtLungo"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Data produzione:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:Label ID="lbl" runat="server" Text="Da"></asp:Label> 
                                        <uc1:Calendario ID="txtFilterDataProduzioneFrom" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                        <asp:Label ID="Label3" runat="server" Text="a"></asp:Label> 
                                       <uc1:Calendario ID="txtFilterDataProduzioneTo" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Data ultima verifica integrità:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:Label ID="Label4" runat="server" Text="Da"></asp:Label> 
                                        <uc1:Calendario ID="txtFilterDataUltimaVerificaFrom" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                        <asp:Label ID="Label5" runat="server" Text="a"></asp:Label> 
                                       <uc1:Calendario ID="txtFilterDataUltimaVerificaTo" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Data prossima verifica integrità:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:Label ID="Label6" runat="server" Text="Da"></asp:Label> 
                                        <uc1:Calendario ID="txtFilterDataProssimaVerificaFrom" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                        <asp:Label ID="Label7" runat="server" Text="a"></asp:Label> 
                                       <uc1:Calendario ID="txtFilterDataProssimaVerificaTo" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                    </td>
                                </tr>
                                <% //MEV CS 1.5 %>
                                <tr>
                                    <td class="tabDateSx">
                                        Data ultima verifica leggibilità:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:Label runat="server" ID="lblUltimaVerLegDataDa" Text="Da"></asp:Label>
                                        <uc1:Calendario runat="server" ID="txtFilterDataUltimaVerLegFrom" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                        <asp:Label runat="server" ID="lblUltimaVerLegDataA" Text="a"></asp:Label>
                                        <uc1:Calendario runat="server" ID="txtFilterDataUltimaVerLegTo" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tabDateSx">
                                        Data prossima verifica leggibilità:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:Label runat="server" ID="lblProxVerLegDataDa" Text="Da"></asp:Label>
                                        <uc1:Calendario runat="server" ID="txtFilterDataProxVerLegFrom" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                        <asp:Label runat="server" ID="lblProxVerLegDataA" Text="a"></asp:Label>
                                        <uc1:Calendario runat="server" ID="txtFilterDataProxVerLegTo" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                    </td>
                                </tr>
                                <% //fine MEV CS 1.5 %>
                                <tr>
                                    <td class="tabDateSx">
                                        Data scadenza marca:
                                    </td>
                                    <td class="tabDateDx">
                                        <asp:Label ID="Label1" runat="server" Text="Da"></asp:Label> 
                                        <uc1:Calendario ID="txtFilterDataScadenzaMarcaFrom" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                        <asp:Label ID="Label2" runat="server" Text="a"></asp:Label> 
                                       <uc1:Calendario ID="txtFilterDataScadenzaMarcaTo" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                    </td>
                                </tr>
                            </table>
                            <div align="center" style="margin-top:5px;padding-top:5px;border-top:1px dashed #eaeaea;">
                                <asp:Button ID="btnFind" runat="server" OnClick="btnFind_Click" Text="Cerca" CssClass="cbtn" />
                            </div>
                        </fieldset>
                        </div>
                    </div>
                

            <asp:UpdatePanel ID="upDettaglio" runat="server" UpdateMode="Conditional">
                <contenttemplate>
                        <asp:DataGrid ID="grdSupporti" runat="server" Width="100%" 
                            AutoGenerateColumns="false" 
                            cssClass="tab_istanze"
                            OnPageIndexChanged="grdSupporti_PageIndexChanged"
                            OnItemCommand="grdSupporti_ItemCommand" 
                            OnPreRender="grdSupporti_PreRender"
                            OnItemCreated="OnDataGridItemCreated"
                            AllowPaging="True" 
                            AllowCustomPaging="false" PageSize="5" >
                            
                            <HeaderStyle cssClass="tab_istanze_header"  />
                            <AlternatingItemStyle CssClass="tab_istanze_a"  />
                            <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                            <PagerStyle CssClass="menu_pager_grigio"  Mode="NumericPages" Position="TopAndBottom"/>

                            <Columns>
                                <asp:TemplateColumn Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdIstanza" runat="server"  Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).idConservazione%>" />
                                        <asp:Label ID="lblIdSupporto" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).SystemID%>"  />
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Istanza" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnIstanza" runat="server" CssClass="link_istanze"  Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).idConservazione%>" CommandName="GO_TO_ISTANZA"></asp:LinkButton>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Supporto" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSupporto" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).SystemID%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Tipo" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="5%">
                                  <ItemTemplate>
                                        <asp:Label ID="lblCodiceTipoSupporto" runat="server" Visible="false" Text="<%#this.GetCodiceTipoSupporto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"></asp:Label>

                                        <asp:Label ID="lblTipo" runat="server" 
                                            Text="<%#this.GetTipoSupporto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                        </asp:Label>
                                    </ItemTemplate>        
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Stato" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStato" runat="server" 
                                            Text="<%#this.GetStatoSupporto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Prodotto il" HeaderStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDataProduzione" runat="server"
                                            Text="<%#this.ToShortDateString(((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).dataProduzione)%>">
                                        </asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Collocazione fisica" HeaderStyle-Width="15%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCollocazioneFisica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).collocazioneFisica%>"></asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Note" HeaderStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNote" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).Note%>"></asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText="Scadenza marca" HeaderStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblScadenzaMarca" runat="server"
                                        Text="<%#this.ToShortDateString(((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).dataScadenzaMarca)%>">
                                        </asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Stato ver.int." HeaderStyle-Width="20%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDatiVerifica" runat="server" Text="<%#this.GetDatiVerifica((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText="Stato ver.legg." HeaderStyle-Width="20%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDatiVerificaLegg" runat="server" Text="<%#this.GetDatiVerificaLeggibilita((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Azioni" ItemStyle-HorizontalAlign="center" HeaderStyle-Width="10%">
                                    <ItemTemplate>
                                   
                                        <asp:Panel ID="pnlAzioniSupportoRemoto" runat="server" Visible="<%#this.IsSupportoRemoto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                    
                                            <div class="link_download">
                                            <asp:Button ID="btnDonwnload" runat="server" Text="Download" CssClass="cbtn" CommandName="DOWNLOAD"
                                            CommandArgument="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).istanzaDownloadUrl%>" 
                                            ToolTip="Download" Visible="<%#this.IsDownlodable((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"/>                                                                                                                                    
                                            </div>
                                            <asp:Button ID="btnBrowse" runat="server" Text="Sfoglia" CssClass="cbtn" CommandName="BROWSE"
                                            CommandArgument="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).istanzaBrowseUrl%>"
                                            Visible="<%#this.IsBrowsable((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"
                                            ToolTip="Sfoglia il contenuto dello Storage" />
                                            <asp:Button ID="btn_verificheIL" runat="server" Text="Verifica" CssClass="cbtn" CommandName="VERIFICHE_IL" ToolTip="Verifica integrità e leggibilità dei file all'interno dello storage" Visible="<%#this.IsDownlodable((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>" />
                                            
                                            <asp:Button ID="btn_verifica_integrita_storage" runat="server" Text="Verifica Integrità" CssClass="cbtn" CommandName="VERIFICA_INTEGRITA_STORAGE" ToolTip="Verifica l'integrità dei file all'interno dello storage" Visible="false" />
                                            <asp:Button ID="btn_verifica_leggibilita" runat="server" Text="Verifica Leggibilità" CssClass="cbtn" CommandName="VERIFICA_LEGGIBILITA" ToolTip="Verifica se i file dell'istanza sono leggibili" Visible="false" />
                                            <asp:Button ID="btn_storia_verifiche_remoto" runat="server" Text="Storia" CssClass="cbtn" CommandName="STORIA_VERIFICHE_SUPPORTO" Visible="<%#this.IsDownlodable((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>" ToolTip="Visualizza la storia delle verifiche di integrità effettuate per il supporto remoto." />
                                            <asp:Button ID="btn_rigenerazione_istanza" runat="server" Text="Rig.Istanza" CssClass="cbtn" CommandName="RIGENERAZIONE_ISTANZA" Visible="<%#this.IsDamaged((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>" ToolTip="Rigenera l'istanza corrotta." />  
                                        </asp:Panel>
                                       
                                        <asp:Panel ID="Panel1" runat="server" Visible="<%#!this.IsSupportoRemoto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                            <asp:Button ID="btnRegistraSupporto" runat="server" Text="Registra" CssClass="cbtn" CommandName="REGISTRA_SUPPORTO" ToolTip="Immetti i dati di registrazione del supporto esterno."  />
                                            <asp:Button ID="btnVerificaSupporto" runat="server" Text="Verifica" CssClass="cbtn" CommandName="VERIFICA_SUPPORTO" Visible="<%#this.AreSupportiRimovibiliVerificabili((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>" ToolTip="Verifica l'integrità del documento su un supporto esterno." />
                                            <asp:Button ID="btnStoriaVerifiche" runat="server" Text="Storia" CssClass="cbtn" CommandName="STORIA_VERIFICHE_SUPPORTO" Visible="<%#(this.AreSupportiRimovibiliVerificabili((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem) && this.IsSupportoRimovibileRegistrato((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem))%>" ToolTip="Visualizza la storia delle verifiche di integrità effettuate per il supporto esterno." />
                                                                                  
                                        </asp:Panel>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>

                        <br />
                        
                        <asp:DataGrid ID="grdStoriaVerifiche" runat="server" Width="100%" 
                            AutoGenerateColumns="false" 
                            cssClass="tab_istanze"
                            OnPageIndexChanged="grdStoriaVerifiche_PageIndexChanged"
                            OnItemCreated="OnDataGridItemCreated"
                            AllowPaging="True" 
                            AllowCustomPaging="false" PageSize="5" >
                            
                            <HeaderStyle cssClass="tab_istanze_header"  />
                            <AlternatingItemStyle CssClass="tab_istanze_a"  />
                            <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                            <PagerStyle CssClass="menu_pager_grigio"  Mode="NumericPages" Position="TopAndBottom" />

                            <Columns>
                                <asp:TemplateColumn Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblId" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Istanza">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdIstanzaVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).idConservazione%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Supporto">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdSupportoVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).SystemID%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Verificato il">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDataVerifica" runat="server" Text="<%#this.ToShortDateString(((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).dataUltimaVerifica)%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Esito">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEsitoVerifica" runat="server" Text="<%#this.GetEsitoVerifica((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPercentualeVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).percVerifica%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Note">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNoteVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).Note%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            
                        </asp:DataGrid>

         
                </contenttemplate>
            </asp:UpdatePanel>
            </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- PopUp Wait-->
    <%--<cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />--%>
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
    
    <input type="hidden" name="hd_istanza_da_rigenerare" id="hd_istanza_da_rigenerare" runat="server" />
    <input type="hidden" name="hd_ID_istanza_da_rigenerare" id="hd_ID_istanza_da_rigenerare" runat="server" />
    <input type="hidden" name="hd_supporto_da_rigenerare" id="hd_supporto_da_rigenerare" runat="server" />
    </form>
</body>
</html>
