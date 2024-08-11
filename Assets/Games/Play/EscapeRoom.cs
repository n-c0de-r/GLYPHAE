using System.Collections.Generic;
using System.Drawing;
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

        [SerializeField] private Transform gridWalls;

        [SerializeField] private Transform gridIcons;

        [SerializeField] private Transform gridPositions;

        #endregion


        #region Fields

        private List<Sprite> previousSprites;
        private Sprite _previousSprite;
        int[,] _mazeData;
        private const int SIZE = 5;
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
            if (_usedGlyphs.Count < 4) CloseGame();

            _mazeData = mazeGenerator.GenerateMaze();
            ResetMaze();
            ShowNextIcons();
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
            // Set Exits
            gridWalls.GetChild(0 + SIZE * 2).GetComponent<Image>().sprite = wallSprites[_mazeData[0, 2] + 8];
            gridWalls.GetChild(4 + SIZE * 2).GetComponent<Image>().sprite = wallSprites[_mazeData[4, 2] + 2];
            
            // Set pet in the midpoint
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
                    gridIcons.GetChild((_currentX + x) + SIZE * (_currentY)).GetComponent<Image>().enabled = true;
                    gridIcons.GetChild((_currentX) + SIZE * (_currentY + y)).GetComponent<Image>().enabled = true;
                }
            }
        }

        #endregion
    }
}