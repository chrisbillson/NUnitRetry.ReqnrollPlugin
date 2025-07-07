Feature: Retry With Scenario Tag

@Retry(3)
Scenario: Retry tag works with parameter for failures from assertion
	Then assertion failures should pass after 3 attempts

@Retry(3)
Scenario: Retry tag works with parameter for failures from exceptions
	Then exception failures should pass after 3 attempts

@retry(3)
Scenario: Retry tag as lowercase works with parameter
	Then exception failures should pass after 3 attempts
	And assertion failures should pass after 3 attempts

@Retry(3)
Scenario Outline: Total retries equals MaxRetries based on parameter when invoked as scenario outline
	Then exception failures should pass after 3 attempts
	And assertion failures should pass after 3 attempts
	# we want to invoke scenario generator; these values don't have any input
	Examples: 
	| someExample |
	| Yes         |
	| No          |