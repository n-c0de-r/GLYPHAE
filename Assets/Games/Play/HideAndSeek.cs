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

        GameBasket _correctBasket;
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
            _buttonCount = 3;

            foreach (GameButton button in _gameInputs) Destroy(button.gameObject);
            _gameInputs = new();

            for (int i = 0; i < _buttonCount; i++)
            {
                Vector3 pos = inputPositions.GetChild(i).position;
                GameButton basket = Instantiate(gameInput, inputContainer);
                basket.GetComponent<RectTransform>().position = pos;
                _gameInputs.Add(basket);
            }
        }


        public override void NextRound()
        {
            ResetBaskets();

            List<GlyphData> temp = new(SelectGlyphs());

            _toMatch = temp[Random.Range(0, temp.Count)];
            _correctBasket = null;

            foreach (GameButton item in _gameInputs)
            {
                GlyphData glyph = temp[Random.Range(0, temp.Count)];
                item.Setup(glyph, glyph.Symbol);
                if (_toMatch == glyph)
                {
                    _correctBasket = (GameBasket)item;
                    _correctBasket.name = _toMatch.name;
                }
                temp.Remove(glyph);
            }

            DisplayRound(_toMatch.Sound, _toMatch.Letter);
            _correctBasket.HideSprite(petSprite.transform);
        }

        #endregion


        #region Helpers

        private void ResetBaskets()
        {
            shuffleNumber = 0;
            grid.enabled = true;

            for (int i = 0; i < _buttonCount; i++)
            {
                Vector3 pos = inputPositions.GetChild(i).position;
                _gameInputs[i].GetComponent<RectTransform>().position = pos;
            }

            grid.enabled = false;
        }

        /// <summary>
        /// Shuffles basket around to another position.
        /// </summary>
        private void ShuffleBaskets()
        {
            List<Transform> _positions = new();
            GameObject temp = new();

            Vector3 outer = _gameInputs[0].transform.position - _gameInputs[1].transform.position;
            outer = _gameInputs[0].transform.position + outer;
            temp.transform.position = outer;
            temp.transform.SetParent(inputContainer);
            _positions.Add(temp.transform);

            temp = new();
            outer = _gameInputs[2].transform.position - _gameInputs[1].transform.position;
            outer = _gameInputs[2].transform.position + outer;
            temp.transform.position = outer;
            temp.transform.SetParent(inputContainer);
            _positions.Add(temp.transform);

            foreach (GameButton item in _gameInputs)
            {
                _positions.Add(item.transform);
            }

            foreach (GameButton item in _gameInputs)
            {
                GameBasket basket = (GameBasket)item;
                Transform target = _positions[Random.Range(0, _positions.Count)];
                _positions.Remove(target);
                basket.MoveTo(target);
            }

            if (shuffleNumber < _rounds)
            {
                Invoke(nameof(ShuffleBaskets), 2/settings.AnimationSpeed);
                shuffleNumber++;
            }
        }

        protected override void CheckInput(GlyphData input)
        {
            if (_correctBasket.name.Equals(input.name))
            {
                _correctGuesses.Add(_toMatch);
                if (_toLearn != null)
                {
                    _toLearn.LevelUp();
                    _correctGuesses.Remove(_toLearn);
                }
                _toLearn = null;
                _isTeaching = false;
                Success();
                foreach (GameButton button in _gameInputs)
                {
                    GameBasket basket = button as GameBasket;
                    if (basket.name.Equals(input.name))
                        basket.RevealContent();
                }
            }
            else
            {
                _toMatch.WronglyGuessed();
                Fail();
                _correctBasket.RevealContent();
            }
        }

        #endregion
    }
}