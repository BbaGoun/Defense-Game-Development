using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sangmin
{
    public static class Utility
    {
        public static void DrawBox(Vector3 position, Vector2 size, Color color)
        {
            Vector2 center = new Vector2(position.x, position.y);
            var sizeX = size.x;
            var sizeY = size.y;
            Debug.DrawLine(center + new Vector2(-sizeX, sizeY) / 2, center + new Vector2(sizeX, sizeY) / 2, color);
            Debug.DrawLine(center + new Vector2(sizeX, sizeY) / 2, center + new Vector2(sizeX, -sizeY) / 2, color);
            Debug.DrawLine(center + new Vector2(sizeX, -sizeY) / 2, center + new Vector2(-sizeX, -sizeY) / 2, color);
            Debug.DrawLine(center + new Vector2(-sizeX, -sizeY) / 2, center + new Vector2(-sizeX, sizeY) / 2, color);
        }

        public static void SetRectLeft(RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRectRight(RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetRectTop(RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetRectBottom(RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static void SetRectWidthHeight(RectTransform rt, float width, float height)
        {
            rt.sizeDelta = new Vector2(width, height);
        }

        public static float GetRectLeft(RectTransform rt)
        {
            return rt.offsetMin.x;
        }
        public static float GetRectRight(RectTransform rt)
        {
            return -rt.offsetMax.x;
        }
        public static float GetRectBottom(RectTransform rt)
        {
            return rt.offsetMin.y;
        }
        public static float GetRectTop(RectTransform rt)
        {
            return -rt.offsetMax.y;
        }
    }
}