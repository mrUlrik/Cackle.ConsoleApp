using System.Text.RegularExpressions;

namespace Cackle.ConsoleApp.Extensions;

/// <summary>
///     Helper methods for <see cref="FileInfo" />
/// </summary>
public static partial class FileInfoExtensions
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

    /// <summary>
    ///     <para>
    ///         Return a <see cref="FileInfo" /> with a file name suffixed with an incrementing number in the provided
    ///         directory that has not already been used.
    ///     </para>
    ///     <para>
    ///         If <c>C:\Temp\Sample.xlsx</c> is provided but the file already exists, <c>:\Temp\Sample (002).xlsx</c> may be
    ///         returned.
    ///     </para>
    /// </summary>
    /// <param name="fileInfo">Preferred file path.</param>
    /// <returns>An available file path.</returns>
    public static FileInfo CreateFileName(this FileInfo fileInfo)
    {
        ulong i = 2;

        var directory = fileInfo.Directory;
        if (directory is null) throw new DirectoryNotFoundException();
        if (!directory.Exists) directory.Create();

        var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
        var match = EndingDigits().Match(fileName);
        if (match.Groups["Digits"].Success)
        {
            var suffix = match.Groups["Digits"].Value.TrimStart('(').TrimEnd(')');
            if (ulong.TryParse(suffix, out var x))
            {
                fileName = fileName.Replace(match.Groups["Digits"].Value, string.Empty).Trim();
                i = ++x;
            }
        }

        while (true)
        {
            if (!fileInfo.Exists) return fileInfo;

            var newFileName = string.Concat(fileName, $" ({i:000})", fileInfo.Extension);
            var filePath = Path.Join(fileInfo.DirectoryName, newFileName);
            fileInfo = new FileInfo(filePath);

            i++;
        }
    }

    [GeneratedRegex("(?<Digits>\\(?[0-9]+\\)?)?$")]
    private static partial Regex EndingDigits();
}