<%@ Page Language="c#" Codebehind="RubricaDocsPA.aspx.cs" AutoEventWireup="false" 
    Inherits="SAAdminTool.popup.RubricaDocsPA.RubricaDocsPA" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta http-equiv="MSThemeCompatible" content="no">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <link href="../../CSS/rubrica.css" type="text/css" rel="stylesheet">
    <base target="_FRAME_RUBRICA">

    <script language="javascript" src="../../LIBRERIE/rubrica.js"></script>

    <script language="javascript">
			function doNuovo(modify)
			{
				var w_width = 550;
				var w_height = 620;
				var newLeft=(screen.availWidth-50);
				var newTop=140;//(screen.availHeight-100);
				
				var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;dialogLeft:"+newLeft+"px;dialogTop:"+newTop+"px;center:no;help:no;resizable:no;scroll:yes;status:no";
				var res = window.showModalDialog ("Inserimento.aspx?modify=" + modify, window, opts);
			  
				if (res!=null)
				{ 
					doWait();
					window.document.forms['frmRubrica'].txtLoadCommand.value=res; 
					//window.document.forms['frmRubrica'].submit(); 
				}			
			}	
			
			function doWait()
			{
				var w_width = 300;
				var w_height = 100;
				var t = (screen.availHeight - w_height) / 2;
				var l = (screen.availWidth - w_width) / 2;			
				if(document.getElementById ("please_wait"))
				{	
				document.getElementById ("please_wait").style.top = t;
				document.getElementById ("please_wait").style.left = l;
				document.getElementById ("please_wait").style.width = w_width;
				document.getElementById ("please_wait").style.height = w_height;				
				//document.getElementById ("please_wait").style.visibility = "visible";
				document.getElementById ("please_wait").style.display = '';
			    
			    }
			}
			
			
			function OpenHelp(from) 
			{		
				var pageHeight= 600;
				var pageWidth = 800;
				//alert(from);
				var posTop = (screen.availHeight-pageHeight)/2;
				var posLeft = (screen.availWidth-pageWidth)/2;
				
				var newwin = window.showModalDialog('../../Help/Manuale.aspx?from=' + from,
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
			}		
			
			function StampaCorrispondenti()
			{				
				var args=new Object;
				args.window=window;
				window.showModalDialog("../../exportDati/exportDatiSelection.aspx?export=rubrica",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
			}
			
			function ApriImportaCorrispondenti()
			{
			    var myUrl = "../ImportCorrispondenti.aspx";
			    rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:490px;dialogHeight:210px;status:no;resizable:no;scroll:no;center:yes;help:no;");
			    //rtnValue = window.open(myUrl,"","dialogWidth:430px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
			    frmRubrica.submit();
//			    window.showModalDialog('../../Import/Rubrica/ImportRubrica.aspx', '',
//								'dialogWidth:690px;dialogHeight:430px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
			}
					
    </script>

    <script language="javascript" id="btnAnnulla_click" event="onclick()" for="btnAnnulla">
			window.close();
    </script>

    <script language="javascript" id="btnCerca_Click" event="onclick()" for="btnCerca">
           
            if(document.getElementById ("txt_codice_fiscale").value != "" && document.getElementById ("txt_codice_fiscale").value.length<4)
            {
                alert("Attenzione, il campo CODICE FISCALE richiede una lunghezza minima di caratteri pari a 4");
                return false;
            }

            if(document.getElementById ("txt_partita_iva").value != "" && document.getElementById ("txt_partita_iva").value.length<4)
            {
                alert("Attenzione, il campo PARTITA IVA richiede una lunghezza minima di caratteri pari a 4");
                return false;
            }

			if(document.getElementById ("tbxDescrizione").value == ""
			 && document.getElementById ("tbxCitta").value == ""
			 && document.getElementById ("tbxCodice").value == ""
              && document.getElementById ("tbxLocal").value == ""
              && document.getElementById ("txt_mail").value == ""
              && document.getElementById ("txt_codice_fiscale").value == ""
              && document.getElementById ("txt_partita_iva").value == ""
              )
			{
                
				if (!window.confirm('Non è stato specificato alcun criterio di ricerca, il sistema\npotrebbe richiedere l\'attesa di un tempo piuttosto lungo.\n\nSi desidera procedere?')) 
				{
				    document.getElementById("tbxDescrizione").focus();
				    return false;
				}
				else
				{
			        window.document.body.style.cursor='wait';
			        
			        doWait();
			    }
						
			}
			else
			{
			        window.document.body.style.cursor='wait';
			        doWait();
			}
            
			
			
    </script>

    <script language="javascript" type="text/javascript">
			
	
			
			function eventoComboIE()
			{
			    var ddlIe = document.getElementById("ddlIE");
			    
			    if(ddlIe!=null)
			    {
			         var indSelIE = ddlIe.selectedIndex;
			         var rfPresente = false;
			          
			         var testoCombo = ddlIe.options[indSelIE].innerHTML;
			        
			         var ddlRf =  document.getElementById("ddl_rf");
			         
			         var inserisci = document.getElementById("btnNuovo");
			         
			         //se è selezionato o Esterni i Interni/Esterni allora
			         //rendo visibile il pannello degli RF, ma solo se cè almeno un RF,
			         //altrimenti rimane invisibile
			       		
			       	var labelReg = document.getElementById("lbl_registro");	
                    //var lblDisable = document.getElementById("lblDisable");
		        
		           
			       	 //calcola se cè almeno un RF nella combo 
	                 if(ddlRf!=null)
			         {
                         for (li=0;li<(ddlRf.options.length);li++)
                         {
                            var valore = ddlRf.options[li].value;

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
                         
                          
                            
                                                     
			             if(indSelIE>0 && (testoCombo.toUpperCase() == "ESTERNI" || testoCombo.toUpperCase() == "TUTTI" ))
			             {
		                    //rendo visibile il pannello degli RF
		                    ddlRf.style.display='';
                            labelReg.style.display='';
                            
		                    ddlRf.selectedIndex=0;
		                    var indSelRf = ddlRf.selectedIndex;
	  
			                 var valoreRf = ddlRf.options[indSelRf].value;
		                   
		                     var arrayValori = valoreRf.split("_");
		                     
		                   
		                    if(arrayValori != null && arrayValori.length > 1)
                             {
                                  if(arrayValori[1]!=null && arrayValori[1]!="")
                                  {
                                  
                                    if(arrayValori[1]=="1")
                                    {
                                        //labelReg.innerHTML = 'RF'; 
                                       /* if(lblDisable!=null)
                                        {
                                            lblDisable.innerHTML='&nbsp;disabilitato';
                                            lblDisable.style.color='red';
                                           
                                            lblDisable.style.display='';
                                        }
                                        */
                                        
                                        inserisci.disabled = true;
                                        inserisci.title = 'Non è possibile creare un nuovo corrispondente in un RF disabilitato'; 
                                    }  
                                    else
                                    { 
                                   
                                        //labelReg.innerHTML = 'RF'; 
                                        inserisci.disabled = false;
                                        inserisci.title = 'Nuovo corrispondente'; 
                    
                                    }
                                  }
                              else
                              {
                                
                              //  labelReg.innerHTML = 'Registro';
                                inserisci.title = 'Nuovo corrispondente'; 
                                inserisci.disabled = false;
                              }
                                                                                      
                            }
                         
                         
                              
		                    if(rfPresente) //se cè almeno un RF
		                    {
		                        // abilito la combo degli RF		                  
		                        ddlRf.disabled = false;
		                       
		                    }     
		                    else
		                    {
			                    // disabilito la combo degli RF	
			                    ddlRf.disabled = true;		                   
		                    }
    			         }
			             else
			             {
			                //rendo invisibile il pannello degli RF se si selezionano 
			                //le voci Interni oppure Interni/Esterni
			                labelReg.style.display = 'none';
			                ddlRf.style.display = 'none';
			                             
			             }
			        }
			    }
			    
			    showCheckRubricaComune();
			}

            // Gestione visibilità check ricerca in rubrica comune
            function showCheckRubricaComune()
            {
                var rcAbilitata = "<%=GestioneRubricaComuneAbilitata%>";
                
			    if (!rcAbilitata)
			    {
			        document.getElementById("pnlRubricaComune").style.display = 'none';
                }
                else
                {
                    var ddlIe = document.getElementById("ddlIE");
                    
                    if (ddlIe != null)
                    {
                        var v = ddlIe.options[ddlIe.selectedIndex].value;
                        
                        if (v == "E" || v == "IE")
                            document.getElementById("pnlRubricaComune").style.display = '';
                        else
                            document.getElementById("pnlRubricaComune").style.display = 'none';
                    }    
                }
            }
			
			function eventoComboRF()
			{
			    var ddlIe = document.getElementById("ddlIE");
			    
			  
			    if(ddlIe!=null)
			    {
			         var indSelIE = ddlIe.selectedIndex;
			         
			         var rfPresente = false;
			         var testoCombo = ddlIe.options[indSelIE].innerHTML;
			        
			         var ddlRf =  document.getElementById("ddl_rf");
			         
			         var inserisci = document.getElementById("btnNuovo");
			         
			         //se è selezionato o Esterni i Interni/Esterni allora
			         //rendo visibile il pannello degli RF, ma solo se cè almeno un RF,
			         //altrimenti rimane invisibile
			       		
			       	var labelReg = document.getElementById("lbl_registro");	
                    //var lblDisable = document.getElementById("lblDisable");
		        
		           
			       	 //calcola se cè almeno un RF nella combo 
	                 if(ddlRf!=null)
			         {
                         for (li=0;li<(ddlRf.options.length);li++)
                         {
                            var valore = ddlRf.options[li].value;

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
                                labelReg.innerHTML = 'Registro/RF';
                            }
                            else
                            {
                                labelReg.innerHTML = 'Registro';       
                            }                        
			             if(indSelIE>0 && ddlRf.options.length>0 && (testoCombo.toUpperCase() == "ESTERNI" || testoCombo.toUpperCase() == "TUTTI" ))
			             {
		                    //rendo visibile il pannello degli RF
		                    labelReg.style.display='';
		                    ddlRf.style.display='';
                           
                            
		                    var indSelRf = ddlRf.selectedIndex;
	  
			                 var valoreRf = ddlRf.options[indSelRf].value;
		                   
		                     var arrayValori = valoreRf.split("_");
		                     
		                   
		                    if(arrayValori != null && arrayValori.length > 1)
                             {
                                  if(arrayValori[1]!=null && arrayValori[1]!="")
                                  {
                                  
                                    if(arrayValori[1]=="1")
                                    {
                                      //  labelReg.innerHTML = 'RF'; 
                                       /* if(lblDisable!=null)
                                        {
                                            lblDisable.innerHTML='&nbsp;disabilitato';
                                            lblDisable.style.color='red';
                                           
                                            lblDisable.style.display='';
                                        }
                                        */
                                        inserisci.disabled = true;
                                        inserisci.title = 'Non è possibile creare un nuovo corrispondente in un RF disabilitato'; 
                                    }  
                                    else
                                    { 
                                   
                                      //  labelReg.innerHTML = 'RF'; 
                                        inserisci.disabled = false;
                                        inserisci.title = 'Inserisci un nuovo corrispondente'; 
                    
                                    }
                                  }
                              else
                              {
                            
                               // labelReg.innerHTML = 'Registro';
                                inserisci.title = 'Inserisci un nuovo corrispondente'; 
                                inserisci.disabled = false;
                              }
                                                                                      
                            }
                              
		                    if(rfPresente) //se cè almeno un RF
		                    {
		                        // abilito la combo degli RF		                  
		                        ddlRf.disabled = false;
		                       
		                    }     
		                    else
		                    {
			                    // disabilito la combo degli RF	
			                    ddlRf.disabled = true;		                   
		                    }
    			         }
			             else
			             {
			                //rendo invisibile il pannello degli RF se si selezionano 
			                //le voci Interni oppure Interni/Esterni
			                
			                labelReg.style.display='none';
			                ddlRf.style.display = 'none';
			                             
			             }
			        }
			        
			    }
			}
    </script>
</head>
<body bgcolor="#f2f2f2" scroll="auto" onload="eventoComboRF();">
    <form id="frmRubrica" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Rubrica" />
        <input type="hidden" id="txtLoadCommand" runat="server">
<table id="Table1Type" height="15px" cellspacing="0" cellpadding="6" width="100%" align="center">
    <tr><td>
        <asp:Label ID="labelCallType" Width="100%" Text="CallType:" runat="server" Visible="false"></asp:Label>
    </td></tr>
</table>           
        <table id="Table12" height="100%" cellspacing="0" cellpadding="6" width="100%" align="center"
            bgcolor="#eaeaea" border="0">
            <tr>
                <td style="height: 86px" valign="top" width="50%" bgcolor="#eaeaea" colspan="2" valign=top >
                    <table id="Table1" style="border-bottom: black 1px solid" cellspacing="1" cellpadding="1"
                        width="100%" border="0">
                        <tr>
                            <td class="testo_grigio_scuro" style="width: 79px; height: 5px">
                                Tipo
                            </td>
                            <td style="width: 224px; height: 5px">
                                <asp:DropDownList ID="ddlIE" runat="server" 
                                    CssClass="testo_grigio_scuro" onchange="eventoComboIE()" Width="220px">
                                    <asp:ListItem Value="I">Interni</asp:ListItem>
                                    <asp:ListItem Value="E">Esterni</asp:ListItem>
                                    <asp:ListItem Value="IE">Tutti</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="height: 5px" align="right" width="80px">
                                <asp:CheckBox ID="cbComandi" TabIndex="4" runat="server" CssClass="testo_grigio_scuro" Text="Uffici" TextAlign="Left">
                                </asp:CheckBox>
                            </td>
                            <td class="testo_grigio_scuro" style="width: 100px; height: 5px" valign="top" align="left">
                                <asp:Panel ID = "pnlRubricaComune" runat = "server">
                                    <asp:CheckBox ID="chkRubricaComune" runat="server" 
                                        CssClass="testo_grigio_scuro" Text="Rub. comune"
                                        TextAlign="Left" Visible="True" Checked="True"></asp:CheckBox>                                 
                                </asp:Panel>
                            </td>
                            <td class="testo_grigio_scuro" style="width: 50px; height: 5px" valign="top" align="right" >
                                <asp:ImageButton ID="btnCerca" runat="server" SkinID="btnCerca" ToolTip="Avvia la ricerca"></asp:ImageButton>
                            </td>
                            <td class="testo_grigio_scuro" style="width: 50px; height: 5px" valign="top" align="right" >
                                <asp:ImageButton ID="btnOk" runat="server" SkinID="btnOk" ToolTip="Chiudi e utilizza la selezione corrente"></asp:ImageButton>
                            </td>
                            <td class="testo_grigio_scuro" rowspan="5">
                                <asp:ImageButton ID="help" runat="server" OnClientClick="OpenHelp('GestioneRubrica')" AlternateText="Aiuto?" SkinID="btnHelp" border="0" />
                                <br />
                                <fieldset style="width:30%; text-align:left;">
                                    <legend class="testo_grigio">Legenda</legend>
                                   
                                    <ul>
                                        <li>
                                            <span class="testo_grigio">Corrispondenti in <span style="color:Red;">rosso</span>: ruoli inibiti alla ricezione di trasmissioni</span>
                                        </li>
                                        <li>
                                            <span class="testo_grigio">Corrispondenti <span style="text-decoration: line-through;">barrati</span>: ruoli disabilitati</span>
                                        </li>
                                    </ul>
                                </fieldset>
                            </td>

                        </tr>
                        <tr>
                            <td class="testo_grigio_scuro" style="width: 79px; height: 5px">
                                Codice
                            </td>
                            <td style="width: 224px; height: 5px">
                                <asp:TextBox ID="tbxCodice" TabIndex="1" runat="server" CssClass="testo_grigio" Width="220px"></asp:TextBox>
                            </td>
                            <td style="height: 5px" align="right" width="80px">
                                <asp:CheckBox ID="cbUtenti" TabIndex="6" runat="server" CssClass="testo_grigio_scuro"
                                    Text="Utenti" TextAlign="Left"></asp:CheckBox>
                            </td>
                            <td class="testo_grigio_scuro" style="width: 80px; height: 5px" valign="top" align="left">
                                &nbsp;</td>
                            <td valign="top">                         
                                <asp:ImageButton ID="btnReset" runat="server" SkinID="btnReset" ToolTip="Reimposta i parametri di ricerca"></asp:ImageButton>                         
                            </td>
                            <td class="testo_grigio_scuro" style="width: 50px; height: 5px" valign="top" align="right" >
                                <cc1:ImageButton ID="btnNuovo" runat="server" Thema="b_" SkinID="nuovo"
                                    ToolTip="Inserisci un nuovo corrispondente" DisabledUrl="../../App_Themes/ImgComuni/btn_nuovo_nonattivo.gif">
                                </cc1:ImageButton>
                            </td>
                            <td style="width: 10px; height: 5px" valign="bottom" align="right" >
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td class="testo_grigio_scuro" style="width: 79px">Descrizione&nbsp;</td>
                            <td style="width: 224px">
                                <asp:TextBox ID="tbxDescrizione" TabIndex="2" runat="server" CssClass="testo_grigio"
                                    Width="220px"></asp:TextBox></td>
                            <td align="right" width="80px">
                                <asp:CheckBox ID="cbRuoli" TabIndex="7" runat="server" CssClass="testo_grigio_scuro"
                                    Text="Ruoli" TextAlign="Left"></asp:CheckBox></td>
                            <td align="left" valign="top" width="80px">
                                &nbsp;</td>
                            <td class="testo_grigio_scuro" style="width: 50px; height: 5px" valign="top" align="right" >
                                <asp:ImageButton ID="btnEsporta" runat="server" SkinID="btnEsporta" ToolTip="Esporta"></asp:ImageButton>                         
                            </td>
                           <td class="testo_grigio_scuro" >
                                <asp:ImageButton ID="btnAnnulla" runat="server" SkinID="btnAnnulla" ToolTip="Chiudi senza salvare le modifiche"></asp:ImageButton></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="testo_grigio_scuro" style="width: 79px">Città</td>
                            <td style="width: 224px">
                                <asp:TextBox ID="tbxCitta" TabIndex="3" runat="server" CssClass="testo_grigio" Width="220px"></asp:TextBox></td>
                            <td class="testo_grigio_scuro" align="right" width="80px">
                                <asp:CheckBox ID="cbListeDist" runat="server" CssClass="testo_grigio_scuro" Text="Liste"
                                    TextAlign="Left" Visible="False" Checked="True"></asp:CheckBox></td>
                            <td class="testo_grigio_scuro" align="left" width="80px">
                                &nbsp;</td>
                           <td class="testo_grigio_scuro" >
                                <cc1:ImageButton ID="btnImporta" runat="server" Thema="btn_" SkinID="importa" ToolTip="Importa">
                                </cc1:ImageButton>
                            </td>  
                             <td align="left">
                               <asp:HyperLink ID="hlLink" runat="server" Target="_blank" CssClass="testo_grigio_scuro" ForeColor="Blue">Scarica modello</asp:HyperLink>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="testo_grigio_scuro" style="width: 79px">Località</td>
                            <td style="width: 224px"><asp:TextBox ID="tbxLocal" TabIndex="3" runat="server" CssClass="testo_grigio" Width="220px"></asp:TextBox></td>
                            <td align="right" width="80px">                                
                                <asp:CheckBox ID="cb_rf" runat="server" CssClass="testo_grigio_scuro" Text="RF"
                                    TextAlign="Left" Visible="False"></asp:CheckBox></td>
                            <td class="testo_grigio_scuro" align="left" width="80px">&nbsp;</td>
                            <td class="testo_grigio_scuro" >&nbsp;</td>  
                            <td class="testo_grigio_scuro">&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lbl_registro" runat="server" CssClass="testo_grigio_scuro">Registro / RF</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddl_rf" runat="server" Width="220px" 
                                    CssClass="testo_grigio_scuro" onchange="eventoComboRF()">
                                </asp:DropDownList>
                            </td>
                            <td align="right" width="80px">
                            </td>
                            <td width="80px"></td>
                            <td></td>
                            <td>&nbsp;</td>
                         </tr>   
                         <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" CssClass="testo_grigio_scuro">Mail Principale</asp:Label>
                            </td>
                            <td>
                           <asp:TextBox ID="txt_mail" TabIndex="3" runat="server" CssClass="testo_grigio" Width="220px"></asp:TextBox>
                            </td>
                            <td align="right" width="80px">
                            </td>
                            <td width="80px"></td>
                            <td></td>
                            <td>&nbsp;</td>
                        </tr>  
                        <tr>
                            <td>
                                <asp:Label ID="lblCodiceFiscale" runat="server" CssClass="testo_grigio_scuro">Codice Fiscale</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txt_codice_fiscale" TabIndex="3" runat="server" CssClass="testo_grigio" Width="220px"  MaxLength="16"></asp:TextBox>
                            </td>
                            <td align="right" width="80px"></td>
                             <td width="80px"></td>
                            <td></td>
                            <td>&nbsp;</td>
                        </tr>       
                        <tr>
                            <td>
                                <asp:Label ID="lblPartitaIva" runat="server" CssClass="testo_grigio_scuro">Partita Iva</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txt_partita_iva" TabIndex="3" runat="server" CssClass="testo_grigio" Width="220px" MaxLength="11"></asp:TextBox>
                            </td>
                            <td align="right" width="80px"></td>
                             <td width="80px"></td>
                            <td></td>
                            <td>&nbsp;</td>
                        </tr>                
                    </table>
                </td>
            </tr>
            <tr bgcolor="#f2f2f2">
                <td valign="top" width="500">
                    <table id="Table8" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
                        <tr>
                            <td>
                                <asp:PlaceHolder ID="phTabStrip" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" height="100%">
                                <asp:Panel ID="pnlElenco" runat="server">
                                    <asp:PlaceHolder ID="phCorr" runat="server"></asp:PlaceHolder>
                                    <asp:CheckBox ID="cbSelAll" runat="server" CssClass="testo_grigio_scuro" Text="Seleziona tutti"
                                        AutoPostBack="True"></asp:CheckBox>
                                </asp:Panel>
                                <asp:Panel ID="pnlOrganigramma" runat="server" Width="500px" Height="100%">
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; width: 500px;
                    padding-top: 0px" valign="top">
                    <table id="Table2" height="100%" cellspacing="3" width="100%" border="0">
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td class="testo_grigio_scuro" id="td_lblA" valign="middle" align="center" width="16"
                                height="50%" runat="server">
                                <asp:Label ID="lblA" runat="server">Label</asp:Label><br>
                                <asp:ImageButton ID="ibtnMoveToA" runat="server" ImageUrl="../../images/rubrica/b_arrow_right2.gif">
                                </asp:ImageButton></td>
                            <td style="overflow: auto" valign="top" height="50%">
                                <asp:PlaceHolder ID="phA" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr>
                            <td class="testo_grigio_scuro" valign="middle" align="center" width="16" height="50%">
                                <asp:Label ID="lblCC" runat="server">Label</asp:Label><br>
                                <asp:ImageButton ID="ibtnMoveToCC" runat="server" ImageUrl="../../images/rubrica/b_arrow_right2.gif">
                                </asp:ImageButton></td>
                            <td style="overflow: auto" valign="top" height="50%">
                                <asp:PlaceHolder ID="phCC" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                    </table>
                    
                </td>
            </tr>
        </table>
        <div id="please_wait" style="display: none; border-right: #000000 2px solid; border-top: #000000 2px solid;
            border-left: #000000 2px solid; border-bottom: #000000 2px solid; position: absolute;
            background-color: #d9d9d9">
            <table cellspacing="0" cellpadding="0" width="350px" border="0">
                <tr>
                    <td valign="middle" style="font-weight: bold; font-size: 12pt; font-family: Verdana"
                        align="center" width="350px" height="90px">
                        Attendere, prego...
                    </td>
                </tr>
            </table>
        </div>
      </form>
</body>
</html>
