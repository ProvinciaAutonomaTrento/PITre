<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaStampe.aspx.cs" Inherits="ConservazioneWA.RicercaStampe" %>

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
        
        /*GM 26-7-2013
         modifica per evitare border doppio nel radiobuttonlist */
        .testoCheck td
        {
            border:none;
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

        //popup
        function showDialogFirma(idDoc, isFirmato) {
            var dimPopup = "700px";
            if (isFirmato == "True") {
                dimPopup = "640px";
            }
            var returnValue = window.showModalDialog("FirmaStampa.aspx?idDocumento=" + idDoc + "&firmato=" + isFirmato, "", "dialogWidth:" + dimPopup + ";dialogHeight:" + dimPopup + ";status:no;resizable:no;scroll:no;center:yes;help:no");
            
            //GM 23-7-2013
            //se il documento è stato firmato aggiorno l'elenco per non visualizzare
            //la stampa tra le non firmate
            if (returnValue == 'reload') {
                document.getElementById('reloadData').value = "1";
                document.forms["form1"].submit();
            }
        }

        //menu
        var cssmenuids = ["cssmenu6"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
        var csssubmenuoffset = -2 //Offset of submenus from main menu. Default is 0 pixels.

        function createcssmenu6() {
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
            window.addEventListener("load", createcssmenu6, false)
        else if (window.attachEvent)
            window.attachEvent("onload", createcssmenu6)

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
                <asp:Label ID="lbl_amm" runat="server"></asp:Label>
            </h2>
        </div>
        <uc6:Menu ID="menuTop" runat="server" PaginaChiamante="REGISTRO" />
        <div id="title">
            <h1>Ricerca Stampe</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upContent" runat="server">
                <ContentTemplate>
                    <div class="box_cerca">
                        <div align="center">
                            <fieldset>
                                <legend>Opzioni di ricerca</legend>
                                <table class="tabDate2">
                                    <tr>
                                        <td class="tabDateSx">Tipo stampe:</td>
                                        <td class="tabDateDx">
                                            <asp:RadioButtonList runat="server" ID="radFirmate" RepeatDirection="Horizontal" CssClass="testoCheck">
                                                <asp:ListItem Text="Tutte" Value="-1" Selected="True" />
                                                <asp:ListItem Text="Non firmate" Value="0" />
                                                <asp:ListItem Text="Firmate" Value="1" />
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tabDateSx">Data di stampa:</td>
                                        <td class="tabDateDx">
                                            <asp:DropDownList ID="ddl_dataStampa" runat="server" AutoPostBack="true" Width="140px" CssClass="ddl_list" OnSelectedIndexChanged="ddl_dataStampa_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Valore Singolo</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="true">Intervallo</asp:ListItem>
                                                <asp:ListItem Value="2">Oggi</asp:ListItem>
                                                <asp:ListItem Value="3">Settimana corrente</asp:ListItem>
                                                <asp:ListItem Value="4">Mese corrente</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="labelDa" runat="server" Text="Da" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="dataStampa_da" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                            <asp:Label ID="labelA" runat="server" Text="a" CssClass="testo_grigio_spazio"></asp:Label>
                                            <uc1:Calendario ID="dataStampa_a" runat="server" PAGINA_CHIAMANTE="RICERCAISTANZE" />
                                        </td>
                                    </tr>
                                </table>
                                <div align="center" style="margin-top: 5px; padding-top: 5px;">
                                    <asp:Button ID="btnFind" runat="server" OnClick="BtnSearch_Click" Text="Cerca" CssClass="cbtn" />
                                    <asp:HiddenField ID="reloadData" runat="server" Value="0" />
                                </div>
                            </fieldset>
                        </div> <!-- align -->
                    </div> <!-- box_cerca -->
                    <asp:UpdatePanel ID="upStampe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DataGrid ID="dg_StampeCons" runat="server" CssClass="tab_istanze" AllowSorting="true"
                            Width="100%" SkinID="datagrid" AllowPaging="true" AutoGenerateColumns="false" 
                            OnPageIndexChanged="SelectedIndexChanged" OnPreRender="dg_StampeCons_PreRender" > 
                                <HeaderStyle CssClass="tab_istanze_header" />
                                <AlternatingItemStyle CssClass="tab_istanze_a" />
                                <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" />
                                <SelectedItemStyle CssClass="selected_row" />
                                <PagerStyle CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom" />
                                    <Columns>
                                        <asp:BoundColumn DataField="idProfile" Visible="false" />
                                        <asp:BoundColumn DataField="chaFirmato" Visible="false" />
                                        <asp:BoundColumn DataField="idDocumento" HeaderText="ID Doc." />
                                        <asp:BoundColumn DataField="dataDocumento" HeaderText="Data Doc." />
                                        <asp:BoundColumn DataField="oggettoDocumento" HeaderText="Oggetto Documento" 
                                        ItemStyle-HorizontalAlign="Left" />
                                        <asp:TemplateColumn HeaderText="Stato">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lbl_firma"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Firma" >
                                            <ItemTemplate>
                                                <asp:ImageButton ID="btn_firma" runat="server" ImageUrl="~/Img/dett_lente.gif" 
                                                ToolTip="Visualizza dettagli firma" OnClick="BtnFirmaStampa_Click" />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                            </asp:DataGrid>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div> <!-- content -->
    </div>
    <!-- PopUp Wait-->
    <cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
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
