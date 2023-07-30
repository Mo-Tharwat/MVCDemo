using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
	//this flag is to make sure that the user who will access this controller will be authed to do that 
	[Authorize]
	public class DepartmentController : Controller
    {
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        //when i make an opject of a controler the CLR will call the Ctor 

        //for this Ctor to work i have to Allow the Dependance injection like Shown in the StartUp.cs 
        public DepartmentController(/*IDepartmentRepository departmentRepository*/ IUnitOfWork unitOfWork , IMapper mapper)//Ask the CLR to make|Create a class that implement the IDepartmentRepository 
        {
            /*_departmentRepository = departmentRepository;*/ //initial it`s value to the _departmentRepository
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // /Deparment/Index
        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAll();
            // View have Four overloads 1.View() || 2.View(View Name) || 3.View(Model) || 4.View(viewName,Model)

            var mappedEmp = _mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(departments);


            return View(mappedEmp);
        }

        #region Create
        // /Deparment/Create
        [HttpGet] //Default
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentViewModel departmentVM)
        {
            if (ModelState.IsValid)//Server Side Validation (BackEnd Validation)
            {
                var mappedDept = _mapper.Map<DepartmentViewModel, Department>(departmentVM);
                await _unitOfWork.DepartmentRepository.Add(mappedDept);

                int count = await _unitOfWork.Completed();
                if (count > 0)
                    TempData["Message"] = "Department is Created Successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(departmentVM);
        } 
        #endregion

        #region Edit
        // /Deparment/Edit/:id
        [HttpGet] //Default
        public async Task<IActionResult> Edit(int? id)
        {
            return await Details(id, "Edit");
        }

        //[HttpPut] But Html Form Dose not Support Put
        [HttpPost]
        [ValidateAntiForgeryToken] // To Make sure that the Request is Coming From the Site 
        public async Task<IActionResult> Edit([FromRoute]int id, DepartmentViewModel departmentVM)
        {
            if (id != departmentVM.Id)
                return BadRequest();

            if (ModelState.IsValid)//Server Side Validation (BackEnd Validation)
            {
                try
                {
                    var mappedDept = _mapper.Map<DepartmentViewModel, Department>(departmentVM);
                    _unitOfWork.DepartmentRepository.Update(mappedDept);
                    await _unitOfWork.Completed();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    //1. log the Exception 
                    //2. User Friendly Message

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                
            }
            return View(departmentVM);
        }
        #endregion

        #region Delete
        // /Deparment/Delete
        public async Task<IActionResult> Delete(int? id)
        {
           return await Details(id,"Delete");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int id, DepartmentViewModel departmentVM)
        {
            if (id != departmentVM.Id)
                return BadRequest();

            if (ModelState.IsValid)//Server Side Validation (BackEnd Validation)
            {
                try
                {
                    var mappedDept = _mapper.Map<DepartmentViewModel, Department>(departmentVM);
                    _unitOfWork.DepartmentRepository.Delete(mappedDept);
                    await _unitOfWork.Completed();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    //1. log the Exception 
                    //2. User Friendly Message

                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(departmentVM);
        }
        #endregion

        #region Details
        // /Deparment/Details/:id
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest("There is no id to Search For");
            var department = await _unitOfWork.DepartmentRepository.Get(id.Value);
            if (department is null)
                return NotFound();

            var mappedDept = _mapper.Map<Department, DepartmentViewModel>(department);

            return View(viewName, mappedDept);
        } 
        #endregion
    }
}
