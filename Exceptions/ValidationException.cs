namespace StudentsApi.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<FluentValidation.Results.ValidationFailure> Errors { get; }

        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
