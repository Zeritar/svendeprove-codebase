docker run --network mynetwork --name systemoverseercontainer -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="CrypticPassword" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/certificate_so.pfx -v %USERPROFILE%\.aspnet\https:/https/ systemoverseerapi:v1 .