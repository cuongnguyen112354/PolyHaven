using TMPro;
using UnityEngine;

public class VersionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text versionName; 

    public Versions data;

    public void Init(Versions version)
    {
        data = version;

        versionName.text = data.nameVersion;
        gameObject.SetActive(true);
    }

    public void ChooseGameData()
    {
        DataPersistence.Instance.SetVersionsSelected(data);
        DataPersistence.Instance.LoadGameData();

        MainMenu.Instance.InteractiveEnterBtn();
    }
}
