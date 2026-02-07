namespace CreativeCoders.MacOS.UserDefaults;

public interface IUserDefaultsExporter
{
    Task ExportDomainAsync(string domainName, string filePath, PlistFormat format = PlistFormat.Original);

    Task ExportDomainsAsync(IEnumerable<string> domainNames, string directoryPath,
        PlistFormat format = PlistFormat.Original);
}
