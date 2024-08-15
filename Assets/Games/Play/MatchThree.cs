using System.Collections.Generic;
using System.Linq;
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

        [Tooltip("The grid holding the buttons to match.")]
        [SerializeField] private GridLayoutGroup grid;

        #endregion


        #region Fields

        private List<List<GameDrag>> _triplets;
        private GameDrag _first, _second;
        private Color _firstColor, _secondColor;

        private const int NR_OF_TRIPPLETS = 4;

        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            GameDrag.OnDropped += CheckDrop;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            GameDrag.OnDropped -= CheckDrop;
        }

        #endregion


        #region Methods

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);
            settings.SelectedPet.GetComponent<SpriteRenderer>().enabled = false;

            foreach (GameButton button in _gameInputs) Destroy(button.gameObject);

            _buttonCount = NR_OF_TRIPPLETS * 3;
            _failsToLose = 2;

            SetupButtons(_buttonCount);
        }


        public override void NextRound()
        {
            if (_gameInputs.Count <= 0) return;
            _triplets = new();

            List<int> positions = new();
            List<GlyphData> temp = new(SelectGlyphs());

            for (int i = 0; i < NR_OF_TRIPPLETS; i++)
            {
                List<GameDrag> triplet = new();

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
                GameDrag drag = (GameDrag)_gameInputs[index];
                drag.DragColor = i+1;
                triplet.Add(drag);

                do
                {
                    index = Random.Range(0, _buttonCount);
                } while (positions.Contains(index));

                positions.Add(index);
                _gameInputs[index].Setup(glyph, glyph.Symbol);
                _gameInputs[index].name = glyph.name;
                drag = (GameDrag)_gameInputs[index];
                drag.DragColor = i+1;
                triplet.Add(drag);


                do
                {
                    index = Random.Range(0, _buttonCount);
                } while (positions.Contains(index));

                positions.Add(index);
                _gameInputs[index].Setup(glyph, glyph.Letter);
                _gameInputs[index].name = glyph.name;
                drag = (GameDrag)_gameInputs[index];
                drag.DragColor = i+1;
                triplet.Add(drag);

                _triplets.Add(triplet);
            }

            SetDrags();

            ActivateButtons(true);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Sets up the targets to each drag button.
        /// Essentially making it a 2D construction.
        /// </summary>
        private void SetDrags()
        {
            foreach (GameDrag drag in _gameInputs.Cast<GameDrag>())
            {
                foreach (GameButton item in _gameInputs)
                {
                    if (drag.gameObject == item.gameObject) continue;
                    drag.Target = item.transform;
                }
            }
        }

        /// <summary>
        /// Checks the currently dropped button against the one dropped at.
        /// </summary>
        /// <param name="original">The button you moved now.</param>
        /// <param name="target">The button you moved to.</param>
        private void CheckDrop(GameDrag original, GameDrag target)
        {
            if (original.name == target.name)
            {
                original.Mark = true;
                original.GetComponent<Button>().enabled = false;
                original.enabled = false;
                foreach (GameDrag item in _gameInputs.Cast<GameDrag>())
                    item.RemoveTargets(original.transform);

                target.Mark = true;
                target.GetComponent<Button>().enabled = false;
                target.enabled = false;
                foreach (GameDrag item in _gameInputs.Cast<GameDrag>())
                    item.RemoveTargets(target.transform);

                List<GameDrag> toRemove = null;

                foreach (List<GameDrag> items in _triplets)
                {
                    items.Remove(original);
                    if (items.Count == 0)
                    {
                        toRemove = items;
                        break;
                    }

                    items.Remove(target);
                    if (items.Count == 0)
                    {
                        toRemove = items;
                        break;
                    }
                }

                if (toRemove != null) _triplets.Remove(toRemove);
                if (_triplets.Count == 0)
                {
                    _correctGuesses.Add(original.Data);
                    if (_toLearn != null)
                    {
                        _toLearn.LevelUp();
                        _correctGuesses.Remove(_toLearn);
                    }
                    _toLearn = null;
                    _isTeaching = false;
                    Success();
                }
            }
            else
            {
                foreach (var item in _gameInputs)
                {
                    if (item.TryGetComponent(out GameDrag drag ))
                        drag.enabled = false;
                }
                _first = original;
                _second = target;
                _firstColor = original.SelectedColor;
                _secondColor = target.SelectedColor;
                original.GetComponent<Image>().color = Color.red;
                target.GetComponent<Image>().color = Color.red;
                Invoke(nameof(Reset), 1f/settings.AnimationSpeed);
            }
        }

        private void Reset()
        {
            if (++_fails >= _failsToLose) CloseGame();
            _first.GetComponent<Image>().color = _first.GetComponent<Button>().enabled ? Color.white : _firstColor;
            _second.GetComponent<Image>().color = _second.GetComponent<Button>().enabled ? Color.white : _secondColor;
            foreach (var item in _gameInputs)
            {
                if (item.TryGetComponent(out GameDrag drag))
                    drag.enabled = true;
            }
        }

        #endregion
    }
}