using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DataHandler : MonoBehaviour
{
    private List<Versions> versions => DataPersistence.Instance.settingsData.versions;

    public bool AddVersion(Versions version)
    {
        if (versions.Count >= 3)
        {
            Debug.LogWarning("Đã đạt tối đa 3 phiên chơi!");
            return false;
        }
        versions.Add(version);
        return true;
    }

    public bool RemoveVersion(Versions version)
    {
        return versions.Remove(version);
    }

    public string GetPathDataFile(Versions version)
    {
        return Path.Combine(Application.persistentDataPath, version.nameFile);
    }

    public void SaveVersion(Versions version, GameData data)
    {
        try
        {
            string path = GetPathDataFile(version);
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);
            // Cập nhật thời gian chỉnh sửa
            version.lastModified = DateTime.Now;
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi khi lưu phiên chơi: {e.Message}");
        }
    }

    public GameData LoadVersion(Versions version)
    {
        try
        {
            string path = GetPathDataFile(version);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<GameData>(json);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi khi tải phiên chơi: {e.Message}");
            return null;
        }
    }

    public void DeleteVersion(Versions version)
    {
        try
        {
            string path = GetPathDataFile(version);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            versions.Remove(version);
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi khi xóa phiên chơi: {e.Message}");
        }
    }

    // public Versions CreateNewVersion(string versionName)
    public void CreateNewVersion()
    {
        int index = versions.Count + 1;
        if (index > 3)
        {
            Debug.LogWarning("Không thể tạo thêm phiên chơi, đã đạt tối đa!");
            // return null;
            return;
        }
        Versions version = new($"data{index}", $"{DataPersistence.Instance.selectedVersion.nameVersion}");
        version.lastModified = DateTime.Now;
        // return version;
        versions.Add(version);
    }

    // Phương thức mới: Lấy danh sách phiên chơi đã sắp xếp theo thời gian gần nhất
    public List<Versions> GetSortedVersions()
    {
        return versions.OrderByDescending(v => v.lastModified).ToList();
    }
}
