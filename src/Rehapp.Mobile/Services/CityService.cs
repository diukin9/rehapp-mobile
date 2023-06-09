﻿using Rehapp.Infrastructure.Core.Abstractions;
using Rehapp.Infrastructure.Core.Models;
using Rehapp.Mobile.Models;

namespace Rehapp.Mobile.Services;

public class CityService : IService, ITransient
{
    private readonly ApiService api;

    public CityService(ApiService api)
    {
        this.api = api;
    }

    public async Task<InternalResponse<List<City>>> GetCitiesAsync()
    {
        var internalResponse = new InternalResponse<List<City>>();

        var response = await api.GetAsync<List<City>>(API_CITIES);
        if (!response.IsSuccess) return internalResponse.Failure();

        return internalResponse.Success(response.Data);
    }
}
