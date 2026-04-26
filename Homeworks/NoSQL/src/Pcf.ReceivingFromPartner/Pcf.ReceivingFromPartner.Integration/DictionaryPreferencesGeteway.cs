using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration;

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
}
