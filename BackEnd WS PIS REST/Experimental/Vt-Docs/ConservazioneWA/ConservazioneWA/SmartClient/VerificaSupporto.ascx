<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VerificaSupporto.ascx.cs" Inherits="ConservazioneWA.SmartClient.VerificaSupporto" %>

<form name="frm">
        <OBJECT id="verificaSupportoControl" height="0" width="0" 
                data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
                classid="http:../SmartClient/Librerie/VerificaSupporto.dll#VerificaSupporto.VerificaSupportoContainer"
                VIEWASTEXT>			
        </OBJECT>    
        <asp:HiddenField ID="hdSuccess" runat="server" />
        <asp:HiddenField ID="hdErrorMessage" runat="server" />
</form>

<script type="text/javascript">
    function verificaSupporto_execute(pathSupporto, percentualeVerifica, idIstanza, idDocumento) {
        var retValue = false;
        
        try {
            var ctrl = document.getElementById("verificaSupportoControl");

            ctrl.ServiceUrl = "<%=this.ServiceUrl%>";
            ctrl.IdPeople = "<%=this.IdPeople%>";
            ctrl.PathSupporto = pathSupporto;
            ctrl.PercentualeVerifica = percentualeVerifica;
            ctrl.IdIstanza = idIstanza;
            ctrl.IdDocumento = idDocumento;
            
            ctrl.Execute();

            var success = ctrl.Success;

            var hdSuccess = document.getElementById("<%=this.HdSuccessClientId%>");
            hdSuccess.value = success;


//            var txtErrorMessage = document.getElementById("txtErrorMessage");
//            alert(txtErrorMessage);
//            txtErrorMessage.value = ctrl.ErrorMessage;
        }
        catch (e) {
            alert(e.message.toString());
        }

        return retValue;
    }

</script>
