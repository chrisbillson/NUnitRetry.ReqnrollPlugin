Feature: Retry Behavior Without Reqnroll.json file
	In order to allow for the plugin to be used in step assemblies without a reqnroll.json configuration file
	As a reqnroll Generator Plugin Developer
	I want to ensure that the plugin falls back to safe defaults

Scenario: Tests should pass without retries when no retry tag is specified
	Then the test should pass without retries

@Retry(3)
Scenario: Number of retries should be equal to retries from scenario tag
	Then the number of retries should be equal to 3
