namespace HistoryPoC.ViewModels;

public static class Mixin
{
    public static int GroupId(this TransactionModel model)
    {
        return model.ParentId ?? -model.Id;
    }
}