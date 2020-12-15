<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneraSupporti.aspx.cs" Inherits="ConservazioneWA.PopUp.GeneraSupporti" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl" TagPrefix="rjs" %>
<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="../CSS/docspa_30.css" type="text/css" rel="Stylesheet" />
   <link href="../CSS/rubrica.css" type="text/css" rel="Stylesheet" />

    <script src="../PopCalendar2005/PopCalendar.js" type="text/javascript"></script>
    <script src="../PopCalendar2005/PopCalendarFunctions.js" type="text/javascript"></script>
   <base target="_self" />
  <style type="text/css">
        .style3
      {
          font-weight: bold;
          font-size: 10px;
          color: #666666;
          font-family: Verdana;
          height: 40px;
      }
      .style4
      {
          font-weight: bold;
          font-size: 10px;
          color: #666666;
          font-family: Verdana;
          height: 43px;
      }
   .style5
      {
          font-weight: bold;
          font-size: 10px;
          color: #666666;
          font-family: Verdana;
          height: 42px;
      }
    </style>
    <script language="javascript" type="text/javascript">
    function controllo()
    {
        var pattern=/^[a-zA-Z0-9éèàìòù\s\?\,\.\+\-\*\;\:\%\&\!\(\)\[\]\{\}\\\/''""]+$/;
        var patternDate=/^([0-9]{2})([\/])([0-9]{2})([\/])([0-9]{4})$/;
        if(window.form1.txt_collFisica.value=="")
        {
            alert('Inserire collocazione fisica');
            return false;
        }
        if(window.form1.TextArea1.value=="")
        {
            alert('Inserire Note');
            return false;
        }
//        if(window.form1.txt_data.value=="")
//        {   
//            alert('Inserire la data prevista per la prossima verifica');
//            return false;
//        }
        if(window.form1.txt_collFisica.value!="")
        {
            if(!pattern.test(window.form1.txt_collFisica.value))
            {
                alert('Formato testo collocazione fisica non valido');
                return false;
            }   
        }
        if(window.form1.TextArea1.value!="")
        {
            if(!pattern.test(window.form1.TextArea1.value))
            {
                alert('Formato testo delle note non valido');
                return false;
            }
        }
        if(document.getElementById('data_txt_Data').value!="")
        {
            if(!patternDate.test(document.getElementById('data_txt_Data').value))
            {
                alert('Il formato della data non è valido.\nIl formato richiesto è gg/mm/aaaa');
                window.form1.data_txt_Data.focus();
                return false;
            }   
        }else
        {
            if(document.getElementById('data_txt_Data').value=="")  
            {
                alert('Inserire la data prevista per la prossima verifica');
                return false;   
            }            
        }  
        return true;
    }
    </script>
</head>
<body style="background-color:#f2f2f2">
    <form id="form1" runat="server">
    <div>
                <table style="width:100%;">
                <tr>
                <td colspan="3" class="pulsanti" colspan="3" align="center">
                 <asp:Label ID="lb_intestazione" runat="server" Text="Registra dati supporto masterizzato:" CssClass="testo_grigio_scuro_grande"></asp:Label>
                </td>
                </tr>
            <tr>
                <td style="width:20px" >
                    &nbsp;</td>
                <td align="center" valign="bottom">
                    <table>
                    <tr>
                        <td align="left" class="testo_grigio_scuro">
                            
                            <asp:Label ID="lb_verifica" runat="server" Text="Risultato Verifica" 
                                Visible="False"></asp:Label>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:DropDownList ID="ddl_verifica" runat="server" Visible="False" 
                                CssClass="testo_grigio">
                                <asp:ListItem>riuscita</asp:ListItem>
                                <asp:ListItem>fallita</asp:ListItem>
                            </asp:DropDownList>
                            
                        </td>
                    </tr>
                        <tr>
                            <td align="left" class="style4">
                                <asp:Label ID="Label1" runat="server" Text="Collocazione fisica*:" 
                                    Width="161px"></asp:Label>
                                <asp:TextBox ID="txt_collFisica" runat="server" CssClass="testo_grigio" 
                                    Height="21px" Width="300px" ></asp:TextBox>
                            </td>
                               
                        </tr>
                        <tr>
                        <td class="style3" align="left">
                        
                            <asp:Label ID="Label2" runat="server" Text="Note*:" Width="161px"></asp:Label>
                            <textarea id="TextArea1" cols="20" name="S1" rows="2" runat="server" 
                                class="testo_grigio" style="height:80px; width:300px"></textarea></td>
                        </tr>
                        <tr>
                            <td align="left" class="style5">
                                
                                <asp:Label ID="Label3" runat="server" Text="Data prossima Verifica*:"></asp:Label>
                                
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <%--<asp:TextBox ID="txt_data" runat="server" CssClass="testo_grigio"></asp:TextBox>
                                &nbsp;
                               <rjs:PopCalendar ID="popCalendar" runat="server" Separator="/" Control="txt_data" />--%>
                               <uc1:Calendario ID="data" runat="server" PAGINA_CHIAMANTE="generaSupporti"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                        <asp:Button ID="btn_inserisci" runat="server"  Text="Inserisci" CssClass="pulsante" 
                                                onclick="btn_inserisci_Click" OnClientClick="return controllo();"/>
                                            </td>
                                        <td>
                                        <asp:Button ID="btn_chiudi" runat="server" Text="Annulla" CssClass="pulsante" 
                                                onclick="btn_chiudi_Click" />
                                           </td>
                                      
                                    </tr>
                                 
                                </table>
                            </td>
                        </tr>
                      <tr>
                            <td>
                                <asp:HiddenField ID="hd_data" runat="server" />
                                <asp:HiddenField ID="hd_verifica" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
        </table>      
    </div>  
    </form>
</body>
</html>
