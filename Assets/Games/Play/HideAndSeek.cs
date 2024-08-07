using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private Transform container;
        
        [Tooltip("The container where the list of items to match will spawn.")]
        [SerializeField] private Transform startPoition;

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

            SelectGlyphs();
            //gameInputs[i].Setup(_toMatch, _toMatch.Symbol);

            //StartCoroutine(InitialAnimation(settings.AnimationSpeed));
        }

        
        public override void NextRound()
        {
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

        private IEnumerator InitialAnimation(float speedFactor)
        {
            Vector2 goal = startPoition.position;

            for (int i = 0; i < _rounds; i++)
            {
                goal.x = _gameInputs[i].transform.position.x;
                int sum = (int)(goal.y + _gameInputs[i].transform.position.y);
                StartCoroutine(AnimateFall(_gameInputs[i].GetComponent<RectTransform>(), goal, sum, 1f / speedFactor / i));
            }
            yield return new WaitForSeconds(1f / speedFactor);
        }

        private IEnumerator AnimateFall(RectTransform glass, Vector2 goal, int steps, float speed)
        {
            int current = 0;
            float ratio;
            while (current < steps)
            {
                ratio = (float)current / steps;
                Vector2.Lerp(glass.position, goal, ratio);
                yield return null;
                current++;
            }
                yield return new WaitForSeconds(speed);
        }

        #endregion
    }
}