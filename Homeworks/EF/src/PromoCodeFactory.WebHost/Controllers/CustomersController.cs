using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController(IRepository<Customer> repCustomer, IRepository<Preference> repPreference)
        : ControllerBase
    {
        /// <summary>
        /// Получить список всех клиентов 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await repCustomer.GetAllAsync();

            var customersModelList = customers.Select(x =>
                new CustomerShortResponse()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email
                }).ToList();

            return customersModelList;
        }
        /// <summary>
        /// Получить клиента и его промокоды по id
        /// </summary>
        /// <param name="id">идентификатор клиента</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ActionName("GetCustomerById")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            //Знаю, закостылил
            Customer customer = await repCustomer.GetByIdWithIncludesAsync(id, default, c => c.PromoCode);

            if (customer == null)
                return NotFound();

            return CustomerResponseMap(customer);
        }


        //в идеале вынести как метод расширения, но мне лень =D
        private CustomerResponse CustomerResponseMap(Customer customer)
        {
            CustomerResponse rep = new CustomerResponse()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PromoCodes = new List<PromoCodeShortResponse>()
            };

            if (customer.PromoCode != null)
                foreach (var pc in customer.PromoCode)
                    rep.PromoCodes.Add(new PromoCodeShortResponse()
                    {
                        Id = pc.Id,
                        BeginDate = pc.BeginDate.ToString(),
                        Code = pc.Code,
                        EndDate = pc.EndDate.ToString(),
                        PartnerName = pc.PartnerName,
                        ServiceInfo = pc.ServiceInfo
                    });
            return rep;
        }
        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="request">Данные клиента</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            Customer customer = new Customer()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Preferences = new List<Preference>()
            };

            foreach (Guid preference in request.PreferenceIds)
            {
                Preference pref = await repPreference.GetByIdAsync(preference) ?? throw new ArgumentNullException(nameof(preference));
                customer.Preferences.Add(pref);
            }
            await repCustomer.AddAsync(customer);
            await repCustomer.SaveChangesAsync();
            return CreatedAtAction("GetCustomerById", new { id = customer.Id }, null);
        }

        /// <summary>
        /// Изменить данные клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="request">Данные клиента</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            Customer customer = await repCustomer.GetByIdWithIncludesAsync(id, default, c => c.Preferences) 
                ?? throw new ArgumentNullException(nameof(id));
           
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.Preferences.Clear();
            foreach (Guid preference in request.PreferenceIds)
            {
                Preference pref = await repPreference.GetByIdAsync(preference) ?? throw new ArgumentNullException(nameof(preference));
                customer.Preferences.Add(pref);
            }
            await repCustomer.UpdateAsync(customer);
            await repCustomer.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {

            Customer customer = await repCustomer.GetByIdAsync(id) ?? throw new ArgumentNullException(nameof(id));
            await repCustomer.DeleteAsync(customer);
            await repCustomer.SaveChangesAsync();
            return NoContent();
        }
    }
}