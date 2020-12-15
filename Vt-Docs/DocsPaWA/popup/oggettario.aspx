<%@ Page Language="c#" CodeBehind="oggettario.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.oggettario" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Src="../documento/Oggetto.ascx" TagName="Oggetto" TagPrefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <script language="javascript" type="text/javascript">

 
   
    /* 
     questa funzione si scatena al click sulla combo dei registri e al load della pagina
     e si occupa di far sparire e disabilitare label e bottoni a seconda se gli RF sono
     abilitati o meno 
  */
  function ddlClick()
    {
        var idReg = null;
        var ddlReg = document.getElementById("ddlRegRf");

        if(ddlReg!=null)
        {
            idReg = document.getElementById("ddlRegRf").value;
          
         }
           
 
        var inserisci = document.getElementById("btn_aggiungi");
        var modifica = document.getElementById("btn_modifica");
        var cerca = document.getElementById("btn_cerca"); 
        var labelCombo = document.getElementById("lblCombo"); 
        var rfPresente = false;
      
       
       if(ddlReg!=null)
       {
            for (li=0;li<(document.getElementById('ddlRegRf').options.length);li++)
            {
                var valore = document.getElementById('ddlRegRf').options[li].value;

                var arrayValori = valore.split("_");
               
                if(arrayValori != null && arrayValori.length > 1)
                {
                  if(arrayValori[1]!=null && arrayValori[1]!="")
                  {
                    rfPresente = true;
                     break;
                  }

                }
            }
           
            if(rfPresente)
            {          
                labelCombo.innerHTML = 'Registro/RF';
            }
            else
            {
                labelCombo.innerHTML = 'Registro';       
            }
       }
      
      
        if(idReg!= null && idReg!="")
        {
            var myArray = idReg.split("_");
 
            if(myArray[1]=="0") // se RF è abilitato: abilito il pulsante INSERISCI e faccio sparire la label di avviso
            {
                     
               inserisci.disabled = false;
               modifica.disabled = false;
               
               inserisci.title = 'Inserisce un nuovo oggetto e lo associa all\'RF selezionato';
               cerca.title = 'Cerca considerando l\'RF selezionato';
            }
            else // RF disabilitato: disabilito il pulsante INSERISCI e faccio apparira la label di avviso
            {
              
               if(myArray[1]=="") //CASO DI UN REGISTRO
               {
                 
                    inserisci.disabled = false;
                    modifica.disabled = false;
                    inserisci.title = 'Inserisce un nuovo oggetto e lo associa al REGISTRO selezionato';
                    cerca.title = 'Cerca considerando il REGISTRO selezionato';
               }
               else
               {
                    modifica.disabled = true;
                    inserisci.disabled = true;
                    inserisci.title = 'Non è possibile associare un nuovo oggetto ad un RF disabilitato'; 
                    cerca.title = 'Cerca considerando l\'RF selezionato';
               } 
            }
            
        }
        else
        {
                        
            if(ddlReg!=null)
            {
              
              var indice_selezionato = document.getElementById('ddlRegRf').selectedIndex;		
            
           	
               var testoCombo = document.getElementById('ddlRegRf').options[indice_selezionato].innerHTML;
               if(testoCombo.toUpperCase()=="TUTTI")
               {
                    inserisci.title = 'Inserisce un nuovo oggetto comune a tutti i registri';
           
                    
                    if(rfPresente)
                    {
                        cerca.title = 'Cerca considerando tutti i REGISTRI  e tutti gli RF associati al ruolo';
   
                    }
                    else
                    {
                        cerca.title = 'Cerca considerando tutti i REGISTRI associati al ruolo';
                     }
               }
               else
               {
                    inserisci.title = 'Inserisce un nuovo oggetto e lo associa al REGISTRO selezionato';
                    
                    cerca.title = 'Cerca considerando il REGISTRO selezionato e gli eventuali RF ad esso associati';
           
               }
            	
           }
            
            inserisci.disabled = false;
            modifica.disabled = false;

            if(ddlReg==null)//se la combo dei registri non è visibile
            {
                var tabella = document.getElementById("principale");
                tabella.style.height = 375;
            }
           
        }
        
        //controllo se l'utente è autorizzato
         if((modifica.disabled==false) && (document.getElementById('hMOD_Authorized').value.toUpperCase()=="FALSE"))
         {
            modifica.disabled=true;
         }
            
         if((inserisci.disabled==false) && (document.getElementById('hINS_Authorized').value.toUpperCase()=="FALSE"))
         {
            inserisci.disabled=true;
         }
        
    }
    
    
    function colorCombo()
    {
         var ddlReg = document.getElementById("ddlRegRf");
         if(ddlReg!=null)
           {
                for (li=0;li<(document.getElementById('ddlRegRf').options.length);li++)
                {
                    var item = document.getElementById('ddlRegRf').options[li];
                    var valore = item.value;

                    var arrayValori = valore.split("_");
                   
                    if(arrayValori != null && arrayValori.length > 1)
                    {
                      if(arrayValori[1]!=null && arrayValori[1]!="")
                      {
                        item.style.color = 'Gray';
                      }

                    }
                }
               
           }
    }
    </script>

    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <base target="_self" />
</head>
<body ms_positioning="GridLayout" bottommargin="0" leftmargin="5" topmargin="5" rightmargin="5"
    onload="ddlClick();">
    <form id="oggettario" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Oggettario" />
    <table class="info" style="width: 550px; height: 343px" align="center" border="0"
        id="principale">
        <tr>
            <td class="item_editbox">
                <p class="boxform_item">
                    <asp:Label ID="lblRicOgg" runat="server">Ricerca/Inserimento in oggettario</asp:Label>
                </p>
            </td>
        </tr>
        <tr>
            <td>
                <table width="550px" border="0" class="infoDotted">
                    <tr>
                            <td valign="top">
                                <asp:label id="lblCombo" class="titolo_scheda" visible="true" runat="Server">
                                    Registro</asp:label>
                            </td>
                            <td width="85%">
                                <asp:DropDownList ID="ddlRegRf" runat="Server" Width="180px" CssClass="testo_grigio_combo"
                                    onchange="ddlClick()">
                                </asp:DropDownList>
                            </td>
                      
                        <td width="5%" align="right">
                            <asp:ImageButton ID="btn_clear" ImageAlign="Right" runat="server" AlternateText="Pulisci tutto" SkinID="clear_flag"></asp:ImageButton>
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom">
                            <asp:Label ID="LabelOggetto" runat="server" CssClass="titolo_scheda">Oggetto&nbsp;</asp:Label>
                        </td>
                        <td colspan="2">
                            <uc1:Oggetto ID="ctrl_oggetto" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <table width="100px">
                                <tr>
                                    <td>
                                        <asp:Button ID="btn_aggiungi" runat="server" CssClass="pulsante_hand" Text="Inserisci"
                                            ToolTip="Inserisci" />
                                    </td>
                                    <td align="center">
                                        <asp:Button ID="btn_modifica" runat="server" ToolTip="Modifica" Text="Modifica" CssClass="pulsante_hand"
                                            Enabled="False"></asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_cerca" runat="server" ToolTip="Cerca" Text="Cerca" CssClass="pulsante_hand">
                                        </asp:Button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table> 
            </td> 
        </tr>
        <tr style="display: none">
            <td>
                <input type="text" id="hCANC_Authorized" runat="server" />
                <input type="text" id="hMOD_Authorized" runat="server" />
                <input type="text" id="hINS_Authorized" runat="server" />
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" width="550px">
                <div style="overflow: auto; height: 200px">
                    <asp:Label ID="lbl_risultatoRicOgg" TabIndex="4" CssClass="testo_grigio_scuro_grande"
                        runat="server" Visible="False">Nessun oggetto presente.</asp:Label>
                    <asp:DataGrid ID="dg_Oggetti" runat="server" SkinID="datagrid" BorderStyle="Inset" AutoGenerateColumns="False"
                        CellPadding="1" BorderWidth="1px" BorderColor="Gray" Width="100%" OnItemCreated="dg_Oggetti_ItemCreated"
                        OnDeleteCommand="dg_Oggetti_ItemCommand" OnSelectedIndexChanged="dg_Oggetti_ItemSelected">
                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                        <ItemStyle HorizontalAlign="Left" Height="20px" CssClass="bg_grigioN"></ItemStyle>
                        <HeaderStyle Wrap="False" CssClass="menu_1_bianco_dg"></HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn HeaderText="Codice" Visible="false">
                                <HeaderStyle Width="150px"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="Label11" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codOggetto") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox11" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codOggetto") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Voce oggettario">
                                <HeaderStyle Width="450px"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="False">
                                <HeaderStyle></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="True">
                                <HeaderStyle></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="Label22" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codRegistro") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codRegistro") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="10px">
                                <HeaderStyle HorizontalAlign="center"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton1" runat="server" BorderWidth="0px" AlternateText="Seleziona"
                                        ImageUrl="../images/proto/ico_riga.gif" CommandName="Select"></asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="10px">
                                <HeaderStyle HorizontalAlign="center"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton2" runat="server" BorderWidth="0px" AlternateText="Cancella"
                                        ImageUrl="../images/proto/cancella.gif" CommandName="Delete"></asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </td>
        </tr>
        <tr>
            <td height="5">
            </td>
        </tr>
        <tr>
            <td align="center" height="30">
                <asp:Button ID="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:Button>
                &nbsp;
                <input class="PULSANTE" id="btn_chiudi" onclick="javascript:window.close()" type="button"
                    value="CHIUDI">
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
