namespace AdsManagement.App.Exceptions
{
    public abstract class EntityNotFoundException : Exception
    {
        public string EntityName { get; }  
        public Guid Key { get; }

        protected EntityNotFoundException(string entityName, Guid key) : base($"{entityName} with key '{key}' was not found.")
        {
            EntityName = entityName;
            Key = key;
        }
    }
}
