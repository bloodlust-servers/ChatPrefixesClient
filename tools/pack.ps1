<#
    Builds the client mod and packs a Thunderstore-ready zip.
    Run from the project root:
        powershell -ExecutionPolicy Bypass -File tools\pack.ps1
#>
param([string]$Configuration = "Release")

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet build -c $Configuration --nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    $ver = (Get-Content "manifest.json" -Raw | ConvertFrom-Json).version_number
    New-Item -ItemType Directory -Force -Path "dist" | Out-Null
    $out = Join-Path $root ("dist\ChatPrefixesClient-" + $ver + ".zip")
    Remove-Item -Force $out -ErrorAction SilentlyContinue

    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::Open($out, [System.IO.Compression.ZipArchiveMode]::Create)
    try {
        $files = [ordered]@{
            ("bin/" + $Configuration + "/ChatPrefixesClient.dll") = "plugins/ChatPrefixesClient.dll"
            "manifest.json"                                       = "manifest.json"
            "README.md"                                           = "README.md"
            "CHANGELOG.md"                                        = "CHANGELOG.md"
            "icon.png"                                            = "icon.png"
        }
        foreach ($src in $files.Keys) {
            [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, (Join-Path $root $src), $files[$src]) | Out-Null
            Write-Host ("  + " + $files[$src]) -ForegroundColor DarkGray
        }
    }
    finally { $zip.Dispose() }

    Write-Host ("Done: " + $out) -ForegroundColor Green
}
finally { Pop-Location }
