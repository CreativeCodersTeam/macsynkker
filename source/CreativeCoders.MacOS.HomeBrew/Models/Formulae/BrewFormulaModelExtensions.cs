namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Provides extension methods for <see cref="BrewFormulaModel"/>.
/// </summary>
public static class BrewFormulaModelExtensions
{
    /// <summary>
    /// Determines whether the formula is installed as a dependency.
    /// </summary>
    /// <param name="formula">The formula to check.</param>
    /// <returns>
    /// <see langword="true"/> if the formula is installed as a dependency;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsInstalledAsDependency(this BrewFormulaModel formula)
    {
        return formula.Installed?.Any(x => x.InstalledAsDependency == true) == true;
    }
}
