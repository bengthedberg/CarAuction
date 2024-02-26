


**Run locally using docker**

`docker compose up -d`

## Technologies used

* [NET Core 8]()
* [Entity Framework]()
* [Docker]()
* [PostgresDb]()
* [MongoDb]()
* [Masss Transit]()
* [Rabbit MQ]()
* [Identity Server](https://docs.duendesoftware.com/identityserver/v7) JWT token
* [Microsoft YARP](https://microsoft.github.io/reverse-proxy/) Reverse Proxy as Gateway Service so it provides a single endpoint for all services. Security as part of the request, URL rewriting. Load balancing and caching.


## Test

### Unit Tests

**Unit test should test one thing**

* Fast, test should complete quickly, (think milliseconds rather than second).
* Isolated, no dependency of execution order.
* Repeatable, should be able to run over and over with the same result.
* Self-Checking, no human interaction.
* Timely, should not take longer to write the test than it took to write the code.

Naming convention of unit tests:
`MethodToTest_Scenario_ExpectedResult`

### Integration Test

Should test how separate part of application work together, without the need of external services like databases.

We use test doubles instead of the real services, as our services still dependent on these external services. We need to control the behaviour of this dependencies by using any of the following:

* Fake, has working implementation. For example a in memory database.
* Mock, has a mock implementation that is programmed to return specific behaviour.
* Stub, just return a predefined value.

For our tests we will use:

* [XUnit](https://xunit.net/)
* [Moq](https://github.com/devlooped/moq/wiki)
    * [Video](https://learn.microsoft.com/en-us/shows/visual-studio-toolbox/unit-testing-moq-framework)