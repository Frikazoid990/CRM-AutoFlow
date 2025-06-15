using System.ComponentModel;

public enum DealStatus
{
    [Description("Не назначена")]
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