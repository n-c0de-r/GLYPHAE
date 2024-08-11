
using System.Collections.Generic;
using UnityEngine;
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

        #endregion


        #region Fields

        private GameButton _clickedButton;
        private GameButton _currentButton;
        private GlyphData _data;

        private List<Sprite> previousSprites;
        private Sprite _previousSprite;
        int[,] _mazeData;
        private const int SIZE = 5;
        private const int NORTH = 1, EAST = 2, SOUTH = 4, WEST = 8;
        private int _currentX, _currentY;
        private int _currentData;

        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            //NeedBubble.OnFeedbackDone += NextRound;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            //NeedBubble.OnFeedbackDone -= NextRound;
        }

        #endregion

        #region Methods

        public override void NextRound()
        {
            SelectGlyphs();

            _mazeData = mazeGenerator.GenerateMaze();
            ResetMaze();
        }

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
                }
            }

            SetExits();
            SetStartPosition();
            ShowNextIcons();
        }

        private void SetExits()
        {
            List<int> directions = new() { 0, 1, 2, 3 };

            int mid = SIZE / 2, low = NORTH / SOUTH, high = SOUTH / NORTH;

            while (directions.Count > 2)
            {
                int rng = Random.Range(0, directions.Count);

                switch (directions[rng])
                {
                    case 0:
                        gridWalls.GetChild(mid + SIZE * low).GetComponent<Image>().sprite = wallSprites[_mazeData[mid, low] + NORTH];
                        break;
                    case 1:
                        gridWalls.GetChild(high + SIZE * mid).GetComponent<Image>().sprite = wallSprites[_mazeData[high, mid] + EAST];
                        break;
                    case 2:
                        gridWalls.GetChild(mid + SIZE * high).GetComponent<Image>().sprite = wallSprites[_mazeData[mid, high] + SOUTH];
                        break;
                    case 3:
                        gridWalls.GetChild(low + SIZE * mid).GetComponent<Image>().sprite = wallSprites[_mazeData[low, mid] + WEST];
                        break;
                }

                directions.Remove(directions[rng]);
            }
        }

        private void SetStartPosition()
        {
            _previousSprite = gridPositions.GetChild(2 + SIZE * 2).GetComponent<Image>().sprite;
            gridPositions.GetChild(2 + SIZE * 2).GetComponent<Image>().sprite = settings.SelectedPet.GetComponent<SpriteRenderer>().sprite;
            _currentX = 2;
            _currentY = 2;
            _currentData = _mazeData[2, 2];
        }

        private void MoveSprite()
        {

        }

        private void ShowNextIcons()
        {
            // Set Exits
            for (int x = -1; x <= 1; x+=2)
            {
                for (int y = -1; y <= 1; y+=2)
                {
                    if (_currentX + x < 0 || _currentX + x >= SIZE || _currentY + y < 0 || _currentY + y >= SIZE) continue;

                    GlyphData glyph = _usedGlyphs[Random.Range(0, _usedGlyphs.Count)];
                    gridFloors.GetChild(_currentX + x + SIZE * _currentY).GetComponent<GameButton>().Setup(glyph, glyph.Symbol);
                    gridFloors.GetChild(_currentX + x + SIZE * _currentY).GetComponent<Button>().enabled = true;
                    gridFloors.GetChild(_currentX + x + SIZE * _currentY).GetChild(0).GetComponent<Image>().enabled = true;

                    glyph = _usedGlyphs[Random.Range(0, _usedGlyphs.Count)];
                    gridFloors.GetChild(_currentX + SIZE * (_currentY + y)).GetComponent<GameButton>().Setup(glyph, glyph.Symbol);
                    gridFloors.GetChild(_currentX + SIZE * (_currentY + y)).GetComponent<Button>().enabled = true;
                    gridFloors.GetChild(_currentX + SIZE * (_currentY + y)).GetChild(0).GetComponent<Image>().enabled = true;
                }
            }
        }

        // COPY
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