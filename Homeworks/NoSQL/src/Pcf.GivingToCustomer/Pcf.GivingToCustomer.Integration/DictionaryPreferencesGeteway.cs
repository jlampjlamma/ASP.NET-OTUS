using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration;

public class DictionaryPreferencesGeteway : IDictionaryPreferencesGeteway
{
    private readonly HttpClient _httpClient;

    public DictionaryPreferencesGeteway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Preference>> GetAllPreferences()
    {
        var response = await _httpClient.GetFromJsonAsync<List<Preference>>("api/preferences");
        return response ?? new();
    }

    public async Task<Preference> GetPreferenceById(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<Preference>($"api/preferences/{id}");
        return response ?? new();
    }

    public async Task<Preference> GetPreferenceByName(string name)
    {
        var response = await _httpClient.GetFromJsonAsync<Preference>($"api/preferences/name?name={name}");
        return response ?? new();
    }

    public async Task<List<Preference>> GetPreferencesByIds(IEnumerable<Guid> ids)
    {
        var response = await _httpClient.PostAsJsonAsync(
                "api/preferences/by-ids",
                ids.ToList());

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Preference>>() ?? new();
    }
}
