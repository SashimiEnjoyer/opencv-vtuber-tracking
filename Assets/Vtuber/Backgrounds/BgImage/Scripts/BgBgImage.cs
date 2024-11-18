using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VTuber {
	public class BgBgImage : MonoBehaviour, Background {
		public void ChangeSkybox() {
			RenderSettings.skybox = null;
			GameObject bgImageCanvas = GameObject.Find("/BgImageCanvas");
			if (bgImageCanvas != null) {
				bgImageCanvas.GetComponent<BgImage>().Activate();
			}
		}

		public Color GetBgColor() {
			return Color.white;
		}
	}
}
