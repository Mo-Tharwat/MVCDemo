using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenaricRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(MVCAppDemoDbcontext dbContext) : base(dbContext)
        {
        }

        public IQueryable<Employee> GetEmployeesByAddress(string address)
        {
            return _dbContext.Employees.Where(E => E.Address == address);
        }

        public IQueryable<Employee> GetEmployeesByName(string Name)
            => _dbContext.Employees.Where(E => E.Name.ToLower().Contains(Name.ToLower()));
    }
}
