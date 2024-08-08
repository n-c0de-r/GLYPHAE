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

            foreach (GameButton button in _gameInputs) Destroy(button.gameObject);
            
            _buttonCount = 9;
            SetupButtons(_buttonCount);
            _failsToLose = 3;

            NextRound();
        }

        
        public override void NextRound()
        {
            if (_gameInputs.Count <= 1) return;

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
            button.GetComponent<Button>().interactable = false;
            button.transform.GetChild(0).GetComponent<Image>().enabled = true;

            if (_clickedButton == null)
            {
                _clickedButton = button;
                return;
            }

            if (_clickedButton.name == input.name)
            {
                _toLearn = null;
                _isTeaching = false;
                _gameInputs.Remove(button);
                _gameInputs.Remove(_clickedButton);
                _clickedButton = null;
                input.CorrectlyGuessed();
            }
            else
            {
                foreach (GameButton item in _gameInputs)
                    item.GetComponent<Button>().interactable = false;
                _data = input;
                _currentButton = button;

                Invoke(nameof(Check), 1f);
            }


            if (_gameInputs.Count <= 1)
            {
                foreach (GameButton item in _gameInputs)
                    item.GetComponent<Button>().interactable = true;
                Invoke(nameof(Success), 1f);
            }
        }

        private void Check()
        {
            foreach (GameButton item in _gameInputs)
                item.GetComponent<Button>().interactable = true;

            _currentButton.GetComponent<Button>().interactable = true;
            _currentButton.transform.GetChild(0).GetComponent<Image>().enabled = false;
            _clickedButton.GetComponent<Button>().interactable = true;
            _clickedButton.transform.GetChild(0).GetComponent<Image>().enabled = false;
            _data.WronglyGuessed();
            if (++_fails >= _failsToLose) CloseGame();
            _clickedButton = null;
        }

        #endregion
    }
}