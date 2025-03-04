using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UDEV.PlatfromGame
{
    public static class Helper
    {
        #region Animation
        public static void PlayAnim(Animator anim, string stateName, int layerIndex = 0)
        {
            if (IsAnimCanPlayState(anim, stateName, layerIndex))
            {
                anim.Play(stateName);
            }
        }

        public static bool IsAnimStateActive(Animator animator, string stateName, int layerIndex = 0)
        {
            if (animator)
                return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);

            return true;
        }

        public static bool IsAnimCanPlayState(Animator animator, string stateName, int layerIndex = 0)
        {
            if (animator)
                return !IsAnimStateActive(animator, stateName, layerIndex)
                && animator.HasState(layerIndex, Animator.StringToHash(stateName));

            return false;
        }

        public static AnimationClip GetClip(Animator anim, string stateName)
        {
            if (anim)
            {
                int maxState = anim.runtimeAnimatorController.animationClips.Length;

                var states = anim.runtimeAnimatorController.animationClips;

                for (int i = 0; i < maxState; i++)
                {
                    if (string.Compare(states[i].name, stateName) == 0)
                    {
                        return states[i];
                    }
                }
            }

            return null;
        }

        #endregion

        public static string TimeConvert(double time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);

            return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }

        public static void ClearChilds(Transform root)
        {
            if (root)
            {
                int childs = root.childCount;

                if (childs > 0)
                {
                    for (int i = 0; i < childs; i++)
                    {
                        var child = root.GetChild(i);

                        if (child)
                            MonoBehaviour.Destroy(child.gameObject);
                    }
                }
            }
        }

        public static Vector2 Get2DCamSize()
        {
            return new Vector2(2f * Camera.main.aspect * Camera.main.orthographicSize, 2f * Camera.main.orthographicSize);
        }

        public static Color ChangAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static void FitSpriteToScreen(SpriteRenderer sp, bool resetScale = true, bool fitX = true, bool fixY = true, float offsetX = 0, float offsetY = 0)
        {
            if (resetScale)
                sp.transform.localScale = Vector3.one;

            var width = sp.sprite.bounds.size.x;
            var height = sp.sprite.bounds.size.y;

            var worldScreenHeight = Camera.main.orthographicSize * 2.0;
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            double scaleX = worldScreenWidth / width;
            double scaleY = worldScreenHeight / height;

            if (fitX)
                sp.transform.localScale = new Vector3((float)scaleX + offsetX, sp.transform.localScale.y + offsetY, sp.transform.localScale.z);

            if (fixY)
                sp.transform.localScale = new Vector3(sp.transform.localScale.x + offsetX, (float)scaleY + offsetY, sp.transform.localScale.z);
        }
    }
}

