using TestsGeneratorLib;

const string savingPath = "../../../../Test/Generated";
if (Directory.Exists(savingPath))
{
    Directory.Delete(savingPath, recursive: true);
}
Directory.CreateDirectory(savingPath);

var files = new List<string>
{
    "../../../../Test/Test.cs",
};
await TestGenerator.GenerateAsync(
    files,
    savingPath,
    4
);