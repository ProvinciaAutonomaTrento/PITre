<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePageNew.aspx.cs" Inherits="ConservazioneWA.HomePageNew" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Menu.ascx" TagName="Menu" TagPrefix="uc6" %>
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<%@ Register Assembly="MessageBox" Namespace="Utilities" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        
        .tab_riepiologo caption
        {
             background-image: url('Img/bg_tab_header.jpg');
             background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('Img/bg_content.jpg');
        }
        
        .tab_riepiologo tr.RowOverFirst
        {
            background-image: url('Img/bg_tab_riepologo.jpg');
            background-repeat: repeat-x;
        }
        
        .tab_riepiologo tr.RowOverSelected
        {
            background-image: url('Img/bg_tab_riepologo_hover.jpg');
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

       
        
        /* Sub level menu links style */
        /*.horizontalcssmenu ul li ul li a{
        width: 160px; /*width of sub menu levels*/
        /*font-weight: normal;
        /*padding: 2px 5px; tolto*/
        /*background: #e3f1bd; tolto*/
        /*border-width: 0 1px 1px 1px;
        /*}



        /*.horizontalcssmenu ul li a:hover{
        background: url(menubgover.gif) center center repeat-x;
        }*/

        /*.horizontalcssmenu ul li ul li a:hover{
        background: #cde686;
        }*/
       
	
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
        /* ultags[t].parentNode.offsetHeight + csssubmenuoffset + "px" */
        /* SCRIPT HORIZONTAL MENU  aggiunto A. Sigalot e C.Fuccia*/
        var cssmenuids = ["cssmenu1"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
        var csssubmenuoffset = -2 //Offset of submenus from main menu. Default is 0 pixels.

        function createcssmenu2() {
            for (var i = 0; i < cssmenuids.length; i++) {
                var ultags = document.getElementById(cssmenuids[i]).getElementsByTagName("ul")
                for (var t = 0; t < ultags.length; t++) {
                    ultags[t].style.top =ultags[t].parentNode.offsetHeight -1 +'px'
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
                }

                if ($('#' + cssmenuids[i] + ' ul.visibility').length > 0) {
                    $('#' + cssmenuids[i] + ' ul.visibility').each(function () {
                        this.style.visibility = "hidden";
                    });
                    $('#' + cssmenuids[i] + ' ul').removeClass('visibility');
                }
            }

            
        }
        if (window.addEventListener)
            window.addEventListener("load", createcssmenu2, false)
        else if (window.attachEvent)
            window.attachEvent("onload", createcssmenu2)
       
        /* Fine aggiunta script A. Sigalot e C.Fuccia */
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
                <h2><asp:Label ID="lbl_amm" runat="server"></asp:Label></h2>
        </div>
        <uc6:Menu ID="menuTop" runat="server" PaginaChiamante="HOME" />
        <div id="title">
            <h1>
                Riepilogo</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upContent" runat="server">
                <contenttemplate>       
                    <div id="box_riepilogo">         
                   <table class="tab_riepiologo" align="center">
                        <caption>Riepilogo istanze</caption>
                        <tr class="RowOverFirst" onmouseover="this.className='RowOverSelected';" onmouseout="this.className='RowOverFirst';" onclick="location.href='RicercaIstanze.aspx?q=I';" title="Nuove Istanze">
                            <td class="tab_riepiologo_sx">Nuove</td>
                            <td class="tab_riepiologo_dx"><asp:Label runat="server" ID="lbl_nuove"></asp:Label></td>
                        </tr>
                        <tr class="RowOverFirst" onmouseover="this.className='RowOverSelected';" onmouseout="this.className='RowOverFirst';" onclick="location.href='RicercaIstanze.aspx?q=L';" title="Istanze in Lavorazione">
                             <td class="tab_riepiologo_sx">In lavorazione</td>
                             <td class="tab_riepiologo_dx"><asp:Label runat="server" ID="lbl_lavorazione"></asp:Label></td>
                         </tr>
                         <tr class="RowOverFirst" onmouseover="this.className='RowOverSelected';" onmouseout="this.className='RowOverFirst';" onclick="location.href='RicercaIstanze.aspx?q=F';" title="Istanze Firmate">
                           <td class="tab_riepiologo_sx">Firmate</td>
                           <td class="tab_riepiologo_dx"><asp:Label runat="server" ID="lbl_firmate"></asp:Label></td>
                        </tr>
                        <tr class="RowOverFirst" onmouseover="this.className='RowOverSelected';" onmouseout="this.className='RowOverFirst';" onclick="location.href='RicercaIstanze.aspx?q=V';" title="Istanze Conservate">
                           <td class="tab_riepiologo_sx">Conservate</td>
                           <td class="tab_riepiologo_dx"><asp:Label runat="server" ID="lbl_conservate"></asp:Label></td>
                        </tr>
                        <tr class="RowOverFirst" onmouseover="this.className='RowOverSelected';" onmouseout="this.className='RowOverFirst';" onclick="location.href='RicercaIstanze.aspx?q=C';" title="Istanze Chiuse">
                           <td class="tab_riepiologo_sx">Chiuse</td>
                           <td class="tab_riepiologo_dx"><asp:Label runat="server" ID="lbl_chiuse"></asp:Label></td>
                        </tr>
                        <tr class="RowOverFirst" onmouseover="this.className='RowOverSelected';" onmouseout="this.className='RowOverFirst';" onclick="location.href='RicercaNotifiche.aspx';" title="Notifiche">
                           <td class="tab_riepiologo_sx">Notifiche</td>
                           <td class="tab_riepiologo_dx"><asp:Label runat="server" ID="lbl_notifiche"></asp:Label></td>
                        </tr>
                  </table>
                  <br />
                  <table class="tab_riepiologo" align="center">
                    <caption>Riepilogo istanze di esibizione</caption>
                    <tr class="RowOverFirst" onmouseover="this.className='RowOverSelected';" onmouseout="this.className='RowOverFirst';" onclick="location.href='Esibizione/RicercaIstanzeEsibizione.aspx?q=I';" title="Istanze di esibizione con richiesta di certificare">
                        <td class="tab_riepiologo_sx">Richiesta Certificazione</td>
                        <td class="tab_riepiologo_dx"><asp:Label runat="server" ID="lbl_daCertificare"></asp:Label></td>
                    </tr>
                  </table>
                  </div>
                 </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- PopUp Wait-->
    <cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
    <div id="Wait" runat="server" style="display: none; font-weight: normal; font-size: 13px;
        font-family: Arial; text-align: center;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <contenttemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="Img/loadingNew.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </contenttemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
