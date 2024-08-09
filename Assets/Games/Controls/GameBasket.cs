using System;
using System.Collections;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameBasket : GameButton
    {
        #region Serialized Fields

        [Header("Input specific.")]
        [Tooltip("The object to animate movement.")]
        [SerializeField] private Transform movablePart;

        #endregion


        #region Fields

        private Vector3 _petStart;
        private Transform _petSprite;

        #endregion


        #region Events

        public static event Action OnHidden;

        #endregion


        #region GetSets / Properties



        #endregion


        #region Methods

        /// <summary>
        /// Hides an object behind this basket.
        /// </summary>
        /// <param name="target">The object to hide.</param>
        public void HideSprite(Transform target)
        {
            StartCoroutine(AnimateHide(target));
        }

        /// <summary>
        /// Moves the basket to a goal position.
        /// Shuffling it around with others.
        /// </summary>
        /// <param name="target">The object to whichs position to move to.</param>
        public void MoveTo(Transform target)
        {
            StartCoroutine(AnimateShuffle(target));
        }

        /// <summary>
        /// Runs the revelation animation of this basket on click.
        /// </summary>
        public void RevealContent()
        {
            StartCoroutine(AnimateReveal());
        }

        #endregion


        #region Helpers

        private IEnumerator AnimateHide(Transform target)
        {
            Vector3 initialPosition = movablePart.position;

            yield return new WaitForSeconds(1/settings.AnimationSpeed);

            float timeTotal = 0;

            Vector3 currentPosition = movablePart.position;

            while (timeTotal < 1)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, target.position + new Vector3(0, 200, 0), timeTotal / 1);
                yield return null;
            }
            yield return new WaitForSeconds(1 / settings.AnimationSpeed);

            _petSprite = target;
            _petStart = target.position;
            Debug.Log(_petStart);

            target.SetParent(transform);
            target.SetAsFirstSibling();

            timeTotal = 0;

            currentPosition = movablePart.position;

            while (timeTotal < 1)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, target.position, timeTotal / 1);
                yield return null;
            }
            
            target.gameObject.SetActive(false);
            yield return new WaitForSeconds(1 / settings.AnimationSpeed);

            timeTotal = 0;

            currentPosition = movablePart.position;

            while (timeTotal < 1)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, initialPosition, timeTotal / 1);
                yield return null;
            }

            OnHidden?.Invoke();
        }

        private IEnumerator AnimateShuffle(Transform target)
        {
            Vector3 initialPosition = movablePart.position;

            yield return new WaitForSeconds(1 / settings.AnimationSpeed);

            float timeTotal = 0;

            Vector3 currentPosition = movablePart.position;

            while (timeTotal < 1)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, target.position, timeTotal / 1);
                yield return null;
            }
            yield return new WaitForSeconds(1 / settings.AnimationSpeed);
        }


        private IEnumerator AnimateReveal()
        {
            Vector3 initialPosition = movablePart.position;

            if (_petSprite != null)
            {
                _petSprite.position = initialPosition;
                _petSprite.gameObject.SetActive(true);
            }
            
            yield return new WaitForSeconds(1 / settings.AnimationSpeed);

            float timeTotal = 0;

            Vector3 currentPosition = movablePart.position;

            while (timeTotal < 1)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, initialPosition + new Vector3(0, 200, 0), timeTotal / 1);
                yield return null;
            }

            if (_petSprite != null)
            {
                _petSprite.SetParent(transform.parent.parent);
                _petSprite.SetAsLastSibling();


                timeTotal = 0;

                if (_petSprite != null) currentPosition = _petSprite.position;
                Vector3 origin = new(0, 0, 0);

                while (timeTotal < 1)
                {
                    timeTotal += Time.deltaTime * settings.AnimationSpeed;
                    _petSprite.localPosition = Vector3.Lerp(_petSprite.localPosition, origin, timeTotal / 1);

                    yield return null;
                }

                yield return new WaitForSeconds(1 / settings.AnimationSpeed);
            }

            timeTotal = 0;

            currentPosition = movablePart.position;

            while (timeTotal < 1)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, initialPosition, timeTotal / 1);
                yield return null;
            }

            yield return new WaitForSeconds(1 / settings.AnimationSpeed);
            Clicked();
        }

        #endregion


        #region Gizmos

        private void OnDrawGizmos()
        {
            
        }

        private void OnDrawGizmosSelected()
        {
             
        }

        #endregion
    }
}