namespace MikietaApi.Models;

public class PagedResult<T>
{
    public T[] Data { get; set; } = null!;
    public int MaxPageCount { get; set; }
    public int MaxRowCount { get; set; }

    public static PagedResult<T> Create(T[] data, int maxLength)
    {
        var maxPageCount = data.Length == maxLength ? 1 : (int)Math.Ceiling((double)maxLength / data.Length);
        return new PagedResult<T>
        {
            Data = data,
            MaxRowCount = maxLength,
            MaxPageCount = maxPageCount,
        };
    }
}