using System;

public static class PlayerEvents
{
    public static Action<ItemData> OnEquipRequest;
    public static Action<AttachPoint> OnUnequipRequest;
    public static Action<ItemData> OnTogglePreviewRequest;
    public static Action OnClearPreviewRequest;
}