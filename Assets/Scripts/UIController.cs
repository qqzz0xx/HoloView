using Microsoft.MixedReality.Toolkit.UI;
using nn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public PinchSlider pinchSlider;
    public Interactable showhideButton;

    private GameObject pickedGameObject;

    public GameObject PickedGameObject
    {
        get => pickedGameObject;
        set
        {
            pickedGameObject = value;
            UpdateSliderValue();
        }
    }

    void Start()
    {

    }
    void UpdateSliderValue()
    {
        if (pickedGameObject != null)
        {
            var renderer = pickedGameObject.GetComponent<Renderer>();
            if (renderer == null)
            {
                renderer = pickedGameObject.transform.GetChild(0).GetComponent<Renderer>();
            }
            var v = renderer.material.color.a;

            pinchSlider.SliderValue = v;
        }
    }

    public void OnResetCliked()
    {
        var root = MainApp.Inst.MeshRoot;
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        foreach (Transform child in root.transform)
        {
            if (child.gameObject.name != "rigRoot")
            {
                child.transform.localPosition = Vector3.zero;
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
            }
        }
    }
    public void OnUpdateAphla(SliderEventData eventData)
    {
        if (pickedGameObject != null)
        {
            float v = eventData.NewValue;
            var renderer = pickedGameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                var material = renderer.material;
                material.color = new Color(material.color.r,
                    material.color.g,
                    material.color.b,
                    v);
            }
            else
            {
                foreach (Transform child in pickedGameObject.transform)
                {
                    if (child.gameObject.name != "rigRoot")
                    {
                        renderer = child.GetComponent<Renderer>();
                        var material = renderer.material;
                        material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        v);
                    }
                }
            }

        }
    }

    public void OnShowHideCliked()
    {
        if (pickedGameObject != null)
        {
            pickedGameObject.SetActive(!pickedGameObject.activeSelf);
        }
    }


}
