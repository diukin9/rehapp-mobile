using Rehapp.Infrastructure.Abstractions;
using Rehapp.Mobile.Models;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.Services;

public class AccountService : IService, ITransient
{
    private readonly ApiService api;

    public AccountService(ApiService api)
    {
        this.api = api;
    }

    public async Task<InternalResponse> SendMailToRecoveryPasswordAsync(string email)
    {
        var internalResponse = new InternalResponse();

        var body = new { email, callback = PASSWORD_RECOVERY_PAGE };
        var response = await api.PostAsync(API_SEND_MAIL_TO_RECOVER_PASSWORD, body);

        return response.IsSuccess && response.Data.IsSuccessStatusCode 
            ? internalResponse.Success() 
            : internalResponse.Failure();
    }
}
