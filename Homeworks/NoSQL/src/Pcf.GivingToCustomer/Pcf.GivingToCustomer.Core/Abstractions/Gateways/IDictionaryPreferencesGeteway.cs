using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways;

public interface IDictionaryPreferencesGeteway
{
    Task<List<Preference>> GetAllPreferences();
    Task<Preference> GetPreferenceById(Guid id);
    Task<Preference> GetPreferenceByName(string name);
    Task<List<Preference>> GetPreferencesByIds(IEnumerable<Guid> ids);
}
