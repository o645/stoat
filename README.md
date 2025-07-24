# Stoat - HTTP Server
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![GitHub last commit](https://img.shields.io/github/last-commit/o645/stoat)


Stoat is a simple HTTP server written in C#. Written as a personal project exploring Networking programming and the HTTP protocol. Simple and lightweight.

## Installation
### Prerequisites
- Latest Dotnet runtime.

### Steps
1. Clone Repository
```bash
git clone https://github.com/o645/stoat.git
cd stoat
```
2. Enter webServer folder, build and run
```bash
cd webServer
dotnet run
```
3. Open localhost in browser
```bash
open http://localhost:80/
```

## Usage

```bash
dotnet run [path to website] [ip address] [port]
```

Defaults to included debugSite, on localhost:80.

You may need to run it as root/admin, to allow it to open the socket.

## License
![GitHub License](https://img.shields.io/github/license/o645/stoat)
Distributed under the [MIT](https://github.com/o645/stoat/blob/main/LICENSE) License.
