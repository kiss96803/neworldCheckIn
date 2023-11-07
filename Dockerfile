# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy everything else and build app
COPY . .
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
ENV userName="1"
ENV pwd="2"
ENV cronExpr="3"
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "neworldCheckIn.dll"]