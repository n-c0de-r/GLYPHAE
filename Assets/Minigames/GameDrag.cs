using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GlyphaeScripts
{
    public class GameDrag : GameButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;
        [SerializeField][Range(0, 1)] float timeToMove = 0.6f;

        #endregion


        #region Fields

        private Transform target;
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

        public override void SetupDrag(Transform rect)
        {
            target = rect;
        }

        public override void SetSColor(Color color)
        {
            back.color = color;
        }

        #endregion


        #region Helpers

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isReturning) return;
            _startPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isReturning) return;
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //transform.position = _startPosition;
            if (Vector2.Distance(transform.localPosition, target.localPosition) <= 110)
            {
                Clicked();

            }
            StartCoroutine(ReturnToStartPosition());
        }



        private IEnumerator ReturnToStartPosition()
        {
            isReturning = true;
            float timeTotal = 0;

            Vector3 currentPosition = transform.position;

            while (timeTotal < timeToMove)
            {
                timeTotal += Time.deltaTime * settings.SpeedFactor;
                transform.position = Vector3.Lerp(currentPosition, _startPosition, timeTotal / timeToMove);
                yield return null;
            }

            transform.position = _startPosition;
            isReturning = false;
        }

        #endregion
    }
}