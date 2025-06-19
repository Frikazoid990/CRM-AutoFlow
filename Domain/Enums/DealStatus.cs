using System.ComponentModel;

public enum DealStatus
{
    [Description("Менеджер не назначен")]
    NOTASSIGNED,
    [Description("Новая")]
    NEW,
    [Description("Консультация")]
    CONSULTATION,
    [Description("Подписание договора")]
    SIGNING,
    [Description("Доставка/Подготовка автомобиля")]
    CARDELIVERY,
    [Description ("Завершена")]
    COMPLETED
}