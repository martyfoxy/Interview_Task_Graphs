using TMPro;

namespace Game
{
    public class ResourcesManager
    {
        public float Resources { get; private set; }
        
        private readonly TMP_Text _resourcesCountText;

        public ResourcesManager(TMP_Text resourcesCountText)
        {
            _resourcesCountText = resourcesCountText;
        }

        public void AddScore(float score)
        {
            Resources += score;
            _resourcesCountText.text = Resources.ToString("F2");
        }
    }
}