namespace WebStorageManagement.Models;
public interface IWebStorageService
{
    /// <summary>
    /// Gets or sets the local storage service.
    /// </summary>
    /// <value>
    /// The local storage service.
    /// </value>
    LocalStorage LocalStorage { get; set; }

    /// <summary>
    /// Gets or sets the session storage service.
    /// </summary>
    /// <value>
    /// The session storage service.
    /// </value>
    SessionStorage SessionStorage { get; set; }
}
