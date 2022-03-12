# NUnitRetry.SpecFlowPlugin
## About

NUnitRetry SpecFlow Plugin is the newest approach to adding "Nunit.Framework.Retry" attribute to SpecFlow's generated tests. It's based on Josh Keegan xRetry (https://github.com/JoshKeegan/xRetry). It's intention is to mimic SpecFlow+ Runner's re-running abilities. It's main features are:
* "Retry" and "Retry(n)" tag on Feature/Scenario/Scenario Outline level - adds "Nunit.Framework.Retry" attribute to given test with default value or "n"
* Adding "Nunit.Framework.Retry" to each generated test method in the project with default max retries value
* Ability to set default max retries value in specflow.json
* Ability to add "Nunit.Framework.Retry" to each test method, without adding any tag to scenario/feature; value are based on default value from config
* Prioritisation - Global setting-> Feature level setting -> Scenario level setting

## "Why should I use that?"

Flaky tests. 
This plugin is here to help you with tests,  which's reason for failure is an external dependency and the failure is transient, e.g:
 - HTTP request over the network
 - Database call that could deadlock, timeout etc...
 - random UI behaviors

Whenever a test includes real-world infrastructure, particularly when communicated with via the internet, there is a risk of the test randomly failing so we want to try and run it again.  This is the intended use case of the library.  

If you have a test that covers some flaky code, where sporadic failures are caused by a bug, this library should **not** be used to cover it up!

## Installation 
1. Include the NuGet package (https://www.nuget.org/packages/NUnitRetry.SpecFlowPlugin/3.9.58) to target project.
2. Add specflow.json to your project.
3. Include following section to specflow.json:
```json
"NRetrySettings": {
    "maxRetries": 3,
    "applyGlobally": true
  }
```
4. Modify maxRetries value - it sets default amout of max retries, which are applied when `@Retry` tag is used.
5. Modify applyGlobally value - it sets whether test methods generated from Features/Scenarios without tag should also obtain "Nunit.Framework.Retry(maxRetries)" attribute.

## Usage

### Features
You can also make every test in a feature retryable by adding the `@Retry` tag to the feature, e.g:
```gherkin
@Retry
Feature: Retryable Feature

Scenario: Retry scenario three times by default
	When I increment the retry count
	Then the result should be 3
```

```gherkin
@Retry(5)
Feature: Retryable Feature

Scenario: Retry scenario five times
	When I increment the retry count
	Then the result should be 5
```

All options that can be used against an individual scenario can also be applied like this at the feature level.  
If a `@Retry` tag exists on both the feature and a scenario within that feature, the tag on the scenario will take
precedent over the one on the feature. This is useful if you wanted all scenarios in a feature to be retried 
by default but for some cases also wanted to wait some time before each retry attempt. You can also use this to prevent a specific scenario not be retried, even though it is within a feature with a `@Retry` tag, by adding `@Retry(1)` to the scenario.

### Scenarios (and outlines)
Above any scenario or scenario outline that should be retried, add a `@retry` tag, e.g:
```gherkin
Feature: Retryable Feature

@Retry
Scenario: Retry scenario three times by default
	When I increment the retry count
	Then the result should be 3
```

```gherkin
Feature: Retryable Feature

@Retry(5)
Scenario: Retry scenario five times
	When I increment the retry count
	Then the result should be 5
```
This will attempt to run the test until it passes, up to 3 times by default (or else, as its based on config located in specflow.json). 
You can optionally specify a number of times to attempt to run the test in brackets, e.g. `@Retry(5)`.  




## Contributing
Feel free to open a pull request! If you want to start any sizeable chunk of work, consider 
opening an issue first to discuss, and make sure nobody else is working on the same problem.  

## Installation - dev state

As this plugin is currently in development state, the installation requires you to follow this steps:
1. Download the repo.
2. Open NUnitRetrySpecFlow in VS.
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
- ✅ Code cleanup
- ✅ Implement logic when specflow.json is not present
- ✅ Implement logic when configuration in specflow.json is not present

## Licence
[MIT](LICENSE)
