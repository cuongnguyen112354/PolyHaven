using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0, 24)]
    public float timeOfDay = 12f;  // giờ trong game (0-24)
    public float dayLength = 120f; // thời gian 1 ngày trong game (tính bằng giây)
    public Light sunLight;         // Directional Light làm mặt trời
    public Gradient sunColor;      // Chuyển màu ánh sáng theo thời gian

    private void Update()
    {
        // Tiến thời gian
        timeOfDay += (24f / dayLength) * Time.deltaTime;
        if (timeOfDay >= 24f) timeOfDay = 0;

        // Xoay mặt trời (0h = nửa đêm, 12h = giữa trưa)
        float sunRotation = (timeOfDay / 24f) * 360f - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunRotation, 170f, 0);

        // Đổi màu ánh sáng theo thời gian (nếu có Gradient setup)
        if (sunColor != null)
        {
            sunLight.color = sunColor.Evaluate(timeOfDay / 24f);
        }
    }
}
