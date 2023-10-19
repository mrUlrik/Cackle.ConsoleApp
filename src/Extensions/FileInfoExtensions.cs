namespace Cackle.ConsoleApp.Extensions;

/// <summary>
///     Helper methods for <see cref="FileInfo" />
/// </summary>
public static class FileInfoExtensions
{
    /// <summary>
    ///     Check if file is locked by another process.
    /// </summary>
    /// <remarks>Will throw an exception for any other failure states; i.e. <see cref="FileNotFoundException" />.</remarks>
    /// <param name="fileInfo">File in question</param>
    /// <returns></returns>
    public static bool IsLocked(this FileInfo fileInfo)
    {
        try
        {
            // Attempt to open the file with a lock which throws an IOException if a lock already exists
            using var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);

            // Immediately close the file, we aren't looking to actually work with it
            stream.Close();
        }
        // Catch specifically a file lock scenario, the rest gets thrown; we aren't looking for anything but a locked status
        catch (IOException ex) when (ex.HResult == -2147024864)
        {
            // File locked, return test
            return true;
        }

        // File not locked, return test
        return false;
    }
}