using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Request;
using PromoCodeFactory.WebHost.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Role> _roleRepository;

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}", Name = "GetEmployeeById")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать сотрудника
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeByIdAsync(CreateEmployeeRequest request)
        {
            List<Role> roles = await GetRolesByIds(request.Roles);
            if (roles.Count != request.Roles.Count)
                return BadRequest("Не корректный список ролей");
            if(!Regex.IsMatch(request.Email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
                return BadRequest("Не корректный Email");
            Employee employee = Employee.Create(firstName: request.FirstName, lastName: request.LastName, email: request.Email, roles);
            await _employeeRepository.AddAsync(employee);
            EmployeeResponse response = employee.ToEmployeeResponse();

            return CreatedAtAction("GetEmployeeById", new { id = employee.Id }, response);
        }

        /// <summary>
        /// изменить сотрудника
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeByIdAsync(UpdateEmployeeRequest request)
        {

            var employee = await _employeeRepository.GetByIdAsync(request.Id);

            if (employee == null)
                return NotFound();

            List<Role> roles = await GetRolesByIds(request.Roles);
            if (roles.Count != request.Roles.Count)
                return BadRequest("Не корректный список ролей");
            if (!Regex.IsMatch(request.Email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
                return BadRequest("Не корректный Email");
            employee.Update(firstName: request.FirstName, lastName: request.LastName, email: request.Email,
                roles: roles, appliedPromocodesCount: request.AppliedPromocodesCount);

            EmployeeResponse response = employee.ToEmployeeResponse();

            return Ok(response);
        }

        private async Task<List<Role>> GetRolesByIds(List<Guid> ids)
        {
            List<Role> roles = new List<Role>();
            foreach (Guid roleId in ids)
            {
                Role role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null)
                    roles.Add(role);
            }
            return roles;
        }

        /// <summary>
        /// удалить сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            await _employeeRepository.DeleteByIdAsync(employee);
            return NoContent();

        }
    }
}