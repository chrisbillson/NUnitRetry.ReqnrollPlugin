@Retry(3)
@Retryy(1)
@(1)Retry
@OtherTag
@SomeOtherTag
@SumTaggerinio
Feature: Other Fcenario Tags
	In order to allow for transient failures
	As a reqnroll Generator Plugin Developer
	I want to ensure that other tags won't cause Retry attribute to appear

Scenario: Retry tag works correctly with other feature tags
	Then exception failures should pass after 3 attempts
	And assertion failures should pass after 3 attempts