using CSharp9Overview.Interfaces;

namespace CSharp9Overview.Implementations
{
    public class W9Service 
    {
        public (float salary, float taxesPaid) GetW9()
        {
            return (150_000, 30_000);
        }
    }
}
