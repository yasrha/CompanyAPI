using DemoCompanyBackend.DataTransferObjects;
using DemoCompanyBackend.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace DemoCompanyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration; // Used to retrieve configuration settings from the apps config.
        private readonly IWebHostEnvironment _env; // Provides information about the web hosting enviornment.

        // Constructor for the controller. 
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        /**
         * HTTP GET request.
         * Handle GET requests to retrieve data from the "Employee" table.
         * 
         * Steps:
         * 1. SQL Query: Define the SQL query to retrieve the data needed from the database.
         * 
         * 2. Configuration Settings: Obtain the database connection string and other configuration settings 
         *    from the app settings or configuration file. This information is essential for connecting to the database.
         *    
         * 3. Database Connection: Create and open a connection to the database using the connection string.
         * 
         * 4. Database Command: Create a command object (MySqlCommand) that represents the SQL query to execute. This 
         *    command is associated with the open database connection.
         *    
         * 5. Data Retrieval: Execute the command, which retrieves the data from the database. The results are obtained 
         *    as a MySqlDataReader.
         *    
         * 6. Data Loading: Load the data from the MySqlDataReader into a DataTable or another suitable data structure, 
         *    effectively converting it into a structured format that can be easily serialized to JSON.
         *    
         * 7. Response: Return the structured data (in this case, the DataTable) as JSON in the HTTP response.
         */
        [HttpGet]
        public JsonResult Get()
        {
            // The SQL query to retrieve the information about the employee
            string query = @"
                        select EmployeeId,EmployeeName,Department,
                        DATE_FORMAT(DateOfJoining,'%Y-%m-%d') as DateOfJoining,
                        PhotoFileName
                        from 
                        Employee
            ";

            DataTable table = new DataTable(); // Create a datatable where we will store the data retrieved from the database
            // Obtain the connection string from the configuration settings.
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            // Use MySqlDataReader to populate the data into a data table.
            MySqlDataReader myReader;

            // Return this DTO object
            var employeeList = new List<EmployeeDto>();

            // Connect to the database
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();

                // Execute the query command in the mycon database
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    // Execute the query against the open databse connection.
                    // myReader will be used to read the results of the query row by row.
                    myReader = myCommand.ExecuteReader();
                    // Load the data from myReader into the datatable table, populating the table with the query results.
                    table.Load(myReader);

                    // Data Transfer Object to avoid serialization issues with the JsonResult
                    foreach (DataRow row in table.Rows)
                    {
                        employeeList.Add(new EmployeeDto
                        {
                            EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                            EmployeeName = row["EmployeeName"].ToString(),
                            Department = row["Department"].ToString(),
                            DateOfJoining = row["DateOfJoining"].ToString(),
                            PhotoFileName = row["PhotoFileName"].ToString()
                        });
                    }

                    myReader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult(employeeList);
        }

        /**
         * HTTP Post request
         * 
         * Designed to insert a new employee into a database using the data provided in the HTTP request body.
         * 
         * Steps:
         * 1. SQL Query: Define the SQL query that performs the insertion into the database.
         * 
         * 2. Configuration Settings: Obtain the database connection string and other configuration settings 
         *    from the app settings or configuration file. This information is essential for connecting to the database.
         *    
         * 3. Database Connection: Create and open a connection to the database using the connection string.
         * 
         * 4. Database Command: Create a command object (MySqlCommand) that represents the SQL query to execute. This 
         *    command is associated with the open database connection.
         *    
         * 5. Set Parameters: If your SQL query includes any parameter(s), set the parameter values. This is an important 
         *    step that can prevent potential SQL injection attacks. Parameterized queries inherently prevent SQL injection 
         *    attacks by ensuring that user input is never executed as SQL code.
         *    
         * 6. Execute the Query: Execute the SQL query using ExecuteNonQuery() for an INSERT statement. This method is used 
         *    because it doesn't return data, it just executes the query and returns the number of affected rows.
         * 
         * 7. Response: Return an appropriate response to the client.
         */

        [HttpPost]
        public IActionResult Post(Employee emp)
        {
            // SQL query to to insert a new employee
            string query = @"
                        insert into Employee 
                        (EmployeeName,Department,DateOfJoining,PhotoFileName) 
                        values
                         (@EmployeeName,@Department,@DateOfJoining,@PhotoFileName) ; 
            ";

            // Obtain the connection string from the configuration settings.
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

            // Connect to the database
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                // Send the query to the mycon database
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    // Add the following paramters into the SQL query
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);

                    // Execute the query
                    myCommand.ExecuteReader();

                    mycon.Close();
                }
            }

            return Ok("Added Successfully");
        }

        /**
         * HTTP PUT request
         * 
         * This will update an existing employee in a database based on the data provided in the HTTP request body.
         * 
         * Steps:
         * 1. SQL Query: Define the SQL query that performs the updating of an existing employee.
         * 
         * 2. Configuration Settings: Obtain the database connection string and other configuration settings 
         *    from the app settings or configuration file. This information is essential for connecting to the database.
         *    
         * 3. Database Connection: Create and open a connection to the database using the connection string.
         * 
         * 4. Database Command: Create a command object (MySqlCommand) that represents the SQL query to execute. This 
         *    command is associated with the open database connection.
         *    
         * 5. Execute the Query: Execute the SQL query using ExecuteNonQuery(). This method is used for SQL statements like 
         *    UPDATE that don't return a result set but perform database modifications. The parameters' values are used to 
         *    safely update the database record.
         *    
         * 6. Response: Return an appropriate response to the client. 
         */
        [HttpPut]
        public IActionResult Put(Employee emp)
        {
            // SQL query to update an employee
            string query = @"
                        update Employee set 
                        EmployeeName =@EmployeeName,
                        Department =@Department,
                        DateOfJoining =@DateOfJoining,
                        PhotoFileName =@PhotoFileName
                        where EmployeeId=@EmployeeId;
            ";

            // Obtain the connection string from the configuration settings.
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

            // Connect to the database
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                // Send the query to the mycon database
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    // Update the values
                    myCommand.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);

                    myCommand.ExecuteReader(); // Execute the query

                    mycon.Close();
                }
            }

            return Ok("Updated Successfully");
        }

    }
}
