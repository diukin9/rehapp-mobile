using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.Models;
//using RehApp.Infrastructure.Common.Interfaces;
using System.Net.Http.Headers;

namespace Rehapp.Mobile.Services;

public class FileService : IService, ITransient
{
    private readonly ApiService api;

    public FileService(ApiService api)
    {
        this.api = api;
    }

    public InternalResponse IsValidFile(string path)
    {
        var response = new InternalResponse();

        if (new FileInfo(path).Length / 1024 / 1024 > 10)
        {
            return response.Failure(new ArgumentException("The file is too large"));
        }

        return response.Success();
    }

    public async Task<InternalResponse> ProcessFileAsync(FileResult fileResult)
    {
        var response = new InternalResponse();

        if (fileResult is null)
        {
            return response.Failure(new ArgumentNullException("FileResult was null"));
        }

        var isValidResponse = IsValidFile(fileResult.FullPath);
        if (isValidResponse.IsSuccess) return isValidResponse;

        byte[] bytes;
        using var fileStream = File.OpenRead(fileResult.FullPath);

        using var memoryStream = new MemoryStream();
        {
            await fileStream.CopyToAsync(memoryStream);
            bytes = memoryStream.ToArray();
        }

        using var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

        using var form = new MultipartFormDataContent
        {
            { fileContent, "fileContent", Path.GetFullPath(fileResult.FullPath) }
        };

        return await UploadFile(form);
    }

    private async Task<InternalResponse> UploadFile(MultipartFormDataContent form)
    {
        var response = new InternalResponse();

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return response.Failure(new IOException("No internet connection"));
        }

        //TODO send file to server

        return await Task.FromResult(response.Success());
    }
}
