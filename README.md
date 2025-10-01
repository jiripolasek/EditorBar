<div align="center">

<p>
	<img alt="Editor Bar Icon" src="assets/Icon.png" />
</p>

<h1>Editor Bar<br /><span style="font-weight: 300; opacity: 0.5">for Visual Studio</span></h1>

[![Visual Studio Marketplace Downloads](https://img.shields.io/visual-studio-marketplace/d/jiripolasek.EditorBar?style=for-the-badge&label=VS%20Marketplace&link=https%3A%2F%2Fmarketplace.visualstudio.com%2Fitems%3FitemName%3Djiripolasek.EditorBar)](https://marketplace.visualstudio.com/items?itemName=jiripolasek.EditorBar)
[![Visual Studio Marketplace Rating](https://img.shields.io/visual-studio-marketplace/stars/jiripolasek.EditorBar?style=for-the-badge)](https://marketplace.visualstudio.com/items?itemName=jiripolasek.EditorBar)
[![Visual Studio Marketplace Version](https://img.shields.io/visual-studio-marketplace/v/jiripolasek.EditorBar?style=for-the-badge)](https://marketplace.visualstudio.com/items?itemName=jiripolasek.EditorBar)
[![Static Badge](https://img.shields.io/badge/💚%20popularity-great-brightgreen?style=for-the-badge)](https://marketplace.visualstudio.com/items?itemName=jiripolasek.EditorBar)

</div>

## Introduction

**Editor Bar** is a Visual Studio 2022 and a Visual Studio 2026 extension designed to simplify your coding workflow by providing clear and intuitive breadcrumbs for effortless navigation.

The extension displays the current file path and project name, enabling quick identification of files with similar or identical names (like `launchSettings.json` in every project or `/Pages/Users/Edit.razor` vs `/Pages/Roles/Edit.razor`).

In supported files, it also displays breadcrumbs for symbols or nodes up to the cursor position. This works for C# and VB.NET files, as well as nodes in XML documents. Additional formats will be supported in future updates.


<div align="center">

![Drop-down menu](assets/dropdown.png)


![Breadcrumbs](assets/breadcrumbs.png)

</div>

## Features

- **Breadcrumb Navigation** 
  - Highlights key elements of the file location and for supported files a breadcrumb trail of the current file node or symbol up to the cursor position:
      - Solution and solution folders
      - Project
      - Project folders
      - Parent folder name
      - File name
      - File structure (types, members, nodes, ...)
        - C#
        - VB.NET
        - XML

- **Quick Actions**
  - Copy full or relative file path to Clipboard.
  - Open file in an custom or default external editor.
  - Open file location in Windows Explorer.
  - Locate the item in Solution Explorer.
  - Switch active target framework for IntelliSense (in multi-targeting projects).

- **Member search**
  - Quickly find and navigate to any member in the current file using a searchable dropdown.

- **Customizable Options**
  - Select what to display in the breadcrumbs bar.
  - Adjust size, colors, and quick-actions to suit your preferences.

- **Seamless Control**:
  - Toggle the Editor Bar on and off using a toolbar icon or a keyboard shortcut.

- **Additional Features:**
  - See [complete changelog](CHANGELOG.md) for more details.

## Screenshots

![Extension Screenshot](assets/screenshot-dropdowns.png)
![Extension Screenshot](assets/screenshot.png)
![Extension Options Screenshop](assets/options.png)

## Licence

Apache 2.0

## Author

[Jiří Polášek](https://jiripolasek.com)
