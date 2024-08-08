using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GlyphaeScripts
{
    /// <summary>
    /// A Button that can be dragged & dropped, extension of <see cref="GameButton"/>
    /// </summary>
    public class GameDrag : GameButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        [Tooltip("The base duration of the move animation.")]
        [SerializeField][Range(0, 1)] float timeToMove = 0.6f;

        #endregion


        #region Fields

        private Color32[] colors = { new(192,32,48,255), new(32, 192, 48, 255), new(32, 48, 192, 255), new(192, 192, 48, 255) };
        private Transform _target;
        private Vector3 _startPosition;
        private bool isReturning = false;
        private int _index = 0;

        #endregion


        #region Events

        public static event Action<bool> OnDragging;

        #endregion


        #region GetSets / Properties

        public Transform Target { set => _target = value; }
        public Color Color { set => back.color = value; }
        
        public Color Red { get => colors[0]; }
        public Color Green { get => colors[1]; }
        public Color Blue { get => colors[2]; }
        public Color Yellow { get => colors[3]; }

        #endregion


        #region Methods

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isReturning || !button.interactable) return;
            OnDragging?.Invoke(true);
            _startPosition = transform.position;
            _index = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isReturning || !button.interactable) return;
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Vector2.Distance(transform.localPosition, _target.localPosition) <= 110)
            {
                Clicked();
            }
            StartCoroutine(ReturnToStartPosition());
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Animation to play, when the button is released.
        /// It returns to its initial position.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReturnToStartPosition()
        {
            isReturning = true;
            float timeTotal = 0;

            Vector3 currentPosition = transform.position;

            while (timeTotal < timeToMove)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                transform.position = Vector3.Lerp(currentPosition, _startPosition, timeTotal / timeToMove);
                yield return null;
            }
            transform.position = _startPosition;
            isReturning = false;
            transform.SetSiblingIndex(_index);
            OnDragging?.Invoke(false);
        }

        #endregion
    }
}