# Modern Scanner Wrapper

A modern .NET 8 command-line wrapper for NAPS2 that provides convenient scanning capabilities with named parameters and automatic device detection.

## What it does

This tool provides a streamlined command-line interface for document scanning using NAPS2 as the backend. It automatically detects available scanners, supports both WIA and TWAIN drivers, and can save scans in multiple formats (PNG, JPEG, TIFF, BMP, PDF) with customizable resolution and color settings.

### Key Features

- ✅ **Automatic scanner detection** - No need to remember device names
- ✅ **Multiple output formats** - PNG, JPEG, TIFF, BMP, PDF
- ✅ **Flexible DPI settings** - From 75 to 1200+ DPI
- ✅ **Color mode options** - Color, grayscale, black & white
- ✅ **ADF and flatbed support** - Works with document feeders and glass beds
- ✅ **Batch scanning** - Multiple pages automatically numbered
- ✅ **Cross-platform** - Runs on Windows, macOS, and Linux
- ✅ **Modern .NET 8** - Fast startup and excellent performance

## Prerequisites

### 1. .NET 8 Runtime
Download and install the .NET 8 runtime from:
- **Windows/macOS/Linux**: https://dotnet.microsoft.com/download/dotnet/8.0

Verify installation:
```bash
dotnet --version
# Should show 8.0.x or higher
```

### 2. NAPS2 Scanner Software
Download and install NAPS2 from:
- **Official website**: https://www.naps2.com/download
- **Recommended version**: 8.1.4 or newer

#### Installation Steps:
1. Download `naps2-8.1.4-win-x64.exe` (or appropriate version for your OS)
2. Run the installer with default settings
3. **Important**: Make sure NAPS2 is added to your system PATH, or manually add it:

**Windows PATH setup:**
```bash
# Add to PATH environment variable:
C:\Program Files\NAPS2
```

**Verify NAPS2 installation:**
```bash
NAPS2.Console --version
# Should show version information
```

### 3. Scanner Drivers
Ensure your scanner has proper drivers installed:
- **WIA drivers** (Windows Image Acquisition) - Usually installed automatically
- **TWAIN drivers** (optional) - Often more reliable for business scanners
- **Manufacturer software** - Install scanner-specific software for best compatibility

## Installation

1. **Clone or download** this repository
2. **Build the project**:
   ```bash
   dotnet build --configuration Release
   ```
3. **Run from output directory**:
   ```bash
   cd bin/Release/net8.0
   ./ModernScanner
   ```

**Or install globally** (optional):
```bash
dotnet pack
dotnet tool install --global --add-source ./nupkg ModernScanner
```

## Usage

### Basic Syntax
```bash
scanner [options]
```

### Command Line Options

| Option | Short | Description | Default |
|--------|-------|-------------|---------|
| `--output <folder>` | `-o` | Output folder path | `scanned_pages` |
| `--prefix <name>` | `-p` | File name prefix | `page` |
| `--format <ext>` | `-f` | Output format (png, jpg, tiff, bmp, pdf) | `png` |
| `--dpi <number>` | `-d` | Scanning resolution | `300` |
| `--color <mode>` | `-c` | Color mode (color, gray, bw) | `color` |
| `--source <src>` | `-s` | Paper source (feeder, glass) | `feeder` |
| `--device <name>` | | Specific scanner device name | auto-detect |
| `--driver <type>` | | Scanner driver (wia, twain) | `wia` |
| `--help` | `-h` | Show help information | |

### Output Formats

- **PNG** - Best quality, lossless compression, moderate file size
- **JPEG** - Good quality, lossy compression, smallest file size
- **TIFF** - Professional quality, optional compression, large file size
- **BMP** - Uncompressed, largest file size, maximum quality
- **PDF** - Multi-page document format, good for archiving

### Color Modes

- **color** - Full color scanning (24-bit RGB)
- **gray** - Grayscale scanning (8-bit)
- **bw** - Black and white scanning (1-bit)

### DPI Recommendations

- **75-150 DPI** - Web use, email attachments
- **300 DPI** - Standard document scanning, printing
- **600 DPI** - High quality documents, small text
- **1200+ DPI** - Professional archival, photography

## Examples

### Basic Examples

**Simple scan with defaults:**
```bash
scanner
# Output: scanned_pages/page_0001.png (300 DPI, color, ADF)
```

**Quick help:**
```bash
scanner --help
```

### Custom Output Examples

**Scan to specific folder:**
```bash
scanner --output "D:\Documents\Scans"
# Output: D:\Documents\Scans\page_0001.png
```

**Custom file prefix:**
```bash
scanner --prefix "invoice" --output "C:\Invoices"
# Output: C:\Invoices\invoice_0001.png, invoice_0002.png, etc.
```

**Different file format:**
```bash
scanner --format jpg --output "D:\Photos"
# Output: D:\Photos\page_0001.jpg
```

### Quality and Format Examples

**High-resolution color scanning:**
```bash
scanner --dpi 600 --color color --format tiff --prefix "document_hq"
# Output: scanned_pages/document_hq_0001.tiff (600 DPI color TIFF)
```

**Web-optimized scanning:**
```bash
scanner --dpi 150 --format jpg --color gray --prefix "web"
# Output: scanned_pages/web_0001.jpg (150 DPI grayscale JPEG)
```

**Archive quality scanning:**
```bash
scanner --dpi 1200 --format tiff --color color --prefix "archive"
# Output: scanned_pages/archive_0001.tiff (1200 DPI color TIFF)
```

**Black and white documents:**
```bash
scanner --color bw --dpi 300 --format pdf --prefix "contract"
# Output: scanned_pages/contract.pdf (300 DPI B&W PDF)
```

### Source Examples

**Scan from document feeder (ADF):**
```bash
scanner --source feeder --format pdf --prefix "multi_page"
# Scans all pages from ADF into one PDF
```

**Scan from flatbed glass:**
```bash
scanner --source glass --prefix "photo" --dpi 600 --format png
# Single page scan from flatbed
```

### Driver Examples

**Use WIA driver (default):**
```bash
scanner --driver wia --dpi 300
```

**Use TWAIN driver (often better for business scanners):**
```bash
scanner --driver twain --format pdf --prefix "report"
```

**Auto-detect best driver:**
```bash
scanner --format png
# Tries WIA first, falls back to TWAIN if needed
```

### Specific Device Examples

**Scan with specific scanner:**
```bash
scanner --device "Xerox WIA - ETE84DEC0F2B58" --source feeder
```

**HP scanner example:**
```bash
scanner --device "HP LaserJet MFP M234dwe" --driver wia --format pdf
```

**Canon scanner with TWAIN:**
```bash
scanner --device "Canon PIXMA" --driver twain --dpi 600 --color color
```

### Batch Processing Examples

**Daily document scanning:**
```bash
scanner --prefix "daily_$(date +%Y%m%d)" --format pdf --dpi 300
# Output: scanned_pages/daily_20241201.pdf
```

**Invoice processing:**
```bash
scanner --output "C:\Accounting\Invoices" --prefix "inv" --format jpg --color gray --dpi 300
# Output: C:\Accounting\Invoices\inv_0001.jpg, inv_0002.jpg, etc.
```

**High-volume scanning:**
```bash
scanner --source feeder --format pdf --prefix "batch" --dpi 200
# Fast scanning for large document batches
```

### Workflow Examples

**Legal document archiving:**
```bash
scanner --output "D:\Legal\Cases\2024" --prefix "case_001" --format tiff --dpi 600 --color gray
```

**Receipt scanning for accounting:**
```bash
scanner --output "C:\Receipts\$(date +%Y-%m)" --prefix "receipt" --format jpg --dpi 300 --color color
```

**Book digitization:**
```bash
scanner --source glass --prefix "book_chapter" --format png --dpi 400 --color color
```

**Medical records:**
```bash
scanner --output "D:\Medical\Patient_Files" --prefix "patient_123" --format pdf --dpi 300 --color gray
```

## Troubleshooting

### Scanner Not Found
```bash
# List available scanners
NAPS2.Console --driver wia --listdevices
NAPS2.Console --driver twain --listdevices

# Try different driver
scanner --driver twain
```

### NAPS2 Not Found Error
1. Verify NAPS2 installation: `NAPS2.Console --version`
2. Add NAPS2 to PATH environment variable
3. Reinstall NAPS2 with default settings

### Permission Issues
- Run terminal as Administrator (Windows)
- Use `sudo` if needed (Linux/macOS)
- Check folder write permissions

### ADF/Feeder Issues
```bash
# Try flatbed instead
scanner --source glass

# Check for paper jams
# Ensure paper is properly loaded
# Try TWAIN driver for better ADF support
scanner --driver twain --source feeder
```

### Poor Scan Quality
```bash
# Increase DPI
scanner --dpi 600

# Use uncompressed format
scanner --format tiff

# Clean scanner glass/ADF
```

## Advanced Usage

### Environment Variables
Set default values using environment variables:
```bash
export SCANNER_OUTPUT_DIR="/home/user/scans"
export SCANNER_DEFAULT_DPI="600"
export SCANNER_DEFAULT_FORMAT="pdf"
```

### Batch Scripts

**Windows batch file example:**
```batch
@echo off
echo Scanning invoices...
scanner --output "C:\Invoices\%DATE%" --prefix "inv" --format pdf --dpi 300
echo Done!
pause
```

**Linux/macOS shell script:**
```bash
#!/bin/bash
echo "Daily scan routine..."
scanner --output "/home/user/scans/$(date +%Y-%m-%d)" --prefix "daily" --format pdf --dpi 300
echo "Scan completed!"
```

### Integration with Other Tools

**Scan and OCR:**
```bash
scanner --format tiff --dpi 300 --prefix "ocr_input"
# Then use Tesseract or other OCR tools
```

**Scan and upload to cloud:**
```bash
scanner --format pdf --prefix "upload" && rsync scanned_pages/*.pdf user@server:/documents/
```

## Technical Notes

- **Async operations** - Non-blocking I/O for better performance
- **Automatic retry** - Handles temporary scanner busy states
- **Memory efficient** - Streams large files without loading into memory
- **Cross-platform** - Works on Windows, macOS, and Linux with appropriate drivers
- **Null-safe** - Modern C# nullable reference types prevent common errors

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

- **Issues**: Report bugs and feature requests on GitHub
- **NAPS2 Support**: Visit https://www.naps2.com for NAPS2-specific issues
- **Scanner Drivers**: Contact your scanner manufacturer for driver support#
