<h1>Introduction</h1>

The Nemeio desktop application is commonly referred to as "disembarked" internally, 
as opposed to the "embedded" system, which constitutes the keyboard's control code (*firmware*).

The purpose of this application is to ensure the connection between the Nemeio keyboard 
and the operating system to which it is connected.
Initially, two types of systems are considered: Windows 10+ and macOS. 
Later, the *Linux* system will be taken into account.

The desktop application currently includes - for the *Windows* version - the *Chromium* browser 
to display the *Configurator*, which is a third-party application but deployed in an integrated manner.

# Table of Contents
1. [Organization](#organization)<br>
1.1. [Philosophy](#philosophy)<br>
1.2. [Tools Used](#tools)<br>
1.2.1. [Windows Tools](#windows_tools)<br>
1.2.1.1. [Git](#windows_tools_git)<br>
1.2.1.2. [Code and Installers](#windows_tools_code)<br>
1.2.1.3. [Icons and Images](#windows_tools_icon)<br>
1.2.1.4. [Documentation](#windows_tools_doc)<br>
1.2.2. [macOS Tools](#macOS_tools)<br>
1.3. [Project Directory](#directory)<br>
1.3.1. [Retrieving the Directory](#directory_retrieval)<br>
1.3.2. [Submodules](#directory_submodules)<br>
1.3.3. [Large Dependencies](#directory_lfs)<br>
1.4. [Modules](#modules)<br>
1.4.1. [Main Modules](#main_modules)<br>
1.4.2. [Windows Modules](#windows_modules)<br>
1.4.3. [macOS Modules](#macOS_modules)<br>
1.4.4. [Test Modules](#test_modules)<br>
1.5. [Installers](#installers)<br>
1.5.1. [Windows Installation](#installers_windows)<br>
1.5.1.1 [Windows Installer](#installers_windows_regular)<br>
1.5.1.2 [Windows Auto-Installer](#installers_windows_auto)<br>
1.5.2. [macOS Installation](#installers_macOS)<br>
1.5.2.1. [macOS Package](#installers_macOS_regular)<br>
1.5.2.2. [macOS Auto-Installer](#installers_macOS_auto)<br>
1.6. [Pipelines](#pipelines)<br>
1.7. [Documentation](#documentation)<br>

<h1>1. Organization<a name="organization"></h1>

<h2>1.1. Philosophy<a name="philosophy"></h2>

A single versioned directory contains all the necessary information 
to build and distribute the project.<br>
This includes:
1. The source code written in C# and ported to Xamarin on macOS
2. The code required to create installers
3. The resources needed to create icons
4. YAML pipelines and associated scripts
5. Documentation

<h2>1.2. Tools Used<a name="tools"></h2>

This section lists the various necessary and/or recommended tools 
for managing this project on each platform.

<h3>1.2.1. Windows Tools<a name="windows_tools"></h3>

<h4>1.2.1.1 - Git Code Retrieval<a name="windows_tools_git"></h4>

- <h5>Git</h5>

The primary and initial tool is ***Git*** for Windows.

> https://gitforwindows.org/

It is used to manage the code currently hosted on *Bitbucket*.
Its <*git bash*> consoles are very useful for easy command-line management.

- <h5>Git-LFS</h5>

The Git-LFS extension is also required to download and store 
binary third-party resources that are large, rarely change, and are not
managed as *Nuget* dependencies...<br>
This module is particularly necessary for generating installers.

> https://git-lfs.github.com/

These dependencies include Windows installer files related to deploying 
*Chromium*, which unfortunately is not managed as a third-party application.<br>
This product is currently integrated into Large File Storage, mainly containing 
the subfolder "*Nemeio.WindowsInstaller/Dependencies/*").<br>
This represents ***342 MB*** of embedded resources...

**Note:**

Regular application resources such as icons, images,
and other small binary files are stored directly in 
the solution.

- <h5>Fork (optional)</h5>

The ***Fork*** tool is very useful for an intuitive graphical interface:

> https://git-fork.com/
  
It is a good replacement for *SourceTree* (*Atlassian*), which is often 
problematic, and is available on both 
*Windows* and *macOS*.

- <h5>TortoiseGit (optional)</h5>
  
The ***TortoiseGit*** tool can also be installed for some 
useful features (history, diffs, etc.):

> <https://tortoisegit.org/>

<h4>1.2.1.2 - Compilation of Code and Installers<a name="windows_tools_code"></h4>

- <h5>Visual Studio Community Edition</h5>

The primary tool at the time of writing is ***Visual Studio 
Community Edition 2019*** commonly referenced as VS2019

> https://visualstudio.microsoft.com/fr/vs/

Required installation options:
* ASP.NET and web development
* .NET desktop development
* Universal Windows Platform development
* Mobile development with .NET (using Xamarin)
* Visual Studio Extension development
* .NET Core cross-platform development

- <h5>Wix Toolset</h5>

To create Windows installers, the ***Wix Toolset Build Tools***
must be deployed.

> https://wixtoolset.org/releases/

- <h5>WixToolset Visual Studio Extension</h5>

This plugin is not essential for compiling the installer but can be useful 
for modifying it within VS2019.

> https://marketplace.visualstudio.com/items?itemName=WixToolset.WixToolsetVisualStudio2019Extension

However, manual editing of <*.wxs*> files will likely still be necessary.

- <h5>Spell Checker</h5>

Installing the following spell checker module may be useful, particularly 
for verifying this documentation (in French in this case).

> https://marketplace.visualstudio.com/items?itemName=EWoodruff.VisualStudioSpellCheckerVS2017andLater

<h4>1.2.1.3 - Editing Icons and SVG Images<a name="windows_tools_icon"></h4>

- <h5>SVG Editor</h5>

SVG format images provided by LDLC can be edited with ***Inkscape***

> https://inkscape.org/fr/

<h4>1.2.1.4 - Documentation<a name="windows_tools_doc"></h4>

- <h5>Markdown Editor</h5>

A Markdown plugin integrated into Visual Studio for editing this documentation.

> https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor

Other external Markdown tools can also be used.

<h3>1.2.2. macOS Tools<a name="macOS_tools"></h3>

< ... ***FixMe***: To be completed ... >

<h2>1.3. Project Directory<a name="directory"></h2>

The container for the entire project on Bitbucket can be accessed at:

> <https://github.com/Nemeio/NemeioKB-desktop-app/>

<h3>1.3.1 Retrieving the Code<a name="directory_retrieval"></h5>

To retrieve this container, execute:

```sh
$ git clone --recursive https://github.com/Nemeio/NemeioKB-desktop-app.git
```

The **--recursive** option ensures the retrieval of submodules, particularly the *Configurator*, which is managed separately.

By default, cloning positions the repository on the "*master*" branch, but you can switch to the development branch with:

```sh
$ git checkout development
```

<h3>1.3.2. Retrieving Submodules<a name="directory_submodules"></h3>

If a recursive clone was not performed earlier, submodules can be retrieved with:

```sh
$ git submodule update --init
```

<h3>1.3.3. Retrieving Large Dependencies<a name="directory_lfs"></h3>

If Windows installers need to be generated, the separately stored resources on Git LFS must be retrieved.
Once the git-lfs module is installed for the first time on the machine, execute:

```sh
$ git lfs install
```

After installation, explicitly retrieve the resources with:

```sh
$ git lfs checkout
```

***Note:***
*The above command may seem unusual, as a `git update` might have been expected...*

<h2>1.4. Modules<a name="modules"></h2>

The versioned Git directory contains various code modules.

<h3>1.4.1. Main Modules<a name="main_modules"></h3>

These are the shared projects across both platforms, consolidating common code. 
These modules are based on the ***.NET Standard 2.0*** framework.

Their structure generally follows a dependency hierarchy on specialized external libraries according to functionality.

![Nemeio Shared Modules](./PlantUML/01-Organization-01-CommonModules.png)

<h4>Nemeio.Core</h4>

This module has no dependencies other than the *.NET Standard 2.0* framework (and it should remain that way). All other modules depend on this one.

It includes all base classes used across all platforms and mechanisms common to these platforms (*Enums*, *Interfaces*, abstract base classes implementing shared functionalities, etc.).
As much as possible, mechanisms should be added to this *Assembly* to ensure platform-wide consistency.

<h4>Nemeio.Infrastructure</h4>

This module primarily manages the database. It is based on EFCore and depends on *Microsoft.EntityFrameworkCore.Sqlite*.

For EF Core operation, refer to Microsoft's official documentation:
https://docs.microsoft.com/en-us/ef/core/

<h4>Nemeio.LayoutGen</h4>

This module dynamically generates images related to a *layout*. It primarily depends on *SkiaSharp*.

A layout is generated using the ***IDeviceMap*** interface, which is implemented and specialized per system.

<h4>Nemeio.API</h4>

This module manages the local server interfacing the desktop application with the *Configurator*. It provides various *End-Points*, which can be documented using *Swagger*.

This server can be secured with HTTPS on Windows, although this mechanism is currently disabled. Securing this service with HTTPS may seem unnecessary since it is only used internally and does not exchange data with external entities.

<h4>Nemeio.Acl</h4>

ACL stands for *Access Control Layer*. This module is originally intended to define an access layer for the disembarked system.
As a result, in addition to its expected dependency on *Nemeio.Core*, it also depends on *Nemeio.API* due to its HttpComm section, which may be somewhat limited here...
It may be a good idea to move the HttpComm section to the *API* module or another dedicated third-party module.

The primary content of the ACL module relates to serial communication between the software and the keyboard (and could therefore be renamed to *Nemeio.Serial*).
It heavily depends on various serial communication modules common to all platforms (*SerialPortStream*, *SerialPortLib*, and *System.IO.Ports*).

A key component of this module is the ***SerialPortComm*** class, which acts as the central interface between the software and the keyboard (*WriteAsync* and *Read*).

<h4>Nemeio.Presentation</h4>

The *Presentation* module aims to provide a shared presentation layer for all platforms.

It embeds a copy of the *Configurator*, retrieved as a submodule and distributed with the disembarked system. Additionally, it defines dependencies on:
* The *MvvmCross* framework, for which it specifies the ***Application*** class (this dependency may be reduced over time, possibly transitioning to *MvvmLight*?)
* The *SeriLog* framework, which handles the application's logging mechanism (*see* the ***Logger*** class)

<h3>1.4.2. Windows Modules<a name="windows_modules"></h3>

![Nemeio Windows Modules](./PlantUML/01-Organization-02-WindowsModules.png)

The Windows version of the disembarked system is based on the previously mentioned shared modules, nearly all of which are referenced by the new *Nemeio.Wpf* module specific to Windows. The only exception is *Nemeio.Api*, which is not directly referenced.

As the name suggests, this module adapts the software to the Windows platform, particularly by implementing the graphical interface using WPF (*Windows Presentation Framework*).<br>
This WPF adaptation utilizes the ***NotifyIcon*** class from the third-party dependency *Hardcodet.NotifyIcon.Wpf*, as WPF does not natively provide an equivalent of the *TaskBarIcon* class available in *Windows Forms*.

Besides adapting the software to the Windows graphical interface, this module also handles mechanisms such as:
* USB port manipulation (*WMI* for *Windows Management Instrumentation*)
* Bluetooth connection management (*UWP* for *Universal Windows Platform*)
* Monitoring Bluetooth connection status
* Managing layouts specific to the Windows system
* Tracking active applications
* Monitoring user sessions (*lock/unlock*)
* etc.

The *Nemeio.Wpf* project is based on *.NET Framework 4.6.1*.

<h3>1.4.3. macOS Modules<a name="macOS_modules"></h3>

![Nemeio macOS Modules](./PlantUML/01-Organization-03-macOSModules.png)

Similarly, the *macOS* version developed under *Visual Studio Code* has the same direct dependencies, along with an additional dependency on an *XCode* project located in the *WKeyboard* folder.

*WKeyboard* defines specific functionalities in *XCode* that are then re-imported using the *Dllimport* mechanism.

![Nemeio WKeyboard XCode Module](./PlantUML/01-Organization-03-macOSModules-01-WKeyboard.png)

The *Nemeio.Mac* project is defined as a *Xamarin macOS* project.

<h3>1.4.4. Test Modules<a name="test_modules"></h3>

For each C# module listed above (shared, Windows, or macOS), a dedicated unit test project is created.

![Nemeio Test Modules](./PlantUML/01-Organization-04-TestModules.png)

Some test projects also depend on *Nemeio.Core.Test*, as it defines *Fake* objects that implement certain behaviors needed for testing.<br>
This practice should be minimized in favor of using *mocked* objects whenever possible.

***Best Practices for Naming Tests***

***FixMe:*** https://adeneo-embedded.atlassian.net/browse/BLDLCK-2367

Test names, based on NUnit, should ideally follow this syntax (all in one line):

1. <class being tested>_
2. [<optional method organizer>_]
3. <method being tested>_
4. [<optional condition organizer>_]
5. <test condition>_
6. <expected test result>

The optional "organizers" ensure that a class is tested in the correct order, from its constructor to its more complex methods. This improves readability and makes it easier to understand that a failure in a constructor may cause subsequent methods to fail.

Examples of test names:

* *SerialPortComm_01_Constructor_01_InvalidArguments_Throws*
* *SerialPortComm_01_Constructor_02_ValidArguments_BuildOk*
* *SerialPortComm_02_StartAsync_01_SerialPortWontOpen_ReturnFalse*
* etc.

<h2>1.5. Installers<a name="installers"></h2>

<h3>1.5.1. Windows Installation<a name="installers_windows"></h3>

Two Windows installers are considered:
* The main application installer, *Nemeio.WindowsInstaller*, which includes a sub-project embedding *CustomActions*.
* The Auto-Installer, which is embedded in the keyboard and allows automatic downloading or updating when the USB is connected.

<h4>1.5.1.1 Windows Installer<a name="installers_windows_regular"></h4>

The Windows installer is built using the third-party *WIX* tool, which provides a simplified XML description of the installation process.

- <h5>Custom Actions</h5>

Originally, *CustomActions* installed and uninstalled the scheduled task allowing the auto-start of the disembarked system, but this mechanism was replaced by simply placing a *Nemeio* shortcut in the Windows startup menu (*StartUp Apps*).

One remaining mechanism is cleaning up the following folders:
* "*Application Data*", which primarily contains the database.
* "*Program Files*", which includes an *implicit* installation of Chromium that can cause problems during uninstallation.

In principle, native *WIX* mechanisms (or even *Windows Installer* mechanisms) should handle such cleanup without needing *Custom Actions*.

- <h5>WIX Installer</h5>

The main installation project in *Nemeio.WindowsInstaller* is described by the *Product.wxs* file and the *CommonComponentGroup* defined in *Common.wxs*.

Two separate installers are created based on the target platforms: <*x86*> and <*x64*>, both built in *Release* mode.

Additional dependencies are generated during *pre-build* using the *Heat* tool:
* *CommonDependencies.wxs*
* *Configurator.wxs*
* *x64.wxs* or *x86.wxs* depending on the platform.

When dependencies in any of these submodules change, the associated <*wxs*> file needs to be regenerated. This is done by deleting the file in the *Nemeio.WindowsInstaller* folder. Upon detection of the missing file during pre-build, *Heat* automatically regenerates it with new <Guids> for each dependency.

- <h5>Version Number</h5>

To facilitate installer management, the version number is explicitly added to the installer name. 
This is achieved by adding the following section in the <*wixproj*> file:

```xml
  <Target Name="BeforeBuild">
    <GetAssemblyIdentity AssemblyFiles="..\Nemeio.Wpf\bin\$(Platform)\$(Configuration)\Nemeio.exe">
      <Output TaskParameter="Assemblies" ItemName="AssemblyInfo" />
    </GetAssemblyIdentity>
    <CreateProperty Value="$(SolutionName)-$(Platform)-%(AssemblyInfo.Version)">
      <Output TaskParameter="Value" PropertyName="TargetName" />
    </CreateProperty>
  </Target>
```

- <h5>Digital Signature</h5>

The digital signature of the program and its installers is performed only on the Azure server. This ensures greater security for the signing certificate and its associated password.

However, this requires breaking down the project into finer steps:
1. Generate the executable and sign it along with its associated modules (*Assemblies*).
2. Once all files are signed, generate the installer using the signed modules (without regenerating them).

<h4>1.5.1.2 Windows Auto-Installer<a name="installers_windows_auto"></h4>

The Windows auto-installer is designed to download the latest version of the application associated with the current keyboard. It is made as lightweight as possible because it is embedded in the keyboard's persistent memory. It currently occupies around <*50 KB*> for Windows.

The keyboard's serial number is retrieved via USB, and the mini-installer queries the update server using this serial number to fetch the version associated with the keyboard. This allows test or development versions to be linked to specific keyboards.

Eventually, this could also facilitate phased updates according to a configurable strategy.
For example, in the first week, only keyboards with serial numbers ending in 1 or 2 receive the update, with additional serial numbers included in subsequent weeks...

This would help balance the load on the update server, preventing unnecessary over-provisioning.

Additionally, the auto-installer includes a progress bar to improve user experience during downloads.

<h3>1.5.2. macOS Installation<a name="installers_macOS"></h3>

Similarly to Windows, the macOS platform requires two separate distribution projects:

<h3>1.5.2.1. macOS Package<a name="installers_macOS_regular"></h4>

The macOS package in <*.dmg*> format allows for a simplified application installation without using the *Apple Store*, which would be the standard distribution method.
The package allows for installation via a drag-and-drop mechanism.

To generate the package, use the following commands from a terminal in the *Nemeio.MacInstaller* folder:

```sh
$ ./build.sh
```

Followed by:

```sh
$ ./dmg-maker.sh
```

<h4>1.5.2.2. macOS Auto-Installer<a name="installers_macOS_auto"></h4>

The macOS auto-installer follows the same principle of size minimization due to its presence in the keyboard’s limited memory.
It currently occupies around <*80 KB*>.

It is contained within the *Nemeio.MacAutoInstaller* folder and developed using *XCode*.

<h2>1.6. Pipelines<a name="pipelines"></h2>

The organization of the *Pipelines* folder aims to version as much as possible the various pipelines
set up for CI/CD integration on the project's Azure server:

> https://dev.azure.com/devops0872/B2047%20-%20LDLC%20-%20Karmeliet

The server contains various pipelines with a <*.yaml*> extension, categorized into 4 to 5 groups:

* **Common Pipelines** (prefixed by *[Common]* or *common-*)
  * ***[Common] Unit Tests*** - A pipeline for unit tests triggered on every release, pull request creation, or PR merge.
  * ***[Common] Installers*** - A pipeline for generating installers (Windows x64, macOS, and Windows x86).
  * ***[Common] Auto-Installers Iso Builder*** - A pipeline that creates the ISO deployed on the keyboard, embedding the Windows and macOS auto-installers.
  * ***[Common] Update Inquiry*** - < ... ***FixMe***: To be completed ... >
  * ***[Common] Sonar Cloud*** - < ... ***FixMe***: To be completed ... >
* **Windows-Specific Pipelines** (prefixed by *[Windows]* or *win-*)
  * ***[Windows] Application*** - A pipeline that generates the Windows *<msi>* installer.
* **macOS-Specific Pipelines** (prefixed by *[Mac]* or *mac-*)
  * ***[Mac] Application*** - A pipeline that generates the macOS *<dmg>* package.
* **Utility Pipelines** (prefixed by *[Utils]* or *utils-*)
  * ***[Utils] Release Notification*** - Notifies Slack of new version availability.
* **Development Pipelines** (prefixed by *[Old]*)
  * These pipelines are not persisted in the repository but exist only on the server to avoid disrupting standard CI/CD mechanisms.

All these pipelines are defined at the root of the *<Pipeline>* folder and, when needed, use templates stored in the *<Templates>* subfolder.

Certain ***Jobs*** leverage scripts. ***Bash*** scripts are preferred whenever possible, but Python scripts may also be used (requiring Python to be installed on the CI server).<br>
Dedicated subfolders organize these scripts based on their functionality, such as <*Versioning*>, <*Publishing*>, etc.

<h2>1.7. Documentation<a name="documentation"></h2>

The *<Documentation>* folder contains versioned, embedded documentation.<br>
The current format is *Markdown* with the *<.md>* extension.

Diagrams are created using *PlantUML* in *<.puml>* format and stored in the *<PlantUML>* subfolder. PNG images are ignored in version control and must be explicitly generated via the *PlantUML.jar* application.

