using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class GameData : Singleton<GameData>
    {
        public int coin;
        public int curLevelId;
        public float musicVol;
        public float soundVol;
        public int hp;
        public int live;
        public int bullet;
        public int key;
        public List<Vector3> checkPoints;
        public List<bool> levelUnlockeds;
        public List<bool> levelPasseds;
        public List<float> playTimes;
        public List<float> completeTimes;

        public void SaveData()
        {
            Pref.GameData = JsonUtility.ToJson(this);
        }
        public void LoadData()
        {
            string data = Pref.GameData;
            if (string.IsNullOrEmpty(data)) return;

            JsonUtility.FromJsonOverwrite(data, this);
        }
        private T GetValue<T>(List<T> dataList, int idx)
        {
            if (dataList == null || dataList.Count <= 0 || idx < 0 || idx > dataList.Count) return default;

            return dataList[idx];
        }

        // Cập nhật giá trị value tại chỉ số idx trong danh sách dataList
        private void UpdateValue<T> (ref List<T> dataList, int idx, T value)
        {
            if (dataList == null || idx < 0) return;

            if(dataList.Count <= 0 || (dataList.Count > 0 && idx >= dataList.Count))
            {
                dataList.Add(value);
            }else
            {
                dataList[idx] = value;
            }
        }
        #region LEVEL
            public bool GetLevelUnlocked(int id)
            {
                return GetValue<bool>(levelUnlockeds, id);
            }

            public void UpdateLevelUnlocked(int id, bool isUnlocked)
            {
                UpdateValue<bool>(ref levelUnlockeds, id, isUnlocked);
            }

            public bool GetLevelPassed(int id)
            {
                return GetValue<bool>(levelPasseds, id);
            }

            public void UpdateLevelPassed(int id, bool isPassed)
            {
                UpdateValue<bool>(ref levelPasseds, id, isPassed);
            }

            public float GetLevelScore(int levelId)
            {
                return GetValue<float>(completeTimes, levelId);
            }

            public void UpdateLevelScore(int levelId, float completeTime)
            {
                float oldCompleteTime = GetLevelScore(levelId);

                if(completeTime < oldCompleteTime)
                    UpdateValue<float>(ref completeTimes, levelId, completeTime);
            }

            public void UpdateLevelScoreNoneCheck(int levelId, float completeTime)
            {
                UpdateValue<float>(ref completeTimes, levelId, completeTime);
            }

            public Vector3 GetCheckPoint(int levelId)
            {
                return GetValue<Vector3>(checkPoints, levelId);
            }

            public void UpdateCheckPoint(int levelId, Vector3 checkPoint)
            {
                UpdateValue<Vector3>(ref checkPoints, levelId, checkPoint);
            }

            public float GetPlayTime(int levelId)
            {
                return GetValue<float>(playTimes, levelId);
            }

            public void UpdatePlayTime(int levelId, float playTime)
            {
                UpdateValue<float>(ref playTimes, levelId, playTime);
            }

            // ####
            public bool IsLevelUnlocked(int id)
            {
                if (levelUnlockeds == null || levelUnlockeds.Count <= 0) return false;

                return levelUnlockeds[id];
            }

            public bool IsLevelPassed(int id)
            {
                if (levelPasseds == null || levelPasseds.Count <= 0) return false;

                return levelPasseds[id];
            }
        #endregion
    }
}

