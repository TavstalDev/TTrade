# TTrade

![Release (latest by date)](https://img.shields.io/github/v/release/TavstalDev/TTrade?style=plastic-square)
![Workflow Status](https://img.shields.io/github/actions/workflow/status/TavstalDev/TTrade/release.yml?branch=stable&label=build&style=plastic-square)
![License](https://img.shields.io/github/license/TavstalDev/TTrade?style=plastic-square)
![Downloads](https://img.shields.io/github/downloads/TavstalDev/TTrade/total?style=plastic-square)
![Issues](https://img.shields.io/github/issues/TavstalDev/TTrade?style=plastic-square)

### What is this?
This is the source code of a .NETFramework library written in C#. This library is a plugin made for Unturned 3.24.x+ servers. 

### Description
A simple and secure trading system for Unturned.

### Features
* Secure Item Trading
* Trade Preview
* Custom Vault Size

## Requirements

- Unturned 3.24.x or later
- [RocketMod](https://rocketmod.net/) installed on the server

## Installation

1. Download the latest release and its libraries from the [Releases](https://github.com/TavstalDev/TTrade/releases) page.
2. Place `TTrade.dll` into your server's `Rocket/Plugins/` directory.
3. Extract the libraries archive into `Rocket/Libraries` directory.
4. Start or restart the server. The plugin will generate a default YAML configuration file on first load.
5. Edit the configuration file to your liking, then reload the plugin or restart the server.

### Commands
| - means <b>or</b></br>
[] - means <b>required</b></br>
<> - means <b>optional</b>

---

<details>
<summary>/trade [player]</summary>
<b>Description:</b> Send trade request to a player
<br>
<b>Permission(s):</b> ttrade.command.trade
</details>

<details>
<summary>/trade accept [player]</summary>
<b>Description:</b>Accept a pending trade request.
<br>
<b>Permission(s):</b> ttrade.command.trade.accept
</details>

<details>
<summary>/trade deny [player]</summary>
<b>Description:</b> Decline a trade request.
<br>
<b>Permission(s):</b> ttrade.command.trade.deny
</details>

<details>
<summary>/trade cancel</summary>
<b>Description:</b> Cancel an ongoing trade.
<br>
<b>Permission(s):</b> ttrade.command.trade.cancel
</details>

<details>
<summary>/trade open</summary>
<b>Description:</b> Open the trade inventory.
<br>
<b>Permission(s):</b> ttrade.command.trade.open
</details>

<details>
<summary>/trade view</summary>
<b>Description:</b> View the other player's trade inventory.
<br>
<b>Permission(s):</b> ttrade.command.trade.view
</details>

<details>
<summary>/trade finish</summary>
<b>Description:</b> Complete, then finalize the trade.
<br>
<b>Permission(s):</b> ttrade.command.trade.finish
</details>

## Building from Source

### Prerequisites

- .NET Framework 4.8 SDK / targeting pack

### Build Steps

1. Clone the repository:
   ```
   git clone https://github.com/TavstalDev/TTrade.git
   ```
2. Open `TTrade.sln` in your IDE.
3. Build the project:
   ```
   dotnet build -c Release
   ```
4. The compiled `TTrade.dll` will be in `TTrade/bin/Release/`.

## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for more details.

## Contact

For issues or feature requests, please use the [GitHub issue tracker](https://github.com/TavstalDev/TTrade/issues).