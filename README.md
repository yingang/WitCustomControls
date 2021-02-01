# WitCustomControls
Forked from [CodePlex](https://archive.codeplex.com/?p=witcustomcontrols), then added support for Visual Studio 2019.

- [WitCustomControls](#witcustomcontrols)
  - [Introduction](#introduction)
  - [Why this project?](#why-this-project)
  - [Implemented Controls](#implemented-controls)
  - [Build Instructions](#build-instructions)
    - [Visual Studio 2019](#visual-studio-2019)
  - [References](#references)

## Introduction
Custom controls is a feature in TFS. It allows customer developed controls to be hosted in work item form. Those controls can implement various business logics or add additional functionality to work with the workitemform. [Here](http://blogs.msdn.com/narend/archive/2006/10/02/How-to-use-Custom-Controls-in-Work-Item-Form.aspx) is the info on how to build custom controls.

## Why this project?
There are many functionalities that customers want to have in a work item form, and most can be build with using this feature and sometimes by supplementing with an addin. If you think a generic control can be built to address needs out there, you can help build it or suggest that idea for someone else to build it here.
* [Custom Controls Blog Post](http://blogs.msdn.com/greggboer/archive/2010/03/30/work-item-tracking-custom-controls.aspx)
* [Naren Datha](http://blogs.msdn.com/narend)
* [Mathias Olausson](http://msmvps.com/blogs/molausson)

## Implemented Controls
The following controls are implemented in the current release:
* [Screenshot controls](doc/Screenshot%20controls.md): A control where users can paste screenshots & images.
* [Web browser control](doc/Web%20browser%20control.md): A control to host a web page and pass field values to that webpage.
* [Multivalue control](doc/Multivalue%20control.md): A control to accept and show multiple values for a field by showing a list of checkboxes.

## Build Instructions
### Visual Studio 2019
* Install [WiX](https://github.com/wixtoolset/wix3)
* Install Wix Toolset Visual Studio 2019 Extension (from Visual Studio Marketplace)
* Install [Visual Studio Team Explorer 2019](https://visualstudio.microsoft.com/zh-hans/downloads/) (Under Visual Studio 2019)
* Install Microsoft.AspNet.WebPages (using NuGet):

  `Install-Package Microsoft.AspNet.WebPages`

* Add reference path of team explorer for the WitCustomControls project:

  `C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\`

## References

* [Custom Work Item Control not working in Visual Studio 2015 TFS 2013](https://stackoverflow.com/questions/32341142/custom-work-item-control-not-working-in-visual-studio-2015-tfs-2013)
* [Reintroducing the Team Explorer standalone installer](https://devblogs.microsoft.com/devops/reintroducing-the-team-explorer-standalone-installer/)
