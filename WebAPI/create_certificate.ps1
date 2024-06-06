$cert = New-SelfSignedCertificate -Subject "CN=systemoverseercontainer" -TextExtension @("2.5.29.17={text}dns=systemoverseercontainer&dns=localhost") -CertStoreLocation cert:\LocalMachine\MY
$pwd = ConvertTo-SecureString -String "CrypticPassword" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath "$($env:USERPROFILE)\.aspnet\https\certificate_so.pfx" -Password $pwd