param(
    [string]$RootDirectory = "."
)

function Get-LatestMajorNuGetVersion {
    param (
        [string]$PackageId,
        [string]$MajorVersion
    )
    $LowerBound = "$MajorVersion."
    $url = "https://api.nuget.org/v3-flatcontainer/$($PackageId.ToLower())/index.json"
    try {
        $response = Invoke-RestMethod -UseBasicParsing -Uri $url -ErrorAction Stop
        if ($response.versions) {
            $matching = $response.versions | Where-Object { $_ -like "$LowerBound*" }
            if ($matching) {
                return $matching[-1]
            }
        }
    } catch {
        Write-Warning "  Problem downloading info for $PackageId ($url)"
    }
    return $null
}

function Update-Packages-InFile {
    param(
        [string]$FilePath
    )

    Write-Host "`nChecking $FilePath" -ForegroundColor Cyan
    try {
        [xml]$xml = [xml](Get-Content $FilePath -Raw)
    } catch {
        Write-Warning "$FilePath is not a valid XML file, skipping."
        return
    }

    $modified = $false
    $chg = @()

    if ($FilePath -like "*Directory.Packages.props") {
        foreach ($itemGroup in $xml.Project.ItemGroup) {
            foreach ($pkg in $itemGroup.PackageVersion) {
                $pkgId   = $pkg.Include
                $currVer = $pkg.Version
                if ($currVer -match '^(\d+)\.') {
                    $major = $matches[1]
                } else {
                    $major = 8 # fallback
                }
                $latest = Get-LatestMajorNuGetVersion -PackageId $pkgId -MajorVersion $major
                if ($latest -and $latest -ne $currVer) {
                    Write-Host ("  Updating {0} ({1} -> {2})" -f $pkgId,$currVer,$latest) -ForegroundColor Yellow
                    $pkg.SetAttribute("Version", "$latest")
                    $modified = $true
                    $chg += "$pkgId : $currVer -> $latest"
                } else {
                    Write-Host ("  {0} already up to date ({1})" -f $pkgId,$currVer) -ForegroundColor Gray
                }
            }
        }
    }
    elseif ($FilePath -like "*.csproj") {
        $itemGroups = $xml.Project.ItemGroup | Where-Object { $_.Condition -match "TargetFramework" }
        foreach ($group in $itemGroups) {
            if ($group.Condition -match "'\$\((TargetFramework)\)'\s*==\s*'(?<tf>[a-zA-Z0-9\.]+)'") {
                $tf = $matches['tf']
                if ($tf -match "^net(?<major>\d+)\.0$") {
                    $major = $matches['major']
                } else {
                    continue
                }
                $packageItems = $group.PackageVersion
                foreach ($pkg in $packageItems) {
                    $pkgId   = $pkg.Include
                    $currVer = $pkg.Version
                    if (-not $pkgId -or -not $currVer) { continue }
                    $latest = Get-LatestMajorNuGetVersion -PackageId $pkgId -MajorVersion $major
                    if ($latest -and $latest -ne $currVer) {
                        Write-Host ("  Updating {0} ({1} -> {2})" -f $pkgId,$currVer,$latest) -ForegroundColor Yellow
                        $pkg.SetAttribute("Version", "$latest")
                        $modified = $true
                        $chg += "$pkgId : $currVer -> $latest"
                    } else {
                        Write-Host ("  {0} already up to date ({1})" -f $pkgId,$currVer) -ForegroundColor Gray
                    }
                }
            }
        }
    }
    else {
        Write-Host "  Skipped - not relevant type" -ForegroundColor DarkGray
        return
    }

    if ($modified) {
        $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
        $stream = [System.IO.StreamWriter]::new($FilePath, $false, $utf8NoBom)
        $xml.Save($stream)
        $stream.Close()
        Write-Host "  Saved $FilePath" -ForegroundColor Green
        return ,$chg
    } else {
        Write-Host "  No changes in $FilePath"
        return @()
    }
}

$files = Get-ChildItem -Path $RootDirectory -Recurse -Include "Directory.Packages.props","*.csproj" -File
$allChg = @()
foreach ($file in $files) {
    $result = Update-Packages-InFile -FilePath $file.FullName
    if ($result) {
        $allChg += $result
    }
}
if ($allChg.Count -gt 0) {
    Set-Content -Path conditional-updates.log -Value ($allChg -join "`n")
} else {
    Set-Content -Path conditional-updates.log -Value ""
}
Write-Host "`nCompleted full scan!" -ForegroundColor Magenta