using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IEmployeeRepository EmployeeRepository { get ; set ; }
        public IDepartmentRepository DepartmentRepository { get ; set; }
        public MVCAppDemoDbcontext _Dbcontext { get; }

        public UnitOfWork(MVCAppDemoDbcontext dbcontext)
        {
            EmployeeRepository = new EmployeeRepository(dbcontext);
            DepartmentRepository = new DepartmentRepository(dbcontext);
            _Dbcontext = dbcontext;
        }

        public async Task<int> Completed()
            => await _Dbcontext.SaveChangesAsync();

        public void Dispose()
            => _Dbcontext.Dispose();
    }
}
