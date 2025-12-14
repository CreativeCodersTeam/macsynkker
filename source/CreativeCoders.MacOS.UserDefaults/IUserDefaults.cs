namespace CreativeCoders.MacOS.UserDefaults;

public interface IUserDefaults
{
    Task<IEnumerable<DefaultsDomain>> GetDomainsAsync();


}
