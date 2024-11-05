namespace MasterData.Domain.Exceptions
{
    public class NotFoundException<T> : Exception
    {
        public NotFoundException() : base($"Item '{typeof(T).Name}' has not been found")
        {
        }
    }
}
