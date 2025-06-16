using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CommandInOutView : Button
    {
        public void Setup(bool isInteractable)
        {
            interactable = isInteractable;
        }

        public void RefreshView(bool bit)
        {
            image.color = bit ? colors.selectedColor : colors.disabledColor;
        }
    }
}