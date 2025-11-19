// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.Test
{
    internal static class TestRunner
    {
        public static async Task Run(Func<Task> testAction, [CallerMemberName] string testName = "")
        {
            try
            {
                await testAction();
                Console.WriteLine($"✅ {testName}: OK");
            }
            catch (AssertionException)
            {
                Console.WriteLine($"❌ {testName}: KO (assert fallito)");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 {testName}: FAILED - Eccezione: {ex.Message}");
                throw;
            }
        }
    }
}
