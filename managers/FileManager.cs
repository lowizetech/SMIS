using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SMIS.models;
using SMIS.utilities;

/// <summary>
/// Overall, This Is The Codebase For The File Managerment.
/// This Code System Will Save The Database Using JSON
/// Default Directory Will Be /database/database.json
/// </summary>

namespace SMIS.managers
{
    public static class FileManager
    {
        private static readonly string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
        private static readonly string basePath = Path.Combine(projectRoot, "database");
        private static readonly string dbFile = Path.Combine(basePath, "database.json");

        public static async Task<List<Student>> LoadStudentsAsync()
        {
            try {
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                if (!File.Exists(dbFile))
                {
                    StaticUtil.Message("no database found. creating new one.", false, ConsoleColor.Yellow);
                    await File.WriteAllTextAsync(dbFile, "[]");
                }

                string json = await File.ReadAllTextAsync(dbFile);
                var students = JsonSerializer.Deserialize<List<Student>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return students ?? new List<Student>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error handling database: {e.Message}");
                return new List<Student>();
            }
        }

        public static async Task SaveStudentsAsync(List<Student> students)
        {
            try {
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                string json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(dbFile, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving database: {e.Message}");
            }
        }
    }
}