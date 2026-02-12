namespace StudentsApi.Dtos
{
    public record PagedResultDto<T>(
        List<T> Items,
        int? NextCursor,// Id of next element (null if no more)
        int PageSize,
        bool HasMore
    );
}
