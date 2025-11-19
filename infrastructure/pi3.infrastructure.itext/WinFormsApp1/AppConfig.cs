// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class AppConfig
    {
        //public string ServerUrl { get; set; } = "https://pitre-test.cloud-test-intra.tndigit.it/Uploader/signatureHub";
        public string ServerUrl { get; set; } = "https://localhost:44390";
        public string UserName { get; set; } = "User Name";
        public string UserEmail { get; set; } = "user@example.com";
        public string UserRole { get; set; } = "Signer";
        public string CustomData { get; set; } = "";
        public bool StartMinimized { get; set; } = true;

        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SignatureClient",
            "config.json");

        public static AppConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    return config ?? new AppConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }

            return new AppConfig();
        }

        public void Save()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }
    }
}
