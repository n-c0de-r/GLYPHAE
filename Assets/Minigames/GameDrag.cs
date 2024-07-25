using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GlyphaeScripts
{
    public class GameDrag : GameButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;
        [SerializeField] private RectTransform target;

        #endregion


        #region Fields

        private Vector3 _startPosition;

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


        #region Events



        #endregion


        #region Methods



        public void TemplateMethod(bool param)
        {
            
        }

        #endregion


        #region Helpers

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //transform.position = _startPosition;
            if (Vector2.Distance(transform.localPosition, target.localPosition) <= 110)
            {
                Debug.Log("hit");
                Clicked();

            }
            StartCoroutine(ReturnToStartPosition());
        }



        private IEnumerator ReturnToStartPosition()
        {
            //isReturning = true; // Block dragging during return
            float timeToMove = 0.6f;
            float t = 0;

            Vector3 currentPosition = transform.position;

            while (t < timeToMove)
            {
                t += Time.deltaTime * settings.SpeedFactor;
                transform.position = Vector3.Lerp(currentPosition, _startPosition, t / timeToMove);
                yield return null;
            }

            transform.position = _startPosition;
            //isReturning = false; // Allow dragging again
        }

        #endregion
    }
}