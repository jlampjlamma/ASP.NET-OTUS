using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Партнеры
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PartnersController
        : ControllerBase
    {
        private readonly IRepository<Partner> _partnersRepository;

        public PartnersController(IRepository<Partner> partnersRepository)
        {
            _partnersRepository = partnersRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync()
        {
            var partners = await _partnersRepository.GetAllAsync();

            var response = partners.Select(x => new PartnerResponse()
            {
                Id = x.Id,
                Name = x.Name,
                NumberIssuedPromoCodes = x.NumberIssuedPromoCodes,
                //TODO: Было true, что не верно
                IsActive = x.IsActive,
                PartnerLimits = x.PartnerLimits
                    .Select(y => new PartnerPromoCodeLimitResponse()
                    {
                        Id = y.Id,
                        PartnerId = y.PartnerId,
                        Limit = y.Limit,
                        CreateDate = y.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        EndDate = y.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        CancelDate = y.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
                    }).ToList()
            });

            return Ok(response);
        }

        [HttpGet("{id}/limits/{limitId}")]
        public async Task<ActionResult<PartnerPromoCodeLimit>> GetPartnerLimitAsync(Guid id, Guid limitId)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();

            //TODO: бизнес вопрос на самом деле, поэтому код править не буду.
            //а если партнер не активен, разве мы должны выводить хоть какую-то инфу по нему? тем более лимиты

            var limit = partner.PartnerLimits
                .FirstOrDefault(x => x.Id == limitId);

            var response = new PartnerPromoCodeLimitResponse()
            {
                Id = limit.Id,
                PartnerId = limit.PartnerId,
                Limit = limit.Limit,
                CreateDate = limit.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                EndDate = limit.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                CancelDate = limit.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
            };

            return Ok(response);
        }

        [HttpPost("{id}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            //TODO: тут не будет Include лимитов, а значит потом мы получим null
            //самый простой в контексте добавить options.UseLazyLoadingProxies(), тем более что коллекция virtual
            //но это как по мне костыль, лучше делать отдельную реализацию репозитория для партрена с переопределением метода
            //но прошу простить мою лень, мне не хочется это писать
            //можно так же написать тест, с использованием тестовой базы, но это уже тест репозитория, а не контроллера.
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();

            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest("Данный партнер не активен");

            //TODO: Перенес проверку перед изменениями. Нет смысла что-то делать, если запрос не верный
            if (request.Limit <= 0)
                return BadRequest("Лимит должен быть больше 0");

            //TODO: пропущена проверка EndDate
            //условие не точное, т.к. не ясна бизнес логика, на сколько EndDate должен быть больше нынешнего времени
            if (request.EndDate <= DateTime.Now)
                return BadRequest("Дата окончания лимита ");

            //Установка лимита партнеру

            //TODO: вынес проверку "активности" лимита в сущность, это позволяет иметь единый источник
            //так же добавил проверку EndDate, рассуждая, что CancelDate актуален, когда EndDate еще не наступил
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x => x.IsActive);

            if (activeLimit != null)
            {
                //Если партнеру выставляется лимит, то мы 
                //должны обнулить количество промокодов, которые партнер выдал, если лимит закончился, 
                //то количество не обнуляется
                partner.NumberIssuedPromoCodes = 0;

                //При установке лимита нужно отключить предыдущий лимит
                activeLimit.CancelDate = DateTime.Now;
            }

            var newLimit = new PartnerPromoCodeLimit()
            {
                Limit = request.Limit,
                Partner = partner,
                PartnerId = partner.Id,
                CreateDate = DateTime.Now,
                EndDate = request.EndDate
            };

            partner.PartnerLimits.Add(newLimit);

            await _partnersRepository.UpdateAsync(partner);

            return CreatedAtAction(nameof(GetPartnerLimitAsync), new { id = partner.Id, limitId = newLimit.Id }, null);
        }

        [HttpPost("{id}/canceledLimits")]
        public async Task<IActionResult> CancelPartnerPromoCodeLimitAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();

            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest("Данный партнер не активен");

            //TODO: опять бизнес вопрос. если EndDate меньше DateTime.Now, изменяем ли мы CancelDate? по сути он и так истек.
            //что делаем с NumberIssuedPromoCodes? Ведь при выдаче мы его обнуляем, если EndDate еще считается активным.
            //и если логика в итоге будет совпадать с логикой из SetPartnerPromoCodeLimitAsync,
            //то часть с анализом и изменением CancelDate нужно вынести в отдельный метод, чтобы была единая точна

            //Отключение лимита
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x =>
                !x.CancelDate.HasValue);

            if (activeLimit != null)
            {
                activeLimit.CancelDate = DateTime.Now;
            }

            await _partnersRepository.UpdateAsync(partner);

            return NoContent();
        }
    }
}