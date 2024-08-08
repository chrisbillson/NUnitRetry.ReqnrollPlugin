Feature: Retry
	In order to allow for transient failures
	As a SpecFlow Generator Plugin Developer
	I want to ensure that result is based on MaxRetries from specflow.json

@Retry
Scenario: Retry tag works and amount of Retries equals to MaxRetries from specflow.json
	When I increment the default retry count
	Then the retry result should be equal to config

@retry
Scenario: Retry tag works as lowercase and amount of Retries equals to MaxRetries from specflow.json
	When I increment the default retry count
	Then the retry result should be equal to config

@Retry(5)
Scenario: Retry tag works with parameter
	When I increment the default retry count
	Then the retry result should be 5

@retry(5)
Scenario: Retry tag as lowercase works with parameter
	When I increment the default retry count
	Then the retry result should be 5

#scenario is based on ApplyGlobbaly; if it will be false - this test should pass, as the amount of retries should equal to 1
Scenario: Global setting of ApplyGlobally works properly and amount of Retries equals to MaxRetries from specflow.json
	When I increment the default retry count
	Then the retry result should be equal to 1 or to config value

@Retry
Scenario Outline: Scenario outline works properly with Retry tag and amount of Retries equals to MaxRetries from specflow.json
	When I increment the default retry count
	Then the retry result should be equal to config

	# we want to invoke scenario generator; these values don't have any input
	Examples: 
	| someExample |
	| Yes         |
	| No          |


@Retry(5)
Scenario Outline: Scenario outline works properly with Retry tag and amount of Retries equals to parameter
	When I increment the default retry count
	Then the retry result should be 5

	# we want to invoke scenario generator; these values don't have any input
	Examples: 
	| someExample |
	| Yes         |
	| No          |