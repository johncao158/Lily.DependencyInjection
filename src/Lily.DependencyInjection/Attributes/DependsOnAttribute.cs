namespace Lily.DependencyInjection
{
    /// <summary>
    /// Defines the types on which a class depends.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class
        /// with specified <paramref name="dependedTypes"/>.
        /// </summary>
        /// <param name="dependedTypes"></param>
        public DependsOnAttribute(params Type[] dependedTypes)
        {
            DependedTypes = dependedTypes ?? Array.Empty<Type>();
        }

        /// <summary>
        /// Gets the depended types.
        /// </summary>
        public Type[] DependedTypes { get; }
    }
}
