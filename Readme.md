# WebJet Movie App
------------------

Build the Application in VS 2017

Please fill the value for token in 
   1. appsettings.json
   2. UnitTest.cs class (if you like to unit test the application)
   
Server side Unit testing is available. This is done via MS test tools.
   
   
There are few asumptions made here:
-----------------------------------

1. Front end development is just for testing the App with minimal style/design
2. The data server are very unstable and the Application will cache the data if necessary.
3. Movie ID across all data servers are same with the Prefix value for the provider.
4. The provider database is only updated once per day.
5. This application refresh the data from servers
	a. In every 24 hours
	b. If any server never responded with a valid data in the past 24 hours
6. Application server is implemented with page wise data load. However, it is not implemented in the client yet.
7. Tested predominantly with Chrome
