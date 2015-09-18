# NUnit.XForms

NUnit.XForms is a test runner for Xamarin Forms applications. It gives you much more flexibility for running tests, than the built-in Xamarin NUnitLite test project. It works with both NUnit and NUnitLite.

To use NUnit.XForms, you should be familiar with Xamarin Forms: http://developer.xamarin.com/guides/cross-platform/xamarin-forms/

For the simplest use-case, see the example below.

```csharp
using NUnit.XForms;
...

public class TestApp : Xamarin.Forms.Application
{
	public TestApp()
	{
		var runner = new TestRunner();
		// Add your test types/assemblies
		runner.Add(typeof(YourTestType));
		var page = new TestRunnerPage(runner);
		MainPage = new NavigationPage(page);
	}
}
```

### History

#### Known issues
* SetupFixture, TearDownFixture not supported yet

#### Version 0.2.0
* Supports NUnitLite also
* Tests and fixtures can be selected individually for run

#### Version 0.1.0
* First public version
* Supports asynchronous tests

Icon of NUnit.XForms is created by [Icons8](http://www.icons8.com)
