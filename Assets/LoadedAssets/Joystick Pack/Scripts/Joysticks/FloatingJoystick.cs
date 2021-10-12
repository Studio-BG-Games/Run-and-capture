using LoadedAssets.Joystick_Pack.Scripts.Joysticks;
using UnityEngine.EventSystems;

public class FloatingJoystick : OpacityJoystick
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        base.OnPointerDown(eventData);
    }
}