param (
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,

    [Parameter(Mandatory=$true)]
    [string]$WebAppName,

    [Parameter(Mandatory=$false)]
    [string]$IpAddress, # Can be a single IP or comma-separated list (optional)

    [int]$Priority = 100,

    [string]$RuleNamePrefix = "Whitelisted IP"
)

# Check if IpAddress is empty or not provided
if ([string]::IsNullOrWhiteSpace($IpAddress)) {
    Write-Host "No WhitelistedIpAddress provided for this environment. Skipping IP whitelisting."
    exit 0
}

try {
    $webApp = Get-AzWebApp -ResourceGroupName $ResourceGroupName -Name $WebAppName
    
    if ($null -eq $webApp) {
        Write-Error "Web App '$WebAppName' not found."
        exit 1
    }

    # Split inputs by comma and trim whitespace
    $ips = $IpAddress -split ',' | ForEach-Object { $_.Trim() } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

    if ($ips.Count -eq 0) {
        Write-Host "No valid IPs found in WhitelistedIpAddress. Skipping."
        exit 0
    }

    foreach ($ip in $ips) {
        $ruleName = "$RuleNamePrefix - $ip"
        $cidrIp = "$ip/32"

        Write-Host "Checking for IP restriction: $ruleName ($cidrIp)..."

        $existingRule = $webApp.SiteConfig.IpSecurityRestrictions | Where-Object { $_.IpAddress -eq $cidrIp }

        if ($existingRule) {
            Write-Host "Rule for IP '$ip' already exists. Skipping."
        }
        else {
            Write-Host "Adding IP restriction rule '$ruleName' for IP '$ip'..."
            
            Add-AzWebAppAccessRestrictionRule -ResourceGroupName $ResourceGroupName `
                -WebAppName $WebAppName `
                -Name $ruleName `
                -IpAddress $cidrIp `
                -Priority $Priority `
                -Action Allow `
                -Description "Whitelisted IP address via pipeline"

            Write-Host "Successfully added rule for $ip."
        }
    }
}
catch {
    Write-Error "An error occurred: $_"
    exit 1
}
