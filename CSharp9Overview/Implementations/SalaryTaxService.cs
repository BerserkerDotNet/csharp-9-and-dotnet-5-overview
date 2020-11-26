using CSharp9Overview.Interfaces;

namespace CSharp9Overview.Implementations
{
    public class SalaryTaxService : ISalaryTaxService
    {
        private readonly IW9Service _w9Service;

        public SalaryTaxService(IW9Service w9Service)
        {
            _w9Service = w9Service;
        }

        public (float salary, float taxesPaid) GetSalaryInfo()
        {
            return _w9Service.GetW9();
        }
    }
}
