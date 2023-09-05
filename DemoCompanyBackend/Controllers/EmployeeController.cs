using DemoCompanyBackend.DataTransferObjects;
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
    }
}
