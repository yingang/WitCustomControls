# WitCustomControls
Forked from [CodePlex](https://archive.codeplex.com/?p=witcustomcontrols), then added support for Visual Studio 2019.

## Build Instructions (Visual Studio 2019)

* Install [WiX](https://github.com/wixtoolset/wix3)
* Install Wix Toolset Visual Studio 2019 Extension (from Visual Studio Marketplace)
* Install [Visual Studio Team Explorer 2019](https://visualstudio.microsoft.com/zh-hans/downloads/) (Under Visual Studio 2019)
* Install Microsoft.AspNet.WebPages (using NuGet):

  Install-Package Microsoft.AspNet.WebPages

* Add reference path of team explorer for the WitCustomControls project:

  C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\


## References

* [Custom Work Item Control not working in Visual Studio 2015 TFS 2013](https://stackoverflow.com/questions/32341142/custom-work-item-control-not-working-in-visual-studio-2015-tfs-2013)
* [Reintroducing the Team Explorer standalone installer](https://devblogs.microsoft.com/devops/reintroducing-the-team-explorer-standalone-installer/)