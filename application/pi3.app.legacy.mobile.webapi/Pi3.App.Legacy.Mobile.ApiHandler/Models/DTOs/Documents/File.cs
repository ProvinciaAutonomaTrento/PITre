// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Documents;
public class File
{
    public byte[]? Content { get; set; }
    public string? ContentType { get; set; }
    public string? EstensioneFile { get; set; }
    public string? FullName { get; set; }
    public string? OriginalFileName { get; set; }
    public int Lenght { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
}
