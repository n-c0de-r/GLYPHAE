using UnityEngine.InputSystem;

namespace GlyphaeScripts
{
    public interface IClickable
    {
        #region Methods

        void OnClick(InputAction.CallbackContext context);

        #endregion
    }
}