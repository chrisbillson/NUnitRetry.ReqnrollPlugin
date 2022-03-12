# NUnitRetry.SpecFlowPlugin
## About

NUnitRetry SpecFlow Plugin is the newest approach to adding "Nunit.Framework.Retry" attribute to SpecFlow's generated tests. It's intention is to mimic SpecFlow+ Runner's re-running abilities. It's main features will be:
* "Retry" and "Retry(n)" tag on Feature/Scenario/Scenario Outline level - adds "Nunit.Framework.Retry" attribute to given test with default value or "n"
* Adding "Nunit.Framework.Retry" to each scenario in the project with default max retries value
* Ability to set default max retries value in specflow.json
* Ability to add "Nunit.Framework.Retry" to each scenario, without adding any tag to scenario/feature; value will be based on default value from config
* Prioritisation - Global setting-> Feature level setting -> Scenario level setting

**Please note, that this plugin is currently in development state.**

## Installation - dev state

As this plugin is currently in development state, the installation requires you to follow this steps:
1. Download the repo.
2. Open NunitRetrySpecFlow in VS.
3. Publish it somewhere locally as a NuGet package.
4. Include the NuGet package to target project from your local directory ( https://stackoverflow.com/questions/10240029/how-do-i-install-a-nuget-package-nupkg-file-locally ).

Example usage is presented in included **NunitRetrySpecFlowTests**. 

## Roadmap

- ✅ Add "Nunit.Framework.Retry" attribute with default value when "Retry" tag is found on scenario level
- ✅ Add "Nunit.Framework.Retry" attribute with N-value when "Retry(N)" tag is found on scenario level
- ✅ Get default max retries value from specflow.json file
- ✅ Add "Nunit.Framework.Retry" attribute with default value when "Retry" tag is found on feature level
- ✅ Add "Nunit.Framework.Retry" attribute with N-value when "Retry(N)" tag is found on feature level
- ✅ Add "Nunit.Framework.Retry" to each scenario in the project with default max retries value
- ✅ Give ability to add "Nunit.Framework.Retry" globally, even when "Retry" tag is missing
- ❌ Code cleanup
- ❌ Implement logic when specflow.json is not present
- ❌ Implement logic when configuration in specflow.json is not present

## Installation 
1. Include the NuGet package to target project.
2. Add specflow.json to your project.
3. Include following section to specflow.json:
```json
"NRetrySettings": {
    "maxRetries": 3,
    "applyGlobally": true
  }
```






# OG STUFF FROM JOSH KEEGAN
# xRetry
Retry flickering test cases for xUnit and SpecFlow in dotnet.

[![pipeline status](https://github.com/JoshKeegan/xRetry/actions/workflows/cicd.yaml/badge.svg)](https://github.com/JoshKeegan/xRetry/actions)

## When to use this
This is intended for use on flickering tests, where the reason for failure is an external 
dependency and the failure is transient, e.g:
 - HTTP request over the network
 - Database call that could deadlock, timeout etc...

Whenever a test includes real-world infrastructure, particularly when communicated with via the
internet, there is a risk of the test randomly failing so we want to try and run it again. 
This is the intended use case of the library.  

If you have a test that covers some flaky code, where sporadic failures are caused by a bug, 
this library should **not** be used to cover it up!

## Usage: SpecFlow 3
Add the `xRetry.SpecFlow` nuget package to your project.  

### Scenarios (and outlines)
Above any scenario or scenario outline that should be retried, add a `@retry` tag, e.g:
```gherkin
@retry
Scenario: Retry three times by default
	When I increment the default retry count
	Then the default result should be 3
```
This will attempt to run the test until it passes, up to 3 times by default. 
You can optionally specify a number of times to attempt to run the test in brackets, e.g. `@retry(5)`.  

You can also optionally specify a delay between each retry (in milliseconds) as a second 
parameter, e.g. `@retry(5,100)` will run your test up to 5 times, waiting 100ms between each attempt.  
Note that you must not include a space between the parameters, as Cucumber/SpecFlow uses
a space to separate tags, i.e. `@retry(5, 100)` would not work due to the space after the comma.


### Features
You can also make every test in a feature retryable by adding the `@retry` tag to the feature, e.g:
```gherkin
@retry
Feature: Retryable Feature

Scenario: Retry scenario three times by default
	When I increment the retry count
	Then the result should be 3
```
All options that can be used against an individual scenario can also be applied like this at the feature level.  
If a `@retry` tag exists on both the feature and a scenario within that feature, the tag on the scenario will take
precedent over the one on the feature. This is useful if you wanted all scenarios in a feature to be retried 
by default but for some cases also wanted to wait some time before each retry attempt. You can also use this to prevent a specific scenario not be retried, even though it is within a feature with a `@retry` tag, by adding `@retry(1)` to the scenario.

## Usage: xUnit
Add the `xRetry` nuget package to your project.

### Facts
Above any `Fact` test case that should be retried, replace the `Fact` attribute, with 
`RetryFact`, e.g:
```cs
private static int defaultNumCalls = 0;

[RetryFact]
public void Default_Reaches3()
{
    defaultNumCalls++;

    Assert.Equal(3, defaultNumCalls);
}

```
This will attempt to run the test until it passes, up to 3 times by default. 
You can optionally specify a number of times to attempt to run the test as an argument, e.g. `[RetryFact(5)]`.  

You can also optionally specify a delay between each retry (in milliseconds) as a second 
parameter, e.g. `[RetryFact(5, 100)]` will run your test up to 5 times, waiting 100ms between each attempt.


### Theories
If you have used the library for retrying `Fact` tests, using it to retry a `Theory` should be very intuitive.  
Above any `Theory` test case that should be retried, replace the `Theory` attribute with `RetryTheory`, e.g:
```cs
// testId => numCalls
private static readonly Dictionary<int, int> defaultNumCalls = new Dictionary<int, int>()
{
    { 0, 0 },
    { 1, 0 }
};

[RetryTheory]
[InlineData(0)]
[InlineData(1)]
public void Default_Reaches3(int id)
{
    defaultNumCalls[id]++;

    Assert.Equal(3, defaultNumCalls[id]);
}
```
The same optional arguments (max attempts and delay between each retry) are supported as for facts, and can be used in the same way.

### Skipping tests at Runtime
In addition to retries, `RetryFact` and `RetryTheory` both support dynamically skipping tests at runtime. To make a test skip just use `Skip.Always()`
within your test code.  
It also supports custom exception types so you can skip a test if a type of exception gets thrown. You do this by specifying the exception type to the 
attribute above your test, e.g.
```cs
[RetryFact(typeof(TestException))]
public void CustomException_SkipsAtRuntime()
{
    throw new TestException();
}
```
This functionality also allows for skipping to work when you are already using another library for dynamically skipping tests by specifying the exception
type that is used by that library to the `RetryFact`. e.g. if you are using the popular Xunit.SkippableFact nuget package and want to add retries, converting the 
test is as simple as replacing `[SkippableFact]` with `[RetryFact(typeof(Xunit.SkipException))]` above the test and you don't need to change the test itself.

## Viewing retry logs
By default, you won't see whether your tests are being retried as we make this information available 
via the xunit diagnostic logs but test runners will hide these detailed logs by default.  
To enable them you must configure your xUnit test project to have `diagnosticMessages` set to `true` in the `xunit.runner.json`. 
See the [xUnit docs](https://xunit.net/docs/configuration-files) for a full setup guide of their config file, or see
this projects own unit tests which has been set up with this enabled.

## Contributing
Feel free to open a pull request! If you want to start any sizeable chunk of work, consider 
opening an issue first to discuss, and make sure nobody else is working on the same problem.  

### Developing locally
#### In an IDE
To build and run locally, always build `xRetry.SpecFlowPlugin` with the Release profile before the tests to ensure MSBuild uses the latest version of your changes when building the UnitTests project.  

#### From the terminal
If you install `make` and go to the `build` directory, you can run the following command to run CI locally (run lint, build, run tests and create the nuget packages):
```bash
make ci
```
If that works, all is well!

### Code formatting
Code formatting rules followed for xRetry are fairly standard for C# and are enforced during CI via `dotnet format`. You can see any non-standard rules in the [.editorconfig](.editorconfig) file. If you find your build fails due to this lint check, you can fix all formatting issues by running the `dotnet format` command from the root of the project.

## Licence
[MIT](LICENSE)
