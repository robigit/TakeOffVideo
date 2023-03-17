using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStorageManagement.Models;

public interface IStorageService
{
    /// <summary>
    /// Initializes the web storage.
    /// </summary>
    /// <returns></returns>
    //Task Init();

    /// <summary>
    /// Saves the specified value according to the keyName.
    /// </summary>
    /// <param name="keyName">Name of the key.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    Task Save(string keyName, string value);

    /// <summary>
    /// Reads the specified value from a key name.
    /// </summary>
    /// <param name="keyName">Name of the key.</param>
    /// <returns></returns>
    Task<string> Read(string keyName);

    /// <summary>
    /// Clears the storage.
    /// </summary>
    /// <returns></returns>
    Task Clear();
}
