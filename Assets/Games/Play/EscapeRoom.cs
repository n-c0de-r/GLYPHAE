using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;


namespace GlyphaeScripts
{
    /// <summary>
    /// Played in the pre-stage. To break the egg's shell.
    /// </summary>
    public class EscapeRoom : Minigame
    {
        #region Serialized Fields

        [Space]
        [Header("Game Specific")]
        [Tooltip("The system responsible fo generating maze data.")]
        [SerializeField] private MazeGenerator mazeGenerator;

        [Tooltip("The container where the maze will be generated in.")]
        //[SerializeField] private GameObject mazeContainer;

        [SerializeField] private Sprite[] wallSprites;

        [SerializeField] private Transform gridFloors;

        [SerializeField] private Transform gridWalls;

        [SerializeField] private Transform gridPositions;

        [Tooltip("The container where the list of items to match will spawn.")]
        [SerializeField] private Image petSprite;


        #endregion


        #region Fields

        private List<GameButton> _floorButtons;
        private List<int> _exitPositions;
        private GameButton _clickedButton, _currentButton;
        private GlyphData _data;
        int[,] _mazeData;
        private const int SIZE = 5, NORTH = 1, EAST = 2, SOUTH = 4, WEST = 8;
        private int _currentX, _currentY, _currentData, _currentDirection;

        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            GameButton.OnMatch += CheckInput;
            //NeedBubble.OnFeedbackDone += NextRound;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            GameButton.OnMatch -= CheckInput;
            //NeedBubble.OnFeedbackDone -= NextRound;
        }

        #endregion

        #region Methods

        public override void NextRound()
        {
            SelectGlyphs();

            for (int i = 0; i < _buttonCount; i++)
            {
                _gameInputs[i].Setup(_usedGlyphs[i], _usedGlyphs[i].Letter);
                _gameInputs[i].name = _usedGlyphs[i].name;
            }

            _mazeData = mazeGenerator.GenerateMaze();
            ResetMaze();
        }

        /// <summary>
        /// To delete. Only debug.
        /// </summary>
        public void GenerateMaze()
        {
            _mazeData = mazeGenerator.GenerateMaze();
            ResetMaze();
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Show the generated maze.
        /// </summary>
        private void ResetMaze()
        {
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    gridWalls.GetChild(x + SIZE * y).GetComponent<Image>().sprite = wallSprites[_mazeData[x, y]];
                    gridWalls.GetChild(x + SIZE * y).name = "" + _mazeData[x, y];
                }
            }

            SetExits();
            SetStartPosition();
            ShowNextIcons();
        }

        /// <summary>
        /// Sets up an exit in the middle of two randomly selected sides.
        /// </summary>
        private void SetExits()
        {
            List<int> directions = new() { 0, 1, 2, 3 };
            _exitPositions = new();

            int mid = SIZE / 2, low = NORTH / SOUTH, high = SOUTH / NORTH;

            gridWalls.GetChild(mid + SIZE * low).GetChild(0).GetComponent<Image>().enabled = false;
            gridWalls.GetChild(high + SIZE * mid).GetChild(0).GetComponent<Image>().enabled = false;
            gridWalls.GetChild(mid + SIZE * high).GetChild(0).GetComponent<Image>().enabled = false;
            gridWalls.GetChild(low + SIZE * mid).GetChild(0).GetComponent<Image>().enabled = false;

            while (directions.Count > 2)
            {
                int rng = Random.Range(0, directions.Count);

                switch (directions[rng])
                {
                    case 0:
                        gridWalls.GetChild(mid + SIZE * low).GetComponent<Image>().sprite = wallSprites[_mazeData[mid, low] + NORTH];
                        gridWalls.GetChild(mid + SIZE * low).name = "" + (_mazeData[mid, low] + NORTH);
                        gridWalls.GetChild(mid + SIZE * low).GetChild(0).GetComponent<Image>().enabled = true;
                        _exitPositions.Add(mid + SIZE * low);
                        break;
                    case 1:
                        gridWalls.GetChild(high + SIZE * mid).GetComponent<Image>().sprite = wallSprites[_mazeData[high, mid] + EAST];
                        gridWalls.GetChild(high + SIZE * mid).name = "" + (_mazeData[high, mid] + EAST);
                        gridWalls.GetChild(high + SIZE * mid).GetChild(0).GetComponent<Image>().enabled = true;
                        _exitPositions.Add(high + SIZE * mid);
                        break;
                    case 2:
                        gridWalls.GetChild(mid + SIZE * high).GetComponent<Image>().sprite = wallSprites[_mazeData[mid, high] + SOUTH];
                        gridWalls.GetChild(mid + SIZE * high).name = "" + (_mazeData[mid, high] + SOUTH);
                        gridWalls.GetChild(mid + SIZE * high).GetChild(0).GetComponent<Image>().enabled = true;
                        _exitPositions.Add(mid + SIZE * high);
                        break;
                    case 3:
                        gridWalls.GetChild(low + SIZE * mid).GetComponent<Image>().sprite = wallSprites[_mazeData[low, mid] + WEST];
                        gridWalls.GetChild(low + SIZE * mid).name = "" + (_mazeData[low, mid] + WEST);
                        gridWalls.GetChild(low + SIZE * mid).GetChild(0).GetComponent<Image>().enabled = true;
                        _exitPositions.Add(low + SIZE * mid);
                        break;
                }

                directions.Remove(directions[rng]);
            }
        }

        /// <summary>
        /// Set the <see cref="Pet"/>'s sprite in the middle of the maze.
        /// </summary>
        private void SetStartPosition()
        {
            petSprite.sprite = settings.SelectedPet.GetComponent<SpriteRenderer>().sprite;
            petSprite.transform.SetParent(gridPositions.GetChild(2 + SIZE * 2));
            petSprite.transform.localPosition = Vector3.zero;
            _currentX = 2;
            _currentY = 2;
            _currentData = _mazeData[_currentX, _currentY];
        }

        /// <summary>
        /// Displays the 4 icons adjecent to the <see cref="Pet"/>'s sprite.
        /// </summary>
        private void ShowNextIcons()
        {
            foreach (GameButton item in _gameInputs)
                item.GetComponent<Button>().interactable = false;

            _floorButtons = new();

            // Set Exits
            for (int x = -1; x <= 1; x+=2)
            {
                for (int y = -1; y <= 1; y+=2)
                {
                    if (_currentX + x < 0 || _currentX + x >= SIZE || _currentY + y < 0 || _currentY + y >= SIZE) continue;

                    GlyphData glyph = _usedGlyphs[Random.Range(0, _usedGlyphs.Count)];
                    GameButton tile = gridFloors.GetChild(_currentX + x + SIZE * _currentY).GetComponent<GameButton>();
                    tile.Setup(glyph, glyph.Symbol);
                    tile.name = glyph.name;
                    tile.GetComponent<Button>().interactable = true;
                    tile.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    _floorButtons.Add(tile);
                    gridPositions.GetChild(_currentX + x + SIZE * _currentY).name = (x == 1) ? "" + EAST : "" + WEST;


                    glyph = _usedGlyphs[Random.Range(0, _usedGlyphs.Count)];
                    tile = gridFloors.GetChild(_currentX + SIZE * (_currentY + y)).GetComponent<GameButton>();
                    tile.Setup(glyph, glyph.Symbol);
                    tile.name = glyph.name;
                    tile.GetComponent<Button>().interactable = true;
                    tile.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    _floorButtons.Add(tile);
                    gridPositions.GetChild(_currentX + SIZE * (_currentY + y)).name = (y == 1) ? "" + SOUTH : "" + NORTH;
                }
            }
        }

        /// <summary>
        /// Erases the 4 icons adjecent to the <see cref="Pet"/>'s sprite.
        /// </summary>
        private void HidePreviousIcons()
        {
            foreach (GameButton item in _floorButtons)
            {
                item.GetComponent<Button>().interactable = false;
                item.transform.GetChild(0).GetComponent<Image>().enabled = false;
                int index = item.transform.GetSiblingIndex();
                gridPositions.GetChild(index).name = "0";
            }
        }

        private void CheckInput(GlyphData input, GameButton button)
        {
            if (_clickedButton == null)
            {
                _clickedButton = button;
                ActivateButtons(true);
                HidePreviousIcons();
                return;
            }

            ActivateButtons(false);

            if (_clickedButton.name == button.name)
            {
                GameButton item = button.transform.parent == gridFloors ? button : _clickedButton;
                int index = item.transform.GetSiblingIndex();
                
                _currentButton = button;
                input.CorrectlyGuessed();

                MoveSprite(index);
            }
            else
            {
                _data = input;
                _currentButton = button;

                Invoke(nameof(Check), 1f / settings.AnimationSpeed);
            }
        }

        private void Check()
        {
            _data.WronglyGuessed();
            if (++_fails >= _failsToLose) CloseGame();
            _clickedButton = null;
        }

        /// <summary>
        /// Moves the <see cref="Pet"/> sprite to the goal tile if possible.
        /// </summary>
        /// <param name="goalPosition">The index of the tile in the hierarchy.</param>
        private void MoveSprite(int goalPosition)
        {
            int.TryParse(gridPositions.GetChild(goalPosition).name, out int direction);

            if (_currentData / direction != 0)
            {
                petSprite.transform.SetParent(gridPositions.GetChild(goalPosition));
                petSprite.transform.localPosition = Vector3.zero;
                _currentX = goalPosition % SIZE;
                _currentY = goalPosition / SIZE;
                _currentData = _mazeData[_currentX, _currentY];
                
                if (_exitPositions.Contains(goalPosition))
                {
                    Invoke(nameof(Success), 1f / settings.AnimationSpeed);
                    return;
                }

                _toLearn = null;
                _isTeaching = false;
                _clickedButton = null;

                ShowNextIcons();
            }
        }

        /// <summary>
        /// Toggles buttons or florr tiles on/off.
        /// </summary>
        /// <param name="button">Either buttons or tiles.</param>
        /// <param name="state">The state to switch to.</param>
        private void ToogleButtons(GameButton button, bool state)
        {
            if (button.transform.parent == inputContainer)
            {
                foreach (GameButton item in _gameInputs)
                    item.GetComponent<Button>().interactable = state;
            }
            else
            {
                foreach (GameButton item in _floorButtons)
                    item.GetComponent<Button>().interactable = state;
            }
        }

        #endregion
    }
}