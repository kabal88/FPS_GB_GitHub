using System;
using UnityEngine;
using static UnityEngine.Random;

namespace Geekbrains
{
    public sealed class FlashLightModel : BaseObjectScene
    {
        [SerializeField] private float _speed = 11;
        [SerializeField] private float _batteryChargeMax;
        [SerializeField] private float Intensity = 1.5f;
        private Light _light;
        private Transform _goFollow;
        private Vector3 _vecOffset;
        private float _share;
        private float _takeAwayTheIntensity;

        public float Charge => BatteryChargeCurrent / _batteryChargeMax;
        public float BatteryChargeCurrent { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _light = GetComponent<Light>();
            _goFollow = Camera.main.transform;
            _vecOffset = transform.position - _goFollow.position;
            BatteryChargeCurrent = _batteryChargeMax;
            _light.intensity = Intensity;
            _share = _batteryChargeMax / 4;
            _takeAwayTheIntensity = Intensity / (_batteryChargeMax * 100);
        }

        public void Switch(FlashLightActiveType value)
        {
            switch (value)
            {
                case FlashLightActiveType.On:
                    _light.enabled = true;
                    transform.position = _goFollow.position + _vecOffset;
                    transform.rotation = _goFollow.rotation;
                    break;
                case FlashLightActiveType.None:
                    break;
                case FlashLightActiveType.Off:
                    _light.enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public void Rotation()
        {
            transform.position = _goFollow.position + _vecOffset;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                _goFollow.rotation, _speed * Time.deltaTime);
        }
        
        public bool EditBatteryCharge()
        {
            if (BatteryChargeCurrent > 0)
            {
                BatteryChargeCurrent -= Time.deltaTime;

                if (BatteryChargeCurrent < _share)
                {
                    _light.enabled = Range(0, 100) >= Range(0, 10);
                }
                else
                {
                    _light.intensity -= _takeAwayTheIntensity;
                }
                return true;
            }

            return false;
        }

        public bool LowBattery()
        {
            return BatteryChargeCurrent <= _batteryChargeMax / 2;
        }

        public bool BatteryRecharge()
        {
            if (BatteryChargeCurrent < _batteryChargeMax)
            {
                BatteryChargeCurrent += Time.deltaTime;
                return true;
            }
            return false;
        }
    }
}
