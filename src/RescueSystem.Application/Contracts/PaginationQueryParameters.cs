namespace RescueSystem.Application.Contracts;

public class PaginationQueryParameters
{
    private int _pageNumber = 1;
    private int _pageSize = 20;
    private const int MaxPageSize = 100;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = (value < 1) ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 1)
                _pageSize = 1;
            else if (value > MaxPageSize)
                _pageSize = MaxPageSize;
            else
                _pageSize = value;
        }
    }

    private string? _searchTerm;
    public string? SearchTerm
    {
        get => _searchTerm;
        set => _searchTerm = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

