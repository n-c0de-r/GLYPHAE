using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GlyphaeScripts
{
    /// <summary>
    /// Played in the pre-stage. To break the egg's shell.
    /// </summary>
    public class HideAndSeek : Minigame
    {
        #region Serialized Fields

        [Space]
        [Header("Game Specific")]
        [Tooltip("The container where the list of items to match will spawn.")]
        [SerializeField] private Image petSprite;

        [Tooltip("The grid holding the baskets to check.")]
        [SerializeField] private HorizontalLayoutGroup grid;

        #endregion


        #region Fields

        private List<Sprite> previousSprites;
        private Sprite previous;
        private int shuffleNumber;

        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            GameBasket.OnHidden += ShuffleBaskets;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            GameBasket.OnHidden -= ShuffleBaskets;
        }

        #endregion

        #region Methods

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);

            petSprite.sprite = settings.SelectedPet.gameObject.GetComponent<SpriteRenderer>().sprite;
            settings.SelectedPet.GetComponent<SpriteRenderer>().enabled = false;

            Vector3 pos = inputPositions.GetChild(inputPositions.childCount-1).position;
            GameButton button = Instantiate(gameInput, inputContainer);
            button.GetComponent<RectTransform>().position = pos;
            _gameInputs.Add(button);
            grid.enabled = false;

            _buttonCount = 3;
        }

        
        public override void NextRound()
        {
            shuffleNumber = 0;
            SelectGlyphs();
            List<GlyphData> temp = new(_usedGlyphs);

            int rng = Random.Range(0, _usedGlyphs.Count);

            foreach (GameButton item in _gameInputs)
            {
                GlyphData glyph = temp[Random.Range(0, temp.Count)];
                item.Setup(glyph, glyph.Symbol);
                _toMatch = item.transform.GetSiblingIndex() == rng ? glyph : _toMatch;
                temp.Remove(glyph);
            }

            GameBasket basket = (GameBasket)_gameInputs[rng];
            basket.HideSprite(petSprite.transform);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Shuffles basket around to another position.
        /// </summary>
        private void ShuffleBaskets()
        {
            List<Transform> positions = new();
            GameObject temp = new();

            Vector3 outer = _gameInputs[0].transform.position - _gameInputs[1].transform.position;
            outer = _gameInputs[0].transform.position + outer;
            temp.transform.position = outer;
            positions.Add(temp.transform);

            temp = new();
            outer = _gameInputs[2].transform.position - _gameInputs[1].transform.position;
            outer = _gameInputs[2].transform.position + outer;
            temp.transform.position = outer;
            positions.Add(temp.transform);

            foreach (GameButton item in _gameInputs)
            {
                positions.Add(item.transform);
            }

            foreach (GameButton item in _gameInputs)
            {
                GameBasket basket = (GameBasket)item;
                Transform target = positions[Random.Range(0, positions.Count)];
                positions.Remove(target);
                basket.MoveTo(target);
            }

            if (shuffleNumber < 2)
            {
                Invoke(nameof(ShuffleBaskets), 2/settings.AnimationSpeed);
                shuffleNumber++;
            }
        }

        #endregion
    }
}