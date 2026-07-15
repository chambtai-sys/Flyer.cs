using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flyer
{
    class Program
    {
        private static string asciiArt = @"
  ██████╗  ██╗     ██╗   ██╗███████╗██████╗       ██████╗███████╗
  ██╔══██╗ ██║     ╚██╗ ██╔╝██╔════╝██╔══██╗     ██╔════╝██╔════╝
  ██████╔╝ ██║      ╚████╔╝ █████╗  ██████╔╝     ██║     ███████╗
  ██╔═══╝  ██║       ╚██╔╝  ██╔══╝  ██╔══██╗██   ██║     ╚════██║
  ██║      ███████╗   ██║   ███████╗██║  ██║╚█████╔╝     ███████║
  ╚═╝      ╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝ ╚════╝      ╚══════╝ [BETA]
";

        static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Check for help flags or empty args
            if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
            {
                ShowHelp();
                return 0;
            }

            string? inputImagePath = null;
            string? outputMarkdownPath = null;
            string? apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            string prompt = "Convert the content in this image into a clean, well-formatted Markdown document. Retain formatting, headers, tables, lists, code blocks, and emphasize structural elements correctly. Output only the Markdown itself, without markdown code block wrappers (do not wrap in ```markdown ... ``` unless necessary, but if you do, keep it clean).";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-i" || args[i] == "--input")
                {
                    if (i + 1 < args.Length) inputImagePath = args[++i];
                }
                else if (args[i] == "-o" || args[i] == "--output")
                {
                    if (i + 1 < args.Length) outputMarkdownPath = args[++i];
                }
                else if (args[i] == "-k" || args[i] == "--key")
                {
                    if (i + 1 < args.Length) apiKey = args[++i];
                }
                else if (args[i] == "-p" || args[i] == "--prompt")
                {
                    if (i + 1 < args.Length) prompt = args[++i];
                }
                else if (args[i] == "-v" || args[i] == "--version")
                {
                    Console.WriteLine(asciiArt);
                    Console.WriteLine("Flyer.cs - Version 1.0.0-beta");
                    return 0;
                }
            }

            if (string.IsNullOrEmpty(inputImagePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error: Input image path is required. Use -i or --input.");
                Console.ResetColor();
                ShowHelp();
                return 1;
            }

            if (!File.Exists(inputImagePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: Input file '{inputImagePath}' does not exist.");
                Console.ResetColor();
                return 1;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error: Gemini API Key is required. Set GEMINI_API_KEY environment variable or use -k/--key flag.");
                Console.ResetColor();
                return 1;
            }

            Console.WriteLine(asciiArt);
            Console.WriteLine($"Starting Flyer.cs conversion...");
            Console.WriteLine($"Input Image:  {inputImagePath}");
            Console.WriteLine($"Output File:  {(string.IsNullOrEmpty(outputMarkdownPath) ? "Console Output" : outputMarkdownPath)}");

            try
            {
                await ConvertImageToMarkdown(inputImagePath, outputMarkdownPath, apiKey, prompt);
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error during conversion: {ex.Message}");
                Console.ResetColor();
                return 2;
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine(asciiArt);
            Console.WriteLine("Flyer.cs - An advanced C#-based CLI to convert Images to structured Markdown texts using the Gemini API.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  Flyer.cs -i <input_image_path> [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -i, --input <path>     Path to the input image file (JPEG, PNG, WEBP, HEIC, etc.) [Required]");
            Console.WriteLine("  -o, --output <path>    Path to save the generated markdown file. If not specified, outputs to console.");
            Console.WriteLine("  -k, --key <key>        Gemini API Key. Can also be set via GEMINI_API_KEY environment variable.");
            Console.WriteLine("  -p, --prompt <text>    Custom prompt instruction for Gemini (Default extracts layout and structured text to MD).");
            Console.WriteLine("  -h, --help             Show this help information.");
            Console.WriteLine("  -v, --version          Show version and beta logo.");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  Flyer.cs -i flyer.png -o output.md -k YOUR_GEMINI_API_KEY");
            Console.WriteLine("  Flyer.cs -i whiteboard.jpg --prompt \"Extract only the action items from this meeting whiteboard.\"");
        }

        static async Task ConvertImageToMarkdown(string inputImagePath, string? outputMarkdownPath, string apiKey, string prompt)
        {
            byte[] imageBytes = await File.ReadAllBytesAsync(inputImagePath);
            string base64Image = Convert.ToBase64String(imageBytes);
            string mimeType = GetMimeType(inputImagePath);

            // Construct Gemini payload JSON
            // We use the modern standard endpoint structure for gemini-2.5-flash
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = mimeType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(payload);

            using (var httpClient = new HttpClient())
            {
                // Set timeout to 2 minutes in case of large images
                httpClient.Timeout = TimeSpan.FromMinutes(2);

                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                Console.WriteLine("Analyzing image content via Gemini API...");
                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Gemini API returned status code {response.StatusCode}. Response: {errorContent}");
                }

                string responseString = await response.Content.ReadAsStringAsync();

                string markdownOutput = ParseGeminiResponse(responseString);

                if (string.IsNullOrEmpty(outputMarkdownPath))
                {
                    Console.WriteLine("\n--- BEGIN MARKDOWN OUTPUT ---");
                    Console.WriteLine(markdownOutput);
                    Console.WriteLine("--- END MARKDOWN OUTPUT ---\n");
                }
                else
                {
                    string? directory = Path.GetDirectoryName(outputMarkdownPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    await File.WriteAllTextAsync(outputMarkdownPath, markdownOutput, Encoding.UTF8);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nSuccess! Markdown successfully saved to: {outputMarkdownPath}");
                    Console.ResetColor();
                }
            }
        }

        private static string GetMimeType(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                ".heic" => "image/heic",
                ".heif" => "image/heif",
                _ => "image/jpeg" // Fallback default
            };
        }

        private static string ParseGeminiResponse(string jsonResponse)
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("candidates", out JsonElement candidates) && candidates.GetArrayLength() > 0)
                {
                    JsonElement firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out JsonElement content))
                    {
                        if (content.TryGetProperty("parts", out JsonElement parts) && parts.GetArrayLength() > 0)
                        {
                            JsonElement firstPart = parts[0];
                            if (firstPart.TryGetProperty("text", out JsonElement textProperty))
                            {
                                string textValue = textProperty.GetString() ?? "";
                                return CleanMarkdownWrappers(textValue);
                            }
                        }
                    }
                }
            }
            throw new Exception("Unable to extract text content from the Gemini API response. JSON structure may be unexpected.");
        }

        private static string CleanMarkdownWrappers(string text)
        {
            // Remove markdown code block wrapping like ```markdown and ``` if the model enclosed the whole block in it
            string trimmed = text.Trim();
            if (trimmed.StartsWith("```markdown", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring("```markdown".Length);
                if (trimmed.EndsWith("```"))
                {
                    trimmed = trimmed.Substring(0, trimmed.Length - 3);
                }
            }
            else if (trimmed.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring("```".Length);
                if (trimmed.EndsWith("```"))
                {
                    trimmed = trimmed.Substring(0, trimmed.Length - 3);
                }
            }
            return trimmed.Trim('\r', '\n');
        }
    }
}
