using System.Text;

namespace ZooTech.CodeGeneration.Writers;

public sealed class FileWriter
{
    public void Write(string path, string content)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, content, Encoding.UTF8);
    }
}
