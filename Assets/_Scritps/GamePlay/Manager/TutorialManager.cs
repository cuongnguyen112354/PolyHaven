using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private List<ItemTutorial> itemTutorials;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void HideAllTutorials()
    {
        foreach (var tutorial in itemTutorials)
        {
            tutorial.gameObject.SetActive(false);
        }
    }

    public void ShowTutorial(List<TutorialStep> steps)
    {
        for (int i = 0; i < itemTutorials.Count; i++)
        {
            if (i < steps.Count)
            {
                itemTutorials[i].gameObject.SetActive(true);
                itemTutorials[i].UpdateTutorialUI(steps[i]);
            }
            else
            {
                itemTutorials[i].gameObject.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public class TutorialStep
{
    public string description;
    public Sprite interaction;
}