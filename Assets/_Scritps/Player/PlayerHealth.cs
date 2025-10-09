using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [HideInInspector] public bool isResting = false;
    public PlayerState currentState = PlayerState.Normal;

    public enum PlayerState
    {
        Normal,
        Weak,
    }

    [Header("----------Health Slider----------")]
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider restSlider;

    [Header("----------Index Decrease/Increase Timer----------")]
    [SerializeField] private float hungerDecreaseTimer = 2f; // Giảm 2 Đói/phút
    [SerializeField] private float restDecreaseTimer = 1f;   // Giảm 1 Nghỉ/phút
    [SerializeField] private float restIncreaseTimer = .5f;   // Giảm 1 Nghỉ/phút

    private float hungerTimer = 0f;
    private float restTimer = 0f;

    private PlayerData playerData;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        // Giảm Hunger theo thời gian
        hungerTimer += Time.deltaTime;
        if (hungerTimer >= hungerDecreaseTimer)
        {
            UpdateHunger(-1);
            hungerTimer = 0f;
        }

        if (!isResting)
        {
            // Giảm Rest theo thời gian
            restTimer += Time.deltaTime;
            if (restTimer >= restDecreaseTimer)
            {
                UpdateRest(-1);
                restTimer = 0f;
            }
        }
        else
        {
            restTimer += Time.deltaTime;
            if (restTimer >= restIncreaseTimer)
            {
                UpdateRest(1);
                restTimer = 0f;
            }
        }
        
        if (playerData.hunger <= 10 || playerData.rest <= 10)
            currentState = PlayerState.Weak;
        else
            currentState = PlayerState.Normal;
    }

    public void UpdateHunger(int value)
    {
        playerData.hunger = Mathf.Clamp(playerData.hunger + value, 1, playerData.maxHunger);

        hungerSlider.value = playerData.hunger;
    }

    public void UpdateRest(int value)
    {
        playerData.rest = Mathf.Clamp(playerData.rest + value, 1, playerData.maxRest);

        restSlider.value = playerData.rest;
    }

    // SAVE AND LOAD DATA
    public void Init(PlayerData data)
    {
        playerData = data;

        hungerSlider.maxValue = playerData.maxHunger;
        restSlider.maxValue = playerData.maxRest;

        hungerSlider.value = playerData.hunger;
        restSlider.value = playerData.rest;

        transform.position = playerData.position;
        transform.rotation = Quaternion.Euler(playerData.rotation);
    }

    public PlayerData GetPlayerData()
    {
        playerData.position = transform.position;
        playerData.rotation = transform.rotation.eulerAngles;

        return playerData;
    }
}

[System.Serializable]
public class PlayerIndex
{
    public IndexType key;
    public int value;

    public enum IndexType
    {
        hunger,
        rest
    }
}

[System.Serializable]
public class PlayerData
{
    public int maxHunger = 100;
    public int maxRest = 100;

    public int hunger;
    public int rest;

    public Vector3 position;
    public Vector3 rotation;

    public PlayerData()
    {
        hunger = maxHunger;
        rest = maxRest;

        position = new Vector3(20, 3, 0);
        rotation = new Vector3(0, -90, 0);
    }
}