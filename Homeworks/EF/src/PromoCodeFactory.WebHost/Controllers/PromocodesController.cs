using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController(IRepository<PromoCode> repPromo, IRepository<Customer> repCust, IRepository<Preference> repPref, IRepository<Employee> repEmpl)
        : ControllerBase
    {
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promocodes = await repPromo.GetAllAsync();

            var customersModelList = promocodes.Select(x =>
                new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    BeginDate = x.BeginDate.ToString(),
                    Code = x.Code,
                    EndDate = x.EndDate.ToString(),
                    PartnerName = x.PartnerName,
                    ServiceInfo = x.ServiceInfo
                }).ToList();

            return customersModelList;
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var employees = await repEmpl.GetAllAsync();
            var partner = employees.FirstOrDefault(e => e.FullName.Equals(request.PartnerName, StringComparison.OrdinalIgnoreCase));
            if (partner == null)
                return BadRequest($"Partner '{request.PartnerName}' not found");

            var preferences = await repPref.GetAllAsync();
            var preference = preferences.FirstOrDefault(p => p.Name.Equals(request.Preference, StringComparison.OrdinalIgnoreCase));
            if (preference == null)
                return BadRequest($"Preference '{request.Preference}' not found");

            var allCustomers = await repCust.GetAllAsync(
                c => c.Preferences,
                c => c.PromoCode
            );

            var targetCustomers = allCustomers
                .Where(c => c.Preferences?.Any(p => p.Id == preference.Id) == true)
                .ToList();

            if (!targetCustomers.Any())
                return NotFound($"No customers found with preference '{request.Preference}'");

            var promo = new PromoCode
            {
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                BeginDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                PartnerManagerId = partner.Id,
                PreferenceId = preference.Id
            };

            foreach (var customer in targetCustomers)
            {
                customer.PromoCode ??= new List<PromoCode>();
                customer.PromoCode.Add(promo);
            }

            await repCust.SaveChangesAsync();

            return Ok(new
            {
                promoCode = promo.Code,
                givenToCount = targetCustomers.Count
            });
        }
    }
}