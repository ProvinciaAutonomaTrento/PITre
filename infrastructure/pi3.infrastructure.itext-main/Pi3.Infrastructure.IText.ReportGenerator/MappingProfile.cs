// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using iText.Layout.Properties;
using Pi3.Core.Services.File.ReportGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.IText.ReportGenerator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Justifications, TextAlignment>()
                .ConvertUsing(justification => ConvertJustification(justification));

            CreateMap<VerticalAlignments, VerticalAlignment>()
                .ConvertUsing(verticalAlignment => ConvertVerticalAlignment(verticalAlignment));
        }

        private TextAlignment ConvertJustification(Justifications justification)
        {
            return justification switch
            {
                Justifications.Left => TextAlignment.LEFT,
                Justifications.Center => TextAlignment.CENTER,
                Justifications.Right => TextAlignment.RIGHT,
                _ => TextAlignment.LEFT
            };
        }

        private VerticalAlignment ConvertVerticalAlignment(VerticalAlignments verticalAlignment)
        {
            return verticalAlignment switch
            {
                VerticalAlignments.Top => VerticalAlignment.TOP,
                VerticalAlignments.Center => VerticalAlignment.MIDDLE,
                VerticalAlignments.Bottom => VerticalAlignment.BOTTOM,
                _ => VerticalAlignment.TOP
            };
        }
    }
}
