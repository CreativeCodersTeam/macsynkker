namespace CreativeCoders.MacOS.UserDefaults;

public interface IUserDefaults
{
    Task<IEnumerable<DefaultsDomain>> GetDomainsAsync();

    Task ExportDomainAsync(string domainName, string filePath);

    Task ImportDomainAsync(string domainName, string filePath);

    Task ExportDomainsAsync(IEnumerable<string> domainNames, string directoryPath);

    Task ImportAllDomainsAsync(string directoryPath);
}
