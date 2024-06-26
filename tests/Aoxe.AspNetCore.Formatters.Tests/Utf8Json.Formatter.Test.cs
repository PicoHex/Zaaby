namespace Aoxe.AspNetCore.Formatters.Tests;

public partial class FormatterTest
{
    [Fact]
    public async Task Utf8JsonFormatterTest()
    {
        var serializer = new Aoxe.Utf8Json.Serializer();
        await TextFormatterAsync(serializer, "application/x-utf8json");
    }

    [Fact]
    public async Task Utf8JsonFormatterNullTest()
    {
        var serializer = new Aoxe.Utf8Json.Serializer();
        await TextFormatterNullAsync(serializer, "application/x-utf8json");
    }
}
