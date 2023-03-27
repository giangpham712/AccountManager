namespace AccountManager.Common
{
    public class Patch<T> : Patch
    {
        public Patch(T value, bool patchable) : this(null, value, patchable)
        {
        }

        public Patch(string name, T value, bool patchable) : base(name, value, patchable)
        {
        }
    }

    public class Patch
    {
        protected Patch(string name, object value, bool patchable)
        {
            Name = name;
            Value = value;
            Patchable = patchable;
        }

        public string Name { get; set; }
        public bool Patchable { get; protected set; }
        public object Value { get; protected set; }
    }
}