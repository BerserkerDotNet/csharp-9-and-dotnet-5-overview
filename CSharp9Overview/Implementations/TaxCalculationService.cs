using CSharp9Overview.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp9Overview.Implementations
{
    public class TaxCalculationService: ITaxCalculationService
    {
        private readonly ISalaryTaxService _salaryService;
        private readonly IPropertyTaxService _propertyService;
        private readonly IAllowanceService _allowances;

        public TaxCalculationService(ISalaryTaxService salaryService, IPropertyTaxService propertyService, IAllowanceService allowances)
        {
            _salaryService = salaryService;
            _propertyService = propertyService;
            _allowances = allowances;
        }

        public void Calculate()
        {
            (float salary, float taxesPaid) = _salaryService.GetSalaryInfo();
            var propertyValue = _propertyService.PropertyValue();
            var allowances = _allowances.GetAllowances();

            Console.WriteLine($"Salary: ${salary}; Taxes paid during the year: ${taxesPaid}");
            Console.WriteLine($"Property value: {propertyValue}; Property tax: ${propertyValue * 0.11f}");
            Console.WriteLine($"Allowances: {allowances};");
            var remainingTax = ((salary * 0.25) + (propertyValue * 0.01f)) - taxesPaid;

            if (remainingTax < 0)
            {
                Console.WriteLine($"IRS will send you a check for ${remainingTax}.");
            }
            else
            {
                Console.WriteLine($"You owe IRS ${remainingTax}.");
            }
        }
    }
}
