using Rehapp.Infrastructure.Core.Abstractions;
using Rehapp.Infrastructure.Core.Models;
using Rehapp.Mobile.Models;
using System.Net.Http.Json;

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

    public async Task<InternalResponse> RegisterAsync(UserRegistrationModel user, List<FileResult> files)
    {
        InternalResponse<HttpResponseMessage> response;
        var internalResponse = new InternalResponse();

        if (user.Type == Models.Enums.AccountType.Patient) 
        {
            response = await api.PostAsync(API_REGISTER, (
                firstName: user.FirstName,
                surname: user.Surname,
                email: user.Email,
                password: user.Password,
                passwordConfirmation: user.PasswordConfirmation,
                userName: user.Username,
                cityId: user.City.Id));
        }
        else if (user.Type == Models.Enums.AccountType.Specialist)
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }

        try
        {
            if (!response.IsSuccess || !response.Data.IsSuccessStatusCode)
            {
                var message = (await response.Data.Content.ReadFromJsonAsync<FailureResponse>()).Message;
                return internalResponse.Failure(new InvalidOperationException(message));
            }
        }
        catch
        {
            return internalResponse.Failure();
        }

        return internalResponse.Success();
    }
}
