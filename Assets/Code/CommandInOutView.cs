using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CommandInOutView : Button
    {
        public bool value { get; private set; }

        public void Setup(bool isInteractable) => interactable = isInteractable;

        public void RefreshView(bool bit)
        {
            value = bit;
            image.color = value ? colors.selectedColor : colors.disabledColor;
        }
    }
}