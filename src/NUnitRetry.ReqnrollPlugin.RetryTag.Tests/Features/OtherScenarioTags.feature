Feature: Other Scenario Tags
	In order to allow for transient failures
	As a reqnroll Generator Plugin Developer
	I want to ensure that other tags won't cause unexpected behaviour when defined alongside NRetry tag

@Retry(3)
@Retryy(1)
@(1)Retry
@Retry[1]
@OtherTag
@SomeOtherTag
Scenario: Retry tag works correctly with other scenario tags
	Then exception failures should pass after 3 attempts
	And assertion failures should pass after 3 attempts