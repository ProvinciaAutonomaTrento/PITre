<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalculateAtipicitaOptions.ascx.cs"
    Inherits="SAAdminTool.AdminTool.UserControl.CalculateAtipicitaOptions" %>
<div>
    <hr style="width: 80%; text-align: center;" />
</div>
<div>
    <asp:RadioButton GroupName="rblAtipicita" ID="optCalculate" runat="server" CssClass="testo"
        Checked="true" Text="Calcola Atipicità su documenti e fascicoli visibili dai sottoposti" />
    <div class="testo" style="margin-left:10px;">
        Scegliere l'opzione attualmente selezionata se, a seguito della creazione / spostamento
        del ruolo, non si intende calcolare la visibilità su documenti e fascicoli dei sottoposti
        (pulsante "Imposta visibilità registri" nella sezione "Registri - RF"). <span style="color: Red;">
            Attenzione!</span> A seconda della posizione gerarchica occupata dal ruolo e
        dalla mole di documenti / fascicoli esistenti, il calcolo dell'atipicità potrebbe
        richiedere un considerevole periodo di tempo per essere portato a termine.
    </div>
    <br />
    <asp:RadioButton GroupName="rblAtipicita" ID="optDontCalculate" runat="server" CssClass="testo"
        Text="Non calcolare Atipicità su documenti e fascicoli visti dai sottoposti" />
    <div class="testo" style="margin-left:10px;">
        Scegliere l'opzione attualmente selezionata se si desidera evitare il calcolo della
        atipicità su documenti e fascicoli. <span style="color: Red;">Attenzione!</span>
        Sarà necessario avviare manualmente il calcolo della visibilità su documenti e fascicoli
        visibili ai ruoli sottoposti (pulsante "Imposta visibilità registri" nella sezione
        "Registri - RF"). Finché non verrà calcolata la visibilità, tutti i documenti e
        fascicoli visibili dai ruoli sottoposti potrebbero risultare erroneamente a visibilità
        "tipica".
    </div>
</div>
