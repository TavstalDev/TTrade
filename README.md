# TTrade

### What is this?
This is the source code of a .NETFramework library written in C#. This library is a plugin made for Unturned 3.24.x+ servers. 

### Description
A simple and secure trading system for Unturned.

### Features
* Secure Item Trading
* Trade Preview
* Custom Vault Size

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
