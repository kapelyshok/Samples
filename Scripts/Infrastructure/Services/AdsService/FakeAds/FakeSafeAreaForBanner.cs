using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicApps
{
    [RequireComponent(typeof(RectTransform))]
    public class FakeSafeAreaForBanner : MonoBehaviour
    {
                RectTransform m_RectTransform;
        RectTransform m_ParentRectTransform;
        Canvas m_Canvas;

        Vector2Int m_PreviousParentSize = Vector2Int.zero;

        void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentRectTransform = m_RectTransform.parent as RectTransform;
        }

        void Start()
        {
            Transform transform = m_ParentRectTransform;
            while (transform)
            {
                m_Canvas = transform.GetComponent<Canvas>();
                if (m_Canvas)
                    break;

                transform = transform.parent;
            }

            UpdateTransform();
        }
        
        void Update()
        {
            if (m_ParentRectTransform.hasChanged)
            {
                CheckTransform();
            }
        }
        
        void CheckTransform()
        {
            if (m_ParentRectTransform == null)
                return;

            int parentWidth = (int)(m_ParentRectTransform.rect.width * m_ParentRectTransform.transform.lossyScale.x);
            int parentHeight = (int)(m_ParentRectTransform.rect.height * m_ParentRectTransform.transform.lossyScale.y);

            if (m_PreviousParentSize.x != parentWidth ||
                m_PreviousParentSize.y != parentHeight)
            {
                StartCoroutine(UpdateTransformRoutine());

                m_PreviousParentSize = new Vector2Int(
                    parentWidth,
                    parentHeight);
            }
        }

        IEnumerator UpdateTransformRoutine()
        {
            yield return new WaitForEndOfFrame();
            UpdateTransform();
        }

        void UpdateTransform()
        {
            if (m_RectTransform == null)
                return;
            
            bool isVerticalOrientation = (Screen.width < Screen.height);

            Rect sourceRect = GetSourceRect();
            Rect safeArea = GetScreenSafeArea();

            float scaleW = safeArea.width / sourceRect.width;
            float scaleH = safeArea.height / sourceRect.height;

            if (isVerticalOrientation)
            {
                float scale = scaleW;
                m_RectTransform.sizeDelta = new Vector2(0.0f, m_ParentRectTransform.rect.height * (scaleH / scale - 1.0f));
                m_RectTransform.localScale = new Vector3(scale, scale, 1.0f);
            }
            else
            {
                float scale = scaleH;
                m_RectTransform.sizeDelta = new Vector2(m_ParentRectTransform.rect.width * (scaleW / scale - 1.0f), 0.0f);
                m_RectTransform.localScale = new Vector3(scale, scale, 1.0f);
            }

            Vector2 screenSize = new Vector2((float)Screen.width, (float)Screen.height);
            Vector2 screenCenter = screenSize * 0.5f;
            Vector2 safeAreaShift = safeArea.center - screenCenter;
            m_RectTransform.localPosition = safeAreaShift;
        }

        Rect GetSourceRect()
        {
            return new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));
        }

        Rect GetScreenSafeArea()
        {
#if (UNITY_EDITOR) && (false)
            // Test
            if (Application.isEditor)
            {
                float rx = 0.1f;
                Rect fakeSafeArea = new Rect(
                    0,
                    rx * Screen.width,
                    Screen.width - 0,
                    Screen.height - Screen.width * rx * 2.0f);

                return fakeSafeArea;
            }
#endif

            return Screen.safeArea;
        }

        void OnValidate()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    rectTransform.sizeDelta = Vector2.zero;
                }
            }
        }
    }
}
