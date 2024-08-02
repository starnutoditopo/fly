FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Fly/Fly.csproj", "Fly/"]
RUN dotnet restore "Fly/Fly.csproj"

COPY ["Fly.Desktop/Fly.Desktop.csproj", "Fly.Desktop/"]
RUN dotnet restore "Fly.Desktop/Fly.Desktop.csproj"
COPY Fly/ Fly/
COPY Fly.Desktop/ Fly.Desktop/

WORKDIR "/src/Fly.Desktop"
RUN dotnet build "Fly.Desktop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fly.Desktop.csproj" -c Release -o /app/publish --runtime linux-x64 --self-contained=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fly.Desktop.dll"]

# Install required runtime libraries:
#  libfontconfig1 -> Required at startup
#  libx11-6       -> Required at startup
#  libsm6         -> Required at startup
#  gedit          -> prevents "Unhandled exception. System.InvalidOperationException: Neither DBus nor GTK are available on the system" when launching open/save file dialogbox

RUN apt-get update
RUN apt-get install -y \
      libfontconfig1 \
	  libx11-6 \
	  libsm6 \
	  gedit



# Build:
#   docker build -t fly .
#
# To run the application within a Windows host:
#   Install VcXsrv Windows X Server to share the display from a windows host
#     see: https://sourceforge.net/projects/vcxsrv/
#
#   Run VcXsrv Windows X Server
#   
#   Set the environment variable (replace IP with yours):
#     set-variable -name DISPLAY -value 172.30.192.1:0.0
#   
#   Run the container:
#     docker run -ti --rm -e DISPLAY=$DISPLAY fly
#
#     Notes:
#
#       - to persist preferences locally, a volume can be mapped for the container's path `/root/.local/share/IsolatedStorage`.
#         Example (bind mount host local path `.preferences`):
#           docker run -ti --rm -e DISPLAY=$DISPLAY -v ${PWD}/.preferences:/root/.local/share/IsolatedStorage fly
#
#       - One may also like to share .fly documents between host and container. To do that, just map another directory.
#         Example (bind mount host local path `Documentation/SampleDocuments/` as container's `/MyDocuments/`):
#           docker run -ti --rm -e DISPLAY=$DISPLAY -v ${PWD}/Documentation/SampleDocuments:/MyDocuments fly
#
#       - Here's an example with both mounts. With it, you can persist your settings, open and save documents in the container's `/MyDocuments/` folder,
#         that are phisically stored in the host's `Documentation/SampleDocuments` local folder.
#           docker run -ti --rm -e DISPLAY=$DISPLAY -v ${PWD}/Documentation/SampleDocuments:/MyDocuments -v ${PWD}/.preferences:/root/.local/share/IsolatedStorage fly