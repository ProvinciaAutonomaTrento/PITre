<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaNotifiche.aspx.cs" Inherits="ConservazioneWA.RicercaNotifiche" %>


<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Menu.ascx" TagName="Menu" TagPrefix="uc6" %>
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<%@ Register Assembly="MessageBox" Namespace="Utilities" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ricerca Notifiche</title>
    <link href="CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <link href="CSS/rubrica.css" type="text/css" rel="Stylesheet" />
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

        var cssmenuids = ["cssmenu8"] //Enter id(s) of CSS Horizontal UL menus, separated by commas
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
             <h2><asp:Label ID="lbl_amm" runat="server"></asp:Label></h2>
        </div>
        <uc6:Menu ID="menuTop" runat="server" PaginaChiamante="NOTIFICHE" />
        <div id="title">
            <h1>
                Ricerca Notifiche</h1>
        </div>
        <div id="content">
            <asp:UpdatePanel ID="upFiltriRicerca" runat="server" UpdateMode="Conditional" Visible="false">
                <contenttemplate>             
                    <div class="box_cerca">
                     <div align="center">
                        <fieldset>
                            <legend>Opzioni di ricerca</legend>
                            <table class="tabDate">
                                <tr>
                                    <td colspan="2">
                                        <asp:CheckBoxList ID="cblTipoScadenzaNotifica" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="0" Text="Notifiche in scadenza"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Notifiche già scadute"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                            </table>
                            <div align="center" style="margin-top:5px;padding-top:5px;border-top:1px dashed #eaeaea;">
                                <asp:Button ID="btnFind" runat="server"  Text="Cerca" CssClass="cbtn" />
                            </div>
                        </fieldset>
                        </div>
                    </div>
                </contenttemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="upDettaglio" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:DataGrid ID="grdNotifiche" runat="server" Width="100%" 
                        AutoGenerateColumns="false" 
                        cssClass="tab_istanze"
                        OnPageIndexChanged="grdNotifiche_PageIndexChanged"
                        OnItemCommand="grdNotifiche_ItemCommand"
                        OnPreRender="grdNotifiche_PreRender"
                        AllowPaging="True"
                        AllowCustomPaging="false" PageSize="20" >
                            
                        <HeaderStyle cssClass="tab_istanze_header"  />
                        <AlternatingItemStyle CssClass="tab_istanze_a"  />
                        <ItemStyle CssClass="tab_istanze_b" HorizontalAlign="Center" Height="50px" />
                        <PagerStyle CssClass="menu_pager_grigio"  Mode="NumericPages" Position="TopAndBottom"/>

                        <Columns>
                            <asp:TemplateColumn HeaderText="Id Istanza" HeaderStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdIstanza" runat="server" Visible="false" Text='<%#((ConservazioneWA.WSConservazioneLocale.InfoNotifica) Container.DataItem).IdIstanza%>' />
                                    <asp:LinkButton ID="btnGoToIstanza" runat="server" ToolTip="Vai all'istanza" CommandName="GO_TO_ISTANZA" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoNotifica) Container.DataItem).IdIstanza%>" />                                    
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Id Supporto" HeaderStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdSupporto" runat="server" Visible="false" Text='<%#((ConservazioneWA.WSConservazioneLocale.InfoNotifica) Container.DataItem).IdSupporto%>' />
                                    <asp:LinkButton ID="btnGoToSupporto" runat="server" ToolTip="Vai al supporto" CommandName="GO_TO_SUPPORTO" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoNotifica) Container.DataItem).IdSupporto%>" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblGiorniScadenza" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoNotifica) Container.DataItem).GiorniScadenza%>"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Data Scadenza" HeaderStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label ID="lblDataScadenza" runat="server" Text='<%#((ConservazioneWA.WSConservazioneLocale.InfoNotifica) Container.DataItem).DataScadenza.ToString("dd/MM/yyyy")%>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Descrizione" HeaderStyle-Width="60%">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescrizione" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoNotifica) Container.DataItem).Descrizione%>" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>

                    </asp:DataGrid>
                </ContentTemplate>
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

