using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialPopupTrigger : MonoBehaviour
{
    private const string TutorialContainerString = "TutorialPopup";
    private VisualElement m_Tutorial_container;
    public GameObject m_GameUi;
    private UIDocument root;

    public TutorialPopUpScriptableObject[] PopupScriptableObjects;

    public int popupIndex = 0;

    private void Awake()
    {
        root = m_GameUi.GetComponent<UIDocument>();
        m_Tutorial_container = root.rootVisualElement.Q<VisualElement>(TutorialContainerString);
        // if (PlayerPrefs.GetInt("Tutorial Skip") == 0)
        // {
        //     m_Tutorial_container.style.display = DisplayStyle.Flex;
        // }
        // else
        // {
        //     m_Tutorial_container.style.display = DisplayStyle.None;
        // }
        m_Tutorial_container.style.display = DisplayStyle.Flex;

    }





    void Update()
    {
        // if (popupIndex == 1)
        // {
        //     TutorialSystem.Show(PopupScriptableObjects[popupIndex].content, PopupScriptableObjects[popupIndex].header);
        // }

        if (Input.GetKeyDown(KeyCode.Space) && popupIndex < PopupScriptableObjects.Length - 1)
        {
            popupIndex++;
        }
        TutorialSystem.Show(PopupScriptableObjects[popupIndex].content, PopupScriptableObjects[popupIndex].header);
    }
}
