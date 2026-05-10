# Change Log

## 4.0.0
- [x] **Added: Child tree popup for location breadcrumbs.**
- [x] **Added: Open in Terminal command.**
- [x] **Added: Separate light and dark color settings and refreshed the options UI.**
- [x] Added: Support for C# 14 extension blocks.
- [x] Added: Settings to control toolbar button visibility.
- [x] Added: Option to show or hide the member list filter box.
- [x] Added: Context menu commands for solution folder and non-solution root breadcrumbs.
- [x] Added: Quick toggle for the built-in VS navigation bar.
- [x] Added: Options button is now a drop-down menu with quick access to some settings. 
- [x] Changed: Upgraded code analysis integration in a backwards-compatible way to support both VS2022 and VS2026.
- [x] Changed: Internal code quality improvements, including nullability and threading fixes, broader encapsulation, and style cleanup.
- [x] Changed: Popups looks nicer and more in-line with VS2026 style.
- [x] Fixed: Filter box background now follows the active theme.
- [x] Fixed: Memory leak in `LegacyLabelViewModel` caused by a static event subscription.
- [x] Fixed: Structural breadcrumbs now degrade gracefully when initialization fails.
- [x] Fixed: Better handling for projects that do not expose their full name.
- [x] Fixed: Locate in Solution Explorer for the root breadcrumb now selects the solution node.
- [x] **Fixed: Editor Bar settings are now correctly included in Visual Studio import/export.**

## 3.0.1024
- [x] Fixed: An issue where Visual Studio incorrectly changed focus when *Navigate Backwards* or *Navigate Forwards* commands were used to navigate between files.

## 3.0.0
- [x] Added: Member list is now searchable - start typing to filter the list.
- [x] Added: Support for Visual 2026.
- [x] Changed: Editor bar is hidden in ReSharper windows (e.g., Find Results, Unit Test Sessions).

## 2.0.2048

 - [x] Fixed: Breadcrumb separator rendering glitch in VS 17.14.10 Preview 1.0.
 - [x] Fixed: Benign exception that could have previously appeared in the extension log.


## 2.0.0
- [x] Added: File structure breadcrumbs (code structure for C#, VB.NET, and XML).
    - Drop-down menu with relevant child items (types in the file, members and fields within types).
    - Displays useful secondary information (e.g., method parameters, property types, constant values, XML node IDs or names).
- [x] Added: Project segment can now display and switch target framework for IntelliSense (in multi-targeted projects).
- [x] Added: Keyboard shortcuts to focus specific segments (project, file, etc.).
- [x] Added: "Locate in Solution Explorer" context menu command for relevant segments.
- [x] Added: "Copy Path" and "Open Containing Folder" context menu commands for project and folder segments.
- [x] Added: Automatic contrast fallback; when text contrast is insufficient, its color is automatically adjusted.
- [x] Changed: Improved performance and reduced memory usage.
- [x] Changed: Updated options page.
- [x] Changed: File path label is now hidden by default.

## 1.1.0
- [x] Added: Parent folder segment.
- [x] Added: In-project path segments.
- [x] Added: Bar's background color settings.
- [x] Added: Additional formats for the file name label (e.g., only the file name or the path relative to the parent project).
- [x] Added: Refined control over where the Editor Bar is displayed (e.g., in annotate/blame views, file compare views, temporary files, and non-editable documents).
- [x] Added: Support for non-solution roots (e.g., file system, network locations, files in solution folders not included in the solution, temporary folders).
- [x] Changed: Switched to using the Visual Studio context menu instead of the .NET Framework.
- [x] Changed: Improved the color selection palette for breadcrumb bar segments.
- [x] Changed: Toggle Editor Bar command is now checked if the Editor Bar is enabled.
- [x] Fixed: Bar button's icon colors now correctly matches the bar background.

## 1.0.4
- [x] Added: Ability to open the current file in an external editor.
- [x] Added: A Visual Studio command to toggle the Editor Bar on and off (menu *View / Editor Bar* and button on *Text Editor* toolbar).
- [x] Added: Options to customize quick actions (double-click and CTRL + double-click).
- [x] Changed: Replaced the property grid options page with a custom dialog page.
- [x] Changed: Replaced the extension icon with a new one.
- [x] Changed: New command icons consistent with Visual Studio UI.

## 1.0.3
- [x] Added: Option to toggle the bar on and off from settings.
- [x] Added: Ability to customize the colors of the elements.
- [x] Added: Ability to hide elements of the bar.
- [x] Added: Additional Compact mode; the bar is smaller and allows more space for your code.
- [x] Changed: Changes in settings are now applied immediately to opened documents.
- [x] Fixed: Misspelled category name in settings.

## 1.0.2
- [x] Fixed: Editor bar was displayed in appropriate text views. Not the display is limited to documents only.

## 1.0.1
- [x] Update `README.md` file and the marketplace summary

## 1.0.0
- [x] Initial release
