namespace moonNest
{
    public abstract class BaseDetail<T> : BaseData where T : BaseDefinition
    {
        public int definitionId;

        public T Definition => GetDefinition(definitionId);

        protected abstract T GetDefinition(int definitionId);

        public BaseDetail(T definition) : base(definition.name)
        {
            definitionId = definition.id;
        }
    }
}