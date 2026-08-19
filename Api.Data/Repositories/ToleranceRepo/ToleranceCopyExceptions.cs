namespace Api.Data.Repositories.ToleranceRepo;

public sealed class SourceProductNotFoundException : Exception;

public sealed class TargetProductAlreadyExistsException : Exception;

public sealed class InvalidToleranceSelectionException(string message) : Exception(message);
