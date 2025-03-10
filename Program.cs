#if WINDOWS
using System.Media;
#endif

using System;
using System.IO;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string[] wavFiles = Directory.GetFiles(currentDirectory, "*.WAV", SearchOption.TopDirectoryOnly);

        if (wavFiles.Length == 0)
        {
            Console.WriteLine("No .wav files found in the current directory.");
            return;
        }

        bool[] played = new bool[wavFiles.Length];

        while (true)
        {
            Console.WriteLine("\nFound these .wav files:");
            for (int i = 0; i < wavFiles.Length; i++)
            {
                if (played[i])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(wavFiles[i])}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(wavFiles[i])}");
                }
            }

            int maxDigit = Math.Min(wavFiles.Length, 9);
            Console.WriteLine($"\nPress [1-{maxDigit}] to play a file, or [Esc] to exit.");

            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nExiting...");
                break;
            }

            if (keyInfo.Key >= ConsoleKey.D1 && keyInfo.Key <= ConsoleKey.D9)
            {
                int chosenNumber = keyInfo.Key - ConsoleKey.D0; 
                int index = chosenNumber - 1;                 

                if (index < wavFiles.Length)
                {
                    string chosenFile = wavFiles[index];
                    Console.WriteLine($"\nPlaying: {Path.GetFileName(chosenFile)}");
                    PlayWav(chosenFile);

                    played[index] = true;
                }
                else
                {
                    Console.WriteLine($"\nInvalid selection: No file at position {chosenNumber}.");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid key. Press a digit [1-9] or [Esc] to quit.");
            }
        }
    }

    private static void PlayWav(string wavPath)
    {
        if (OperatingSystem.IsWindows())
        {
#if WINDOWS
            using var player = new SoundPlayer(wavPath);
            player.PlaySync(); 
#else
            Console.WriteLine("Windows detected, but this build was not compiled for SoundPlayer.");
#endif
        }
        else if (OperatingSystem.IsLinux())
        {
            RunProcess("aplay", $"\"{wavPath}\"");
        }
        else if (OperatingSystem.IsMacOS())
        {
            RunProcess("afplay", $"\"{wavPath}\"");
        }
        else
        {
            Console.WriteLine("Unsupported OS for audio playback in this example.");
        }
    }

    private static void RunProcess(string cmd, string args)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = cmd,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        process.WaitForExit();
    }
}
