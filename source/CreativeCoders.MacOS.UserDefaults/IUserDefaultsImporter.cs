namespace CreativeCoders.MacOS.UserDefaults;

public interface IUserDefaultsImporter
{
    Task ImportDomainAsync(string domainName, string filePath);

    Task ImportAllDomainsAsync(string directoryPath, bool onlyImportExistingDomains = true);

    Task ImportAllDomainsAsync(string directoryPath, Func<string, bool> domainNameFilter);
}
