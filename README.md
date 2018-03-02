# samsara-.NET-C-
Examples of Samsara API POST and GET calls in Visual Studio / .Net / C#


Example calls made to /fleet/locations, /fleet/hos_logs and /fleet/maintenance/dvirs

Code is found in the MakeCalls() method, in MainWindow.xaml.cs

Project runs on the .Net Framework, ver 4.6.

Nuget package Microsoft.AspNet.WebApi.Client must be added to to the Visual Studio project. (This will also load Newtonsoft.Json.)

Nothing is done with the results of the calls.  You'll have to set a breakpoint and then examine the results with a Watch feature in order to see what comes back.
