<%@ Import Namespace="DocsPAWA.DocsPaWR" %>
<%@ Import Namespace="DocsPAWA" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Login" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile" Assembly="DocsPAWA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<%bool notAcc = "1".Equals(System.Configuration.ConfigurationManager.AppSettings["NO_SITO_ACCESSIBILE"]);
  if (notAcc)
  {%>
      <script type="text/javascript">
          alert("Attenzione: si può accedere all'applicativo solo con Internet Explorer");
          var win = window.open("about:blank", "_self");
          win.close();
      </script>   
  <%}else{%>    
<html>
	<head>
		<title><%
            string policyAgent = null;
            policyAgent = ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED);    
            string titolo = System.Configuration.ConfigurationManager.AppSettings["TITLE"];
            if (titolo != null)
            {
             %>
                <%= titolo%>
             <%
                   }
            else
            {
             %>
                DOCSPA
		     <%} %> > Login</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
		<meta http-equiv="Content-Language" content="it-IT" />
		<meta name="keywords" content="DocsPA, Protocollo informatico" />
		<meta name="description" content="Sistema per la gestione del protocollo informatico" />
		<link rel="stylesheet" href="./Css/main.css" type="text/css" media="screen" title="default" />
	</head>
	<body>
		<div id="container">
			<%if (existsLogoAmm)
     {
            %>
			<div id="headerLoginCustom"><!-- dummy comment --></div>
			<%
                   }
     else
     {	    
            %>
     			<div id="headerLogin"><!-- dummy comment --></div>
            <%
                   }
            %>
			<div id="contentLogin">
	
				<%
				this.Create(Session);
			
				switch (this.Type)
				{
					case DocsPAWA.SitoAccessibile.Login.PageType.authentication:
                    case DocsPAWA.SitoAccessibile.Login.PageType.authentication_ammin:    
						Amministrazione[] amministrazioni = this.GetAmministrazioni();
			
						/*if (this.ErrorMessage!=null)
						{*/
				%>
						<div class="loginMessage">
							<%
							if (this.ErrorMessage!=null)
							{
							%>
								<em>Attenzione : <%=(this.ErrorMessage!=null) ? this.ErrorMessage : ""%></em>
							<%
							}
							%>	
						</div>
					<%
						/*}*/
						
						if (amministrazioni!=null)
						{
				    %>
								<form id="frmLogin" method="post" action="Login.aspx">
									<input type="hidden" id="action" name="action" value="login" />
									<fieldset>
										<legend>Inserimento credenziali di accesso</legend>
										
										<%
                                            if (amministrazioni.Length == 1)
                                            {
										%>
										<p class="labelFieldPair">
											<label for="txt_amministrazioni">Amministrazione </label>
											<input type="text" id="txt_amministrazioni" class="textField adminame" value="<%=amministrazioni[0].descrizione%>" readonly="readonly" />
										</p>
											<input name="cmbamministrazioni" id="cmbamministrazioni" type="hidden" value="<%=amministrazioni[0].systemId.ToString()%>" />
											<br />
										<%
                                            }
                                            else
                                           {
                                               if (this.Type.Equals(DocsPAWA.SitoAccessibile.Login.PageType.authentication))
                                               {
                                                   %>
                                                   <br />
                                                   <%
                                               
                                               }
                                               else {                                    
										%>
										<p class="labelFieldPair">
											<label for="cmbamministrazioni">Amministrazione </label>
											<select name="cmbamministrazioni" id="Select1">
											<!-- aggiunto il campo vuoto per avere la stessa gestione della login standard -->
											<option selected="selected", label="", value=""></option>  
										<%	
													bool foundSelection = false;
													foreach (DocsPAWA.DocsPaWR.Amministrazione amministrazione in amministrazioni)
													{
														bool toSelect = false;
														if (!foundSelection)
														{
															if (this.IdAmministrazione==null)
															{
																//toSelect = true;  //eliminato sabrina
																foundSelection = true;
															}
															else if (this.IdAmministrazione==amministrazione.systemId.ToString().ToLower())
															{
																toSelect = true;
																foundSelection = true;
															}
														}
									    %>
												<option label="<%=amministrazione.descrizione.ToLower()%>" 
													<%=((toSelect) ? " selected=\"selected\" " : "")%>
													value="<%=amministrazione.systemId.ToString().ToLower()%>">
												<%=amministrazione.descrizione%>
												</option>						
										<%
													}
										%>
											</select>
											</p>
											<br />
			                             <%
											}
                                        %>
                                         <%
											}
                                        %>
				
											<p class="labelFieldPair">
											<label for="userid">UserId </label>
			     							<input name="userid" id="userid" class="textField" type="text" size="20" <%=(this.UserId!=null && this.UserId!="") ? "value=\""+this.UserId+"\"" : ""%> />
			     							</p>
                                            <%			                        
                                                if (!(policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                                                {
                                                     %>
											<br />
											<p class="labelFieldPair">
											<label for="password">Password </label>
			     							<input name="password" id="password" class="textField" type="password" size="20" />
			     							</p>
                                             <%}%>
									</fieldset>
			                        <%
			                        
                                    if (!(policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                                    {
                                    %>
									<div>
										<input id="submit" type="submit" value="Accedi" class="button" />
									</div>
                                    <%}
                                        if ((policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper())){%>

                                        <div class="loginMessage" id="logout">
							        <%
							            if (this.LogoutMessage!=null)
							            {
							            %>
								            <em><%=(this.LogoutMessage != null) ? this.LogoutMessage : ""%></em>
							            <%
							            }
                                    }
							        %>	
						</div>
								</form>
							<%
						}
						break;
					case DocsPAWA.SitoAccessibile.Login.PageType.choice:
							%>
							
										<p class="message">
											<em><%=(this.ErrorMessage!=null) ? this.ErrorMessage : ""%></em>
										</p>
									
										<form id="frmSi" method="post" action="Login.aspx">
											<p>
												<input type="hidden" id="force" name="force" value="true" />
												<input type="hidden" id="action" name="action" value="login" />
												<input type="hidden" id="session" name="session" value="true" />
												<input id="Si" class="button" type="submit" value="Si" />
											</p>
										</form>
																
										<form id="frmNo" method="post" action="Login.aspx">
											<p>
                                            <input type="hidden" id="forced" name="forced" value="true" />
											<input id="No" class="button" type="submit" value="No" />
											</p>
										</form>									
															
							<%
							this.AddToSession();
						break;
					default:
						break;
				}
			
				%>
			</div><!-- end content -->
		</div><!-- end container -->
	</body>
</html>
<%} %>
