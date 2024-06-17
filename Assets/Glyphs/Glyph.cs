using UnityEngine;

namespace GlyphaeScripts
{
    [CreateAssetMenu(fileName = "Glyph", menuName = "ScriptableObjects/Glyph")]
    public class Glyph : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField] private Sprite symbol;
        [SerializeField] private Sprite character;
        [SerializeField] private AudioClip sound;

        #endregion


        #region Fields



        #endregion


        #region GetSets / Properties

        public Sprite Symbol { get => symbol; }
        public Sprite Character { get => character; }
        public AudioClip Sound { get => sound; }

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            
        }

        void Start()
        {
            
        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            
        }

        #endregion


        #region Events

        #endregion


        #region Methods
        
        

        public void TemplateMethod(bool param)
        {
            
        }

        #endregion


        #region Helpers
        
        

        private void TemplateHelper(bool param)
        {
            
        }

        #endregion
    }
}