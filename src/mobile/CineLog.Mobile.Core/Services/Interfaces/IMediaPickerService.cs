namespace CineLog.Mobile.Core.Services.Interfaces;

public sealed record PickedPhoto(string LocalPath, string FileName, string ContentType, Func<Task<Stream>> OpenStreamAsync);

public interface IMediaPickerService
{
    Task<PickedPhoto?> PickPhotoAsync();
}
