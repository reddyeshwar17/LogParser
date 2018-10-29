<#
.SYNOPSIS
    This script creates a new Azure AD application and new Key Vault with an associated certificate for Key Vault access authentication.

    This Powershell script is added to projects by the Microsoft.CloudInfrastructure.SecurityHelper NuGet package. Any changes
    which need to be persisted with the master version should be made in the Microsoft.CloudInfrastructure.SecurityHelper project.
    Then create and deploy a new version of the NuGet package:

        NuGet pack Microsoft.CloudInfrastructure.SecurityHelper.nuspec -version n.n.n
#>

Param(
# Subscription Name - example: 'MCIO-E MyTeam Production Corp'
[Parameter(Mandatory=$True, Position=0)]
[string]$subscriptionName,

# Azure AD Application Name - example: 'MCIO MyTeam Corp Key Vault'
[Parameter(Mandatory=$True, Position=1)]
[string]$azureADApplicationDisplayName,

# Azure Key Vault Name - example: 'MCIOMyTeamCorpKeyVault'
# NOTE: Vault name must match regular expression: ^[a-zA-Z0-9-]{3,24}$
[Parameter(Mandatory=$True, Position=2)]
[string]$vaultName,

# Azure Resource Group Name - example: 'MCIOMyTeamCorpResourceGroup'
[Parameter(Mandatory=$True, Position=3)]
[string]$resourceGroupName,

# Azure Location - Example: 'West US'
# NOTE: Use Get-AzureLocation to see values
[Parameter(Mandatory=$True, Position=4)]
[string]$location,

# Local path to certificate - Example: 'D:\Certificates\mycert.corp.microsoft.com.cer'
# NOTE: This must be a .cer file.
[Parameter(Mandatory=$True, Position=5)]
[string]$pathToCertificate
)

# Log into Azure and ensure that correct subscription is selected.
Login-AzureRmAccount
Select-AzureRmSubscription -SubscriptionName $subscriptionName

# Prepare certificate.
$x509 = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$x509.Import($pathToCertificate)
"Authentication certificate thumbprint: " + $x509.Thumbprint
$credentialValue = [System.Convert]::ToBase64String($x509.GetRawCertData())
$now = [System.DateTime]::Now
$oneYearFromNow = $now.AddYears(1)

# Add Azure AD application.
$azureADApplicationUrl = [string]::Format("http://{0}", $vaultName)
"Azure AD application URL: " + $azureADApplicationUrl
$azureADApplication = New-AzureRmADApplication -DisplayName $azureADApplicationDisplayName -HomePage $azureADApplicationUrl -IdentifierUris $azureADApplicationUrl -KeyValue $credentialValue -KeyType "AsymmetricX509Cert" -KeyUsage "Verify" -StartDate $now -EndDate $oneYearFromNow
"Azure AD application ID: " + $azureADApplication.ApplicationId
$servicePrincipal = New-AzureRmADServicePrincipal -ApplicationId $azureADApplication.ApplicationId
"Service Principal application ID: " + $servicePrincipal.ApplicationId

# Create Azure resource group.
New-AzureRmResourceGroup -Name $resourceGroupName -Location $location
"Resource group: " + $resourceGroupName + " in '" + $location + "'"

# Create Key Vault.
$vault = New-AzureRmKeyVault -VaultName $vaultName -ResourceGroupName $resourceGroupName -Sku premium -Location $location
"Vault name: " + $vaultName

# Authorize Azure AD application to access vault.
Set-AzureRmKeyVaultAccessPolicy -VaultName $vaultName -ObjectId $servicePrincipal.Id -PermissionsToKeys all -PermissionsToSecrets all

# Show configuration settings.
"`nAdd the following configuration settings to the cloud service .csfg file:"
'<ConfigurationSettings>'
'  <Setting key="KeyVault.Authentication.Certificate.Thumbprint" value="' + $x509.Thumbprint + '" />'
'  <Setting key="KeyVault.Authentication.ClientId" value="' + $servicePrincipal.ApplicationId + '" />'
'  <Setting key="KeyVault.Vault.Address" value="https://' + $vaultName.ToLower() + '.vault.azure.net/" />'
'</ConfigurationSettings>'
