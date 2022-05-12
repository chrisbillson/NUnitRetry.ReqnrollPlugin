Feature: OtherTags
	In order to allow for transient failures
	As a SpecFlow Generator Plugin Developer
	I want to ensure that other tags won't cause Retry attribute to appear

# Launch with applyGlobally set to true or false
@OtherTag
@SomeOtherTag
@SumTaggerinio
Scenario: I will pass with other tag
	When I increment the default retry count
	Then the retry result should be equal to 1 or to config value

# Launch with applyGlobally set to true
@OtherTag
@SomeOtherTag
@Retry(5)
@SumTaggerinio
Scenario: I will pass with other tag and Retry amongst them
	When I increment the default retry count
	Then the retry result should be 5