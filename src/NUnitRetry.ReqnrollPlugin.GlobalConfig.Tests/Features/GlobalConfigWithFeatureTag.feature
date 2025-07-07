@Retry(3)
Feature: Global Config With Feature Tags
	In order to allow for transient failures
	As a reqnroll Generator Plugin Developer
	I want to ensure that retries are correctly applied based on MaxRetries from reqnroll.json

Scenario: MaxRetries from feature tag overrides value from global config
	Then the number of retries should be equal to 3
	And the number of retries should not be equal to value from config