using CSharp9Overview.Interfaces;

namespace CSharp9Overview.Implementations
{
    public class PropertyService : IPropertyTaxService
    {
        public float PropertyValue()
        {
            return 500_000;
        }
    }
}
