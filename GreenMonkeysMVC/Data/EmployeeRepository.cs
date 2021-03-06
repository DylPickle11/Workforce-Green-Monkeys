﻿
using System.Collections.Generic;
using System.Data.SqlClient;
using GreenMonkeysMVC.Models;


namespace GreenMonkeysMVC.Data
{
    /// <summary>
    ///  An object to contain all database interactions.
    /// </summary>
    public class EmployeeRepository
    {
        /// <summary>
        ///  Represents a connection to the database.
        ///   This is a "tunnel" to connect the application to the database.
        ///   All communication between the application and database passes through this connection.
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                // This is "address" of the database
                // "Data Source=localhost\\SQLEXPRESS;Initial Catalog=DepartmentsEmployees;Integrated Security=True";
                string _connectionString = "Server=localhost\\SQLEXPRESS;Database=BangazonWorkforce;Trusted_Connection=True;";
                return new SqlConnection(_connectionString);
            }
        }

        /// <summary>
        ///  Returns a list of all employees in the database
        /// </summary>
        public List<Employee> GetAllEmployees()
        { 
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block   doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, DepartmentId, Email, 
                                        IsSupervisor, ComputerId FROM Employee";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the departments we retrieve from the database.
                    List<Employee> employees = new List<Employee>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                            employees.Add(new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            });
                        }
                        reader.Close();
                        return employees;
                }
            }
        }





    /// <summary>
    ///  Returns a single department with the given id.
    /// </summary>
    public Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, DepartmentId, Email, IsSupervisor, ComputerId FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                        };
                    }

                    reader.Close();

                    return employee;
                }
            }
        }


        

        // ||||||||||||||||||||||||| GetAllEmployeesWithDepartment ||||||||||||||||||||||||||||||

        /// <summary>
        ///  Returns a list of all employees in the database
        /// </summary>
        public List<Employee> GetAllEmployeesWithDepartment()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block   doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, FirstName, LastName, DepartmentId, Email, IsSupervisor, ComputerId FROM Employee";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the departments we retrieve from the database.
                    List<Employee> employees = new List<Employee>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "DeptName" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int firstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string firstNameValue = reader.GetString(firstNameColumnPosition);

                        int lastNameColumnPosition = reader.GetOrdinal("LastName");
                        string lastNameValue = reader.GetString(lastNameColumnPosition);

                        int departmentIdColumnPosition = reader.GetOrdinal("DepartmentId");
                        int departmentIdValue = reader.GetInt32(departmentIdColumnPosition);

                        int emailColumnPosition = reader.GetOrdinal("Email");
                        string emailValue = reader.GetString(emailColumnPosition);

                        int isSupervisorColumnPosition = reader.GetOrdinal("IsSupervisor");
                        bool isSupervisorValue = reader.GetBoolean(isSupervisorColumnPosition);

                        int computerIdColumnPosition = reader.GetOrdinal("ComputerId");
                        int computerIdValue = reader.GetInt32(computerIdColumnPosition);

                        DepartmentRepository departmentRepo = new DepartmentRepository();
                        Department singleDepartment = departmentRepo.GetDepartmentDetails(departmentIdValue);

                        // Now let's create a new department object using the data from the database.
                        Employee employee = new Employee
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            DepartmentId = departmentIdValue,
                            ComputerId = computerIdValue,
                            Email = emailValue,
                            IsSupervisor = isSupervisorValue,
                            Department = singleDepartment
                        };

                        // ...and add that department object to our list.
                        employees.Add(employee);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of departments who whomever called this method.
                    return employees;
                }
            }
        }

        

        // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

        /// <summary>
        ///  Add a new department to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void AddEmployee(Employee employee)
        {
            using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSupervisor, ComputerId)
                                            VALUES (@firstName, @lastName, @departmentId, @isSupervisor, @computerId)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));

                        cmd.ExecuteNonQuery();
                    }
                }
        }

        /// <summary>
        ///  Updates the department with the given id
        /// </summary>
        public void UpdateEmployee(int id, Employee employee)
        {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE Employee
                                            SET FirstName = @firstName, 
                                                LastName = @lastName, 
                                                DepartmentId = @departmentId,
                                                Email = @email,
                                                IsSupervisor = @isSupervisor,
                                                ComputerId = @computerId
                                            WHERE Id = @id";

                            cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                            cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                            cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                            cmd.Parameters.Add(new SqlParameter("@email", employee.Email));
                            cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                            cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            cmd.ExecuteNonQuery();
                        }
                    }
        }

        /// <summary>
        ///  Delete the department with the given id
        /// </summary>
        public void DeleteEmployee(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}