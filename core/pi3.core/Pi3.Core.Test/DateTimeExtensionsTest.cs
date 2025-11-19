// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    public class DateTimeExtensionsTest
    {
        [Test]
        public async Task AsDateFormatTest()
        {
            var asString = DateTime.Now.AsDateFormat();
            asString = DateTime.Now.AsDateTimeFormat();
            asString = DateTime.Now.AsHoursMinutesSecondsFormat();

            Assert.True(true);
        }
    }
}
