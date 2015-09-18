# Getting Started with NUnit.XForms

First, you have to setup Xamarin Forms. See the tutorials below to learn how to achieve this on different platforms.

Android, iOS, Windows Phone 8.0 Silverlight guide [here](http://developer.xamarin.com/guides/cross-platform/xamarin-forms/introduction-to-xamarin-forms/)

Windows Store (beta) guide [here](http://developer.xamarin.com/guides/cross-platform/xamarin-forms/windows/)

After that your only job is to setup a ```TestRunnerPage``` and navigate to it.

```csharp
using NUnit.XForms;

...

var runner = new TestRunner();
// Add your test types/assemblies
runner.Add(typeof(MyTestType));
runner.Add(typeof(MyTestType2).Assembly);
var page = new TestRunnerPage(runner);
// now you can display 'page'
```

See the samples for further code. All samples have been built using Visual Studio 2013.

Being a Portable Class Library, NUnit.XForms should work on iOS also, but has not been tested.
