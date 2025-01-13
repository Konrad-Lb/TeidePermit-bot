namespace PermitService.Sources
{
    public interface ITeideWebPageClawler
    {
        void ClickNextMonthLink();
        void ClickNextStepLink();
        string PageSource { get; }
    }
}