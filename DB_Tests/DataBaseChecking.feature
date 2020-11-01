Feature: DataBaseChecking
	In order to be in touch with customers
	As a marketer
	I want the data of new users to be saved in the database

	In order to avoid database anomalies
	As a developer 
	I want to have unique IDs for all unique users

	Background: 
		Given Connection to Shop database is established

@AddUserData
Scenario: Adding new user data to database
	Given Script for adding a new user data ready
	When I send request to database
	When I send request to get data of new user
	Then I get response with data of added user

@negative
Scenario: Adding order with invalid user
	When I send request to database to get data of users
	When I prepared script for creating order to non-existing user
	When I send invalid request to database 
	Then I get error message
	When I get Orders table from database
	Then Order with invalid user data wasn't created

@negative
Scenario: Updating order with invalid user ID
	When I send request to database to get data of users 
	When I prepared script for updating order on non-existing user
	When I send invalid request to database 
	Then I get error message
	When I get Orders table from database
	Then Order wasn't updated with invalid data

@userDataUpdates
Scenario Outline: Editting user personal information
	When I send request to database to get data of users 
	When I prepared script for updating user <personal> information
	When I send request to database
	When I send request to database to get data of users
	Then User <personal> information changed in database
	Examples: 
	| personal  |
	| FirstName |
	| LastName  |
	| Age       |
	| City      |

@negative
Scenario: Checking uniques of user IDs
	When I send request to database to get data of users 
	Then I see that all IDs is unique
