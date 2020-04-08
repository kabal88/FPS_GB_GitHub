using UnityEngine;
using UnityEngine.UI;

namespace Geekbrains
{
	public sealed class FlashLightUiBar : MonoBehaviour
	{
		private Image _bar;

		private void Awake()
		{
			_bar = GetComponent<Image>();
		}

		public float Fill
		{
			set => _bar.fillAmount = value;
		}

		public void SetActive(bool value)
		{
			_bar.gameObject.SetActive(value);
		}
		
		public void SetColor(Color col)
		{
			_bar.color = col;
		}
	}
}