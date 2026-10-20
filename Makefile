build:
	dotnet build Client/src/WebApi
	dotnet build Server/src/WebApi

client:
	dotnet run --project Client/src/WebApi

server:
	dotnet run --project Server/src/WebApi
