using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemTutorial : MonoBehaviour
{
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image interaction;

    public void UpdateTutorialUI(TutorialStep step)
    {
        if (step != null)
        {
            descriptionText.text = step.description;
            interaction.sprite = step.interaction;
        }
        else
        {
            descriptionText.text = "";
            interaction = null;
        }
    }
}
