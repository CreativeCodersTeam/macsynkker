namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>
/// Represents a single entry that <c>brew cleanup --dry-run</c> reported as removable,
/// together with its on-disk size in bytes.
/// </summary>
/// <param name="Path">The file or directory path that would be removed.</param>
/// <param name="SizeInBytes">Size of the entry in bytes as reported by Homebrew.</param>
public record ReclaimableItem(string Path, long SizeInBytes);
