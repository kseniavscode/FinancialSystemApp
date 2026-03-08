using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using LR1.Models;
using System.IO;

namespace LR1.Services
{
    public class DataStorage
    {
        private const string FileName = "data.json";
        public List<User> Users { get; set; } = new List<User>();

        public void Save()
        {
            
            var options = new JsonSerializerOptions { WriteIndented = true};
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(FileName, json);
        }
        public static DataStorage Load()
        {
            if (!File.Exists(FileName))
            {
                return new DataStorage();
            }

            try
            {
                string json = File.ReadAllText(FileName);
                return JsonSerializer.Deserialize<DataStorage>(json) ?? new DataStorage();
            }
            catch
            {
                return new DataStorage();
            }

        }
    }
}
