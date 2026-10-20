Sheffield

dotnet new webapi 



Sheffield.Client.WebApi

Sheffield.

Sheffield.Server.WebApi


dotnet new webapi --framework net10.0 --no-https --use-program-main --output Server/src/WebApi --use-controllers --name Sheffield.Server.WebApi

dotnet new webapi --framework net10.0 --no-https --use-program-main --output Client/src/WebApi --use-controllers --name Sheffield.Client.WebApi


dotnet new classlib --framework net10.0 --output Shared/src/Grains --name Sheffield.Shared.Grains
dotnet new classlib --framework net10.0 --output Shared/src/Services --name Sheffield.Shared.Services

dotnet add Client/src/WebApi reference Shared/src/Grains
dotnet add Shared/src/Services reference Shared/src/Grains
dotnet add Server/src/WebApi reference Shared/src/Services

dotnet sln add Client/src/WebApi
dotnet sln add Server/src/WebApi

dotnet add Client/src/WebApi package Microsoft.Orleans.Client
dotnet add Client/src/WebApi package Microsoft.Orleans.Clustering.Consul
dotnet add Client/src/WebApi package Swashbuckle.AspNetCore.SwaggerUI

dotnet add Shared/src/Grains package Microsoft.Orleans.Sdk

dotnet add Server/src/WebApi package Microsoft.Orleans.Server
dotnet add Server/src/WebApi package Microsoft.Orleans.Clustering.Consul
dotnet add Server/src/WebApi package Microsoft.Orleans.Dashboard

```powershell
consul acl policy create -name "read-write-policy" -rules @rw-policy.hcl
consul acl token create -description "Orleans" -policy-name "read-write-policy"
```
