using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class EnergyView : MonoBehaviour
    {
        [SerializeField]
        private Image fill;
        [SerializeField]
        private TMP_Text leftTmp;
        [SerializeField]
        private TutorView tutorView;

        private LevelData _data;

        private bool _updating;
        private double _timeLastDec;

        public void Setup(in LevelData data)
        {
            _data = data;
            _timeLastDec = Time.timeAsDouble;

            SetAmount((float)_data.currentEnergy / _data.totalEnergy);
            _updating = true;
        }

        private void Update()
        {
            if (tutorView.open)
                return;
            if (_updating && Time.frameCount % 20 == 0)
            {
                var time = Time.timeAsDouble;
                if (time - _timeLastDec >= 1.0)
                {
                    _timeLastDec = time;
                    _data.currentEnergy--;
                }

                var curr = (float)_data.currentEnergy / _data.totalEnergy;
                var toAdd = Mathf.Sign(curr - fill.fillAmount) * 2f * Time.deltaTime;

                // linear speed can fly over
                if (Mathf.Sign(curr - fill.fillAmount) != Mathf.Sign(curr - (fill.fillAmount + toAdd)))
                    toAdd = curr - fill.fillAmount;

                SetAmount(fill.fillAmount + toAdd);
            }
        }

        private void SetAmount(float value)
        {
            fill.fillAmount = value;
            leftTmp.text = $"{Mathf.RoundToInt(_data.totalEnergy * fill.fillAmount)}<sprite=0>";
        }

        public void SubtractEnergy(int energy) => _data.currentEnergy -= energy;

        public void Teardown() => _updating = false;
    }
}