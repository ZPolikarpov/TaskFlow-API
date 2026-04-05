FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ENV NUGET_PACKAGES=/root/.nuget/packages

COPY TaskFlow.sln .
COPY TaskFlow.Api/TaskFlow.Api.csproj                       TaskFlow.Api/
COPY TaskFlow.Application/TaskFlow.Application.csproj       TaskFlow.Application/
COPY TaskFlow.Domain/TaskFlow.Domain.csproj                 TaskFlow.Domain/
COPY TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj TaskFlow.Infrastructure/
COPY TaskFlow.Tests/TaskFlow.Tests.csproj                   TaskFlow.Tests/

RUN dotnet restore --packages /root/.nuget/packages

COPY . .

RUN dotnet publish TaskFlow.Api/TaskFlow.Api.csproj \
    -c Release -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskFlow.Api.dll"]