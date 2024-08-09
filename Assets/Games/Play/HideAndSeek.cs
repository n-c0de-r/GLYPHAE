using System.Collections;
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

        #endregion


        #region Events



        #endregion


        #region Events



        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            NeedBubble.OnFeedbackDone += NextRound;

        }

        private new void OnDisable()
        {
            base.OnDisable();
            NeedBubble.OnFeedbackDone -= NextRound;
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

            /*
            _usedGlyphs = new();

            if (_isTeaching && !_hasLearned && _newGlyphs.Count > 0)
            {
                // On criticals prefer new glyphs, to teach
                _toMatch = _newGlyphs[Random.Range(0, _newGlyphs.Count)];
                _newGlyphs.Remove(_toMatch);
                _hasLearned = true;
            }
            else if (_allOtherGlyphs.Count > 0)
            {
                // Normally pick known ones
                _toMatch = _allOtherGlyphs[Random.Range(0, _allOtherGlyphs.Count)];
                _allOtherGlyphs.Remove(_toMatch);
            }
            _usedGlyphs.Add(_toMatch);

            int correctPosition = Random.Range(0, _buttonCount);

            for (int i = 0; i < _buttonCount; i++)
            {
                if (i == correctPosition)
                {
                    gameInputs[i].Setup(_toMatch, _toMatch.Symbol);
                }
                else
                {
                    GlyphData wrongGlyph;
                    wrongGlyph = _allOtherGlyphs[Random.Range(0, _allOtherGlyphs.Count)];
                    _allOtherGlyphs.Remove(wrongGlyph);
                    _usedGlyphs.Add(wrongGlyph);
                    gameInputs[i].Setup(wrongGlyph, wrongGlyph.Symbol);
                }
            }

            DisplayRound(_toMatch.Letter);
            */
        }

        #endregion


        #region Helpers

        private void ShuffleBaskets()
        {

        }

        #endregion
    }
}