using System.IO;
using System.Runtime.InteropServices;

class CopiedFile
{
    public CopiedFile(string relativePath, string sourceChecksum, string? destChecksum)
    {
        RelativePath = relativePath;
        SourceChecksum = sourceChecksum;
        DestChecksum = destChecksum;
    }

    public string RelativePath { get; set; }
    public string SourceChecksum { get; set; }
    public string? DestChecksum { get; set; }
}

public class Program
{
    public static string GetChecksumOf(string path)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        using var stream = File.OpenRead(path);

        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
    public static void Main(string[] args)
    {
        Console.WriteLine("Arg1: " + args[1]);
        Console.WriteLine("Arg2: " + args[2]);
        var source = args[1];
        var dest = args[2];

        if(!Directory.Exists(source)) throw new Exception("Source file not exists.");
        if(!Directory.Exists(dest)) Directory.CreateDirectory(dest);

        Console.Write("Computing checksums...");
        var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Select(path => new
        {
            Path = path,
            RelativePath = Path.GetRelativePath(source, path),
            SourceChecksum = GetChecksumOf(path),
            DestPath = ""
        }).Select(file => file with
        {
            DestPath = Path.Join(dest, file.RelativePath)
        }).Select(file => new CopiedFile(file.RelativePath, file.SourceChecksum, File.Exists(file.DestPath) ? GetChecksumOf(file.DestPath) : null))
        .Where(file => file.SourceChecksum != file.DestChecksum)
        .ToArray();
        Console.WriteLine(" Complete.");

        Console.WriteLine($"{files.Count()} files found.");

        int progress = 1;
        foreach(var file in files) {
            Console.Write($"Copying file {progress} out of {files.Count()}...");
            File.Copy(Path.Join(source, file.RelativePath), Path.Join(dest, file.RelativePath), true);
            Console.WriteLine(" Complete.");
            progress++;
        }

        Console.WriteLine("Copy complete !");
    }
}