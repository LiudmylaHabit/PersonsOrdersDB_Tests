using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TechTalk.SpecFlow;

namespace DB_Tests.Steps
{
    [Binding]
    public class DataBaseCheckingSteps
    {
        DataBaseMethods dbMethods;
        DataTable response;
        Dictionary<string, string> initialUserData;
        string sqlRequest;
        bool invalidRequest;
        int invalidID;

        // Background connection to database
        [Given(@"Connection to Shop database is established")]
        public void GivenConnectionToShopDatabaseIsEstablished()
        {
            dbMethods = new DataBaseMethods();
            dbMethods.ConnectToCatalog("TEST_DB");            
        }

        // Scenario - Adding new user data to database
        [Given(@"Script for adding a new user data ready")]
        public void GivenScriptForAddingANewUserDataReady()
        {
            string createTable = "CREATE TABLE TabForTests (Id INT NOT NULL, Name VARCHAR(20), Surname VARCHAR(30)) ";
            string addData = "INSERT INTO TabForTests VALUES (150, 'Test', 'User')";
            sqlRequest = createTable + addData;
        }

        // Scenario - Adding new user data to database
        // Scenario - Editting user personal information
        [When(@"I send request to database")]
        public void WhenISendRequestToDatabase()
        {
            dbMethods.Execute(sqlRequest);
        }

        // Scenario - Adding new user data to database
        [When(@"I send request to get data of new user")]
        public void WhenISendRequestToGetDataOfNewUser()
        {
            sqlRequest = "SELECT * FROM TabForTests";
            response = dbMethods.Execute(sqlRequest);
        }        

        // Scenario - Adding order with invalid user
        // Scenario - Updating order with invalid user ID
        // Scenario - Editting user personal information
        // Scenario - Checking uniques of user IDs
        [When(@"I send request to database to get data of users")]
        public void WhenISendRequestToDatabaseToGetDataOfUsers()
        {
            sqlRequest = "SELECT * FROM Persons";
            response = dbMethods.Execute(sqlRequest);            
        }

        // Scenario - Adding order with invalid user
        // Scenario - Updating order with invalid user ID
        [When(@"I prepared script for creating order to non-existing user")]
        public void WhenIPreparedScriptForCreatingOrderToNon_ExistingUser()
        {
            List<int> existingId = dbMethods.GetAllUsersID(response);
            invalidID = GetInvalidID(existingId);
            sqlRequest = $"INSERT INTO Orders(ID_order, SUM_order, ID) VALUES (37, 350, {invalidID})";
        }

        // Scenario - Adding order with invalid user
        // Scenario - Updating order with invalid user ID
        [When(@"I send invalid request to database")]
        public void WhenISendInvalidRequestToDatabase()
        {
            try
            {
                response = dbMethods.Execute(sqlRequest);
            }
            catch (SqlException ex)
            {
                invalidRequest = true;
            }
        }               

        // Scenario - Adding order with invalid user
        // Scenario - Updating order with invalid user ID
        [When(@"I get Orders table from database")]
        public void WhenIGetOrdersTableFromDatabase()
        {
            sqlRequest = "SELECT * FROM Orders";
            response = dbMethods.Execute(sqlRequest);
        }             

        //Updating order data on invalid
        [When(@"I prepared script for updating order on non-existing user")]
        public void WhenIPreparedScriptForUpdatingOrderOnNon_ExistingUser()
        {
            List<int> existingId = dbMethods.GetAllUsersID(response);
            invalidID = GetInvalidID(existingId);
            sqlRequest = $"UPDATE Orders SET ID = {invalidID} WHERE ID_order = 1)";
        }

        // Scenario - Editting user personal information
        [When(@"I prepared script for updating user (.*) information")]
        public void WhenIPreparedScriptForUpdatingUserInformation(string info)
        {
            initialUserData = new Dictionary<string, string>()
            {
                { "id", response.Rows[0][0].ToString()},
                { "name", response.Rows[0][1].ToString()},
                { "surname", response.Rows[0][2].ToString()},
                { "age", response.Rows[0][3].ToString()},
                { "city", response.Rows[0][4].ToString() }
            };
            string forUpdate = "";
            switch (info)
            {
                case "FirstName":
                    forUpdate = info + " = 'TestName'";
                    break;
                case "LastName":
                    forUpdate = info + " = 'TestSurname'";
                    break;
                case "Age":
                    int.TryParse(initialUserData["age"], out int newAge);
                    newAge += 5;
                    forUpdate = info + " = " + newAge.ToString(); ;
                    break;
                case "City":
                    forUpdate = info + " = 'NotACity'";
                    break;
                default:
                    break;
            }
            sqlRequest = $"UPDATE Persons SET " + forUpdate + $" WHERE ID = {initialUserData["id"]}";
        }

        // Scenario - Adding new user data to database
        [Then(@"I get response with data of added user")]
        public void ThenIGetResponseWithDataOfAddedUser()
        {
            int amount = response.Rows.Count;
            string respId = response.Rows[amount - 1][0].ToString();
            string respName = response.Rows[amount - 1][1].ToString();
            string respSurname = response.Rows[amount - 1][2].ToString();

            Assert.AreEqual("150", respId);
            Assert.AreEqual("Test", respName);
            Assert.AreEqual("User", respSurname);
        }        

        // Scenario - Adding order with invalid user
        // Scenario - Updating order with invalid user ID
        [Then(@"I get error message")]
        public void ThenIGetErrorMessage()
        {
            Assert.IsTrue(invalidRequest);
        }

        // Scenario - Adding order with invalid user
        [Then(@"Order with invalid user data wasn't created")]
        public void ThenOrderWithInvalidUserDataWasnTCreated()
        {
            List<int> existingId = dbMethods.GetAllUsersID(response);
            bool idExists = false;
            foreach (int id in existingId)
            {
                if (id == invalidID) idExists = true;
            }
            Assert.IsFalse(idExists);
        }        

        // Scenario - Updating order with invalid user ID
        [Then(@"Order wasn't updated with invalid data")]
        public void ThenOrderWasnTUpdatedWithInvalidData()
        {
            List<int> existingId = dbMethods.GetAllUsersID(response);
            bool changed = false;
            foreach (DataRow row in response.Rows)
            {
                if (row[0].ToString() == "1")
                {
                    if (invalidID.ToString() == row[2].ToString())
                        changed = true;
                }
            }
            Assert.IsFalse(changed);
        }

        // Scenario - Checking uniques of user IDs
        [Then(@"I see that all IDs is unique")]
        public void ThenISeeThatAllIDsIsUnique()
        {
            bool unique = true;
            List<int> existingId = dbMethods.GetAllUsersID(response);
            existingId.Sort();
            for (int i = 1; i < existingId.Count; i++)
            {
                if (existingId[i-1] == existingId[i])
                {
                    unique = false;
                    break;
                }
            }
            Assert.IsTrue(unique);
        }

        // Scenario - Editting user personal information
        [Then(@"User (.*) information changed in database")]
        public void ThenUserInformationChangedInDatabase(string info)
        {
            switch (info)
            {
                case "FirstName":
                    Assert.AreEqual("TestName", response.Rows[0][1].ToString());
                    break;
                case "LastName":
                    Assert.AreEqual("TestSurname", response.Rows[0][2].ToString());
                    break;
                case "Age":
                    int.TryParse(initialUserData["age"], out int setAge);
                    setAge += 5;
                    Assert.AreEqual(setAge.ToString(), response.Rows[0][3].ToString());
                    break;
                case "City":
                    Assert.AreEqual("NotACity", response.Rows[0][4].ToString());
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Hooks - returning the database to its original position
        /// </summary>
        [AfterScenario("userDataUpdates")]
        public void UpdateUserData()
        {
            sqlRequest = $"UPDATE Persons SET FirstName = {initialUserData["name"]} "+
                $"LastName = {initialUserData["surname"]} Age = {initialUserData["age"]} " +
                $"City = {initialUserData["city"]} WHERE ID = {initialUserData["id"]}";
        }

        [AfterScenario("AddUserData")]
        public void DeleteUserTable()
        {
            string delRequest = "DROP TABLE TabForTests";
            dbMethods.Execute(delRequest);
        }

        public int GetInvalidID(List<int> existingId)
        {
            Random rdm = new Random();
            invalidID = 0;
            bool exist = true;
            while (exist)
            {
                invalidID = rdm.Next(0, 1000);
                exist = false;
                foreach (int item in existingId)
                {
                    if (invalidID == item)
                    {
                        exist = true;
                        break;
                    }
                }
            }
            return invalidID;
        }
    }
}
