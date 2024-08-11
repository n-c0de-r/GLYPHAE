using Random = UnityEngine.Random;
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

        private Transform _petSprite;
        private float _delay = 2f;

        #endregion


        #region Events

        public static event Action OnHidden;
        public static event Action<bool> OnBasketPick;

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

            yield return new WaitForSeconds(_delay / settings.AnimationSpeed);

            float timeTotal = 0;

            Vector3 currentPosition = movablePart.position;

            while (timeTotal < _delay)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, target.position + new Vector3(0, 200, 0), timeTotal / _delay);
                yield return null;
            }
            yield return new WaitForSeconds(_delay / settings.AnimationSpeed);

            _petSprite = target;

            target.SetParent(transform);
            target.SetAsFirstSibling();

            timeTotal = 0;

            currentPosition = movablePart.position;

            while (timeTotal < _delay)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, target.position, timeTotal / _delay);
                yield return null;
            }
            
            target.gameObject.SetActive(false);
            yield return new WaitForSeconds(_delay / settings.AnimationSpeed);

            timeTotal = 0;

            currentPosition = movablePart.position;

            while (timeTotal < _delay)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, initialPosition, timeTotal / _delay);
                yield return null;
            }

            OnHidden?.Invoke();
        }

        /// <summary>
        /// Animates the shuffling of this basket's position.
        /// Using code parts from https://gamedev.stackexchange.com/a/157647
        /// </summary>
        /// <param name="target">The position to shuffle to.</param>
        private IEnumerator AnimateShuffle(Transform target)
        {
            icon.enabled = false;
            Vector2 heightPoint1 = movablePart.position - (movablePart.position - target.position)/2 + new Vector3(0, Random.Range(300,600), 0);

            yield return new WaitForSeconds(_delay / settings.AnimationSpeed);

            float timeTotal = 0;

            Vector3 currentPosition = movablePart.position;

            while (timeTotal < _delay)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;

                Vector3 m1 = Vector2.Lerp(currentPosition, heightPoint1, timeTotal / _delay);
                Vector3 m2 = Vector2.Lerp(heightPoint1, target.position, timeTotal / _delay);
                movablePart.position = Vector2.Lerp(m1, m2, timeTotal / _delay);
                yield return null;
            }
            yield return new WaitForSeconds(_delay / settings.AnimationSpeed);

            icon.sprite = data.Letter;
        }

        /// <summary>
        /// Animates the result reveal, lifting the basket.
        /// </summary>
        private IEnumerator AnimateReveal()
        {
            Vector3 initialPosition = movablePart.position;

            if (_petSprite != null)
            {
                _petSprite.position = initialPosition;
                _petSprite.gameObject.SetActive(true);
            }
            
            yield return new WaitForSeconds(_delay / settings.AnimationSpeed);

            float timeTotal = 0;

            Vector3 currentPosition = movablePart.position;

            while (timeTotal < _delay)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, initialPosition + new Vector3(0, 200, 0), timeTotal / _delay);
                yield return null;
            }

            if (_petSprite != null)
            {
                _petSprite.SetParent(transform.parent.parent);
                _petSprite.SetAsLastSibling();


                timeTotal = 0;

                Vector3 origin = new(0, 0, 0);

                while (timeTotal < _delay)
                {
                    timeTotal += Time.deltaTime * settings.AnimationSpeed;
                    _petSprite.localPosition = Vector3.Lerp(_petSprite.localPosition, origin, timeTotal / _delay);

                    yield return null;
                }

                yield return new WaitForSeconds(1 / settings.AnimationSpeed);
            }
            OnBasketPick?.Invoke(_petSprite != null);

            timeTotal = 0;

            currentPosition = movablePart.position;

            while (timeTotal < _delay)
            {
                timeTotal += Time.deltaTime * settings.AnimationSpeed;
                movablePart.position = Vector3.Lerp(currentPosition, initialPosition, timeTotal / _delay);
                yield return null;
            }

            yield return new WaitForSeconds(_delay / settings.AnimationSpeed);
        }

        #endregion
    }
}