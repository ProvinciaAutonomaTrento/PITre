// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Models.InsertTemplates;
public class Notification
{
    public long SystemId { get; set; }
    public long IdEvent { get; set; }
    public string? DescProducer { get; set; }
    public long IdPeopleReceiver { get; set; }
    public long? IdGroupReceiver { get; set; }
    public string? TypeNotify { get; set; }
    public DateTime? DtaNotify { get; set; }
    public string? Field1 { get; set; }
    public string? Field2 { get; set; }
    public string? Field3 { get; set; }
    public string? Field4 { get; set; }
    public string? Multiplicity { get; set; }
    public string? SpecializedField { get; set; }
    public string? TypeEvent { get; set; }
    public string? DomainObject { get; set; }
    public long? IdObject { get; set; }
    public long? IdSpecializedObject { get; set; }
    public DateTime? DtaEvent { get; set; }
    public string? ReadNotification { get; set; }
    public string? Notes { get; set; }
}
