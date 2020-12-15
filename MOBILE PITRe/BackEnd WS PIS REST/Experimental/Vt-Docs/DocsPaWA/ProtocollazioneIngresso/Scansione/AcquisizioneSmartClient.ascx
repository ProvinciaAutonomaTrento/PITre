<%@ Control Language="c#" AutoEventWireup="false" Codebehind="AcquisizioneSmartClient.ascx.cs" Inherits="ProtocollazioneIngresso.Scansione.AcquisizioneSmartClient" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<OBJECT id="imageViewer" height="0" width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTYzIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
	classid="http:../SmartClient/librerie/DPA.Web.dll#DPA.Web.Imaging.ImageViewerContainerControl"
	VIEWASTEXT>
</OBJECT>

<SCRIPT type="text/javascript">

    // Visualizzatore immagini per la scansione
    function ShowImageViewer(segnatura) {
        try {
            var imageViewer = document.getElementById("imageViewer");

            imageViewer.ApplyImageCompression = true;
            imageViewer.AllowScan = true;
            imageViewer.ShowOpenFile = false;
            imageViewer.ShowCloseFile = false;
            imageViewer.AllowSave = true;
            imageViewer.ShowPageNavigations = true;
            imageViewer.ShowZoomCapabilities = true;
            imageViewer.Title = "Acquisisci documento";
            imageViewer.SetSizeScreenPercentage(100, 100);
            imageViewer.Signature = segnatura;

            var ret = imageViewer.ShowImageViewer();

            var imagePath = "";

            if (ret) {
                imagePath = imageViewer.ImagePath;
            }

            return imagePath;
        }
        catch (ex) {
            alert("Errore nell'utilizzo dei componenti client necessari per l'acquisizione:\n" + ex.message.toString());
        }
    }

</SCRIPT>