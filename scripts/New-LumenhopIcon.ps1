$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$outDir = Join-Path $PSScriptRoot "..\src\Lumenhop\Assets"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function New-OrbBitmap([int]$size) {
    $bmp = New-Object System.Drawing.Bitmap $size, $size
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::FromArgb(0, 0, 0, 0))

    $pad = [Math]::Max(1, [int]($size * 0.06))
    $bg = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 12, 18, 24))
    $g.FillEllipse($bg, $pad, $pad, $size - 2 * $pad, $size - 2 * $pad)
    $bg.Dispose()

    $rings = @(
        @{ Scale = 0.78; A = 40 },
        @{ Scale = 0.56; A = 70 },
        @{ Scale = 0.34; A = 120 }
    )
    foreach ($ring in $rings) {
        $dim = [int]($size * $ring.Scale)
        $x = [int](($size - $dim) / 2)
        $pen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb($ring.A, 46, 230, 199)), [Math]::Max(1, $size / 28)
        $g.DrawEllipse($pen, $x, $x, $dim, $dim)
        $pen.Dispose()
    }

    $core = [int]($size * 0.22)
    $cx = [int](($size - $core) / 2)
    $glow = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(180, 46, 230, 199))
    $g.FillEllipse($glow, $cx - 1, $cx - 1, $core + 2, $core + 2)
    $glow.Dispose()
    $orb = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 46, 230, 199))
    $g.FillEllipse($orb, $cx, $cx, $core, $core)
    $orb.Dispose()
    $g.Dispose()
    return $bmp
}

function Save-Ico($path, $bitmaps) {
    $stream = [System.IO.File]::Open($path, [System.IO.FileMode]::Create)
    $writer = New-Object System.IO.BinaryWriter $stream
    $writer.Write([uint16]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]$bitmaps.Count)

    $pngs = @()
    foreach ($bmp in $bitmaps) {
        $ms = New-Object System.IO.MemoryStream
        $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
        $pngs += , $ms.ToArray()
        $ms.Dispose()
    }

    $offset = 6 + (16 * $bitmaps.Count)
    for ($i = 0; $i -lt $bitmaps.Count; $i++) {
        $w = $bitmaps[$i].Width
        $writer.Write([byte]($(if ($w -ge 256) { 0 } else { $w })))
        $writer.Write([byte]($(if ($w -ge 256) { 0 } else { $w })))
        $writer.Write([byte]0)
        $writer.Write([byte]0)
        $writer.Write([uint16]1)
        $writer.Write([uint16]32)
        $writer.Write([int32]$pngs[$i].Length)
        $writer.Write([int32]$offset)
        $offset += $pngs[$i].Length
    }

    foreach ($png in $pngs) { $writer.Write($png) }
    $writer.Flush()
    $stream.Dispose()
}

$sizes = 16, 32, 48, 256
$bitmaps = @()
foreach ($size in $sizes) { $bitmaps += New-OrbBitmap $size }
$ico = Join-Path $outDir "Lumenhop.ico"
Save-Ico $ico $bitmaps
foreach ($bmp in $bitmaps) { $bmp.Dispose() }
Write-Output "Wrote $ico"
