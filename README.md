# macsynkker

[![Build Status](https://img.shields.io/github/actions/workflow/status/CreativeCodersTeam/macsynkker/main.yml?style=flat-square&label=Build)](https://github.com/CreativeCodersTeam/macsynkker/actions)
![.NET](https://img.shields.io/badge/.NET-10-512bd4?style=flat-square)
![macOS](https://img.shields.io/badge/Platform-macOS-000?style=flat-square&logo=apple)
[![License](https://img.shields.io/badge/License-Apache_2.0-blue?style=flat-square)](LICENSE)

> Synchronize Homebrew packages and macOS user defaults across machines

macsynkker is a .NET CLI tool that exports and imports your Homebrew formulae, casks, and macOS user defaults so you can
replicate your setup on a new Mac or keep multiple machines in sync.

## Features

- **Homebrew Export/Import** — Serialize installed formulae and casks to a JSON file and restore them on another
  machine, including tap management
- **macOS User Defaults Export/Import** — Back up and restore per-domain `defaults` settings as plist files
- **Homebrew Upgrade** — Upgrade installed formulae and casks with configurable options
- **Dependency-aware Export** — Optionally include or exclude packages installed as dependencies

## Prerequisites

- macOS (the CLI refuses to run on other platforms)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Homebrew](https://brew.sh/) (for Homebrew-related features)

## Installation

### As a .NET global tool

```bash
dotnet tool install -g CreativeCoders.MacSynkker.Cli
```

### From a GitHub release

Download the latest `MacSynkker.Cli.tar.gz` from
the [Releases](https://github.com/CreativeCodersTeam/macsynkker/releases) page and extract it.

## Usage

The CLI is invoked as `macsk`. Run it without arguments to see available commands:

```bash
macsk
```

### Export Homebrew packages

```bash
macsk brew export --file ~/brew-packages.json
```

### Import Homebrew packages on another machine

```bash
macsk brew import --file ~/brew-packages.json
```

### Export macOS user defaults

```bash
macsk defaults export --domain com.apple.finder --file ~/finder-defaults.plist
```

> [!TIP]
> Store the exported JSON/plist files in a Git repository or cloud drive to keep your machines in sync.

## Project structure

```
source/
  CreativeCoders.MacOS.Core/            Core macOS utilities (program locator)
  CreativeCoders.MacOS.HomeBrew/        Homebrew query, install, upgrade, export, import
  CreativeCoders.MacOS.UserDefaults/    macOS user defaults export/import
  CreativeCoders.MacSynkker.Cli/        CLI entry point (macsk)
samples/
  SampleConsoleApp/                     Sample showing library usage
tests/
  CreativeCoders.MacOS.HomeBrew.Tests/  Unit tests for Homebrew library
  CreativeCoders.MacOS.UserDefaults.Tests/
build/                                  Cake-based build system
```

## Development

```bash
git clone https://github.com/CreativeCodersTeam/macsynkker.git
cd macsynkker
```

### Build

```bash
dotnet build
```

### Run tests

```bash
dotnet test
```

## CI/CD pipeline

The CI/CD pipeline uses a [Cake](https://cakebuild.net/)-based build system.

```bash
# Build targets
sh ./build.sh -t test
sh ./build.sh -t publish
sh ./build.sh -t createdistpackages
```
