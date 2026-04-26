using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferencesController : ControllerBase
    {
        private readonly PreferenceDbContext _context;
        private readonly HybridCache _cache;

        public PreferencesController(PreferenceDbContext context,
            HybridCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Получить список всех предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Preference>>> GetPreferences(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(
                "preferences:all",
                factory: async _ => await _context.Preferences.ToListAsync(),
                tags: ["preferences"],
                cancellationToken: ct
                );
        }

        /// <summary>
        /// Получить предпочтение по id
        /// </summary>
        /// <param name="id">идентификатор предпочтения</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Preference>> GetPreferenceById(Guid id)
        {
            var preference = await _cache.GetOrCreateAsync(
                key: $"preference:id:{id}",
                factory: async (ct) =>
                    await _context.Preferences.FindAsync([id]),
                tags: ["preferences"],
                cancellationToken: HttpContext.RequestAborted
            );

            return preference is null ? NotFound() : Ok(preference);
        }

        /// <summary>
        /// Получить предпочтение по наименованию
        /// </summary>
        /// <param name="name">наименование предпочтения</param>
        [HttpGet("name")]
        public async Task<ActionResult<Preference>> GetPreferenceByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Параметр name не может быть пустым");

            var preference = await _cache.GetOrCreateAsync(
                key: $"preference:name:{Uri.EscapeDataString(name)}",
                factory: async (ct) =>
                    await _context.Preferences.FirstOrDefaultAsync(p => EF.Functions.ILike(p.Name, name)),
                tags: ["preferences"],
                cancellationToken: HttpContext.RequestAborted
            );

            return preference is null ? NotFound() : Ok(preference);
        }

        [HttpPost("by-ids")]
        public async Task<ActionResult<IEnumerable<Preference>>> GetPreferencesByIds(
    [FromBody] List<Guid> ids,
    CancellationToken ct = default)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("Список идентификаторов не может быть пустым");

            var sortedIds = ids.Distinct().OrderBy(x => x).ToList();
            var cacheKey = $"preferences:batch:{string.Join(",", sortedIds)}";

            var preferences = await _cache.GetOrCreateAsync(
                key: cacheKey,
                factory: async ct => await _context.Preferences
                    .AsNoTracking()
                    .Where(p => sortedIds.Contains(p.Id))
                    .ToListAsync(ct),
                tags: ["preferences"],
                cancellationToken: ct
            );

            return Ok(preferences);
        }

        /// <summary>
        /// Изменить предпочтение по идентификатору
        /// </summary>
        /// <param name="id">идентификатор предпочтения</param>
        /// <param name="name">новое наименование предпочтения</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreference(Guid id, string name)
        {
            var preference = await _context.Preferences.FindAsync(id);

            if (preference == null)
            {
                return NotFound();
            }

            preference.Name = name;

            await _context.SaveChangesAsync();
            await _cache.RemoveByTagAsync("preferences", HttpContext.RequestAborted);

            return NoContent();
        }

        /// <summary>
        /// Создать нвоое предпочтение
        /// </summary>
        /// <param name="name">наименование предпочтения</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Preference>> PostPreference(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Название предпочтения не может быть пустым");

            Preference? pref = await _context.Preferences.
                FirstOrDefaultAsync(p => EF.Functions.ILike(p.Name, name));

            if (pref != null)
            {
                return Conflict($"Предпочтение \"{name}\" уже существует");
            }

            Preference preference = new Preference()
            {
                Name = name
            };
            _context.Preferences.Add(preference);
            await _context.SaveChangesAsync();

            await _cache.RemoveAsync("preferences:all", HttpContext.RequestAborted);

            return CreatedAtAction("GetPreference", new { id = preference.Id }, preference);
        }

        /// <summary>
        /// Удалить преподчтение по идентификатору
        /// </summary>
        /// <param name="id">идентификатор предпочтения</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreference(Guid id)
        {
            var preference = await _context.Preferences.FindAsync(id);
            if (preference == null)
            {
                return NotFound();
            }

            _context.Preferences.Remove(preference);
            await _context.SaveChangesAsync();
            await _cache.RemoveByTagAsync("preferences", HttpContext.RequestAborted);

            return NoContent();
        }
    }
}
