using UnityEngine;

namespace Geekbrains
{
    public sealed class FlagBot : BaseObjectScene
    {

        private Affiliation _affiliation;

        [SerializeField] private Color _sideOneColor = Color.red;
        [SerializeField] private Color _sideTwoColor = Color.blue;



        public Affiliation AffiliationSide
        {
            get
            {
                return _affiliation;
            }
            set
            {
                _affiliation = value;
                ColorizeSide(_affiliation);
            }

        }

        private void ColorizeSide(Affiliation side)
        {
            switch (side)
            {
                case Affiliation.SideOne:
                    Color = _sideOneColor;
                    break;
                case Affiliation.SideTwo:
                    Color = _sideTwoColor;
                    break;
                case Affiliation.None:
                default:
                    Color = Color.grey;
                    break;
            }
        }
    }

}
