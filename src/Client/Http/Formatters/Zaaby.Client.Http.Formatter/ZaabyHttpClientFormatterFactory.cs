namespace Zaaby.Client.Http.Formatter;

public static class ZaabyHttpClientFormatterFactory
{
    public static ZaabyHttpClientFormatter Create(ZaabyClientFormatterOptions options) =>
        options.Serializer switch
        {
            ITextSerializer => new ZaabyHttpClientTextFormatter(options),
            not null => new ZaabyHttpClientStreamFormatter(options),
            _ => throw new ArgumentOutOfRangeException(nameof(options.Serializer),
                $"options.Serializer must be {nameof(ITextSerializer)} or {nameof(IStreamSerializer)}.")
        };
}