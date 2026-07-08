using ZooTech.CodeGeneration.Writers;

namespace ZooTech.CodeGeneration.UnitTests;

public sealed class FileWriterTests
{
    [Fact]
    public void Write_CreatesDirectoryAndFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "sub", "test.g.cs");

        try
        {
            var sut = new FileWriter();
            sut.Write(filePath, "// test");

            Assert.True(File.Exists(filePath));
            Assert.Equal("// test", File.ReadAllText(filePath));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public void Write_OverwritesExistingFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "test.g.cs");

        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(filePath, "old content");

            var sut = new FileWriter();
            sut.Write(filePath, "new content");

            Assert.Equal("new content", File.ReadAllText(filePath));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public void Write_UsesUtf8Encoding()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "test.g.cs");

        try
        {
            var sut = new FileWriter();
            sut.Write(filePath, "// üñíçödé");

            var bytes = File.ReadAllBytes(filePath);
            Assert.Contains(bytes, b => b > 127);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }
}
