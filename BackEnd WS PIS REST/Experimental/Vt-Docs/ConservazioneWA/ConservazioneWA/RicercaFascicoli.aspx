<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaFascicoli.aspx.cs"
    Inherits="ConservazioneWA.RicercaFascicoli" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Menu.ascx" TagName="Menu" TagPrefix="uc6" %>
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<%@ Register Assembly="MessageBox" Namespace="Utilities" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ricerca Fascicoli</title>
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
            /*padding: 2px 8px; */
            border: 1px ;/* solid #202020*/
            border-left-width: 0;
            text-decoration: none;
            /*background: url(menubg.gif) center center repeat-x;*/
            /*color: black;*/
            /*font: bold 13px Tahoma;*/
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
        var _urlCampiProfilati = '<%=UrlCampiProfilati %>';

        var _urlChooseProject = '<%=UrlChooseProject %>';

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
        var cssmenuids = ["cssmenu5"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
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

    
        /* SCRIPT HORIZONTAL MENU  aggiunto A. Sigalot e C.Fuccia*/
        

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
        <uc6:Menu ID="menuTop" runat="server" PaginaChiamante="FASCICOLI" />
        <div id="title">
            <h1>
                Ricerca Fascicoli</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upFiltriRicerca" runat="server" UpdateMode="Conditional">
                <contenttemplate>             
                    <div class="box_cerca">
                    <div align="center">
                        <fieldset>
                            <legend>Opzioni di ricerca</legend>
                            <table class="tabDate2"> 
                            <tr>
                                <td class="tabDateSx">Titolario:</td>
                                <td class="tabDateDx">
                                     <asp:dropdownlist id="ddl_titolari" runat="server" CssClass="ddl_list_no_space" Width="100%">		
								    </asp:dropdownlist>
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
                                    <asp:label id="lblDa" runat="server"  Width="18px">Il</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneDa" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                    <asp:label id="lblA" runat="server" Width="18px">A</asp:label>
                                    <uc1:Calendario id="lbl_dataCreazioneA" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                                </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Data di chiusura:</td>
                            <td class="tabDateDx">
                                    <asp:dropdownlist id="ddl_dataC" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="130px" OnSelectedIndexChanged="ddl_dataC_SelectedIndexChanged">
										<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
									    <asp:ListItem Value="1">Intervallo</asp:ListItem>
									    <asp:ListItem Value="2">Oggi</asp:ListItem>
								        <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								        <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
									</asp:dropdownlist>
                                    <asp:label id="lbl_initdataC" runat="server"  Width="18px">Il</asp:label>
                                    <uc1:Calendario id="txt_initDataC" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                    <asp:label id="lbl_finedataC" runat="server" Width="18px">A</asp:label>
                                    <uc1:Calendario id="txt_fineDataC" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                            </td>
                            </tr>
                             <tr>
                            <td class="tabDateSx">Data di apertura:</td>
                            <td class="tabDateDx">
                                    <asp:dropdownlist id="ddl_dataA" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="130px" OnSelectedIndexChanged="ddl_dataA_SelectedIndexChanged">
										<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
									    <asp:ListItem Value="1">Intervallo</asp:ListItem>
									    <asp:ListItem Value="2">Oggi</asp:ListItem>
								        <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								        <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
									</asp:dropdownlist>
                                    <asp:label id="lbl_initdataA" runat="server"  Width="18px">Il</asp:label>
                                    <uc1:Calendario id="txt_initDataA" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                    <asp:label id="lbl_finedataA" runat="server" Width="18px">A</asp:label>
                                    <uc1:Calendario id="txt_fineDataA" runat="server" Visible="true" PAGINA_CHIAMANTE="RICERCAISTANZE"/>
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Numero:</td>
                            <td class="tabDateDx">  
                               <asp:TextBox ID="txtNumFasc" runat="server" CssClass="testo_grigio_no_spazio"></asp:TextBox>
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
                                <asp:TextBox ID="txtCodRuolo" runat="server" CssClass="testo_grigio" Width="90" MaxLength="30" OnTextChanged="txtCodRuolo_TextChanged" AutoPostBack="true" Height="16"></asp:TextBox>
                                <asp:TextBox ID="txtDescRuolo" runat="server" CssClass="testo_grigio" Width="370" MaxLength="30" Enabled="false" Height="16"></asp:TextBox>
                               <%-- <asp:ImageButton ID="btnApriRubrica" Width="30px" AlternateText="Seleziona da Rubrica" ToolTip="Seleziona da Rubrica" ImageUrl="Img/rubrica.gif" runat="server" Height="20px"></asp:ImageButton>                                       --%>
                                <asp:HiddenField ID="id_corr" runat="server" />
                                <asp:HiddenField ID="tipo_corr" runat="server" />
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Codice Classifica:</td>
                            <td class="tabDateDx">
                                <asp:TextBox ID="txtCodFascicolo" runat="server" CssClass="testo_grigio" Width="90" Height="16" MaxLength="30" OnTextChanged="txtCodFascicolo_TextChanged" AutoPostBack="true"></asp:TextBox>
                                <asp:TextBox ID="txtDescFascicolo" runat="server" CssClass="testo_grigio" Width="370" Height="16" MaxLength="30" Enabled="false"></asp:TextBox>
                                <asp:HiddenField ID="is_fasc" runat="server" />
                                <asp:HiddenField ID="id_Fasc" runat="server" />
                            </td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Descrizione:</td>
                            <td class="tabDateDx">
                             <asp:TextBox ID="txtDescr" runat="server" CssClass="testo_grigio" Width="470" Height="16"></asp:TextBox></td>
                            </tr>
                            <tr>
                            <td class="tabDateSx">Sottofascicolo:</td>
                            <td class="tabDateDx">
                             <asp:TextBox ID="txt_sottofascicolo" runat="server" CssClass="testo_grigio" Width="470" Height="16"></asp:TextBox>
                            </td>
                            </tr>                          
                            <tr>
                            <td class="tabDateSx">Tipologia del fascicolo</td>
                            <td class="tabDateDx">  
                            <asp:DropDownList ID="ddl_type_documents" runat="server" CssClass="ddl_list_no_space" Width="440" OnSelectedIndexChanged="ChangeTypeDocument" AutoPostBack="true">
                                    </asp:DropDownList>            
                                     <asp:ImageButton ID="btnCampiProfilati" runat="server" OnClick="ViewCampiProlilati" ImageUrl="~/Img/ico_oggettario.gif" Enabled="false" ToolTip="Cerca per campi profilati" AlternateText="Cerca per campi profilati"/>                   
                            </td>
                            </tr>
                            </table>
                            <div align="center" style="margin-top:5px;padding-top:5px;">
                                <asp:Button ID="btnFind" runat="server" OnClick="BtnSearch_Click" Text="Cerca" CssClass="cbtn" />
                            </div>
                        </fieldset>
                        </div>
                    </div>
                </contenttemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="upRisultati" runat="server" UpdateMode="Conditional">
                <contenttemplate>   
                <asp:Panel id="pnl_result" visible="false" runat="server">
                   <asp:DataGrid ID="dgResult" runat="server" Width="100%" AllowSorting="false" AutoGenerateColumns="false"
                    HorizontalAlign="Center" AllowPaging="true" PageSize="10" AllowCustomPaging="true"
                    ShowHeader="true" OnPageIndexChanged="dgResult_SelectedPageIndexChanged" OnItemCreated="dgResult_ItemCreated" CssClass="tab_istanze">
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
                         <asp:TemplateColumn ItemStyle-Width="5%" HeaderText="TIPO">
                            <ItemTemplate>
                                <asp:Label ID="tipo" runat="server" Text='<%# this.GetTipo((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="10%" HeaderText="COD CLASS">
                            <ItemTemplate>
                                <asp:Label ID="codiceClass" runat="server" Text='<%# this.GetCodClass((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="10%" HeaderText="CODICE">
                            <ItemTemplate>
                                <asp:Label ID="codice" runat="server" Text='<%# this.GetCodice((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="15%" HeaderText="DESCRIZIONE">
                            <ItemTemplate>
                                <asp:Label ID="descrizione" runat="server" Text='<%# this.GetDescrizione((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="10%" HeaderText="Apertura">
                            <ItemTemplate>
                                <asp:Label ID="apertura" runat="server" Text='<%# this.GetDataApertura((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>                                                                 
                         <asp:TemplateColumn ItemStyle-Width="25%" HeaderText="TIPOLOGIA">
                            <ItemTemplate>
                                <asp:Label ID="tipologia" runat="server" Text='<%# this.GetTipologia((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                         <asp:TemplateColumn ItemStyle-Width="30%" HeaderText="ISTANZE">
                            <ItemTemplate>
                               <%# this.GetIstanze((ConservazioneWA.WSConservazioneLocale.SearchObject)Container.DataItem) %>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
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
