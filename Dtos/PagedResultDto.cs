namespace EstudiantesApi.Dtos
{
    public record PagedResultDto<T>(
        List<T> Items,
        int? NextCursor,          // Id del siguiente elemento (null si no hay más)
        int PageSize,
        bool HasMore
    );
}
