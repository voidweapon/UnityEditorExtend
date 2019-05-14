using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class HexagonMetrics
    {
        public float    OuterRadius {
            get { return m_outerRadius; }
            set {
                m_outerRadius = value;
                m_innerRadius = m_outerRadius * 0.866025404f;
                m_outerDiameter = 2f * m_outerRadius;
                m_innerDiameter = 2f * m_innerRadius;
            }
        }

        [SerializeField] private float   m_outerRadius;
        public float    InnerRadius { get { return m_innerRadius; } }
        [SerializeField] private float   m_innerRadius;

        public float    InnerDiameter { get { return m_innerDiameter; } }
        [SerializeField] private float   m_innerDiameter;
        public float    OuterDiameter { get { return m_outerDiameter; } }
        [SerializeField] private float   m_outerDiameter;
    }
    [System.Serializable]
    public class HexCoordinates
    {
        public Text label;

        [SerializeField] private int x;
        public int X { get { return x; } private set { x = value; } }

        [SerializeField] private int z;
        public int Z { get { return z; } private set { z = value; } }

        public HexCoordinates(int x, int z)
        {
            X = x;
            Z = z;
        }

        public void OffsetCoordinates(int x, int z)
        {
            X = x - z / 2;
            Z = z;
            //if (label)
            //{
            //    label.text = X + "\n" + Z;
            //}
        }
        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }
    }



    [System.Serializable]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class HexagonLayoutGroup : LayoutGroup
    {
        public enum Corner { UpperLeft = 0, UpperRight = 1, LowerLeft = 2, LowerRight = 3 }

        [SerializeField] protected float m_CellSize = 100f;
        public float cellSize { get { return m_CellSize; } set { SetProperty(ref m_CellSize, value); m_hexagonMetrics.OuterRadius = value / 2f; } }
        [SerializeField] protected Corner m_StartCorner = Corner.UpperLeft;
        public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }
        [SerializeField] protected float m_Spacing = 0;
        public float spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }
  
        public List<HexCoordinates> ChildHexCoordinates { get { return m_childHexCoordinates; } }
        [HideInInspector] [SerializeField] private List<HexCoordinates> m_childHexCoordinates = new List<HexCoordinates>();

        public int cellCountX { get { return m_cellCountX; } }
        [HideInInspector] [SerializeField] private int m_cellCountX;

        public int cornerX { get { return m_cornerX; } }
        [HideInInspector] [SerializeField] private int m_cornerX;
        public int cornerY { get { return m_cornerY; } }
        [HideInInspector] [SerializeField] private int m_cornerY;
        public int actualCellCountX { get { return m_actualCellCountX; } }
        [HideInInspector] [SerializeField] private int m_actualCellCountX;
        public int actualCellCountY { get { return m_actualCellCountY; } }
        [HideInInspector] [SerializeField] private int m_actualCellCountY;

        [SerializeField]  private HexagonMetrics m_hexagonMetrics = new HexagonMetrics();
        [SerializeField]  private HexagonMetrics m_hexagonMetricsWithSapcing = new HexagonMetrics();



#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            cellSize = cellSize;
            m_hexagonMetricsWithSapcing.OuterRadius = m_hexagonMetrics.OuterRadius + spacing;
        }

#endif


        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            m_childHexCoordinates.Clear();
            for (int i = 0; i < rectChildren.Count; i++)
            {
                HexCoordinates t = HexCoordinates.FromOffsetCoordinates(0, 0);
                t.label = rectChildren[i].GetComponentInChildren<Text>();
                m_childHexCoordinates.Add(t);
            }

            int minColumns = 0;
            int preferredColumns = 0;
            int spacing = 0;

            minColumns = 1;
            preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));


            SetLayoutInputForAxis(
                (cellSize + spacing) * minColumns - spacing,
                (cellSize + spacing) * preferredColumns - spacing,
                -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }

        private void SetCellsAlongAxis(int axis)
        {
            int spacing = 0;

            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;
            int cellCountX = 1;
            int cellCountY = 1;

            if (cellSize + spacing <= 0)
                cellCountX = int.MaxValue;
            else
                cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing + 0.001f) / (m_hexagonMetrics.InnerDiameter + spacing)));

            if (cellSize + spacing <= 0)
                cellCountY = int.MaxValue;
            else
                cellCountY = Mathf.Max(1, Mathf.CeilToInt(rectChildren.Count * 1f / cellCountX));
                //cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing + 0.001f) / (m_hexagonMetrics.OuterDiameter + spacing)));

            m_cellCountX = cellCountX;

            int cornerX = m_cornerX = (int)startCorner % 2;
            int cornerY = m_cornerY = (int)startCorner / 2;

            int cellsPerMainAxis, actualCellCountX, actualCellCountY;
            cellsPerMainAxis = cellCountX;
            actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);

            actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildren.Count);
            m_actualCellCountX = actualCellCountX;
            m_actualCellCountY = actualCellCountY;

            Vector2 requiredSpace = new Vector2(
                    actualCellCountX * cellSize + (actualCellCountX - 1) * spacing,
                    actualCellCountY * cellSize + (actualCellCountY - 1) * spacing
                    );
            Vector2 startOffset = new Vector2(
                    GetStartOffset(0, requiredSpace.x),
                    GetStartOffset(1, requiredSpace.y)
                    );

            int positionX;
            int positionY;
            Vector2 position;
            float rectSize = cellSize;

            m_hexagonMetricsWithSapcing.OuterRadius = (m_hexagonMetricsWithSapcing.OuterRadius + spacing);

            for (int i = 0; i < rectChildren.Count; i++)
            {
                positionX = i % cellsPerMainAxis;
                positionY = i / cellsPerMainAxis;


                if (cornerX == 1)
                    positionX = actualCellCountX - 1 - positionX;
                if (cornerY == 1)
                    positionY = actualCellCountY - 1 - positionY;

                position = CalculateHexagonPosition(positionX, positionY);
                m_childHexCoordinates[i].OffsetCoordinates(positionX, positionY);

                SetChildAlongAxis(rectChildren[i], 0, position.x, rectSize);
                SetChildAlongAxis(rectChildren[i], 1, position.y, rectSize);
            }
        }

        private Vector2 CalculateHexagonPosition(int x, int y)
        {
            Vector2 position;

            position.x = (x + y * 0.5f - y/2) * (m_hexagonMetricsWithSapcing.InnerDiameter);
            position.y = y * (m_hexagonMetricsWithSapcing.OuterRadius * 1.5f);
            return position;
        }

    }
}

