using DemoCompanyBackend.DataTransferObjects;
using DemoCompanyBackend.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        /**
         * Constructor.
         * Use dependency injection to retrieve configuration settings.
         */
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /**
         * HTTP GET request.
         * Handle GET requests to retrieve data from the "Department" table.
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
            // SQL query to get the DepartmentId and DepartmentName from the Department table
            string query = @"
                        select DepartmentId,DepartmentName from 
                        Department
            ";

            DataTable table = new DataTable(); // Store the retrieved data from the database.
            // Obtain the connection string from the configuration settings.
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            // Use MySqlDataReader to populate the data into a data table.
            MySqlDataReader myReader;

            // Return this DTO object
            var departmentList = new List<DepartmentDto>();


            // Connect to the SQL database, sqlDataSource contains the server address, credentials, and database name.
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open(); // Open the database connection.

                // Create an instance of a MySqlCommand, takes in the query to be executed, and the open databse connection mycon.
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
                        departmentList.Add(new DepartmentDto
                        {
                            DepartmentId = Convert.ToInt32(row["DepartmentId"]),
                            DepartmentName = row["DepartmentName"].ToString()
                        });
                    }

                    // Closing resources.
                    myReader.Close();
                    mycon.Close();
                }
            }
            // Note: we use "using" to ensure the objects are disposed properly when they go out of scope at the end of the "using"
            // blocks. Helps manage resource efficiency and reduces the risk of resource leaks.

            return new JsonResult(departmentList);
        }

        /**
         * HTTP Post request
         * 
         * Designed to insert a new department into a database using the data provided in the HTTP request body.
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
        public IActionResult Post(Department dep)
        {
            // Insert query to indicate that we will be adding a new department into the database.
            string query = @"
                        insert into Department (DepartmentName) values
                                                    (@DepartmentName);  
            ";

            // Obtain the connection string from the configuration settings.
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

            // Open the database using MySqlConnection
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();

                // Send the query command to the database we just connected to.
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    // Add parameter DepartmentName which is equal to dep.DepartmentName, to safely insert the
                    // department name into the query to prevent an SQL injection.
                    myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);
                    myCommand.ExecuteReader(); // Execute the query

                    mycon.Close();
                }
            }

            return Ok("Added Successfully");
        }

    }
}
