using System;
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

        private double _totalEnergy;
        private double _timeStart;

        private bool _update;

        public void Setup(in LevelData data)
        {
            _totalEnergy = data.totalEnergy;
            _timeStart = Time.timeAsDouble;
            _update = true;
        }

        private void Update()
        {
            if (_update && Time.frameCount % 20 == 0)
            {
                var e = Math.Max(_totalEnergy - (Time.timeAsDouble - _timeStart), 0.0) / _totalEnergy;
                fill.fillAmount = (float)e;
                leftTmp.text = $"{Mathf.FloorToInt(100 * fill.fillAmount)}<sprite=0>";
            }
        }

        public void Teardown()
        {
            _update = false;
        }
    }
}