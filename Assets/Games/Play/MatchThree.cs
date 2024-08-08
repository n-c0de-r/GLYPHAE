using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    /// <summary>
    /// Played in the pre-stage. To break the egg's shell.
    /// </summary>
    public class MatchThree : Minigame
    {
        #region Serialized Fields

        [Space]
        [Header("Game Specific")]
        [Tooltip("The sprite for listening to buttons.")]
        [SerializeField] private Sprite soundSprite;

        #endregion

        #region Fields

        private GameButton _clickedButton;
        private GameButton _currentButton;
        private GlyphData _data;

        private const int NR_OF_TRIPPLETS = 4;

        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            GameDrag.OnDragging += ToggleGrid;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            GameDrag.OnDragging -= ToggleGrid;
        }

        #endregion

        #region Methods

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);

            foreach (GameButton button in _gameInputs) Destroy(button.gameObject);

            _buttonCount = NR_OF_TRIPPLETS * _rounds * 3;
            _failsToLose = 2;

            SetupButtons(_buttonCount);
        }


        public override void NextRound()
        {
            if (_gameInputs.Count <= 0) return;

            List<int> positions = new();
            List<GlyphData> temp = new(SelectGlyphs());

            for (int i = 0; i < NR_OF_TRIPPLETS; i++)
            {
                GlyphData glyph = temp[Random.Range(0, temp.Count)];
                int index;
                temp.Remove(glyph);


                do
                {
                    index = Random.Range(0, _buttonCount);
                } while (positions.Contains(index));

                positions.Add(index);
                _gameInputs[index].Setup(glyph, soundSprite);
                _gameInputs[index].name = glyph.name;
                Destroy(_gameInputs[index].GetComponent<GameDrag>());
                Transform target = _gameInputs[index].transform;


                do
                {
                    index = Random.Range(0, _buttonCount);
                } while (positions.Contains(index));

                positions.Add(index);
                _gameInputs[index].Setup(glyph, glyph.Symbol);
                _gameInputs[index].name = glyph.name;
                GameDrag drag = (GameDrag)_gameInputs[index];
                drag.Target = target;


                do
                {
                    index = Random.Range(0, _buttonCount);
                } while (positions.Contains(index));

                positions.Add(index);
                _gameInputs[index].Setup(glyph, glyph.Letter);
                _gameInputs[index].name = glyph.name;
                drag = (GameDrag)_gameInputs[index];
                drag.Target = target;
            }

            ActivateButtons(true);
        }

        #endregion


        #region Helpers

        private void ToggleGrid(bool state)
        {
            inputContainer.GetComponent<GridLayoutGroup>().enabled = !state;
        }

        //private void CheckInput(GlyphData input, GameButton button)
        //{
        //    button.GetComponent<Button>().interactable = false;

        //    if (_clickedButton == null)
        //    {
        //        _clickedButton = button;
        //        return;
        //    }

        //    if (_clickedButton.name == input.name)
        //    {
        //        _toLearn = null;
        //        _isTeaching = false;
        //        _gameInputs.Remove(button);
        //        _gameInputs.Remove(_clickedButton);
        //        _clickedButton = null;
        //        input.CorrectlyGuessed();
        //    }
        //    else
        //    {
        //        foreach (GameButton item in _gameInputs)
        //            item.GetComponent<Button>().interactable = false;
        //        _data = input;
        //        _currentButton = button;

        //        Invoke(nameof(Check), 1f);
        //    }


        //    if (_gameInputs.Count <= 0)
        //    {
        //        foreach (GameButton item in _gameInputs)
        //            item.GetComponent<Button>().interactable = true;
        //        Invoke(nameof(Success), 1f);
        //    }
        //}

        //private void Check()
        //{
        //    foreach (GameButton item in _gameInputs)
        //        item.GetComponent<Button>().interactable = true;

        //    _currentButton.GetComponent<Button>().interactable = true;
        //    _clickedButton.GetComponent<Button>().interactable = true;
        //    //_clickedButton.transform.GetChild(0).GetComponent<Image>().enabled = false;
        //    _data.WronglyGuessed();
        //    if (++_fails >= _failsToLose) CloseGame();
        //    _clickedButton = null;
        //}

        #endregion
    }
}