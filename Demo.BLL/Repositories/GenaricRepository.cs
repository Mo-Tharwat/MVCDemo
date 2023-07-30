using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    //Have the Implmentaion For The IGenaricRepo
    public class GenaricRepository<T>:IGenaricRepository<T> where T : class
    {
        //till this point we didn`t open the Connection with the DataBase
        private protected readonly MVCAppDemoDbcontext _dbContext;

        public GenaricRepository(MVCAppDemoDbcontext dbContext)
        {
            //this way we will Connect with the DataBase Only when we Call the DepartmentRepository
            /*dbContext = new MVCAppDemoDbcontext();*/
            // there is an Issue in this Way  
            /// the issue is some cases i need to make two instance of the same Class that mean the Ctor Will be called 2 times each time 
            /// will in initialize dbContext two times means 2 dataBase Connection !!
            /// To Fix that we Will Make the CLR Handle that 
            /// how you ask by setting a param in the Ctor it Self public DepartmentRepository(MVCAppDemoDbcontext dbContext)
            /// to make it work in front End Don`t Forget to Add "services.AddDbContext<MVCAppDemoDbcontext>();" in your PL.StartUp.CS 

            _dbContext = dbContext;

        }
        public async Task Add(T item)
          =>  await _dbContext.Set<T>().AddAsync(item);

        public void Delete(T item)
            => _dbContext.Set<T>().Remove(item);

        public async Task<T> Get(int Id)
            //var department =  _dbContext.Departments.Local.Where(D=>D.Id == id).FirstOrDefault();
            //if (department == null)
            //    department = _dbContext.Departments.Where(D => D.Id == id).FirstOrDefault();
            => await _dbContext.Set<T>().FindAsync(Id);

        public async Task<IEnumerable<T>> GetAll()
        {
            // this is a wrong way to handle this 
            // this will be fixed useing the sepecifecation Design patern
            if (typeof(T) == typeof(Employee))
                return (IEnumerable<T>) await _dbContext.Employees.Include(E => E.Department).ToListAsync();
            else
                return await _dbContext.Set<T>().ToListAsync();
        }
           

        public void Update(T item)
            => _dbContext.Set<T>().Update(item);
    }
}
