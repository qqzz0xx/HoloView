using UnityEngine;
using System.Collections;
using Microsoft.MixedReality.Toolkit.UI;

public class InteractableToggleCollectionExt : InteractableToggleCollection
{

    public void SetToggleItemAt(int i, Interactable it)
    {
        if (i >= 0 && i < ToggleList.Length)
        {
            ToggleList[i] = it;
            int itemIndex = i;
            ToggleList[i].OnClick.AddListener(() => OnSelection(i));
            ToggleList[i].CanDeselect = false;
        }
    }

    protected override void OnSelection(int index, bool force = false)
    {
        for (int i = 0; i < ToggleList.Length; ++i)
        {
            if (i != index)
            {
                ToggleList[i].SetDimensionIndex(0);
            }
        }

        CurrentIndex = index;

        if (force)
        {
            if (ToggleList.Length != 0)
            {
                ToggleList[index].SetDimensionIndex(1);
            }
        }
        else
        {
            OnSelectionEvents.Invoke();
        }
    }
}
