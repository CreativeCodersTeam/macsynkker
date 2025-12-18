namespace CreativeCoders.MacOS.UserDefaults;

public interface IUserDefaultsExporter
{
    Task ExportDomainAsync(string domainName, string filePath);

    Task ExportDomainsAsync(IEnumerable<string> domainNames, string directoryPath);
}
