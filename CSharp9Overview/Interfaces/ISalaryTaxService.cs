namespace CSharp9Overview.Interfaces
{
    public interface ISalaryTaxService
    {
        (float salary, float taxesPaid) GetSalaryInfo();
    }
}
