Add-Type -AssemblyName System.Drawing

$assets = Join-Path $PSScriptRoot "..\src\Lumenhop\Assets"
New-Item -ItemType Directory -Force -Path $assets | Out-Null

function New-Mark([int]$size) {
    $bmp = New-Object System.Drawing.Bitmap $size, $size
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::FromArgb(0, 0, 0, 0))

    $pad = [int]($size * 0.06)
    $rect = New-Object System.Drawing.Rectangle $pad, $pad, ($size - 2 * $pad), ($size - 2 * $pad)
    $radius = [int]($size * 0.22)
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $radius * 2
    $path.AddArc($rect.X, $rect.Y, $d, $d, 180, 90)
    $path.AddArc($rect.Right - $d, $rect.Y, $d, $d, 270, 90)
    $path.AddArc($rect.Right - $d, $rect.Bottom - $d, $d, $d, 0, 90)
    $path.AddArc($rect.X, $rect.Bottom - $d, $d, $d, 90, 90)
    $path.CloseFigure()

    $bg = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 12, 18, 24))
    $g.FillPath($bg, $path)

    $cx = $size / 2
    $cy = $size / 2
    $glow = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(70, 46, 230, 199))
    $g.FillEllipse($glow, $cx - $size * 0.28, $cy - $size * 0.28, $size * 0.56, $size * 0.56)

    $penWidth = [Math]::Max(1.0, $size / 32.0)
    $ringColor = [System.Drawing.Color]::FromArgb(160, 46, 230, 199)
    $ring = New-Object System.Drawing.Pen -ArgumentList @($ringColor, $penWidth)
    $g.DrawEllipse($ring, $cx - $size * 0.22, $cy - $size * 0.22, $size * 0.44, $size * 0.44)

    $core = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 46, 230, 199))
    $g.FillEllipse($core, $cx - $size * 0.09, $cy - $size * 0.09, $size * 0.18, $size * 0.18)

    $g.Dispose()
    $path.Dispose()
    $bg.Dispose()
    $glow.Dispose()
    $ring.Dispose()
    $core.Dispose()
    return $bmp
}

function Write-Ico($path, [System.Drawing.Bitmap[]]$images) {
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter $ms
    $bw.Write([uint16]0)
    $bw.Write([uint16]1)
    $bw.Write([uint16]$images.Count)

    $payloads = @()
    foreach ($img in $images) {
        $png = New-Object System.IO.MemoryStream
        $img.Save($png, [System.Drawing.Imaging.ImageFormat]::Png)
        $payloads += , $png.ToArray()
        $png.Dispose()
    }

    $offset = 6 + (16 * $images.Count)
    for ($i = 0; $i -lt $images.Count; $i++) {
        $img = $images[$i]
        $w = if ($img.Width -ge 256) { 0 } else { [byte]$img.Width }
        $h = if ($img.Height -ge 256) { 0 } else { [byte]$img.Height }
        $bw.Write([byte]$w)
        $bw.Write([byte]$h)
        $bw.Write([byte]0)
        $bw.Write([byte]0)
        $bw.Write([uint16]1)
        $bw.Write([uint16]32)
        $bw.Write([uint32]$payloads[$i].Length)
        $bw.Write([uint32]$offset)
        $offset += $payloads[$i].Length
    }

    foreach ($bytes in $payloads) { $bw.Write($bytes) }
    [System.IO.File]::WriteAllBytes($path, $ms.ToArray())
    $bw.Dispose()
    $ms.Dispose()
}

$sizes = 16, 32, 48, 256
$bitmaps = @()
foreach ($size in $sizes) { $bitmaps += New-Mark $size }

$ico = Join-Path $assets "Lumenhop.ico"
Write-Ico $ico $bitmaps
$bitmaps[3].Save((Join-Path $assets "Lumenhop.png"), [System.Drawing.Imaging.ImageFormat]::Png)
foreach ($bmp in $bitmaps) { $bmp.Dispose() }
Write-Output "Wrote $ico"
