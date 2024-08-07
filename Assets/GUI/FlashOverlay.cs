using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class FlashOverlay : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("Overlay image to simulate a flash.")]
        [SerializeField] private Image overlay;

        #endregion


        #region Helpers

        public IEnumerator Flash(float start, float end, float speedFactor)
        {
            yield return new WaitForSeconds(1f / speedFactor);
            Color color;

            for (float i = start; i <= end; i += Time.deltaTime * speedFactor)
            {
                color = overlay.color;
                color.a = i;
                overlay.color = color;
                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator Flash(Color color,float start, float end, float speedFactor)
        {
            yield return new WaitForSeconds(1f / speedFactor);
            overlay.color = color;

            for (float i = start; i <= end; i += Time.deltaTime * speedFactor)
            {
                color.a = i;
                overlay.color = color;
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}