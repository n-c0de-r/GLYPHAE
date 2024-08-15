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
        #region Fields

        private GameButton _clickedButton;
        private GameButton _currentButton;
        private GlyphData _data;
        private int _pairsFound = 0;

        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            GameButton.OnMatch += CheckInput;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            GameButton.OnMatch -= CheckInput;
        }

        #endregion

        #region Methods

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);
            settings.SelectedPet.GetComponent<SpriteRenderer>().enabled = false;

            foreach (GameButton button in _gameInputs) Destroy(button.gameObject);
            _buttonCount = 9;
            SetupButtons(_buttonCount);
            _failsToLose = _buttonCount / 2;
        }


        public override void NextRound()
        {
            _pairsFound = _fails = 0;
            settings.SelectedPet.GetComponent<SpriteRenderer>().enabled = false;
            inputContainer.gameObject.SetActive(true);

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

        protected override void SetupButtons(int count)
        {
            _gameInputs = new();

            for (int i = 0; i < count; i++)
            {
                GameButton button = Instantiate(gameInput, inputContainer);
                _gameInputs.Add(button);
            }
        }

        private void ActivateButtons()
        {
            for (int i = 0; i < _buttonCount; i++)
            {
                _gameInputs[i].GetComponent<Button>().interactable = true;
                _gameInputs[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }

        private void CheckInput(GlyphData input, GameButton button)
        {
            button.GetComponent<Button>().enabled = false;
            button.transform.GetChild(0).GetComponent<Image>().enabled = true;

            if (_clickedButton == null)
            {
                _clickedButton = button;
                return;
            }

            if (_clickedButton.name == input.name)
            {
                _correctGuesses.Add(input);
                if (_toLearn != null)
                {
                    _toLearn.LevelUp();
                    _correctGuesses.Remove(_toLearn);
                }
                _toLearn = null;
                _isTeaching = false;
                _clickedButton = null;
                _pairsFound++;
            }
            else
            {
                foreach (GameButton item in _gameInputs)
                    item.GetComponent<Button>().enabled = false;
                _data = input;
                _currentButton = button;

                Invoke(nameof(Check), 2f / settings.AnimationSpeed);
            }


            if (_pairsFound >= _buttonCount / 2)
            {
                foreach (GameButton item in _gameInputs)
                    item.GetComponent<Button>().enabled = true;

                inputContainer.gameObject.SetActive(false);
                settings.SelectedPet.GetComponent<SpriteRenderer>().enabled = true;
                Success();
                Invoke(nameof(NextRound), 2f / settings.AnimationSpeed);
            }
        }

        private void Check()
        {
            _currentButton.transform.GetChild(0).GetComponent<Image>().enabled = false;
            _clickedButton.transform.GetChild(0).GetComponent<Image>().enabled = false;

            _data.WronglyGuessed();
            if (++_fails >= _failsToLose) Invoke(nameof(CloseGame), 2f / settings.AnimationSpeed);

            foreach (GameButton item in _gameInputs)
                item.GetComponent<Button>().enabled = true;
            _clickedButton = null;
        }

        #endregion
    }
}