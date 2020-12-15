<form method="post" action="Rubrica.aspx">
	<fieldset>	
		<legend>Filtri rubrica</legend>
		<div>
			<p class="labelFieldPair equals">
				<label for="cod">Codice</label>
				<input type="text" class="textField" name="cod" id="cod" size="40" maxlength="50" <%=(Properties.Codice!=null && Properties.Codice!="") ? "value=\""+Properties.Codice+"\"" : ""%> />
			</p>
			<br /><br />
			<p class="labelFieldPair equals">
				<label for="desc">Descrizione</label>
				<input type="text" class="textField" name="desc" id="desc" size="40" maxlength="50" <%=(Properties.Descrizione!=null && Properties.Descrizione!="") ? "value=\""+Properties.Descrizione+"\"" : ""%> />
			</p>
			<br /><br />
			<p class="labelFieldPair equals">
				<label for="citta">Citt&agrave;</label>
				<input type="text" class="textField" name="citta" id="citta" size="40" maxlength="50" <%=(Properties.Citta!=null && Properties.Citta!="") ? "value=\""+Properties.Citta+"\"" : ""%> />
			</p>
		</div>
		<div>
			<fieldset class="small">
				<legend>Tipologia</legend>
				<p class="labelFieldPair">
				<%
					if (Properties.UCheck)
					{
				%>	
						<label for="u">Unit&agrave; Organizzative</label>
						<input type="checkbox" <%=Properties.UStatus ? "checked=\"checked\"" : ""%> name="u" id="u" value="u" />
				<%
					}
					if (Properties.RCheck)
					{
				%>
						<label for="r">Ruoli</label>
						<input type="checkbox" <%=Properties.RStatus ? "checked=\"checked\"" : ""%> name="r" id="r" value="r" />
				<%
					}
					if (Properties.PCheck)
					{
				%>
						<label for="p">Utenti</label>
						<input type="checkbox" <%=Properties.PStatus ? "checked=\"checked\"" : ""%> name="p" id="p" value="p" />
				<%
					}
					if (Properties.LCheck)
					{
				%>
						<label for="l">Liste di Distribuzione</label>
						<input type="checkbox" <%=Properties.LStatus ? "checked=\"checked\"" : ""%> name="l" id="l" value="l" />
				<%
					}
				%>
				</p>
			</fieldset>
			<fieldset class="small">
				<legend>Locazione</legend>
				<p class="labelFieldPair">
					<%
						if (Properties.ICheck)
						{
					%>
							<label for="i">Interni</label>
							<input type="checkbox" <%=Properties.IStatus ? "checked=\"checked\"" : ""%> name="i" id="i" value="i" />
					<%
						}
						if (Properties.ECheck)
						{
					%>
							<label for="e">Esterni</label>
							<input type="checkbox" <%=Properties.EStatus ? "checked=\"checked\"" : ""%> name="e" id="e" value="e" />            			
					<%
						}
					%>
				</p>
			</fieldset>			 
		</div>
	</fieldset>
	<input type="hidden" id="dosearch" name="action" value="dosearch" />
	<p class="centerButtons">
		<input id="ts" type="submit" class="button" value="Avvia ricerca" />
		<input id="tr" type="reset" class="button" value="Pulisci modulo" />
	</p>
</form>