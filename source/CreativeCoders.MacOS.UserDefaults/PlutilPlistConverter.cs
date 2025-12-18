namespace CreativeCoders.MacOS.UserDefaults;

public class PlutilPlistConverter : IPlistConverter
{
    public Task ConvertFileAsync(string sourceFileName, string outputFileName, PlistFormat format)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> ConvertBytesAsync(byte[] inputData, PlistFormat format)
    {
        throw new NotImplementedException();
    }
}