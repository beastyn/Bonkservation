namespace UI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Transactions;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using Unity.Collections.LowLevel.Unsafe;

    public enum OverflawType
    {
        None,
        Top,
        Right,
        Bottom,
        Left,
    }

    public class IdolHUDRotator : MonoBehaviour
    {
        [SerializeField] Transform parent;
        [SerializeField] Vector3 insidePositionOffset;
        [SerializeField] Vector3 topOverflawPositionOffset;
        [SerializeField] Vector3 rightOverflawPositionOffset;
        [SerializeField] Vector3 bottomOverflawPositionOffset;
        [SerializeField] Vector3 leftOverflawPositionOffset;
        [SerializeField] Image checkingImage;
        [SerializeField] Image emoteImage;
        [SerializeField] SpriteRenderer roomBorder;

        bool needChangePos = false;
        Vector3 offset;

        void OnEnable()
        {
            this.needChangePos = true;
            this.transform.localScale = Vector3.zero;
            this.offset = Vector3.zero;
            this.CalcOffset();
            this.CalcPosition();
        }

        void LateUpdate()
        {
            this.CalcOffset();
            this.CalcPosition();
        }

        /// <summary>
        ///   Returns corners of a sprite in order [TopRight, TopLeft, BottomLeft, BottomRight]
        /// </summary>
        public static Vector3[] GetSpriteCorners(SpriteRenderer renderer)
        {
            Vector3 topRight = renderer.bounds.max;
            Vector3 topLeft = new Vector3(renderer.bounds.max.x, renderer.bounds.min.y, 0);
            Vector3 botLeft = renderer.bounds.min;
            Vector3 botRight = new Vector3(renderer.bounds.min.x, renderer.bounds.max.y, 0);
            return new Vector3[] { topRight, topLeft, botLeft, botRight };
        }

        public static OverflawType GetOverflowType(Vector3[] container, Vector3[] innerSprite)
        {
            foreach (var innerPoint in innerSprite)
            {
                if (container[0].y < innerPoint.y) return OverflawType.Top;
                if (container[0].x < innerPoint.x) return OverflawType.Right;
                if (container[2].y > innerPoint.y) return OverflawType.Bottom;
                if (container[2].x > innerPoint.x) return OverflawType.Left;
            }

            return OverflawType.None;
        }
        void CalcPosition()
        {
            this.transform.rotation = Quaternion.identity;
            var direction = (this.transform.position - this.parent.position).normalized;
            var angle = Vector3.SignedAngle(direction, Vector3.up + this.offset.normalized, Vector3.forward);
            this.transform.localPosition = Quaternion.AngleAxis(angle, Vector3.forward) * this.transform.localPosition;
            this.checkingImage.transform.localPosition = Quaternion.AngleAxis(angle, Vector3.forward) * this.transform.localPosition;
        }

        void CalcOffset()
        {
            var contentCorners = GetSpriteCorners(this.roomBorder);

            Vector3[] emoteCorners = new Vector3[4];
            this.checkingImage.rectTransform.GetWorldCorners(emoteCorners);
            Vector3[] rotEmoteCorners = { emoteCorners[2], emoteCorners[3], emoteCorners[0], emoteCorners[1] };

            var overflawType = GetOverflowType(contentCorners, rotEmoteCorners);

            if (overflawType == OverflawType.None && this.needChangePos)
                this.offset = this.insidePositionOffset;
            else if (this.needChangePos)
            {
                if (overflawType == OverflawType.Top) this.offset = this.topOverflawPositionOffset;
                if (overflawType == OverflawType.Right) this.offset = this.rightOverflawPositionOffset;
                if (overflawType == OverflawType.Bottom) this.offset = this.bottomOverflawPositionOffset;
                if (overflawType == OverflawType.Left) this.offset = this.leftOverflawPositionOffset;

                this.needChangePos = false;
            }
        }
    }

    public static class RectExtensions
    {
        public static Rect ToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect rect = new Rect(transform.position, size);
            rect.x -= (transform.pivot.x * size.x);
            rect.y -= (transform.pivot.y * size.y);
            return rect;
        }

    }
}