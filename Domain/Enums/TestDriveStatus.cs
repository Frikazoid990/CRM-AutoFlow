using System.ComponentModel;
using System.Reflection;

public enum TestDriveStatus
{
    [Description("Менеджер не назначен")]
    NOTASSIGNED,
    [Description("В процессе")]
    INITIAL,
    [Description("Завершен")]
    COMPLETED,
    [Description("Отменен")]
    CANCELED
}
