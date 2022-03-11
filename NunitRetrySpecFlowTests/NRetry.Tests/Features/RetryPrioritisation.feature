@Retry(4)
Feature: RetryPrioritisation
	In order to allow for transient failures
	As a SpecFlow Generator Plugin Developer
	I want to ensure if global-fetaure-scenario retries are working properly


Scenario: Retry 4 times when Feature file has Retry4, as it overriden configuration
	When I increment the default retry count
	Then the retry result should be 4


@Retry(6)
Scenario: Retry 6 times when Scenario has Retry 6
	When I increment the default retry count
	Then the retry result should be 6