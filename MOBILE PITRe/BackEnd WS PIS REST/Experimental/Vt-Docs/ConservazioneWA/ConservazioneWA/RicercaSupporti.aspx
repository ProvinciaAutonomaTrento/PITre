<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaSupporti.aspx.cs" Inherits="ConservazioneWA.RicercaSupporti" %>
<%@ Register Assembly="MessageBox"  Namespace="Utilities" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl" TagPrefix="rjs" %>
<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>

<script src="PopCalendar2005/PopCalendarFunctions.js" type="text/javascript"></script>
<script src="PopCalendar2005/PopCalendar.js" type="text/javascript"></script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="CSS/docspa_30.css" type="text/css" rel="Stylesheet" />
   <link href="CSS/rubrica.css" type="text/css" rel="Stylesheet" /> 
   <link href="CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
   <script language="javascript">
   function showModalDialogSupporti()
   {
       var returnValue = window.showModalDialog("PopUp/GeneraSupporti.aspx?periodoVer=" + form1.hd_periodoVer.value + "&stato=P", "", "dialogWidth:600px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no");  

        form1.hd_generaSupp.value=returnValue;
    }

   function showModalDialogSuppProdotti()
   {
        alert('Il supporto dovrà essere cercato tra quelli in verifica');
   }
   
   function ControllaInput()
   {
        var pattern=/^[0-9]+$/;
        var pattern2=/^[a-zA-Z0-9\s]+$/;
        var patternDate=/^([0-9]{2})([\/])([0-9]{2})([\/])([0-9]{4})$/;
        if(window.form1.txt_idIstanza.value!="")
        {
            if(!pattern.test(window.form1.txt_idIstanza.value))
            {
                alert('Formato id istanza sbagliato');
                return false;
            }
        }
        if(window.form1.txt_idSupp.value!="")
        {
            if(!pattern.test(window.form1.txt_idSupp.value))
            {
                alert('Formato id supporto non valido');
                return false;
            }
        }
        if(window.form1.txt_collFisica.value!="")
        {
            if(!pattern2.test(window.form1.txt_collFisica.value))
            {
                alert('Formato collocazione fisica non valido');
                return false;
            }   
        }
        if(document.getElementById('dataScadMarca_da_txt_Data')!=null)
        {
            if(document.getElementById('dataScadMarca_da_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataScadMarca_da_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataScadMarca_da_txt_Data.focus();
                    return false;
                }   
            }
        }
        if(document.getElementById('dataScadMarca_a_txt_Data')!=null)
        {
            if(document.getElementById('dataScadMarca_a_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataScadMarca_a_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataScadMarca_a_txt_Data.focus();
                    return false;
                }   
            }
        } 
        if(document.getElementById('dataUltimaVer_da_txt_Data')!=null)
        {
            if(document.getElementById('dataUltimaVer_da_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataUltimaVer_da_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataUltimaVer_da_txt_Data.focus();
                    return false;
                }   
            }
        } 
        if(document.getElementById('dataUltimaVer_a_txt_Data')!=null)
        {
            if(document.getElementById('dataUltimaVer_a_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataUltimaVer_a_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataUltimaVer_a_txt_Data.focus();
                    return false;
                }   
            }
        } 
        if(document.getElementById('dataProxVer_da_txt_Data')!=null)
        {
            if(document.getElementById('dataProxVer_da_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataProxVer_da_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataProxVer_da_txt_Data.focus();
                    return false;
                }   
            }
        } 
        if(document.getElementById('dataProxVer_a_txt_Data')!=null)
        {
            if(document.getElementById('dataProxVer_a_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataProxVer_a_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataProxVer_a_txt_Data.focus();
                    return false;
                }   
            }
        } 
        if(document.getElementById('dataProd_da_txt_Data')!=null)
        {
            if(document.getElementById('dataProd_da_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataProd_da_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataProd_da_txt_Data.focus();
                    return false;
                }   
            }
        } 
        if(document.getElementById('dataProd_a_txt_Data')!=null)
        {
            if(document.getElementById('dataProd_a_txt_Data').value!="")
            {
                if(!patternDate.test(document.getElementById('dataProd_a_txt_Data').value))
                {
                    alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                    window.form1.dataProd_a_txt_Data.focus();
                    return false;
                }   
            }
        } 
   }
   
//   function alertSupporto()
//   {
//        alert('Impossibile generare supporto: deve essere ancora apposta la firma sui files di suppporto torna a gestione istanze');
   //   }

   function showModalDialogRegistraSupportoRimovibile(idIstanza, idSupporto) {
       var returnValue = window.showModalDialog("PopUp/RegistraSupporto.aspx?idIstanza=" + idIstanza + "&idSupporto=" + idSupporto,
                            "",
                            "dialogWidth:600px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no");


       return returnValue;
   }

   function showModalDialogVerificaSupportoRegistrato(idIstanza, idSupporto) {
       var returnValue = window.showModalDialog("PopUp/VerificaSupportoRegistrato.aspx?idIstanza=" + idIstanza + "&idSupporto=" + idSupporto,
                            "",
                            "dialogWidth:600px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no");


       return returnValue;
   }

   
   function showModalDialogSuppInVerifica()
   {
       var retValue = window.showModalDialog("PopUp/VerificaSupporto.aspx",
                            "",
                            "dialogWidth:600px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no");

       form1.hd_verifica.value = retValue;
   }
   
   function closePagina()
   {
    if(confirm("Si desidera uscire dall'applicazione?"))
        {
            parent.close();
            form1.hd_logOff.value="true";
            return true;
        }
        else
        {
            return false;
        }
   }
   
   var abilita = 'false';
   
   
   function wait()
        {
            if (abilita == 'true')
        {
            document.getElementById("divWait").style.display="none";
        }
        else
        {
            document.getElementById("divWait").style.display="block";
        }            
        }

        function go()
        {
            document.getElementById("divWait").style.display="none";

        }

        
        function setFocus()
        {
            window.form1.btn_cerca.focus();   
        }
   </script>
    <style type="text/css">
        .style4
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 141px;
        }
        .style11
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 438px;
            height: 33px;
        }
        .style13
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 125px;
        }
        .style15
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 418px;
        }
        .style17
        {
            width: 141px;
        }
        .style18
        {
            width: 223px;
        }
        .style19
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 223px;
            height: 31px;
        }
        .style20
        {
            width: 258px;
        }
        .style21
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 258px;
            height: 31px;
        }
        .style22
        {
            width: 175px;
        }
        .style23
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 175px;
            height: 31px;
        }
        .style24
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 151px;
            height: 31px;
        }
        .style25
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 438px;
        }
        .style26
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            height: 31px;
        }
        .style27
        {
            width: 101px;
        }
        </style>
</head>
<body onload="go();" defaultbutton="btnFind">
    <form id="form1" runat="server" onsubmit="wait();" defaultbutton="btnFind">
    <input type="hidden" name="hd_generaSupp" id="hd_generaSupp" runat="server" />
    <input type="hidden" name="hd_periodoVer" id="hd_periodoVer" runat="server" />
    <input type="hidden" name="hd_verifica" id="hd_verifica" runat="server" />
    <input type="hidden" name="hd_logOff" id="hd_logOff" runat="server" />   
    <input type="hidden" name="hd_dimSchermo" id="hd_dimSchermo" runat="server" />
    <div>
    
        <table style="width:100%;">
            <tr>
                <td>
                    <asp:Panel ID="PanelRicerca" runat="server" BorderWidth="1px" BorderColor="#810D06" BorderStyle="Solid" BackColor="#f2f2f2">
                        <table style="width:100%;">
                        <tr>
                        <td class="pulsanti">
                            <asp:Label ID="Label11" runat="server" Text="Ricerca Supporti" CssClass="testo_grigio_scuro"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td align="right">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="testo_red" 
                                onclick="LinkButton1_Click">ESCI</asp:LinkButton>
                        </td>
                        </tr>
                            <tr>
                                <td class="testo_grigio_scuro">
                                <table runat="server" width="100%">
                                <tr>
                                <td class="style19">
                                    <asp:Label ID="Label1" runat="server" Text="Stato supporto:"></asp:Label>
                                    </td>
                                    <td class="style21">
                                        <asp:CheckBox ID="cb_in_prod" runat="server" Text="In produzione" />
                                    </td>
                                    <%--<td class="style23">
                                        <asp:CheckBox ID="cb_prod" runat="server" Text="Prodotto" AutoPostBack="True" 
                                            oncheckedchanged="cb_prod_CheckedChanged"  />
                                    </td>--%>
                                    <td class="style21">
                                        <asp:CheckBox ID="cb_ver" runat="server" Text="In Verifica" />
                                    </td>
                                    <td class="style26">
                                        <asp:CheckBox ID="cb_eliminato" runat="server" Text="Eliminato"  />
                                    </td>
                                    <td class="style26">
                                    <asp:CheckBox ID="cb_danneggiato" runat="server" Text="Danneggiato" AutoPostBack="false" />
                                    </td>
                                </tr>
                                </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                <table runat="server" width="100%">
                                <tr>
                                <td class="style18">
                                <asp:Label ID="Label10" runat="server" CssClass="testo_grigio_scuro" 
                                                    Text="Coll. fisica:"></asp:Label>
                                                     &nbsp;<asp:TextBox ID="txt_collFisica" runat="server" 
                                        Width="127px" CssClass="testo_grigio"></asp:TextBox>
                                </td>
                                <td class="style20">
                                            <asp:Label ID="Label_idIstanza" runat="server" 
                                                Text="Id Istanza di conservazione:" CssClass="testo_grigio_scuro"></asp:Label>
                                                &nbsp;
                                         <asp:TextBox ID="txt_idIstanza" runat="server" Width="61px" 
                                                CssClass="testo_grigio"></asp:TextBox>
                                </td>
                                <td class="style22">
                                    <asp:Label ID="lbl_idSupporto" runat="server" CssClass="testo_grigio_scuro" 
                                        Text="Id Supporto:"></asp:Label>
                                    &nbsp;
                                    <asp:TextBox ID="txt_idSupp" runat="server" Width="61px" 
                                        CssClass="testo_grigio"></asp:TextBox>
                                    </td>
                                <td class="testo_grigio_scuro">
                                <%--<asp:CheckBox ID="cb_ricAggregata" runat="server"  Text="Ricerca aggregata"/>--%>
                                </td>
                                </tr>
                                </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table style="width:100%;">
                                        <tr>
                                            <td class="style25">
                                                                                             <asp:Panel ID="panel_data_scad_marca" runat="server"  Width="414px">
                                            <table>
                                            <tr>
                                            <td class="style17">
                                                <asp:Label ID="lb_dataScadMarca_da" runat="server" Text="Data scadenza marca:" 
                                                    CssClass="testo_grigio_scuro"></asp:Label>
                                            </td>
                                           <td>
                                           <asp:Label ID="lb_da" runat="server" Text="DA" CssClass="testo_grigio_scuro"></asp:Label>
                                            </td>
<%--                                           <td>
                                           <asp:TextBox ID="txt_dataScadMarca_da" runat="server" Width="75px" 
                                                   CssClass="testo_grigio"></asp:TextBox>
                                            </td>--%>
                                            <td >
                                            <%--<rjs:PopCalendar ID="popDataScad" runat="server" Control="txt_dataScadMarca_da" Separator="/" />--%>
                                            <uc1:Calendario ID="dataScadMarca_da" runat="server" />
                                            </td>
                                            <td>
                                            <asp:Label ID="lb_a" Text="A" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
                                            </td>
                     <%--                       <td>
                                            <asp:TextBox ID="txt_dataScadMarca_a" runat="server" Width="75px" 
                                                    CssClass="testo_grigio"></asp:TextBox>
                                            </td>--%>
                                            <td>
                                            <%--<rjs:PopCalendar ID="popDataScadMarcA" runat="server" Control="txt_dataScadMarca_a" Separator="/" />--%>
                                            <uc1:Calendario ID="dataScadMarca_a" runat="server" />
                                            </td>
                                            </tr>
                                            </table>
                                            </asp:Panel>                                                 
                                            </td>
                                            <td class="style15">                                                                                                    
                                                 <asp:Panel ID="Panel_data_ultima_ver" runat="server" 
                                                    Width="426px">
                                                    <table>
                                                        <tr>
                                                            <td class="style13">
                                                                <asp:Label ID="Label4" runat="server" Text="Data Ultima Verifica:"></asp:Label>
                                                            </td>
                                                            <td class="testo_grigio_scuro">
                                                                <asp:Label ID="Label5" runat="server" Text="DA"></asp:Label>
                                                            </td>
                                                           <%-- <td>
                                                                <asp:TextBox ID="txt_dataUltimaVer_da" runat="server" Width="75px" 
                                                                    CssClass="testo_grigio"></asp:TextBox>
                                                            </td>--%>
                                                            <td >
                                                                <%--<rjs:PopCalendar ID="PopCalendar3" runat="server" Separator="/" Control="txt_dataUltimaVer_da" />--%>
                                                                <uc1:Calendario ID="dataUltimaVer_da" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label6" runat="server" CssClass="testo_grigio_scuro" Text="A"></asp:Label>
                                                            </td>
                                                           <%-- <td>
                                                                <asp:TextBox ID="txt_dataUltimaVer_a" runat="server" Width="75px" 
                                                                    CssClass="testo_grigio"></asp:TextBox>
                                                            </td>--%>
                                                            <td>
                                                                <%--<rjs:PopCalendar ID="PopCalendar4" runat="server" Separator="/" 
                                                                    Control="txt_dataUltimaVer_a" />--%>
                                                                    <uc1:Calendario ID="dataUltimaVer_a" runat="server" />
                                                            </td>
                    
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style11">
                                            
                                             <asp:Panel ID="Panel_data_prox_ver" runat="server"  
                                                    Width="428px">
                                                    <table>
                                                        <tr>
                                                            <td class="style4">
                                                                <asp:Label ID="Label7" runat="server" Text="Data Prossima Verifica:"></asp:Label>
                                                            </td>
                                                            <td class="testo_grigio_scuro">
                                                                <asp:Label ID="Label8" runat="server" Text="DA"></asp:Label>
                                                            </td>
                                                            <%--<td>
                                                                <asp:TextBox ID="txt_dataProxVer_da" runat="server" Width="75px" 
                                                                    CssClass="testo_grigio"></asp:TextBox>
                                                            </td>--%>
                                                            <td >
                                                                <%--<rjs:PopCalendar ID="PopCalendar5" runat="server" Separator="/" Control="txt_dataProxVer_da" />--%>
                                                                <uc1:Calendario ID="dataProxVer_da" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label9" runat="server" CssClass="testo_grigio_scuro" Text="A"></asp:Label>
                                                            </td>
                                                            <%--<td>
                                                                <asp:TextBox ID="txt_dataProxVer_a" runat="server" Width="75px" 
                                                                    CssClass="testo_grigio"></asp:TextBox>
                                                            </td>--%>
                                                            <td>
                                                                <%--<rjs:PopCalendar ID="PopCalendar6" runat="server" Separator="/" 
                                                                    Control="txt_dataProxVer_a" />--%>
                                                                    <uc1:Calendario ID="dataProxVer_a" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                            <td class="style15"> 
                                                                                             <asp:Panel ID="Panel_data_prod" runat="server" Width="418px" >
                                                    <table>
                                                        <tr>
                                                            <td class="style13">
                                                                <asp:Label ID="lb_data" runat="server" Text="Data Produzione:"></asp:Label>
                                                            </td>
                                                            <td class="testo_grigio_scuro">
                                                                <asp:Label ID="Label2" runat="server" Text="DA"></asp:Label>
                                                            </td>
                                                            <%--<td>
                                                                <asp:TextBox ID="txt_dataProd_da" runat="server" Width="75px" 
                                                                    CssClass="testo_grigio"></asp:TextBox>
                                                            </td>--%>
                                                            <td >
                                                                <%--<rjs:PopCalendar ID="PopCalendar2" runat="server" Separator="/" Control="txt_dataProd_da" />--%>
                                                                <uc1:Calendario ID="dataProd_da" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label3" runat="server" CssClass="testo_grigio_scuro" Text="A"></asp:Label>
                                                            </td>
                                                           <%-- <td>
                                                                <asp:TextBox ID="txt_dataProd_a" runat="server" Width="75px" 
                                                                    CssClass="testo_grigio"></asp:TextBox>
                                                            </td>--%>
                                                            <td>
                                                                <%--<rjs:PopCalendar ID="PopCalendar1" runat="server" Separator="/" 
                                                                    Control="txt_dataProd_a" />--%>
                                                                    <uc1:Calendario ID="dataProd_a" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel> 
                                                        </td>

                                        </tr>
                                        <tr>
                                            <td class="style25">
                                                <%--<asp:CheckBox ID="cb_ricAggregata" runat="server"  Text="Ricerca aggregata"/>--%>
                                            </td>
                                       <td class="style15">
                                        </td>
                                        </tr>
                                        <tr>
           
                                            <td align="right" class="testo_grigio_scuro">
                                            <asp:Button ID="btn_cerca" runat="server" CssClass="pulsante"  Text="Cerca" 
                                                                onclick="btn_cerca_Click" OnClientClick="return ControllaInput();"/>
                                            </td>
                                            <td align="right">
                                            <asp:Button ID="btn_pulisci" runat="server" CssClass="pulsante"  
                                                    Text="Pulisci campi" onclick="btn_pulisci_Click"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                </td>
            </tr>
            <tr>
                <td align="center">                    
                    <asp:Panel ID="Panel_dettaglio" runat="server" Visible="true"
                    AllowSorting="True" AutoGenerateColumns="false"
                    Width="100%" SkinID="datagrid" AllowPaging="True" 
                    AllowCustomPaging="false" PageSize="10" >
                        <asp:DataGrid ID="grdSupporti" runat="server" Width="100%" 
                            AutoGenerateColumns="false" 
                            OnPageIndexChanged="grdSupporti_PageIndexChanged"
                            OnItemCommand="grdSupporti_ItemCommand" 
                            OnPreRender="grdSupporti_PreRender">
                            <Columns>
                                <asp:TemplateColumn Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdIstanza" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).idConservazione%>" />
                                        <asp:Label ID="lblIdSupporto" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).SystemID%>" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Istanza N. / Supporto N.">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnIstanza" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).idConservazione%>" CommandName="GO_TO_ISTANZA"></asp:LinkButton>
                                        <br />    
                                        /
                                        <br />                                    
                                        <asp:Label ID="lblSupporto" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).SystemID%>"> </asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Tipo / Stato" HeaderStyle-HorizontalAlign="Center">
                                  <ItemTemplate>
                                        <asp:Label ID="lblTipo" runat="server" 
                                            Text="<%#this.GetTipoSupporto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                        </asp:Label>
                                        <br />
                                        /
                                        <br />
                                        <asp:Label ID="lblStato" runat="server" 
                                            Text="<%#this.GetStatoSupporto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                        </asp:Label>
                                    </ItemTemplate>        
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Prodotto il">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDataProduzione" runat="server"
                                            Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).dataProduzione%>">
                                        </asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Collocazione fisica / Note">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCollocazioneFisica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).collocazioneFisica%>"></asp:Label>
                                            <br />
                                            /
                                            <br />
                                        <asp:Label ID="lblNote" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).Note%>"></asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText="Scadenza marca">
                                    <ItemTemplate>
                                        <asp:Label ID="lblScadenzaMarca" runat="server"
                                        Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).dataScadenzaMarca%>">
                                        </asp:Label>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Stato verifica">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDatiVerifica" runat="server" Text="<%#this.GetDatiVerifica((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>

                                <asp:TemplateColumn HeaderText="Azioni" ItemStyle-HorizontalAlign="center">
                                    <ItemTemplate>
                                        <asp:Panel ID="pnlAzioniSupportoRemoto" runat="server" 
                                            Visible="<%#this.IsSupportoRemoto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                            <a id="btnDownload" runat="server" href="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).istanzaDownloadUrl%>"
                                                    Visible="<%#this.IsDownlodable((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                                Download
                                            </a>
                                            <asp:Button ID="btnBrowse" runat="server" Text="Sfoglia" CommandName="BROWSE"
                                            CommandArgument="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).istanzaBrowseUrl%>"
                                            Visible="<%#this.IsBrowsable((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>"/>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel1" runat="server" Visible="<%#!this.IsSupportoRemoto((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>">
                                            <asp:Button ID="btnRegistraSupporto" runat="server" Text="Registra" CommandName="REGISTRA_SUPPORTO"  />
                                            <asp:Button ID="btnVerificaSupporto" runat="server" Text="Verifica" CommandName="VERIFICA_SUPPORTO" Visible="<%#this.IsSupportoRimovibileRegistrato((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>" />
                                            <asp:Button ID="btnStoriaVerifiche" runat="server" Text="Storia" CommandName="STORIA_VERIFICHE_SUPPORTO" Visible="<%#this.IsSupportoRimovibileRegistrato((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem)%>" />
                                        </asp:Panel>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </asp:Panel>
                    
                </td>
            </tr>
            
            <tr>
            <td align="center">
            <asp:Panel ID="Panel_verifica" runat="server" Visible="False">
                <div style="height:170px; overflow:auto; width:98%" id="div_verifica">

                </div>
                         </asp:Panel>
            </td>
            </tr>
            <tr>
            <td>
            </td>
            </tr>
            <tr>
            <td align="right">
                <%--<asp:Button runat="server" ID="bt_esci" OnClientClick="return closePagina();" CssClass="pulsante" Text="Esci" />--%>
            </td>
            </tr>
            <tr>
            <td>
                <asp:HiddenField ID="hd_idIstanza" runat="server" />
                <asp:HiddenField ID="hd_idSupporto" runat="server" />
                <asp:HiddenField ID="hd_numMarca" runat="server" />
                <asp:HiddenField ID="hd_ProfileTrasm" runat="server" />
            </td>
            </tr>
        </table>
    <cc1:MessageBox ID="msgRigeneraMarca" runat="server" />     
    </div>
    </form>    
    <div id="divWait" style="display:none; position:absolute; top:0; left:0; width:100%; height:600px">
        <div id="waitTrans"></div>            
        <div id="waitImg"></div>
    </div>
</body>
</html>
