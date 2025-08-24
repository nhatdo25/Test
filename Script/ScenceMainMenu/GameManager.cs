using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Data")]
    public string playerName = "Player";
    public int level = 1;
    public int exp = 0;
    public PlayerStats player;
    [Header("Settings")]
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    [Header("All Class Definitions")]
    [Tooltip("Kéo tất cả các file ClassDefinition asset của bạn vào đây")]
    public List<ClassDefinition> allClassDefinitions;
    public ClassDefinition playerClass { get; private set; } // Dữ liệu class đã chọn
    public string selectedClassName { get; private set; } // Tên class đã chọn
    public GameObject playerPrefab { get; private set; } // Prefab của class đã chọn

    private string saveDir;
    private string savePath;
    private string key = "my_secret_key_123"; // 🔑 Khóa AES 
    //public ClassDefinition playerClass;
    //public string selectedClassName;
    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 📂 Tạo folder ẩn trong persistentDataPath
        saveDir = Path.Combine(Application.persistentDataPath, ".playerdata");
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            File.SetAttributes(saveDir, FileAttributes.Hidden); // Ẩn trên Windows
#endif
        }

        savePath = Path.Combine(saveDir, "data.sav");

        // 🔄 Load data & settings khi mở game
        LoadGame();
        LoadSettings();
    }

    // ================== SAVE / LOAD GAME ==================
    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            playerName = this.playerName,
            level = this.level,
            exp = this.exp
        };

        string json = JsonUtility.ToJson(data);
        string encrypted = Encrypt(json, key);

        File.WriteAllText(savePath, encrypted);
        Debug.Log("✅ Đã lưu game (mã hóa).");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string encrypted = File.ReadAllText(savePath);
            string json = Decrypt(encrypted, key);

            SaveData data = JsonUtility.FromJson<SaveData>(json);
            this.playerName = data.playerName;
            this.level = data.level;
            this.exp = data.exp;

            Debug.Log("✅ Đã load dữ liệu người chơi.");
        }
        else
        {
            Debug.Log("⚠️ Không tìm thấy save, tạo mới.");
            SaveGame();
        }
    }

    // ================== SAVE / LOAD SETTINGS ==================
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();

        Debug.Log("✅ Đã lưu settings.");
    }

    public void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    // ================== AES ENCRYPTION ==================
    private string Encrypt(string plainText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            aes.IV = new byte[16]; // IV mặc định = 0

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            return System.Convert.ToBase64String(encrypted);
        }
    }

    private string Decrypt(string cipherText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            aes.IV = new byte[16];

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] bytes = System.Convert.FromBase64String(cipherText);
            byte[] decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
    public void SetPlayerClass(string className)
    {
        selectedClassName = className;

        // Tìm ClassDefinition tương ứng trong danh sách
        playerClass = allClassDefinitions.FirstOrDefault(c => c.classType.ToString() == className);

        if (playerClass != null)
        {
            Debug.Log($"Đã chọn và tải dữ liệu cho class: {className}");
            // (Bạn có thể thêm logic gán playerPrefab ở đây nếu mỗi class có prefab khác nhau)
        }
        else
        {
            Debug.LogError($"LỖI: Không tìm thấy ClassDefinition cho class có tên '{className}' trong danh sách allClassDefinitions của GameManager!");
        }
    }

    // Hàm cộng chỉ số
    public void IncreaseStat(string statName, int value)
    {
        if (playerClass == null)
        {
            Debug.LogWarning("Chưa có class nào được chọn!");
            return;
        }

        if (!playerClass.CanIncrease(statName))
        {
            Debug.LogWarning($"{playerClass.classType} không thể tăng {statName}");
            return;
        }

        var stats = playerClass.baseStats;
        switch (statName)
        {
            case "HP": stats.HP += value; break;
            case "Mana": stats.Mana += value; break;
            case "PHI": stats.PHI += value; break;
            case "MAG": stats.MAG += value; break;
            case "ARP": stats.ARP += value; break;
            case "ARM": stats.ARM += value; break;
            case "CRITR": stats.CRITR += value; break;
            case "CRITD": stats.CRITD += value; break;
        }
    }
    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("❌ Save file đã bị xóa.");
        }
    }
}

[System.Serializable]
public class SaveData
{
    public string playerName;
    public int level;
    public int exp;
}
