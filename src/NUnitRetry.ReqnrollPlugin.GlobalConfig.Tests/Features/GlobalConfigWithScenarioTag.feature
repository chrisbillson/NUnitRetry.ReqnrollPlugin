Feature: Retry Tag With Global Config
	In order to allow for transient failures
	As a reqnroll Generator Plugin Developer
	I want to ensure that result is based on MaxRetries from reqnroll.json

@Retry
Scenario: Total retries equals MaxRetries from global config when no retry count specified by scenario tag
	Then the number of retries should be equal to value from config

@Retry(3)
Scenario: MaxRetries from scenario tag overrides value from global config
	Then the number of retries should be equal to 3
	And the number of retries should not be equal to value from config

# Scenario assumes ApplyGlobally is set to true in reqnroll.json.
Scenario: ApplyGlobally applies expected retries from config when no scenario tag is specified
	Then the number of retries should be equal to value from config

@Retry
Scenario Outline: Total retries equals MaxRetries from global config when invoked as scenario outline
	Then the number of retries should be equal to value from config
	# we want to invoke the scenario generator; these values don't have any input.
	Examples: 
	| someExample |
	| Yes         |
	| No          |

@Retry(3)
Scenario Outline: Scenario outline works properly with Retry tag and amount of Retries equals to parameter
	Then the number of retries should be equal to 3
	And the number of retries should not be equal to value from config
	# we want to invoke the scenario generator; these values don't have any input
	Examples: 
	| someExample |
	| Yes         |
	| No          |