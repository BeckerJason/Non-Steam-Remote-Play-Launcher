using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

public class ExeConfiguration
{
    public string EXEPath { get; set; }
}

class Program
{
    static void Main()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        string configFilePath = Path.Combine(currentDir, "Configuration.json");

        ExeConfiguration config;

        if (!File.Exists(configFilePath))
        {
            // Create and save configuration
            config = new ExeConfiguration { EXEPath = "" };
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
            Console.WriteLine("Created Config file");
            return;
        }
        else
        {
            try
            {
                string json = File.ReadAllText(configFilePath);
                config = JsonSerializer.Deserialize<ExeConfiguration>(json);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Invalid JSON format in Configuration.json. Make sure there are only forward slashes in your path:");
                Console.WriteLine(ex.Message);
                return;
            }
            if (config == null||config.EXEPath=="")
            {
                Console.WriteLine("Failed to load configuration. Please check the Configuration.json file.");
                return;
            }
            Console.WriteLine("Loaded EXEPath: " + config.EXEPath);
        }

        // Start the target process
        Process firstProc = new();
        firstProc.StartInfo.FileName = config.EXEPath;
        firstProc.EnableRaisingEvents = true;
        firstProc.StartInfo.WorkingDirectory = Path.GetDirectoryName(config.EXEPath);
        firstProc.Start();
        firstProc.WaitForExit();
    }
}