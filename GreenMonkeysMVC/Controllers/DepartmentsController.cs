using System;
using System.Collections.Generic;
using GreenMonkeysMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using GreenMonkeysMVC.Data;

namespace GreenMonkeysMVC.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;
        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                string _connectionString = "Server=localhost\\SQLEXPRESS;Database=BangazonWorkforce;Trusted_Connection=True;";
                return new SqlConnection(_connectionString);
            }
        }

        // GET: Department
        public ActionResult Index()
        {
            DepartmentRepository departmentRepo = new DepartmentRepository();
            List<Department> allDepartments = departmentRepo.GetAllDepartments();

            return View(allDepartments);
        }

        // GET: Departments/Details
        public ActionResult Details(int id)
        {
            DepartmentRepository departmentRepo = new DepartmentRepository();
            Department department = departmentRepo.GetDepartmentDetails(id);

            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            {
                DepartmentRepository departmentRepo = new DepartmentRepository();
                departmentRepo.AddDepartment(department);
                return RedirectToAction(nameof(Index));
            }
        }








        //// GET: Departments
        //public ActionResult Index()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT d.[Name] AS DepartmentName, COUNT(e.Id) AS EmployeeCount, d.Budget, d.Id 
        //                                FROM Department d LEFT JOIN Employee e ON d.Id = e.DepartmentId
        //                                GROUP BY d.[Name], d.Budget, d. Id";
        //            var reader = cmd.ExecuteReader();

        //            var departments = new List<Department>();

        //            while (reader.Read())
        //            {
        //                departments.Add(new Department
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
        //                    Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
        //                    EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount"))
        //                });
        //            }
        //            reader.Close();
        //            return View(departments);
        //        }
        //    }
        //}



        // GET: Departments/Edit
        public ActionResult Edit(int id)
        {
            DepartmentRepository departmentRepo = new DepartmentRepository();
            var department = departmentRepo.GetDepartmentById(id);
            return View(department);
        }

        // POST: Departments/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Department department)
        {
            try
            {
                DepartmentRepository departmentRepo = new DepartmentRepository();
                departmentRepo.UpdateDepartment(id, department);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int id)
        {
            DepartmentRepository departmentRepo = new DepartmentRepository();
            var department = departmentRepo.GetDepartmentById(id);
            return View(department);
        }

        // POST: Departments/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Department department)
        {
            DepartmentRepository departmentRepo = new DepartmentRepository();
            departmentRepo.DeleteDepartment(department.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Private method to get a list of Employees

        private List<Employee> GetEmployeeCount()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, DepartmentId
                                       FROM Employee";

                    var reader = cmd.ExecuteReader();

                    var exercises = new List<Employee>();

                    while (reader.Read())
                    {
                        exercises.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                        });
                    }
                    reader.Close();
                    return exercises;

                }
            }
        }
    }
}