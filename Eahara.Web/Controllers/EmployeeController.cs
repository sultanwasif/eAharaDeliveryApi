using Eahara.Model;
using Eahara.Web.Filter;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmployeeController : ApiController
    {
        [HttpPost]
        [Route("AddEmployee")]
        public bool AddEmployee(EmployeeDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var emp = context.Employees.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (dataDto != null)
                        {
                            emp.Name = dataDto.Name;
                            emp.Email = dataDto.Email;
                            emp.MobileNo = dataDto.MobileNo;
                            emp.TelephoneNo = dataDto.TelephoneNo;
                            emp.Address = dataDto.Address;
                            emp.LocationId = dataDto.LocationId;
                            emp.BankAccount = dataDto.BankAccount;
                            emp.BankName = dataDto.BankName;
                            emp.JoiningDate = dataDto.JoiningDate;
                            emp.NormalWorkingHours = dataDto.NormalWorkingHours;
                            emp.IsTemp = dataDto.IsTemp;
                            emp.IsInActive = dataDto.IsInActive;
                            emp.IsOwnEmployee = dataDto.IsOwnEmployee;

                            context.Entry(emp).Property(x => x.Email).IsModified = true;
                            context.Entry(emp).Property(x => x.Name).IsModified = true;
                            context.Entry(emp).Property(x => x.MobileNo).IsModified = true;
                            context.Entry(emp).Property(x => x.TelephoneNo).IsModified = true;
                            context.Entry(emp).Property(x => x.Address).IsModified = true;
                            context.Entry(emp).Property(x => x.BankAccount).IsModified = true;
                            context.Entry(emp).Property(x => x.BankName).IsModified = true;
                            context.Entry(emp).Property(x => x.JoiningDate).IsModified = true;
                            context.Entry(emp).Property(x => x.NormalWorkingHours).IsModified = true;
                            context.Entry(emp).Property(x => x.IsOwnEmployee).IsModified = true;
                            context.Entry(emp).Property(x => x.IsInActive).IsModified = true;
                            context.Entry(emp).Property(x => x.IsTemp).IsModified = true;

                            context.SaveChanges();
                            return true;

                        }
                        return false;
                    }
                    else
                    {
                        Employee addData = new Employee();

                        addData.Name = dataDto.Name;
                        addData.Designation = dataDto.Designation;
                        addData.Email = dataDto.Email;
                        addData.MobileNo = dataDto.MobileNo;
                        addData.TelephoneNo = dataDto.TelephoneNo;
                        addData.Address = dataDto.Address;
                        addData.LocationId = dataDto.LocationId;
                        addData.BankName = dataDto.BankName;
                        addData.BankAccount = dataDto.BankAccount;
                        addData.JoiningDate = dataDto.JoiningDate;
                        addData.IsTemp = dataDto.IsTemp;
                        addData.IsInActive = dataDto.IsInActive;
                        addData.IsOwnEmployee = dataDto.IsOwnEmployee;
                        addData.NormalWorkingHours = dataDto.NormalWorkingHours;
                        addData.IsActive = true;
                        context.Employees.Add(addData);
                        context.SaveChanges();

                        return true;
                    }
                }
            }
            return false;
        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("EmployeesInView")]

        public DataSourceResult EmployeesInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Employees.Where(x => x.IsActive == true);
                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }

                var dataSourceResult = query
                    .Select(x => new EmployeeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Designation = x.Designation,
                        Email = x.Email,
                        MobileNo = x.MobileNo,
                        TelephoneNo = x.TelephoneNo,
                        Address = x.Address,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        BankName = x.BankName,
                        BankAccount = x.BankAccount,
                        JoiningDate = x.JoiningDate,
                        IsTemp = x.IsTemp,
                        IsInActive = x.IsInActive,
                        IsOwnEmployee = x.IsOwnEmployee,
                        NormalWorkingHours = x.NormalWorkingHours,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("DeleteEmployeeById/{id}")]
        public bool DeleteEmployeeById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Employees.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                    }
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("EmployeesInDropdown")]
        public List<EmployeeDto> EmployeesInDropdown()
        {
            List<EmployeeDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Employees.Where(x => x.IsActive == true)
                    .Select(x => new EmployeeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Designation = x.Designation,
                        Email = x.Email,
                        MobileNo = x.MobileNo,
                        TelephoneNo = x.TelephoneNo,
                        Address = x.Address,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        BankName = x.BankName,
                        BankAccount = x.BankAccount,
                        JoiningDate = x.JoiningDate,
                        IsTemp = x.IsTemp,
                        IsInActive = x.IsInActive,
                        IsOwnEmployee = x.IsOwnEmployee,
                        NormalWorkingHours = x.NormalWorkingHours,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }


         [HttpGet]
        [Route("DriversInDropdown")]
        public List<EmployeeDto> DriversInDropdown()
        {
            List<EmployeeDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Employees.Where(x => x.IsActive == true && (x.Designation == "Driver" || x.Designation == "driver"))
                    .Select(x => new EmployeeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Designation = x.Designation,
                        Email = x.Email,
                        MobileNo = x.MobileNo,
                        TelephoneNo = x.TelephoneNo,
                        Address = x.Address,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        BankName = x.BankName,
                        BankAccount = x.BankAccount,
                        JoiningDate = x.JoiningDate,
                        IsTemp = x.IsTemp,
                        IsInActive = x.IsInActive,
                        IsOwnEmployee = x.IsOwnEmployee,
                        NormalWorkingHours = x.NormalWorkingHours,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("EmployeesInNoUserDropdown")]
        public List<EmployeeDto> EmployeesInNoUserDropdown()
        {
            List<EmployeeDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Employees.Where(x => x.IsActive == true && context.Users.Where(y => y.IsActive == true && y.EmployeeId == x.Id).Count() == 0);

                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }
                var data = query
                    .Select(x => new EmployeeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Designation = x.Designation,
                        Email = x.Email,
                        MobileNo = x.MobileNo,
                        TelephoneNo = x.TelephoneNo,
                        Address = x.Address,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        BankName = x.BankName,
                        BankAccount = x.BankAccount,
                        JoiningDate = x.JoiningDate,
                        IsTemp = x.IsTemp,
                        IsInActive = x.IsInActive,
                        IsOwnEmployee = x.IsOwnEmployee,
                        NormalWorkingHours = x.NormalWorkingHours,
                    }).ToList();
                DtoList = data;
            }
            return DtoList;
        }


    }
}
