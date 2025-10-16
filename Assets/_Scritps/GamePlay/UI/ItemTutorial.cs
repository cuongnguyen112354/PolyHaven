using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemTutorial : MonoBehaviour
{
    [SerializeField] private TMP_Text descriptionText1;
    [SerializeField] private Image interaction1;

    public void UpdateTutorialUI(TutorialStep step)
    {
        if (step != null)
        {
            descriptionText1.text = step.description;
            interaction1.sprite = step.interaction;
        }
        else
        {
            descriptionText1.text = "";
            interaction1 = null;
        }
    }
}
