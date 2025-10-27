// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models;
public class File
{
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? FullName { get; set; }
    public string? OriginalFileName { get; set; }
    public byte[]? Content { get; set; }
    public int Length { get; set; }
    public string? ContentType { get; set; }
    public string? EstensioneFile { get; set; }
}
