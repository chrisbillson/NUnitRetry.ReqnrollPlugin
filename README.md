[![CI](https://github.com/chrisbillson/NUnitRetry-ReqnrollPlugin/actions/workflows/cicd.yml/badge.svg?branch=master)](https://github.com/chrisbillson/NUnitRetry-ReqnrollPlugin/actions/workflows/cicd.yml)
[![NuGet](https://img.shields.io/nuget/v/NUnitRetry.ReqnrollPlugin.svg)](https://www.nuget.org/packages/NUnitRetry.ReqnrollPlugin)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NUnitRetry.ReqnrollPlugin.svg)](https://www.nuget.org/packages/NUnitRetry.ReqnrollPlugin)

# NUnitRetry.ReqnrollPlugin
## About

Reqnroll conversion of NUnitRetry.SpecFlowPlugin by Piotr Niedzialek (https://github.com/farum12/NUnitRetry.SpecFlowPlugin)

NUnitRetry plugin is the newest approach to applying retries to your tests in Reqnroll. It allows your generated tests to be automatically retried when required up to _n_ number of times. It's based on Josh Keegan's xRetry (https://github.com/JoshKeegan/xRetry). It's intention is to mimic SpecFlow+ Runner's re-running abilities. The main features are:
* Support for "Retry" and "Retry(n)" tags at Feature/Scenario/Scenario Outline level - this adds a custom retry attribute to the given test with a default value or "n".
* Full configurability via `reqnroll.json`
	- Ability to set a default max retries value
	- Ability to apply retries globally across your whole test project without needing to add tags to specific scenarios/features
	- Logical configuration overrides - Global setting -> Feature level setting -> Scenario level setting

## "Why should I use that?"

Flaky tests. 
This plugin is here to help you with tests which occasionally fail due to transient issues which are outside your control, e.g:
 - HTTP request over the network
 - Database call that could deadlock, timeout etc...
 - random UI behaviors

Whenever a test includes real-world infrastructure, particularly when communicated with via the internet, there is a risk of the test randomly failing so we want to try and run it again.  This is the intended use case of the library.  

If you have a test that covers some flaky code, where sporadic failures are caused by a bug, this library should **not** be used to cover it up!

## Installation 
1. Include the NuGet package (https://www.nuget.org/packages/NUnitRetry.ReqnrollPlugin/) to target project.
2. Add reqnroll.json to your project **(NOTE: Without reqnroll.json, default retry values will be applied)**.
3. Include the following section to reqnroll.json:
```json
"NRetrySettings": {
    "maxRetries": 3,
    "applyGlobally": true
  }
```
4. Modify maxRetries value - sets default amount of max retries, which are applied when `@Retry` tag is used.
5. Modify applyGlobally value - sets whether test methods generated from Features/Scenarios without a tag should also be retried.

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
by default but for some cases also wanted to wait some time before each retry attempt. You can also use this to prevent a specific scenario from being retried, even though it is within a feature with a `@Retry` tag, by adding `@Retry(1)` to the scenario.

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
This will attempt to run the test until it passes, up to 3 times by default (or else, as its based on config located in reqnroll.json). 
You can optionally specify a number of times to attempt to run the test in brackets, e.g. `@Retry(5)`.  


## Contributing
Feel free to open a pull request! If you want to start any sizeable chunk of work, consider 
opening an issue first to discuss, and make sure nobody else is working on the same problem.  

## Development

1. Download the repo.
2. Open `NUnitRetry.sln` in VS.
3. Make any changes to the plugin in the `NUnitRetry.ReqnrollPlugin` project
4. Build and test your changes via the relevant `NUnitRetry` test project (these are configured to pickup the latest version of the plugin built from local source).
5. That's it!

## Roadmap

- ✅ Add "Nunit.Framework.Retry" attribute with default value when "Retry" tag is found on scenario level
- ✅ Add "Nunit.Framework.Retry" attribute with N-value when "Retry(N)" tag is found on scenario level
- ✅ Get default max retries value from reqnroll.json file
- ✅ Add "Nunit.Framework.Retry" attribute with default value when "Retry" tag is found on feature level
- ✅ Add "Nunit.Framework.Retry" attribute with N-value when "Retry(N)" tag is found on feature level
- ✅ Add "Nunit.Framework.Retry" to each scenario in the project with default max retries value
- ✅ Give ability to add "Nunit.Framework.Retry" globally, even when "Retry" tag is missing
- ✅ Code cleanup
- ✅ Implement logic when reqnroll.json is not present
- ✅ Implement logic when configuration in reqnroll.json is not present
- ✅ Implement custom retry attribute to workaround Nunit's retry limitations

## Licence
[MIT](LICENSE)
