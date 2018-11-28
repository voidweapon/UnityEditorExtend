using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI
{
    public class ParallelogramLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        public float OffsetZ { get { return m_offsetZ; } set { SetProperty(ref m_offsetZ, value); } }
        [SerializeField] private float m_offsetZ;

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, true);
        }

        public override void SetLayoutHorizontal()
        {
            SetParallelogramAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            //throw new System.NotImplementedException();
            SetParallelogramAlongAxis(1);
        }

        private void SetParallelogramAlongAxis(int axis)
        {
            float size = rectTransform.rect.size[axis];
            bool controlSize = (axis == 0 ? m_ChildControlWidth : m_ChildControlHeight);
            bool childForceExpandSize = (axis == 0 ? childForceExpandWidth : childForceExpandHeight);
            float alignmentOnAxis = GetAlignmentOnAxis(axis);

            bool alongOtherAxis = (true ^ (axis == 1));
            float posY = 0;
            if (alongOtherAxis)
            {
                float innerSize = size;
                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform child = rectChildren[i];
                    float min, preferred, flexible;
                    GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);

                    float requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
                    float startOffset = GetStartOffset(axis, requiredSpace);
                    if (controlSize)
                    {
                        SetChildAlongAxis(child, axis, startOffset + posY, requiredSpace);
                    }
                    else
                    {
                        float offsetInCell = (requiredSpace - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxis(child, axis, startOffset + offsetInCell + posY);
                        SetChildAlongAxis(child, 1, child.sizeDelta[1]*child.localScale.y);
                    }

                    posY += m_offsetZ;
                }
            }
            else
            {
                float pos = (axis == 0 ? padding.left : padding.top);
                if (GetTotalFlexibleSize(axis) == 0 && GetTotalPreferredSize(axis) < size)
                    pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical));

                float minMaxLerp = 0;
                if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
                    minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

                float itemFlexibleMultiplier = 0;
                if (size > GetTotalPreferredSize(axis))
                {
                    if (GetTotalFlexibleSize(axis) > 0)
                        itemFlexibleMultiplier = (size - GetTotalPreferredSize(axis)) / GetTotalFlexibleSize(axis);
                }

                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform child = rectChildren[i];
                    float min, preferred, flexible;
                    GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);

                    float childSize = Mathf.Lerp(min, preferred, minMaxLerp) * child.localScale.y;
                    childSize += flexible * itemFlexibleMultiplier;
                    if (controlSize)
                    {
                        SetChildAlongAxis(child, axis, pos, childSize);
                    }
                    else
                    {
                        float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxis(child, axis, pos + offsetInCell);
                    }
                    pos += childSize + spacing;
                }
            }
        }

        private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
            out float min, out float preferred, out float flexible)
        {
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }

            if (childForceExpand)
                flexible = Mathf.Max(flexible, 1);
        }
    }


}

