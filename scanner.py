#!/usr/bin/env python3
"""
Modern Scanner Wrapper for NAPS2
Python implementation with identical functionality to the C# version
"""

import argparse
import asyncio
import os
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Optional, List


async def main():
    """Main entry point"""
    parser = create_argument_parser()
    args = parser.parse_args()
    
    if args.help:
        parser.print_help()
        return
    
    print("=== MODERN SCANNER WRAPPER ===")
    print(f"Output: {args.output}")
    print(f"Prefix: {args.prefix}")
    print(f"Format: {args.format}")
    print(f"DPI: {args.dpi}")
    print(f"Color: {args.color}")
    print(f"Source: {args.source}")
    print(f"Device: {args.device or 'auto-detect'}")
    print(f"Driver: {args.driver}")
    print()
    
    # Execute scan
    success = await scan_with_naps2(
        args.output, args.prefix, args.format, args.dpi,
        args.color, args.source, args.device, args.driver
    )
    
    print("✓ Scan completed successfully!" if success else "✗ Scan failed.")
    print("\nPress Enter to exit...")
    input()


def create_argument_parser() -> argparse.ArgumentParser:
    """Creates and configures the argument parser"""
    parser = argparse.ArgumentParser(
        description="Modern Scanner Wrapper for NAPS2",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
EXAMPLES:
  python scanner.py
    Basic scan with auto-detection

  python scanner.py -o "D:\\scans" -p "doc" -f jpg -d 600 -c gray
    Custom folder, prefix, format, DPI and color

  python scanner.py --device "Xerox WIA - ETE84DEC0F2B58" --source feeder
    Specific device and source

  python scanner.py --driver twain --format pdf --dpi 300
    Use TWAIN driver, save as PDF
        """
    )
    
    parser.add_argument(
        "-o", "--output",
        default="scanned_pages",
        help="Output folder (default: scanned_pages)"
    )
    
    parser.add_argument(
        "-p", "--prefix",
        default="page",
        help="File name prefix (default: page)"
    )
    
    parser.add_argument(
        "-f", "--format",
        default="png",
        choices=["png", "jpg", "jpeg", "tiff", "bmp", "pdf"],
        help="Output format (default: png)"
    )
    
    parser.add_argument(
        "-d", "--dpi",
        type=int,
        default=300,
        help="DPI resolution (default: 300)"
    )
    
    parser.add_argument(
        "-c", "--color",
        default="color",
        choices=["color", "gray", "bw"],
        help="Color mode (default: color)"
    )
    
    parser.add_argument(
        "-s", "--source",
        default="feeder",
        choices=["feeder", "glass"],
        help="Paper source (default: feeder)"
    )
    
    parser.add_argument(
        "--device",
        help="Scanner device name (auto-detect if not specified)"
    )
    
    parser.add_argument(
        "--driver",
        default="wia",
        choices=["wia", "twain"],
        help="Scanner driver (default: wia)"
    )
    
    return parser


async def scan_with_naps2(
    output_folder: str,
    file_prefix: str,
    file_format: str,
    dpi: int,
    color_mode: str,
    source: str,
    device_name: Optional[str],
    driver: str
) -> bool:
    """
    Executes scan using NAPS2
    
    Args:
        output_folder: Destination folder
        file_prefix: File name prefix
        file_format: File format (png, jpg, tiff, bmp, pdf)
        dpi: DPI resolution
        color_mode: Color mode (color, gray, bw)
        source: Source (feeder, glass)
        device_name: Scanner device name (None for auto-detect)
        driver: Driver type (wia, twain)
    
    Returns:
        True if scan succeeded
    """
    try:
        # Create output folder if it doesn't exist
        Path(output_folder).mkdir(parents=True, exist_ok=True)
        if not Path(output_folder).exists():
            print(f"Created folder: {output_folder}")
        
        # Check if NAPS2 is available in PATH
        if not await is_naps2_available():
            print("ERROR: NAPS2.Console not found in system PATH.")
            print("Install NAPS2 from: https://www.naps2.com/download")
            print("Or add NAPS2 installation folder to PATH environment variable.")
            return False
        
        # Find scanner device if not specified
        if device_name is None:
            device_name = await get_scanner_device(driver)
            if not device_name:
                print("ERROR: No scanner found.")
                return False
        
        print(f"Scanner: {device_name}")
        
        # Build output path
        if file_format.lower() == "pdf":
            output_path = Path(output_folder) / f"{file_prefix}.{file_format}"
        else:
            output_path = Path(output_folder) / f"{file_prefix}_$(nnnn).{file_format}"
        
        # Build NAPS2 command arguments
        arguments = [
            "NAPS2.Console",
            "--driver", driver,
            "--device", device_name,
            "--source", source,
            "--dpi", str(dpi),
            "--bitdepth", color_mode,
            "--output", str(output_path),
            "--verbose"
        ]
        
        print()
        print("NAPS2 Command:")
        print(" ".join(arguments))
        print()
        print("Starting scan...")
        print("MAKE SURE PAPER IS IN THE ADF TRAY!")
        print()
        
        # Execute NAPS2
        process = await asyncio.create_subprocess_exec(
            *arguments,
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE
        )
        
        # Handle output in real-time
        async def read_output(stream, prefix):
            while True:
                line = await stream.readline()
                if not line:
                    break
                print(f"{prefix}: {line.decode().strip()}")
        
        # Start reading output streams
        await asyncio.gather(
            read_output(process.stdout, "NAPS2"),
            read_output(process.stderr, "ERROR")
        )
        
        # Wait for process to complete
        exit_code = await process.wait()
        
        print()
        print(f"NAPS2 finished with exit code: {exit_code}")
        
        # Check if files were created (more reliable than exit code)
        has_files = check_for_scanned_files(output_folder, file_prefix, file_format)
        
        if has_files or exit_code == 0:
            show_scan_results(output_folder, file_prefix, file_format)
            return True
        
        print("No files were created.")
        return False
        
    except Exception as e:
        print(f"Error during scan: {e}")
        return False


async def is_naps2_available() -> bool:
    """
    Checks if NAPS2.Console is available in system PATH
    
    Returns:
        True if NAPS2 is available
    """
    try:
        process = await asyncio.create_subprocess_exec(
            "NAPS2.Console", "--version",
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE
        )
        exit_code = await process.wait()
        return exit_code == 0
    except (FileNotFoundError, OSError):
        return False


async def get_scanner_device(driver: str) -> Optional[str]:
    """
    Gets available scanner device
    
    Args:
        driver: Driver type (wia, twain)
    
    Returns:
        Scanner device name or None if not found
    """
    try:
        process = await asyncio.create_subprocess_exec(
            "NAPS2.Console", "--driver", driver, "--listdevices",
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE
        )
        
        stdout, stderr = await process.communicate()
        exit_code = await process.wait()
        
        if exit_code == 0 and stdout:
            output = stdout.decode().strip()
            if output:
                return output.split('\n')[0].strip()
                
    except Exception as e:
        print(f"Error finding scanner: {e}")
    
    return None


def check_for_scanned_files(output_folder: str, file_prefix: str, file_format: str) -> bool:
    """
    Checks if scanned files exist in the output folder
    
    Args:
        output_folder: Output folder path
        file_prefix: File prefix to search for
        file_format: File format extension
    
    Returns:
        True if files were found
    """
    try:
        folder_path = Path(output_folder)
        if not folder_path.exists():
            return False
        
        search_pattern = f"{file_prefix}*.{file_format}"
        files = list(folder_path.glob(search_pattern))
        
        return len(files) > 0
        
    except Exception:
        return False


def show_scan_results(output_folder: str, file_prefix: str, file_format: str) -> None:
    """
    Shows scan results summary
    
    Args:
        output_folder: Output folder path
        file_prefix: File prefix
        file_format: File format
    """
    try:
        folder_path = Path(output_folder)
        search_pattern = f"{file_prefix}*.{file_format}"
        files = list(folder_path.glob(search_pattern))
        
        print(f"\nFiles created ({len(files)}):")
        for file_path in sorted(files):
            size_kb = file_path.stat().st_size // 1024
            print(f"  {file_path.name} ({size_kb:,} KB)")
        
        if files:
            print(f"\nAll files saved to: {folder_path.absolute()}")
            
    except Exception as e:
        print(f"Error showing results: {e}")


if __name__ == "__main__":
    # Handle KeyboardInterrupt gracefully
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("\n\nScan interrupted by user.")
        sys.exit(1)
    except Exception as e:
        print(f"\nUnexpected error: {e}")
        sys.exit(1)