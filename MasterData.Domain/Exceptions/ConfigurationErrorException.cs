namespace MasterData.Domain.Exceptions;

public class ConfigurationErrorException(string error) : Exception(error)
{
}
