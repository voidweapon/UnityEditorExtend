using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class ContentFixedSizeScaleFiter : UIBehaviour, ILayoutSelfController
    {
        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }

        [SerializeField] protected FitMode m_HorizontalFit = FitMode.Unconstrained;
        public FitMode horizontalFit { get { return m_HorizontalFit; } set { m_HorizontalFit = value; SetDirty(); } }

        [SerializeField] protected FitMode m_VerticalFit = FitMode.Unconstrained;
        public FitMode verticalFit { get { return m_VerticalFit; } set { m_VerticalFit = value; SetDirty(); } }

        [SerializeField] private Vector2 m_fixedSize;

        protected DrivenRectTransformTracker m_Tracker;

        [System.NonSerialized] private RectTransform m_Rect;
        protected RectTransform rectTsfm
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_Tracker.Clear();

            rectTsfm.localScale = Vector3.one;
            rectTsfm.sizeDelta = m_fixedSize;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_Tracker.Clear();
        }


        protected void SetDirty()
        {
            if (!IsActive())
                return;

            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }


        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
            if (fitting == FitMode.Unconstrained)
            {
                // Keep a reference to the tracked transform, but don't control its properties:
                m_Tracker.Add(this, rectTsfm, DrivenTransformProperties.None);
                return;
            }

            DrivenTransformProperties flags = DrivenTransformProperties.ScaleX |DrivenTransformProperties.ScaleY;
            if(horizontalFit != FitMode.Unconstrained)
            {
                flags |= DrivenTransformProperties.SizeDeltaX;
            }
            if (verticalFit != FitMode.Unconstrained)
            {
                flags |= DrivenTransformProperties.SizeDeltaY;
            }
            m_Tracker.Add(this, rectTsfm, flags);

            // Set size to min or preferred size
            float newSize = 0f;
            if (fitting == FitMode.MinSize)
            {
                newSize = LayoutUtility.GetMinSize(m_Rect, axis);
                newSize = axis == 0 ? Mathf.Max(newSize, m_fixedSize.x) : Mathf.Max(newSize, m_fixedSize.y);
            }
            else
            {
                newSize = LayoutUtility.GetPreferredSize(m_Rect, axis);
            }


            Vector2 newSizeV;
            if (axis == 0)
            {
                newSizeV = new Vector2(Mathf.Max(newSize, m_fixedSize.x), m_Rect.sizeDelta.y);
            }
            else
            {
                newSizeV = new Vector2(m_Rect.sizeDelta.x, Mathf.Max(newSize, m_fixedSize.y));
            }
            m_Rect.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, newSize);
            m_Rect.localScale = CalculateSizeScale(newSizeV, axis);
        }


        private Vector3 CalculateSizeScale(Vector2 newSize, int axis)
        {
            Vector2 newScale = m_fixedSize / newSize;

            return new Vector3(newScale.x > 1f ? 1f : newScale.x, newScale.y > 1f ? 1f : newScale.y, 1f);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            SetDirty();
        }
#endif
    }

}