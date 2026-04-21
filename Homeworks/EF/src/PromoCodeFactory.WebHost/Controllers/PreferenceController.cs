using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferenceController(IRepository<Preference> repository) : ControllerBase
    {
        /// <summary>
        /// Получить данные всех предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<PrefernceResponse>> GetPreferencesAsync()
        {
            var preferences = await repository.GetAllAsync();

            var preferencesModelList = preferences.Select(x =>
                new PrefernceResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();

            return preferencesModelList;
        }

        /// <summary>
        /// Получить предпочтение по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PrefernceResponse>> GetPreferenceByIdAsync(Guid id)
        {
            var preferences = await repository.GetByIdAsync(id);

            if (preferences == null)
                return NotFound();

            var preferenceModel = new PrefernceResponse()
            {
                Id = preferences.Id,
                Name = preferences.Name,
            };

            return preferenceModel;
        }
    }
}
