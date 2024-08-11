using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    /// <summary>
    /// A Button that can be dragged & dropped, extension of <see cref="GameButton"/>
    /// </summary>
    public class GameDrag : GameButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Serialized Fields

        [Header("Input specific.")]
        [Tooltip("The base duration of the move animation.")]
        [SerializeField][Range(0, 1)] float timeToMove = 0.6f;

        #endregion


        #region Fields

        private Color32[] _colors = {
            new(224, 64, 80, 255), new(64, 224, 80, 255), new(64, 80, 224, 255), new(224, 224, 80, 255), new(192, 64, 224, 255)
        };
        private HashSet<Transform> _targets = new();
        private Vector3 _startPosition;
        private int _index = 0, _checkDistance = 110;
        private Color _selectedColor;
        private bool isReturning = false;

        #endregion


        #region Events

        public static event Action<bool> OnDragging;
        public static event Action<GameDrag, GameDrag> OnDropped;

        #endregion


        #region GetSets / Properties

        public Transform Target { set => _targets.Add(value); }
        public HashSet<Transform> Targets { get => _targets; }
        
        public Color32[] Colors { get => _colors; set => _colors = value; }

        public int DragColor { set => _selectedColor = _colors[value]; }

        public Color SelectedColor { get => _selectedColor; }

        public bool Mark { set { if (value) back.color = _selectedColor; } }

        public int CheckDistance { set => _checkDistance = value; }

        #endregion


        #region Methods

        public void RemoveTargets(Transform target)
        {
            foreach (Transform item in _targets)
            {
                if (item == target) continue;

                if( target == item)
                {
                    _targets.Remove(item);
                    return;
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isReturning || !button.interactable) return;
            transform.parent.gameObject.GetComponent<LayoutGroup>().enabled = false;
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
            foreach (Transform item in _targets)
            {
                if (Vector2.Distance(transform.localPosition, item.localPosition) <= _checkDistance)
                {
                    Clicked();
                    OnDropped?.Invoke(this, item.GetComponent<GameDrag>());
                }
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
            transform.parent.gameObject.GetComponent<LayoutGroup>().enabled = true;
        }

        #endregion
    }
}