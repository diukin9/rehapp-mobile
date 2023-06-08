using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rehapp.Infrastructure.Core.Abstractions;
using Rehapp.Infrastructure.Core.Extensions;
using Rehapp.Infrastructure.Core.Static;
using Rehapp.Mobile.Models;
using Rehapp.Mobile.Models.Enums;
using System.Collections.ObjectModel;

namespace Rehapp.Mobile.ViewModels;

public partial class RegistrationPageViewModel : BaseViewModel, ITransient, IQueryAttributable
{
    #region fields

    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    private string lastName;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private bool emailIsReadOnly;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string passwordConfirmation;

    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private bool usernameIsReadOnly;

    private AccountType accountType;

    [ObservableProperty]
    private int uploadedFilesCount;

    [ObservableProperty]
    private bool isSpecialist;

    private List<FileResult> uploadedFiles = new();

    [ObservableProperty]
    public bool canClearUploadedFiles;

    [ObservableProperty]
    private City city;

    [ObservableProperty]
    private ObservableCollection<City> cities;

    [ObservableProperty]
    private bool consentToTermsOfDataProcessing;

    #endregion

    #region services

    private readonly FileService fileService;

    private readonly CityService cityService;

    private readonly AccountService accountService;

    #endregion

    #region properties

    public bool RegisterCommandIsNotRunning => !RegisterCommand.IsRunning;

    #endregion

    public RegistrationPageViewModel(FileService fileService, CityService cityService, AccountService accountService)
    {
        this.fileService = fileService;
        this.cityService = cityService;
        this.accountService = accountService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var getValue = (string key) =>
        {
            return query.TryGetValue(key, out var value) && value is not null ? $"{value}" : "";
        };

        accountType = (AccountType)query[ACCOUNT_TYPE];
        IsSpecialist = accountType == AccountType.Specialist;

        FirstName = getValue(FIRSTNAME);
        LastName = getValue(SURNAME);

        Username = getValue(USERNAME);
        UsernameIsReadOnly = !string.IsNullOrEmpty(Username);

        Email = getValue(EMAIL);
        EmailIsReadOnly = !string.IsNullOrEmpty(Email);
    }

    [RelayCommand]
    private async Task OnFilePickerClickedAsync()
    {
        HideError();
        ShowLoader();

        var options = new PickOptions { PickerTitle = "Выберите файлы для загрузки" };
        var selected = await FilePicker.PickMultipleAsync(options);

        HideLoader();

        if (selected is not null && selected.Any()) 
        {
            foreach (var file in  selected)
            {
                var isValid = !ShowErrorByCondition(
                    condition: !fileService.IsValidFile(file.FullPath).IsSuccess,
                    message: "Не удалось загрузить один или несколько файлов");

                if (!isValid) continue;

                uploadedFiles.Add(file);
                UploadedFilesCount = uploadedFiles.Count;
                CanClearUploadedFiles = true;
            }
        }
    }

    [RelayCommand]
    private void ClearUploadedFiles()
    {
        HideError();
        uploadedFiles.Clear();
        UploadedFilesCount = uploadedFiles.Count;
        CanClearUploadedFiles = false;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await NavigationService.GoToLoginPageAsync(true);
    }

    [RelayCommand]
    private async Task GetCitiesAsync()
    {
        var response = await cityService.GetCitiesAsync();
        if (response.IsSuccess) Cities = new ObservableCollection<City>(response.Data);
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        HideError();
        ShowLoader();

        await Task.Delay(100);

        FirstName = FirstName?.Trim();
        LastName = LastName?.Trim();

        #region form validation

        var formValidationConditions = new[]
        {
            KeyValuePair.Create(
                key: string.IsNullOrEmpty(FirstName), 
                value: $"Необходимо заполнить поле 'Имя'"),
            KeyValuePair.Create(
                key: string.IsNullOrEmpty(LastName),
                value: $"Необходимо заполнить поле 'Фамилия'"),
            KeyValuePair.Create(
                key: string.IsNullOrEmpty(Username),
                value: $"Необходимо заполнить поле 'Логин'"),
            KeyValuePair.Create(
                key : string.IsNullOrEmpty(Email),
                value: $"Необходимо заполнить поле 'Почтовый ящик'"),
            KeyValuePair.Create(
                key : string.IsNullOrEmpty(Password),
                value: $"Необходимо заполнить поле 'Пароль'"),
            KeyValuePair.Create(
                key : string.IsNullOrEmpty(PasswordConfirmation),
                value: $"Необходимо заполнить поле 'Подтверждение пароля'"),
            KeyValuePair.Create(
                key: City is null,
                value: "Необходимо выбрать город"),
            KeyValuePair.Create(
                key : accountType == AccountType.Specialist && !(uploadedFiles?.Any() ?? false),
                value: "Необходимо загрузить файлы для подтвержения наличия образования"),
            KeyValuePair.Create(
                key : FirstName.StartsWith('-') || FirstName.EndsWith('-'),
                value: "Поле 'Имя' не может начинаться или заканчиваться с тире"),
            KeyValuePair.Create(
                key: LastName.StartsWith('-') || LastName.EndsWith('-'),
                value: "Поле 'Фамилия' не может начинаться или заканчиваться с тире"),
            KeyValuePair.Create(
                key: LastName.StartsWith('-') || LastName.EndsWith('-'),
                value: "Поле 'Логин' не может начинаться или заканчиваться с тире"),
            KeyValuePair.Create(
                key : FirstName.Any(x => !char.IsLetter(x) && !char.IsWhiteSpace(x) && x != '-'),
                value: "Поле 'Имя' может содержать только буквы, символы пробела и тире"),
            KeyValuePair.Create(
                key : LastName.Any(x => !char.IsLetter(x) && !char.IsWhiteSpace(x) && x != '-'),
                value: "Поле 'Фамилия' может содержать только буквы, символы пробела и тире"),
            KeyValuePair.Create(
                key : Username.Any(x => !char.IsLetter(x) && !char.IsDigit(x) && x != '-' && x != '.'),
                value: "Поле 'Логин' может содержать только буквы, цифры, точку и тире"),
            KeyValuePair.Create(
                key : !Email.IsEmail(),
                value: "Указан неверный формат почтового ящика"),
            KeyValuePair.Create(
                key : Password?.Length < 8,
                value: "Минимальная длина пароля 8 символов"),
            KeyValuePair.Create(
                key : Password?.Any(char.IsWhiteSpace) ?? false,
                value: "Пароль не должен содержать пробельных символов"),
            KeyValuePair.Create(
                key : Password != PasswordConfirmation,
                value: "Пароли не совпадают"),
            KeyValuePair.Create(
                key : !ConsentToTermsOfDataProcessing,
                value: "Необходимо принять условия обработки персональных данных"),
        };

        foreach (var kvp in formValidationConditions)
        {
            if (ShowErrorByCondition(kvp.Key, kvp.Value))
            {
                HideLoader();
                return;
            }
        }

        var response = await accountService.RegisterAsync(new UserRegistrationModel
        {
            City = City,
            Email = Email,
            FirstName = FirstName,
            Surname = LastName,
            Username = Username,
            Type = accountType
        }, uploadedFiles);

        var responseValidationConditions = new[]
        {
            KeyValuePair.Create(
                key: string.IsNullOrEmpty(FirstName),
                value: $"Необходимо заполнить поле 'Имя'")
        };
        if (response.Exception?.Message == Exceptions.UserWithSuchEmailIsAlreadyRegistered.Message)
        {
            ShowError("");
            return;
        }
        else if (response.Exception?.Message == Exceptions.UserWithSuchUsernameIsAlreadyRegistered.Message)
        {
            ShowError("");
            return;
        }
        else if (response.Exception?.Message == Exceptions.InvalidCityIdPassed.Message)
        {
            ShowError("");
            return;
        }
        else if (response.Exception is not null)
        {
            ShowError("");
            return;
        }

        #endregion
    }
}
