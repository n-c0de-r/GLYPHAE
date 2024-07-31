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
        
        [SerializeField] private Transform target;
        private Vector3 _startPosition;
        private bool isReturning = false;

        #endregion


        #region Events



        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {

        }

        void Start()
        {

        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            
        }

        #endregion


        #region Methods

        public void SetSColor(Color color)
        {
            back.color = color;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isReturning || !button.interactable) return;
            _startPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isReturning || !button.interactable) return;
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Vector2.Distance(transform.localPosition, target.localPosition) <= 110)
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
        }

        #endregion
    }
}