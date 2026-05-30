using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public sealed class MauiMediaPickerService : IMediaPickerService
{
    public async Task<PickedPhoto?> PickPhotoAsync()
    {
        var result = await MediaPicker.Default.PickPhotoAsync();
        if (result is null)
            return null;
        return new PickedPhoto(result.FullPath, result.FileName, result.ContentType, result.OpenReadAsync);
    }
}
