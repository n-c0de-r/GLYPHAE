using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    /// <summary>
    /// Played in the pre-stage. To break the egg's shell.
    /// </summary>
    public class OddOneOut : Minigame
    {
        #region Serialized Fields

        [Space]
        [Header("Game Specific")]
        [Tooltip("The container where the items to match will spawn.")]
        [SerializeField] private GridLayoutGroup container;

        #endregion


        #region Fields

        private GameButton _clickedButton;
        private int _foundPairs = 0;

        #endregion

        #region Methods

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);

            foreach (GameButton button in _gameInputs) Destroy(button.gameObject);
            
            _buttonCount = 9;
            SetupButtons(_buttonCount);

            NextRound();
        }

        
        public override void NextRound()
        {
            List<int> positions = new();
            List<GlyphData> temp = new(SelectGlyphs());

            GlyphData glyph = temp[Random.Range(0, temp.Count)];
            while (glyph == _toLearn)
            {
                glyph = temp[Random.Range(0, temp.Count)];
            }
            temp.Remove(glyph);

            // Wrong one
            int index = Random.Range(0, _buttonCount);
            positions.Add(index);
            Sprite sprite = Random.Range(0, 2) == 0 ? glyph.Symbol : glyph.Letter;
            _gameInputs[index].Setup(glyph, sprite);
            _gameInputs[index].name = glyph.name;

            for (int i = 0; i < _buttonCount/2; i++)
            {
                // Correct ones
                glyph = temp[Random.Range(0, temp.Count)];

                //if (_toLearn != null && _toLearn == glyph) glyph = _toLearn;
                //else if (_toMatch == null) _toMatch = glyph;
                temp.Remove(glyph);

                do
                {
                    index = Random.Range(0, _buttonCount);
                } while (positions.Contains(index));

                positions.Add(index);
                _gameInputs[index].Setup(glyph, glyph.Symbol);
                _gameInputs[index].name = glyph.name;

                do
                {
                    index = Random.Range(0, _buttonCount);
                } while (positions.Contains(index));

                positions.Add(index);
                _gameInputs[index].Setup(glyph, glyph.Letter);
                _gameInputs[index].name = glyph.name;
            }

            ActivateButtons();
        }

        #endregion


        #region Helpers

        private void ActivateButtons()
        {
            for (int i = 0; i < _buttonCount; i++)
            {
                _gameInputs[i].GetComponent<Button>().interactable = true;
                _gameInputs[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }

        protected override void CheckInput(GlyphData input)
        {
            GameButton current = _gameInputs.Find(button => button.name == input.name);
            current.GetComponent<Button>().interactable = false;
            current.transform.GetChild(0).GetComponent<Image>().enabled = true;

            if (_toMatch == null)
            {
                _toMatch = input;
                _clickedButton = current;
            } else
            {
                if (_toMatch == input)
                {
                    _toLearn = null;
                    _isTeaching = false;
                    _toMatch.CorrectlyGuessed();
                    _foundPairs++;
                }
                else
                {
                    _toMatch.WronglyGuessed();
                    current.GetComponent<Button>().interactable = true;
                    current.transform.GetChild(0).GetComponent<Image>().enabled = false;
                    _clickedButton.GetComponent<Button>().interactable = true;
                    _clickedButton.transform.GetChild(0).GetComponent<Image>().enabled = false;
                }
                _clickedButton = null;
                _toMatch = null;

                if (_foundPairs >= 3) Success();
            }
        }

        #endregion
    }
}