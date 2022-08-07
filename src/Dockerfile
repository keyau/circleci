ARG DOTNET_VERSION=6.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine as base-build
WORKDIR /app

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} as base-runtime
EXPOSE 80
WORKDIR /app

FROM base-build AS dependencies
COPY *.sln nuget.config dotnet-tools.json global.json ./
RUN dotnet tool restore
COPY /*/*.*sproj  ./
RUN for file in $(ls *.*sproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

FROM base-build AS source
RUN echo "📜 Source" > /dev/null
COPY . .

FROM base-build AS build
COPY --from=dependencies /root/.nuget /root/.nuget
COPY --from=dependencies /app ./
COPY --from=source /app ./
RUN echo "🔨 BUILD" > /dev/null
RUN dotnet build --no-restore -c Release

FROM base-build AS test
COPY --from=dependencies /root/.nuget /root/.nuget
COPY --from=dependencies /root/.dotnet /root/.dotnet
COPY --from=build /app ./
RUN echo "🧪 TEST" > /dev/null
RUN \
  mkdir /result; \
  dotnet test -c Release --no-build \
    /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput="/artifacts/coverage.xml" \
    --test-adapter-path:. --logger:"junit;LogFilePath=/artifacts/test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose"; \
  exitcode=$?; \
  if [ $exitcode -eq 0 ]; \
  then \
    dotnet reportgenerator "-reports:/artifacts/coverage.xml" "-targetdir:/artifacts/testreport" "-reporttypes:Html"; \
  fi; \
  if [ $exitcode -ne 0 ]; \
  then \
    touch /result/failed; \
  fi; \
  exit 0

FROM base-build AS base-publish
COPY --from=dependencies /root/.nuget /root/.nuget
COPY --from=build /app ./

FROM base-publish AS publish-app
RUN echo "📦 PUBLISH Api" > /dev/null
RUN dotnet publish Weather.Api/Weather.Api.csproj --no-restore -c Release -o out

FROM base-runtime AS runtime
COPY --from=publish-app /app/out ./
ENTRYPOINT ["dotnet", "./Weather.Api.dll"]