# Flyer.cs (BETA)

```text
  ██████╗  ██╗     ██╗   ██╗███████╗██████╗       ██████╗███████╗
  ██╔══██╗ ██║     ╚██╗ ██╔╝██╔════╝██╔══██╗     ██╔════╝██╔════╝
  ██████╔╝ ██║      ╚████╔╝ █████╗  ██████╔╝     ██║     ███████╗
  ██╔═══╝  ██║       ╚██╔╝  ██╔══╝  ██╔══██╗██   ██║     ╚════██║
  ██║      ███████╗   ██║   ███████╗██║  ██║╚█████╔╝     ███████║
  ╚═╝      ╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝ ╚════╝      ╚══════╝ [BETA]
```

**Flyer.cs** is a high-performance, advanced, C#-based command-line tool (CLI) designed to effortlessly convert images into beautifully formatted, structured **Markdown** texts. Powered by Google's cutting-edge `gemini-2.5-flash` model, Flyer.cs parses layouts, headings, paragraphs, code-blocks, lists, and tables within any image (such as infographics, screenshots, whiteboards, or scanned documents) and yields pixel-perfect Markdown directly in your terminal or inside an output file.

---

## 🚀 Key Features

- **⚡ Blazing Fast Conversions**: Uses the latest Gemini Multimodal APIs to perform instant processing.
- **📁 Output Flexibility**: Render the Markdown output straight to your Terminal/Console or write directly into any target text file.
- **🎨 Structurally Intelligent**: Correctly handles formatting, headers, list structures, tables, and emphasizes critical design flows.
- **⚙️ Customizable Prompts**: Tailor extraction tasks by injecting custom instructions (e.g. *"Only extract code snippets"* or *"Focus on tables"*).
- **📦 Wide Asset Format Support**: Seamlessly processes JPEG, PNG, WEBP, GIF, HEIC, and HEIF.

---

## 🛠️ Prerequisites & Installation

### 1. Ensure you have the .NET SDK
Flyer.cs is built with modern .NET. Ensure you have the **.NET SDK** installed.
- [Download .NET SDK](https://dotnet.microsoft.com/download)

### 2. Get a Gemini API Key
To execute conversions, you need an API key from Google AI Studio.
- [Get your Gemini API Key](https://aistudio.google.com/)

Set your API Key as an environment variable (recommended):
```bash
# On Linux/macOS
export GEMINI_API_KEY="your-api-key-here"

# On Windows (Command Prompt)
set GEMINI_API_KEY="your-api-key-here"

# On Windows (PowerShell)
$env:GEMINI_API_KEY="your-api-key-here"
```

---

## 💻 Building and Running

Clone or download this repository, navigate to the folder, and build or run the program.

### Build the project
```bash
dotnet build
```

### Run using `dotnet run`
```bash
# Display help and usage instructions
dotnet run -- --help

# Display version with ASCII beta logo
dotnet run -- --version
```

---

## 📖 Command-Line Usage

```text
Usage:
  Flyer.cs -i <input_image_path> [options]

Options:
  -i, --input <path>     Path to the input image file (JPEG, PNG, WEBP, HEIC, etc.) [Required]
  -o, --output <path>    Path to save the generated markdown file. If not specified, outputs to console.
  -k, --key <key>        Gemini API Key. Can also be set via GEMINI_API_KEY environment variable.
  -p, --prompt <text>    Custom prompt instruction for Gemini (Default extracts layout and structured text to MD).
  -h, --help             Show this help information.
  -v, --version          Show version and beta logo.
```

### Examples

**Convert an image and print output to the Console:**
```bash
dotnet run -- -i sample_flyer.png -k YOUR_GEMINI_API_KEY
```

**Convert and save output to a file (using environment variable):**
```bash
dotnet run -- -i document.jpg -o document.md
```

**Use a custom prompt to only extract lists:**
```bash
dotnet run -- -i whiteboard.png -o actions.md --prompt "Find any lists of items or action plans and represent them as structured markdown checklists."
```

---

## 📄 License

This project is licensed under the **MIT License**. Check out the [LICENSE](LICENSE) file for more information.
