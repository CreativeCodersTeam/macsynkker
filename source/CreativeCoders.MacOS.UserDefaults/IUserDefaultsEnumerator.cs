namespace CreativeCoders.MacOS.UserDefaults;

public interface IUserDefaultsEnumerator
{
    Task<IEnumerable<DefaultsDomain>> GetDomainsAsync();

    Task<IEnumerable<string>> GetDomainNamesAsync();
}
