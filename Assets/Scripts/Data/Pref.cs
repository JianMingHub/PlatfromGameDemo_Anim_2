using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public static class Pref
    {
        public static bool IsFirstTime
        {
            set => SetBool(GamePref.IsFirstTime.ToString(), value);
            get => GetBool(GamePref.IsFirstTime.ToString(), true);
        }
        public static string GameData
        {
            set => PlayerPrefs.SetString(GamePref.GameData.ToString(), value);
            get => PlayerPrefs.GetString(GamePref.GameData.ToString());
        }
        public static void SetBool(string key, bool isOn)
        {
            PlayerPrefs.SetInt(key, isOn ? 1 : 0); // Nếu giá trị value là true, nó sẽ lưu thành 1. Nếu là false, nó sẽ lưu thành 0.
        }
        // Lấy giá trị bool từ PlayerPrefs đã lưu dưới dạng số nguyên.
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.HasKey(key) ?    // Kiểm tra xem trong PlayerPrefs có tồn tại khóa (key) này hay không.
                PlayerPrefs.GetInt(key) == 1 ? true : false : defaultValue; // Nếu không có: Trả về giá trị mặc định defaultValue, thường là false nếu không được chỉ định giá trị khác.
        }
    }
}