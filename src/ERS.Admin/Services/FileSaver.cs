using Microsoft.JSInterop;

namespace ERS.Admin.Services;

public sealed class FileSaver
{
    private readonly IJSRuntime _js;

    public FileSaver(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SaveAsync(byte[] content, string fileName, string contentType)
    {
        var base64 = Convert.ToBase64String(content);
        await _js.InvokeVoidAsync("ersFile.saveBase64", fileName, contentType, base64);
    }
}