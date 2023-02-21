FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./src/VVTDE/VVTDE.csproj", "VVTDE/"]
RUN dotnet restore "./VVTDE/VVTDE.csproj"
COPY ./src /src
WORKDIR "/src/VVTDE"
RUN dotnet build "VVTDE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VVTDE.csproj" -c Release -o /app/publish
RUN dotnet dev-certs https

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VVTDE.dll"]
