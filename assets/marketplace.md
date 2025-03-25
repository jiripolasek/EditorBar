**Editor Bar** is a Visual Studio 2022 extension designed to simplify your coding workflow by providing clear and intuitive breadcrumbs for effortless navigation.

The extension shows the current file path and project name, enabling quick identification of files with similar or identical names (e.g. `launchSettings.json` or  `/Pages/Users/Edit.razor` vs `/Pages/Roles/Edit.razor`). 

It now also displays breadcrumbs for the internal structure of your code, allowing easy navigation to types, methods, and members in C# and VB.NET files, or nodes in XML documents. Additional formats and features will be supported in future updates.

<div align="center">

![breadcrumbs.png](breadcrumbs.png)

</div>

## Features
- **Breadcrumb Navigation:** Highlights key elements of the file location and current caret position in the file:
  - Solution and solution folders
  - Project name
  - Project folders
  - Parent folder name
  - File name
  - File structure (types, members, nodes, ...)
    - C#
    - VB.NET
    - XML

- **Quick Actions:**
  - Copy full or relative file path to Clipboard.
  - Open file in an custom or default external editor.
  - Open file location in Windows Explorer.
  - Locate the item in Solution Explorer.
  - Switch active target framework for IntelliSense (in multi-targeting projects).

- **Customizable Options:**
  - Select what to display in the breadcrumbs bar.
  - Adjust size, colors, and quick-actions to suit your preferences.

- **Seamless File Access**:
  - Quickly open the file's location in Windows Explorer or in an external editor of your choice, enhancing file management efficiency.

## Visual Preview
![screenshot__1.png](screenshot__1.png)

## Changes

### 2.0
* Added: File structure breadcrumbs (code structure for C#, VB.NET and XML).
    - Drop-drop menu with relevant child items (types in the file, members fields in types).
    - Displays useful secondary information (e.g., method parameters, property types, constant value, xml node id or name).
* Added: Project segment can now display and switch target framework (in multi-targeted projects).
* Added: Keyboard shortcuts to focus specific segments (project, file, etc.).
* Added: "Locate in Soltion Explorer" context menu command for relevant segments.
* Added: "Copy Path" and "Open Containing Folder" context menu command for project and folder segments.
* Added: Automatic contrast fallback: when text contrast is insufficient, its color is automatically adjusted.
* Changed: Improved performance and reduced memory usage.
* Changed: Updated options page.
* Changed: File path label is now hidden by default.

### v1.1.0
* Added: Parent folder segment.
* Added: In-project path segments.
* Added: Bar's background color settings.
* Added: Additional formats for the file name label (e.g., only the file name or the path relative to the parent project).
* Added: Refined control over where the Editor Bar is displayed (e.g., in annotate/blame views, file compare views, temporary files, and non-editable documents).
* Added: Support for non-solution roots (e.g., file system, network locations, files in solution folders not included in the solution, temporary folders).
* Changed: Switched to using the Visual Studio context menu instead of the .NET Framework.
* Changed: Improved the color selection palette for breadcrumb bar segments.
* Changed: Toggle Editor Bar command is now checked if the Editor Bar is enabled.
* Fixes: Bar button's icon colors now correctly matches the bar background.

### v1.0.4
* Added: Ability to open the current file in an external editor.
* Added: A Visual Studio command to toggle the Editor Bar on and off (menu View / Editor Bar and button on Text Editor toolbar). You can also associate a keyboard shortcut with it.
* Added: Options to customize quick actions (double-click and CTRL + double-click).
* Changed: Replaced the property grid options page with a custom dialog page.
* Changed: Replaced the extension icon with a new one.
* Changed: New command icons consistent with Visual Studio UI.


### v1.0.3
* Added: Option to toggle the bar on and off from settings.
* Added: Ability to customize the colors of the elements.
* Added: Ability to hide elements of the bar.
* Added: Additional Compact mode; the bar is smaller and allows more space for your code.
* Changed: Changes in settings are now applied immediately to opened documents.
* Fixed: Misspelled category name in settings.

### v1.0.2
- Fixed: Editor bar was displayed in inappropriate text views. Now the display is limited to documents only.