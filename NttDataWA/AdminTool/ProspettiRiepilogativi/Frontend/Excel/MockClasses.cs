using System;
using System.Collections.Generic;

namespace ReflectionExcelReportEngine
{
    class Company
    {
        public string CompanyName { get; set; }
        public Address CompanyAddress { get; set; }
        public decimal YearSales { get; set; }
        public decimal YearProfit { get; set; }
        public List<Investor> Investors { get; set; }
        public List<Employee> Employees { get; set; }
    }

    class Investor
    {
        public string InvestorName { get; set; }
        public DateTime InvestmentDate { get; set; }
        public decimal Percentage { get; set; }
    }

    class Employee
    {
        public string EmployeeName { get; set; }
        public DateTime HiringDate { get; set; }
        public decimal Salary { get; set; }
    }

    class Address
    {
        public int StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string OfficeNumber { get; set; }
        public string CountryName { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public PhoneNumber Telephone { get; set; }
    }

    class PhoneNumber
    {
        public int CountryCode { get; set; }
        public int AreaCode { get; set; }
        public string TelephoneNumber { get; set; }
        public int Extension { get; set; }
    }

    class Summary
    {
        public string CompanyName { get; set; }
        public decimal TotalOutsideInvestmet { get; set; }
        public decimal TotalSalaryExpense { get; set; }
    }
}