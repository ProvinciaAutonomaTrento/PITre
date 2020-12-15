<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpellWrapper.ascx.cs" Inherits="NttDataWA.ActivexWrappers.SpellWrappers" %>
<% if (this.UseSpellWrapper && !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "SpellWrapperObject")) { %> 
<object classid="CLSID:A47C22B1-3CC3-45bc-801E-3FCC4FFD3E45" codebase="../activex/AxSpell.cab#version=1,0,0,0" height="0" width="0"></object>
<% this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "SpellWrapperObject", string.Empty);} %>
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "SpellWrapperControlScript")){%>
<script id="SpellWrapperControlScript" type="text/javascript">
    //Questa funzione prende in input un testo e restituisce come output il medesimo testo corretto
    function SpellWrapper_Spell(myForm)
    {
        //Per il momento è disabilitato perchè non è ancora firmato!!!
        //if ("<%=UseActivexWrapper%>" == "True")
        //{
            if("<% =UseSpellWrapper %>" == "True")
            {
		        try
		        {
			        var objDownload = new ActiveXObject( "axSpellcheck.axSpellcheck" );
			        var ret =objDownload.InvokeMethod(myForm); //document.myForm.content.value);
			        ////document.myForm.content.value = ret;
		        }
		        catch(exception) {
			        //questa eccezione invece può segnalare la mancata installazione dell'ActiveX oppure un
			        //errato funzionamento!!!
			        alert( "Non è stato possibile caricare il controllo ortografico" );
			        return myForm;
		        }
		        return ret;
		    }
		    else
		    {
		        //questo alert deve essere tolto, ora serve solo per test!!!
		        //alert("Non è stato attivato il controllo ortografico da configurazione");
		        return myForm;
		    }
		//}
    }
</script>
<% this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "SpellWrapperControlScript", string.Empty); } %>
